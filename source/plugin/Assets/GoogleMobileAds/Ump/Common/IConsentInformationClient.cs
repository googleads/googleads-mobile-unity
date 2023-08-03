// Copyright (C) 2022 Google LLC.
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

using GoogleMobileAds.Ump.Api;

namespace GoogleMobileAds.Ump.Common
{
    /// <summary>
    /// Client interface for <see cref="GoogleMobileAds.Ump.Api.ConsentInformation">
    /// ConsentInformation</see> which platforms can implement against.
    /// </summary>
    public interface IConsentInformationClient
    {
        /// <summary>
        /// Resets <see cref="GoogleMobileAds.Ump.Api.ConsentInformation">ConsentInformation</see>
        /// to initialized status. This should only used for debugging.
        void Reset();

        /// <summary>
        /// Requests a consent information update.
        /// <summary>
        /// <param name="onConsentInfoUpdateSuccessCallback">Called when update succeeds.</param>
        /// <param name="onConsentInfoUpdateFailureCallback">Called when update fails.</param>
        void Update(ConsentRequestParameters consentRequestParameters,
                                      Action onConsentInfoUpdateSuccessCallback,
                                      Action<FormError> onConsentInfoUpdateFailureCallback);

        /// <summary>
        /// Gets the current consent status.
        /// <para>The underlying platform caches this value between app sessions and can be read
        /// before requesting updated parameters.</para>
        /// </summary>
        int GetConsentStatus();

        /// <summary>
        /// Get the privacy options requirement status.
        /// </summary>
        int GetPrivacyOptionsRequirementStatus();

        /// <summary>
        /// Check if the app has finished all the required consent flow and can request ads now.
        /// A return value of true means the app can request ads now.
        /// </summary>
        bool CanRequestAds();

        /// <summary>
        /// Returns <c>true</c> if a <see cref="GoogleMobileAds.Ump.Api.ConsentForm">ConsentForm
        /// </see> is available, <c>false</c> otherwise.
        /// </summary>
        bool IsConsentFormAvailable();
    }
}
