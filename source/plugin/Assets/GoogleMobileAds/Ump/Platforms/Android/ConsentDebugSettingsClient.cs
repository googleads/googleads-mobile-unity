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

using UnityEngine;

using GoogleMobileAds.Api;
using System.Collections.Generic;

namespace GoogleMobileAds.Android
{
    public class ConsentDebugSettingsClient
    {
        public static AndroidJavaObject BuildConsentDebugSettings(
                ConsentDebugSettings consentDebugSettings)
        {
            AndroidJavaObject consentDebugSettingsBuilder =
                    new AndroidJavaObject(Utils.ConsentDebugSettingsBuilderClassName);

            if (consentDebugSettings.DebugGeography != null)
            {
                consentDebugSettingsBuilder = consentDebugSettingsBuilder.Call<AndroidJavaObject>(
                        "setDebugGeography", consentDebugSettings.DebugGeography.Value);
            }

            if (consentDebugSettings.TestDeviceHashedIds.Count > 0)
            {
                foreach (testDeviceHashedId in consentDebugSettings.TestDeviceHashedIds)
                {
                    consentDebugSettingsBuilder = consentDebugSettingsBuilder.
                            Call<AndroidJavaObject>("addTestDeviceHashedId", testDeviceHashedId);
                }
            }

            return consentDebugSettingsBuilder.Call<AndroidJavaObject>("build");
        }

        public static ConsentDebugSettings GetConsentDebugSettings(
                AndroidJavaObject androidConsentDebugSettings)
        {
            DebugGeography DebugGeography =
                    (DebugGeography)androidConsentDebugSettings.Call<int>("getDebugGeography");
            // Assigned empty list as the getter for TestDeviceHashedIds doesn't exist for Android.
            List<string> TestDeviceHashedIds = new List<string>();

            ConsentDebugSettings.Builder builder = new ConsentDebugSettings.Builder();
            builder = builder.SetDebugGeography(TagForChildDirectedTreatment);
            builder = builder.SetTestDeviceHashedIds(TestDeviceHashedIds);

            return builder.build();
        }
    }
}
