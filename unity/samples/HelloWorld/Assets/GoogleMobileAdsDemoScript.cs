using System;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

// Example script showing how to invoke the Google Mobile Ads Unity plugin.
public class GoogleMobileAdsDemoScript : MonoBehaviour {

    private BannerView bannerView;

    void Start()
    {
        #if UNITY_EDITOR
            string adUnitId = "unused";
        #elif UNITY_ANDROID
            string adUnitId = "INSERT_YOUR_ANDROID_AD_UNIT_HERE";
        #elif UNITY_IPHONE
            string adUnitId = "INSERT_YOUR_IOS_AD_UNIT_HERE";
        #else
            string adUnitId = "unexpected_platform";
        #endif

        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
        // Register for ad events.
        bannerView.AdLoaded += HandleAdLoaded;
        bannerView.AdFailedToLoad += HandleAdFailedToLoad;
        bannerView.AdOpened += HandleAdOpened;
        bannerView.AdClosing += HandleAdClosing;
        bannerView.AdClosed += HandleAdClosed;
        bannerView.AdLeftApplication += HandleAdLeftApplication;
    }

    void OnGUI() {
        // Puts some basic buttons onto the screen.
        GUI.skin.button.fontSize = (int) (0.05f * Screen.height);

        Rect buttonRect = new Rect(0.1f * Screen.width, 0.1f * Screen.height,
                                   0.8f * Screen.width, 0.2f * Screen.height);
        if (GUI.Button(buttonRect, "Request Banner")) {
            RequestBanner();
        }

        Rect bannerRect = new Rect(0.1f * Screen.width, 0.4f * Screen.height,
                                   0.8f * Screen.width, 0.2f * Screen.height);
        if (GUI.Button(bannerRect, "Show Banner")) {
            ShowBanner();
        }

        Rect interstitialRect = new Rect(0.1f * Screen.width, 0.7f * Screen.height,
                                         0.8f * Screen.width, 0.2f * Screen.height);
        if (GUI.Button(interstitialRect, "Hide Banner")) {
            HideBanner();
        }
    }

    void RequestBanner() {
        // Request a banner ad, with optional custom ad targeting.
        AdRequest request = new AdRequest.Builder()
            .AddTestDevice(AdRequest.TestDeviceSimulator)
            .AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
            .AddKeyword("game")
            .SetGender(Gender.Male)
            .SetBirthday(new DateTime(1985, 1, 1))
            .TagForChildDirectedTreatment(false)
            .AddExtra("color_bg", "9B30FF")
            .Build();
        bannerView.LoadAd(request);
    }

    void ShowBanner() {
        bannerView.Show();
    }

    void HideBanner() {
        bannerView.Hide();
    }

    #region Banner callback handlers

    public void HandleAdLoaded()
    {
        print("HandleAdLoaded event received.");
    }

    public void HandleAdFailedToLoad(string message)
    {
        print("HandleFailedToReceiveAd event received with message: " + message);
    }

    public void HandleAdOpened()
    {
        print("HandleAdOpened event received");
    }

    void HandleAdClosing ()
    {
        print("HandleAdClosing event received");
    }

    public void HandleAdClosed()
    {
        print("HandleAdClosed event received");
    }

    public void HandleAdLeftApplication()
    {
        print("HandleAdLeftApplication event received");
    }

    #endregion
}
