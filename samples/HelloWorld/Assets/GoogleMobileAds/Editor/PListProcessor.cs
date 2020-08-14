#if UNITY_IPHONE || UNITY_IOS
using System;
using System.IO;

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

using GoogleMobileAds.Editor;

public static class PListProcessor
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        string plistPath = Path.Combine(path, "Info.plist");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        if (!GoogleMobileAdsSettings.Instance.IsAdManagerEnabled && !GoogleMobileAdsSettings.Instance.IsAdMobEnabled)
        {
            NotifyBuildFailure("Neither Ad Manager nor AdMob is enabled yet.");
        }

        if (GoogleMobileAdsSettings.Instance.IsAdManagerEnabled)
        {
            plist.root.SetBoolean("GADIsAdManagerApp", true);
        }

        if (GoogleMobileAdsSettings.Instance.IsAdMobEnabled)
        {
            string appId = GoogleMobileAdsSettings.Instance.AdMobIOSAppId;
            if (appId.Length == 0)
            {
                NotifyBuildFailure(
                    "iOS AdMob app ID is empty. Please enter a valid app ID to run ads properly.");
            }
            else
            {
                plist.root.SetString("GADApplicationIdentifier", appId);
            }
        }

        if (GoogleMobileAdsSettings.Instance.DelayAppMeasurementInit)
        {
            plist.root.SetBoolean("GADDelayAppMeasurementInit", true);
        }

        File.WriteAllText(plistPath, plist.WriteToString());
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
}

#endif
