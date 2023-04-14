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

namespace GoogleMobileAds.Mediation.Vungle.Api
{
    public class VungleInterstitialMediationExtras : VungleMediationExtras
    {
        public VungleInterstitialMediationExtras() : base() { }

        public override string AndroidMediationExtraBuilderClassName
        {
            get { return "com.google.unity.mediation.vungle.VungleUnityInterstitialExtrasBuilder"; }
        }
    }
}

namespace GoogleMobileAds.Api.Mediation.Vungle
{
    [System.Obsolete("Use `GoogleMobileAds.Mediation.Vungle.Api.VungleInterstitialMediationExtras` instead.")]
    public class VungleInterstitialMediationExtras :
                 GoogleMobileAds.Mediation.Vungle.Api.VungleInterstitialMediationExtras
    {
    }
}
