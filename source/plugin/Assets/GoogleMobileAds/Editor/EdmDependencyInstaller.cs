using System;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace GoogleMobileAds.Editor
{
    /// <summary>
    /// Installs and updates Unity's External Dependency Manager
    /// (`com.unity.external-dependency-manager`) through the Unity Package Manager API when the
    /// Google Mobile Ads plugin is imported as a `.unitypackage`.
    ///
    /// On first import, and once after each plugin release that requires a different EDM version,
    /// the required EDM version (<see cref="EdmPackageVersion"/>) is installed. This replaces
    /// whatever version or source (such as a git URL) the project declared at the time. Between
    /// such releases the declared version is not changed, so a version the publisher sets is kept.
    ///
    /// Publishers opt out by removing the package from the Package Manager. Once a previously
    /// detected EDM package is found to be missing, the installer disables
    /// <see cref="GoogleMobileAdsSettings.EnableExternalDependencyManager"/>. An opted-out project
    /// is never modified again, including when the plugin is re-imported or upgraded. The setting
    /// is stored in `GoogleMobileAdsSettings.asset`, which the `.unitypackage` does not ship, so
    /// re-importing the plugin keeps it, and it is shared with the whole team and CI through
    /// version control.
    ///
    /// Publishers can reinstall EDM at any time by re-enabling the setting in the Google Mobile Ads
    /// settings.
    ///
    /// The plugin never deletes or modifies any other dependency manager already in the project,
    /// such as Google's legacy EDM or a publisher's custom tooling.
    /// </summary>
    internal static class EdmDependencyInstaller
    {
        internal const string EdmPackageName = "com.unity.external-dependency-manager";

        // Update this when a plugin release requires a newer EDM. Projects that have not opted out
        // are set to this version on re-import.
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
            EditorApplication.delayCall += ApplyEdmInstallPolicy;
        }

        /// <summary>
        /// Called when the publisher re-enables EDM in the Google Mobile Ads settings. Enables
        /// the setting, then installs EDM at the
        /// version this plugin requires.
        /// </summary>
        internal static void ReinstallEdm()
        {
            var settings = GoogleMobileAdsSettings.LoadInstance();
            // Always save the setting: the settings editor has already enabled it in memory but
            // has not saved it.
            settings.EnableExternalDependencyManager = true;
            // Treat the reinstall like a first-time import, so that a failed or interrupted
            // installation is retried instead of being mistaken for a publisher removal.
            // `MonitorAddRequest`, or the next `ApplyEdmInstallPolicy` run once EDM is declared,
            // sets this again.
            settings.EdmPackageDetected = false;
            SaveSettings(settings);

            AddEdmPackage();
        }

        /// <summary>
        /// Runs on Editor load. Installs EDM at <see cref="EdmPackageVersion"/>, opts out if the
        /// publisher removed it, or does nothing, as decided by <see cref="EdmInstallPolicy"/>.
        /// </summary>
        private static void ApplyEdmInstallPolicy()
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
            if (!TryReadIsEdmDeclared(out isDeclared))
            {
                return;
            }

            // EDM is present: remember it so a later removal can be detected.
            if (isDeclared && !settings.EdmPackageDetected)
            {
                settings.EdmPackageDetected = true;
                SaveSettings(settings);
            }

            var state = new EdmInstallPolicy.ProjectState
            {
                IsDeclared = isDeclared,
                EnableEdm = settings.EnableExternalDependencyManager,
                PackageDetected = settings.EdmPackageDetected,
                EdmVersionAdded = settings.AddedEdmVersion == EdmPackageVersion,
            };
            switch (EdmInstallPolicy.Decide(state))
            {
                case EdmInstallPolicy.Action.Install:
                    AddEdmPackage();
                    break;
                case EdmInstallPolicy.Action.OptOut:
                    settings.EnableExternalDependencyManager = false;
                    SaveSettings(settings);
                    Debug.Log(LogPrefix + $"{EdmPackageName} was removed from the project, so " +
                              "Unity External Dependency Manager has been disabled. " + ReinstallHint);
                    break;
                case EdmInstallPolicy.Action.None:
                default:
                    break;
            }
        }

        /// <summary>
        /// Loads the settings asset without risking overwriting it. While Unity is compiling or
        /// importing (for example right after the plugin is re-imported), or if the asset exists on
        /// disk but cannot be loaded yet, <see cref="ApplyEdmInstallPolicy"/> is retried on a later
        /// Editor tick instead of letting <see cref="GoogleMobileAdsSettings.LoadInstance"/>
        /// create a new, empty asset that would lose the publisher's opt-out.
        /// </summary>
        private static bool TryLoadSettings(out GoogleMobileAdsSettings settings)
        {
            settings = null;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                if (++_settingsLoadAttempts < MaxSettingsLoadAttempts)
                {
                    EditorApplication.delayCall += ApplyEdmInstallPolicy;
                }
                return false;
            }

            if (File.Exists(GoogleMobileAdsSettings.AssetPath))
            {
                settings = AssetDatabase.LoadAssetAtPath<GoogleMobileAdsSettings>(
                    GoogleMobileAdsSettings.AssetPath);
                if (settings == null)
                {
                    if (++_settingsLoadAttempts < MaxSettingsLoadAttempts)
                    {
                        EditorApplication.delayCall += ApplyEdmInstallPolicy;
                    }
                    return false;
                }
                return true;
            }

            settings = GoogleMobileAdsSettings.LoadInstance();
            return settings != null;
        }

        /// <summary>
        /// Returns true if the plugin was imported through the Unity Package Manager. In that case
        /// EDM is already declared as a dependency in the plugin's `package.json`.
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
                settings.AddedEdmVersion = EdmPackageVersion;
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
        /// Reads `Packages/manifest.json` and outputs whether EDM is declared.
        /// Returns false if the manifest could not be read.
        /// </summary>
        private static bool TryReadIsEdmDeclared(out bool isDeclared)
        {
            isDeclared = false;
            string manifestPath = Path.GetFullPath(
                Path.Combine(Application.dataPath, "..", "Packages", "manifest.json"));
            if (!File.Exists(manifestPath))
            {
                return false;
            }

            try
            {
                isDeclared = EdmInstallPolicy.IsDeclared(File.ReadAllText(manifestPath),
                                                         EdmPackageName);
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
