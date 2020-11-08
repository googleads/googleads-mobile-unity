// Copyright (C) 2020 Google LLC
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

namespace GoogleMobileAds.Android
{
  public class RewardedInterstitialAdClient : AndroidJavaProxy, IRewardedInterstitialAdClient
  {
    private AndroidJavaObject androidRewardedInterstitialAd;

    public RewardedInterstitialAdClient() : base(Utils.UnityRewardedInterstitialAdCallbackClassName)
    {
      AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
      AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
      androidRewardedInterstitialAd = new AndroidJavaObject(Utils.UnityRewardedInterstitialAdClassName, activity, this);
    }

    public event EventHandler<EventArgs> OnAdLoaded;

    public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

    public event EventHandler<Reward> OnUserEarnedReward;

    public event EventHandler<AdValueEventArgs> OnPaidEvent;

    public event EventHandler<AdErrorEventArgs> OnAdFailedToPresentFullScreenContent;

    public event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;

    public event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;

    public void CreateRewardedInterstitialAd()
    {
        // Do nothing.
    }

    public void LoadAd(string adUnitId, AdRequest request)
    {
      androidRewardedInterstitialAd.Call("loadAd", adUnitId, Utils.GetAdRequestJavaObject(request));
    }

    public void Show()
    {
      androidRewardedInterstitialAd.Call("show");
    }

    public Reward GetRewardItem()
    {
      AndroidJavaObject rewardItem = this.androidRewardedInterstitialAd.Call<AndroidJavaObject>("getRewardItem");
      if (rewardItem == null)
      {
          return null;
      }
      string type = rewardItem.Call<string>("getType");
      int amount = rewardItem.Call<int>("getAmount");
      return new Reward()
      {
          Type = type,
          Amount = (double)amount
      };
    }

    public void SetServerSideVerificationOptions(ServerSideVerificationOptions serverSideVerificationOptions)
    {
      androidRewardedInterstitialAd.Call("setServerSideVerificationOptions", Utils.GetServerSideVerificationOptionsJavaObject(serverSideVerificationOptions));
    }

    public IResponseInfoClient GetResponseInfoClient()
    {
      return new ResponseInfoClient(this.androidRewardedInterstitialAd);
    }

    void onRewardedInterstitialAdLoaded()
    {
        if (this.OnAdLoaded != null)
        {
            this.OnAdLoaded(this, EventArgs.Empty);
        }
    }

    void onRewardedInterstitialAdFailedToLoad(string errorReason)
    {
        if (this.OnAdFailedToLoad != null)
        {
            AdFailedToLoadEventArgs args = new AdFailedToLoadEventArgs()
            {
                Message = errorReason
            };
            this.OnAdFailedToLoad(this, args);
        }
    }

    void onAdFailedToShowFullScreenContent(string errorReason)
    {
        if (this.OnAdFailedToPresentFullScreenContent != null)
        {
            AdErrorEventArgs args = new AdErrorEventArgs()
            {
                Message = errorReason
            };
            this.OnAdFailedToPresentFullScreenContent(this, args);
        }
    }

    void onAdShowedFullScreenContent()
    {
        if (this.OnAdDidPresentFullScreenContent != null)
        {
            this.OnAdDidPresentFullScreenContent(this, EventArgs.Empty);
        }
    }


    void onAdDismissedFullScreenContent()
    {
        if (this.OnAdDidDismissFullScreenContent != null)
        {
            this.OnAdDidDismissFullScreenContent(this, EventArgs.Empty);
        }
    }

    void onUserEarnedReward(string type, float amount)
    {
        if (this.OnUserEarnedReward != null)
        {
            Reward args = new Reward()
            {
                Type = type,
                Amount = amount
            };
            this.OnUserEarnedReward(this, args);
        }
    }

    public void onPaidEvent(int precision, long valueInMicros, string currencyCode)
    {
        if (this.OnPaidEvent != null)
        {
            AdValue adValue = new AdValue()
            {
                Precision = (AdValue.PrecisionType)precision,
                Value = valueInMicros,
                CurrencyCode = currencyCode
            };
            AdValueEventArgs args = new AdValueEventArgs()
            {
                AdValue = adValue
            };

            this.OnPaidEvent(this, args);
        }
    }
  }
}
