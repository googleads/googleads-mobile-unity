using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

using Newtonsoft.Json;

namespace GoogleMobileAds.Editor
{
    public class EditorLocalization
    {
        private const string LOCALIZATION_DATA_JSON_RELATIVE_PATH = "GoogleMobileAds/Editor";
        private const string LOCALIZATION_DATA_JSON_FILENAME = "gma_settings_editor_localization_data.json";

        private readonly Lazy<EditorLocalizationData> _localizationData = new(() => InitLocalizationDataOrThrow());
        private EditorLocalizationData GetLocalizationData() => _localizationData.Value;

        /**
        * Defines the available languages that should be listed in the settings editor drop-down list.
        */
        public Dictionary<string, string> GetLanguages()
        {
            return GetLocalizationData().Languages;
        }

        /**
        * Gets the default language for the settings editor.
        * We assume the default locale used belong to the list of supported cultures
        * (https://www.csharp-examples.net/culture-names/), and that each key has a default
        * localization provided.
        * Note: As more language packs get released in Unity
        * (https://screenshot.googleplex.com/8ovypc6xvhTPm2s), we should preferably use the Unity
        * Hub user language instead, and only use the fallback language below as last resort if the
        * key is missing in the user language. Then, we should get rid of the UI drop-down list.
        */
        public string GetDefaultLanguage()
        {
            return GetLocalizationData().DefaultLanguage;
        }

        /**
        * Localizes a resource key based on a provided user locale (originating from a ddl field in the
        * view).
        * Returns the key name if the key could not be localized.
        */
        public string ForKey(string key)
        {
#nullable enable
            if (GetLocalizationData().LocalizationsByKey.TryGetValue(key, out Dictionary<string, string>? localizations))
            {
                // Key was found. Try to localize the key with the user locale (e.g., "en" or "fr").
                // Else, use the default (fallback) language, if the localization key is missing for
                // the chosen locale (or no locale was selected).
                string userLocale = GoogleMobileAdsSettings.LoadInstance().UserLocale;
                return localizations.TryGetValue(userLocale, out string? localization) && !String.IsNullOrEmpty(localization) ? localization : localizations[GetDefaultLanguage()];
            }
#nullable disable
            // Error, key not found, no localization to return so let's fallback to the key name
            // to provide some sort of indication in the UI.
            Debug.LogError($"Localization key not found: {key}.");
            return key;
        }

        /**
        * Deserializes the localization data, encoded in json.
        * Returns the json deserialized to a EditorLocalizationData class instance.
        * Throws an ArgumentException if the json file cannot be deserialized.
        */
        private static EditorLocalizationData InitLocalizationDataOrThrow()
        {
            string localizationDataPath = Path.Combine(Application.dataPath, LOCALIZATION_DATA_JSON_RELATIVE_PATH, LOCALIZATION_DATA_JSON_FILENAME);
            // Handle importing the localization data file via Unity Package Manager.
            EditorPathUtils pathUtils = ScriptableObject.CreateInstance<EditorPathUtils>();
            if (pathUtils.IsPackageRootPath())
            {
                localizationDataPath = Path.Combine(pathUtils.GetDirectoryAssetPath(), LOCALIZATION_DATA_JSON_FILENAME);
            }
            try
            {
                string jsonString = File.ReadAllText(localizationDataPath);
                EditorLocalizationData data = JsonConvert.DeserializeObject<EditorLocalizationData>(jsonString);
                if (data.LocalizationsByKey == null) {
                    throw new ArgumentNullException("localizationsByKey");
                }
                return data;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Exception thrown while retrieving localization data from {localizationDataPath}: {ex:full}");
            }
        }
    }
}
