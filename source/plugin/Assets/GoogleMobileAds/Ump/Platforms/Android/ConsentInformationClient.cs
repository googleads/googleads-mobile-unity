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

using GoogleMobileAds.Ump.Api;
using GoogleMobileAds.Ump.Common;

namespace GoogleMobileAds.Ump.Android
{
    internal class ConsentInformationClient : IConsentInformationClient
    {
        internal OnConsentInfoUpdateSuccessListener _onSuccess;
        internal OnConsentInfoUpdateFailureListener _onFailure;
        private static readonly ConsentInformationClient _instance = new ConsentInformationClient();
        private readonly AndroidJavaObject _consentInformation;
        private readonly AndroidJavaObject _activity;

        private ConsentInformationClient()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaObject playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
                _activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass userMessagingPlatformClass =
                        new AndroidJavaClass(Utils.UserMessagingPlatformClassName);
                _consentInformation = userMessagingPlatformClass.CallStatic<AndroidJavaObject>(
                        "getConsentInformation", _activity);
            }
        }

        public static ConsentInformationClient Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// Requests consent information update.
        /// </summary>
        /// <param name="request">The request params.</param>
        /// <param name="onConsentInfoUpdateSuccessCallback">Called when ConsentStatus is updated.
        /// <param name="onConsentInfoUpdateFailureCallback">Called on consent form request error.
        /// Includes a UmpResult argument if show failed. </param>
        public void Update(ConsentRequestParameters request,
                           Action onConsentInfoUpdateSuccessCallback,
                           Action<FormError> onConsentInfoUpdateFailureCallback)
        {
            _onSuccess = new OnConsentInfoUpdateSuccessListener(onConsentInfoUpdateSuccessCallback);
            _onFailure = new OnConsentInfoUpdateFailureListener(onConsentInfoUpdateFailureCallback);
            AndroidJavaObject consentRequestParameters =
                    Utils.GetConsentRequestParametersJavaObject(request, _activity);

            if (Application.platform == RuntimePlatform.Android)
            {
                _consentInformation.Call("requestConsentInfoUpdate", _activity,
                                         consentRequestParameters, _onSuccess, _onFailure);
            }
        }

        /// <summary>
        /// Clears all consent status from persistent storage.
        /// </summary>
        public void Reset()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                _consentInformation.Call("reset");
            }
            else
            {
                throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name
                                                    + " on non-Android runtime");
            }
        }

        /// <summary>
        /// Gets the current consent status.
        /// <para>This value is cached between app sessons and can be read before
        /// requesting updated parameters.</para>
        /// </summary>
        public int GetConsentStatus()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return _consentInformation.Call<int>("getConsentStatus");
            }
            throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
                                                " on non-Android runtime");
        }

        /// <summary>
        /// Returns <c>true</c> if a <see cref="GoogleMobileAds.Ump.Api.ConsentForm">
        /// ConsentForm</see> is available, <c>false</c> otherwise.
        /// </summary>
        public bool IsConsentFormAvailable()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return _consentInformation.Call<bool>("isConsentFormAvailable");
            }
            throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
                                                " on non-Android runtime");
        }
    }
}
#endif
