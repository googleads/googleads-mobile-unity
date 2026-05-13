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
    /// </summary>
    public class AndroidBuildPreProcessor : IPreprocessBuildWithReport
    {
        private const string NextGenLibrary = "com.google.android.libraries.ads.mobile.sdk:ads-mobile-sdk";
        private const string NextGenVersion = "1.0.1";
        private const string NextGenSpec = NextGenLibrary + ":" + NextGenVersion;

        private const string CurrentLibrary = "com.google.android.gms:play-services-ads";
        private const string CurrentVersion = "25.2.0";
        private const string CurrentSpec = CurrentLibrary + ":" + CurrentVersion;

        private static readonly string NextGenRegex = Regex.Escape(NextGenLibrary) + @":[\d\.]+[-a-zA-Z0-9]*";
        private static readonly string CurrentRegex = Regex.Escape(CurrentLibrary) + @":[\d\.]+[-a-zA-Z0-9]*";

        const int MinimumAPILevel = 23;
        const string CustomGradlePropertiesTemplatesFileName = "gradleTemplate.properties";
        const string CustomMainGradleTemplateFileName = "mainTemplate.gradle";
        const string JetifierEntry =
            "android.jetifier.ignorelist=annotation-experimental-1.4.0.aar";

        // Set the callback order to be before EDM4U.
        // https://github.com/googlesamples/unity-jar-resolver/blob/master/source/AndroidResolver/src/PlayServicesPreBuild.cs#L39
        public int callbackOrder { get { return -1; } }

        public void OnPreprocessBuild(BuildReport report)
        {
            UpdateGmaDependency();

            if(!GoogleMobileAdsSettings.LoadInstance().EnableGradleBuildPreProcessor)
            {
                return;
            }
            // For more details see, https://developers.google.com/admob/unity/android
#if ANDROID_GRADLE_BUILD_PRE_PROCESSOR_ENABLED
            ApplyBuildSettings(report);
#endif
        }

        private void ApplyBuildSettings(BuildReport report)
        {
            Debug.Log("Running Android Gradle Build Pre-Processor.");

            // Set Minimum Api Level.
            if (PlayerSettings.Android.minSdkVersion < (AndroidSdkVersions)MinimumAPILevel)
            {
                PlayerSettings.Android.minSdkVersion = (AndroidSdkVersions)MinimumAPILevel;
                Debug.Log($"Set minimum API Level to: {MinimumAPILevel}.");
            }
            else
            {
                Debug.Log($"Verified Minimum API Level is >= {MinimumAPILevel}.");
            }

            // Create Assets/Plugins folder.
            if (!AssetDatabase.IsValidFolder(Path.Combine("Assets", "Plugins")))
            {
                AssetDatabase.CreateFolder("Assets", "Plugins");
                AssetDatabase.Refresh();
            }

            // Create Assets/Plugins/Android folder.
            if (!AssetDatabase.IsValidFolder(Path.Combine("Assets", "Plugins", "Android")))
            {
                AssetDatabase.CreateFolder(Path.Combine("Assets", "Plugins"), "Android");
                AssetDatabase.Refresh();
            }

            // Ensure Custom Main Gradle Template.
            EnsureGradleFileExists(CustomMainGradleTemplateFileName);

            // Ensure Custom Gradle Properties Templates.
            EnsureGradleFileExists(CustomGradlePropertiesTemplatesFileName);

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
                    Debug.Log($"Added Jetifier Entry.");
                }
                else
                {
                    Debug.Log($"Verified Jetifier Entry exists.");
                }
            }
            else
            {
                Debug.LogError("Failed to add Jetifier Entry.");
            }
            #endif

            Debug.Log("Resolving Android Gradle dependencies.");
            PlayServicesResolver.ResolveSync(true);
            Debug.Log("Android Build Pre-Processor finished.");
        }

        /// <summary>
        /// Ensures that the given Gradle file exists.
        /// </summary>
        /// <param name="fileName">name of the given Gradle file.</param>
        private void EnsureGradleFileExists(string fileName)
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
                return;
            }
            // If DISABLED exists, move it to target.
            if (foundDisabledFile)
            {
                File.Move(disabledPath, targetPath);
                AssetDatabase.Refresh();
                Debug.Log($"Enabled {fileName}.");
                return;
            }
            // If target exists, return true.
            if (foundTargetFile)
            {
                Debug.Log($"Verified {fileName}.");
                return;
            }

            // If target does not exist, create it from source.
            var unityGradleTemplateDirectory = Path.Combine(
                PlayServicesResolver.AndroidPlaybackEngineDirectory,
                "Tools",
                "GradleTemplates");
            string sourceFileName = Path.Combine(unityGradleTemplateDirectory, fileName);
            if (!File.Exists(sourceFileName))
            {
                throw new BuildFailedException(
                    "Android Build Pre-Processor failed. "+
                    $"Unable to find source {sourceFileName}. Is your file system read-only?" +
                    "If this issue persists, contact Google Mobile Ads Support "+
                    "at https://developers.google.com/admob/support");
            }
            File.Copy(sourceFileName, targetPath);
            AssetDatabase.Refresh();
            Debug.Log($"Created {fileName}.");
        }

        /// <summary>
        /// Updates the GoogleMobileAdsDependencies.xml file with the selected SDK dependency.
        /// If the file is changed, the EDM4U will be triggered to resolve the dependencies.
        /// </summary>
        private void UpdateGmaDependency()
        {
            string desiredSpec = (GoogleMobileAdsSettings.LoadInstance().EffectiveGmaAndroidSdk ==
                                    GoogleMobileAdsSettings.GmaAndroidSdk.NextGen)
                                    ? NextGenSpec
                                    : CurrentSpec;

            var pathUtils = ScriptableObject.CreateInstance<EditorPathUtils>();
            string directoryPath = pathUtils.GetDirectoryAssetPath();
            string dependenciesFilePath =
                Path.Combine(directoryPath, "GoogleMobileAdsDependencies.xml");

            if (!File.Exists(dependenciesFilePath))
            {
                Debug.LogError($"GoogleMobileAdsDependencies.xml not found at {dependenciesFilePath}");
                return;
            }

            string fileContent = File.ReadAllText(dependenciesFilePath);
            string newContent = fileContent;

            if (Regex.IsMatch(fileContent, NextGenRegex))
            {
                newContent = Regex.Replace(fileContent, NextGenRegex, desiredSpec);
            }
            else if (Regex.IsMatch(fileContent, CurrentRegex))
            {
                newContent = Regex.Replace(fileContent, CurrentRegex, desiredSpec);
            }
            else
            {
                Debug.LogWarning(
                    "Could not find existing Google Mobile Ads SDK dependency in " +
                    "GoogleMobileAdsDependencies.xml to replace.");
            }

            if (newContent != fileContent)
            {
                Debug.Log($"Updating GoogleMobileAdsDependencies.xml with {desiredSpec}");
                File.WriteAllText(dependenciesFilePath, newContent);
                AssetDatabase.Refresh();
                PlayServicesResolver.ResolveSync(true);
            }
        }
    }
}
#endif
