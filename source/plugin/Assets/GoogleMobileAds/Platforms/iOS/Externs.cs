// Copyright (C) 2015 Google, Inc.
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
using System.Runtime.InteropServices;

namespace GoogleMobileAds.iOS
{
    // Externs used by the iOS component.
    internal class Externs
    {
        #region Common externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateRequest();

        [DllImport("__Internal")]
        internal static extern void GADUAddTestDevice(IntPtr request, string deviceId);

        [DllImport("__Internal")]
        internal static extern void GADUAddKeyword(IntPtr request, string keyword);

        [DllImport("__Internal")]
        internal static extern void GADUSetBirthday(IntPtr request, int year, int month, int day);

        [DllImport("__Internal")]
        internal static extern void GADUSetGender(IntPtr request, int genderCode);

        [DllImport("__Internal")]
        internal static extern void GADUTagForChildDirectedTreatment(
                IntPtr request, bool childDirectedTreatment);

        [DllImport("__Internal")]
        internal static extern void GADUSetExtra(IntPtr request, string key, string value);

        [DllImport("__Internal")]
        internal static extern void GADUSetRequestAgent(IntPtr request, string requestAgent);

        [DllImport("__Internal")]
        internal static extern void GADURelease(IntPtr obj);

        #endregion

        #region Banner externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateBannerView(
                IntPtr bannerClient, string adUnitId, int width, int height, int positionAtTop);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateSmartBannerView(
                IntPtr bannerClient, string adUnitId, int positionAtTop);

        [DllImport("__Internal")]
        internal static extern void GADUSetBannerCallbacks(
                IntPtr bannerView,
                BannerClient.GADUAdViewDidReceiveAdCallback adReceivedCallback,
                BannerClient.GADUAdViewDidFailToReceiveAdWithErrorCallback adFailedCallback,
                BannerClient.GADUAdViewWillPresentScreenCallback willPresentCallback,
                BannerClient.GADUAdViewDidDismissScreenCallback didDismissCallback,
                BannerClient.GADUAdViewWillLeaveApplicationCallback willLeaveCallback);

        [DllImport("__Internal")]
        internal static extern void GADUHideBannerView(IntPtr bannerView);

        [DllImport("__Internal")]
        internal static extern void GADUShowBannerView(IntPtr bannerView);

        [DllImport("__Internal")]
        internal static extern void GADURemoveBannerView(IntPtr bannerView);

        [DllImport("__Internal")]
        internal static extern void GADURequestBannerAd(IntPtr bannerView, IntPtr request);

        #endregion

        #region Interstitial externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateInterstitial(
                IntPtr interstitialClient, string adUnitId);

        [DllImport("__Internal")]
        internal static extern void GADUSetInterstitialCallbacks(
                IntPtr interstitial,
                InterstitialClient.GADUInterstitialDidReceiveAdCallback adReceivedCallback,
                InterstitialClient.GADUInterstitialDidFailToReceiveAdWithErrorCallback
                        adFailedCallback,
                InterstitialClient.GADUInterstitialWillPresentScreenCallback willPresentCallback,
                InterstitialClient.GADUInterstitialDidDismissScreenCallback didDismissCallback,
                InterstitialClient.GADUInterstitialWillLeaveApplicationCallback
                        willLeaveCallback
        );

        [DllImport("__Internal")]
        internal static extern bool GADUInterstitialReady(IntPtr interstitial);

        [DllImport("__Internal")]
        internal static extern void GADUShowInterstitial(IntPtr interstitial);

        [DllImport("__Internal")]
        internal static extern void GADURequestInterstitial(IntPtr interstitial, IntPtr request);

        #endregion

        #region Reward based video externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADUCreateRewardBasedVideoAd(IntPtr rewardBasedVideo);

        [DllImport("__Internal")]
        internal static extern bool GADURewardBasedVideoAdReady(IntPtr rewardBasedVideo);

        [DllImport("__Internal")]
        internal static extern void GADUShowRewardBasedVideoAd(IntPtr rewardBasedVideo);

        [DllImport("__Internal")]
        internal static extern void GADURequestRewardBasedVideoAd(
            IntPtr bannerView, IntPtr request, string adUnitId);

        [DllImport("__Internal")]
        internal static extern void GADUSetRewardBasedVideoAdCallbacks(
            IntPtr rewardBasedVideo,
            RewardBasedVideoAdClient.GADURewardBasedVideoAdDidReceiveAdCallback
            adReceivedCallback,
            RewardBasedVideoAdClient.GADURewardBasedVideoAdDidFailToReceiveAdWithErrorCallback
            adFailedCallback,
            RewardBasedVideoAdClient.GADURewardBasedVideoAdDidOpenCallback didOpenCallback,
            RewardBasedVideoAdClient.GADURewardBasedVideoAdDidStartCallback didStartCallback,
            RewardBasedVideoAdClient.GADURewardBasedVideoAdDidCloseCallback didCloseCallback,
            RewardBasedVideoAdClient.GADURewardBasedVideoAdDidRewardCallback didRewardcallback,
            RewardBasedVideoAdClient.GADURewardBasedVideoAdWillLeaveApplicationCallback
            willLeaveCallback
        );

        #endregion
    }
}
