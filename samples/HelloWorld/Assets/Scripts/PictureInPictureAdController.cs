// Copyright 2026 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Sample
{
    /// <summary>
    /// Demonstrates how to load, show, hide, and manage Picture-in-Picture (PiP) ads in Unity.
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/PictureInPictureAdController")]
    public class PictureInPictureAdController : MonoBehaviour
    {
        /// <summary>
        /// UI element activated when an ad is ready to show.
        /// </summary>
        public GameObject AdLoadedStatus;

        // Test ad units for Picture-in-Picture.
#if UNITY_ANDROID
        private const string _adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
        private const string _adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
        private const string _adUnitId = "unused";
#endif

        private PictureInPictureAd _pipAd;

        /// <summary>
        /// Loads the Picture-in-Picture ad.
        /// </summary>
        public void LoadAd()
        {
            // Clean up old ad before loading a new one.
            if (_pipAd != null)
            {
                DestroyAd();
            }

            Debug.Log("Loading Picture-in-Picture ad.");

            var request = new AdRequest();

            PictureInPictureAd.Load(_adUnitId, request, (PictureInPictureAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Picture-in-Picture ad failed to load with error: " + error);
                    return;
                }

                Debug.Log("Picture-in-Picture ad loaded with response: " + ad.GetResponseInfo());
                _pipAd = ad;
                RegisterEventHandlers(ad);

                // Update UI on the main thread
                MobileAdsEventExecutor.ExecuteInUpdate(() =>
                {
                    if (AdLoadedStatus != null)
                    {
                        AdLoadedStatus.SetActive(true);
                    }
                });
            });
        }

        /// <summary>
        /// Shows the Picture-in-Picture ad with smart restoration (or default bottom-right on 1st show).
        /// </summary>
        public void ShowAd()
        {
            if (_pipAd != null)
            {
                Debug.Log("Showing Picture-in-Picture ad (Restore / Default).");
                _pipAd.Show();
            }
            else
            {
                Debug.LogError("Picture-in-Picture ad is not ready yet.");
            }
        }

        /// <summary>
        /// Shows the Picture-in-Picture ad forced to the Top-Right corner.
        /// </summary>
        public void ShowAdTopRight()
        {
            if (_pipAd != null)
            {
                Debug.Log("Showing Picture-in-Picture ad at TopRight.");
                _pipAd.Show(PictureInPictureAdPosition.TopRight);
            }
            else
            {
                Debug.LogError("Picture-in-Picture ad is not ready yet.");
            }
        }

        /// <summary>
        /// Shows the Picture-in-Picture ad forced to the Top-Left corner.
        /// </summary>
        public void ShowAdTopLeft()
        {
            if (_pipAd != null)
            {
                Debug.Log("Showing Picture-in-Picture ad at TopLeft.");
                _pipAd.Show(PictureInPictureAdPosition.TopLeft);
            }
            else
            {
                Debug.LogError("Picture-in-Picture ad is not ready yet.");
            }
        }

        /// <summary>
        /// Shows the Picture-in-Picture ad forced to the Bottom-Left corner.
        /// </summary>
        public void ShowAdBottomLeft()
        {
            if (_pipAd != null)
            {
                Debug.Log("Showing Picture-in-Picture ad at BottomLeft.");
                _pipAd.Show(PictureInPictureAdPosition.BottomLeft);
            }
            else
            {
                Debug.LogError("Picture-in-Picture ad is not ready yet.");
            }
        }

        /// <summary>
        /// Shows the Picture-in-Picture ad forced to the Bottom-Right corner.
        /// </summary>
        public void ShowAdBottomRight()
        {
            if (_pipAd != null)
            {
                Debug.Log("Showing Picture-in-Picture ad at BottomRight.");
                _pipAd.Show(PictureInPictureAdPosition.BottomRight);
            }
            else
            {
                Debug.LogError("Picture-in-Picture ad is not ready yet.");
            }
        }

        /// <summary>
        /// Hides the Picture-in-Picture ad from screen.
        /// </summary>
        public void HideAd()
        {
            if (_pipAd != null)
            {
                Debug.Log("Hiding Picture-in-Picture ad.");
                _pipAd.Hide();
            }
            else
            {
                Debug.LogError("Picture-in-Picture ad is null.");
            }
        }

        /// <summary>
        /// Destroys the Picture-in-Picture ad.
        /// </summary>
        public void DestroyAd()
        {
            if (_pipAd != null)
            {
                Debug.Log("Destroying Picture-in-Picture ad.");
                _pipAd.Destroy();
                _pipAd = null;
            }

            if (AdLoadedStatus != null)
            {
                AdLoadedStatus.SetActive(false);
            }
        }

        /// <summary>
        /// Logs the Ad Response Info.
        /// </summary>
        public void LogResponseInfo()
        {
            if (_pipAd != null)
            {
                var responseInfo = _pipAd.GetResponseInfo();
                Debug.Log("Picture-in-Picture Ad Response Info: " + (responseInfo != null ? responseInfo.ToString() : "null"));
            }
            else
            {
                Debug.LogError("Picture-in-Picture ad is null.");
            }
        }

        /// <summary>
        /// Logs the Current Ad Position.
        /// </summary>
        public void LogAdPosition()
        {
            if (_pipAd != null)
            {
                PictureInPictureAdPosition position = _pipAd.GetAdPosition();
                Debug.Log("Picture-in-Picture Ad Position: " + position.ToString());
            }
            else
            {
                Debug.LogError("Picture-in-Picture ad is null.");
            }
        }

        private void RegisterEventHandlers(PictureInPictureAd ad)
        {
            ad.OnAdShown += () =>
            {
                Debug.Log("Picture-in-Picture ad shown event received. Position: " + ad.GetAdPosition());
            };

            ad.OnAdHidden += () =>
            {
                Debug.Log("Picture-in-Picture ad hidden event received.");
            };

            ad.OnAdClicked += () =>
            {
                Debug.Log("Picture-in-Picture ad clicked event received.");
            };

            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Picture-in-Picture ad impression recorded.");
            };

            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(string.Format("Picture-in-Picture ad paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
            };
        }
    }
}
