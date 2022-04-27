using System.IO;
using UnityEditor;
using UnityEngine;

namespace GoogleMobileAds.Editor
{
    internal class GoogleMobileAdsSettings : ScriptableObject
    {
        private const string MobileAdsSettingsResDir = "Assets/GoogleMobileAds/Resources";

        private const string MobileAdsSettingsFile = "GoogleMobileAdsSettings";

        private const string MobileAdsSettingsFileExtension = ".asset";

        internal static GoogleMobileAdsSettings LoadInstance()
        {
            //Read from resources.
            var instance = Resources.Load<GoogleMobileAdsSettings>(MobileAdsSettingsFile);

            //Create instance if null.
            if (instance == null)
            {
                Directory.CreateDirectory(MobileAdsSettingsResDir);
                instance = ScriptableObject.CreateInstance<GoogleMobileAdsSettings>();
                string assetPath = Path.Combine(
                    MobileAdsSettingsResDir,
                    MobileAdsSettingsFile + MobileAdsSettingsFileExtension);
                AssetDatabase.CreateAsset(instance, assetPath);
                AssetDatabase.SaveAssets();
            }

            return instance;
        }


        [SerializeField]
        private string adMobAndroidAppId = string.Empty;

        [SerializeField]
        private string adMobIOSAppId = string.Empty;

        [SerializeField]
        private bool delayAppMeasurementInit;

        public string GoogleMobileAdsAndroidAppId
        {
            get { return Instance.adMobAndroidAppId; }

            set
            {
                Instance.adMobAndroidAppId = value;
                EditorUtility.SetDirty(Instance);
            }
        }

        public string GoogleMobileAdsIOSAppId
        {
            get { return Instance.adMobIOSAppId; }

            set
            {
                Instance.adMobIOSAppId = value;
                EditorUtility.SetDirty(Instance);
            }
        }

        public bool DelayAppMeasurementInit
        {
            get { return Instance.delayAppMeasurementInit; }

            set
            {
                Instance.delayAppMeasurementInit = value;
                EditorUtility.SetDirty(Instance);
            }
        }
    }
}
