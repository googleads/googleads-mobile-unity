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

using System;
using UnityEngine;

using GoogleMobileAds.Mediation.UnityAds.Common;

namespace GoogleMobileAds.Mediation.UnityAds
{
    public class UnityAdsClientFactory
    {
        public static IUnityAdsClient CreateUnityAdsClient()
        {
            #if UNITY_EDITOR
            return new GoogleMobileAds.Mediation.UnityAds.Common.DummyClient();
            #elif UNITY_ANDROID
            return GoogleMobileAds.Mediation.UnityAds.Android.UnityAdsClient.Instance;
            #elif UNITY_IOS
            return GoogleMobileAds.Mediation.UnityAds.iOS.UnityAdsClient.Instance;
            #else
            return new GoogleMobileAds.Mediation.UnityAds.Common.DummyClient();
            #endif
        }
    }
}

