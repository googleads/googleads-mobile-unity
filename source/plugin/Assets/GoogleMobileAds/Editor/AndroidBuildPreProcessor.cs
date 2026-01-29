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
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
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
        const int MinimumAPILevel = 23;
        const string CustomGradlePropertiesTemplatesFileName = "gradleTemplate.properties";
        const string CustomMainGradleTemplateFileName = "mainTemplate.gradle";
        const string JetifierEntry =
            "android.jetifier.ignorelist=annotation-experimental-1.4.0.aar";
        const string PlayServicesAdsSDK = "com.google.android.gms:play-services-ads:24.9.0";
        const string NextGenSDK =
            "com.google.android.libraries.ads.mobile.sdk:ads-mobile-sdk:0.23.0-beta01";
        const string NextGenSDKRegex =
            "com\\.google\\.android\\.libraries\\.ads\\.mobile\\.sdk:ads-mobile-sdk:[^\"]*";
        const string PlayServicesAdsSDKRegex =
            "com\\.google\\.android\\.gms:play-services-ads:[^\"]*";

        // Set the callback order to be before EDM4U.
        // https://github.com/googlesamples/unity-jar-resolver/blob/master/source/AndroidResolver/src/PlayServicesPreBuild.cs#L39
        public int callbackOrder { get { return -1; } }

        public void OnPreprocessBuild(BuildReport report)
        {
            UpdateSDKDependencies();

            if(!GoogleMobileAdsSettings.LoadInstance().EnableGradleBuildPreProcessor)
            {
                return;
            }
            // For more details see, https://developers.google.com/admob/unity/android
#if ANDROID_GRADLE_BUILD_PRE_PROCESSOR_ENABLED
            ApplyBuildSettings(report);
#endif
        }

        private void UpdateSDKDependencies()
        {
            // Update GoogleMobileAdsDependencies.xml if Next Gen SDK is enabled.
            string dependenciesDir =
                Path.Combine(Application.dataPath, "GoogleMobileAds", "Editor");
            string projectPath = Directory.GetParent(Application.dataPath).ToString();
            string[] scripts = Directory.GetFiles(projectPath, "AndroidBuildPreProcessor.cs",
                                                  SearchOption.AllDirectories);
            if (scripts.Length > 0)
            {
                dependenciesDir = Path.GetDirectoryName(scripts[0]);
            }
            string dependenciesFilePath =
                Path.Combine(dependenciesDir, "GoogleMobileAdsDependencies.xml");
            string content = File.ReadAllText(dependenciesFilePath);

            if (File.Exists(dependenciesFilePath))
            {
                if (GoogleMobileAdsSettings.LoadInstance().EnableNextGenSDK &&
                    Regex.IsMatch(content, PlayServicesAdsSDKRegex))
                {
                    string newContent = Regex.Replace(content, PlayServicesAdsSDKRegex, NextGenSDK);
                    if (content != newContent)
                    {
                        File.WriteAllText(dependenciesFilePath, newContent);
                    }
                }
                else if (!GoogleMobileAdsSettings.LoadInstance().EnableNextGenSDK &&
                    Regex.IsMatch(content, NextGenSDKRegex))
                {
                    // Reverting from Next Gen SDK to Play Services Ads SDK.
                    string newContent = Regex.Replace(content, NextGenSDKRegex, PlayServicesAdsSDK);
                    if (content != newContent)
                    {
                        File.WriteAllText(dependenciesFilePath, newContent);
                    }
                }
                PlayServicesResolver.ResolveSync(true);
            }
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
    }
}
#endif
