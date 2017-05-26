using UnityEngine;

// Example script showing how you can easily call into the GoogleMobileAdsPlugin.
public class GoogleMobileAdsDemoScript : MonoBehaviour {

    // Use these editor properties to test the various options
    public bool AdsEnabled;
    public bool BannerAtTop;
    public bool TestAds;
    public string AndroidAdUnitID;
    public string IOSAdUnitID;

    void Start()
    {
		// Show a warning when running in the editor. Our Ad libraries require
		// running on a physical device.
		if (Application.isEditor)
		{
			Debug.LogWarning("Ads cannot be displayed in the editor, you must run on an Android or iOS device");
			AdsEnabled = false;
		}

        // Do nothing if we don't want ads.
        if (AdsEnabled == false)
        {
            this.enabled = false;
            return;
        }

        // Determine which ad space identifier to use, based on the platform.
        string adUnitID;
#if UNITY_IOS
        adUnitID = IOSAdUnitID;
#elif UNITY_ANDROID
        adUnitID = AndroidAdUnitID;
#else
        adUnitID = null;
#endif

        if (string.IsNullOrEmpty(adUnitID))
        {
            Debug.LogError("You must define a valid Ad Unity ID in order to use the GoogleMobileAds plugin!");
        }
        else
        {
            GoogleMobileAdsPlugin.CreateBannerView(adUnitID,
                                                   GoogleMobileAdsPlugin.AdSize.SmartBanner,
                                                   BannerAtTop);
            Debug.Log("Created Banner View");
            
            GoogleMobileAdsPlugin.RequestBannerAd(TestAds);
            Debug.Log("Requested Banner Ad");
        }
    }

    void OnEnable()
    {
        Debug.Log("Registering for AdMob Events");
        GoogleMobileAdsPlugin.ReceivedAd += HandleReceivedAd;
        GoogleMobileAdsPlugin.FailedToReceiveAd += HandleFailedToReceiveAd;
        GoogleMobileAdsPlugin.ShowingOverlay += HandleShowingOverlay;
        GoogleMobileAdsPlugin.DismissingOverlay += HandleDismissingOverlay;
        GoogleMobileAdsPlugin.DismissedOverlay += HandleDismissedOverlay;
        GoogleMobileAdsPlugin.LeavingApplication += HandleLeavingApplication;
    }

    void OnDisable()
    {
        Debug.Log("Unregistering for AdMob Events");
        GoogleMobileAdsPlugin.ReceivedAd -= HandleReceivedAd;
        GoogleMobileAdsPlugin.FailedToReceiveAd -= HandleFailedToReceiveAd;
        GoogleMobileAdsPlugin.ShowingOverlay -= HandleShowingOverlay;
        GoogleMobileAdsPlugin.DismissingOverlay -= HandleDismissingOverlay;
        GoogleMobileAdsPlugin.DismissedOverlay -= HandleDismissedOverlay;
        GoogleMobileAdsPlugin.LeavingApplication -= HandleLeavingApplication;
    }

    public void HandleReceivedAd()
    {
        Debug.Log("HandleReceivedAd event received");
    }

    public void HandleFailedToReceiveAd(string message)
    {
        Debug.Log("HandleFailedToReceiveAd event received with message:");
        Debug.Log(message);
    }

    public void HandleShowingOverlay()
    {
        Debug.Log("HandleShowingOverlay event received");
    }

    public void HandleDismissingOverlay()
    {
        Debug.Log("HandleDismissingOverlay event received");
    }

    public void HandleDismissedOverlay()
    {
        Debug.Log("HandleDismissedOverlay event received");
    }

    public void HandleLeavingApplication()
    {
        Debug.Log("HandleLeavingApplication event received");
    }
}

