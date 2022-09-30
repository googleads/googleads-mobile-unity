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

using GoogleMobileAds.Ump.Common;

using System;

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
            this._client = client;
        }

        /// <summary>
        /// Loads a consent form.
        /// </summary>
        /// <param name="onLoad">Called when the consent form is loaded. </param>
        /// <param name="OnError">Called when the consent form fails to load. </param>
        public static void LoadConsentForm(Action<ConsentForm> onLoad, Action<FormError> onError)
        {
            IConsentFormClient client = ConsentInformation.GetClientFactory().ConsentFormClient();
            //client.CreateConsentForm();

            client.LoadConsentForm(() =>
                    {
                        onLoad(new ConsentForm(client));
                    }, onError);
        }

        /// <summary>
        /// Shows the consent form.
        /// </summary>
        /// <param name="onDismiss">Called when the consent form is dismissed. </param>
        public void Show(Action<FormError> onDismiss)
        {
            this._client.Show(onDismiss);
        }
    }
}
