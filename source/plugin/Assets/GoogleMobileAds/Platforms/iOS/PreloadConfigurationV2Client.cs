#if UNITY_IOS
// Copyright (C) 2025 Google LLC
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
    internal class PreloadConfigurationV2Client
    {
        private AdRequest _adRequest;
        private readonly IntPtr _preloadConfigurationV2Ptr;

        internal PreloadConfigurationV2Client(IntPtr preloadConfigurationV2)
        {
            _preloadConfigurationV2Ptr = preloadConfigurationV2;
        }

        internal string AdUnitId
        {
            get
            {
                return Externs.GADUGetPreloadConfigurationV2AdUnitID(_preloadConfigurationV2Ptr);
            }

            set
            {
                Externs.GADUSetPreloadConfigurationV2AdUnitID(_preloadConfigurationV2Ptr, value);
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
                Externs.GADUSetPreloadConfigurationV2AdRequest(_preloadConfigurationV2Ptr,
                                                             Utils.BuildAdRequest(value));
                _adRequest = value;
            }
        }

        internal uint BufferSize
        {
            get
            {
                return Externs.GADUGetPreloadConfigurationV2BufferSize(_preloadConfigurationV2Ptr);
            }

            set
            {
                Externs.GADUSetPreloadConfigurationV2BufferSize(_preloadConfigurationV2Ptr, value);
            }
        }
    }
}
#endif // UNITY_IOS
