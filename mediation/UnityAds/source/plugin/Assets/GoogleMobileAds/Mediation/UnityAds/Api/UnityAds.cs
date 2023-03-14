// Copyright 2018 Google LLC
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

using UnityEngine;

using GoogleMobileAds.Mediation.UnityAds;
using GoogleMobileAds.Mediation.UnityAds.Common;

namespace GoogleMobileAds.Mediation.UnityAds.Api
{
    public class UnityAds
    {
        internal static readonly IUnityAdsClient client =
                UnityAdsClientFactory.CreateUnityAdsClient();

        public static void SetConsentMetaData(string key, bool metaDataValue)
        {
            client.SetConsentMetaData(key, metaDataValue);
        }
    }
}


namespace GoogleMobileAds.Api.Mediation.UnityAds
{
    [System.Obsolete("Use `GoogleMobileAds.Mediation.UnityAds.Api.UnityAds` instead.")]
    public class UnityAds
    {
        public static void SetConsentMetaData(string key, bool metaDataValue)
        {
            GoogleMobileAds.Mediation.UnityAds.Api.UnityAds.SetConsentMetaData(key, metaDataValue);
        }
    }
}
