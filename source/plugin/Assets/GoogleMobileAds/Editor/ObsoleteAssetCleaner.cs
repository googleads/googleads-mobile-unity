using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GoogleMobileAds.Editor
{
    /// <summary>
    /// Cleans up obsolete plugin assets (such as replaced iOS `.a` static libraries and older
    /// `*_manifest.txt` files) when the Google Mobile Ads or Google Mobile Ads Native plugin is
    /// upgraded in-place via `.unitypackage`.
    /// </summary>
    internal static class ObsoleteAssetCleaner
    {
        private const string LogPrefix = "[Google Mobile Ads] ";
        private const int MaxRetryAttempts = 100;

        private static readonly string[] ManifestDirectories =
        {
            "Assets/GoogleMobileAds",
            "Assets/GoogleMobileAdsNative",
        };

        private static int _retryAttempts;

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            _retryAttempts = 0;
            EditorApplication.delayCall += CleanUpObsoleteAssets;
        }

        private static void CleanUpObsoleteAssets()
        {
            if (EdmDependencyInstaller.IsInstalledViaUpm())
            {
                return;
            }

            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                if (++_retryAttempts < MaxRetryAttempts)
                {
                    EditorApplication.delayCall += CleanUpObsoleteAssets;
                }
                return;
            }

            List<ObsoleteAssetCleanupPolicy.ManifestEntry> manifests = LoadManifestEntries();
            IList<string> assetsToDelete = ObsoleteAssetCleanupPolicy.GetAssetsToDelete(
                manifests, DoesProjectAssetExist);

            for (int i = 0; i < assetsToDelete.Count; i++)
            {
                string assetPath = assetsToDelete[i];
                if (AssetDatabase.DeleteAsset(assetPath))
                {
                    Debug.Log(LogPrefix + $"Removed obsolete asset: {assetPath}");
                }
            }
        }

        private static List<ObsoleteAssetCleanupPolicy.ManifestEntry> LoadManifestEntries()
        {
            var entries = new List<ObsoleteAssetCleanupPolicy.ManifestEntry>();
            string projectRoot = GetProjectRootPath();

            for (int i = 0; i < ManifestDirectories.Length; i++)
            {
                string relativeDir = ManifestDirectories[i];
                string fullDir = Path.Combine(
                    projectRoot, relativeDir.Replace('/', Path.DirectorySeparatorChar));
                if (!Directory.Exists(fullDir))
                {
                    continue;
                }

                try
                {
                    string[] files = Directory.GetFiles(fullDir, "*_manifest.txt",
                                                        SearchOption.TopDirectoryOnly);
                    for (int j = 0; j < files.Length; j++)
                    {
                        string filePath = files[j];
                        string fileName = Path.GetFileName(filePath);
                        string assetPath = relativeDir + "/" + fileName;
                        entries.Add(new ObsoleteAssetCleanupPolicy.ManifestEntry
                        {
                            AssetPath = assetPath,
                            Contents = File.ReadAllText(filePath),
                        });
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(
                        LogPrefix + $"Unable to inspect manifests in {relativeDir}: {ex.Message}");
                }
            }

            return entries;
        }

        private static bool DoesProjectAssetExist(string assetPath)
        {
            if (!ObsoleteAssetCleanupPolicy.IsSafeDeletableAssetPath(assetPath))
            {
                return false;
            }

            string fullPath = Path.Combine(
                GetProjectRootPath(), assetPath.Replace('/', Path.DirectorySeparatorChar));
            return File.Exists(fullPath) || Directory.Exists(fullPath);
        }

        private static string GetProjectRootPath()
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        }
    }
}
