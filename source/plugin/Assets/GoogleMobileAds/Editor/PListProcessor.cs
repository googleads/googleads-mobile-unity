#if UNITY_IPHONE || UNITY_IOS || UNITY_TVOS

using System.IO;

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public static class PListProcessor
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        // Replace with your iOS AdMob app ID. Your AdMob App ID will look
        // similar to this sample ID: ca-app-pub-3940256099942544~1458002511
        string appId = "ADMOB_APPLICATION_ID";

        string plistPath = Path.Combine(path, "Info.plist");
        PlistDocument plist = new PlistDocument();

        plist.ReadFromFile(plistPath);
        plist.root.SetString("GADApplicationIdentifier", appId);
        File.WriteAllText(plistPath, plist.WriteToString());
    }
}

#endif
