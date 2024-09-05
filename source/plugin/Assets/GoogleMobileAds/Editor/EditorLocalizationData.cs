using System.Collections.Generic;

namespace GoogleMobileAds.Editor
{
  public class EditorLocalizationData
  {
    // Key: the displayed language option. Value: the language.
    public Dictionary<string, string> Languages { get; set; }
    // TODO: b/364890345 - We are planning to hardcode the languages in the
    // "Assets/ Google Mobile Ads/Language" menu instead.
    public string DefaultLanguage { get; set; }
    // First key: the localization key. Second key: the language to lookup. Value: the resulting
    // localization.
    public Dictionary<string, Dictionary<string, string>> LocalizationsByKey { get; set; }
  }
}
