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
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace GoogleMobileAds.Editor
{
    /// <summary>
    /// Coordinates the complete migration from legacy .unitypackage asset imports
    /// to OpenUPM packages.
    /// </summary>
    public static class PackageMigrator
    {
        public const string CorePackageName = "com.google.ads.mobile";
        public const string DefaultFallbackVersion = "11.7.0";
        private const string LogPrefix = "[Google Mobile Ads] ";

        [MenuItem("Assets/Google Mobile Ads/Migrate to Packages", false, 20)]
        public static void MigrateToPackages()
        {
            StartMigration();
        }

        [MenuItem("Assets/Google Mobile Ads/Migrate to Packages", true)]
        public static bool ValidateMigrateToPackages()
        {
            // Only enabled if GMA is currently imported as an asset (not via UPM)
            return !EdmDependencyInstaller.IsInstalledViaUpm();
        }

        [Obsolete("Use MigrateToPackages() instead.")]
        public static void MigrateToPackageImport()
        {
            MigrateToPackages();
        }

        /// <summary>
        /// Initiates the interactive migration workflow.
        /// </summary>
        public static void StartMigration()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string assetsDir = Path.Combine(projectRoot, "Assets");

            // 1. Detect Core GMA Version
            string gmaVersion = MediationVersionResolver.DetectInstalledGmaVersion(assetsDir);
            if (string.IsNullOrEmpty(gmaVersion))
            {
                gmaVersion = DefaultFallbackVersion;
            }

            // 2. Resolve installed mediation adapters
            var resolver = new MediationVersionResolver();
            MediationResolutionResult resolution = resolver.ResolveInstalledAdapters(assetsDir);

            var packagesToAdd = new Dictionary<string, string>(resolution.ResolvedPackages);
            packagesToAdd[CorePackageName] = gmaVersion;

            // 3. Format package list for confirmation dialog
            var packageListLines = new List<string>
            {
                $"  • {CorePackageName}: {gmaVersion}"
            };
            foreach (var kvp in resolution.ResolvedPackages)
            {
                packageListLines.Add($"  • {kvp.Key}: {kvp.Value}");
            }
            string packageListText = string.Join("\n", packageListLines);

            bool confirm = EditorUtility.DisplayDialog(
                "Migrate Google Mobile Ads to Packages",
                "This tool will migrate the Google Mobile Ads Unity Plugin and installed mediation " +
                "adapters to Unity Package Manager (UPM) packages via OpenUPM.\n\n" +
                "Packages to install:\n" +
                $"{packageListText}\n\n" +
                "• Your App IDs and settings in Assets/GoogleMobileAds/Resources will be strictly preserved.\n" +
                "• Legacy asset files will be cleanly removed and replaced with UPM packages.\n\n" +
                "Do you want to proceed?",
                "Migrate",
                "Cancel");

            if (!confirm)
            {
                return;
            }

            ExecuteMigration(projectRoot, packagesToAdd, resolution);
        }

        /// <summary>
        /// Executes the migration steps.
        /// </summary>
        public static bool ExecuteMigration(
            string projectRoot = null,
            Dictionary<string, string> packagesToAdd = null,
            MediationResolutionResult resolution = null)
        {
            if (string.IsNullOrEmpty(projectRoot))
            {
                projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            }

            string assetsDir = Path.Combine(projectRoot, "Assets");

            string gmaVersion;
            if (packagesToAdd == null || resolution == null)
            {
                // 1. Detect Core GMA Version
                gmaVersion = MediationVersionResolver.DetectInstalledGmaVersion(assetsDir);
                if (string.IsNullOrEmpty(gmaVersion))
                {
                    gmaVersion = DefaultFallbackVersion;
                }

                // 2. Resolve installed mediation adapters
                var resolver = new MediationVersionResolver();
                resolution = resolver.ResolveInstalledAdapters(assetsDir);

                packagesToAdd = new Dictionary<string, string>(resolution.ResolvedPackages);
                packagesToAdd[CorePackageName] = gmaVersion;
            }
            else
            {
                if (!packagesToAdd.TryGetValue(CorePackageName, out gmaVersion))
                {
                    gmaVersion = DefaultFallbackVersion;
                }
            }

            // 3. Handle unmapped (stale GMA plugin) adapters
            if (resolution.UnmappedAdapters.Count > 0)
            {
                string unmappedNames = string.Join(
                    ", ", resolution.UnmappedAdapters.Select(a => a.AdapterName ?? a.AdapterId));
                bool updateToLatest = EditorUtility.DisplayDialog(
                    "Google Mobile Ads Plugin Update Recommended",
                    $"One or more installed mediation adapters ({unmappedNames}) are newer than " +
                    $"the mapping bundled with this Google Mobile Ads plugin (v{gmaVersion}).\n\n" +
                    "To ensure full compatibility, we recommend updating to the latest Google Mobile " +
                    "Ads plugin and mediation packages from OpenUPM.\n\n" +
                    "Would you like to update to latest and migrate, or cancel?",
                    "Update to Latest & Migrate",
                    "Cancel");

                if (!updateToLatest)
                {
                    Debug.Log(LogPrefix + "Migration cancelled by user due to unmapped adapter versions.");
                    return false;
                }

                // If user agrees to update to latest, set Core GMA and unmapped adapters to latest
                packagesToAdd[CorePackageName] = DefaultFallbackVersion;
                foreach (var unmapped in resolution.UnmappedAdapters)
                {
                    // Fallback to latest known package name if available
                    string pkg = GetPackageNameForAdapter(unmapped.AdapterId);
                    if (!string.IsNullOrEmpty(pkg))
                    {
                        packagesToAdd[pkg] = "latest";
                    }
                }
            }

            // 4. Update Packages/manifest.json atomically
            string manifest = ManifestManager.ReadManifest(projectRoot);
            if (string.IsNullOrEmpty(manifest))
            {
                manifest = "{\n  \"dependencies\": {}\n}\n";
            }

            manifest = ManifestManager.EnsureOpenUpmRegistry(manifest);
            manifest = ManifestManager.AddDependencies(manifest, packagesToAdd);
            ManifestManager.WriteManifest(manifest, projectRoot);
            Debug.Log(LogPrefix + $"Updated Packages/manifest.json with {packagesToAdd.Count} package(s).");

            // 5. Safely clean up legacy Assets
            LegacyAssetCleaner.ExecuteCleanup(projectRoot);
            Debug.Log(LogPrefix + "Migration to Unity Package Manager completed successfully.");

            // 6. Request package resolution and asset refresh
            Client.Resolve();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            return true;
        }

        private static string GetPackageNameForAdapter(string adapterId)
        {
            if (string.IsNullOrEmpty(adapterId))
            {
                return null;
            }

            switch (adapterId.ToLowerInvariant())
            {
                case "applovin": return "com.google.ads.mobile.mediation.applovin";
                case "bidmachine": return "com.google.ads.mobile.mediation.bidmachine";
                case "bigo": return "com.google.ads.mobile.mediation.bigo";
                case "chartboost": return "com.google.ads.mobile.mediation.chartboost";
                case "fyber": return "com.google.ads.mobile.mediation.dtexchange";
                case "imobile": return "com.google.ads.mobile.mediation.imobile";
                case "inmobi": return "com.google.ads.mobile.mediation.inmobi";
                case "ironsource": return "com.google.ads.mobile.mediation.ironsource";
                case "vungle": return "com.google.ads.mobile.mediation.liftoffmonetize";
                case "line": return "com.google.ads.mobile.mediation.line";
                case "maio": return "com.google.ads.mobile.mediation.maio";
                case "facebook": return "com.google.ads.mobile.mediation.metaaudiencenetwork";
                case "mintegral": return "com.google.ads.mobile.mediation.mintegral";
                case "moloco": return "com.google.ads.mobile.mediation.moloco";
                case "mytarget": return "com.google.ads.mobile.mediation.mytarget";
                case "pangle": return "com.google.ads.mobile.mediation.pangle";
                case "pubmatic": return "com.google.ads.mobile.mediation.pubmatic";
                case "unity": return "com.google.ads.mobile.mediation.unity";
                case "verve": return "com.google.ads.mobile.mediation.verve";
                default: return $"com.google.ads.mobile.mediation.{adapterId.ToLowerInvariant()}";
            }
        }
    }
}
