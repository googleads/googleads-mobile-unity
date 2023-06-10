#if UNITY_ANDROID
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

namespace GoogleMobileAds.Ump.Android
{
    [Preserve]
    public class UmpClientFactory : IUmpClientFactory
    {
        public IConsentFormClient ConsentFormClient()
        {
            /// <summary>
            /// <see cref="ConsentFormClient"/> can only be instantiated in Android runtime.
            /// </summary>
            if (Application.platform == RuntimePlatform.Android)
            {
                return GoogleMobileAds.Ump.Android.ConsentFormClient.Instance;
            }
            throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
                                                " on non-Android runtime");
        }

        public IConsentInformationClient ConsentInformationClient()
        {
            /// <summary>
            /// <see cref="ConsentInformationClient"/> can only be instantiated in Android runtime.
            /// </summary>
            if (Application.platform == RuntimePlatform.Android)
            {
                return GoogleMobileAds.Ump.Android.ConsentInformationClient.Instance;
            }
            throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
                                                " on non-Android runtime");
        }
    }
}
#endif
