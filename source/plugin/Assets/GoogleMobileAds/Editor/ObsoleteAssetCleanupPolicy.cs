using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace GoogleMobileAds.Editor
{
    /// <summary>
    /// Determines which obsolete plugin assets should be removed from a Unity project after an
    /// in-place `.unitypackage` upgrade. Kept free of `UnityEditor` APIs so the rules can be unit
    /// tested directly.
    /// </summary>
    internal static class ObsoleteAssetCleanupPolicy
    {
        internal const string GmaIosXcframeworkPath =
            "Assets/Plugins/iOS/unity-plugin-library.xcframework";
        internal const string GmaIosStaticLibraryPath =
            "Assets/Plugins/iOS/unity-plugin-library.a";

        internal const string RaycastIosXcframeworkPath =
            "Assets/Plugins/iOS/googlemobileadsnative-plugin-library.xcframework";
        internal const string RaycastIosStaticLibraryPath =
            "Assets/Plugins/iOS/googlemobileadsnative-plugin-library.a";

        private const string AssetsPrefix = "Assets/";
        private const string LegacyEdmDirectory = "Assets/ExternalDependencyManager";
        private const string LegacyEdmPrefix = "Assets/ExternalDependencyManager/";

        private static readonly Regex ManifestFileNameRegex = new Regex(
            @"^(?<prefix>.+)_version-(?<version>[0-9]+(?:\.[0-9]+){1,3})_manifest\.txt$",
            RegexOptions.CultureInvariant);

        /// <summary>
        /// Represents a `*_manifest.txt` asset found in the project.
        /// </summary>
        internal struct ManifestEntry
        {
            // Project-relative asset path (for example,
            // `Assets/GoogleMobileAds/GoogleMobileAds_version-11.6.0_manifest.txt`).
            internal string AssetPath;
            // Full text contents of the manifest file.
            internal string Contents;
        }

        private struct ParsedManifest
        {
            internal ManifestEntry Entry;
            internal Version Version;
        }

        /// <summary>
        /// Parses a manifest asset path or filename of the form
        /// `<prefix>_version-<version>_manifest.txt`.
        /// </summary>
        internal static bool TryParseManifestFileName(
            string pathOrFileName, out string packagePrefix, out Version version)
        {
            packagePrefix = null;
            version = null;

            if (string.IsNullOrEmpty(pathOrFileName))
            {
                return false;
            }

            string normalized = pathOrFileName.Replace('\\', '/');
            string fileName = Path.GetFileName(normalized);
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }

            Match match = ManifestFileNameRegex.Match(fileName);
            if (!match.Success)
            {
                return false;
            }

            Version rawVersion;
            if (!Version.TryParse(match.Groups["version"].Value, out rawVersion))
            {
                return false;
            }

            packagePrefix = match.Groups["prefix"].Value;
            version = new Version(
                rawVersion.Major,
                rawVersion.Minor,
                rawVersion.Build >= 0 ? rawVersion.Build : 0,
                rawVersion.Revision >= 0 ? rawVersion.Revision : 0);
            return true;
        }

        /// <summary>
        /// Returns true if <paramref name="assetPath"/> is a safe, project-relative `Assets/...`
        /// path that the plugin is allowed to clean up. Paths outside `Assets/`, paths with `.` or
        /// `..` segments, and paths under `Assets/ExternalDependencyManager/` are rejected.
        /// </summary>
        internal static bool IsSafeDeletableAssetPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return false;
            }

            string normalized = assetPath.Trim();
            if (normalized.IndexOf('\\') >= 0 ||
                !normalized.StartsWith(AssetsPrefix, StringComparison.Ordinal))
            {
                return false;
            }

            string[] segments = normalized.Split('/');
            if (segments.Length < 2)
            {
                return false;
            }

            for (int i = 0; i < segments.Length; i++)
            {
                string segment = segments[i];
                if (string.IsNullOrEmpty(segment) || segment == "." || segment == "..")
                {
                    return false;
                }
            }

            // Never delete Google's legacy External Dependency Manager if present in the project.
            if (string.Equals(normalized, LegacyEdmDirectory, StringComparison.Ordinal) ||
                normalized.StartsWith(LegacyEdmPrefix, StringComparison.Ordinal))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the ordered list of obsolete project-relative `Assets/...` paths to delete,
        /// combining manifest diffing (older manifests vs. latest manifest per package prefix) with
        /// a fallback check for iOS `.a` static libraries that have been replaced by
        /// `.xcframework` bundles.
        /// </summary>
        internal static IList<string> GetAssetsToDelete(
            IEnumerable<ManifestEntry> manifests, Func<string, bool> assetExists)
        {
            var groupedByPrefix = new Dictionary<string, List<ParsedManifest>>(
                StringComparer.Ordinal);

            if (manifests != null)
            {
                foreach (ManifestEntry entry in manifests)
                {
                    string prefix;
                    Version version;
                    if (!TryParseManifestFileName(entry.AssetPath, out prefix, out version))
                    {
                        continue;
                    }

                    List<ParsedManifest> list;
                    if (!groupedByPrefix.TryGetValue(prefix, out list))
                    {
                        list = new List<ParsedManifest>();
                        groupedByPrefix[prefix] = list;
                    }

                    list.Add(new ParsedManifest
                    {
                        Entry = entry,
                        Version = version,
                    });
                }
            }

            var activeAssets = new HashSet<string>(StringComparer.Ordinal);
            var olderManifests = new List<ParsedManifest>();

            foreach (KeyValuePair<string, List<ParsedManifest>> pair in groupedByPrefix)
            {
                List<ParsedManifest> packageManifests = pair.Value;
                Version maxVersion = packageManifests[0].Version;
                for (int i = 1; i < packageManifests.Count; i++)
                {
                    if (packageManifests[i].Version > maxVersion)
                    {
                        maxVersion = packageManifests[i].Version;
                    }
                }

                for (int i = 0; i < packageManifests.Count; i++)
                {
                    ParsedManifest candidate = packageManifests[i];
                    if (candidate.Version == maxVersion)
                    {
                        if (!string.IsNullOrEmpty(candidate.Entry.AssetPath))
                        {
                            activeAssets.Add(candidate.Entry.AssetPath.Trim());
                        }
                        foreach (string path in ParseManifestLines(candidate.Entry.Contents))
                        {
                            activeAssets.Add(path);
                        }
                    }
                    else
                    {
                        olderManifests.Add(candidate);
                    }
                }
            }

            var toDelete = new List<string>();
            var seen = new HashSet<string>(StringComparer.Ordinal);

            for (int i = 0; i < olderManifests.Count; i++)
            {
                ManifestEntry oldManifest = olderManifests[i].Entry;
                foreach (string oldAsset in ParseManifestLines(oldManifest.Contents))
                {
                    if (!activeAssets.Contains(oldAsset) &&
                        IsSafeDeletableAssetPath(oldAsset) &&
                        (assetExists == null || assetExists(oldAsset)) &&
                        seen.Add(oldAsset))
                    {
                        toDelete.Add(oldAsset);
                    }
                }

                if (!string.IsNullOrEmpty(oldManifest.AssetPath))
                {
                    string oldManifestPath = oldManifest.AssetPath.Trim();
                    if (!activeAssets.Contains(oldManifestPath) &&
                        IsSafeDeletableAssetPath(oldManifestPath) &&
                        (assetExists == null || assetExists(oldManifestPath)) &&
                        seen.Add(oldManifestPath))
                    {
                        toDelete.Add(oldManifestPath);
                    }
                }
            }

            // Fallback for pre-manifest plugins (such as Raycast <= 11.1.0) or projects where an
            // older manifest file was removed manually: if the new .xcframework is present on disk,
            // the legacy .a static library is obsolete.
            AddReplacedStaticLibraryIfPresent(
                GmaIosXcframeworkPath, GmaIosStaticLibraryPath,
                activeAssets, assetExists, seen, toDelete);
            AddReplacedStaticLibraryIfPresent(
                RaycastIosXcframeworkPath, RaycastIosStaticLibraryPath,
                activeAssets, assetExists, seen, toDelete);

            return toDelete;
        }

        private static void AddReplacedStaticLibraryIfPresent(
            string xcframeworkPath,
            string staticLibraryPath,
            HashSet<string> activeAssets,
            Func<string, bool> assetExists,
            HashSet<string> seen,
            List<string> toDelete)
        {
            if (assetExists == null || activeAssets.Contains(staticLibraryPath))
            {
                return;
            }

            if (assetExists(xcframeworkPath) &&
                assetExists(staticLibraryPath) &&
                IsSafeDeletableAssetPath(staticLibraryPath) &&
                seen.Add(staticLibraryPath))
            {
                toDelete.Add(staticLibraryPath);
            }
        }

        private static IEnumerable<string> ParseManifestLines(string contents)
        {
            if (string.IsNullOrEmpty(contents))
            {
                yield break;
            }

            string[] lines = contents.Split(new[] { '\r', '\n' },
                                            StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                string trimmed = lines[i].Trim();
                if (!string.IsNullOrEmpty(trimmed))
                {
                    yield return trimmed;
                }
            }
        }
    }
}
