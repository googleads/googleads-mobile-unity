// Copyright (C) 2017 Google, Inc.
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
    public class MobileAdsClient : AndroidJavaProxy, IMobileAdsClient
    {
        private static MobileAdsClient instance = new MobileAdsClient();
        public static MobileAdsClient Instance
        {
            get
            {
                return instance;
            }
        }

        private AndroidJavaClass _playerClass;
        private AndroidJavaClass _mobileAdsClass;
        private AndroidJavaObject _activity;
        private Action<IInitializationStatusClient> initCompleteAction;

        public MobileAdsClient() : base(Utils.OnInitializationCompleteListenerClassName)
        {
            _playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            _activity = _playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            _mobileAdsClass = new AndroidJavaClass(Utils.MobileAdsClassName);
        }

        public void Initialize(Action<IInitializationStatusClient> initCompleteAction)
        {
            this.initCompleteAction = initCompleteAction;
            _mobileAdsClass.CallStatic("initialize", _activity, this);
        }

        public void SetApplicationVolume(float volume)
        {
            _mobileAdsClass.CallStatic("setAppVolume", volume);
        }

        public void DisableMediationInitialization()
        {
            _mobileAdsClass.CallStatic("disableMediationAdapterInitialization", _activity);
        }

        public void SetApplicationMuted(bool muted)
        {
            _mobileAdsClass.CallStatic("setAppMuted", muted);
        }

        public void SetRequestConfiguration(RequestConfiguration requestConfiguration)
        {
            AndroidJavaObject requestConfigurationAndroidObject =
                RequestConfigurationClient.BuildRequestConfiguration(requestConfiguration);
            _mobileAdsClass.CallStatic("setRequestConfiguration", requestConfigurationAndroidObject);
        }

        public RequestConfiguration GetRequestConfiguration()
        {
            AndroidJavaObject androidRequestConfiguration =
                _mobileAdsClass.CallStatic<AndroidJavaObject>("getRequestConfiguration");
            RequestConfiguration requestConfiguration =
                RequestConfigurationClient.GetRequestConfiguration(androidRequestConfiguration);
            return requestConfiguration;
        }

        public void SetiOSAppPauseOnBackground(bool pause)
        {
            // Do nothing on Android. Default behavior is to pause when app is backgrounded.
        }

        public void OpenAdInspector(Action<IAdErrorClient> onAdInspectorClosed)
        {
            AndroidJavaClass adInspectorClass =
                        new AndroidJavaClass(Utils.UnityAdInspectorClassName);
            AdInspectorListener listener = new AdInspectorListener(onAdInspectorClosed);
            adInspectorClass.CallStatic("openAdInspector", _activity, listener);
        }

        public float GetDeviceScale()
        {
            AndroidJavaObject resources = _activity.Call<AndroidJavaObject>("getResources");
            AndroidJavaObject metrics = resources.Call<AndroidJavaObject>("getDisplayMetrics");
            return metrics.Get<float>("density");
        }

        public int GetDeviceSafeWidth()
        {
            return Utils.GetScreenWidth();
        }

        #region Callbacks from OnInitializationCompleteListener.

        public void onInitializationComplete(AndroidJavaObject initStatus)
        {
            if (initCompleteAction != null)
            {
                IInitializationStatusClient statusClient
                    = new InitializationStatusClient(initStatus);
                initCompleteAction(statusClient);
            }
        }

        #endregion

    }
}
