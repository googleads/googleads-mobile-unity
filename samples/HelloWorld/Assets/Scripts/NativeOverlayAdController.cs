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
        /// UI element which acts as a placeholder for the native overlay ad.
        /// </summary>
        public RectTransform AdCustomTarget;

        /// <summary>
        /// Define our native ad advanced options.
        /// </summary>
        public NativeAdOptions Option = new NativeAdOptions
        {
            AdChoicesPosition = AdChoicesPlacement.TopRightCorner,
            MediaAspectRatio = NativeMediaAspectRatio.Any,
        };

        /// <summary>
        /// Define our native ad template style.
        /// </summary>
        public NativeTemplateStyle Style = new NativeTemplateStyle
        {
            TemplateID = NativeTemplateID.Medium,
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
            /*
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Native Overlay ad failed to open full screen content with error : "
                    + error);
            };
            */
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
        /// Renders the ad.
        /// </summary>
        public void RenderCustomAd()
        {
            UnityEngine.Debug.Log("Nick RenderAd");
            if (AdCustomTarget == null)
            {
                Debug.LogError("Native Overlay ad custom target is null.");
                return;
            }
            if (_nativeOverlayAd == null)
            {
                Debug.LogError("Native Overlay ad is null.");
                return;
            }

            // Renders a native overlay ad at a specific screen location which
            // which is defined in your game by a rect transform.

            // Get the rectangle corners of the target location in world space.
            Vector3[] corners1 = new Vector3[4];
            AdCustomTarget.GetWorldCorners(corners1);

            // The corners start on the top left and rotate clockwise.
            // We return a rect which returns the native overlay rect in
            // screen space.
            var woldRect = new Rect(
                corners1[0].x,
                Screen.height - corners1[1].y,
                corners1[2].x - corners1[0].x,
                corners1[2].y - corners1[0].y);

            // For iOS we must modify this based on a scale factor;
            // NOTE : scaleFactor is device dependent and will change.
            // Please update this value to match your specific device.
            var scaleFactor = 3;

            // Calculate adjustY from safe area and status bar height.
            // NOTE : status bar height is device dependent and will change.
            // Please update this value to match your specific device.
            var adjustY = (Screen.safeArea.y + 44);

            // Get the native overlay location in screen space considering
            // the adjustments of safe area, status bar, and scale factor.
            var screenRect = new Rect(
                woldRect.x / scaleFactor,
                (woldRect.y - adjustY) / scaleFactor,
                woldRect.width / scaleFactor,
                woldRect.height / scaleFactor);

            var size = new AdSize(
                Mathf.RoundToInt(screenRect.width),
                Mathf.RoundToInt(screenRect.height));
            var x = Mathf.RoundToInt(screenRect.x);
            var y = Mathf.RoundToInt(screenRect.y);
            _nativeOverlayAd.RenderTemplate(Style, size, x, y);
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
