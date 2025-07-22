#if UNITY_ANDROID
using System;
using UnityEditor;
using UnityEditor.Callbacks;

using GoogleMobileAds.Editor;

public static class AndroidBuildPostProcessor
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (GoogleMobileAdsSettings.LoadInstance().GoogleMobileAdsAndroidAppId.Length == 0)
        {
            NotifyBuildFailure(
                "Android Google Mobile Ads app ID is empty. Please enter a valid app ID to run ads properly.");
        }
        if (GoogleMobileAdsSettings.LoadInstance().EnableAndroidLifecycleDependency)
        {
            AddAndroidLifecycleDependency();
        }
    }

    private static void NotifyBuildFailure(string message)
    {
        string prefix = "[GoogleMobileAds] ";

        bool openSettings = EditorUtility.DisplayDialog(
            "Google Mobile Ads", "Error: " + message, "Open Settings", "Close");
        if (openSettings)
        {
            GoogleMobileAdsSettingsEditor.OpenInspector();
        }
#if UNITY_2017_1_OR_NEWER
        throw new BuildPlayerWindow.BuildMethodException(prefix + message);
#else
        throw new OperationCanceledException(prefix + message);
#endif
    }

    private static void AddAndroidLifecycleDependency()
    {
        // Create Assets/Plugins folder.
        if (!AssetDatabase.IsValidFolder("Assets/GoogleMobileAds"))
        {
            AssetDatabase.CreateFolder("Assets", "GoogleMobileAds");
            AssetDatabase.Refresh();
        }

        // Create Assets/Plugins/Android folder.
        if (!AssetDatabase.IsValidFolder("Assets/GoogleMobileAds/Editor"))
        {
            AssetDatabase.CreateFolder("Assets/GoogleMobileAds", "Editor");
            AssetDatabase.Refresh();
        }

        // Check for target file.
        string targetPath = Path.Combine(Application.dataPath,
            $"GoogleMobileAds/Editor/LifecycleDependencies.xml");

        if (File.Exists(targetPath))
        {
            Debug.Log($"Verified LifecycleDependencies.xml exists.");
            return;
        }

        // Use StringBuilder to construct the XML content
        var lifecycleDependency = "androidx.lifecycle:lifecycle-process:2.9.2";
        var xmlContent =
            "<dependencies>" +
            "  <androidPackages>" +
            $"    <androidPackage spec=\"{lifecycleDependency}\">" +
            "      <repositories>" +
            "        <repository>https://maven.google.com/</repository>" +
            "      </repositories>" +
            "    </androidPackage>" +
            "  </androidPackages>" +
            "</dependencies>";

        // Create target file.
        File.WriteAllText(targetPath, xmlContent);
        AssetDatabase.Refresh();
        Debug.Log($"Created LifecycleDependencies.xml.");
    }
}

#endif
