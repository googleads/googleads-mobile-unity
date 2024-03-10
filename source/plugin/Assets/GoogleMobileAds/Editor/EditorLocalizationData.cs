using System.Collections.Generic;

namespace GoogleMobileAds.Editor
{
    public class EditorLocalizationData
    {
        // Key: the displayed language option. Value: the locale.
        public Dictionary<string, string> Languages { get; set; }
        public string DefaultLanguage { get; set; }
        // First key: the localization key. Second key: the locale to lookup. Value: the resulting
        // localization.
        public Dictionary<string, Dictionary<string, string>> LocalizationsByKey { get; set; }
    }
}
