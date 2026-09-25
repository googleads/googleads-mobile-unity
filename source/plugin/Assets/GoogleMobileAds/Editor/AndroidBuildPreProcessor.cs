#if UNITY_ANDROID
#if UNITY_2022 || UNITY_2021_3_58 || UNITY_2021_3_57 || UNITY_2021_3_56 || UNITY_2021_3_55 || UNITY_2021_3_54 || UNITY_2021_3_53 || UNITY_2021_3_52 || UNITY_2021_3_51 || UNITY_2021_3_50 || UNITY_2021_3_49 || UNITY_2021_3_48 || UNITY_2021_3_47 || UNITY_2021_3_46 || UNITY_2021_3_45 || UNITY_2021_3_44 || UNITY_2021_3_43 || UNITY_2021_3_42 || UNITY_2021_3_41
// 2021.3.41f1+	Gradle version 7.5.1+
// https://docs.unity3d.com/2021.3/Documentation/Manual/android-gradle-overview.html
#define ANDROID_GRADLE_BUILD_JETIFIER_ENTRY_ENABLED
#endif

#if UNITY_6000_0_OR_NEWER || UNITY_2023 || ANDROID_GRADLE_BUILD_JETIFIER_ENTRY_ENABLED
#define ANDROID_GRADLE_BUILD_PRE_PROCESSOR_ENABLED
#endif

