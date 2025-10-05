using System.Collections.Generic;

namespace GoogleMobileAds.Editor
{
  public class EditorLocalizationData
  {
    // First key: the localization key. Second key: the language to lookup. Value: the resulting
    // localization.
    public Dictionary<string, Dictionary<string, string>> LocalizationsByKey { get; set; }
  }
}
