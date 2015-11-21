#if UNITY_ANDROID

using System;
using System.Collections.Generic;

using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Android
{
    internal class AndroidInterstitialClient : IGoogleMobileAdsInterstitialClient
    {
        private AndroidJavaObject interstitial;

        public AndroidInterstitialClient(IAdListener listener)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            AndroidJavaObject activity =
                    playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            interstitial = new AndroidJavaObject(
                    Utils.InterstitialClassName, activity, new AdListener(listener));
        }

        #region IGoogleMobileAdsInterstitialClient implementation

        public void CreateInterstitialAd(string adUnitId) {
            interstitial.Call("create", adUnitId);
        }

        public void LoadAd(AdRequest request) {
            interstitial.Call("loadAd", Utils.GetAdRequestJavaObject(request));
        }

        public bool IsLoaded() {
            return interstitial.Call<bool>("isLoaded");
        }

        public void ShowInterstitial() {
            interstitial.Call("show");
        }

        public void DestroyInterstitial() {
            interstitial.Call("destroy");
        }

        public void SetInAppPurchaseParams(IInAppPurchaseListener listener, string publicKey) {
            interstitial.Call("setPlayStorePurchaseParams", new InAppPurchaseListener(listener), publicKey);
        }

        #endregion
    }
}

#endif
