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

using UnityEngine;

using GoogleMobileAds.Ump.Api;

namespace GoogleMobileAds.Ump.Android
{
    internal class Utils
    {
        #region Fully-qualified class names

            #region UMP SDK class names

            public const string ConsentDebugSettingsBuilderClassName =
                    "com.google.android.ump.ConsentDebugSettings$Builder";

            public const string UnityConsentFormClassName = "com.google.unity.ump.UnityConsentForm";

            public const string UnityConsentFormCallbackClassName =
                    "com.google.unity.ump.UnityConsentFormCallback";

            public const string ConsentRequestParametersBuilderClassName =
                    "com.google.android.ump.ConsentRequestParameters$Builder";

            public const string UserMessagingPlatformClassName =
                    "com.google.android.ump.UserMessagingPlatform";

            public const string OnConsentInfoUpdateSuccessListenerClassName =
                    "com.google.android.ump.ConsentInformation$OnConsentInfoUpdateSuccessListener";

            public const string OnConsentInfoUpdateFailureListenerClassName =
                    "com.google.android.ump.ConsentInformation$OnConsentInfoUpdateFailureListener";

            public const string OnConsentFormLoadSuccessListenerClassName =
                    "com.google.android.ump.UserMessagingPlatform$OnConsentFormLoadSuccessListener";

            public const string OnConsentFormLoadFailureListenerClassName =
                    "com.google.android.ump.UserMessagingPlatform$OnConsentFormLoadFailureListener";

            public const string OnConsentFormDismissedListenerClassName =
                    "com.google.android.ump.ConsentForm$OnConsentFormDismissedListener";

            #endregion

            #region Unity class names

            public const string UnityActivityClassName = "com.unity3d.player.UnityPlayer";

            #endregion

        #endregion

        #region JavaObject creators

        // TODO: b/284206705 - we need to revisit the way we are testing the platform code.
        // We can use integration tests for the untestable platform dependencies. For now, we are
        // using this static internal varialbe for dependency injection that returns an
        // AndroidJavaObject for the unit test (e.g: TestUpdateSuccess in 
        // ConsentInformationClientTestLine 32).

        public static AndroidJavaObject GetConsentRequestParametersJavaObject(
                ConsentRequestParameters consentRequestParameters, AndroidJavaObject activity)
        {
            // Build ConsentRequestParameters object
            AndroidJavaObject consentRequestParametersBuilder =
                    new AndroidJavaObject(ConsentRequestParametersBuilderClassName);

            //Set TagForUnderAgeOfConsent
            consentRequestParametersBuilder =
                    consentRequestParametersBuilder.Call<AndroidJavaObject>(
                            "setTagForUnderAgeOfConsent",
                            consentRequestParameters.TagForUnderAgeOfConsent);

            // Build ConsentDebugSettings object
            ConsentDebugSettings consentDebugSettings =
                    consentRequestParameters.ConsentDebugSettings;
            AndroidJavaObject consentDebugSettingsBuilder =
                    new AndroidJavaObject(ConsentDebugSettingsBuilderClassName, activity);

            consentDebugSettingsBuilder = consentDebugSettingsBuilder.Call<AndroidJavaObject>(
                    "setDebugGeography", (int)consentDebugSettings.DebugGeography);

            // Add TestDeviceHashedIds
            if (consentDebugSettings.TestDeviceHashedIds != null &&
                consentDebugSettings.TestDeviceHashedIds.Count > 0)
            {
                foreach (string testDeviceHashedId in consentDebugSettings.TestDeviceHashedIds)
                {
                    consentDebugSettingsBuilder = consentDebugSettingsBuilder.
                            Call<AndroidJavaObject>("addTestDeviceHashedId", testDeviceHashedId);
                }
            }

            AndroidJavaObject consentDebugSettingsJavaObject =
                    consentDebugSettingsBuilder.Call<AndroidJavaObject>("build");
            consentRequestParametersBuilder =
                    consentRequestParametersBuilder.Call<AndroidJavaObject>(
                            "setConsentDebugSettings", consentDebugSettingsJavaObject);
            AndroidJavaObject consentRequestParametersJavaObject =
                    consentRequestParametersBuilder.Call<AndroidJavaObject>("build");
            return consentRequestParametersJavaObject;
        }

    #endregion

    }
}
#endif
