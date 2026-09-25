using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace GoogleMobileAds.Editor
{
    /// <summary>
    /// Automatically installs or updates Unity's External Dependency Manager (com.unity.external-dependency-manager)
    /// in the project's Packages/manifest.json during initial asset import or Editor initialization.
    /// </summary>
    [InitializeOnLoad]
    public class EdmDependencyInstaller : AssetPostprocessor
    {
        private const string NewPackageName = "com.unity.external-dependency-manager";
        private const string TargetPackageVersion = "2.1.0";

        static EdmDependencyInstaller()
        {
            ExecuteMigration();
        }

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            ExecuteMigration();
        }

        private static void ExecuteMigration()
        {
            string manifestPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Packages", "manifest.json"));
            if (!File.Exists(manifestPath))
            {
                return;
            }

            try
            {
                string manifestText = File.ReadAllText(manifestPath);

                // Regex matches: "com.unity.external-dependency-manager": "x.y.z"
                var pattern = $@"""{Regex.Escape(NewPackageName)}""\s*:\s*""([^""]+)""";
                var match = Regex.Match(manifestText, pattern);

                if (match.Success)
                {
                    string installedVersionStr = match.Groups[1].Value;

                    // If installed version is valid SemVer/System.Version and meets or exceeds target, exit
                    if (Version.TryParse(installedVersionStr, out Version installedVer) &&
                        Version.TryParse(TargetPackageVersion, out Version targetVer))
                    {
                        if (installedVer >= targetVer)
                        {
                            return; // Target version is already satisfied
                        }
                    }
                }

                // Not found, or installed version < TargetPackageVersion:
                string packageSpec = $"{NewPackageName}@{TargetPackageVersion}";
                Debug.Log($"[Google Mobile Ads] Updating {NewPackageName} to {TargetPackageVersion} via UPM API...");
                Client.Add(packageSpec);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Google Mobile Ads] Failed to update Packages/manifest.json: {ex.Message}");
            }
        }
    }
}
