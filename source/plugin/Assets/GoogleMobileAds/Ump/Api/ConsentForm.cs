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
using GoogleMobileAds.Ump.Common;

namespace GoogleMobileAds.Ump.Api
{
    /// <summary>
    /// A rendered form for collecting consent from a user.
    /// </summary>
    public class ConsentForm
    {
        private IConsentFormClient _client;

        internal ConsentForm(IConsentFormClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Loads a consent form.
        /// </summary>
        /// <param name="formLoadCallback">Called when the client loads a consent form or
        /// returns an error. </param>
        public static void Load(Action<ConsentForm, FormError> formLoadCallback)
        {
            IConsentFormClient client = ConsentInformation.ClientFactory.ConsentFormClient();
            client.Load(() =>
            {
                if (formLoadCallback != null)
                {
                    GoogleMobileAds.Api.MobileAds.RaiseAction(() =>
                    {
                        formLoadCallback(new ConsentForm(client), null);
                    });
                }
            }, error =>
            {
                if (formLoadCallback != null)
                {
                    GoogleMobileAds.Api.MobileAds.RaiseAction(() =>
                    {
                        formLoadCallback(null, error);
                    });
                }
            });
        }

        /// <summary>
        /// Shows the consent form.
        /// </summary>
        /// <param name="onDismissed">Called when the consent form is dismissed. </param>
        public void Show(Action<FormError> onDismissed)
        {
            _client.Show((FormError error) =>
            {
                if (onDismissed != null)
                {
                    GoogleMobileAds.Api.MobileAds.RaiseAction(() =>
                    {
                        onDismissed(error);
                    });
                }
            });
        }

        /// <summary>
        /// Load and show the consent form when the user consent is required but not yet obtained.
        /// <param name="onDismissed">The listener that gets called when the consent form is
        /// dismissed or fails to show.</param>
        /// </summary>
        public static void LoadAndShowConsentFormIfRequired(Action<FormError> onDismissed)
        {
            IConsentFormClient client = ConsentInformation.ClientFactory.ConsentFormClient();
            client.LoadAndShowConsentFormIfRequired((FormError error) =>
            {
                if (onDismissed != null)
                {
                    GoogleMobileAds.Api.MobileAds.RaiseAction(() =>
                    {
                        onDismissed(error);
                    });
                }
            });
        }

        /// <summary>
        /// Show the privacy options form when the privacy options button is required.
        /// <param name="onDismissed">The listener that gets called when the privacy options form is
        /// dismissed or fails to show.</param>
        /// </summary>
        public static void ShowPrivacyOptionsForm(Action<FormError> onDismissed)
        {
            IConsentFormClient client = ConsentInformation.ClientFactory.ConsentFormClient();
            client.ShowPrivacyOptionsForm((FormError error) =>
            {
                if (onDismissed != null)
                {
                    GoogleMobileAds.Api.MobileAds.RaiseAction(() =>
                    {
                        onDismissed(error);
                    });
                }
            });
        }
    }
}