using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using System.IO;
using GooglePlayServices;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace GoogleMobileAds.Editor
{
    /// <summary>
    /// This script will verify and configure your Android build settings to be compatible
    /// with the Google Mobile Ads SDK. This includes:
    ///  - Verify the Android Google Mobile Ads app ID is set.
    ///  - Throw an exception if the Android Google Mobile Ads app ID is not set.
    ///  - Set minimum API level to 23 (the target API level may be automatically set, we should not
    ///    hardcode it).
    ///  - Enable Custom Main Gradle Template.
    ///  - Update Custom Main Gradle Template with dependencies using the Play Services Resolver.
    ///  - Enable Custom Gradle Properties Template.
    ///  - Update Custom Gradle Properties Template with the Jetifier Ignorelist.
    ///  - Enable Custom Settings Gradle Template and shared Gradle scripts (Unity 6+).
    /// </summary>
    [InitializeOnLoad]
    public class AndroidBuildPreProcessor : IPreprocessBuildWithReport, IActiveBuildTargetChanged
    {
        private const string NextGenLibrary = "com.google.android.libraries.ads.mobile.sdk:ads-mobile-sdk";
        private const string CurrentLibrary = "com.google.android.gms:play-services-ads";
        private const string LatestNextGenVersion = "1.4.0";
        private const string CurrentVersion = "25.4.0";

        private const string NextGenSpec = NextGenLibrary + ":" + LatestNextGenVersion;
        private const string CurrentSpec = CurrentLibrary + ":" + CurrentVersion;

        private static readonly string NextGenRegex =
            Regex.Escape(NextGenLibrary) + @":(?:[\d\.]+[-a-zA-Z0-9]*|LATEST)";
        private static readonly string CurrentRegex =
            Regex.Escape(CurrentLibrary) + @":(?:[\d\.]+[-a-zA-Z0-9]*|LATEST)";

        const int StandardMinimumAPILevel = 23;
        const int NextGenMinimumAPILevel = 24;

        const string CustomGradlePropertiesTemplatesFileName = "gradleTemplate.properties";
        const string CustomMainGradleTemplateFileName = "mainTemplate.gradle";
        const string CustomSettingsGradleTemplateFileName = "settingsTemplate.gradle";
        const string CustomSharedGradleTemplateDirectoryName = "shared";
        const string JetifierEntry =
            "android.jetifier.ignorelist=annotation-experimental-1.4.0.aar";

        static AndroidBuildPreProcessor()
        {
            try
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                {
                    string androidPluginsDir = Path.Combine(
                        Application.dataPath, "Plugins", "Android");
                    if (!Directory.Exists(androidPluginsDir))
                    {
                        Directory.CreateDirectory(androidPluginsDir);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to pre-create Android plugins directory: {e.Message}");
            }

            EditorApplication.delayCall += InitializeOnLoadOrTargetSwitch;
        }

        /// <summary>
        /// Callback order set to run before External Dependency Manager for Unity (EDM4U).
        /// </summary>
        /// <see cref="https://github.com/googlesamples/unity-jar-resolver/blob/master/source/AndroidResolver/src/PlayServicesPreBuild.cs#L39"/>
        public int callbackOrder { get { return -1; } }

        /// <summary>
        /// Invoked when the active build target changes in the Unity Editor.
        /// </summary>
        /// <param name="previousTarget">The previous active build target.</param>
        /// <param name="newTarget">The newly selected active build target.</param>
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            if (newTarget == BuildTarget.Android)
            {
                InitializeOnLoadOrTargetSwitch();
            }
        }

        private static void InitializeOnLoadOrTargetSwitch()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android ||
                EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            if (string.IsNullOrEmpty(PlayServicesResolver.AndroidPlaybackEngineDirectory))
            {
                return;
            }

            UpdateGmaDependency(isBuild: false);

            if (!GoogleMobileAdsSettings.LoadInstance().EnableGradleBuildPreProcessor)
            {
                return;
            }

#if ANDROID_GRADLE_BUILD_PRE_PROCESSOR_ENABLED
            ApplyBuildSettings(isBuild: false);
#endif
        }

        /// <summary>
        /// Invoked before an Android build starts to verify and configure build settings.
        /// </summary>
        /// <param name="report">Build report containing information about the current build.</param>
        public void OnPreprocessBuild(BuildReport report)
        {
            UpdateGmaDependency(isBuild: true);

            if (!GoogleMobileAdsSettings.LoadInstance().EnableGradleBuildPreProcessor)
            {
                return;
            }

            // For more details, see https://developers.google.com/admob/unity/android.
#if ANDROID_GRADLE_BUILD_PRE_PROCESSOR_ENABLED
            ApplyBuildSettings(isBuild: true);
#endif
        }

        private static void ApplyBuildSettings(bool isBuild)
        {
            if (isBuild)
            {
                Debug.Log("Running Android Gradle Build Pre-Processor.");
            }

            var sdk = GoogleMobileAdsSettings.LoadInstance().EffectiveGmaAndroidSdk;
            int targetMinApi = (sdk == GoogleMobileAdsSettings.GmaAndroidSdk.NextGen)
                                   ? NextGenMinimumAPILevel
                                   : StandardMinimumAPILevel;
            if (PlayerSettings.Android.minSdkVersion < (AndroidSdkVersions)targetMinApi)
            {
                PlayerSettings.Android.minSdkVersion = (AndroidSdkVersions)targetMinApi;
                Debug.Log($"Set minimum API Level to: {targetMinApi}.");
            }
            else if (isBuild)
            {
                Debug.Log($"Verified Minimum API Level is >= {targetMinApi}.");
            }

            string pluginsPath = Path.Combine(Application.dataPath, "Plugins");
            if (!Directory.Exists(pluginsPath))
            {
                Directory.CreateDirectory(pluginsPath);
            }
            if (!AssetDatabase.IsValidFolder(Path.Combine("Assets", "Plugins")))
            {
                AssetDatabase.CreateFolder("Assets", "Plugins");
                AssetDatabase.Refresh();
            }

            string androidPluginsPath = Path.Combine(pluginsPath, "Android");
            if (!Directory.Exists(androidPluginsPath))
            {
                Directory.CreateDirectory(androidPluginsPath);
            }
            if (!AssetDatabase.IsValidFolder(Path.Combine("Assets", "Plugins", "Android")))
            {
                AssetDatabase.CreateFolder(Path.Combine("Assets", "Plugins"), "Android");
                AssetDatabase.Refresh();
            }

            bool changed = false;

            // Ensure Custom Main Gradle Template.
            changed |= EnsureGradleFileExists(
                CustomMainGradleTemplateFileName, isBuild, required: true);

            // Ensure Custom Gradle Properties Templates.
            changed |= EnsureGradleFileExists(
                CustomGradlePropertiesTemplatesFileName, isBuild, required: true);

            // Ensure Custom Settings Gradle Template.
            changed |= EnsureGradleFileExists(
                CustomSettingsGradleTemplateFileName, isBuild, required: false);

            // Ensure Custom Shared Gradle Template Directory (Unity 6000.3+).
            changed |= EnsureGradleSharedDirectoryExists();

            #if ANDROID_GRADLE_BUILD_JETIFIER_ENTRY_ENABLED
            string customGradlePropertiesTemplatesFilePath = Path.Combine(
                Application.dataPath,
                "Plugins", "Android",
                CustomGradlePropertiesTemplatesFileName);
            if (File.Exists(customGradlePropertiesTemplatesFilePath))
            {
                var gradlePropertiesFileContent =
                    File.ReadAllText(customGradlePropertiesTemplatesFilePath);
                if (!gradlePropertiesFileContent.Contains(JetifierEntry))
                {
                    File.AppendAllText(
                        customGradlePropertiesTemplatesFilePath,
                        Environment.NewLine + JetifierEntry);
                    Debug.Log("Added Jetifier Entry.");
                    changed = true;
                }
                else if (isBuild)
                {
                    Debug.Log("Verified Jetifier Entry exists.");
                }
            }
            else if (isBuild)
            {
                Debug.LogError("Failed to add Jetifier Entry.");
            }
            #endif

            if (isBuild || changed)
            {
                Debug.Log("Resolving Android Gradle dependencies.");
                PlayServicesResolver.ResolveSync(true);
                if (isBuild)
                {
                    Debug.Log("Android Build Pre-Processor finished.");
                }
            }
        }

        /// <summary>
        /// Ensures that the given Gradle file exists.
        /// </summary>
        /// <param name="fileName">Name of the given Gradle file.</param>
        /// <param name="isBuild">Whether this check is running during a build.</param>
        /// <param name="required">Whether missing source file should fail the build.</param>
        /// <returns>True if the target file was created or enabled; otherwise false.</returns>
        private static bool EnsureGradleFileExists(string fileName, bool isBuild, bool required)
        {
            bool foundTargetFile = false;
            bool foundDisabledFile = false;

            // Check for target file.
            string targetPath = Path.Combine(Application.dataPath, "Plugins", "Android", fileName);
            if (File.Exists(targetPath))
            {
                foundTargetFile = true;
            }

            // Check for the ".DISABLED" file.
            string disabledPath = Path.Combine(Application.dataPath, "Plugins", "Android",
                    $"{fileName}.DISABLED");
            if (File.Exists(disabledPath))
            {
                foundDisabledFile = true;
            }

            // If DISABLED and target exist, delete DISABLED.
            if (foundTargetFile && foundDisabledFile)
            {
                File.Delete(disabledPath);
                Debug.Log($"Removed disabled {fileName}.");
                return true;
            }
            // If DISABLED exists, move it to target.
            if (foundDisabledFile)
            {
                File.Move(disabledPath, targetPath);
                AssetDatabase.Refresh();
                Debug.Log($"Enabled {fileName}.");
                return true;
            }
            // If target exists, return false.
            if (foundTargetFile)
            {
                if (isBuild)
                {
                    Debug.Log($"Verified {fileName}.");
                }
                return false;
            }

            if (string.IsNullOrEmpty(PlayServicesResolver.AndroidPlaybackEngineDirectory))
            {
                if (required && isBuild)
                {
                    throw new BuildFailedException(
                        "Android Build Pre-Processor failed. " +
                        "Unable to locate Android Playback Engine directory.");
                }
                return false;
            }

            // If target does not exist, create it from source.
            var unityGradleTemplateDirectory = Path.Combine(
                PlayServicesResolver.AndroidPlaybackEngineDirectory,
                "Tools",
                "GradleTemplates");
            string sourceFileName = Path.Combine(unityGradleTemplateDirectory, fileName);
            if (!File.Exists(sourceFileName))
            {
                if (required && isBuild)
                {
                    throw new BuildFailedException(
                        "Android Build Pre-Processor failed. "+
                        $"Unable to find source {sourceFileName}. Is your file system read-only?" +
                        "If this issue persists, contact Google Mobile Ads Support "+
                        "at https://developers.google.com/admob/support");
                }
                return false;
            }
            File.Copy(sourceFileName, targetPath);
            AssetDatabase.Refresh();
            Debug.Log($"Created {fileName}.");
            return true;
        }

        private static bool EnsureGradleSharedDirectoryExists()
        {
            if (string.IsNullOrEmpty(PlayServicesResolver.AndroidPlaybackEngineDirectory))
            {
                return false;
            }

            string unityGradleTemplateDirectory = Path.Combine(
                PlayServicesResolver.AndroidPlaybackEngineDirectory,
                "Tools",
                "GradleTemplates");
            string sourceSharedDir = Path.Combine(
                unityGradleTemplateDirectory,
                CustomSharedGradleTemplateDirectoryName);
            if (!Directory.Exists(sourceSharedDir))
            {
                return false;
            }

            string targetSharedDir = Path.Combine(
                Application.dataPath,
                "Plugins",
                "Android",
                CustomSharedGradleTemplateDirectoryName);

            bool copied = CopyDirectoryRecursively(sourceSharedDir, targetSharedDir);
            if (copied)
            {
                AssetDatabase.Refresh();
                Debug.Log($"Created {CustomSharedGradleTemplateDirectoryName} Gradle directory.");
            }
            return copied;
        }

        private static bool CopyDirectoryRecursively(string sourceDir, string targetDir)
        {
            bool copiedAny = false;
            Directory.CreateDirectory(targetDir);
            foreach (string filePath in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(filePath);
                if (fileName.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                string destFile = Path.Combine(targetDir, fileName);
                if (!File.Exists(destFile))
                {
                    File.Copy(filePath, destFile);
                    copiedAny = true;
                }
            }
            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(subDir);
                string destSubDir = Path.Combine(targetDir, dirName);
                if (CopyDirectoryRecursively(subDir, destSubDir))
                {
                    copiedAny = true;
                }
            }
            return copiedAny;
        }

        /// <summary>
        /// Updates GoogleMobileAdsDependencies.xml with the selected GMA SDK dependency.
        /// Existing dependency versions are preserved if the desired GMA SDK is already present.
        /// If the file is modified, EDM4U is triggered to resolve dependencies.
        /// </summary>
        private static void UpdateGmaDependency(bool isBuild)
        {
            var pathUtils = ScriptableObject.CreateInstance<EditorPathUtils>();
            string directoryPath = pathUtils.GetDirectoryAssetPath();
            if (string.IsNullOrEmpty(directoryPath))
            {
                return;
            }
            string dependenciesFilePath =
                Path.Combine(directoryPath, "GoogleMobileAdsDependencies.xml");

            if (!File.Exists(dependenciesFilePath))
            {
                if (isBuild)
                {
                    Debug.LogError(
                        $"GoogleMobileAdsDependencies.xml not found at {dependenciesFilePath}");
                }
                return;
            }

            bool isNextGen = (GoogleMobileAdsSettings.LoadInstance().EffectiveGmaAndroidSdk ==
                              GoogleMobileAdsSettings.GmaAndroidSdk.NextGen);
            string fileContent = File.ReadAllText(dependenciesFilePath);

            string desiredRegex = isNextGen ? NextGenRegex : CurrentRegex;
            if (Regex.IsMatch(fileContent, desiredRegex))
            {
                if (isBuild)
                {
                    Debug.Log("GoogleMobileAdsDependencies.xml already matches the desired " +
                              "Google Mobile Ads SDK.");
                }
                return;
            }

            // Identify the regex for the SDK currently in the file to be replaced
            // (e.g., if switching to Next Gen, look for the existing Standard SDK to replace).
            string targetRegex = isNextGen ? CurrentRegex : NextGenRegex;

            if (Regex.IsMatch(fileContent, targetRegex))
            {
                string desiredSpec = isNextGen ? NextGenSpec : CurrentSpec;
                string newContent = Regex.Replace(fileContent, targetRegex, desiredSpec);
                Debug.Log($"Updating GoogleMobileAdsDependencies.xml with {desiredSpec}");
                File.WriteAllText(dependenciesFilePath, newContent);
                AssetDatabase.Refresh();
                PlayServicesResolver.ResolveSync(true);
            }
            else if (isBuild)
            {
                Debug.LogWarning(
                    "Could not find existing Google Mobile Ads SDK dependency in " +
                    "GoogleMobileAdsDependencies.xml to replace.");
            }
        }
    }
}
#endif
