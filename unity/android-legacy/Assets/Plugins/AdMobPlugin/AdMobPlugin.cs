using System;
using UnityEngine;

// The AdMob Plugin used to call into the AdMob Android Unity library.
public class AdMobPlugin : MonoBehaviour {

    // Defines string values for supported ad sizes.
    public class AdSize
    {
        private string adSize;
        private AdSize(string value)
        {
            this.adSize = value;
        }

        public override string ToString()
        {
            return adSize;
        }

        public static AdSize Banner = new AdSize("BANNER");
        public static AdSize MediumRectangle = new AdSize("IAB_MRECT");
        public static AdSize IABBanner = new AdSize("IAB_BANNER");
        public static AdSize Leaderboard = new AdSize("IAB_LEADERBOARD");
        public static AdSize SmartBanner = new AdSize("SMART_BANNER");
    }

    // These are the ad callback events that can be hooked into.
    public static event Action ReceivedAd = delegate {};
    public static event Action<string> FailedToReceiveAd = delegate {};
    public static event Action ShowingOverlay = delegate {};
    public static event Action DismissedOverlay = delegate {};
    public static event Action LeavingApplication = delegate {};

    void Awake()
    {
        gameObject.name = this.GetType().ToString();
        SetCallbackHandlerName(gameObject.name);
        DontDestroyOnLoad(this);
    }

    // Create a banner view and add it into the view hierarchy.
    public static void CreateBannerView(string publisherId, AdSize adSize, bool positionAtTop)
    {
        AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass pluginClass = new AndroidJavaClass("com.google.unity.AdMobPlugin");
        pluginClass.CallStatic("createBannerView",
            new object[4] {activity, publisherId, adSize.ToString(), positionAtTop});
    }

    // Request a new ad for the banner view without any extras.
    public static void RequestBannerAd(bool isTesting)
    {
        AndroidJavaClass pluginClass = new AndroidJavaClass("com.google.unity.AdMobPlugin");
        pluginClass.CallStatic("requestBannerAd", new object[1] {isTesting});
    }

    // Request a new ad for the banner view with extras.
    public static void RequestBannerAd(bool isTesting, string extras)
    {
        AndroidJavaClass pluginClass = new AndroidJavaClass("com.google.unity.AdMobPlugin");
        pluginClass.CallStatic("requestBannerAd", new object[2] {isTesting, extras});
    }

    // Set the name of the callback handler so the right component gets ad callbacks.
    public static void SetCallbackHandlerName(string callbackHandlerName)
    {
        AndroidJavaClass pluginClass = new AndroidJavaClass("com.google.unity.AdMobPlugin");
        pluginClass.CallStatic("setCallbackHandlerName", new object[1] {callbackHandlerName});
    }

    // Hide the banner view from the screen.
    public static void HideBannerView()
    {
        AndroidJavaClass pluginClass = new AndroidJavaClass("com.google.unity.AdMobPlugin");
        pluginClass.CallStatic("hideBannerView");
    }

    // Show the banner view on the screen.
    public static void ShowBannerView() {
        AndroidJavaClass pluginClass = new AndroidJavaClass("com.google.unity.AdMobPlugin");
        pluginClass.CallStatic("showBannerView");
    }

    public void OnReceiveAd(string unusedMessage)
    {
        ReceivedAd();
    }

    public void OnFailedToReceiveAd(string message)
    {
        FailedToReceiveAd(message);
    }

    public void OnPresentScreen(string unusedMessage)
    {
        ShowingOverlay();
    }

    public void OnDismissScreen(string unusedMessage)
    {
        DismissedOverlay();
    }

    public void OnLeaveApplication(string unusedMessage)
    {
        LeavingApplication();
    }
}

