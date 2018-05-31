// Copyright (C) 2017 Google, Inc.
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

using UnityEngine;
using System.Collections;

namespace GoogleMobileAds.Common.Mediation.AppLovin
{
    public interface IAppLovinClient
    {
        // Initialize the AppLovin SDK.
        void Initialize();

        // Sets a flag indicating whether a user located in the
        // European Union (i.e., EU/GDPR data subject) has provided
        // opt-in consent for the collection and use of personal data.
        void SetHasUserConsent(bool hasUserConsent);

        // Sets a flag indicating whether a user is known to be
        // in an age-restricted category (i.e., under the age of 16).
        void SetIsAgeRestrictedUser(bool isAgeRestrictedUser);
    }
}
