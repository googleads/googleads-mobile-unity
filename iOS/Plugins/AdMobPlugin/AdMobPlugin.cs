using System;
using System.ComponentModel;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

using AdMobMobilePlugin = AdMobPluginiOS;

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
    public static event Action DismissingOverlay = delegate {};
    public static event Action DismissedOverlay = delegate {};
    public static event Action LeavingApplication = delegate {};

    void Awake()
    {
        gameObject.name = this.GetType().ToString();
        SetCallbackHandlerName(gameObject.name);
        DontDestroyOnLoad(this);
    }

    // Create a GADBannerView and adds it into the view hierarchy.
    public static void CreateBannerView(string publisherId, AdSize adSize, bool positionAtTop)
    {
        // Call plugin only when running on real device.
        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            return;
        }
        AdMobMobilePlugin.CreateBannerView(publisherId, adSize.ToString(), positionAtTop);
    }

    // Request a new ad for the GADBannerView.
    public static void RequestBannerAd(bool isTesting, string extras)
    {
        // Call plugin only when running on real device.
        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            return;
        }
        AdMobMobilePlugin.RequestBannerAd(isTesting, extras);
    }

    // Set the name of the callback handler so the right component gets ad callbacks.
    public static void SetCallbackHandlerName(string callbackHandlerName)
    {
        // Call plugin only when running on real device.
        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            return;
        }
        AdMobMobilePlugin.SetCallbackHandlerName(callbackHandlerName);
    }

    // Hide the GADBannerView from the screen.
    public static void HideBannerView()
    {
        // Call plugin only when running on real device.
        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            return;
        }
        AdMobMobilePlugin.HideBannerView();
    }

    // Show the GADBannerView on the screen if it's hidden.
    public static void ShowBannerView() {
        // Call plugin only when running on real device.
        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            return;
        }
        AdMobMobilePlugin.ShowBannerView();
    }

    public void AdViewDidReceiveAd(string message)
    {
        ReceivedAd();
    }

    public void AdViewDidFailToReceiveAd(string message)
    {
        FailedToReceiveAd(message);
    }

    public void AdViewWillPresentScreen(string message)
    {
        ShowingOverlay();
    }

    public void AdViewWillDismissScreen(string message)
    {
        DismissingOverlay();
    }

    public void AdViewDidDismissScreen(string message)
    {
        DismissedOverlay();
    }

    public void AdViewWillLeaveApplication(string message)
    {
        LeavingApplication();
    }
}
