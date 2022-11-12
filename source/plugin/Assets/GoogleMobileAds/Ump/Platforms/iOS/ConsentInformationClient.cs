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

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

using GoogleMobileAds.Ump.Common;

namespace GoogleMobileAds.Ump.iOS
{
    public class ConsentInformationClient : IConsentInformationClient
    {
        private static readonly ConsentInformationClient instance = new ConsentInformationClient();
        private readonly IntPtr _consentInformationClientPtr;
        private IntPtr _consentInformationPtr;
        private Action _consentInfoUpdateSuccessAction;
        private Action<FormError> _consentInfoUpdateFailureAction;
        internal delegate void GADUUMPConsentInfoUpdateCallback(
                IntPtr clientRef, IntPtr errorRef);

        private static int code = 123;
        private static string message = "Test Error";
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
                return instance;
            }
        }

        /// <summary>
        /// Requests consent information update.
        /// </summary>
        /// <param name="request">The request params.</param>
        /// <param name="onConsentInfoUpdateSuccessCallback">Called when ConsentStatus is updated.
        /// <param name="onConsentInfoUpdateFailureCallback">Called on consent form request error.
        /// Includes a UmpResult argument if show failed. </param>
        public void RequestConsentInfoUpdate(ConsentRequestParameters request,
                Action onConsentInfoUpdateSuccessCallback,
                Action<FormError> onConsentInfoUpdateFailureCallback)
        {
            _consentInfoUpdateSuccessAction = onConsentInfoUpdateSuccessCallback;
            _consentInfoUpdateFailureAction = onConsentInfoUpdateFailureCallback;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                IntPtr requestParametersPtr = Externs.GADUCreateUMPRequestParameters();
                Externs.GADUSetUMPRequestParametersTagForUnderAgeOfConsent(requestParametersPtr,
                        request.TagForUnderAgeOfConsent);

                IntPtr debugSettingsPtr = Externs.GADUCreateUMPDebugSettings();
                Externs.GADUSetUMPDebugSettingsDebugGeography(debugSettingsPtr,
                        (int)request.ConsentDebugSettings.TestDebugGeography);
                int testDeviceHashedIdsCount = request.ConsentDebugSettings.TestDeviceHashedIds.Count;
                if (testDeviceHashedIdsCount > 0)
                {
                    string[] TestDeviceHashedIdsArray = new string[testDeviceHashedIdsCount];
                    request.ConsentDebugSettings.TestDeviceHashedIds.CopyTo(TestDeviceHashedIdsArray);
                    Externs.GADUSetUMPDebugSettingsTestDeviceIdentifiers(debugSettingsPtr,
                            TestDeviceHashedIdsArray, testDeviceHashedIdsCount);
                }

                Externs.GADUSetUMPRequestParametersUMPDebugSettings(requestParametersPtr,
                        debugSettingsPtr);
                Externs.GADUUMPRequestConsentInfoUpdate(ConsentInformationPtr,
                        requestParametersPtr, UMPConsentInfoUpdateCallback);
            }
        }

        /// <summary>
        /// Clears all consent status from persistent storage.
        /// </summary>
        public void ResetInfo()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Externs.GADUUMPConsentInformationReset(ConsentInformationPtr);
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
                return Externs.GADUUMPGetConsentStatus(ConsentInformationPtr);
            }
            throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
                    " on non-iOS runtime");
        }

        /// <summary>
        /// Returns <c>true</c> if a <see cref="GoogleMobileAds.Ump.Api.ConsentInformation">
        /// ConsentInformation</see> is available, <c>false</c> otherwise.
        /// </summary>
        public bool IsConsentFormAvailable()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return Externs.GADUUMPIsConsentFormAvailable(ConsentInformationPtr);
            }
            throw new InvalidOperationException(@"Called " + MethodBase.GetCurrentMethod().Name +
                    " on non-iOS runtime");
        }

        [MonoPInvokeCallback(typeof(GADUUMPConsentInfoUpdateCallback))]
        internal static void UMPConsentInfoUpdateCallback(IntPtr clientRef,
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
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    code = Externs.GADUGetFormErrorCode(errorRef);
                    message = Externs.GADUGetFormErrorMessage(errorRef);
                }
                // Run the failure callback, if available.
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
