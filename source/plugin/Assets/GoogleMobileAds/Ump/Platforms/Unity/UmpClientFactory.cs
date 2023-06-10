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

using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;

using GoogleMobileAds.Ump.Common;

namespace GoogleMobileAds.Ump.Unity
{
    [Preserve]
    public class UmpClientFactory : IUmpClientFactory
    {
        /// <summary>
        /// Creates and returns an instance of  a Unity Editor client for
        /// <see cref="GoogleMobileAds.Ump.Api.ConsentForm">ConsentForm</see>.
        /// </summary>
        public IConsentFormClient ConsentFormClient()
        {
            Debug.Log("Placeholder " + MethodBase.GetCurrentMethod().Name);
            return new ConsentFormClient();
        }

        /// <summary>
        /// Creates and returns an instance of a Unity Editor client for
        /// <see cref="GoogleMobileAds.Ump.Api.ConsentInformation">ConsentInformation</see>.
        /// </summary>
        public IConsentInformationClient ConsentInformationClient()
        {
            Debug.Log("Placeholder " + MethodBase.GetCurrentMethod().Name);
            return new ConsentInformationClient();
        }
    }
}
