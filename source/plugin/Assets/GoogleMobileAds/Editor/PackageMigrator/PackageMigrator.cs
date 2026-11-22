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
using System.Text;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace GoogleMobileAds.Editor
{
    /// <summary>
    /// Everything the migration will do, computed before any file is modified so it can be shown
    /// to the developer for confirmation.
    /// </summary>
    public class MigrationPlan
    {
        /// <summary>
        /// True when Google Mobile Ads is already installed as a package, so only mediation
        /// adapters imported into Assets are migrated and Core GMA is left untouched.
        /// </summary>
        public bool AdaptersOnly;

        /// <summary>Core GMA version to install. Null in adapters-only mode.</summary>
        public string GmaVersion;

        /// <summary>UPM dependencies to write to manifest.json: packageName -> version.</summary>
        public Dictionary<string, string> PackagesToAdd = new Dictionary<string, string>();

        /// <summary>Where each mediation package version came from.</summary>
        public Dictionary<string, VersionSource> PackageSources =
            new Dictionary<string, VersionSource>();

        /// <summary>Adapters with no known OpenUPM package; they stay in Assets.</summary>
        public List<InstalledMediationAdapter> UnmovableAdapters =
            new List<InstalledMediationAdapter>();

        /// <summary>Project-relative folders of unmovable adapters, kept during cleanup.</summary>
        public List<string> PreservedAdapterFolders = new List<string>();

        /// <summary>Project-relative folders of adapters being replaced by packages.</summary>
        public List<string> MigratedAdapterFolders = new List<string>();

        public bool UsedBundledFallback =>
            PackageSources.Values.Contains(VersionSource.BundledFallback);
    }

    /// <summary>
    /// Coordinates the complete migration from legacy .unitypackage asset imports
    /// to OpenUPM packages.
    /// </summary>
    public static class PackageMigrator
    {
        public const string CorePackageName = "com.google.ads.mobile";
        public const string DefaultFallbackVersion = "11.7.0";
        private const string LogPrefix = "[Google Mobile Ads] ";
        private const string MenuPath = "Assets/Google Mobile Ads/Migrate to Packages";

        internal const string DialogTitle = "Migrate Google Mobile Ads to Packages";
        internal const string ProgressBarTitle = "Google Mobile Ads";
        internal const string OpenUpmLabel = " (latest from OpenUPM)";
        internal const string BundledFallbackLabel = " (OpenUPM unreachable, using bundled version)";
        internal const string BundledFallbackWarning =
            "⚠ Could not reach OpenUPM. Some adapters will use the newest version bundled " +
            "with this plugin, which may be older than what you currently have installed. " +
            "Check your internet connection and try again, or continue with these versions.";
        internal const string UnmovableAdaptersHeader =
            "The following adapters can't be moved to packages and will stay in Assets:";

        // Menu validation runs every time the menu is drawn, so cache what it needs. Statics are
        // reset on every domain reload; the adapter check is also invalidated whenever assets
        // under the mediation folder change (see MediationAdapterImportWatcher).
        private static bool? _gmaInstalledViaUpm;
        private static bool? _hasMovableAdaptersInAssets;
        private static MediationVersionResolver _mappingOnlyResolver;

        [MenuItem(MenuPath, false, 20)]
        public static void MigrateToPackages()
        {
            StartMigration();
        }

        [MenuItem(MenuPath, true)]
        public static bool ValidateMigrateToPackages()
        {
            // Enabled while Google Mobile Ads itself is imported as assets, or, once it is
            // installed as a package, while Assets contains adapters that can be moved.
            if (!IsGmaInstalledViaUpm())
            {
                return true;
            }

            if (_hasMovableAdaptersInAssets == null)
            {
                _hasMovableAdaptersInAssets =
                    HasMovableAdapters(Application.dataPath, GetMappingOnlyResolver());
            }
            return _hasMovableAdaptersInAssets.Value;
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
            bool adaptersOnly = IsGmaInstalledViaUpm();

            MigrationPlan plan = BuildMigrationPlan(
                assetsDir, new MediationVersionResolver(), showProgress: true, adaptersOnly);

            if (plan.AdaptersOnly && plan.PackagesToAdd.Count == 0)
            {
                EditorUtility.DisplayDialog(
                    DialogTitle, BuildNothingToMigrateMessage(plan), "OK");
                return;
            }

            bool confirm = EditorUtility.DisplayDialog(
                DialogTitle, BuildConfirmationMessage(plan), "Migrate", "Cancel");
            if (!confirm)
            {
                return;
            }

            ExecuteMigration(projectRoot, plan);
        }

        /// <summary>
        /// Detects installed GMA and mediation adapters and resolves the package versions to
        /// install. Adapters whose installed version is not in the mapping are resolved to the
        /// latest version on OpenUPM, or to the highest bundled version if OpenUPM is unreachable.
        /// Does not modify any files.
        /// </summary>
        /// <param name="assetsDir">The project's Assets directory.</param>
        /// <param name="resolver">Resolver used for mapping lookups and OpenUPM queries.</param>
        /// <param name="showProgress">Whether to show an Editor progress bar during lookups.</param>
        /// <param name="adaptersOnly">
        /// True when Google Mobile Ads is already installed as a package; Core GMA is then left out.
        /// </param>
        public static MigrationPlan BuildMigrationPlan(
            string assetsDir,
            MediationVersionResolver resolver,
            bool showProgress = false,
            bool adaptersOnly = false)
        {
            var plan = new MigrationPlan { AdaptersOnly = adaptersOnly };

            if (!adaptersOnly)
            {
                plan.GmaVersion = MediationVersionResolver.DetectInstalledGmaVersion(assetsDir);
                if (string.IsNullOrEmpty(plan.GmaVersion))
                {
                    plan.GmaVersion = DefaultFallbackVersion;
                }
                plan.PackagesToAdd[CorePackageName] = plan.GmaVersion;
            }

            MediationResolutionResult resolution = resolver.ResolveInstalledAdapters(assetsDir);
            foreach (var kvp in resolution.ResolvedPackages)
            {
                plan.PackagesToAdd[kvp.Key] = kvp.Value;
                plan.PackageSources[kvp.Key] = VersionSource.Mapped;
            }

            List<InstalledMediationAdapter> unmapped = resolution.UnmappedAdapters;
            try
            {
                for (int i = 0; i < unmapped.Count; i++)
                {
                    InstalledMediationAdapter adapter = unmapped[i];
                    string packageName = resolver.GetPackageName(adapter.AdapterId);
                    if (string.IsNullOrEmpty(packageName))
                    {
                        MarkUnmovable(plan, adapter);
                        continue;
                    }

                    if (plan.PackagesToAdd.ContainsKey(packageName))
                    {
                        continue;
                    }

                    if (showProgress)
                    {
                        ShowProgressBar(
                            $"Checking OpenUPM for the latest {packageName} version...",
                            (float)i / unmapped.Count);
                    }

                    string version = resolver.ResolveLatestVersion(
                        adapter.AdapterId, out VersionSource source);
                    if (string.IsNullOrEmpty(version))
                    {
                        MarkUnmovable(plan, adapter);
                        continue;
                    }

                    plan.PackagesToAdd[packageName] = version;
                    plan.PackageSources[packageName] = source;

                    if (source == VersionSource.BundledFallback &&
                        resolver.LastOpenUpmLookupUnreachable)
                    {
                        // OpenUPM is unreachable; don't wait for another timeout per adapter.
                        resolver.OpenUpmLatestVersionFetcher = null;
                    }
                }
            }
            finally
            {
                if (showProgress)
                {
                    ClearProgressBar();
                }
            }

            foreach (InstalledMediationAdapter adapter in resolution.InstalledAdapters)
            {
                string folder = GetAdapterFolderPath(adapter);
                if (folder != null &&
                    !plan.PreservedAdapterFolders.Contains(folder) &&
                    !plan.MigratedAdapterFolders.Contains(folder))
                {
                    plan.MigratedAdapterFolders.Add(folder);
                }
            }

            PreserveUnhandledMediationFolders(assetsDir, plan);

            LogPlanWarnings(plan);
            return plan;
        }

        /// <summary>
        /// Builds the text of the confirmation dialog for a migration plan.
        /// </summary>
        public static string BuildConfirmationMessage(MigrationPlan plan)
        {
            var sb = new StringBuilder();
            if (plan.AdaptersOnly)
            {
                sb.Append("Google Mobile Ads is already installed as a package. This tool will " +
                          "migrate the mediation adapters found in Assets to Unity Package " +
                          "Manager (UPM) packages via OpenUPM.\n\n");
            }
            else
            {
                sb.Append("This tool will migrate the Google Mobile Ads Unity Plugin and installed " +
                          "mediation adapters to Unity Package Manager (UPM) packages via OpenUPM.\n\n");
            }

            sb.Append("Packages to install:\n");
            if (!plan.AdaptersOnly)
            {
                sb.Append($"  • {CorePackageName}: {plan.GmaVersion}\n");
            }
            foreach (var kvp in plan.PackagesToAdd)
            {
                if (kvp.Key == CorePackageName)
                {
                    continue;
                }
                sb.Append($"  • {kvp.Key}: {kvp.Value}{GetSourceLabel(plan, kvp.Key)}\n");
            }

            if (plan.UsedBundledFallback)
            {
                sb.Append("\n").Append(BundledFallbackWarning).Append("\n");
            }

            AppendUnmovableAdapters(sb, plan);

            if (plan.AdaptersOnly)
            {
                sb.Append("\n• The migrated adapter folders will be removed from " +
                          "Assets/GoogleMobileAds/Mediation.\n\n");
            }
            else
            {
                sb.Append("\n• Your App IDs and settings in Assets/GoogleMobileAds/Resources will be " +
                          "strictly preserved.\n");
                sb.Append("• Legacy asset files will be cleanly removed and replaced with UPM " +
                          "packages.\n\n");
            }
            sb.Append("Do you want to proceed?");
            return sb.ToString();
        }

        /// <summary>
        /// Executes the migration steps. If no plan is given, a full migration plan is built
        /// from the project.
        /// </summary>
        public static bool ExecuteMigration(string projectRoot = null, MigrationPlan plan = null)
        {
            if (string.IsNullOrEmpty(projectRoot))
            {
                projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            }

            if (plan == null)
            {
                bool adaptersOnly =
                    IsGmaInstalledViaUpm() || IsCoreGmaDeclaredInManifest(projectRoot);
                plan = BuildMigrationPlan(
                    Path.Combine(projectRoot, "Assets"),
                    new MediationVersionResolver(),
                    adaptersOnly: adaptersOnly);
            }

            // 1. Update Packages/manifest.json atomically
            string manifest = ManifestManager.ReadManifest(projectRoot);
            if (string.IsNullOrEmpty(manifest))
            {
                manifest = "{\n  \"dependencies\": {}\n}\n";
            }

            manifest = ManifestManager.EnsureOpenUpmRegistry(manifest);
            manifest = ManifestManager.AddDependencies(manifest, plan.PackagesToAdd);
            ManifestManager.WriteManifest(manifest, projectRoot);
            Debug.Log(LogPrefix +
                      $"Updated Packages/manifest.json with {plan.PackagesToAdd.Count} package(s).");

            // 2. Safely clean up legacy Assets, keeping adapters that could not be migrated
            if (plan.AdaptersOnly)
            {
                LegacyAssetCleaner.DeleteAdapterFolders(projectRoot, plan.MigratedAdapterFolders);
            }
            else
            {
                LegacyAssetCleaner.ExecuteCleanup(projectRoot, plan.PreservedAdapterFolders);
            }

            // 3. Report anything that could not be removed
            List<string> leftovers = FindLeftoverAssets(projectRoot, plan);
            if (leftovers.Count > 0)
            {
                Debug.LogWarning(LogPrefix + "Migration finished, but these legacy files could " +
                                 "not be removed. Please delete them manually:\n  • " +
                                 string.Join("\n  • ", leftovers));
            }
            else
            {
                Debug.Log(LogPrefix + "Migration to Unity Package Manager completed successfully.");
            }
            InvalidateMenuState();

            // 4. Request package resolution and asset refresh
            try
            {
                Client.Resolve();
            }
            catch
            {
                // Ignored when running outside Unity Editor GUI (e.g. in headless unit test runners)
            }

            try
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
            catch
            {
                // Ignored when running outside Unity Editor GUI (e.g. in headless unit test runners)
            }

            return true;
        }

        /// <summary>
        /// Returns legacy assets that the plan expected to remove but that still exist.
        /// </summary>
        public static List<string> FindLeftoverAssets(string projectRoot, MigrationPlan plan)
        {
            if (plan.AdaptersOnly)
            {
                return plan.MigratedAdapterFolders
                    .Where(folder => Directory.Exists(Path.Combine(
                        projectRoot, folder.Replace('/', Path.DirectorySeparatorChar))))
                    .ToList();
            }

            return LegacyAssetCleaner.GetAssetsToDelete(projectRoot, plan.PreservedAdapterFolders)
                .Where(assetPath => PathExistsInProject(projectRoot, assetPath))
                .ToList();
        }

        private static bool PathExistsInProject(string projectRoot, string relativePath)
        {
            if (string.IsNullOrEmpty(projectRoot) || string.IsNullOrEmpty(relativePath))
            {
                return false;
            }
            string fullPath = Path.Combine(
                projectRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
            return File.Exists(fullPath) || Directory.Exists(fullPath);
        }

        /// <summary>
        /// Returns true if Assets contains at least one mediation adapter that has a known
        /// OpenUPM package (i.e. its adapter ID is in the mapping). Does not query OpenUPM.
        /// </summary>
        public static bool HasMovableAdapters(string assetsDir, MediationVersionResolver resolver)
        {
            if (!Directory.Exists(Path.Combine(assetsDir, "GoogleMobileAds", "Mediation")))
            {
                return false;
            }

            return MediationVersionResolver.ScanInstalledAdapters(assetsDir)
                .Any(adapter => !string.IsNullOrEmpty(resolver.GetPackageName(adapter.AdapterId)));
        }

        /// <summary>
        /// Returns the distinct adapter folder names under Assets/GoogleMobileAds/Mediation that
        /// the given asset paths belong to.
        /// </summary>
        public static List<string> GetAdapterFolderNames(IEnumerable<string> assetPaths)
        {
            var names = new List<string>();
            if (assetPaths == null)
            {
                return names;
            }

            string prefix = LegacyAssetCleaner.MediationFolderPath + "/";
            foreach (string path in assetPaths)
            {
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                string normalized = path.Replace('\\', '/');
                if (!normalized.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string rest = normalized.Substring(prefix.Length);
                int slash = rest.IndexOf('/');
                string name = slash >= 0 ? rest.Substring(0, slash) : rest;
                if (!string.IsNullOrEmpty(name) &&
                    !name.EndsWith(".meta", StringComparison.OrdinalIgnoreCase) &&
                    !names.Contains(name, StringComparer.OrdinalIgnoreCase))
                {
                    names.Add(name);
                }
            }
            return names;
        }

        /// <summary>
        /// Builds the console message suggesting migration for newly imported adapters, or
        /// returns null if none of them can be moved to packages.
        /// </summary>
        public static string BuildImportedAdaptersMessage(
            string assetsDir, IEnumerable<string> importedFolderNames, MediationVersionResolver resolver)
        {
            var folders = new HashSet<string>(
                importedFolderNames ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);
            if (folders.Count == 0)
            {
                return null;
            }

            List<string> movable = MediationVersionResolver.ScanInstalledAdapters(assetsDir)
                .Where(a => a.FolderName != null && folders.Contains(a.FolderName) &&
                            !string.IsNullOrEmpty(resolver.GetPackageName(a.AdapterId)))
                .Select(GetDisplayName)
                .Distinct()
                .ToList();
            if (movable.Count == 0)
            {
                return null;
            }

            string verb = movable.Count == 1 ? "was" : "were";
            string pronoun = movable.Count == 1 ? "it" : "them";
            return LogPrefix + $"{string.Join(", ", movable)} {verb} imported into Assets. Use " +
                   $"Assets > Google Mobile Ads > Migrate to Packages to move {pronoun} to a package.";
        }

        internal static bool IsGmaInstalledViaUpm()
        {
            if (_gmaInstalledViaUpm == null)
            {
                try
                {
                    _gmaInstalledViaUpm = EdmDependencyInstaller.IsInstalledViaUpm();
                }
                catch
                {
                    // ScriptableObject.CreateInstance throws in headless unit tests.
                    return false;
                }
            }
            return _gmaInstalledViaUpm.Value;
        }

        private static bool IsCoreGmaDeclaredInManifest(string projectRoot)
        {
            string manifestJson = ManifestManager.ReadManifest(projectRoot);
            return ManifestManager.HasDependency(manifestJson, CorePackageName);
        }

        internal static void InvalidateMenuState()
        {
            _hasMovableAdaptersInAssets = null;
        }

        internal static MediationVersionResolver GetMappingOnlyResolver()
        {
            if (_mappingOnlyResolver == null)
            {
                // Only used for mapping lookups, so never query OpenUPM.
                _mappingOnlyResolver = new MediationVersionResolver
                {
                    OpenUpmLatestVersionFetcher = null
                };
            }
            return _mappingOnlyResolver;
        }

        private static string BuildNothingToMigrateMessage(MigrationPlan plan)
        {
            var sb = new StringBuilder(
                "Google Mobile Ads is already installed as a package and there are no mediation " +
                "adapters in Assets that can be moved to packages.\n");
            AppendUnmovableAdapters(sb, plan);
            return sb.ToString();
        }

        private static void AppendUnmovableAdapters(StringBuilder sb, MigrationPlan plan)
        {
            if (plan.UnmovableAdapters.Count == 0)
            {
                return;
            }

            sb.Append("\n").Append(UnmovableAdaptersHeader).Append("\n");
            foreach (InstalledMediationAdapter adapter in plan.UnmovableAdapters)
            {
                sb.Append($"  • {GetDisplayName(adapter)}\n");
            }
        }

        private static void MarkUnmovable(MigrationPlan plan, InstalledMediationAdapter adapter)
        {
            plan.UnmovableAdapters.Add(adapter);
            string folder = GetAdapterFolderPath(adapter);
            if (folder != null && !plan.PreservedAdapterFolders.Contains(folder))
            {
                plan.PreservedAdapterFolders.Add(folder);
            }
        }

        private static string GetAdapterFolderPath(InstalledMediationAdapter adapter)
        {
            return string.IsNullOrEmpty(adapter.FolderName)
                ? null
                : $"{LegacyAssetCleaner.MediationFolderPath}/{adapter.FolderName}";
        }

        private static string GetSourceLabel(MigrationPlan plan, string packageName)
        {
            if (!plan.PackageSources.TryGetValue(packageName, out VersionSource source))
            {
                return string.Empty;
            }

            switch (source)
            {
                case VersionSource.OpenUpm:
                    return OpenUpmLabel;
                case VersionSource.BundledFallback:
                    return BundledFallbackLabel;
                default:
                    return string.Empty;
            }
        }

        private static void PreserveUnhandledMediationFolders(string assetsDir, MigrationPlan plan)
        {
            if (string.IsNullOrEmpty(assetsDir))
            {
                return;
            }

            string mediationDir = Path.Combine(assetsDir, "GoogleMobileAds", "Mediation");
            if (!Directory.Exists(mediationDir))
            {
                return;
            }

            foreach (string dir in Directory.GetDirectories(mediationDir))
            {
                string folderName = Path.GetFileName(dir);
                if (string.IsNullOrEmpty(folderName))
                {
                    continue;
                }

                string relativePath = $"Assets/GoogleMobileAds/Mediation/{folderName}";
                bool alreadyHandled =
                    plan.PreservedAdapterFolders.Any(f =>
                        string.Equals(f, relativePath, StringComparison.OrdinalIgnoreCase)) ||
                    plan.MigratedAdapterFolders.Any(f =>
                        string.Equals(f, relativePath, StringComparison.OrdinalIgnoreCase));
                if (!alreadyHandled)
                {
                    MarkUnmovable(plan, new InstalledMediationAdapter
                    {
                        AdapterName = folderName,
                        FolderName = folderName
                    });
                }
            }
        }

        private static string GetDisplayName(InstalledMediationAdapter adapter)
        {
            if (adapter == null)
            {
                return "Unknown adapter";
            }
            if (!string.IsNullOrEmpty(adapter.AdapterName))
            {
                return adapter.AdapterName;
            }
            if (!string.IsNullOrEmpty(adapter.FolderName))
            {
                return adapter.FolderName;
            }
            if (!string.IsNullOrEmpty(adapter.AdapterId))
            {
                return adapter.AdapterId;
            }
            return "Unknown adapter";
        }

        private static void LogPlanWarnings(MigrationPlan plan)
        {
            if (plan.UsedBundledFallback)
            {
                var fallbackPackages = plan.PackageSources
                    .Where(kvp => kvp.Value == VersionSource.BundledFallback)
                    .Select(kvp => $"{kvp.Key}: {plan.PackagesToAdd[kvp.Key]}");
                Debug.LogWarning(LogPrefix + "Could not reach OpenUPM. Using bundled versions for: " +
                                 string.Join(", ", fallbackPackages));
            }

            if (plan.UnmovableAdapters.Count > 0)
            {
                Debug.LogWarning(LogPrefix + "These adapters can't be moved to packages and will " +
                                 "stay in Assets: " +
                                 string.Join(", ", plan.UnmovableAdapters.Select(GetDisplayName)));
            }
        }

        private static void ShowProgressBar(string info, float progress)
        {
            try
            {
                EditorUtility.DisplayProgressBar(ProgressBarTitle, info, progress);
            }
            catch
            {
                // Ignored in headless test runners where native EditorUtility is not bound.
            }
        }

        private static void ClearProgressBar()
        {
            try
            {
                EditorUtility.ClearProgressBar();
            }
            catch
            {
                // Ignored in headless test runners where native EditorUtility is not bound.
            }
        }
    }

    /// <summary>
    /// Watches for mediation adapters imported into Assets after Google Mobile Ads is installed
    /// as a package, refreshes the Migrate to Packages menu state, and points the developer to it.
    /// </summary>
    internal class MediationAdapterImportWatcher : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            List<string> importedFolders = PackageMigrator.GetAdapterFolderNames(
                importedAssets.Concat(movedAssets));
            bool mediationChanged = importedFolders.Count > 0 ||
                PackageMigrator.GetAdapterFolderNames(
                    deletedAssets.Concat(movedFromAssetPaths)).Count > 0;
            if (!mediationChanged)
            {
                return;
            }

            PackageMigrator.InvalidateMenuState();

            // While GMA itself is in Assets the menu is always enabled, so only point developers
            // to it once GMA is installed as a package.
            if (importedFolders.Count == 0 || !PackageMigrator.IsGmaInstalledViaUpm())
            {
                return;
            }

            string message = PackageMigrator.BuildImportedAdaptersMessage(
                Application.dataPath, importedFolders, PackageMigrator.GetMappingOnlyResolver());
            if (message != null)
            {
                Debug.Log(message);
            }
        }
    }
}
