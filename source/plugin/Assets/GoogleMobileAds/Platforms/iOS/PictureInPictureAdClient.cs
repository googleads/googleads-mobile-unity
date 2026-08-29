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
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    public class PictureInPictureAdClient : IPictureInPictureAdClient
    {
        public event EventHandler<EventArgs> OnAdLoaded;
        public event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;
        public event EventHandler<EventArgs> OnAdShown;
        public event EventHandler<EventArgs> OnAdHidden;
        public event EventHandler<EventArgs> OnAdDidRecordImpression;
        public event Action OnAdClicked;
        public event EventHandler<AdErrorClientEventArgs> OnAdFailedToPresentFullScreenContent;
        public event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;
        public event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;
        public event Action<AdValue> OnPaidEvent;

        public void CreatePictureInPictureAd()
        {
            // No-op for iOS dummy client.
        }

        public void LoadAd(string adUnitId, AdRequest request)
        {
            // No-op for iOS dummy client.
        }

        public void Show(PictureInPictureAdPosition position)
        {
            // No-op for iOS dummy client.
        }

        public void Hide()
        {
            // No-op for iOS dummy client.
        }

        public void Destroy()
        {
            // No-op for iOS dummy client.
        }

        public PictureInPictureAdPosition GetAdPosition()
        {
            return PictureInPictureAdPosition.Default;
        }

        public IResponseInfoClient GetResponseInfoClient()
        {
            return null;
        }
    }
}
