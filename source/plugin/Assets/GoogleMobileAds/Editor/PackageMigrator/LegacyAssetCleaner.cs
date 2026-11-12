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
using UnityEditor;
using UnityEngine;

namespace GoogleMobileAds.Editor
{
    /// <summary>
    /// Safely deletes legacy .unitypackage assets during migration to OpenUPM,
    /// strictly preserving publisher settings and configs under Assets/GoogleMobileAds/Resources/.
    /// </summary>
    public static class LegacyAssetCleaner
    {
        private const string LogPrefix = "[Google Mobile Ads] ";

        // Assets that must NEVER be deleted under any circumstances
        public const string PreservedResourcesPath = "Assets/GoogleMobileAds/Resources";

        // Folder that holds mediation adapters imported as .unitypackage files
        public const string MediationFolderPath = "Assets/GoogleMobileAds/Mediation";

        // Legacy platform binaries in Assets/Plugins/
        public static readonly string[] LegacyPluginPaths = new[]
        {
            "Assets/Plugins/Android/googlemobileads-unity.aar",
            "Assets/Plugins/Android/GoogleMobileAdsPlugin.androidlib",
            "Assets/Plugins/Android/gson.jar",
            "Assets/Plugins/iOS/GADUAdNetworkExtras.h",
            "Assets/Plugins/iOS/NativeTemplates",
            "Assets/Plugins/iOS/unity-plugin-library.xcframework",
            "Assets/Plugins/iOS/unity-plugin-library.a",
            "Assets/Plugins/Google.MiniJson.dll"
        };

        /// <summary>
        /// Compiles a complete list of asset paths to delete.
        /// </summary>
        /// <param name="projectRoot">Project root; defaults to the open Unity project.</param>
        /// <param name="additionalPreservedPaths">
        /// Extra project-relative paths (files or folders) to keep, such as mediation adapter
        /// folders that cannot be migrated to packages.
        /// </param>
        public static List<string> GetAssetsToDelete(
            string projectRoot = null, IEnumerable<string> additionalPreservedPaths = null)
        {
            var assetsToDelete = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(projectRoot))
            {
                projectRoot = GetProjectRootPath();
            }

            List<string> extraPreserved = NormalizePaths(additionalPreservedPaths);
            string assetsDir = Path.Combine(projectRoot, "Assets");

            // 1. Files enumerated in manifest.txt
            string gmaDir = Path.Combine(assetsDir, "GoogleMobileAds");
            if (Directory.Exists(gmaDir))
            {
                string[] manifests = Directory.GetFiles(
                    gmaDir, "GoogleMobileAds_version-*_manifest.txt");
                foreach (string manifestPath in manifests)
                {
                    try
                    {
                        string[] lines = File.ReadAllLines(manifestPath);
                        foreach (string rawLine in lines)
                        {
                            string line = rawLine.Trim().Replace('\\', '/');
                            if (string.IsNullOrEmpty(line) || IsPreservedPath(line))
                            {
                                continue;
                            }

                            if (string.Equals(line, "Assets/package.json", StringComparison.OrdinalIgnoreCase))
                            {
                                if (IsGmaPackageJson(Path.Combine(projectRoot, line)))
                                {
                                    assetsToDelete.Add(line);
                                }
                                continue;
                            }

                            assetsToDelete.Add(line);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning(LogPrefix + $"Failed to read manifest {manifestPath}: {ex.Message}");
                    }

                    // Add the manifest file itself
                    string relManifest = ToProjectRelativePath(manifestPath, projectRoot);
                    if (!string.IsNullOrEmpty(relManifest))
                    {
                        assetsToDelete.Add(relManifest);
                    }
                }
            }

            // 2. Migrated mediation folders
            string mediationDir = Path.Combine(gmaDir, "Mediation");
            if (Directory.Exists(mediationDir))
            {
                string[] adapterDirs = Directory.GetDirectories(mediationDir);
                foreach (string dir in adapterDirs)
                {
                    string relDir = ToProjectRelativePath(dir, projectRoot);
                    if (!string.IsNullOrEmpty(relDir))
                    {
                        assetsToDelete.Add(relDir);
                    }
                }
            }

            // 3. Legacy Plugins files
            foreach (string pluginPath in LegacyPluginPaths)
            {
                if (DoesProjectAssetExist(pluginPath, projectRoot))
                {
                    assetsToDelete.Add(pluginPath);
                }
            }

            // 4. Remaining loose files in Assets/GoogleMobileAds (except Resources/)
            if (Directory.Exists(gmaDir))
            {
                string[] allGmaFiles = Directory.GetFiles(gmaDir, "*", SearchOption.AllDirectories);
                foreach (string file in allGmaFiles)
                {
                    string relFile = ToProjectRelativePath(file, projectRoot);
                    if (!string.IsNullOrEmpty(relFile) &&
                        !relFile.EndsWith(".meta", StringComparison.OrdinalIgnoreCase) &&
                        !IsPreservedPath(relFile))
                    {
                        assetsToDelete.Add(relFile);
                    }
                }
            }

            if (extraPreserved.Count > 0)
            {
                // Deleting a preserved path, anything inside it, or a folder containing it
                // would remove assets the caller asked to keep.
                assetsToDelete.RemoveWhere(path => OverlapsAny(path, extraPreserved));
            }

            var result = new List<string>(assetsToDelete);
            // Sort longest paths first so nested files are processed before directories
            result.Sort((a, b) => b.Length.CompareTo(a.Length));
            return result;
        }

