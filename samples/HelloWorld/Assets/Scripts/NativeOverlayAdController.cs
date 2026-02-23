using System;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.UIElements;

namespace GoogleMobileAds.Sample
{
    /// <summary>
    /// Demonstrates how to use Google Mobile Ads native ads.
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/NativeOverlayAdController")]
    public class NativeOverlayAdController : MonoBehaviour
    {
        // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
        private const string _adUnitId = "ca-app-pub-3940256099942544/2247696110";
#elif UNITY_IPHONE
        private const string _adUnitId = "ca-app-pub-3940256099942544/3986624511";
#else
        private const string _adUnitId = "unused";
#endif

        /// <summary>
        /// UI element activated when an ad is ready to show.
        /// </summary>
        public GameObject AdLoadedStatus;

        /// <summary>
        /// Placeholder target for the native overlay ad.
        /// </summary>
        public RectTransform AdPlacmentTarget;

        /// <summary>
        /// Define our native ad advanced options.
        /// </summary>
        public NativeAdOptions Option = new NativeAdOptions
        {
            AdChoicesPlacement = AdChoicesPlacement.TopRightCorner,
            MediaAspectRatio = MediaAspectRatio.Any,
        };

        /// <summary>
        /// Define our native ad template style.
        /// </summary>
        public NativeTemplateStyle Style = new NativeTemplateStyle
        {
            TemplateId = NativeTemplateId.Medium,
        };

        private NativeOverlayAd _nativeOverlayAd;

        /// <summary>
        /// Loads the ad.
        /// </summary>
        public void LoadAd()
        {
            // Clean up the old ad before loading a new one.
            if (_nativeOverlayAd != null)
            {
                DestroyAd();
            }

            Debug.Log("Loading native overlay ad.");

            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // Send the request to load the ad.
            NativeOverlayAd.Load(_adUnitId, adRequest, Option,
                (NativeOverlayAd ad, LoadAdError error) =>
            {
                // If the operation failed with a reason.
                if (error != null)
                {
                    Debug.LogError("Native Overlay ad failed to load an ad with error : " + error);
                    return;
                }
                // If the operation failed for unknown reasons.
                // This is an unexpected error, please report this bug if it happens.
                if (ad == null)
                {
                    Debug.LogError("Unexpected error: Native Overlay ad load event fired with " +
                    " null ad and null error.");
                    return;
                }

                // The operation completed successfully.
                Debug.Log("Native Overlay ad loaded with response : " + ad.GetResponseInfo());
                _nativeOverlayAd = ad;

                // Register to ad events to extend functionality.
                RegisterEventHandlers(ad);

                // Inform the UI that the ad is ready.
                AdLoadedStatus?.SetActive(true);
            });
        }

        private void RegisterEventHandlers(NativeOverlayAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Native Overlay ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Native Overlay ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("Native Overlay ad was clicked.");
            };
            // Raised when the ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Native Overlay ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Native Overlay ad full screen content closed.");
            };
        }

        /// <summary>
        /// Shows the ad.
        /// </summary>
        public void ShowAd()
        {
            if (_nativeOverlayAd != null)
            {
                Debug.Log("Showing Native Overlay ad.");
                _nativeOverlayAd.Show();
            }
        }

        /// <summary>
        /// Hides the ad.
        /// </summary>
        public void HideAd()
        {
            if (_nativeOverlayAd != null)
            {
                Debug.Log("Hiding Native Overlay ad.");
                _nativeOverlayAd.Hide();
            }
        }

        /// <summary>
        /// Renders the ad.
        /// </summary>
        public void RenderAd()
        {
            if (_nativeOverlayAd != null)
            {
                Debug.Log("Rendering Native Overlay ad.");

                // Renders a native overlay ad at the default size
                // and anchored to the bottom of the screne.
                _nativeOverlayAd.RenderTemplate(Style, AdPosition.Bottom);
            }
        }

        /// <summary>
        /// Destroys the ad.
        /// When you are finished with the ad, make sure to call the Destroy()
        /// method before dropping your reference to it.
        /// </summary>
        public void DestroyAd()
        {
            if (_nativeOverlayAd != null)
            {
                Debug.Log("Destroying Native Overlay ad.");
                _nativeOverlayAd.Destroy();
                _nativeOverlayAd = null;
            }

            // Inform the UI that the ad is not ready.
            AdLoadedStatus?.SetActive(false);
        }

        /// <summary>
        /// Logs the ResponseInfo.
        /// </summary>
        public void LogResponseInfo()
        {
            if (_nativeOverlayAd != null)
            {
                var responseInfo = _nativeOverlayAd.GetResponseInfo();
                if (responseInfo != null)
                {
                    Debug.Log(responseInfo);
                }
            }
        }
    }
}
