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
using System.Runtime.InteropServices;
using UnityEngine;

using GoogleMobileAds.Ump.Common;

namespace GoogleMobileAds.Ump.iOS
{
    public class ConsentFormClient : IConsentFormClient
    {
        private static readonly ConsentFormClient _instance = new ConsentFormClient();
        private readonly IntPtr _consentForm;
        private readonly IntPtr _consentFormClientPtr;
        private Action _loadCompleteAction;
        private Action<FormError> _loadFailedAction;
        private Action<FormError> _consentFormDismissedAction;
        internal delegate void GADUUMPConsentFormLoadCompleteCallback(IntPtr clientRef,
                IntPtr errorRef);
        internal delegate void GADUUMPConsentFormPresentCompleteCallback(IntPtr clientRef,
                IntPtr errorRef);
        private static int code = 123;
        private static string message = "Test Error";

        private ConsentFormClient()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _consentFormClientPtr = (IntPtr)GCHandle.Alloc(this);
                _consentForm = Externs.GADUCreateConsentForm(_consentFormClientPtr);
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
        /// <param name="onError">Called when the consent form fails to load.</param>
        /// </summary>
        public void LoadConsentForm(Action onLoad, Action<FormError> onError)
        {
            _loadCompleteAction = onLoad;
            _loadFailedAction = onError;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Externs.GADUUMPLoadConsentForm(_consentForm,
                        UMPConsentFormLoadCompleteCallback);
            }
        }

        /// <summary>
        /// Shows the consent form.
        /// <param name="onDismiss">Called when the consent form is dismissed. </param>
        /// </summary>
        public void Show(Action<FormError> onDismiss)
        {
            _consentFormDismissedAction = onDismiss;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Externs.GADUUMPPresentConsentForm(_consentForm,
                        UMPConsentFormPresentCompleteCallback);
            }
        }

        [MonoPInvokeCallback(typeof(GADUUMPConsentFormLoadCompleteCallback))]
        internal static void UMPConsentFormLoadCompleteCallback(IntPtr clientRef, IntPtr errorRef)
        {
            ConsentFormClient client = IntPtrToConsentFormClient(clientRef);
            if (errorRef == IntPtr.Zero)
            {
                if (client._loadCompleteAction != null)
                {
                    client._loadCompleteAction();
                }
            }
            else if (client._loadFailedAction != null)
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    code = Externs.GADUGetFormErrorCode(errorRef);
                    message = Externs.GADUGetFormErrorMessage(errorRef);
                }
                // Run the failure callback, if available.
                client._loadFailedAction(new FormError(code, message));
            }
        }

        [MonoPInvokeCallback(typeof(GADUUMPConsentFormPresentCompleteCallback))]
        internal static void UMPConsentFormPresentCompleteCallback(IntPtr clientRef, IntPtr errorRef)
        {
            ConsentFormClient client = IntPtrToConsentFormClient(clientRef);
            if (client._consentFormDismissedAction == null)
            {
                return;
            }

            FormError formError = null;
            if (errorRef != IntPtr.Zero)
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    code = Externs.GADUGetFormErrorCode(errorRef);
                    message = Externs.GADUGetFormErrorMessage(errorRef);
                }
                formError = new FormError(code, message);
            }

            // Run the callback, if available.
            client._consentFormDismissedAction(formError);
        }

        private static ConsentFormClient IntPtrToConsentFormClient(IntPtr clientRef)
        {
            GCHandle handle = (GCHandle)clientRef;
            return handle.Target as ConsentFormClient;
        }

        public void Dispose()
        {
            ((GCHandle)_consentFormClientPtr).Free();
        }

        ~ConsentFormClient()
        {
            Dispose();
        }
    }
}
#endif
