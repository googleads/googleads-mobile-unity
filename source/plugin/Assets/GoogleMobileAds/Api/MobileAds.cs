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

using System;
using System.Reflection;

using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
    public class MobileAds
    {
        private static readonly IMobileAdsClient client = GetMobileAdsClient();

        public static void Initialize(string appId)
        {
            client.Initialize(appId);
        }

        private static IMobileAdsClient GetMobileAdsClient()
        {
            Type googleMobileAdsClientFactory = Type.GetType(
                "GoogleMobileAds.GoogleMobileAdsClientFactory,Assembly-CSharp");
            MethodInfo method = googleMobileAdsClientFactory.GetMethod(
                "MobileAdsInstance",
                BindingFlags.Static | BindingFlags.Public);
            return (IMobileAdsClient)method.Invoke(null, null);
        }
    }
}