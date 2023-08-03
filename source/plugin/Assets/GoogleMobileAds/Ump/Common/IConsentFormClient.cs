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
    /// Client interface for <see cref="GoogleMobileAds.Ump.Api.ConsentForm">ConsentForm</see>
    /// which platforms can implement against.
    /// </summary>
    public interface IConsentFormClient
    {
        /// <summary>
        /// Loads a consent form.
        /// </summary>
        /// <param name="onFormLoaded">Called when the consent form is loaded. </param>
        /// <param name="OnError">Called when the consent form fails to load. </param>
        void Load(Action onFormLoaded, Action<FormError> onError);

        /// <summary>
        /// Shows the consent form.
        /// </summary>
        /// <param name="onDismissed">Called when the consent form is dismissed. </param>
        void Show(Action<FormError> onDismissed);

        /// <summary>
        /// Load and show the consent form when the user consent is required but not yet obtained.
        /// <param name="onDismissed">The listener that gets called when the consent form is
        /// dismissed or fails to show.</param>
        /// </summary>
        void LoadAndShowConsentFormIfRequired(Action<FormError> onDismissed);

        /// <summary>
        /// Show the privacy options form when the privacy options are required.
        /// <param name="onDismissed">The listener that gets called when the privacy options form
        /// is dismissed or fails to show.</param>
        /// </summary>
        void ShowPrivacyOptionsForm(Action<FormError> onDismissed);
    }
}
