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

using GoogleMobileAds.Ump.Common;

namespace GoogleMobileAds.Ump.Android
{
    internal class ConsentFormClient : IConsentFormClient
    {
        private static ConsentFormClient _instance = new ConsentFormClient();
        private readonly AndroidJavaObject _userMessagingPlatformClass;
        private readonly AndroidJavaObject _playerClass;
        private readonly AndroidJavaObject _activity;
        private AndroidJavaObject _consentForm;
        internal OnConsentFormLoadSuccessListener onSuccess;
        internal OnConsentFormLoadFailureListener onFailure;
        internal OnConsentFormDismissedListener onDismiss;

        private ConsentFormClient()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                _playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
                _activity = _playerClass.GetStatic<AndroidJavaObject>("currentActivity");
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
        /// <param name="onLoad">Called when the consent form is loaded.</param>
        /// <param name="OnError">Called when the consent form fails to load.</param>
        /// </summary>
        public void LoadConsentForm(Action onLoad, Action<FormError> onError)
        {
            onSuccess = new OnConsentFormLoadSuccessListener((consentFormJavaObject) =>
                    {
                        _consentForm = consentFormJavaObject;
                        onLoad();
                    });

            onFailure = new OnConsentFormLoadFailureListener(onError);

            if (Application.platform == RuntimePlatform.Android)
            {
                _activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                    {
                        _userMessagingPlatformClass.CallStatic("loadConsentForm", _activity,
                                                              onSuccess, onFailure);
                    }));
            }
        }

        /// <summary>
        /// Shows the consent form.
        /// <param name="onDismiss">Called when the consent form is dismissed. </param>
        /// </summary>
        public void Show(Action<FormError> onDismiss)
        {
            this.onDismiss = new OnConsentFormDismissedListener(onDismiss);

            if (Application.platform == RuntimePlatform.Android)
            {
                _activity.Call("runOnUiThread",
                        new AndroidJavaRunnable(
                                () => _consentForm.Call("show", _activity, this.onDismiss)));
            }
        }
    }
}
#endif
