// Copyright (C) 2023 Google LLC.
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
using GoogleMobileAds;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Unity
{
    public class NativeOverlayAdClient : BaseAdClient, INativeOverlayAdClient
    {
        public event EventHandler<EventArgs> OnAdLoaded;
        // Ad event fired when the rewarded ad has failed to load.
        public event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;
        // Ad event fired when an ad impression has been recorded.
        public event EventHandler<EventArgs> OnAdDidRecordImpression;
        // Ad event fired when the full screen content has been presented.
        public event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;
        // Ad event fired when the full screen content has been dismissed.
        public event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;
        // Ad event fired when an ad has been clicked.
        public event Action OnAdClicked;
        public event Action<AdValue> OnPaidEvent;

        public void Load(string adUnitId, AdRequest request, NativeAdOptions nativeOptions)
        {
            // No op.
        }

        public void Render(NativeTemplateStyle templateViewStyle, AdSize adSize, AdPosition adPosition)
        {
            // No op.
        }

        public void Render(NativeTemplateStyle templateViewStyle, AdSize adSize, int x, int y)
        {
            // No op.
        }

        public void Render(NativeTemplateStyle templateViewStyle, AdPosition adPosition)
        {
            // No op.
        }

        public void Render(NativeTemplateStyle templateViewStyle, int x, int y)
        {
            // No op.
        }

        public void Hide()
        {
            // No op.
        }

        public void Show()
        {
            // No op.
        }

        public void SetPosition(AdPosition adPosition)
        {
            // No op.
        }

        public void SetPosition(int x, int y)
        {
            // No op.
        }

        public void DestroyAd()
        {
            // No op.
        }

        public float GetHeightInPixels()
        {
            return 0.0f;
        }

        public float GetWidthInPixels()
        {
            return 0.0f;
        }
    }
}
