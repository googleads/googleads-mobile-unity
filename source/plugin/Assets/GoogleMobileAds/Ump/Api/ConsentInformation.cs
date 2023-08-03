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
using System.Reflection;
using UnityEngine;

using GoogleMobileAds.Ump.Common;

namespace GoogleMobileAds.Ump.Api
{
    /// <summary>
    /// Utility methods for managing consent information from users.
    /// </summary>
    public class ConsentInformation
    {
        internal static IUmpClientFactory ClientFactory
        {
            get
            {
                if (_clientFactory == null)
                {
                    _clientFactory = Utils.GetClientFactory();
                }
                return _clientFactory;
            }
            set
            {
                _clientFactory = value;
            }
        }
        private static IUmpClientFactory _clientFactory;

        /// <summary>
        /// The user's consent status.
        /// This value is cached between app sessions and can be read before
        /// requesting updated parameters.
        /// </summary>
        public static ConsentStatus ConsentStatus
        {
            get
            {
                IConsentInformationClient client = ClientFactory.ConsentInformationClient();
                return (ConsentStatus)client.GetConsentStatus();
            }
        }

        /// <summary>
        /// Privacy options requirement status.
        /// </summary>
        public static PrivacyOptionsRequirementStatus PrivacyOptionsRequirementStatus
        {
            get
            {
                IConsentInformationClient client = ClientFactory.ConsentInformationClient();
                return (PrivacyOptionsRequirementStatus)client.GetPrivacyOptionsRequirementStatus();
            }
        }

        /// <summary>
        /// Requests consent information update.
        /// </summary>
        /// <param name="request">The request params.</param>
        /// <param name="consentInfoUpdateCallback">Called when client updates ConsentStatus or
        /// returns a FormError argument with ErrorCode and Message. </param>
        public static void Update(ConsentRequestParameters request,
                                  Action<FormError> consentInfoUpdateCallback)
        {
            IConsentInformationClient client = ClientFactory.ConsentInformationClient();

            client.Update(request, () =>
            {
                if (consentInfoUpdateCallback != null)
                {
                    GoogleMobileAds.Api.MobileAds.RaiseAction(() =>
                    {
                        consentInfoUpdateCallback(null);
                    });
                }
            }, error =>
            {
                if (consentInfoUpdateCallback != null)
                {
                    GoogleMobileAds.Api.MobileAds.RaiseAction(() =>
                    {
                        consentInfoUpdateCallback(error);
                    });
                }
            });
        }

        /// <summary>
        /// Check if the app has finished all the required consent flow and can request ads now.
        /// </summary>
        public static bool CanRequestAds()
        {
            IConsentInformationClient client = ClientFactory.ConsentInformationClient();
            return client.CanRequestAds();
        }

        /// <summary>
        /// Clears all consent status from persistent storage.
        /// </summary>
        public static void Reset()
        {
            IConsentInformationClient client = ClientFactory.ConsentInformationClient();
            client.Reset();
        }

        /// <summary>
        /// Returns <c>true</c> if a
        /// <see cref="GoogleMobileAds.Ump.Api.ConsentForm">ConsentForm</see>
        /// is available, otherwise returns <c>false</c>.
        /// </summary>
        public static bool IsConsentFormAvailable()
        {
            IConsentInformationClient client = ClientFactory.ConsentInformationClient();
            return client.IsConsentFormAvailable();
        }
    }
}
