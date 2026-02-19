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
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Android
{
    public class MobileAdsClient : AndroidJavaProxy, IMobileAdsClient
    {
        private readonly static MobileAdsClient _instance = new MobileAdsClient();

        private readonly AndroidJavaClass _mobileAdsClass;
        // Ensures InsightsEmitter is initialized from the main thread to handle CUIs.
        private readonly IInsightsEmitter _insightsEmitter = InsightsEmitter.Instance;
        private readonly ITracer _tracer;
        private readonly AsyncTraceScope _asyncTraceScope;
        private Action<IInitializationStatusClient> _initCompleteAction;

        private MobileAdsClient() : base(Utils.OnInitializationCompleteListenerClassName) {
            _mobileAdsClass = new AndroidJavaClass(Utils.UnityMobileAdsClassName);
            _tracer = new Tracer(_insightsEmitter);
            _asyncTraceScope = new AsyncTraceScope(_tracer);
            // Ensures GlobalExceptionHandler is initialized from the main thread to handle Android
            // untrapped exceptions.
            var _ = GlobalExceptionHandler.Instance;
        }

        public static MobileAdsClient Instance
        {
          get { return _instance; }
        }

        public void Initialize(Action<IInitializationStatusClient> initCompleteAction)
        {
            _asyncTraceScope.StartTraceIfInactive("MobileAdsClient.Initialize");
            _initCompleteAction = initCompleteAction;

            Task.Run(() => {
              using (_tracer.StartTrace("AttachCurrentThread"))
              {
                int env = AndroidJNI.AttachCurrentThread();
                if (env < 0) {
                  UnityEngine.Debug.LogError("Failed to attach current thread to JVM.");
                  return;
                }
              }

              try {
                _mobileAdsClass.CallStatic("initialize",
                                           Utils.GetCurrentActivityAndroidJavaObject(),
                                           this);
              } finally {
                AndroidJNI.DetachCurrentThread();
              }
            });
       }

        public void SetApplicationVolume(float volume)
        {
          _mobileAdsClass.CallStatic("setAppVolume", volume);
        }

        public void DisableMediationInitialization()
        {
          _mobileAdsClass.CallStatic("disableMediationAdapterInitialization",
                                     Utils.GetCurrentActivityAndroidJavaObject());
        }

        public void SetApplicationMuted(bool muted)
        {
          _mobileAdsClass.CallStatic("setAppMuted", muted);
        }

        public void SetRequestConfiguration(RequestConfiguration requestConfiguration)
        {
            // putPublisherFirstPartyIdEnabled resides in MobileAds class in Android.
            if (requestConfiguration.PublisherFirstPartyIdEnabled.HasValue)
            {
              _mobileAdsClass.CallStatic<bool>(
                  "putPublisherFirstPartyIdEnabled",
                  requestConfiguration.PublisherFirstPartyIdEnabled.Value);
            }

            var requestConfigurationAndroidObject =
                RequestConfigurationClient.BuildRequestConfiguration(requestConfiguration);
            _mobileAdsClass.CallStatic("setRequestConfiguration",
                                       requestConfigurationAndroidObject);
        }

        public RequestConfiguration GetRequestConfiguration()
        {
          var androidRequestConfiguration =
              _mobileAdsClass.CallStatic<AndroidJavaObject>("getRequestConfiguration");
          var requestConfiguration =
              RequestConfigurationClient.GetRequestConfiguration(androidRequestConfiguration);
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
          var listener = new AdInspectorListener(onAdInspectorClosed);
          _mobileAdsClass.CallStatic("openAdInspector", Utils.GetCurrentActivityAndroidJavaObject(),
                                     listener);
        }

#if GMA_PREVIEW_FEATURES

        public void Preload(List<PreloadConfiguration> configurations,
                            Action<PreloadConfiguration> onAdAvailable,
                            Action<PreloadConfiguration> onAdsExhausted)
        {
          var listener = new PreloadListener(onAdAvailable, onAdsExhausted);
          var configurationsArrayList = new AndroidJavaObject("java.util.ArrayList");
          foreach (var configuration in configurations) {
            var preloadConfigAndroidJavaObject =
                Utils.GetPreloadConfigurationJavaObject(configuration);
            configurationsArrayList.Call<bool>("add", preloadConfigAndroidJavaObject);
          }
          _mobileAdsClass.CallStatic("startPreload", Utils.GetCurrentActivityAndroidJavaObject(),
                                     configurationsArrayList, listener);
        }

#endif

        public float GetDeviceScale()
        {
          var pluginUtilsClass = new AndroidJavaClass(Utils.PluginUtilsClassName);
          return pluginUtilsClass.CallStatic<float>("getDeviceDensity",
                                                    Utils.GetCurrentActivityAndroidJavaObject());
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

        public void onInitializationComplete(AndroidJavaObject initStatus)
        {
            _asyncTraceScope.Complete();
            _insightsEmitter.Emit(new Insight() { Name = Insight.CuiName.SdkInitialized });
            if (_initCompleteAction != null) {
              IInitializationStatusClient statusClient = new InitializationStatusClient(initStatus);
              _initCompleteAction(statusClient);
            }
            string nativePluginVersion = "";
            try {
              var assembly = Assembly.Load("GoogleMobileAdsNative.Common");
              var assemblyVersion = assembly.GetName().Version;
              nativePluginVersion = string.Format("{0}.{1}.{2}", assemblyVersion.Major,
                                                  assemblyVersion.Minor, assemblyVersion.Revision);
            } catch (Exception e) {
              if (GlobalExceptionHandler.Instance != null)
              {
                GlobalExceptionHandler.Instance.ReportTrappedException(e);
              }
            }
            string versionString = AdRequest.BuildVersionString(nativePluginVersion);
            _mobileAdsClass.CallStatic("setPlugin", versionString);
        }

        #endregion

    }
}
