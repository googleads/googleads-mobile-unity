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

#if UNITY_ANDROID
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using UnityEditor;
using UnityEditor.Build;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;
using GoogleMobileAds.Editor;

#if UNITY_2018_1_OR_NEWER
public class ManifestProcessor : IPreprocessBuildWithReport
#else
public class ManifestProcessor : IPreprocessBuild
#endif
{
    private const string MANIFEST_RELATIVE_PATH =
            "Plugins/Android/GoogleMobileAdsPlugin.androidlib/AndroidManifest.xml";

    private const string METADATA_APPLICATION_ID  =
            "com.google.android.gms.ads.APPLICATION_ID";

    private const string METADATA_DELAY_APP_MEASUREMENT_INIT =
            "com.google.android.gms.ads.DELAY_APP_MEASUREMENT_INIT";

    private const string METADATA_OPTIMIZE_INITIALIZATION =
            "com.google.android.gms.ads.flag.OPTIMIZE_INITIALIZATION";

    private const string METADATA_OPTIMIZE_AD_LOADING =
            "com.google.android.gms.ads.flag.OPTIMIZE_AD_LOADING";

    private XNamespace ns = "http://schemas.android.com/apk/res/android";

    public int callbackOrder { get { return 0; } }

#if UNITY_2018_1_OR_NEWER
    public void OnPreprocessBuild(BuildReport report)
#else
    public void OnPreprocessBuild(BuildTarget target, string path)
#endif
    {
        string manifestPath = Path.Combine(
                Application.dataPath, MANIFEST_RELATIVE_PATH);
        if (AssetDatabase.IsValidFolder("Packages/com.google.ads.mobile"))
        {
            manifestPath = Path.Combine("Packages/com.google.ads.mobile", MANIFEST_RELATIVE_PATH);
        }

        XDocument manifest = null;
        try
        {
            manifest = XDocument.Load(manifestPath);
        }
        #pragma warning disable 0168
        catch (IOException e)
        #pragma warning restore 0168
        {
            StopBuildWithMessage("AndroidManifest.xml is missing. Try re-importing the plugin.");
        }

        XElement elemManifest = manifest.Element("manifest");
        if (elemManifest == null)
        {
            StopBuildWithMessage("AndroidManifest.xml is not valid. Try re-importing the plugin.");
        }

        XElement elemApplication = elemManifest.Element("application");
        if (elemApplication == null)
        {
            StopBuildWithMessage("AndroidManifest.xml is not valid. Try re-importing the plugin.");
        }

        GoogleMobileAdsSettings instance = GoogleMobileAdsSettings.LoadInstance();
        string appId = instance.GoogleMobileAdsAndroidAppId;

        if (appId.Length == 0)
        {
            StopBuildWithMessage(
                "Android Google Mobile Ads app ID is empty. Please enter a valid app ID to run ads properly.");
        }

        IEnumerable<XElement> metas = elemApplication.Descendants()
                .Where( elem => elem.Name.LocalName.Equals("meta-data"));

        SetMetadataElement(elemApplication,
                           metas,
                           METADATA_APPLICATION_ID,
                           appId);

        SetMetadataElement(elemApplication,
                           metas,
                           METADATA_DELAY_APP_MEASUREMENT_INIT,
                           instance.DelayAppMeasurementInit);

        SetMetadataElement(elemApplication,
                           metas,
                           METADATA_OPTIMIZE_INITIALIZATION,
                           instance.OptimizeInitialization);

        SetMetadataElement(elemApplication,
                           metas,
                           METADATA_OPTIMIZE_AD_LOADING,
                           instance.OptimizeAdLoading);

        elemManifest.Save(manifestPath);
    }

    private XElement CreateMetaElement(string name, object value)
    {
        return new XElement("meta-data",
                new XAttribute(ns + "name", name), new XAttribute(ns + "value", value));
    }

    private XElement GetMetaElement(IEnumerable<XElement> metas, string metaName)
    {
        foreach (XElement elem in metas)
        {
            IEnumerable<XAttribute> attrs = elem.Attributes();
            foreach (XAttribute attr in attrs)
            {
                if (attr.Name.Namespace.Equals(ns)
                        && attr.Name.LocalName.Equals("name") && attr.Value.Equals(metaName))
                {
                    return elem;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Utility for setting a metadata element
    /// </summary>
    /// <param name="elemApplication">application element</param>
    /// <param name="metas">all metadata elements</param>
    /// <param name="metadataName">name of the element to set</param>
    /// <param name="metadataValue">value to set</param>
    private void SetMetadataElement(XElement elemApplication,
                                    IEnumerable<XElement> metas,
                                    string metadataName,
                                    string metadataValue)
    {
        XElement element = GetMetaElement(metas, metadataName);
        if (element == null)
        {
            elemApplication.Add(CreateMetaElement(metadataName, metadataValue));
        }
        else
        {
            element.SetAttributeValue(ns + "value", metadataValue);
        }
    }

    /// <summary>
    /// Utility for setting a metadata element
    /// </summary>
    /// <param name="elemApplication">application element</param>
    /// <param name="metas">all metadata elements</param>
    /// <param name="metadataName">name of the element to set</param>
    /// <param name="metadataValue">value to set</param>
    /// <param name="defaultValue">If metadataValue is default, node will be removed.</param>
    private void SetMetadataElement(XElement elemApplication,
                                    IEnumerable<XElement> metas,
                                    string metadataName,
                                    bool metadataValue,
                                    bool defaultValue = false)
    {
        XElement element = GetMetaElement(metas, metadataName);
        if (metadataValue != defaultValue)
        {
            if (element == null)
            {
                elemApplication.Add(CreateMetaElement(metadataName, metadataValue));
            }
            else
            {
                element.SetAttributeValue(ns + "value", metadataValue);
            }
        }
        else
        {
            if (element != null)
            {
                element.Remove();
            }
        }
    }

    private void StopBuildWithMessage(string message)
    {
        string prefix = "[GoogleMobileAds] ";
    #if UNITY_2017_1_OR_NEWER
        throw new BuildPlayerWindow.BuildMethodException(prefix + message);
    #else
        throw new OperationCanceledException(prefix + message);
    #endif
    }
}
#endif
