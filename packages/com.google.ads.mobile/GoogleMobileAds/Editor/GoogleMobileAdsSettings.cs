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

    public enum GmaAndroidSdk
    {
      Standard = 0,
      NextGen = 1
    }

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
    private bool enableGradleBuildPreProcessor = true;

    [SerializeField]
    private bool disableOptimizeInitialization;

    [SerializeField]
    private bool disableOptimizeAdLoading;

    [SerializeField]
    private string userTrackingUsageDescription;

    [SerializeField]
    private string userLanguage = "en";

    [SerializeField]
    private bool overrideDefaultGmaAndroidSdk = false;

    [SerializeField]
    private int selectedGmaAndroidSdk = 0;

    public string GoogleMobileAdsAndroidAppId
    {
      get { return adMobAndroidAppId; }

      set { adMobAndroidAppId = value; }
    }

    public bool EnableGradleBuildPreProcessor
    {
      get { return enableGradleBuildPreProcessor; }

      set { enableGradleBuildPreProcessor = value; }
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

    public bool OverrideDefaultGmaAndroidSdk
    {
      get { return overrideDefaultGmaAndroidSdk; }

      set { overrideDefaultGmaAndroidSdk = value; }
    }

    public int SelectedGmaAndroidSdk
    {
      get { return selectedGmaAndroidSdk; }

      set { selectedGmaAndroidSdk = value; }
    }

    /// <summary>
    /// Returns the active GMA Android SDK architecture.
    /// This property is decoupled from the stored value to allow easily switching the default
    /// to Next-Gen in the next phase of migration for users who haven't overridden the default.
    /// </summary>
    public GmaAndroidSdk EffectiveGmaAndroidSdk
    {
      get
      {
        if (overrideDefaultGmaAndroidSdk)
        {
          return (GmaAndroidSdk)selectedGmaAndroidSdk;
        }
        return GmaAndroidSdk.Standard;
      }
    }
  }
}
