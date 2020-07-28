// Copyright 2018 Google LLC
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

namespace GoogleMobileAds.Common.Mediation.MyTarget
{
    public interface IMyTargetClient
    {
        // Forward the user's consent status to the MyTarget SDK
        void SetUserConsent (bool userConsent);

        // Sets a flag to indicate if the user is known to be in an
        // age-restricted category (i.e., under the age of 16)
        void SetUserAgeRestricted (bool userAgeRestricted);

        // Gets the user's current consent status
        bool IsConsent ();

        // Gets the flag indicating that the user is known to be in
        // an age-restricted category (i.e., under the age of 16)
        bool IsUserAgeRestricted ();
    }
}
