// Copyright (C) 2020 Google LLC
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

using System.Collections.Generic;
using System;

namespace GoogleMobileAds.Api
{
    /// <summary>
    /// Global configuration used for every <see cref="AdRequest"/>.
    /// </summary>
    [Serializable]
    public class RequestConfiguration
    {
        /// <summary>
        /// A maximum ad content rating. AdMob ads returned for these requests have
        /// a content rating at or below the level set.
        /// </summary>
        public MaxAdContentRating MaxAdContentRating;

        /// <summary>
        /// This property allows you to specify whether you would like your app to be treated as
        /// child-directed for purposes of the Children’s Online Privacy Protection Act (COPPA) -
        /// <seealso href="http://business.ftc.gov/privacy-and-security/childrens-privacy">
        /// http://business.ftc.gov/privacy-and-security/childrens-privacy</seealso>.
        ///
        /// If you set this property to True, you will
        /// indicate that your app should be treated as child-directed for purposes of the Children’s
        /// Online Privacy Protection Act (COPPA).
        ///
        /// If you set this property to False, you will
        /// indicate that your app should not be treated as child-directed for purposes of the
        /// Children’s Online Privacy Protection Act (COPPA).
        ///
        /// If you do not set this property, or set this method to Unspecified ad requests will include no
        /// indication of how you would like your app treated with respect to COPPA.
        ///
        /// By using this property, you certify that this notification is accurate and you are
        /// authorized to act on behalf of the owner of the app. You understand that abuse of this
        /// setting may result in termination of your Google account.
        /// </summary>
        /// </remarks>
        /// Note: it may take some time for this designation to be fully implemented in applicable
        /// Google services.
        /// </remarks>
        public TagForChildDirectedTreatment? TagForChildDirectedTreatment;

        /// <summary>
        /// Indicates the publisher specified that the ad request should receive treatment for
        /// users in the European Economic Area (EEA) under the age of consent.
        /// </summary>
        public TagForUnderAgeOfConsent? TagForUnderAgeOfConsent;

        /// <summary>
        /// This property lets you specify the personalization treatment that applies to subsequent
        /// ad requests.
        /// </summary>
        public PublisherPrivacyPersonalizationState? PublisherPrivacyPersonalizationState;

        /// <summary>
        /// The test device IDs corresponding to test device that will always request
        /// test ads. Returns an empty list if test device IDs were not previously set.
        /// </summary>
        public List<string> TestDeviceIds = new List<string>();

        /// <summary>
        /// Controls whether the Google Mobile Ads SDK publisher first-party ID, formerly known as
        /// the same app key, is enabled. The value set persists across app sessions once
        /// MobileAds.SetRequestConfiguration() is called.
        /// </summary>
        public bool? PublisherFirstPartyIdEnabled;

        public RequestConfiguration() {}

        public RequestConfiguration(RequestConfiguration requestConfiguration)
        {
            MaxAdContentRating = requestConfiguration.MaxAdContentRating;
            TagForChildDirectedTreatment =
                        requestConfiguration.TagForChildDirectedTreatment;
            TagForUnderAgeOfConsent = requestConfiguration.TagForUnderAgeOfConsent;
            PublisherPrivacyPersonalizationState =
                requestConfiguration.PublisherPrivacyPersonalizationState;
            TestDeviceIds = requestConfiguration.TestDeviceIds;
            PublisherFirstPartyIdEnabled = requestConfiguration.PublisherFirstPartyIdEnabled;
        }
    }
}
