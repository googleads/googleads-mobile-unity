using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine.Assertions;

public static class PluginSettingsProvider 
{
    public static PluginSettings Read() 
    {
        string path = $"ProjectSettings/{MobileAdsPluginSettingsFile}";

        if (!File.Exists(path)) {
            PluginSettings settings = CreateDefault();
            Write(settings);
            return settings;
        }
        
        IDictionary<string, string> keyValue = new Dictionary<string, string>();

        try {
            XDocument manifest = XDocument.Load(path);

            XElement settingsElement = manifest.Element("settings");
            if (settingsElement == null) {
                throw new FileLoadException($"Missing {MobileAdsPluginSettingsFile} file.");
            }
        
            foreach (XElement element in settingsElement.Descendants()) {
                IList<XAttribute> attributes = element.Attributes().ToList();
                Assert.IsTrue(attributes.Count == 2);
                Assert.AreEqual("name", attributes[0].Name);
                Assert.AreEqual("value", attributes[1].Name);
                string key = attributes[0].Value;
                string value = attributes[1].Value;
                keyValue[key] = value;
            }
        }
        catch (Exception e) {
            throw new FileLoadException($"Error read {MobileAdsPluginSettingsFile} file. " + e.Message);
        }

        return new PluginSettings {
            mobileAdsSettingsDir = keyValue[MobileAdsSettingsDir],
            rootPluginDirectory = keyValue[RootPluginFolderDir]
        };
    }

    public static void Write(PluginSettings settings) 
    {
        XDocument doc =
            new XDocument(
                new XElement("settings",
                    new List<object> {
                        new XElement("projectSettings", new XAttribute("name", RootPluginFolderDir),
                            new XAttribute("value", settings.rootPluginDirectory)),
                        new XElement("projectSettings", new XAttribute("name", MobileAdsSettingsDir),
                            new XAttribute("value", settings.mobileAdsSettingsDir)),
                    }
                )
            );
        doc.Save($"ProjectSettings/{MobileAdsPluginSettingsFile}");
    }

    public static PluginSettings CreateDefault() 
    {
        return new PluginSettings {
            rootPluginDirectory = "Assets/GoogleMobileAds",
            mobileAdsSettingsDir = "Assets/GoogleMobileAds/Resources",
        };
    }
    
    private const string MobileAdsSettingsDir = "mobile-ads-settings-dir";
    private const string RootPluginFolderDir = "root-plugin-folder-dir";
    private const string MobileAdsPluginSettingsFile = "MobileAdsPluginSettings.xml";
}