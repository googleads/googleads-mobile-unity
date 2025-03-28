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
        private AdRequest _adRequest;
        internal IntPtr preloadConfigurationPtr;

        internal PreloadConfigurationClient(IntPtr preloadConfiguration)
        {
            preloadConfigurationPtr = preloadConfiguration;
        }

        internal string AdUnitId
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        internal AdFormat Format
        {
            get
            {
                return 0;
            }
            set
            {
                // TODO(b/398243349):Will be removed as part of Zenith V2.
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
                _adRequest = value;
            }
        }

        internal uint BufferSize
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }
    }
}
#endif
