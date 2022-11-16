#if UNITY_ANDROID
// Copyright 2022 Google LLC.
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

namespace GoogleMobileAds.Ump.Android
{
    public class OnConsentFormDismissedListener : AndroidJavaProxy
    {
        private Action<FormError> _onConsentFormDismissedAction;

        public OnConsentFormDismissedListener(Action<FormError> _onConsentFormDismissedAction)
            : base(Utils.OnConsentFormDismissedListenerClassName)
        {
            this._onConsentFormDismissedAction = _onConsentFormDismissedAction;
        }

        #region Callbacks from ConsentForm

        // Proxy method for onConsentFormDismissed in Android UMP SDK.
        internal void onConsentFormDismissed(AndroidJavaObject error)
        {
            if (_onConsentFormDismissedAction == null)
            {
                return;
            }
            var args = error == null ? null : new FormError(error.Call<int>("getErrorCode"),
                                                            error.Call<string>("getMessage"));
            // Run the callback, if available.
            _onConsentFormDismissedAction(args);
        }

        #endregion
    }
}
#endif
