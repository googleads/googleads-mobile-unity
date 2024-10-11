// Copyright (C) 2024 Google LLC
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

namespace GoogleMobileAds.Android
{
    public class PreloadConfigurationClient
    {

        private AndroidJavaObject _preloadConfiguration;

        public PreloadConfigurationClient(AndroidJavaObject preloadConfiguration)
        {
            this._preloadConfiguration = preloadConfiguration;
        }

        public string GetAdUnitId() {
            return _preloadConfiguration.Call<string>("getAdUnitId");
        }

        public AdFormat GetAdFormat() {
            AndroidJavaObject format = _preloadConfiguration.Call<AndroidJavaObject>("getAdFormat");
            int enumValue = format.Call<int>("getValue");
            switch(enumValue)
            {
                case 0: // BANNER
                    return AdFormat.BANNER;
                case 1: // INTERSTITIAL
                    return AdFormat.INTERSTITIAL;
                case 2: // REWARDED
                    return AdFormat.REWARDED;
                case 3: // REWARDED_INTERSTITIAL
                    return AdFormat.REWARDED_INTERSTITIAL;
                case 4: // NATIVE
                    return AdFormat.NATIVE;
                case 6: // APP_OPEN_AD
                    return AdFormat.APP_OPEN_AD;
                default:
                    throw new ArgumentOutOfRangeException("Value not supported.");
            }
        }

        public static AndroidJavaObject BuildPreloadConfiguration(PreloadConfiguration preloadConfiguration)
        {
            if(preloadConfiguration.AdUnitId == null)
            {
                throw new ArgumentNullException(nameof(preloadConfiguration.AdUnitId));
            }
            AndroidJavaClass adFormat = new AndroidJavaClass(Utils.AdFormatEnumName);
            AndroidJavaObject adFormatEnum = adFormat.GetStatic<AndroidJavaObject>(preloadConfiguration.Format.ToString());
            AndroidJavaObject preloadConfigurationBuilder =
                    new AndroidJavaObject(Utils.PreloadConfigurationBuilderClassName,
                                          preloadConfiguration.AdUnitId,
                                          adFormatEnum);
            if (preloadConfiguration.Request != null)
            {
                preloadConfigurationBuilder =
                        preloadConfigurationBuilder.Call<AndroidJavaObject>("setAdRequest",
                                Utils.GetAdRequestJavaObject(preloadConfiguration.Request));
            }
            return preloadConfigurationBuilder.Call<AndroidJavaObject>("build");
        }
    }
}
