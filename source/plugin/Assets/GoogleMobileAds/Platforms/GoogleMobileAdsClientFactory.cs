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

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds
{
    public class GoogleMobileAdsClientFactory
    {
        public static IBannerClient BuildBannerClient()
        {
            #if UNITY_EDITOR
                // Testing UNITY_EDITOR first because the editor also responds to the currently
                // selected platform.
                return new GoogleMobileAds.Common.DummyClient();
            #elif UNITY_ANDROID
                return new GoogleMobileAds.Android.BannerClient();
            #elif (UNITY_5 && UNITY_IOS) || UNITY_IPHONE
                return new GoogleMobileAds.iOS.BannerClient();
            #else
                return new GoogleMobileAds.Common.DummyClient();
            #endif
        }

        public static IInterstitialClient BuildInterstitialClient()
        {
            #if UNITY_EDITOR
                // Testing UNITY_EDITOR first because the editor also responds to the currently
                // selected platform.
                return new GoogleMobileAds.Common.DummyClient();
            #elif UNITY_ANDROID
                return new GoogleMobileAds.Android.InterstitialClient();
            #elif (UNITY_5 && UNITY_IOS) || UNITY_IPHONE
                return new GoogleMobileAds.iOS.InterstitialClient();
            #else
                return new GoogleMobileAds.Common.DummyClient();
            #endif
        }

        public static IRewardBasedVideoAdClient BuildRewardBasedVideoAdClient()
        {
            #if UNITY_EDITOR
                // Testing UNITY_EDITOR first because the editor also responds to the currently
                // selected platform.
                return new GoogleMobileAds.Common.DummyClient();
            #elif UNITY_ANDROID
                return new GoogleMobileAds.Android.RewardBasedVideoAdClient();
            #elif (UNITY_5 && UNITY_IOS) || UNITY_IPHONE
                return new GoogleMobileAds.iOS.RewardBasedVideoAdClient();
            #else
                return new GoogleMobileAds.Common.DummyClient();
            #endif
        }

        public static IAdLoaderClient BuildAdLoaderClient(AdLoader adLoader)
        {
            #if UNITY_EDITOR
                // Testing UNITY_EDITOR first because the editor also responds to the currently
                // selected platform.
                return new GoogleMobileAds.Common.DummyClient();
            #elif UNITY_ANDROID
                return new GoogleMobileAds.Android.AdLoaderClient(adLoader);
            #elif (UNITY_5 && UNITY_IOS) || UNITY_IPHONE
                return new GoogleMobileAds.iOS.AdLoaderClient(adLoader);
            #else
                return new GoogleMobileAds.Common.DummyClient();
            #endif
        }

        public static IMobileAdsClient MobileAdsInstance()
        {
            #if UNITY_EDITOR
                // Testing UNITY_EDITOR first because the editor also responds to the currently
                // selected platform.
                return new GoogleMobileAds.Common.DummyClient();
            #elif UNITY_ANDROID
                return GoogleMobileAds.Android.MobileAdsClient.Instance;
            #elif (UNITY_5 && UNITY_IOS) || UNITY_IPHONE
                return GoogleMobileAds.iOS.MobileAdsClient.Instance;
            #else
                return new GoogleMobileAds.Common.DummyClient();
            #endif
        }
    }
}
