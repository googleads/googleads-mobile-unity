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
    internal class ConsentFormClient : AndroidJavaProxy, IConsentFormClient
    {
        internal OnConsentFormLoadSuccessListener _onSuccess;
        internal OnConsentFormLoadFailureListener _onFailure;
        internal OnConsentFormDismissedListener _onDismissed;
        private static readonly ConsentFormClient _instance = new ConsentFormClient();
        private readonly AndroidJavaObject _activity;
        private readonly AndroidJavaObject _userMessagingPlatformClass;
        private AndroidJavaObject _consentForm;
        private AndroidJavaObject _unityConsentForm;
        private Action<FormError> _onConsentFormDismissed;

        public ConsentFormClient() : base(Utils.UnityConsentFormCallbackClassName)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaObject playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
                _activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
                _userMessagingPlatformClass =
                        new AndroidJavaClass(Utils.UserMessagingPlatformClassName);
                _unityConsentForm = new AndroidJavaObject(Utils.UnityConsentFormClassName,
                                                          _activity, this);
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
                        // Logging a message as the user won't be able to see any callbacks fired if
                        // there is an error.
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
            _onConsentFormDismissed = onDismissed;

            if (Application.platform == RuntimePlatform.Android)
            {
                _activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    try
                    {
                        _unityConsentForm.Call("show", _consentForm);
                    }
                    catch (Exception e)
                    {
                        // Logging a message as the user won't be able to see any callbacks fired if
                        // there is an error.
                        Debug.LogError("Error calling show: " + e.StackTrace);
                    }
                }));
            }
        }

        /// <summary>
        /// Load and show the consent form when the user consent is required but not yet obtained.
        /// <param name="onDismissed">The listener that gets called when the consent form is
        /// dismissed or fails to show.</param>
        /// </summary>
        public void LoadAndShowConsentFormIfRequired(Action<FormError> onDismissed)
        {
            _onConsentFormDismissed = onDismissed;

            if (Application.platform == RuntimePlatform.Android)
            {
                _activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    try
                    {
                        _unityConsentForm.Call("loadAndShowConsentFormIfRequired");
                    }
                    catch (Exception e)
                    {
                        // Logging a message as the user won't be able to see any callbacks fired if
                        // there is an error.
                        Debug.LogError("Error calling loadAndShowConsentFormIfRequired: " +
                                       e.Message);
                    }
                }));
            }
        }

        /// <summary>
        /// Show the privacy options form when the privacy options are required.
        /// <param name="onDismissed">The listener that gets called when the privacy options form is
        /// dismissed or fails to show.</param>
        /// </summary>
        public void ShowPrivacyOptionsForm(Action<FormError> onDismissed)
        {
            _onConsentFormDismissed = onDismissed;

            if (Application.platform == RuntimePlatform.Android)
            {
                _activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    try
                    {
                        _unityConsentForm.Call("showPrivacyOptionsForm");
                    }
                    catch (Exception e)
                    {
                        // Logging a message as the user won't be able to see any callbacks fired if
                        // there is an error.
                        Debug.LogError("Error calling showPrivacyOptionsForm: " + e.Message);
                    }
                }));
            }
        }

        #region Callbacks from UnityConsentFormCallback.

        public void onConsentFormDismissed(AndroidJavaObject error)
        {
            if (_onConsentFormDismissed != null)
            {
                var args = error == null ? null : new FormError(error.Call<int>("getErrorCode"),
                                                                error.Call<string>("getMessage"));
                _onConsentFormDismissed(args);
                _onConsentFormDismissed = null;
            }
            else
            {
                Debug.LogError("The consent form dismiss callback is null.");
            }
        }

        #endregion
    }
}
#endif
