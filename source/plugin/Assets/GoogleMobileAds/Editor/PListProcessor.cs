#if UNITY_IPHONE || UNITY_IOS
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

using GoogleMobileAds.Editor;

public static class PListProcessor
{
    private const string KEY_SK_ADNETWORK_ITEMS = "SKAdNetworkItems";

    private const string KEY_SK_ADNETWORK_ID = "SKAdNetworkIdentifier";

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

        List<string> skNetworkIds = ReadSKAdNetworkIdentifiersFromXML();
        if (skNetworkIds.Count > 0)
        {
            AddSKAdNetworkIdentifier(plist, skNetworkIds);
        }

        File.WriteAllText(plistPath, plist.WriteToString());
    }

    private static PlistElementArray GetSKAdNetworkItemsArray(PlistDocument document)
    {
        PlistElementArray array;
        if (document.root.values.ContainsKey(KEY_SK_ADNETWORK_ITEMS))
        {
            try
            {
                PlistElement element;
                document.root.values.TryGetValue(KEY_SK_ADNETWORK_ITEMS, out element);
                array = element.AsArray();
            }
#pragma warning disable 0168
            catch (Exception e)
#pragma warning restore 0168
            {
                // The element is not an array type.
                array = null;
            }
        }
        else
        {
            array = document.root.CreateArray(KEY_SK_ADNETWORK_ITEMS);
        }
        return array;
    }

    private static List<string> ReadSKAdNetworkIdentifiersFromXML()
    {
        List<string> skAdNetworkItems = new List<string>();

        string path = Path.Combine(Application.dataPath,
            "GoogleMobileAds/Editor/GoogleMobileAdsSKAdNetworkItems.xml");

        try
        {
            if (!File.Exists(path))
            {
                throw new IOException();
            }
            using (FileStream fs = File.OpenRead(path))
            {
                XmlDocument document = new XmlDocument();
                document.Load(fs);

                XmlNode root = document.FirstChild;

                XmlNodeList nodes = root.SelectNodes(KEY_SK_ADNETWORK_ID);
                foreach (XmlNode node in nodes)
                {
                    skAdNetworkItems.Add(node.InnerText);
                }
            }
        }
        catch (IOException e)
        {
            NotifyBuildFailure("Failed to read GoogleMobileAdsSKAdNetworkIds.xml: " + e.Message, false);
        }

        return skAdNetworkItems;
    }

    private static void AddSKAdNetworkIdentifier(PlistDocument document, List<string> skAdNetworkIds)
    {
        PlistElementArray array = GetSKAdNetworkItemsArray(document);
        if (array != null)
        {
            foreach (string id in skAdNetworkIds)
            {
                if (!ContainsSKAdNetworkIdentifier(array, id))
                {
                    PlistElementDict added = array.AddDict();
                    added.SetString(KEY_SK_ADNETWORK_ID, id);
                }
            }
        }
        else
        {
            NotifyBuildFailure("SKAdNetworkItems element already exists in Info.plist, but is not an array.", false);
        }
    }

    private static bool ContainsSKAdNetworkIdentifier(PlistElementArray skAdNetworkItemsArray, string id)
    {
        foreach (PlistElement elem in skAdNetworkItemsArray.values)
        {
            try
            {
                PlistElementDict elemInDict = elem.AsDict();
                PlistElement value;
                bool identifierExists = elemInDict.values.TryGetValue(KEY_SK_ADNETWORK_ID, out value);

                if (identifierExists && value.AsString().Equals(id))
                {
                    return true;
                }
            }
#pragma warning disable 0168
                catch (Exception e)
#pragma warning restore 0168
            {
                // Do nothing
            }
        }

        return false;
    }

    private static void NotifyBuildFailure(string message, bool showOpenSettingsButton = true)
    {
        string dialogTitle = "Google Mobile Ads";
        string dialogMessage = "Error: " + message;

        if (showOpenSettingsButton)
        {
            bool openSettings = EditorUtility.DisplayDialog(
                dialogTitle, dialogMessage, "Open Settings", "Close");
            if (openSettings)
            {
                GoogleMobileAdsSettingsEditor.OpenInspector();
            }
        }
        else
        {
            EditorUtility.DisplayDialog(dialogTitle, dialogMessage, "Close");
        }

        ThrowBuildException("[GoogleMobileAds] " + message);
    }

    private static void ThrowBuildException(string message)
    {
#if UNITY_2017_1_OR_NEWER
        throw new BuildPlayerWindow.BuildMethodException(message);
#else
        throw new OperationCanceledException(message);
#endif
    }
}

#endif
