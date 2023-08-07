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
    public class ConsentFormClient : IConsentFormClient
    {
        # region Consent Form Callbacks
        internal delegate void GADUConsentFormLoadCompletionHandler(IntPtr clientRef,
                                                                   IntPtr errorRef);
        internal delegate void GADUConsentFormPresentCompletionHandler(IntPtr clientRef,
                                                                      IntPtr errorRef);
        # endregion

        private static readonly ConsentFormClient _instance = new ConsentFormClient();
        private readonly IntPtr _consentForm;
        private readonly IntPtr _consentFormClientPtr;
        private Action _loadCompleteAction;
        private Action<FormError> _loadFailedAction;
        private Action<FormError> _consentFormDismissedAction;

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
        /// <param name="onFormLoaded">Called when the consent form is loaded.</param>
        /// <param name="onError">Called when the consent form fails to load.</param>
        /// </summary>
        public void Load(Action onFormLoaded, Action<FormError> onError)
        {
            _loadCompleteAction = onFormLoaded;
            _loadFailedAction = onError;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Externs.GADULoadConsentForm(_consentForm,
                        ConsentFormLoadCompletionHandler);
            }
        }

        /// <summary>
        /// Shows the consent form.
        /// <param name="onDismissed">Called when the consent form is dismissed. </param>
        /// </summary>
        public void Show(Action<FormError> onDismissed)
        {
            _consentFormDismissedAction = onDismissed;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Externs.GADUPresentConsentForm(_consentForm,
                        ConsentFormPresentCompletionHandler);
            }
        }

        /// <summary>
        /// Load and show the consent form when the user consent is required but not yet obtained.
        /// <param name="onDismissed">The listener that gets called when the consent form is
        /// dismissed or fails to show.</param>
        /// </summary>
        public void LoadAndShowConsentFormIfRequired(Action<FormError> onDismissed)
        {
            _consentFormDismissedAction = onDismissed;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Externs.GADULoadAndPresentConsentForm(_consentForm,
                        ConsentFormPresentCompletionHandler);
            }
        }

        /// <summary>
        /// Show the privacy option form when the privacy options are required.
        /// <param name="onDismissed">The listener that gets called when the consent form is
        /// dismissed or fails to show.</param>
        /// </summary>
        public void ShowPrivacyOptionsForm(Action<FormError> onDismissed)
        {
            _consentFormDismissedAction = onDismissed;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Externs.GADUPresentPrivacyOptionsForm(_consentForm,
                        ConsentFormPresentCompletionHandler);
            }
        }

        [AOT.MonoPInvokeCallback(typeof(GADUConsentFormLoadCompletionHandler))]
        internal static void ConsentFormLoadCompletionHandler(IntPtr clientRef, IntPtr errorRef)
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
                int code = 7;
                string message = "Form is unavailable.";
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
                client._loadFailedAction(new FormError(code, message));
            }
        }

        [AOT.MonoPInvokeCallback(typeof(GADUConsentFormPresentCompletionHandler))]
        internal static void ConsentFormPresentCompletionHandler(
                IntPtr clientRef, IntPtr errorRef)
        {
            ConsentFormClient client = IntPtrToConsentFormClient(clientRef);
            if (client._consentFormDismissedAction == null)
            {
                return;
            }

            FormError formError = null;
            if (errorRef != IntPtr.Zero)
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