        /// <summary>
        /// Executes the safe asset deletion.
        /// </summary>
        /// <param name="projectRoot">Project root; defaults to the open Unity project.</param>
        /// <param name="additionalPreservedPaths">
        /// Extra project-relative paths (files or folders) to keep.
        /// </param>
        public static void ExecuteCleanup(
            string projectRoot = null, IEnumerable<string> additionalPreservedPaths = null)
        {
            if (string.IsNullOrEmpty(projectRoot))
            {
                projectRoot = GetProjectRootPath();
            }

            List<string> assetsToDelete = GetAssetsToDelete(projectRoot, additionalPreservedPaths);
            bool anyDeleted = false;

            foreach (string assetPath in assetsToDelete)
            {
                if (IsPreservedPath(assetPath))
                {
                    Debug.Log(LogPrefix + $"Preserving critical asset: {assetPath}");
                    continue;
                }

                if (DeleteAsset(assetPath, projectRoot))
                {
                    anyDeleted = true;
                    Debug.Log(LogPrefix + $"Removed legacy asset: {assetPath}");
                }
            }

            // Clean up empty directories under Assets/GoogleMobileAds (keeping Resources/) and empty Assets/Plugins
            CleanEmptyDirectories(Path.Combine(projectRoot, "Assets", "GoogleMobileAds"));
            CleanEmptyDirectories(Path.Combine(projectRoot, "Assets", "Plugins"));

            if (anyDeleted)
            {
                try
                {
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                }
                catch
                {
                    // Ignored in headless test runners where native AssetDatabase is not bound.
                }
            }
        }

        /// <summary>
        /// Deletes only the given mediation adapter folders (project-relative), then prunes empty
        /// folders under Assets/GoogleMobileAds/Mediation. Used when Google Mobile Ads is already
        /// installed as a package and only newly imported adapters need to be migrated.
        /// </summary>
        public static void DeleteAdapterFolders(string projectRoot, IEnumerable<string> adapterFolders)
        {
            if (string.IsNullOrEmpty(projectRoot))
            {
                projectRoot = GetProjectRootPath();
            }

            bool anyDeleted = false;
            foreach (string folder in NormalizePaths(adapterFolders))
            {
                if (!folder.StartsWith(MediationFolderPath + "/", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.LogWarning(LogPrefix + $"Skipping non-adapter path: {folder}");
                    continue;
                }

                if (DeleteAsset(folder, projectRoot))
                {
                    anyDeleted = true;
                    Debug.Log(LogPrefix + $"Removed legacy adapter folder: {folder}");
                }
            }

            CleanEmptyDirectories(Path.Combine(
                projectRoot, MediationFolderPath.Replace('/', Path.DirectorySeparatorChar)));

            if (anyDeleted)
            {
                try
                {
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                }
                catch
                {
                    // Ignored in headless test runners where native AssetDatabase is not bound.
                }
            }
        }

        /// <summary>
        /// Checks whether an asset path is strictly preserved.
        /// </summary>
        public static bool IsPreservedPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return false;
            }

