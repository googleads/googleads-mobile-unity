// Copyright 2026 Google LLC

using System;
using UnityEngine;

namespace GoogleMobileAds.Snippets
{
    /// <summary>
    /// Code snippets for Native Ads guide.
    /// </summary>
    internal class NativeAdSnippets : MonoBehaviour
    {
        private const string AD_UNIT_ID = "/6499/example/native";

        // [START request_native_ad]
        private void RequestNativeAd()
        {
            AdLoader adLoader = new AdLoader.Builder(AD_UNIT_ID)
                .ForNativeAd()
                .Build();
        }
        // [END request_native_ad]

        private void LoadAd(AdLoader adLoader)
        {
            // [START load_ad]
            adLoader.LoadAd(new AdRequest.Builder().Build());
            // [END load_ad]
        }

        // [START put_ad_request_together]
        private void RequestNativeAdWithEvents()
        {
            AdLoader adLoader = new AdLoader.Builder(AD_UNIT_ID)
                .ForNativeAd()
                .Build();
            adLoader.OnNativeAdLoaded += this.HandleNativeAdLoaded;
            adLoader.OnAdFailedToLoad += this.HandleAdFailedToLoad;
            adLoader.LoadAd(new AdRequest.Builder().Build());
        }
        // [END put_ad_request_together]

        // [START handle_failed_ad_loads]
        private void HandleNativeAdFailedToLoad(
            object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log("Native ad failed to load: " + args.Message);
        }
        // [END handle_failed_ad_loads]

        private void HandleAdFailedToLoad(object sender, EventArgs args)
        {
        }

        // [START handle_native_ad_loaded]
        private NativeAd nativeAd;

        private void HandleNativeAdLoaded(object sender, NativeAdEventArgs args)
        {
            Debug.Log("Native ad loaded.");
            this.nativeAd = args.nativeAd;
        }
        // [END handle_native_ad_loaded]

        private void RegisterIconGameObject(GameObject icon)
        {
            // [START register_game_objects]
            if (!this.nativeAd.RegisterIconImageGameObject(icon))
            {
                // Handle failure to register the icon ad asset.
            }
            // [END register_game_objects]
        }

        private void ConfigureHeadlineGameObject()
        {
            // [START headline_box_collider]
            // Create GameObject that will display the headline ad asset.
            GameObject headline = new GameObject();
            headline.AddComponent<TextMesh>();
            headline.GetComponent<TextMesh>().characterSize = 0.5f;
            headline.GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;
            headline.GetComponent<TextMesh>().color = Color.black;

            // Get string of the headline asset.
            string headlineText = this.nativeAd.GetHeadlineText();
            headline.GetComponent<TextMesh>().text = headlineText;

            // Add box collider to the GameObject which will automatically scale.
            headline.AddComponent<BoxCollider>();
            // [END headline_box_collider]
        }

        internal class AssetRetrievalScript : MonoBehaviour
        {
            // [START retrieve_native_ad_assets]
            private bool nativeAdLoaded;
            private NativeAd nativeAd;

            private void Update()
            {
                if (this.nativeAdLoaded)
                {
                    this.nativeAdLoaded = false;
                    // Get Texture2D for the icon asset of native ad.
                    Texture2D iconTexture = this.nativeAd.GetIconTexture();

                    // Get string for headline asset of native ad.
                    string headline = this.nativeAd.GetHeadlineText();
                }
            }

            private void HandleNativeAdLoaded(object sender, NativeAdEventArgs args)
            {
                Debug.Log("Native ad loaded.");
                this.nativeAd = args.nativeAd;
                this.nativeAdLoaded = true;
            }
            // [END retrieve_native_ad_assets]
        }

        internal class DemoScript : MonoBehaviour
        {
            // [START native_ad_demo]
            private GameObject icon;
            private bool nativeAdLoaded;
            private NativeAd nativeAd;

            private void Update()
            {
                if (this.nativeAdLoaded)
                {
                    this.nativeAdLoaded = false;
                    // Get Texture2D for icon asset of native ad.
                    Texture2D iconTexture = this.nativeAd.GetIconTexture();

                    icon = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    icon.transform.position = new Vector3(1, 1, 1);
                    icon.transform.localScale = new Vector3(1, 1, 1);
                    icon.GetComponent<Renderer>().material.mainTexture = iconTexture;

                    // Register GameObject that will display icon asset of native ad.
                    if (!this.nativeAd.RegisterIconImageGameObject(icon))
                    {
                        // Handle failure to register ad asset.
                    }
                }
            }

            private void HandleNativeAdLoaded(object sender, NativeAdEventArgs args)
            {
                Debug.Log("Native ad loaded.");
                this.nativeAd = args.nativeAd;
                this.nativeAdLoaded = true;
            }
            // [END native_ad_demo]
        }

        #region Compilation Stubs
        internal class AdLoader
        {
            public event EventHandler<NativeAdEventArgs> OnNativeAdLoaded;
            public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

            public void LoadAd(AdRequest request) { }

            public class Builder
            {
                public Builder(string adUnitId) { }
                public Builder ForNativeAd() => this;
                public AdLoader Build() => new AdLoader();
            }
        }

        internal class NativeAd
        {
            public Texture2D GetIconTexture() => null;
            public string GetHeadlineText() => string.Empty;
            public bool RegisterIconImageGameObject(GameObject gameObject) => true;
        }

        internal class NativeAdEventArgs : EventArgs
        {
            public NativeAd nativeAd { get; set; }
        }

        internal class AdFailedToLoadEventArgs : EventArgs
        {
            public string Message { get; set; }
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
