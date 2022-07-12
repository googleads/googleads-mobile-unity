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

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Android
{
    /// <summary>
    /// Bridge interface for UnityAdInspectorListener.java
    /// </summary>
    public class AdInspectorListener : AndroidJavaProxy
    {
        Action<IAdErrorClient> _completeHandler;

        public AdInspectorListener(Action<IAdErrorClient> completeHandler)
            : base(Utils.UnityAdInspectorListenerClassname)
        {
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.AdInspectorListener()");
            //END_DEBUG_STRIP
            _completeHandler = completeHandler;
        }


        #region Callbacks from UnityAdInspectorListener

        void onAdInspectorClosed(AndroidJavaObject error)
        {
            //START_DEBUG_STRIP
            UnityEngine.Debug.Log("Android.AdInspectorListener onAdInspectorClosed");
            //END_DEBUG_STRIP
            if(_completeHandler == null)
            {
                return;
            }

            // Success
            if (error == null)
            {
                _completeHandler(null);
                return;
            }

            _completeHandler(new AdErrorClient(error));
        }

        #endregion
    }
}
