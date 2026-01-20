// Copyright (C) 2025 Google, Inc.
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
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Android
{
    public class NextGenMobileAdsClient : AndroidJavaProxy, IMobileAdsClient
    {
        private static readonly NextGenMobileAdsClient _instance = new NextGenMobileAdsClient();
        private readonly AndroidJavaObject _mobileAdsClass;
        // Ensures InsightsEmitter is initialized from the main thread to handle CUIs.
        private readonly IInsightsEmitter _insightsEmitter = InsightsEmitter.Instance;
        private readonly ITracer _tracer;
        private Action<IInitializationStatusClient> _initCompleteAction;

        private NextGenMobileAdsClient()
            : base(NextGenUtils.OnInitializationCompleteListenerClassName)
        {
          _mobileAdsClass = new AndroidJavaClass(NextGenUtils.UnityMobileAdsClassName);
          _tracer = new Tracer(_insightsEmitter);
          // Ensures GlobalExceptionHandler is initialized from the main thread to handle Android
          // untrapped exceptions.
          var _ = GlobalExceptionHandler.Instance;
        }

        public static NextGenMobileAdsClient Instance
        {
            get { return _instance; }
        }

        public void Initialize(Action<IInitializationStatusClient> initCompleteAction)
        {
            using (_tracer.StartTrace("NextGenMobileAdsClient.Initialize"))
            {
                _initCompleteAction = initCompleteAction;

                Task.Run(() => {
                    using (_tracer.StartTrace("AttachCurrentThread"))
                    {
                        int env = AndroidJNI.AttachCurrentThread();
                        if (env < 0)
                        {
                            UnityEngine.Debug.LogError("Failed to attach current thread to JVM.");
                            return;
                        }
                    }

                    try
                    {
                        _mobileAdsClass.CallStatic("initialize",
                                                 Utils.GetCurrentActivityAndroidJavaObject(),
                                                 this);
                    }
                    finally
                    {
                        AndroidJNI.DetachCurrentThread();
                    }
                });
            }
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.SdkInitialized
            });
        }

        public void DisableMediationInitialization()
        {
            // TODO(vkini): Add disable mediation initialization to NextGen SDK.
        }

        public void SetApplicationVolume(float volume)
        {
            _mobileAdsClass.CallStatic("setUserControlledAppVolume", volume);
        }

        public void SetApplicationMuted(bool muted)
        {
            _mobileAdsClass.CallStatic("setApplicationMuted", muted);
        }

        public void SetRequestConfiguration(RequestConfiguration requestConfiguration)
        {
            if (requestConfiguration.PublisherFirstPartyIdEnabled.HasValue)
            {
                _mobileAdsClass.CallStatic<bool>("putPublisherFirstPartyIdEnabled", requestConfiguration.PublisherFirstPartyIdEnabled.Value);
            }

            AndroidJavaObject requestConfigurationAndroidObject =
                NextGenRequestConfigurationClient.BuildRequestConfiguration(requestConfiguration);
            _mobileAdsClass.CallStatic("setRequestConfiguration", requestConfigurationAndroidObject);
        }

        public RequestConfiguration GetRequestConfiguration()
        {
          AndroidJavaObject androidRequestConfiguration =
              _mobileAdsClass.CallStatic<AndroidJavaObject>("getRequestConfiguration");
          RequestConfiguration requestConfiguration =
              NextGenRequestConfigurationClient.GetRequestConfiguration(
                  androidRequestConfiguration);
          return requestConfiguration;
        }

        public void SetiOSAppPauseOnBackground(bool pause)
        {
            // Do nothing on Android. Default behavior is to pause when app is backgrounded.
        }

        public void DisableSDKCrashReporting()
        {
            // This feature is not available for the Android platform.
        }

        public void OpenAdInspector(Action<AdInspectorErrorClientEventArgs> onAdInspectorClosed)
        {
            AndroidJavaObject activity = Utils.GetCurrentActivityAndroidJavaObject();
            AndroidJavaClass adInspectorClass =
                new AndroidJavaClass(NextGenUtils.UnityAdInspectorClassName);
            NextGenAdInspectorCallback callback = new NextGenAdInspectorCallback(onAdInspectorClosed);
            adInspectorClass.CallStatic("openAdInspector", activity, callback);
        }

        public void Preload(List<PreloadConfiguration> configurations,
                            Action<PreloadConfiguration> onAdAvailable,
                            Action<PreloadConfiguration> onAdsExhausted)
        {
            // We will not support ad preloading v1 in the NextGen Unity plugin
            // implementation.
            UnityEngine.Debug.LogWarning("Preloading using this API is not supported in NextGen SDK.");
        }

        public float GetDeviceScale()
        {
            AndroidJavaObject activity = Utils.GetCurrentActivityAndroidJavaObject();
            AndroidJavaObject resources = activity.Call<AndroidJavaObject>("getResources");
            AndroidJavaObject metrics = resources.Call<AndroidJavaObject>("getDisplayMetrics");
            return metrics.Get<float>("density");
        }

        public int GetDeviceSafeWidth()
        {
            return Utils.GetScreenWidth();
        }

        public Version GetSDKVersion()
        {
            return new Version(_mobileAdsClass.CallStatic<string>("getSdkVersionString"));
        }

        #region Callbacks from OnInitializationCompleteListener.
        public void onAdapterInitializationComplete(AndroidJavaObject initStatus)
        {
            if (_initCompleteAction != null)
            {
                IInitializationStatusClient statusClient = new NextGenInitializationStatusClient(initStatus);
                _initCompleteAction(statusClient);
            }
        }

        #endregion
    }
}
