// Copyright (C) 2026 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace GoogleMobileAds.Editor
{
    /// <summary>
    /// Information extracted from an installed mediation adapter's Dependencies.xml.
    /// </summary>
    public class InstalledMediationAdapter
    {
        public string AdapterId;
        public string AdapterName;
        public string AndroidVersion;
        public string IosVersion;

        /// <summary>
        /// True when IosVersion is the adapter's Swift Package Manager version (taken from
        /// remoteSwiftPackages) rather than its CocoaPods version.
        /// </summary>
        public bool UsesSwiftPackageManager;

        /// <summary>
        /// Name of the adapter's top-level folder under Assets/GoogleMobileAds/Mediation/.
        /// </summary>
        public string FolderName;

        public string VersionKey => $"{AndroidVersion}/{IosVersion}";
    }

    /// <summary>
    /// Where a resolved mediation package version came from.
    /// </summary>
    public enum VersionSource
    {
        /// <summary>Exact match for the installed adapter in MediationVersionMap.json.</summary>
        Mapped,
        /// <summary>Latest version published on OpenUPM.</summary>
        OpenUpm,
        /// <summary>Highest version in MediationVersionMap.json, used when OpenUPM is unreachable.</summary>
        BundledFallback
    }

    /// <summary>
    /// Mapping info for an OpenUPM mediation package.
    /// </summary>
    public class MediationPackageInfo
    {
        public string PackageName;
        public Dictionary<string, string> Versions = new Dictionary<string, string>();
    }

    /// <summary>
    /// Result of mediation resolution across installed adapters.
    /// </summary>
    public class MediationResolutionResult
    {
        /// <summary>
        /// Resolved UPM package dependencies: packageName -> unityPackageVersion.
        /// </summary>
        public Dictionary<string, string> ResolvedPackages = new Dictionary<string, string>();

        /// <summary>
        /// Adapters whose (Android/iOS) versions were not found in the version mapping.
        /// </summary>
        public List<InstalledMediationAdapter> UnmappedAdapters =
            new List<InstalledMediationAdapter>();

        /// <summary>
        /// All installed adapters discovered in the project.
        /// </summary>
        public List<InstalledMediationAdapter> InstalledAdapters =
            new List<InstalledMediationAdapter>();
    }

    /// <summary>
    /// Resolves installed mediation adapters to their corresponding OpenUPM package versions
    /// using MediationVersionMap.json.
    /// </summary>
    public class MediationVersionResolver
    {
        private const string ManifestResourceName =
            "GoogleMobileAds.Editor.MediationVersionMap.json";
        private const string MappingFileName = "MediationVersionMap.json";

        private static readonly Regex AdapterBlockRegex = new Regex(
            @"\""(?<adapter>[a-zA-Z0-9_-]+)\""\s*:\s*\{\s*\""packageName\""\s*:\s*\""(?<pkg>[^\""]+)\""\s*,\s*\""versions\""\s*:\s*\{(?<verContent>[^\}]+)\}",
            RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex VersionPairRegex = new Regex(
            @"\""(?<key>[^\""]+)""\s*:\s*\""(?<val>[^\""]+)""",
            RegexOptions.Compiled);

        private static readonly Regex ManifestVersionRegex = new Regex(
            @"GoogleMobileAds_version-([0-9a-zA-Z._-]+)_manifest\.txt",
            RegexOptions.Compiled);

        private const string OpenUpmPackageUrlFormat = "https://package.openupm.com/{0}";
        private const string NpmAbbreviatedMetadataAccept = "application/vnd.npm.install-v1+json";
        internal const int OpenUpmRequestTimeoutMs = 5000;

        private static readonly Regex LatestDistTagRegex = new Regex(
            @"""dist-tags""\s*:\s*\{[^}]*?""latest""\s*:\s*""(?<ver>[^""]+)""",
            RegexOptions.Compiled);

        // UPM only accepts SemVer strings in manifest.json, so reject anything else from the registry.
        private static readonly Regex SemverRegex = new Regex(
            @"^\d+\.\d+\.\d+(-[0-9A-Za-z.-]+)?(\+[0-9A-Za-z.-]+)?$",
            RegexOptions.Compiled);

        private readonly Dictionary<string, MediationPackageInfo> _adapterMap;

        /// <summary>
        /// Looks up the latest published version of a package on OpenUPM. Returns null on failure.
        /// Replaceable for testing.
        /// </summary>
        public Func<string, string> OpenUpmLatestVersionFetcher { get; set; } =
            FetchLatestVersionFromOpenUpm;

        /// <summary>
        /// True when the most recent OpenUPM lookup in <see cref="ResolveLatestVersion"/>
        /// failed due to a network error, timeout, or transient server error (rather than
        /// HTTP 404 or missing metadata).
        /// </summary>
        public bool LastOpenUpmLookupUnreachable { get; private set; }

        public MediationVersionResolver(string customJson = null)
        {
            if (!string.IsNullOrEmpty(customJson))
            {
                _adapterMap = ParseVersionMapJson(customJson);
            }
            else
            {
                _adapterMap = LoadVersionMap();
            }
        }

        /// <summary>
        /// Resolves installed mediation adapters located under `Assets/GoogleMobileAds/Mediation/`.
        /// </summary>
        public MediationResolutionResult ResolveInstalledAdapters(string assetsDirectory = null)
        {
            var result = new MediationResolutionResult();
            List<InstalledMediationAdapter> installed = ScanInstalledAdapters(assetsDirectory);
            result.InstalledAdapters = installed;

            foreach (var adapter in installed)
            {
                if (string.IsNullOrEmpty(adapter.AdapterId))
                {
                    result.UnmappedAdapters.Add(adapter);
                    continue;
                }

                if (_adapterMap.TryGetValue(adapter.AdapterId, out MediationPackageInfo pkgInfo) &&
                    pkgInfo.Versions.TryGetValue(adapter.VersionKey, out string unityVer))
                {
                    result.ResolvedPackages[pkgInfo.PackageName] = unityVer;
                }
                else
                {
                    result.UnmappedAdapters.Add(adapter);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the OpenUPM package name for a given adapter ID, or null if not found.
        /// </summary>
        public string GetPackageName(string adapterId)
        {
            if (!string.IsNullOrEmpty(adapterId) &&
                _adapterMap.TryGetValue(adapterId, out MediationPackageInfo pkgInfo))
            {
                return pkgInfo.PackageName;
            }
            return null;
        }

        /// <summary>
        /// Gets the highest known OpenUPM package version for a given adapter ID, or null if not found.
        /// </summary>
        public string GetLatestVersion(string adapterId)
        {
            if (string.IsNullOrEmpty(adapterId) ||
                !_adapterMap.TryGetValue(adapterId, out MediationPackageInfo pkgInfo) ||
                pkgInfo.Versions == null || pkgInfo.Versions.Count == 0)
            {
                return null;
            }

            string highestVersion = null;
            int[] highestParts = null;
            foreach (string ver in pkgInfo.Versions.Values)
            {
                int[] parts = ParseSemverParts(ver);
                if (highestParts == null || CompareSemver(parts, highestParts) > 0)
                {
                    highestParts = parts;
                    highestVersion = ver;
                }
            }
            return highestVersion;
        }

        /// <summary>
        /// Resolves a version for an adapter whose installed version is not in the mapping.
        /// Prefers the latest version published on OpenUPM, and falls back to the highest version
        /// in MediationVersionMap.json if OpenUPM cannot be reached. Returns null if the adapter
        /// is not in the mapping at all (no known package name) or no version can be resolved.
        /// </summary>
        public string ResolveLatestVersion(string adapterId, out VersionSource source)
        {
            source = VersionSource.BundledFallback;
            LastOpenUpmLookupUnreachable = false;
            string packageName = GetPackageName(adapterId);
            if (string.IsNullOrEmpty(packageName))
            {
                return null;
            }

            string remoteVersion = null;
            if (OpenUpmLatestVersionFetcher != null)
            {
                if (OpenUpmLatestVersionFetcher ==
                    (Func<string, string>)FetchLatestVersionFromOpenUpm)
                {
                    bool unreachable;
                    remoteVersion = TryFetchLatestVersionFromOpenUpm(
                        packageName, out unreachable);
                    LastOpenUpmLookupUnreachable = unreachable;
                }
                else
                {
                    try
                    {
                        remoteVersion = OpenUpmLatestVersionFetcher(packageName);
                    }
                    catch (WebException ex)
                    {
                        LastOpenUpmLookupUnreachable = IsNetworkFailure(ex);
                        Debug.LogWarning(
                            $"[Google Mobile Ads] OpenUPM lookup failed for {packageName}: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        LastOpenUpmLookupUnreachable = true;
                        Debug.LogWarning(
                            $"[Google Mobile Ads] OpenUPM lookup failed for {packageName}: {ex.Message}");
                    }
                }
            }

            if (IsValidSemver(remoteVersion))
            {
                source = VersionSource.OpenUpm;
                return remoteVersion;
            }

            return GetLatestVersion(adapterId);
        }

        /// <summary>
        /// Queries the OpenUPM registry for the latest published version of a package.
        /// Returns null if the request fails, times out, or returns an unexpected response.
        /// </summary>
        public static string FetchLatestVersionFromOpenUpm(string packageName)
        {
            bool unused;
            return TryFetchLatestVersionFromOpenUpm(packageName, out unused);
        }

        private static string TryFetchLatestVersionFromOpenUpm(
            string packageName, out bool unreachable)
        {
            unreachable = false;
            if (string.IsNullOrEmpty(packageName))
            {
                return null;
            }

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(
                    string.Format(OpenUpmPackageUrlFormat, Uri.EscapeDataString(packageName)));
                request.Method = "GET";
                request.Accept = NpmAbbreviatedMetadataAccept;
                request.Timeout = OpenUpmRequestTimeoutMs;
                request.ReadWriteTimeout = OpenUpmRequestTimeoutMs;

                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string version = ParseLatestDistTag(reader.ReadToEnd());
                    return IsValidSemver(version) ? version : null;
                }
            }
            catch (WebException ex)
            {
                unreachable = IsNetworkFailure(ex);
                string detail = unreachable
                    ? $"Could not reach OpenUPM for {packageName}"
                    : $"OpenUPM lookup returned an error for {packageName}";
                Debug.LogWarning($"[Google Mobile Ads] {detail}: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                unreachable = true;
                Debug.LogWarning(
                    $"[Google Mobile Ads] Could not reach OpenUPM for {packageName}: {ex.Message}");
                return null;
            }
        }

        internal static bool IsNetworkFailure(WebException ex)
        {
            if (ex == null)
            {
                return false;
            }
            var httpResponse = ex.Response as HttpWebResponse;
            if (httpResponse != null)
            {
                int statusCode = (int)httpResponse.StatusCode;
                return statusCode >= 500 ||
                       httpResponse.StatusCode == HttpStatusCode.RequestTimeout ||
                       statusCode == 429;
            }
            // No HTTP response (DNS failure, connection failure, or timeout).
            return true;
        }

        /// <summary>
        /// Extracts dist-tags.latest from an npm registry package document.
        /// </summary>
        public static string ParseLatestDistTag(string registryJson)
        {
            if (string.IsNullOrEmpty(registryJson))
            {
                return null;
            }

            Match m = LatestDistTagRegex.Match(registryJson);
            return m.Success ? m.Groups["ver"].Value : null;
        }

        private static bool IsValidSemver(string version)
        {
            return !string.IsNullOrEmpty(version) && SemverRegex.IsMatch(version);
        }

        private static int[] ParseSemverParts(string versionStr)
        {
            if (string.IsNullOrEmpty(versionStr))
            {
                return new int[0];
            }
            string clean = versionStr.TrimStart('v');
            int dashIndex = clean.IndexOfAny(new[] { '-', '+' });
            if (dashIndex >= 0)
            {
                clean = clean.Substring(0, dashIndex);
            }
            string[] rawParts = clean.Split('.');
            int[] result = new int[rawParts.Length];
            for (int i = 0; i < rawParts.Length; i++)
            {
                int.TryParse(rawParts[i], out result[i]);
            }
            return result;
        }

        private static int CompareSemver(int[] a, int[] b)
        {
            int maxLen = Math.Max(a.Length, b.Length);
            for (int i = 0; i < maxLen; i++)
            {
                int valA = i < a.Length ? a[i] : 0;
                int valB = i < b.Length ? b[i] : 0;
                if (valA != valB)
                {
                    return valA.CompareTo(valB);
                }
            }
            return 0;
        }

        /// <summary>
        /// Scans the project's mediation directories and parses all Dependencies.xml files.
        /// </summary>
        public static List<InstalledMediationAdapter> ScanInstalledAdapters(
            string assetsDirectory = null)
        {
            var list = new List<InstalledMediationAdapter>();
            if (string.IsNullOrEmpty(assetsDirectory))
            {
                assetsDirectory = Application.dataPath;
            }

            string mediationBase = Path.Combine(assetsDirectory, "GoogleMobileAds", "Mediation");
            if (!Directory.Exists(mediationBase))
            {
                return list;
            }

            string[] files = Directory.GetFiles(
                mediationBase, "*Dependencies.xml", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                InstalledMediationAdapter adapter = ParseDependenciesXml(file);
                if (adapter != null)
                {
                    adapter.FolderName = GetTopLevelFolderName(mediationBase, file);
                    list.Add(adapter);
                }
            }

            return list;
        }

        private static string GetTopLevelFolderName(string baseDirectory, string filePath)
        {
            string fullBase = Path.GetFullPath(baseDirectory).TrimEnd(
                Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string fullFile = Path.GetFullPath(filePath);
            if (!fullFile.StartsWith(fullBase, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            string relative = fullFile.Substring(fullBase.Length).TrimStart(
                Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            int separator = relative.IndexOfAny(
                new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
            return separator > 0 ? relative.Substring(0, separator) : null;
        }

        /// <summary>
        /// Parses a Dependencies.xml file to extract adapter_id, Android version, and iOS version.
        /// </summary>
        public static InstalledMediationAdapter ParseDependenciesXml(string xmlFilePath)
        {
            if (!File.Exists(xmlFilePath))
            {
                return null;
            }

            try
            {
                string xmlContent = File.ReadAllText(xmlFilePath);
                return ParseDependenciesXmlContent(xmlContent, xmlFilePath);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[Google Mobile Ads] Error reading {xmlFilePath}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Parses XML content directly.
        /// </summary>
        public static InstalledMediationAdapter ParseDependenciesXmlContent(
            string xmlContent,
            string filePath = null)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xmlContent);

            string adapterId = null;
            string androidVersion = null;
            string iosVersion = null;
            string adapterName = null;

            if (!string.IsNullOrEmpty(filePath))
            {
                string dir = Path.GetDirectoryName(filePath);
                adapterName = Path.GetFileName(dir);
                if (string.Equals(adapterName, "Editor", StringComparison.OrdinalIgnoreCase))
                {
                    adapterName = Path.GetFileName(Path.GetDirectoryName(dir));
                }
            }

            XmlNodeList androidNodes = doc.SelectNodes("//androidPackage");
            if (androidNodes != null)
            {
                foreach (XmlNode node in androidNodes)
                {
                    string spec = node.Attributes?["spec"]?.Value;
                    if (!string.IsNullOrEmpty(spec) &&
                        spec.StartsWith("com.google.ads.mediation:"))
                    {
                        string[] parts = spec.Split(':');
                        if (parts.Length >= 3)
                        {
                            adapterId = parts[1];
                            androidVersion = parts[2];
                            break;
                        }
                    }
                }
            }

            XmlNodeList iosNodes = doc.SelectNodes("//iosPod");
            if (iosNodes != null)
            {
                foreach (XmlNode node in iosNodes)
                {
                    string name = node.Attributes?["name"]?.Value;
                    string version = node.Attributes?["version"]?.Value;
                    if (!string.IsNullOrEmpty(name) &&
                        name.StartsWith("GoogleMobileAdsMediation") &&
                        !string.IsNullOrEmpty(version))
                    {
                        iosVersion = version;
                        if (string.IsNullOrEmpty(adapterName))
                        {
                            adapterName = name.Replace("GoogleMobileAdsMediation", "");
                        }
                        break;
                    }
                }
            }

            // Swift Package Manager takes precedence over CocoaPods: when the adapter is declared
            // as a Swift package, its (3-part) SPM version is used for the mapping key and the
            // CocoaPods version is ignored. MediationVersionMap.json is generated the same way.
            string spmVersion = FindAdapterSwiftPackageVersion(doc);
            bool usesSpm = !string.IsNullOrEmpty(spmVersion);
            if (usesSpm)
            {
                iosVersion = spmVersion;
            }

            if (string.IsNullOrEmpty(adapterId) && string.IsNullOrEmpty(androidVersion) &&
                string.IsNullOrEmpty(iosVersion))
            {
                return null;
            }

            return new InstalledMediationAdapter
            {
                AdapterId = adapterId,
                AdapterName = adapterName ?? adapterId,
                AndroidVersion = androidVersion,
                IosVersion = iosVersion,
                UsesSwiftPackageManager = usesSpm
            };
        }

        /// <summary>
        /// Returns the version of the adapter's own Swift package (not the partner SDK's), or
        /// null if the adapter is not declared as a Swift package. The adapter package is the one
        /// that replaces the GoogleMobileAdsMediation* pod, or, failing that, the one hosted in a
        /// googleads-mobile-ios-mediation-* repository.
        /// </summary>
        private static string FindAdapterSwiftPackageVersion(XmlDocument doc)
        {
            XmlNodeList packages = doc.SelectNodes("//remoteSwiftPackage");
            if (packages == null || packages.Count == 0)
            {
                return null;
            }

            foreach (XmlNode package in packages)
            {
                XmlNodeList products = package.SelectNodes("swiftPackage");
                if (products == null)
                {
                    continue;
                }

                foreach (XmlNode product in products)
                {
                    string replacesPod = product.Attributes?["replacesPod"]?.Value;
                    if (!string.IsNullOrEmpty(replacesPod) &&
                        replacesPod.Split(',').Any(pod =>
                            pod.Trim().StartsWith("GoogleMobileAdsMediation", StringComparison.Ordinal)))
                    {
                        return package.Attributes?["version"]?.Value;
                    }
                }
            }

            foreach (XmlNode package in packages)
            {
                string url = package.Attributes?["url"]?.Value;
                if (!string.IsNullOrEmpty(url) &&
                    url.IndexOf("googleads-mobile-ios-mediation-", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return package.Attributes?["version"]?.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Detects the currently installed Core GMA plugin version from manifest.txt or assembly.
        /// </summary>
        public static string DetectInstalledGmaVersion(string assetsDirectory = null)
        {
            if (string.IsNullOrEmpty(assetsDirectory))
            {
                assetsDirectory = Application.dataPath;
            }

            string gmaDir = Path.Combine(assetsDirectory, "GoogleMobileAds");
            if (Directory.Exists(gmaDir))
            {
                string[] manifestFiles = Directory.GetFiles(
                    gmaDir, "GoogleMobileAds_version-*_manifest.txt");
                string highestVersion = null;
                int[] highestParts = null;
                foreach (string manifestFile in manifestFiles)
                {
                    string filename = Path.GetFileName(manifestFile);
                    Match m = ManifestVersionRegex.Match(filename);
                    if (!m.Success)
                    {
                        continue;
                    }

                    string candidate = m.Groups[1].Value;
                    int[] candidateParts = ParseSemverParts(candidate);
                    if (highestVersion == null || CompareSemver(candidateParts, highestParts) > 0)
                    {
                        highestVersion = candidate;
                        highestParts = candidateParts;
                    }
                }
                if (highestVersion != null)
                {
                    return highestVersion;
                }
            }

            // Fallback: Check loaded assemblies
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name == "GoogleMobileAds.Core")
                {
                    string formatted = FormatAssemblyVersion(assembly.GetName().Version);
                    if (formatted != null)
                    {
                        return formatted;
                    }
                }
            }

            return null;
        }

        internal static string FormatAssemblyVersion(Version ver)
        {
            if (ver == null || ver.Major <= 0)
            {
                return null;
            }

            int build = ver.Build >= 0 ? ver.Build : 0;
            return $"{ver.Major}.{ver.Minor}.{build}";
        }

        private static string GetEditorAssetDirectory()
        {
            try
            {
                var pathUtils = ScriptableObject.CreateInstance<EditorPathUtils>();
                try
                {
                    return pathUtils.GetDirectoryAssetPath();
                }
                finally
                {
                    UnityEngine.Object.DestroyImmediate(pathUtils);
                }
            }
            catch
            {
                return null;
            }
        }

        private static string FindFirstExistingFile(params string[] paths)
        {
            foreach (string path in paths)
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    return path;
                }
            }
            return null;
        }

        private static Dictionary<string, MediationPackageInfo> LoadVersionMap()
        {
            // 1. Try reading from embedded assembly resource
            Assembly assembly = typeof(MediationVersionResolver).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream(ManifestResourceName))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return ParseVersionMapJson(reader.ReadToEnd());
                    }
                }
            }

            // 2. Try loading via Unity's AssetDatabase / PackageManager (handles UPM Library/PackageCache).
            string editorDir = GetEditorAssetDirectory();
            string assetJson = LoadJsonFromAssetDatabase(editorDir);
            if (!string.IsNullOrEmpty(assetJson))
            {
                return ParseVersionMapJson(assetJson);
            }

            // 3. Fallback: Read directly from disk using standard paths
            string jsonPath = FindFirstExistingFile(
                !string.IsNullOrEmpty(editorDir)
                    ? Path.Combine(editorDir, "PackageMigrator", MappingFileName)
                    : null,
                !string.IsNullOrEmpty(editorDir)
                    ? Path.Combine(editorDir, MappingFileName)
                    : null,
                Path.Combine(Application.dataPath, "GoogleMobileAds", "Editor", "PackageMigrator", MappingFileName),
                Path.Combine(Application.dataPath, "GoogleMobileAds", "Editor", MappingFileName)
            );

            if (jsonPath != null)
            {
                return ParseVersionMapJson(File.ReadAllText(jsonPath));
            }

            Debug.LogWarning("[Google Mobile Ads] MediationVersionMap.json not found.");
            return new Dictionary<string, MediationPackageInfo>();
        }

        private static string LoadJsonFromAssetDatabase(string editorDir)
        {
            var candidates = new List<string>();
            if (!string.IsNullOrEmpty(editorDir))
            {
                string normalizedDir = editorDir.Replace('\\', '/').TrimEnd('/');
                candidates.Add($"{normalizedDir}/PackageMigrator/{MappingFileName}");
                candidates.Add($"{normalizedDir}/{MappingFileName}");
            }
            candidates.Add($"Assets/GoogleMobileAds/Editor/PackageMigrator/{MappingFileName}");
            candidates.Add($"Assets/GoogleMobileAds/Editor/{MappingFileName}");
            candidates.Add($"Packages/com.google.ads.mobile/Editor/PackageMigrator/{MappingFileName}");
            candidates.Add($"Packages/com.google.ads.mobile/Editor/{MappingFileName}");

            foreach (string assetPath in candidates.Distinct())
            {
                string content = TryReadTextAsset(assetPath);
                if (!string.IsNullOrEmpty(content))
                {
                    return content;
                }
            }

            return null;
        }

        private static string TryReadTextAsset(string assetPath)
        {
            try
            {
                var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                if (textAsset != null && !string.IsNullOrEmpty(textAsset.text))
                {
                    return textAsset.text;
                }
            }
            catch
            {
                // Ignore when running outside a full Unity Editor environment (e.g., headless tests).
            }

            try
            {
                UnityEditor.PackageManager.PackageInfo pkgInfo =
                    UnityEditor.PackageManager.PackageInfo.FindForAssetPath(assetPath);
                if (pkgInfo != null)
                {
                    string diskPath = ResolvePackageRelativeDiskPath(
                        assetPath, pkgInfo.assetPath, pkgInfo.resolvedPath);
                    if (!string.IsNullOrEmpty(diskPath) && File.Exists(diskPath))
                    {
                        return File.ReadAllText(diskPath);
                    }
                }
            }
            catch
            {
                // Ignore when PackageManager API is unavailable in headless unit tests.
            }

            return null;
        }

        internal static string ResolvePackageRelativeDiskPath(
            string assetPath, string packageAssetPath, string packageResolvedPath)
        {
            if (string.IsNullOrEmpty(assetPath) ||
                string.IsNullOrEmpty(packageAssetPath) ||
                string.IsNullOrEmpty(packageResolvedPath))
            {
                return null;
            }

            string normalizedAsset = assetPath.Replace('\\', '/');
            string normalizedPkgAsset = packageAssetPath.Replace('\\', '/').TrimEnd('/');
            if (!normalizedAsset.StartsWith(
                    normalizedPkgAsset + "/", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            string relativeInsidePackage = normalizedAsset.Substring(normalizedPkgAsset.Length + 1);
            return Path.Combine(
                packageResolvedPath,
                relativeInsidePackage.Replace('/', Path.DirectorySeparatorChar));
        }

        /// <summary>
        /// Lightweight JSON parser for MediationVersionMap.json to avoid external JSON dependencies.
        /// </summary>
        public static Dictionary<string, MediationPackageInfo> ParseVersionMapJson(string json)
        {
            var result = new Dictionary<string, MediationPackageInfo>();
            if (string.IsNullOrEmpty(json))
            {
                return result;
            }

            MatchCollection matches = AdapterBlockRegex.Matches(json);
            foreach (Match match in matches)
            {
                string adapterId = match.Groups["adapter"].Value;
                string pkgName = match.Groups["pkg"].Value;
                string verContent = match.Groups["verContent"].Value;

                var pkgInfo = new MediationPackageInfo
                {
                    PackageName = pkgName
                };

                MatchCollection verMatches = VersionPairRegex.Matches(verContent);
                foreach (Match vm in verMatches)
                {
                    pkgInfo.Versions[vm.Groups["key"].Value] = vm.Groups["val"].Value;
                }

                result[adapterId] = pkgInfo;
            }

            return result;
        }
    }
}
