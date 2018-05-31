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

namespace GoogleMobileAds.Common.Mediation.Tapjoy
{
    public interface ITapjoyClient
    {
        // Forward user's consent status to the Tapjoy SDK in the form
        // of a consent string.
        void SetUserConsent(string consentString);

        // Indicate if GDPR is applicable to the user or not.
        void SubjectToGDPR(bool gdprApplicability);
    }
}
