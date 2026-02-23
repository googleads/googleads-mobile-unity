// Copyright (C) 2022 Google LLC
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

namespace GoogleMobileAds.Ump.Api
{
    /// <summary>
    /// Parameters sent on updating user consent info.
    /// </summary>
    [System.Serializable]
    public class ConsentRequestParameters
    {
        /// <summary>
        /// Determines whether the user is tagged for under the age of consent.
        /// </summary>
        public bool TagForUnderAgeOfConsent;

        /// <summary>
        /// The consent sync ID used to sync the user consent status.
        /// </summary>
        /// <remarks>
        /// <para>The consent sync ID must meet the following requirements:</para>
        /// <list type="bullet">
        /// <item><description>Constructed as a UUID string as matches the regular expression
        /// (regex) ^[0-9a-zA-Z+.=\/_\-$,{}]{22,150}$</description></item>
        /// <item><description>A minimum of 22 characters.</description></item>
        /// <item><description>A maximum of 150 characters.</description></item>
        /// </list>
        /// <para>Failure to meet the requirements results in the consent sync ID not being set and
        /// the UMP SDK logging a warning to the console.</para>
        /// </remarks>
        public string ConsentSyncId;

        /// <summary>
        /// Debug settings for the request.
        /// </summary>
        public ConsentDebugSettings ConsentDebugSettings = new ConsentDebugSettings();
    }
}
