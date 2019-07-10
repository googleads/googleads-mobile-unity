#if UNITY_IPHONE || UNITY_IOS

using System.IO;

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

using GoogleMobileAds.Editor;

public static class PListProcessor
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
#if UNITY_IPHONE || UNITY_IOS
        string plistPath = Path.Combine(path, "Info.plist");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        if (GoogleMobileAdsSettings.Instance.IsAdManagerEnabled)
        {
            plist.root.SetBoolean("GADIsAdManagerApp", true);
        }

        if (GoogleMobileAdsSettings.Instance.IsAdMobEnabled)
        {
            string appId = GoogleMobileAdsSettings.Instance.AdMobIOSAppId;
            if (appId.Length == 0)
            {
                Debug.LogError("iOS AdMob app ID is empty. Please enter a valid app ID to run ads properly.");
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
#endif
    }
}

#endif
