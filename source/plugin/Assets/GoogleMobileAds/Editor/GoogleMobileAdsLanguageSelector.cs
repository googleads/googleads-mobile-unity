using UnityEngine;
using UnityEditor;

namespace GoogleMobileAds.Editor
{
  public class GoogleMobileAdsLanguageSelector
  {
    [MenuItem("Assets/Google Mobile Ads/Language/English")]
    static void English()
    {
      // We use the language code, as defined in the json file.
      GoogleMobileAdsSettings.LoadInstance().UserLanguage = "en";
    }

    [MenuItem("Assets/Google Mobile Ads/Language/French")]
    static void French()
    {
      // We use the language code, as defined in the json file.
      GoogleMobileAdsSettings.LoadInstance().UserLanguage = "fr";
    }
  }
}
