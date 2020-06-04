#if UNITY_ANDROID
using System;
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
    private const string META_AD_MANAGER_APP = "com.google.android.gms.ads.AD_MANAGER_APP";

    private const string META_APPLICATION_ID  = "com.google.android.gms.ads.APPLICATION_ID";

    private const string META_DELAY_APP_MEASUREMENT_INIT =
            "com.google.android.gms.ads.DELAY_APP_MEASUREMENT_INIT";

    private XNamespace ns = "http://schemas.android.com/apk/res/android";

    public int callbackOrder { get { return 0; } }

#if UNITY_2018_1_OR_NEWER
    public void OnPreprocessBuild(BuildReport report)
#else
    public void OnPreprocessBuild(BuildTarget target, string path)
#endif
    {
        string manifestPath = Path.Combine(
                Application.dataPath, "Plugins/Android/GoogleMobileAdsPlugin.androidlib/AndroidManifest.xml");

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

        if (!GoogleMobileAdsSettings.Instance.IsAdManagerEnabled && !GoogleMobileAdsSettings.Instance.IsAdMobEnabled)
        {
            GoogleMobileAdsSettingsEditor.OpenInspector();
            StopBuildWithMessage("Neither Ad Manager nor AdMob is enabled yet.");
        }

        IEnumerable<XElement> metas = elemApplication.Descendants()
                .Where( elem => elem.Name.LocalName.Equals("meta-data"));

        XElement elemAdManagerEnabled = GetMetaElement(metas, META_AD_MANAGER_APP);
        if (GoogleMobileAdsSettings.Instance.IsAdManagerEnabled)
        {
            if (elemAdManagerEnabled == null)
            {
                elemApplication.Add(CreateMetaElement(META_AD_MANAGER_APP, true));
            }
            else
            {
                elemAdManagerEnabled.SetAttributeValue(ns + "value", true);
            }
        }
        else
        {
            if (elemAdManagerEnabled != null)
            {
                elemAdManagerEnabled.Remove();
            }
        }

        XElement elemAdMobEnabled = GetMetaElement(metas, META_APPLICATION_ID);
        if (GoogleMobileAdsSettings.Instance.IsAdMobEnabled)
        {
            string appId = GoogleMobileAdsSettings.Instance.AdMobAndroidAppId;

            if (appId.Length == 0)
            {
                StopBuildWithMessage(
                    "Android AdMob app ID is empty. Please enter a valid app ID to run ads properly.");
            }

            if (elemAdMobEnabled == null)
            {
                elemApplication.Add(CreateMetaElement(META_APPLICATION_ID, appId));
            }
            else
            {
                elemAdMobEnabled.SetAttributeValue(ns + "value", appId);
            }
        }
        else
        {
            if (elemAdMobEnabled != null)
            {
                elemAdMobEnabled.Remove();
            }
        }

        XElement elemDelayAppMeasurementInit =
                GetMetaElement(metas, META_DELAY_APP_MEASUREMENT_INIT);
        if (GoogleMobileAdsSettings.Instance.DelayAppMeasurementInit)
        {
            if (elemDelayAppMeasurementInit == null)
            {
                elemApplication.Add(CreateMetaElement(META_DELAY_APP_MEASUREMENT_INIT, true));
            }
            else
            {
                elemDelayAppMeasurementInit.SetAttributeValue(ns + "value", true);
            }
        }
        else
        {
            if (elemDelayAppMeasurementInit != null)
            {
                elemDelayAppMeasurementInit.Remove();
            }
        }

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
