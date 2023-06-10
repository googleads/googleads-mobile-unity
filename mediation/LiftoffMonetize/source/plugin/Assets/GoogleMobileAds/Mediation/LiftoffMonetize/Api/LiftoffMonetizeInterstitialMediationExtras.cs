// Copyright 2017 Google LLC
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

namespace GoogleMobileAds.Mediation.LiftoffMonetize.Api
{
    public class LiftoffMonetizeInterstitialMediationExtras : LiftoffMonetizeMediationExtras
    {
        public LiftoffMonetizeInterstitialMediationExtras() : base() { }

        public override string AndroidMediationExtraBuilderClassName
        {
            get { return "com.google.unity.mediation.liftoffmonetize.VungleUnityInterstitialExtrasBuilder"; }
        }
    }
}

namespace GoogleMobileAds.Api.Mediation.LiftoffMonetize
{
    [System.Obsolete("Use `GoogleMobileAds.Mediation.LiftoffMonetize.Api.LiftoffMonetizeInterstitialMediationExtras` instead.")]
    public class LiftoffMonetizeInterstitialMediationExtras :
                 GoogleMobileAds.Mediation.LiftoffMonetize.Api.LiftoffMonetizeInterstitialMediationExtras
    {
    }
}
