using System;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Sample
{
    /// <summary>
    /// Demonstrates how to use the Google Mobile Ads App open ad.
    /// </summary>
    /// <remarks>
    /// Before loading ads, initialize the Mobile Ads SDK  by calling MobileAds.Initialize();
    /// This needs to be done only once, ideally at app launch.
    /// <remarks>
    [AddComponentMenu("GoogleMobileAds/Samples/AppOpenAdController")]
    public class AppOpenAdController : MonoBehaviour
    {
        // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
        private string _adUnitId = "ca-app-pub-3940256099942544/3419835294";
#elif UNITY_IPHONE
        private string _adUnitId = "ca-app-pub-3940256099942544/5662855259";
#else
        private string _adUnitId = "unused";
#endif
        public bool IsAdAvailable
        {
            get
            {
                return _appOpenAd != null && _appOpenAd.IsLoaded() && DateTime.Now < _expireTime;
            }
        }

        // App open ads can be preloaded for up to 4 hours.
        private readonly TimeSpan TIMEOUT = TimeSpan.FromHours(4);
        private DateTime _expireTime;
        private AppOpenAd _appOpenAd;

        private void Awake()
        {
            // Use the AppStateEventNotifier to listen to application open/close events.
            // This is used to launch the loaded ad when we open the app.
            AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
        }

        private void OnDestroy()
        {
            // Always unlisten to events when complete.
            AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
        }

        /// <summary>
        /// Loads the ad.
        /// </summary>
        public void LoadAd()
        {
            // Clean up the old ad before loading a new one.
            if (_appOpenAd != null)
            {
                DestroyAd();
            }

            Debug.Log("Loading app open ad.");

            // create our request used to load the ad.
            var adRequest = new AdRequest.Builder()
                .AddKeyword("unity-admob-sample")
                .Build();

            // send the request to load the ad.
            AppOpenAd.Load(_adUnitId, ScreenOrientation.Portrait, adRequest,
                (AppOpenAd ad, LoadAdError error) =>
                {
                    // If the operation completed successfully, no error is returned.
                    if (error != null || ad == null)
                    {
                        Debug.LogError("App open ad failed to load an ad with error : " + error);
                        return;
                    }

                    Debug.Log("App open ad loaded with response : " + ad.GetResponseInfo());

                    // App open ads can be preloaded for up to 4 hours.
                    _expireTime = DateTime.Now + TIMEOUT;

                    _appOpenAd = ad;

                    // Register to ad events to extend functionality.
                    RegisterEventHandlers(ad);
                });
        }

        /// <summary>
        /// Shows the ad.
        /// </summary>
        public void ShowAd()
        {
            if (_appOpenAd != null && _appOpenAd.IsLoaded())
            {
               Debug.Log("Showing app open ad.");
                _appOpenAd.Show();
            }
            else
            {
                Debug.LogError("App open ad is not ready yet.");
            }
        }

        /// <summary>
        /// Destroys the ad.
        /// </summary>
        public void DestroyAd()
        {
            if (_appOpenAd != null)
            {
                Debug.Log("Destroying app open ad.");
                _appOpenAd.Destroy();
                _appOpenAd = null;
            }
        }

        private void OnAppStateChanged(AppState state)
        {
            Debug.Log("App State changed to : "+ state);

            // if the app is Foregrounded and the ad is available, show it.
            if (state == AppState.Foreground)
            {
                if (IsAdAvailable)
                {
                    ShowAd();
                }
            }
        }

        private void RegisterEventHandlers(AppOpenAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("App open ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("App open ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("App open ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += ()
            {
                Debug.Log("App open ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += ()
            {
                Debug.Log("App open ad full screen content closed.");

                // It may be useful to load a new ad when the current one is complete.
                // LoadAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error)
            {
                Debug.LogError("App open ad failed to open full screen content with error : "
                    + error);
            };
        }
    }
}
