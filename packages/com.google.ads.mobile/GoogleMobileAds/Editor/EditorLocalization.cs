using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GoogleMobileAds.Editor
{
  public class EditorLocalization
  {
    private const string LOCALIZATION_DATA_JSON_RELATIVE_PATH = "GoogleMobileAds/Editor";
    private const string LOCALIZATION_DATA_JSON_FILENAME =
      "gma_settings_editor_localization_data.json";
    private const string LOCALIZATIONS_JSON_KEY = "LocalizationsByKey";
    private const string LOCALIZATION_KEY_PREFIX = "KEY_";

    private readonly Lazy<EditorLocalizationData> _localizationData =
      new Lazy<EditorLocalizationData>(() => InitLocalizationDataOrThrow());
    private EditorLocalizationData GetLocalizationData() => _localizationData.Value;

    /**
     * Gets the default language for the settings editor.
     * We assume the default locale used belong to the list of supported cultures
     * (https://www.csharp-examples.net/culture-names/), and that each key has a default
     * localization provided.
     */
    public string GetDefaultLanguage()
    {
      return "en"; // English
    }

    /**
     * Checks that a localization key exists.
     */
    public bool HasKey(string key)
    {
      return GetLocalizationData().LocalizationsByKey.ContainsKey(key);
    }

    /**
     * Localizes a resource key based on a provided user language.
     * Returns the key name if the key could not be localized.
     */
    public string ForKey(string key)
    {
      key = key.ToUpper();
      // Accept both key syntaxes.
      if (key.StartsWith(LOCALIZATION_KEY_PREFIX))
          key = key.Replace(LOCALIZATION_KEY_PREFIX, "");

      if (GetLocalizationData().LocalizationsByKey.TryGetValue(key,
          out Dictionary<string, string> localizations))
      {
          // Key was found. Try to localize the key with the user language (e.g., "en" or "fr").
          // Else, use the default (fallback) language, if the localization key is missing for
          // the chosen language (or no language was selected).
          // The region is omitted purposely as we don't currently require this level of details.
          string userLanguage = GoogleMobileAdsSettings.LoadInstance().UserLanguage;
          if (localizations == null)
          {
            return null;
          }
          bool userLanguageExists = localizations.TryGetValue(userLanguage,
                                                              out string userLocalization);
          bool userLocalizationIsValid = userLanguageExists &&
              !string.IsNullOrEmpty(userLocalization);
          return userLocalizationIsValid ? userLocalization: localizations[GetDefaultLanguage()];
      }

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
      string localizationDataPath =
        Path.Combine(Application.dataPath, LOCALIZATION_DATA_JSON_RELATIVE_PATH,
          LOCALIZATION_DATA_JSON_FILENAME);
      // Handle importing the localization data file via Unity Package Manager.
      EditorPathUtils pathUtils = ScriptableObject.CreateInstance<EditorPathUtils>();
      if (pathUtils.IsPackageRootPath())
      {
        localizationDataPath =
          Path.Combine(pathUtils.GetDirectoryAssetPath(), LOCALIZATION_DATA_JSON_FILENAME);
      }
      try
      {
        string json = File.ReadAllText(localizationDataPath);
        EditorLocalizationData data = DeserializeFromJson(json);
        if (data.LocalizationsByKey == null)
        {
          throw new ArgumentNullException("LocalizationsByKey");
        }
        return data;
      }
      catch (Exception)
      {
        throw new ArgumentException(
          $"Exception thrown while retrieving localization data from {localizationDataPath}:" +
          " {ex:full}");
      }
    }

    // We would like to handle the deserialization of the JSON file referenced above but without
    // leveraging any JSON library to avoid adding any dependency.
    private static EditorLocalizationData DeserializeFromJson(string json)
    {
      var data = new EditorLocalizationData();
      data.LocalizationsByKey = new Dictionary<string, Dictionary<string, string>>();
      // We match every field in the JSON. The order in which those matches are found is used to
      // deserialize the localization values.
      var regex = new Regex(@"""(?<val>[^""]+)""");
      var matches = regex.Matches(json);
      var currentKeys = new List<string>();
      var valueProcessed = false;
      foreach (Match match in matches)
      {
        var val = match.Groups["val"].Value;
        if (val.Equals(LOCALIZATIONS_JSON_KEY))
        {
          currentKeys.Clear();
          continue;
        }

        if (valueProcessed)
        {
          valueProcessed = false;
          if (val.StartsWith(LOCALIZATION_KEY_PREFIX))
          {
            // Start a new level.
            currentKeys.Clear();
          }
          else if (currentKeys.Count > 0)
          {
            // Go up one level by removing the latest key.
            currentKeys.RemoveAt(currentKeys.Count - 1);
          }
        }

        // The localization values are 2 levels deep.
        if (currentKeys.Count < 2)
        {
          currentKeys.Add(val);
          continue;
        }

        ProcessValue(data, currentKeys, val);
        valueProcessed = true;
      }

      return data;
    }

    private static void ProcessValue(EditorLocalizationData data, List<string> currentKeys,
                                     string val)
    {
      if (currentKeys.Count != 2)
        return;
      currentKeys[0] = currentKeys[0].Replace(LOCALIZATION_KEY_PREFIX, "");
      if (!data.LocalizationsByKey.ContainsKey(currentKeys[0]))
        data.LocalizationsByKey[currentKeys[0]] = new Dictionary<string, string>();
      data.LocalizationsByKey[currentKeys[0]][currentKeys[1]] = val;
    }
  }
}
