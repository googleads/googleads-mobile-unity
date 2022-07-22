using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("GoogleMobileAds/Samples/BannerAdManager")]
public class BannerAdManager : MonoBehaviour
{
    public const string FORMAT_NAME = "Banner ad";

    [Tooltip("Displays the ad loading status.")]
    public Text statusText;

    private BannerView _bannerView;

    /// <summary>
    /// Loads the ad.
    /// </summary>
    public void LoadAd()
    {
        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up banner before reusing
        if (_bannerView != null)
        {
            _bannerView.Destroy();
            _bannerView = null;
        }

        Log(String.Format("Loading {0}.", FORMAT_NAME));

        // Create a 320x50 banner at top of the screen
        _bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);

        // listen to events the banner may raise.
        RegisterEventHandlers(_bannerView);

        // create our request used to load the ad.
        var adRequest = new AdRequest.Builder()
            .AddKeyword("unity-admob-sample")
            .Build();

        // send the request to load the ad.
        _bannerView.LoadAd(adRequest);
    }

    /// <summary>
    /// Destroys the ad.
    /// </summary>
    public void DestroyAd()
    {
        if (_bannerView != null)
        {
            Log(String.Format("Destroying {0}.", FORMAT_NAME));
            _bannerView.Destroy();
            _bannerView = null;
        }
    }

    private void RegisterEventHandlers(BannerView ad)
    {
        ad.OnAdClickRecorded += OnAdClickRecorded;
        ad.OnAdImpressionRecorded += OnAdImpressionRecorded;
        ad.OnAdPaid += OnAdPaid;
        ad.OnBannerAdLoaded += OnBannerAdLoaded;
        ad.OnBannerAdLoadFailed += OnBannerAdLoadFailed;
        ad.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
        ad.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened;
    }

    private void OnAdPaid(AdValue adValue)
    {
        Log(String.Format("{0} paid {1} {2}.",
            FORMAT_NAME,
            adValue.Value,
            adValue.CurrencyCode));
    }

    private void OnAdImpressionRecorded()
    {
        Log(String.Format("{0} recorded an impression.", FORMAT_NAME));
    }

    private void OnAdClickRecorded()
    {
        Log(String.Format("{0} recorded a click.", FORMAT_NAME));
    }


    private void OnBannerAdLoadFailed(LoadAdError error)
    {
        LogError(String.Format("{0} failed to load an ad with error : {1}", FORMAT_NAME,
                                                                            error));
    }

    private void OnBannerAdLoaded()
    {
        Log(String.Format("{0} loaded an ad with response : {1}", FORMAT_NAME,
                                                                  _bannerView.GetResponseInfo()));
    }

    private void OnAdFullScreenContentOpened()
    {
        Log(String.Format("{0} full screen content opened.", FORMAT_NAME));
    }

    private void OnAdFullScreenContentClosed()
    {
        Log(String.Format("{0} full screen content closed.", FORMAT_NAME));
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
