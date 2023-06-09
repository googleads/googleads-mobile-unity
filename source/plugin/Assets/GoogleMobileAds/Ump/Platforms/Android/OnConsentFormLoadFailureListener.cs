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

using GoogleMobileAds.Ump.Api;

namespace GoogleMobileAds.Ump.Android
{
    public class OnConsentFormLoadFailureListener : AndroidJavaProxy
    {
        private readonly Action<FormError> _onConsentFormLoadFailureAction;

        public OnConsentFormLoadFailureListener(Action<FormError> onConsentFormLoadFailureAction)
            : base(Utils.OnConsentFormLoadFailureListenerClassName)
        {
            _onConsentFormLoadFailureAction = onConsentFormLoadFailureAction;
        }

        #region Callbacks from ConsentForm

        // Proxy method for onConsentFormLoadFailure in Android UMP SDK.
        internal void onConsentFormLoadFailure(AndroidJavaObject error)
        {
            if (_onConsentFormLoadFailureAction == null)
            {
                Debug.Log("onError action cannot be null.");
                return;
            }
            var args = error == null ? null : new FormError(error.Call<int>("getErrorCode"),
                                                            error.Call<string>("getMessage"));
            _onConsentFormLoadFailureAction(args);
        }

        #endregion
    }
}
#endif
