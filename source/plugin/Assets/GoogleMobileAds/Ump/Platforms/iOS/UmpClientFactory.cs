#if UNITY_IOS

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
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;

using GoogleMobileAds.Ump.Common;

namespace GoogleMobileAds.Ump.iOS
{
    [Preserve]
    public class UmpClientFactory : IUmpClientFactory
    {
        public IConsentFormClient ConsentFormClient()
        {
            /// <summary>
            /// <see cref="ConsentFormClient"/> can only be instantiated in iOS runtime.
            /// </summary>
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return GoogleMobileAds.Ump.iOS.ConsentFormClient.Instance;
            }
            throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
                                                " on non-iOS runtime");
        }

        public IConsentInformationClient ConsentInformationClient()
        {
            /// <summary>
            /// <see cref="ConsentInformationClient"/> can only be instantiated in iOS runtime.
            /// </summary>
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return GoogleMobileAds.Ump.iOS.ConsentInformationClient.Instance;
            }
            throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
                                                " on non-iOS runtime");
        }
    }
}
#endif
