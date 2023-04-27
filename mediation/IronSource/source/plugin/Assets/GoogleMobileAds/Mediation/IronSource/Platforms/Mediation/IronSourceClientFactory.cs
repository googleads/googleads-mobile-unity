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
using System.Collections;
using UnityEngine;

using GoogleMobileAds.Mediation.IronSource.Common;

namespace GoogleMobileAds.Mediation.IronSource
{
    public class IronSourceClientFactory
    {
        public static IIronSourceClient CreateIronSourceClient()
        {
            #if UNITY_EDITOR
            return new GoogleMobileAds.Mediation.IronSource.Common.DummyClient();
            #elif UNITY_ANDROID
            return GoogleMobileAds.Mediation.IronSource.Android.IronSourceClient.Instance;
            #elif UNITY_IOS
            return GoogleMobileAds.Mediation.IronSource.iOS.IronSourceClient.Instance;
            #else
            return new GoogleMobileAds.Mediation.IronSource.Common.DummyClient();
            #endif
        }
    }
}
