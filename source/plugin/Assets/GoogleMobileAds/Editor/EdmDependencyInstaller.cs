using System;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace GoogleMobileAds.Editor
{
    /// <summary>
    /// Installs and upgrades Unity's External Dependency Manager
    /// (com.unity.external-dependency-manager) through the Unity Package Manager API when the
    /// Google Mobile Ads plugin is imported as a .unitypackage.
    ///
    /// On first import the required EDM version is installed. When a later plugin release requires
    /// a higher EDM version, the declared version is upgraded. Versions the publisher has set to
    /// something higher, or to a non-registry source such as a git URL, are left alone.
    ///
    /// Publishers opt out by removing the package from the Package Manager. Once a previously
    /// detected EDM package is found to be missing, the installer disables
    /// <see cref="GoogleMobileAdsSettings.EnableExternalDependencyManager"/>. An opted-out project
    /// is never modified again, including when the plugin is re-imported or upgraded. The setting
    /// is stored in GoogleMobileAdsSettings.asset, which the .unitypackage does not ship, so
    /// re-importing the plugin keeps it, and it is shared with the whole team and CI through
    /// version control.
    ///
    /// Publishers can reinstall EDM at any time by re-enabling the setting in the Google Mobile Ads
    /// settings.
    ///
    /// The plugin never deletes or modifies any other dependency manager already in the project,
    /// such as Google's legacy EDM or a publisher's custom tooling.
    /// </summary>
    public static class EdmDependencyInstaller
    {
        internal const string EdmPackageName = "com.unity.external-dependency-manager";

        // Update this when a plugin release requires a newer EDM. Projects that have not opted out
        // are upgraded to this version on import.
        internal const string EdmPackageVersion = "2.1.0";

        private const string ReinstallHint =
            "To reinstall it, enable \"Enable External Dependency Manager\" in " +
            "Assets > Google Mobile Ads > Settings.";
        private const string LogPrefix = "[Google Mobile Ads] ";

        // How many Editor ticks to wait for the settings asset to become loadable before giving up
        // for this Editor session.
        private const int MaxSettingsLoadAttempts = 100;

        // Pending Package Manager request, if an installation is in progress.
        private static AddRequest _addRequest;

        private static int _settingsLoadAttempts;

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            // Defer until the Editor has finished loading so the AssetDatabase is ready.
            _settingsLoadAttempts = 0;
            EditorApplication.delayCall += CheckEdmInstallation;
        }

        /// <summary>
        /// Called when the publisher re-enables EDM in the Google Mobile Ads settings. Enables
        /// the setting, then installs EDM if it is missing or upgrades it if it is older than the
        /// version this plugin requires.
        /// </summary>
        internal static void ReinstallEdm()
        {
            var settings = GoogleMobileAdsSettings.LoadInstance();
            if (!settings.EnableExternalDependencyManager)
            {
                settings.EnableExternalDependencyManager = true;
                SaveSettings(settings);
            }

            bool isDeclared;
            string declaredVersion;
            if (TryReadDeclaredEdmVersion(out isDeclared, out declaredVersion) && isDeclared &&
                !EdmInstallPolicy.IsOlderVersion(declaredVersion, EdmPackageVersion))
            {
                Debug.Log(LogPrefix + $"{EdmPackageName} {declaredVersion} is already installed.");
                return;
            }

            AddEdmPackage();
        }

        /// <summary>
        /// Installs, upgrades, or leaves EDM alone according to <see cref="EdmInstallPolicy"/>.
        /// </summary>
        private static void CheckEdmInstallation()
        {
            if (_addRequest != null || IsInstalledViaUpm())
            {
                return;
            }

            GoogleMobileAdsSettings settings;
            if (!TryLoadSettings(out settings))
            {
                return;
            }

            bool isDeclared;
            string declaredVersion;
            if (!TryReadDeclaredEdmVersion(out isDeclared, out declaredVersion))
            {
                return;
            }

            // EDM is present: remember it so a later removal can be detected.
            if (isDeclared && !settings.EdmPackageDetected)
            {
                settings.EdmPackageDetected = true;
                SaveSettings(settings);
            }

            switch (EdmInstallPolicy.Decide(isDeclared, declaredVersion,
                                            settings.EnableExternalDependencyManager,
                                            settings.EdmPackageDetected, EdmPackageVersion))
            {
                case EdmInstallPolicy.Action.Install:
                    AddEdmPackage();
                    break;
                case EdmInstallPolicy.Action.Upgrade:
                    Debug.Log(LogPrefix + $"Upgrading {EdmPackageName} from {declaredVersion} " +
                              $"to {EdmPackageVersion}.");
                    AddEdmPackage();
                    break;
                case EdmInstallPolicy.Action.OptOut:
                    settings.EnableExternalDependencyManager = false;
                    SaveSettings(settings);
                    Debug.Log(LogPrefix + $"{EdmPackageName} was removed from the project, so " +
                              "External Dependency Manager has been disabled. " + ReinstallHint);
                    break;
                case EdmInstallPolicy.Action.None:
                default:
                    break;
            }
        }

        /// <summary>
        /// Loads the settings asset without risking overwriting it. While Unity is compiling or
        /// importing (for example right after the plugin is re-imported), or if the asset exists on
        /// disk but cannot be loaded yet, the check is retried on a later Editor tick instead of
        /// letting <see cref="GoogleMobileAdsSettings.LoadInstance"/> create a new, empty asset
        /// that would lose the publisher's opt-out.
        /// </summary>
        private static bool TryLoadSettings(out GoogleMobileAdsSettings settings)
        {
            settings = null;
            bool assetNotReady = File.Exists(GoogleMobileAdsSettings.AssetPath) &&
                AssetDatabase.LoadAssetAtPath<GoogleMobileAdsSettings>(
                    GoogleMobileAdsSettings.AssetPath) == null;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating || assetNotReady)
            {
                if (++_settingsLoadAttempts < MaxSettingsLoadAttempts)
                {
                    EditorApplication.delayCall += CheckEdmInstallation;
                }
                return false;
            }

            settings = GoogleMobileAdsSettings.LoadInstance();
            return settings != null;
        }

        /// <summary>
        /// Returns true if the plugin was imported through the Unity Package Manager. In that case
        /// EDM is already declared as a dependency in the plugin's package.json.
        /// </summary>
        internal static bool IsInstalledViaUpm()
        {
            var pathUtils = ScriptableObject.CreateInstance<EditorPathUtils>();
            try
            {
                return pathUtils.IsPackageRootPath();
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(pathUtils);
            }
        }

        /// <summary>
        /// Adds EDM at <see cref="EdmPackageVersion"/>. If EDM is already declared, the Package
        /// Manager updates the declared version.
        /// </summary>
        private static void AddEdmPackage()
        {
            if (_addRequest != null)
            {
                return;
            }

            string packageSpec = $"{EdmPackageName}@{EdmPackageVersion}";
            Debug.Log(LogPrefix + $"Installing {packageSpec} using the Unity Package Manager.");
            _addRequest = Client.Add(packageSpec);
            EditorApplication.update += MonitorAddRequest;
        }

        private static void MonitorAddRequest()
        {
            if (_addRequest == null)
            {
                EditorApplication.update -= MonitorAddRequest;
                return;
            }

            if (!_addRequest.IsCompleted)
            {
                return;
            }

            EditorApplication.update -= MonitorAddRequest;

            if (_addRequest.Status == StatusCode.Success)
            {
                // Only record the installation once it has succeeded, so that a failed or
                // interrupted install is retried rather than mistaken for a publisher removal.
                var settings = GoogleMobileAdsSettings.LoadInstance();
                settings.EdmPackageDetected = true;
                settings.EnableExternalDependencyManager = true;
                SaveSettings(settings);
                Debug.Log(LogPrefix + $"Installed {EdmPackageName} " +
                          $"{_addRequest.Result?.version ?? EdmPackageVersion}.");
            }
            else
            {
                string error =
                    _addRequest.Error != null ? _addRequest.Error.message : "unknown error";
                Debug.LogError(LogPrefix + $"Failed to install {EdmPackageName}: {error}. " +
                               "Install it from the Package Manager.");
            }

            _addRequest = null;
        }

        /// <summary>
        /// Reads Packages/manifest.json and returns whether EDM is declared and at which version.
        /// Returns false if the manifest could not be read.
        /// </summary>
        private static bool TryReadDeclaredEdmVersion(out bool isDeclared,
                                                      out string declaredVersion)
        {
            isDeclared = false;
            declaredVersion = null;
            string manifestPath = Path.GetFullPath(
                Path.Combine(Application.dataPath, "..", "Packages", "manifest.json"));
            if (!File.Exists(manifestPath))
            {
                return false;
            }

            try
            {
                isDeclared = EdmInstallPolicy.TryGetDeclaredVersion(
                    File.ReadAllText(manifestPath), EdmPackageName, out declaredVersion);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning(LogPrefix + $"Unable to read {manifestPath}: {ex.Message}");
                return false;
            }
        }

        private static void SaveSettings(GoogleMobileAdsSettings settings)
        {
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }
    }
}
