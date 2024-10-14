#if UNITY_IOS
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

using GoogleMobileAds.Api;

namespace GoogleMobileAds.iOS
{
    internal class PreloadConfigurationClient
    {

        private IntPtr _preloadConfigurationPtr = Externs.GADUCreatePreloadConfiguration();
        private AdRequest _adRequest;

        internal PreloadConfigurationClient(IntPtr client)
        {
            _preloadConfigurationPtr = client;
        }

        internal string AdUnitId
        {
            get
            {
                return Externs.GADUGetPreloadConfigurationAdUnitId(_preloadConfigurationPtr);
            }

            set
            {
                Externs.GADUSetPreloadConfigurationAdUnitId(_preloadConfigurationPtr, value);
            }
        }

        internal AdFormat Format
        {
            get
            {
                int enumValue = Externs.GADUGetPreloadConfigurationAdFormat(_preloadConfigurationPtr);
                switch (enumValue)
                {
                    case 0: // GADAdFormatBanner
                        return AdFormat.BANNER;
                    case 1: // GADAdFormatInterstitial
                        return AdFormat.INTERSTITIAL;
                    case 2: // GADAdFormatRewarded
                        return AdFormat.REWARDED;
                    case 3: // GADAdFormatNative
                        return AdFormat.NATIVE;
                    case 4: // GADAdFormatRewardedInterstitial
                        return AdFormat.REWARDED_INTERSTITIAL;
                    case 6: // GADAdFormatAppOpen
                        return AdFormat.APP_OPEN_AD;
                    default:
                        throw new ArgumentOutOfRangeException("Format");
                }
            }

            set
            {
                Externs.GADUSetPreloadConfigurationAdFormat(_preloadConfigurationPtr, (int)value);
            }
        }

        internal AdRequest Request
        {
            get
            {
                return _adRequest;
            }

            set
            {
                Externs.GADUSetPreloadConfigurationAdRequest(_preloadConfigurationPtr, Utils.BuildAdRequest(value));
                _adRequest = value;
            }
        }
    }
}
#endif
