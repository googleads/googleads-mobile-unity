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
using System.Collections.Generic;

using UnityEngine;

using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    public class AppOpenAd
    {
        private IAppOpenAdClient client;

        private static HashSet<IAppOpenAdClient> loadingClients = new HashSet<IAppOpenAdClient>();

        private AppOpenAd(IAppOpenAdClient client)
        {
            this.client = client;

            this.client.OnAdFailedToPresentFullScreenContent += (sender, args) =>
            {
                if (this.OnAdFailedToPresentFullScreenContent != null)
                {
                    AdError adError = new AdError(args.AdErrorClient);
                    this.OnAdFailedToPresentFullScreenContent(this, new AdErrorEventArgs()
                    {
                        AdError = adError,
                    });
                }
            };

            this.client.OnAdDidPresentFullScreenContent += (sender, args) =>
            {
                  if (this.OnAdDidPresentFullScreenContent != null)
                  {
                      this.OnAdDidPresentFullScreenContent(this, args);
                  }
            };
            
            this.client.OnAdDidDismissFullScreenContent += (sender, args) =>
            {
                  if (this.OnAdDidDismissFullScreenContent != null)
                  {
                      this.OnAdDidDismissFullScreenContent(this, args);
                  }
            };

            this.client.OnAdDidRecordImpression += (sender, args) =>
            {
                if (this.OnAdDidRecordImpression != null)
                {
                    this.OnAdDidRecordImpression(this, args);
                }
            };

            this.client.OnPaidEvent += (sender, args) =>
            {
                if (this.OnPaidEvent != null)
                {
                    this.OnPaidEvent(this, args);
                }
            };
        }

        // Called when the ad is estimated to have earned money.
        public event EventHandler<AdValueEventArgs> OnPaidEvent;

        // Full screen content events.
        public event EventHandler<AdErrorEventArgs> OnAdFailedToPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;

        public event EventHandler<EventArgs> OnAdDidRecordImpression;

        // Loads a new app open ad.
        public static void LoadAd(string adUnitID,
            ScreenOrientation orientation,
            AdRequest request,
            Action<AppOpenAd, AdFailedToLoadEventArgs> adLoadCallback)
        {
            IAppOpenAdClient client = MobileAds.GetClientFactory().BuildAppOpenAdClient();
            loadingClients.Add(client);
            client.CreateAppOpenAd();

            client.OnAdLoaded += (sender, args) =>
            {
                if (adLoadCallback != null)
                {
                    adLoadCallback(new AppOpenAd(client), null);
                    loadingClients.Remove(client);
                }
            };

            client.OnAdFailedToLoad += (sender, args) =>
            {
                if (adLoadCallback != null)
                {
                    LoadAdError loadAdError = new LoadAdError(args.LoadAdErrorClient);
                    adLoadCallback(null, new AdFailedToLoadEventArgs()
                    {
                        LoadAdError = loadAdError,
                    });
                    loadingClients.Remove(client);
                }
            };

            client.LoadAd(adUnitID, request, orientation);
        }

        // Shows an app open ad.
        public void Show()
        {
            if (client != null)
            {
                client.Show();
            }
        }

        // Destroys the AppOpenAd.
        public void Destroy()
        {
            client.DestroyAppOpenAd();
        }

        // Returns ad request response info.
        public ResponseInfo GetResponseInfo()
        {
            return new ResponseInfo(client.GetResponseInfoClient());
        }
    }
}
