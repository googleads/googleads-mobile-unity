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
            this.MaxAdContentRating = builder.MaxAdContentRating;
            this.TagForChildDirectedTreatment = builder.TagForChildDirectedTreatment;
            this.TagForUnderAgeOfConsent = builder.TagForUnderAgeOfConsent;
            this.TestDeviceIds = builder.TestDeviceIds;
        }

        public Builder ToBuilder()
        {
            return (new Builder()).
            SetMaxAdContentRating(this.MaxAdContentRating).
            SetTagForChildDirectedTreatment(this.TagForChildDirectedTreatment).
            SetTagForUnderAgeOfConsent(this.TagForUnderAgeOfConsent).
            SetTestDeviceIds(this.TestDeviceIds);
        }

        public class Builder
        {

            internal MaxAdContentRating MaxAdContentRating { get; private set; }
            internal TagForChildDirectedTreatment? TagForChildDirectedTreatment { get; private set; }
            internal TagForUnderAgeOfConsent? TagForUnderAgeOfConsent { get; private set; }
            internal List<string> TestDeviceIds { get; private set; }

            public Builder()
            {
                this.MaxAdContentRating = null;
                this.TagForChildDirectedTreatment = null;
                this.TagForUnderAgeOfConsent = null;
                this.TestDeviceIds = new List<string>();
            }

            public Builder SetMaxAdContentRating(MaxAdContentRating maxAdContentRating)
            {
                this.MaxAdContentRating = maxAdContentRating;
                return this;
            }

            public Builder SetTagForChildDirectedTreatment(TagForChildDirectedTreatment? tagForChildDirectedTreatment)
            {
                this.TagForChildDirectedTreatment = tagForChildDirectedTreatment;
                return this;
            }
            public Builder SetTagForUnderAgeOfConsent(TagForUnderAgeOfConsent? tagForUnderAgeOfConsent)
            {
                this.TagForUnderAgeOfConsent = tagForUnderAgeOfConsent;
                return this;
            }

            public Builder SetTestDeviceIds(List<string> testDeviceIds)
            {
                this.TestDeviceIds = testDeviceIds;
                return this;
            }

            public RequestConfiguration build()
            {
                return new RequestConfiguration(this);
            }

        }

    }
}
