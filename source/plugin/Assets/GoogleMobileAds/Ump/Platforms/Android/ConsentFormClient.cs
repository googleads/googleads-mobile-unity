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
using UnityEngine;

using GoogleMobileAds.Ump.Api;
using GoogleMobileAds.Ump.Common;

namespace GoogleMobileAds.Ump.Android
{
    internal class ConsentFormClient : IConsentFormClient
    {
        internal OnConsentFormLoadSuccessListener _onSuccess;
        internal OnConsentFormLoadFailureListener _onFailure;
        internal OnConsentFormDismissedListener _onDismissed;
        private readonly AndroidJavaObject _activity;
        private readonly AndroidJavaObject _userMessagingPlatformClass;
        private AndroidJavaObject _consentForm;
        private static readonly ConsentFormClient _instance = new ConsentFormClient();

        private ConsentFormClient()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaObject playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
                _activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
                _userMessagingPlatformClass =
                        new AndroidJavaClass(Utils.UserMessagingPlatformClassName);
            }
        }

        public static ConsentFormClient Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// Loads a consent form.
        /// <paramref name="onFormLoaded">Called when the form is successfully loaded.</paramref>
        /// <paramref name="onError">Called when form couldn't load.
        /// Provides <see cref="FormError"/> as argument for details about the error.</paramref>
        /// </summary>
        public void Load(Action onFormLoaded, Action<FormError> onError)
        {
            _onSuccess = new OnConsentFormLoadSuccessListener((consentFormJavaObject) =>
                    {
                        _consentForm = consentFormJavaObject;
                        onFormLoaded();
                    });

            _onFailure = new OnConsentFormLoadFailureListener(onError);

            if (Application.platform == RuntimePlatform.Android)
            {
                _activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    try
                    {
                        _userMessagingPlatformClass.CallStatic("loadConsentForm", _activity,
                                                               _onSuccess, _onFailure);
                    }
                    catch (Exception e)
                    {
                        // Logging a message as the user won't be able to see any callbacks
                        // fired if there is an error.
                        Debug.LogError("Error calling loadConsentForm: " + e.StackTrace);
                    }
                }));
            }
        }

        /// <summary>
        /// Shows the consent form.
        /// <param name="onDismissed">Called when the consent form is dismissed. </param>
        /// </summary>
        public void Show(Action<FormError> onDismissed)
        {
            _onDismissed = new OnConsentFormDismissedListener(onDismissed);

            if (Application.platform == RuntimePlatform.Android)
            {
                _activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    try
                    {
                        _consentForm.Call("show", _activity, _onDismissed);
                    }
                    catch (Exception e)
                    {
                        // Logging a message as the user won't be able to see any callbacks
                        // fired if there is an error.
                        Debug.LogError("Error calling show: " + e.StackTrace);
                    }
                }));
            }
        }
    }
}
#endif
