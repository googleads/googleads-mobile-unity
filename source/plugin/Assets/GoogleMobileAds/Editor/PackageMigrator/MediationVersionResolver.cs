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
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
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
        public string FolderPath;
        public string DependenciesXmlPath;
        public string AndroidVersion;
        public string IosVersion;

        public string VersionKey => $"{AndroidVersion}/{IosVersion}";
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
        private const string CorePackageName = "com.google.ads.mobile";

        private static readonly Regex AdapterBlockRegex = new Regex(
            @"\""(?<adapter>[a-zA-Z0-9_-]+)\""\s*:\s*\{\s*\""packageName\""\s*:\s*\""(?<pkg>[^\""]+)\""\s*,\s*\""versions\""\s*:\s*\{(?<verContent>[^\}]+)\}",
            RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex VersionPairRegex = new Regex(
            @"\""(?<key>[^\""]+)""\s*:\s*\""(?<val>[^\""]+)""",
            RegexOptions.Compiled);

        private static readonly Regex ManifestVersionRegex = new Regex(
            @"GoogleMobileAds_version-([0-9a-zA-Z._-]+)_manifest\.txt",
            RegexOptions.Compiled);

        private readonly Dictionary<string, MediationPackageInfo> _adapterMap;

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
                    list.Add(adapter);
                }
            }

            return list;
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

            if (string.IsNullOrEmpty(adapterId) && string.IsNullOrEmpty(androidVersion) &&
                string.IsNullOrEmpty(iosVersion))
            {
                return null;
            }

            return new InstalledMediationAdapter
            {
                AdapterId = adapterId,
                AdapterName = adapterName ?? adapterId,
                FolderPath = !string.IsNullOrEmpty(filePath) ? Path.GetDirectoryName(filePath) : "",
                DependenciesXmlPath = filePath ?? "",
                AndroidVersion = androidVersion,
                IosVersion = iosVersion
            };
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
                if (manifestFiles.Length > 0)
                {
                    string filename = Path.GetFileName(manifestFiles[0]);
                    Match m = ManifestVersionRegex.Match(filename);
                    if (m.Success)
                    {
                        return m.Groups[1].Value;
                    }
                }
            }

            // Fallback: Check loaded assemblies
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name == "GoogleMobileAds.Core")
                {
                    Version ver = assembly.GetName().Version;
                    if (ver != null && ver.Major > 0)
                    {
                        return $"{ver.Major}.{ver.Minor}.{ver.Build}";
                    }
                }
            }

            return null;
        }

        private static string GetEditorAssetDirectory()
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

            // 2. Fallback: Read from disk using EditorPathUtils or standard paths
            string editorDir = GetEditorAssetDirectory();
            string jsonPath = FindFirstExistingFile(
                !string.IsNullOrEmpty(editorDir) ? Path.Combine(editorDir, MappingFileName) : null,
                Path.Combine(Application.dataPath, "GoogleMobileAds", "Editor", MappingFileName),
                Path.Combine(Application.dataPath, "GoogleMobileAds", "Editor", "PackageMigrator", MappingFileName)
            );

            if (jsonPath != null)
            {
                return ParseVersionMapJson(File.ReadAllText(jsonPath));
            }

            Debug.LogWarning("[Google Mobile Ads] MediationVersionMap.json not found.");
            return new Dictionary<string, MediationPackageInfo>();
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
