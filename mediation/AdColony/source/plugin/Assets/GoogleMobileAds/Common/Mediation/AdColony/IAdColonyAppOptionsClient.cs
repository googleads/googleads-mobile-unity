// Copyright 2019 Google LLC
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

namespace GoogleMobileAds.Common.Mediation.AdColony
{
    public interface IAdColonyAppOptionsClient
    {
        // Indicates whether the user has given consent to store
        // and process personal information.
        void SetGDPRConsentString(string consentString);

        // Indicates whether the user is subject to GDPR laws.
        void SetGDPRRequired(bool gdprRequired);

        // Sets a custom User ID for this specific app session to be reported
        // to AdColony.
        void SetUserId(string userId);

        // Enables Test Ads for AdColony.
        void SetTestMode(bool isTestMode);

        // Gets the consent string indicating whether the user has given
        // consent to store and process personal information.
        string GetGDPRConsentString();

        // Gets the flag indicating whether the user is subject to GDPR laws.
        bool IsGDPRRequired();

        // Gets the custom User ID set in the AdColony App Options.
        string GetUserId();

        // Gets the flag indicating whether Test Ads for AdColony is enabled.
        bool IsTestMode();
    }
}
