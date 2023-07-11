// Copyright (C) 2023 Google LLC.
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
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Android
{
    public class ApplicationPreferencesClient : IApplicationPreferencesClient
    {
        public static ApplicationPreferencesClient Instance
        {
            get
            {
                return _instance;
            }
        }

        private static ApplicationPreferencesClient _instance = new ApplicationPreferencesClient();

        public void SetInt(string key, int value)
        {
            // TODO (b/290781398): Move the logic into android-library
            AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            AndroidJavaObject activity =
                    playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass preferenceManagerClass =
                    new AndroidJavaClass(Utils.PreferenceManagerClassName);
            AndroidJavaObject sharedPreferences =
                    preferenceManagerClass.CallStatic<AndroidJavaObject>(
                            "getDefaultSharedPreferences", activity);
            AndroidJavaObject sharedPrefsEditor = sharedPreferences.Call<AndroidJavaObject>("edit");
            sharedPrefsEditor.Call<AndroidJavaObject>("putInt", key, value);
            sharedPrefsEditor.Call("apply");
        }

        public void SetString(string key, string value)
        {
            // TODO (b/290781398): Move the logic into android-library
            AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            AndroidJavaObject activity =
                    playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass preferenceManagerClass =
                    new AndroidJavaClass(Utils.PreferenceManagerClassName);
            AndroidJavaObject sharedPreferences =
                    preferenceManagerClass.CallStatic<AndroidJavaObject>(
                            "getDefaultSharedPreferences", activity);
            AndroidJavaObject sharedPrefsEditor = sharedPreferences.Call<AndroidJavaObject>("edit");
            sharedPrefsEditor.Call<AndroidJavaObject>("putString", key, value);
            sharedPrefsEditor.Call("apply");
        }
    }
}