            string normalized = assetPath.Replace('\\', '/').TrimEnd('/');
            if (normalized.Equals(PreservedResourcesPath, StringComparison.OrdinalIgnoreCase) ||
                normalized.Equals(PreservedResourcesPath + ".meta", StringComparison.OrdinalIgnoreCase) ||
                normalized.EndsWith("/" + PreservedResourcesPath, StringComparison.OrdinalIgnoreCase) ||
                normalized.EndsWith("/" + PreservedResourcesPath + ".meta", StringComparison.OrdinalIgnoreCase) ||
                normalized.StartsWith(PreservedResourcesPath + "/", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains("/" + PreservedResourcesPath + "/"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Deletes an asset via AssetDatabase, falling back to File/Directory Delete on disk.
        /// </summary>
        public static bool DeleteAsset(string assetPath, string projectRoot = null)
        {
            if (IsPreservedPath(assetPath))
            {
                return false;
            }

            try
            {
                if (AssetDatabase.DeleteAsset(assetPath))
                {
                    return true;
                }
            }
            catch
            {
                // Fallback to disk deletion when native AssetDatabase is not bound.
            }

            // Fallback to disk deletion
            if (string.IsNullOrEmpty(projectRoot))
            {
                projectRoot = GetProjectRootPath();
            }

            string fullPath = Path.Combine(
                projectRoot, assetPath.Replace('/', Path.DirectorySeparatorChar));

            bool deleted = false;
            try
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    deleted = true;
                }
                else if (Directory.Exists(fullPath))
                {
                    Directory.Delete(fullPath, true);
                    deleted = true;
                }

                string metaPath = fullPath + ".meta";
                if (File.Exists(metaPath))
                {
                    File.Delete(metaPath);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(LogPrefix + $"Failed to delete {fullPath}: {ex.Message}");
            }

            return deleted;
        }

        private static List<string> NormalizePaths(IEnumerable<string> paths)
        {
            var normalized = new List<string>();
            if (paths == null)
            {
                return normalized;
            }

            foreach (string path in paths)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    normalized.Add(path.Replace('\\', '/').TrimEnd('/'));
                }
            }
            return normalized;
        }

        private static bool OverlapsAny(string assetPath, List<string> preservedPaths)
        {
            string normalized = assetPath.Replace('\\', '/').TrimEnd('/');
            foreach (string preserved in preservedPaths)
            {
                if (normalized.Equals(preserved, StringComparison.OrdinalIgnoreCase) ||
                    normalized.Equals(preserved + ".meta", StringComparison.OrdinalIgnoreCase) ||
                    normalized.StartsWith(preserved + "/", StringComparison.OrdinalIgnoreCase) ||
                    preserved.StartsWith(normalized + "/", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool DoesProjectAssetExist(string assetPath, string projectRoot)
        {
            string fullPath = Path.Combine(
                projectRoot, assetPath.Replace('/', Path.DirectorySeparatorChar));
            return File.Exists(fullPath) || Directory.Exists(fullPath);
        }

        private static bool IsGmaPackageJson(string fullPath)
        {
            if (!File.Exists(fullPath))
            {
                return false;
            }

            try
            {
                string json = File.ReadAllText(fullPath);
                return json.Contains("\"name\"") && json.Contains("\"com.google.ads.mobile\"");
            }
            catch
            {
                return false;
            }
        }

        internal static string ToProjectRelativePath(string fullPath, string projectRoot)
        {
            string normFull = Path.GetFullPath(fullPath);
            string normRoot = Path.GetFullPath(projectRoot).TrimEnd(
                Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (string.Equals(normFull, normRoot, StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }
            if (normFull.Length > normRoot.Length &&
                normFull.StartsWith(normRoot, StringComparison.OrdinalIgnoreCase) &&
                (normFull[normRoot.Length] == Path.DirectorySeparatorChar ||
                 normFull[normRoot.Length] == Path.AltDirectorySeparatorChar))
            {
                string rel = normFull.Substring(normRoot.Length + 1);
                return rel.Replace('\\', '/');
            }
            return null;
        }

        private static void CleanEmptyDirectories(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            foreach (string subDir in Directory.GetDirectories(directoryPath))
            {
                CleanEmptyDirectories(subDir);
            }

            if (IsPreservedPath(directoryPath))
            {
                return;
            }

            try
            {
                string[] files = Directory.GetFiles(directoryPath);
                string[] dirs = Directory.GetDirectories(directoryPath);
                if (files.Length == 0 && dirs.Length == 0)
                {
                    Directory.Delete(directoryPath, false);
                    string meta = directoryPath + ".meta";
                    if (File.Exists(meta))
                    {
                        File.Delete(meta);
                    }
                }
            }
            catch
            {
                // Ignored
            }
        }

        private static string GetProjectRootPath()
        {
            string dataPath = Application.dataPath;
            DirectoryInfo parent = Directory.GetParent(dataPath);
            return parent != null ? parent.FullName : Directory.GetCurrentDirectory();
        }
    }
}
