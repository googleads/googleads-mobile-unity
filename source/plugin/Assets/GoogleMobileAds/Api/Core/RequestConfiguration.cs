// Copyright (C) 2020 Google, Inc.
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

namespace GoogleMobileAds.Api
{
    public class RequestConfiguration
    {
        public MaxAdContentRating MaxAdContentRating { get; private set; }
        public TagForChildDirectedTreatment? TagForChildDirectedTreatment { get; private set; }
        public TagForUnderAgeOfConsent? TagForUnderAgeOfConsent { get; private set; }
        public List<string> TestDeviceIds { get; private set; }

        private RequestConfiguration(Builder builder)
        {
            MaxAdContentRating = builder.MaxAdContentRating;
            TagForChildDirectedTreatment = builder.TagForChildDirectedTreatment;
            TagForUnderAgeOfConsent = builder.TagForUnderAgeOfConsent;
            TestDeviceIds = builder.TestDeviceIds;
        }

        public Builder ToBuilder()
        {
            return (new Builder()).
            SetMaxAdContentRating(MaxAdContentRating).
            SetTagForChildDirectedTreatment(TagForChildDirectedTreatment).
            SetTagForUnderAgeOfConsent(TagForUnderAgeOfConsent).
            SetTestDeviceIds(TestDeviceIds);
        }

        public class Builder
        {
            internal MaxAdContentRating MaxAdContentRating { get; private set; }
            internal TagForChildDirectedTreatment? TagForChildDirectedTreatment { get; private set; }
            internal TagForUnderAgeOfConsent? TagForUnderAgeOfConsent { get; private set; }
            internal List<string> TestDeviceIds { get; private set; }

            public Builder()
            {
                MaxAdContentRating = null;
                TagForChildDirectedTreatment = null;
                TagForUnderAgeOfConsent = null;
                TestDeviceIds = new List<string>();
            }

            public Builder SetMaxAdContentRating(MaxAdContentRating maxAdContentRating)
            {
                MaxAdContentRating = maxAdContentRating;
                return this;
            }

            public Builder SetTagForChildDirectedTreatment(TagForChildDirectedTreatment? tagForChildDirectedTreatment)
            {
                TagForChildDirectedTreatment = tagForChildDirectedTreatment;
                return this;
            }
            public Builder SetTagForUnderAgeOfConsent(TagForUnderAgeOfConsent? tagForUnderAgeOfConsent)
            {
                TagForUnderAgeOfConsent = tagForUnderAgeOfConsent;
                return this;
            }

            public Builder SetTestDeviceIds(List<string> testDeviceIds)
            {
                TestDeviceIds = testDeviceIds;
                return this;
            }

            public RequestConfiguration Build()
            {
                return new RequestConfiguration(this);
            }
        }
    }
}