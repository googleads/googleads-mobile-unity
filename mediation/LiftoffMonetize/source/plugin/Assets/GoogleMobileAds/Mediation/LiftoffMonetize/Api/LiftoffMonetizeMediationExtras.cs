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

using GoogleMobileAds.Api.Mediation;

namespace GoogleMobileAds.Mediation.LiftoffMonetize.Api
{
    public abstract class LiftoffMonetizeMediationExtras : MediationExtras
    {
        public const string UserIdKey = "user_id";

        public LiftoffMonetizeMediationExtras() : base() { }

        public override string IOSMediationExtraBuilderClassName
        {
            get { return "LiftoffMonetizeExtrasBuilder"; }
        }

        public void SetUserId(string userId)
        {
            this.Extras.Add(UserIdKey, userId);
        }
    }
}


namespace GoogleMobileAds.Api.Mediation.LiftoffMonetize
{
    [System.Obsolete("Use `GoogleMobileAds.Mediation.LiftoffMonetize.Api.LiftoffMonetizeMediationExtras` instead.")]
    public abstract class LiftoffMonetizeMediationExtras :
                          GoogleMobileAds.Mediation.LiftoffMonetize.Api.LiftoffMonetizeMediationExtras
    {
    }
}
