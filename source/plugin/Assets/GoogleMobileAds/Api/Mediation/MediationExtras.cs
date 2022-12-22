// Copyright (C) 2022 Google, LLC
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
// limitations under the License.using System.Collections.Generic;

using System.Collections.Generic;

namespace GoogleMobileAds.Api.Mediation
{
    /// <summary>
    /// Extra parameters for mediation custom events. These parameters will be
    /// passed by the Google Mobile Ads SDK to mediation.
    /// <seealso href="https://developers.google.com/admob/unity/custom-events"/>
    /// </summary>
    public abstract class MediationExtras
    {
        /// <summary>
        /// The extra parameters passed to the mediation adapter.
        /// </summary>
        public Dictionary<string, string> Extras { get; protected set; }

        public MediationExtras()
        {
            Extras = new Dictionary<string, string>();
        }

        public abstract string AndroidMediationExtraBuilderClassName { get; }

        public abstract string IOSMediationExtraBuilderClassName { get; }
    }
}
