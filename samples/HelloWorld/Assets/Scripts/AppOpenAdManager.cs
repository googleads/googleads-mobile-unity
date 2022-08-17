using UnityEngine;
using System;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.UI;

namespace GoogleMobileAds.Sample
{
    [AddComponentMenu("GoogleMobileAds/Samples/AppOpenAdManager")]
    public class AppOpenAdManager : MonoBehaviour
    {
        [Tooltip("Displays the ad loading status.")]
        public Text statusText;

        public bool IsAdAvailable
        {
            get
            {
                return (!_isShowingAppOpenAd
                        && _ad != null
                        && DateTime.Now < _expireTime);
            }
        }

        // App open ads can be preloaded for up to 4 hours.
        private readonly TimeSpan TIMEOUT = TimeSpan.FromHours(4);
        private DateTime _expireTime;
        private bool _isShowingAppOpenAd;
        private AppOpenAd _ad;

        private void Awake()
        {
            // Use the AppStateEventNotifier to listen to application open/close events.
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
            // These ad units are configured to always serve test ads.
    #if UNITY_EDITOR
            string adUnitId = "unused";
    #elif UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/3419835294";
    #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/5662855259";
    #else
            string adUnitId = "unexpected_platform";
    #endif

            if (_ad != null)
            {
                _ad.Destroy();
                _ad = null;
            }

            Log("Loading app open ad.");

            // create our request used to load the ad.
            var adRequest = new AdRequest.Builder()
                .AddKeyword("unity-admob-sample")
                .Build();

            // send the request to load the ad.
            AppOpenAd.LoadAppOpenAd(adUnitId,
                                    ScreenOrientation.Portrait,
                                    adRequest,
                                    OnLoadAppOpenAd);
        }

        /// <summary>
        /// Shows the ad.
        /// </summary>
        public void ShowAd()
        {
            if (_ad != null)
            {
                Log("Showing app open ad.");
                _isShowingAppOpenAd = true;
                _ad.Show();
            }
            else
            {
                LogError("App open ad is not ready yet.");
            }
        }

        /// <summary>
        /// Destroys the ad.
        /// </summary>
        public void DestroyAd()
        {
            if (_ad != null)
            {
                Log("Destroying app open ad.");
                _ad.Destroy();
                _ad = null;
            }
        }

        private void OnAppStateChanged(AppState state)
        {
            // if the app is Foregrounded and the ad is available, show it.
            if (state == AppState.Foreground)
            {
                if (IsAdAvailable)
                {
                    ShowAd();
                }
            }
        }

        private void OnLoadAppOpenAd(AppOpenAd ad, LoadAdError error)
        {
            if (error != null)
            {
                LogError("App open ad failed to load an ad with error : " + error);
                return;
            }

            Log("App open ad loaded with response : " + ad.GetResponseInfo()));

            _ad = ad;

            // App open ads can be preloaded for up to 4 hours.
            _expireTime = DateTime.Now + TIMEOUT;

            RegisterEventHandlers(ad);
        }

        private void RegisterEventHandlers(AppOpenAd ad)
        {
            // Raised when a click is recorded for an ad.
            ad.OnAdClickRecorded += OnAdClickRecorded;
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += OnAdImpressionRecorded;
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += OnAdPaid;
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened;
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += OnAdFullScreenContentFailed;
        }

        private void OnAdPaid(AdValue adValue)
        {
            Log(String.Format("App open ad paid {0} {1}.",
                              adValue.Value,
                              adValue.CurrencyCode));
        }

        private void OnAdImpressionRecorded()
        {
            Log("App open ad recorded an impression.");
        }

        private void OnAdClickRecorded()
        {
            Log("App open ad recorded a click.");
        }

        private void OnAdFullScreenContentFailed(AdError error)
        {
            LogError("App open ad failed to open full screen content with error : " + error);
        }

        private void OnAdFullScreenContentOpened()
        {
            Log("App open ad full screen content opened.");
        }

        private void OnAdFullScreenContentClosed()
        {
            Log("App open ad full screen content closed.");

            // It may be useful to load a new ad when the current one is complete.
            LoadAd();
        }

        private void Log(string message)
        {
            Debug.Log(message);
            // Api Events are not thread safe and may cause exceptions if they touch the UI thread.
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                statusText.text = message;
            });
        }

        private void LogError(string message)
        {
            Debug.LogError(message);
            // Api Events are not thread safe and may cause exceptions if they touch the UI thread.
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                statusText.text = message;
            });
        }
    }
}
