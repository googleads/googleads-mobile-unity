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
using UnityEngine;

using GoogleMobileAds.Ump.Api;
using GoogleMobileAds.Ump.Common;

namespace GoogleMobileAds.Ump.Unity
{
    public class ConsentInformationClient : IConsentInformationClient
    {
        public const string PlayerPrefsKeyConsentStatus = "GoogleMobileAds.Ump.ConsentStatus";
        private static ConsentInformationClient _instance;

        public static ConsentInformationClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ConsentInformationClient();
                }
                return _instance;
            }
            internal set
            {
                _instance = value;
            }
        }

        /// <summary>
        /// Requests consent information to be updated with new values.
        /// </summary>
        /// <param name="request">The request params.</param>
        /// <param name="onConsentInfoUpdateSuccessCallback">Called when ConsentStatus is updated.
        /// <param name="onConsentInfoUpdateFailureCallback">Called when ConsentStatus couldn't be
        /// updated. Includes a <see cref="FormError">FormError</see> for failure details.</param>
        public void Update(ConsentRequestParameters request,
                           Action onConsentInfoUpdateSuccessCallback,
                           Action<FormError> onConsentInfoUpdateFailureCallback)
        {
            try
            {
                // Do not update, if ConsentStatus is already obtained.
                if (PlayerPrefs.GetInt(PlayerPrefsKeyConsentStatus, 0) == 3)
                {
                    onConsentInfoUpdateSuccessCallback();
                    return;
                }

                // Consent is only required when the user is not a child and is in EEA region.
                if (!request.TagForUnderAgeOfConsent &&
                    request.ConsentDebugSettings.DebugGeography == DebugGeography.EEA)
                {
                    // ConsentStatus.Required
                    PlayerPrefs.SetInt(PlayerPrefsKeyConsentStatus, 2);
                }
                else
                {
                    // ConsentStatus.NotRequired
                    PlayerPrefs.SetInt(PlayerPrefsKeyConsentStatus, 1);
                }
                Debug.Log("Consent Info updated.");
                onConsentInfoUpdateSuccessCallback();
            }
            catch(Exception e)
            {
                onConsentInfoUpdateFailureCallback(new FormError(5, e.Message));
                Debug.LogWarning("Consent Info couldn't be updated. " + e.Message);
            }
        }

        /// <summary>
        /// Clears consent status from persistent storage.
        /// </summary>
        public void Reset()
        {
            PlayerPrefs.SetInt(PlayerPrefsKeyConsentStatus, 0);
            Debug.Log("Consent information reset.");
        }

        /// <summary>
        /// Gets the current consent status.
        /// <para>This value is cached between app sessions and can be read before
        /// requesting updated parameters.</para>
        /// </summary>
        public int GetConsentStatus()
        {
            return PlayerPrefs.GetInt(PlayerPrefsKeyConsentStatus, 0);
        }

        /// <summary>
        /// Get the privacy options requirement status.
        /// </summary>
        public int GetPrivacyOptionsRequirementStatus()
        {
            return (int)PrivacyOptionsRequirementStatus.Required;
        }

        /// <summary>
        /// Check if the app has finished all the required consent flow and can request ads now.
        /// A return value of true means the app can request ads now.
        /// </summary>
        public bool CanRequestAds()
        {
            ConsentStatus consentStatus = (ConsentStatus)GetConsentStatus();
            return consentStatus == ConsentStatus.NotRequired ||
                   consentStatus == ConsentStatus.Obtained;
        }

        /// <summary>
        /// Returns <c>true</c> if a <see cref="GoogleMobileAds.Ump.Api.ConsentForm">ConsentForm
        /// </see> is available, <c>false</c> otherwise.
        /// </summary>
        public bool IsConsentFormAvailable()
        {
            return true;
        }
    }
}
