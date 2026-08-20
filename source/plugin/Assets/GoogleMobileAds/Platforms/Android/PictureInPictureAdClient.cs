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
using UnityEngine;

namespace GoogleMobileAds.Android
{
    public class PictureInPictureAdClient : AndroidJavaProxy, IPictureInPictureAdClient
    {
        internal AndroidJavaObject androidPictureInPictureAd;

        public PictureInPictureAdClient() : base(NextGenUtils.UnityPictureInPictureAdCallbackClassName)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            this.androidPictureInPictureAd = new AndroidJavaObject(
                NextGenUtils.UnityPictureInPictureAdClassName, activity, this);
        }

        public event EventHandler<EventArgs> OnAdLoaded;
        public event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;
        public event EventHandler<EventArgs> OnAdShown;
        public event EventHandler<EventArgs> OnAdHidden;
        public event EventHandler<EventArgs> OnAdDidRecordImpression;
        public event Action OnAdClicked;
        public event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;
        public event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;
        public event Action<AdValue> OnPaidEvent;

        public void CreatePictureInPictureAd()
        {
            // No-op for Next-Gen.
        }

        public void LoadAd(string adUnitId, AdRequest request)
        {
            this.androidPictureInPictureAd.Call("load", adUnitId, NextGenUtils.GetAdRequestJavaObject(request, adUnitId));
        }

        public void Show(PictureInPictureAdPosition position)
        {
            this.androidPictureInPictureAd.Call("show", (int)position);
        }

        public void Hide()
        {
            this.androidPictureInPictureAd.Call("hide");
        }

        public void Destroy()
        {
            this.androidPictureInPictureAd.Call("destroy");
        }

        public PictureInPictureAdPosition GetAdPosition()
        {
            int ordinal = this.androidPictureInPictureAd.Call<int>("getAdPosition");
            return (PictureInPictureAdPosition)ordinal;
        }

        public IResponseInfoClient GetResponseInfoClient()
        {
            return new ResponseInfoClient(ResponseInfoClientType.AdLoaded, this.androidPictureInPictureAd);
        }

        #region Callback methods called from Java via AndroidJavaProxy

        public void onAdLoaded()
        {
            if (this.OnAdLoaded != null)
            {
                this.OnAdLoaded(this, EventArgs.Empty);
            }
        }

        public void onAdFailedToLoad(AndroidJavaObject error)
        {
            if (this.OnAdFailedToLoad != null)
            {
                LoadAdErrorClientEventArgs args = new LoadAdErrorClientEventArgs
                {
                    LoadAdErrorClient = new NextGenLoadAdErrorClient(error)
                };
                this.OnAdFailedToLoad(this, args);
            }
        }

        public void onAdShown()
        {
            if (this.OnAdShown != null)
            {
                this.OnAdShown(this, EventArgs.Empty);
            }
        }

        public void onAdHidden()
        {
            if (this.OnAdHidden != null)
            {
                this.OnAdHidden(this, EventArgs.Empty);
            }
        }

        public void onAdClicked()
        {
            if (this.OnAdClicked != null)
            {
                this.OnAdClicked();
            }
        }

        public void onAdImpression()
        {
            if (this.OnAdDidRecordImpression != null)
            {
                this.OnAdDidRecordImpression(this, EventArgs.Empty);
            }
        }

        public void onAdShowedFullScreenContent()
        {
            if (this.OnAdDidPresentFullScreenContent != null)
            {
                this.OnAdDidPresentFullScreenContent(this, EventArgs.Empty);
            }
        }

        public void onAdDismissedFullScreenContent()
        {
            if (this.OnAdDidDismissFullScreenContent != null)
            {
                this.OnAdDidDismissFullScreenContent(this, EventArgs.Empty);
            }
        }

        public void onPaidEvent(int precision, long valueMicros, string currencyCode)
        {
            if (this.OnPaidEvent != null)
            {
                AdValue adValue = new AdValue
                {
                    Precision = (AdValue.PrecisionType)precision,
                    Value = valueMicros,
                    CurrencyCode = currencyCode
                };
                this.OnPaidEvent(adValue);
            }
        }

        #endregion
    }
}
