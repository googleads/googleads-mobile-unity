using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("GoogleMobileAds/Samples/InterstitialAdManager")]
public class InterstitialAdManager : MonoBehaviour
{
    public const string FORMAT_NAME = "Interstitial ad";

    [Tooltip("Displays the ad loading status.")]
    public Text statusText;

    private InterstitialAd _ad;

    public void LoadAd()
    {
        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif

        if (_ad != null)
        {
            _ad.Destroy();
            _ad = null;
        }

        Log(String.Format("Loading {0}.", FORMAT_NAME));

        // create our request used to load the ad.
        var adRequest = new AdRequest.Builder()
            .AddKeyword("unity-admob-sample")
            .Build();

        // send the request to load the ad.
        InterstitialAd.LoadInterstitialAd(adUnitId, adRequest, OnLoad);
    }

    public void ShowAd()
    {
        if (_ad != null)
        {
            Log(String.Format("Showing {0}.", FORMAT_NAME));
            _ad.Show();
        }
        else
        {
            LogError(String.Format("{0} is not ready yet.", FORMAT_NAME));
        }
    }

    public void DestroyAd()
    {
        if (_ad != null)
        {
            Log(String.Format("Destroying {0}.", FORMAT_NAME));
            _ad.Destroy();
            _ad = null;
        }
    }

    private void OnLoad(InterstitialAd ad, LoadAdError error)
    {
        if (error != null)
        {
            LogError(String.Format("{0} failed to load an ad with error : {1}", FORMAT_NAME,
                                                                                error));
            return;
        }

        Log(String.Format("{0} loaded with response : {1}.", FORMAT_NAME,
                                                             ad.GetResponseInfo()));

        _ad = ad;
        RegisterEventHandlers(ad);
    }

    private void RegisterEventHandlers(InterstitialAd ad)
    {
        ad.OnAdClickRecorded += OnAdClickRecorded;
        ad.OnAdImpressionRecorded += OnAdImpressionRecorded;
        ad.OnAdPaid += OnAdPaid;
        ad.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
        ad.OnAdFullScreenContentOpened += OnAdFullScreenContentOpened;
        ad.OnAdFullScreenContentFailed += OnAdFullScreenContentFailed;
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

    private void OnAdFullScreenContentFailed(AdError error)
    {
        LogError(String.Format("{0} failed to open full screen content. {1}.", FORMAT_NAME,
                                                                               error));
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
