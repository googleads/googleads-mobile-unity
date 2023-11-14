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

using AOT;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

using GoogleMobileAds.Ump.Api;
using GoogleMobileAds.Ump.Common;

namespace GoogleMobileAds.Ump.iOS
{
    public class ConsentInformationClient : IConsentInformationClient
    {
        #region Consent Information Callback
        internal delegate void GADUConsentInfoUpdateCallback(IntPtr clientRef, IntPtr errorRef);
        #endregion

        private static readonly ConsentInformationClient _instance = new ConsentInformationClient();
        private readonly IntPtr _consentInformationClientPtr;
        private IntPtr _consentInformationPtr;
        private Action _consentInfoUpdateSuccessAction;
        private Action<FormError> _consentInfoUpdateFailureAction;

        private IntPtr ConsentInformationPtr
        {
            get
            {
                return _consentInformationPtr;
            }

            set
            {
                Externs.GADURelease(_consentInformationPtr);
                _consentInformationPtr = value;
            }
        }

        private ConsentInformationClient()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _consentInformationClientPtr = (IntPtr)GCHandle.Alloc(this);
                ConsentInformationPtr =
                        Externs.GADUCreateConsentInformation(_consentInformationClientPtr);
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
            _consentInfoUpdateSuccessAction = onConsentInfoUpdateSuccessCallback;
            _consentInfoUpdateFailureAction = onConsentInfoUpdateFailureCallback;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                IntPtr requestParametersPtr = Externs.GADUCreateRequestParameters();
                Externs.GADUSetRequestParametersTagForUnderAgeOfConsent(requestParametersPtr,
                        request.TagForUnderAgeOfConsent);

                IntPtr debugSettingsPtr = Externs.GADUCreateDebugSettings();
                Externs.GADUSetDebugSettingsDebugGeography(debugSettingsPtr,
                        (int)request.ConsentDebugSettings.DebugGeography);
                int testDeviceHashedIdsCount =
                        request.ConsentDebugSettings.TestDeviceHashedIds.Count;
                if (testDeviceHashedIdsCount > 0)
                {
                    string[] TestDeviceHashedIdsArray = new string[testDeviceHashedIdsCount];
                    request.ConsentDebugSettings.TestDeviceHashedIds
                            .CopyTo(TestDeviceHashedIdsArray);
                    Externs.GADUSetDebugSettingsTestDeviceIdentifiers(debugSettingsPtr,
                            TestDeviceHashedIdsArray, testDeviceHashedIdsCount);
                }

                Externs.GADUSetRequestParametersDebugSettings(requestParametersPtr,
                        debugSettingsPtr);
                Externs.GADURequestConsentInfoUpdate(ConsentInformationPtr,
                        requestParametersPtr, ConsentInfoUpdateCallback);
            }
        }

        /// <summary>
        /// Clears all consent status from persistent storage.
        /// </summary>
        public void Reset()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Externs.GADUResetConsentInformation(ConsentInformationPtr);
            }
            else
            {
                throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name
                                                    + " on non-iOS runtime");
            }
        }

        /// <summary>
        /// Gets the current consent status.
        /// <para>This value is cached between app sessons and can be read before
        /// requesting updated parameters.</para>
        /// </summary>
        public int GetConsentStatus()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                int status = Externs.GADUGetConsentStatus(ConsentInformationPtr);
                /// The consent status values change depending on the runtime platform.
                /// iOS - https://developers.google.com/admob/ios/privacy/api/reference/Enums/UMPConsentStatus
                /// Android - https://developers.google.com/admob/android/privacy/api/reference/com/google/android/ump/ConsentInformation.ConsentStatus
                /// We are converting consent status values to match those in Unity and Android.
                switch(status) {
                    case 1: // UMPConsentInformation.UMPConsentStatusRequired
                        return (int) ConsentStatus.Required;
                    case 2: // UMPConsentInformation.UMPConsentStatusNotRequired
                        return (int) ConsentStatus.NotRequired;
                    case 0: // UMPConsentInformation.UMPConsentStatusUnknown
                    case 3: // UMPConsentInformation.UMPConsentStatusObtained
                    default:
                        return status;
                }
            }
            throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
                                                " on non-iOS runtime");
        }

        /// <summary>
        /// Get the privacy options requirement status.
        /// </summary>
        public int GetPrivacyOptionsRequirementStatus()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                int status = Externs.GADUGetPrivacyOptionsRequirementStatus(ConsentInformationPtr);
                /// The privacy options values change depending on the runtime platform.
                /// iOS - https://developers.google.com/admob/ios/privacy/api/reference/Enums/UMPPrivacyOptionsRequirementStatus
                /// Android - https://developers.google.com/admob/android/privacy/api/reference/com/google/android/ump/ConsentInformation.PrivacyOptionsRequirementStatus
                /// We are converting privacy options values to match those in Unity and Android.
                switch(status) {
                    case 1: // UMPConsentInformation.UMPPrivacyOptionsRequirementStatusRequired
                        return (int) PrivacyOptionsRequirementStatus.Required;
                    case 2: // UMPConsentInformation.UMPPrivacyOptionsRequirementStatusNotRequired
                        return (int) PrivacyOptionsRequirementStatus.NotRequired;
                    case 0: // UMPConsentInformation.UMPPrivacyOptionsRequirementStatusUnknown
                    default:
                        return status;
                }
            }
            throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
                                                " on non-iOS runtime");
        }

        /// <summary>
        /// Check if the app has finished all the required consent flow and can request ads now.
        /// A return value of true means the app can request ads now.
        /// </summary>
        public bool CanRequestAds()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return Externs.GADUUMPCanRequestAds(ConsentInformationPtr);
            }
            throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
                                                " on non-iOS runtime");
        }

        /// <summary>
        /// Returns <c>true</c> if a <see cref="GoogleMobileAds.Ump.Api.ConsentForm">
        /// ConsentForm</see> is available, <c>false</c> otherwise.
        /// </summary>
        public bool IsConsentFormAvailable()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return Externs.GADUIsConsentFormAvailable(ConsentInformationPtr);
            }
            throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
                                                " on non-iOS runtime");
        }

        [AOT.MonoPInvokeCallback(typeof(GADUConsentInfoUpdateCallback))]
        internal static void ConsentInfoUpdateCallback(IntPtr clientRef,
                                                       IntPtr errorRef)
        {
            ConsentInformationClient client = IntPtrToConsentInformationClient(clientRef);
            if (errorRef == IntPtr.Zero)
            {
                if (client._consentInfoUpdateSuccessAction != null)
                {
                    client._consentInfoUpdateSuccessAction();
                }
            }
            else if (client._consentInfoUpdateFailureAction != null)
            {
                int code = 5;
                string message = "Internal error.";
                try
                {
                    code =  Externs.GADUGetFormErrorCode(errorRef);
                    message = Externs.GADUGetFormErrorMessage(errorRef);
                }
                catch (System.EntryPointNotFoundException e)
                {
                    Debug.Log(@"Called " + MethodBase.GetCurrentMethod().Name +
                              " on non-iOS runtime." + e.Message);
                }
                client._consentInfoUpdateFailureAction(new FormError(code, message));
            }
        }

        private static ConsentInformationClient IntPtrToConsentInformationClient(
                IntPtr clientRef)
        {
            GCHandle handle = (GCHandle)clientRef;
            return handle.Target as ConsentInformationClient;
        }

        public void Dispose()
        {
            ((GCHandle)_consentInformationClientPtr).Free();
        }

        ~ConsentInformationClient()
        {
            Dispose();
        }
    }
}
#endif
