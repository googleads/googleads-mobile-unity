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
        private readonly IInsightsEmitter _insightsEmitter = InsightsEmitter.Instance;
        private const Insight.AdFormat PictureInPictureFormat = Insight.AdFormat.PictureInPicture;

        internal AndroidJavaObject androidPictureInPictureAd;
        private string _adUnitId;

        public PictureInPictureAdClient() : base(NextGenUtils.UnityPictureInPictureAdCallbackClassName)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            this.androidPictureInPictureAd = new AndroidJavaObject(
                NextGenUtils.UnityPictureInPictureAdClassName, activity, this);
        }

        public event Action OnAdLoaded;
        public event Action<LoadAdErrorClientEventArgs> OnAdFailedToLoad;
        public event Action OnAdShown;
        public event Action OnAdHidden;
        public event Action OnAdDidRecordImpression;
        public event Action OnAdClicked;
        public event Action<AdErrorClientEventArgs> OnAdFailedToPresentFullScreenContent;
        public event Action OnAdDidPresentFullScreenContent;
        public event Action OnAdDidDismissFullScreenContent;
        public event Action<AdValue> OnPaidEvent;

        public void LoadAd(string adUnitId, AdRequest request)
        {
            _adUnitId = adUnitId;
            this.androidPictureInPictureAd.Call(
                "load", NextGenUtils.GetPictureInPictureAdRequestJavaObject(adUnitId, request));
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

        public string GetAdUnitID()
        {
            return this.androidPictureInPictureAd.Call<string>("getAdUnitId");
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

        private void EmitInsight(Insight.CuiName name, bool success = true)
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = name,
                Format = PictureInPictureFormat,
                AdUnitId = _adUnitId,
                Success = success,
            });
        }

        public void onAdLoaded()
        {
            EmitInsight(Insight.CuiName.AdLoaded);

            if (this.OnAdLoaded != null)
            {
                this.OnAdLoaded();
            }
        }

        public void onAdFailedToLoad(AndroidJavaObject error)
        {
            EmitInsight(Insight.CuiName.AdLoaded, success: false);

            if (this.OnAdFailedToLoad != null)
            {
                LoadAdErrorClientEventArgs args = new LoadAdErrorClientEventArgs
                {
                    LoadAdErrorClient = new NextGenLoadAdErrorClient(error)
                };
                this.OnAdFailedToLoad(args);
            }
        }

        public void onAdShown()
        {
            if (this.OnAdShown != null)
            {
                this.OnAdShown();
            }
        }

        public void onAdHidden()
        {
            if (this.OnAdHidden != null)
            {
                this.OnAdHidden();
            }
        }

        public void onAdClicked()
        {
            EmitInsight(Insight.CuiName.AdClicked);

            if (this.OnAdClicked != null)
            {
                this.OnAdClicked();
            }
        }

        public void onAdImpression()
        {
            EmitInsight(Insight.CuiName.AdShown);

            if (this.OnAdDidRecordImpression != null)
            {
                this.OnAdDidRecordImpression();
            }
        }

        public void onAdFailedToShowFullScreenContent(AndroidJavaObject error)
        {
            EmitInsight(Insight.CuiName.AdShowedFullScreenContent, success: false);

            if (this.OnAdFailedToPresentFullScreenContent != null)
            {
                AdErrorClientEventArgs args = new AdErrorClientEventArgs
                {
                    AdErrorClient = new NextGenFullScreenContentErrorClient(error)
                };
                this.OnAdFailedToPresentFullScreenContent(args);
            }
        }

        public void onAdShowedFullScreenContent()
        {
            EmitInsight(Insight.CuiName.AdShowedFullScreenContent);

            if (this.OnAdDidPresentFullScreenContent != null)
            {
                this.OnAdDidPresentFullScreenContent();
            }
        }

        public void onAdDismissedFullScreenContent()
        {
            EmitInsight(Insight.CuiName.AdDismissedFullScreenContent);

            if (this.OnAdDidDismissFullScreenContent != null)
            {
                this.OnAdDidDismissFullScreenContent();
            }
        }

        public void onPaidEvent(int precision, long valueMicros, string currencyCode)
        {
            EmitInsight(Insight.CuiName.AdPaid);

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
