// Copyright 2026 Google LLC

using System;
using UnityEngine;

namespace GoogleMobileAds.Snippets
{
    /// <summary>
    /// Code snippets for Custom Native Ad Formats guide.
    /// </summary>
    internal class CustomNativeSnippets : MonoBehaviour
    {
        private const string AD_UNIT_ID = "/6499/example/native";

        // [START load_custom_native_ad]
        private void LoadCustomNativeTemplateAd()
        {
            AdLoader adLoader = new AdLoader.Builder(AD_UNIT_ID)
                .forCustomNativeAd("10063170")
                .Build();
            adLoader.LoadAd(new AdRequest.Builder().Build());
        }
        // [END load_custom_native_ad]

        // [START load_multiple_custom_native_ads]
        private void LoadMultipleCustomNativeTemplateAds()
        {
            AdLoader adLoader = new AdLoader.Builder(AD_UNIT_ID)
                .forCustomNativeAd("10063170")
                .forCustomNativeAd("10063171")
                .forCustomNativeAd("10063172")
                .Build();
            adLoader.LoadAd(new AdRequest.Builder().Build());
        }
        // [END load_multiple_custom_native_ads]

        private void RegisterAdEvents(AdLoader adLoader)
        {
            // [START custom_native_events]
            adLoader.onCustomNativeTemplateAdLoaded += HandleCustomNativeAdLoaded;
            adLoader.OnAdFailedToLoad += HandleCustomNativeAdFailedToLoad;
            // [END custom_native_events]
        }

        // [START handle_custom_native_ad_loaded]
        private bool adLoaded;
        private CustomNativeTemplateAd customNativeTemplateAd;

        private void HandleCustomNativeAdLoaded(object sender, CustomNativeEventArgs args)
        {
            customNativeTemplateAd = args.nativeAd;
            adLoaded = true;
        }
        // [END handle_custom_native_ad_loaded]

        private void HandleCustomNativeAdFailedToLoad(object sender, EventArgs args)
        {
        }

        // [START display_custom_native_ad]
        public const string NATIVE_AD_TEMPLATE_1 = "10063170";
        public const string NATIVE_AD_TEMPLATE_2 = "10063171";

        private Texture2D mainImageTexture;
        private string headline;

        private void Update()
        {
            if (adLoaded)
            {
                mainImageTexture = customNativeTemplateAd.GetTexture2D("MainImage");
                headline = customNativeTemplateAd.GetText("Headline");

                string templateId = customNativeTemplateAd.GetCustomTemplateId();
                if (templateId == NATIVE_AD_TEMPLATE_1)
                {
                    // Custom native ad format 1 loaded.
                }
                else if (templateId == NATIVE_AD_TEMPLATE_2)
                {
                    // Custom native ad format 2 loaded.
                }
                adLoaded = false;
            }
        }
        // [END display_custom_native_ad]

        private void RecordImpressionExample()
        {
            // [START record_impression]
            customNativeTemplateAd.RecordImpression();
            // [END record_impression]
        }

        private void PerformClickExample()
        {
            // [START perform_click]
            customNativeTemplateAd.PerformClick("MainImage");
            // [END perform_click]
        }

        // [START custom_click_action]
        private void LoadCustomNativeTemplateAdWithClickAction()
        {
            AdLoader adLoader = new AdLoader.Builder(AD_UNIT_ID)
                .forCustomNativeAd("10063170", HandleCustomNativeAdClicked)
                .Build();
            adLoader.onCustomNativeTemplateAdLoaded += HandleCustomNativeAdLoaded;
            adLoader.OnAdFailedToLoad += HandleCustomNativeAdFailedToLoad;
            adLoader.LoadAd(new AdRequest.Builder().Build());
        }

        private void HandleCustomNativeAdClicked(
            CustomNativeTemplateAd customNativeTemplateAd, string assetName)
        {
            Debug.Log("Native ad asset with name " + assetName + " was clicked.");
        }
        // [END custom_click_action]

        #region Compilation Stubs
        internal class AdLoader
        {
            public event EventHandler<CustomNativeEventArgs> onCustomNativeTemplateAdLoaded;
            public event EventHandler<EventArgs> OnAdFailedToLoad;

            public void LoadAd(AdRequest request) { }

            public class Builder
            {
                public Builder(string adUnitId) { }
                public Builder forCustomNativeAd(string templateId) => this;
                public Builder forCustomNativeAd(
                    string templateId,
                    Action<CustomNativeTemplateAd, string> callback) => this;
                public AdLoader Build() => new AdLoader();
            }
        }

        internal class CustomNativeTemplateAd
        {
            public Texture2D GetTexture2D(string key) => null;
            public string GetText(string key) => string.Empty;
            public string GetCustomTemplateId() => string.Empty;
            public void RecordImpression() { }
            public void PerformClick(string assetName) { }
        }

        internal class CustomNativeEventArgs : EventArgs
        {
            public CustomNativeTemplateAd nativeAd { get; set; }
        }

        internal class AdRequest
        {
            public class Builder
            {
                public AdRequest Build() => new AdRequest();
            }
        }
        #endregion
    }
}
