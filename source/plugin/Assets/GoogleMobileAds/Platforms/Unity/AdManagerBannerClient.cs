// Copyright (C) 2025 Google LLC.
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
using System.Collections.Generic;
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Api.AdManager;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Unity
{
    public class AdManagerBannerClient : BannerClient, IAdManagerBannerClient
    {
        public event Action<AppEvent> OnAppEvent;
        private List<AdSize> _validAdSizes = new List<AdSize>();
        public List<AdSize> ValidAdSizes {
            get { return this._validAdSizes; }
            set
            {
                if (value != null && value.Count < 1)
                {
                    Debug.LogError(
                            "ValidAdSizes must contain at least one valid ad size.");
                    return;
                }
                this._validAdSizes = value;
            }
        }
    }
}