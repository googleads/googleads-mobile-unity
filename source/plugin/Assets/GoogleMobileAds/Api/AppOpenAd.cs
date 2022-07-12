// Copyright (C) 2021 Google LLC
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
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    /// <summary>
    /// App open ads are used to display ads when users enter your app. An AppOpenAd object
    /// contains all the data necessary to display an ad. Unlike interstitial ads, app open ads
    /// make it easy to provide an app branding so that users understand the context in which they
    /// see the ad.
    /// </summary>
    public class AppOpenAd : BaseFullScreenAd
    {
        /// <summary>
        /// Loads an app open ad.
        /// </summary>
        public static void LoadAppOpenAd(string adUnitId,
                                  ScreenOrientation orientation,
                                  AdRequest request,
                                  Action<AppOpenAd, LoadAdError> callback)
        {
            Action<IAppOpenAdClient, ILoadAdErrorClient> apiCallback = (ad, error) =>
            {
                callback(new AppOpenAd(ad),
                    error == null ? null : new LoadAdError(error));
            };
            var client = MobileAds.GetClientFactory().BuildAppOpenAdClient();
            client.LoadAppOpenAd(adUnitId, orientation, request, apiCallback);
        }

        /// <summary>
        /// Loads a new app open ad.
        /// <summary>
        [Obsolete("Use LoadAppOpenAd().")]
        public static void LoadAd(string adUnitId,
                                  ScreenOrientation orientation,
                                  AdRequest request,
                                  Action<AppOpenAd, AdFailedToLoadEventArgs> adLoadCallback)
        {
            Action<AppOpenAd, LoadAdError> oldCallback = (ad, error) =>
            {
                AdFailedToLoadEventArgs failedToLoadEventArgs = error == null
                  ? null
                  : new AdFailedToLoadEventArgs{ LoadAdError = error };
                adLoadCallback(ad,failedToLoadEventArgs);
            };
            LoadAppOpenAd(adUnitId, orientation, request, oldCallback);
        }

        // Events

        [Obsolete("Use OnFullScreenAdOpened.")]
        public event EventHandler<EventArgs> OnAdDidPresentFullScreenContent = delegate{};
        [Obsolete("Use OnFullScreenAdClosed.")]
        public event EventHandler<EventArgs> OnAdDidDismissFullScreenContent = delegate{};
        [Obsolete("Use OnFullScreenAdFailed.")]
        public event EventHandler<AdErrorEventArgs>
                                    OnAdFailedToPresentFullScreenContent = delegate{};
        [Obsolete("Use OnAdImpressionRecorded.")]
        public event EventHandler<EventArgs> OnAdDidRecordImpression = delegate{};
        [Obsolete("Use OnAdPaid.")]
        public event EventHandler<AdValueEventArgs> OnPaidEvent = delegate{};

        // Fields
        private IAppOpenAdClient _client;

        // Constructors
        private AppOpenAd(IAppOpenAdClient client)
        {
            Init(client);
            _client = client;
            OnAdPaid += (adValue) =>
            {
                OnPaidEvent(this, new AdValueEventArgs(adValue));
            };
            OnAdImpressionRecorded += () =>
            {
                OnAdDidRecordImpression(this, EventArgs.Empty);
            };
            OnAdFullScreenContentFailed += (error) =>
            {;
                OnAdFailedToPresentFullScreenContent(this, new AdErrorEventArgs
                {
                    AdError = error
                });
            };
            OnAdFullScreenContentClosed += () =>
            {
                OnAdDidDismissFullScreenContent(this, EventArgs.Empty);
            };
            OnAdFullScreenContentOpened += () =>
            {
                OnAdDidPresentFullScreenContent(this, EventArgs.Empty);
            };
        }

        /// <summary>
        /// Returns ad request response info.
        /// </summary>
        [Obsolete("Use ResponseInfo.")]
        public ResponseInfo GetResponseInfo()
        {
            return base.Response;
        }

        /// <summary>
        /// Shows an app open ad.
        /// </summary>
        public void Show()
        {
            if (_client != null)
            {
                _client.Show();
            }
        }
    }
}
