// Copyright (C) 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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

    private const string SKADNETWORKS_RELATIVE_PATH = "GoogleMobileAds/Editor/GoogleMobileAdsSKAdNetworkItems.xml";

    private const string SKADNETWORKS_FILE_NAME = "GoogleMobileAdsSKAdNetworkItems.xml";

    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        string plistPath = Path.Combine(path, "Info.plist");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        GoogleMobileAdsSettings instance = GoogleMobileAdsSettings.LoadInstance();
        string appId = instance.GoogleMobileAdsIOSAppId;
        if (appId.Length == 0)
        {
            NotifyBuildFailure(
                "iOS Google Mobile Ads app ID is empty. Please enter a valid app ID to run ads properly.");
        }
        else
        {
            plist.root.SetString("GADApplicationIdentifier", appId);
        }

        string userTrackingDescription = instance.UserTrackingUsageDescription;
        if (!string.IsNullOrEmpty(userTrackingDescription))
        {
            plist.root.SetString("NSUserTrackingUsageDescription", userTrackingDescription);
        }

        if (instance.DelayAppMeasurementInit)
        {
            plist.root.SetBoolean("GADDelayAppMeasurementInit", true);
        }

        List<string> skNetworkIds = ReadSKAdNetworkIdentifiersFromXML();
        if (skNetworkIds.Count > 0)
        {
            AddSKAdNetworkIdentifier(plist, skNetworkIds);
        }

        string unityVersion = Application.unityVersion;
        if (!string.IsNullOrEmpty(unityVersion))
        {
            plist.root.SetString("GADUUnityVersion", unityVersion);
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

        string path = Path.Combine(Application.dataPath, SKADNETWORKS_RELATIVE_PATH);

        /*
         * Handle importing GMA via Unity Package Manager.
         */
        EditorPathUtils pathUtils = ScriptableObject.CreateInstance<EditorPathUtils>();
        if (pathUtils.IsPackageRootPath())
        {
            string parentDirectoryPath = pathUtils.GetDirectoryAssetPath();
            path = Path.Combine(parentDirectoryPath, SKADNETWORKS_FILE_NAME);
        }

        try
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
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
        #pragma warning disable 0168
        catch (FileNotFoundException e)
        #pragma warning restore 0168
        {
            NotifyBuildFailure("GoogleMobileAdsSKAdNetworkItems.xml not found", false);
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
