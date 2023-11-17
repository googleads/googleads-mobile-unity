using System;
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
                Version agp = Version.Parse(Utils.AndroidGradlePluginVersion);
                instance.validateGradleDependencies = true;
                // Turn on Gradle Dependency Validation if AGP < 4.2.2
                if (agp.Major > 4 || (agp.Major == 4 && agp.Minor >= 2 && agp.Build >= 2))
                {
                    instance.validateGradleDependencies = false;
                }
            }

            return instance;
        }

        [SerializeField]
        private string adMobAndroidAppId = string.Empty;

        [SerializeField]
        private string adMobIOSAppId = string.Empty;

        [SerializeField]
        private bool delayAppMeasurementInit;

        [SerializeField]
        private bool enableKotlinXCoroutinesPackagingOption = true;

        [SerializeField]
        private bool optimizeInitialization;

        [SerializeField]
        private bool optimizeAdLoading;

        [SerializeField]
        private string userTrackingUsageDescription;

        [SerializeField]
        private bool validateGradleDependencies;

        public string GoogleMobileAdsAndroidAppId
        {
            get { return adMobAndroidAppId; }

            set { adMobAndroidAppId = value; }
        }

        public bool EnableKotlinXCoroutinesPackagingOption
        {
            get { return enableKotlinXCoroutinesPackagingOption; }

            set { enableKotlinXCoroutinesPackagingOption = value; }
        }

        public string GoogleMobileAdsIOSAppId
        {
            get { return adMobIOSAppId; }

            set { adMobIOSAppId = value; }
        }

        public bool DelayAppMeasurementInit
        {
            get { return delayAppMeasurementInit; }

            set { delayAppMeasurementInit = value; }
        }

        public bool OptimizeInitialization
        {
            get { return optimizeInitialization; }

            set { optimizeInitialization = value; }
        }

        public bool OptimizeAdLoading
        {
            get { return optimizeAdLoading; }

            set { optimizeAdLoading = value; }
        }

        public string UserTrackingUsageDescription
        {
            get { return userTrackingUsageDescription; }

            set { userTrackingUsageDescription = value; }
        }

        public bool ValidateGradleDependencies
        {
            get { return validateGradleDependencies; }

            set { validateGradleDependencies = value; }
        }
    }
}
