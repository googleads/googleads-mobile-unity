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
      // Read from resources.
      var instance = Resources.Load<GoogleMobileAdsSettings>(MobileAdsSettingsFile);

      // Create instance if null.
      if (instance == null)
      {
        Directory.CreateDirectory(MobileAdsSettingsResDir);
        instance = ScriptableObject.CreateInstance<GoogleMobileAdsSettings>();
        string assetPath = Path.Combine(MobileAdsSettingsResDir,
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
    private bool enableKotlinXCoroutinesPackagingOption = true;

    [SerializeField]
    private bool disableOptimizeInitialization;

    [SerializeField]
    private bool disableOptimizeAdLoading;

    [SerializeField]
    private string userTrackingUsageDescription;

    [SerializeField]
    private string userLanguage = "en";

    [SerializeField]
    private bool defaultAllowAdStorage;

    [SerializeField]
    private bool defaultAllowAdPersonalization;

    [SerializeField]
    private bool defaultAllowAdUserData;

    [SerializeField]
    private bool defaultAllowAnalyticsStorage;

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

    public bool DisableOptimizeInitialization
    {
      get { return disableOptimizeInitialization; }

      set { disableOptimizeInitialization = value; }
    }

    public bool DisableOptimizeAdLoading
    {
      get { return disableOptimizeAdLoading; }

      set { disableOptimizeAdLoading = value; }
    }

    public string UserTrackingUsageDescription
    {
      get { return userTrackingUsageDescription; }

      set { userTrackingUsageDescription = value; }
    }

    public string UserLanguage
    {
      get { return userLanguage; }

      set { userLanguage = value; }
    }

    public bool DefaultAllowAdStorage
    {
        get { return defaultAllowAdStorage; }
        set { defaultAllowAdStorage = value; }
    }

    public bool DefaultAllowAdPersonalization
    {
        get { return defaultAllowAdPersonalization; }
        set { defaultAllowAdPersonalization = value; }
    }

    public bool DefaultAllowAdUserData
    {
        get { return defaultAllowAdUserData; }
        set { defaultAllowAdUserData = value; }
    }

    public bool DefaultAllowAnalyticsStorage
    {
        get { return defaultAllowAnalyticsStorage; }
        set { defaultAllowAnalyticsStorage = value; }
    }
  }
}
