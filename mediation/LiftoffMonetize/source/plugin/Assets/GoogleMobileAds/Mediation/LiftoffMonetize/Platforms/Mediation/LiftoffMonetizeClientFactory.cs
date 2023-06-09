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

using GoogleMobileAds.Mediation.LiftoffMonetize.Common;

namespace GoogleMobileAds.Mediation.LiftoffMonetize
{
    public class LiftoffMonetizeClientFactory
    {
        public static ILiftoffMonetizeClient CreateLiftoffMonetizeClient()
        {
            #if UNITY_EDITOR
            return new GoogleMobileAds.Mediation.LiftoffMonetize.Common.DummyClient();
            #elif UNITY_ANDROID
            return GoogleMobileAds.Mediation.LiftoffMonetize.Android.LiftoffMonetizeClient.Instance;
            #elif UNITY_IOS
            return GoogleMobileAds.Mediation.LiftoffMonetize.iOS.LiftoffMonetizeClient.Instance;
            #else
            return new GoogleMobileAds.Mediation.LiftoffMonetize.Common.DummyClient();
            #endif
        }
    }
}
