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
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Android
{
    public class MobileAdsClient : AndroidJavaProxy, IMobileAdsClient
    {
        private static MobileAdsClient instance = new MobileAdsClient();

        private Action<IInitializationStatusClient> initCompleteAction;

        private MobileAdsClient() : base(Utils.OnInitializationCompleteListenerClassName) { }

        public static MobileAdsClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void Initialize(Action<IInitializationStatusClient> initCompleteAction)
        {
            this.initCompleteAction = initCompleteAction;

            AndroidJavaObject activity = Utils.GetCurrentActivityAndroidJavaObject();
            AndroidJavaClass mobileAdsClass = new AndroidJavaClass(Utils.MobileAdsClassName);
            mobileAdsClass.CallStatic("initialize", activity, this);
        }

        public void SetApplicationVolume(float volume)
        {
            AndroidJavaClass mobileAdsClass = new AndroidJavaClass(Utils.MobileAdsClassName);
            mobileAdsClass.CallStatic("setAppVolume", volume);
        }

        public void DisableMediationInitialization()
        {
            AndroidJavaObject activity = Utils.GetCurrentActivityAndroidJavaObject();
            AndroidJavaClass mobileAdsClass = new AndroidJavaClass(Utils.MobileAdsClassName);
            mobileAdsClass.CallStatic("disableMediationAdapterInitialization", activity);
        }

        public void SetApplicationMuted(bool muted)
        {
            AndroidJavaClass mobileAdsClass = new AndroidJavaClass(Utils.MobileAdsClassName);
            mobileAdsClass.CallStatic("setAppMuted", muted);
        }

        public void SetRequestConfiguration(RequestConfiguration requestConfiguration)
        {
            AndroidJavaClass mobileAdsClass = new AndroidJavaClass(Utils.MobileAdsClassName);

            // putPublisherFirstPartyIdEnabled resides in MobileAds class in Android.
            if (requestConfiguration.PublisherFirstPartyIdEnabled.HasValue)
            {
                mobileAdsClass.CallStatic<bool>("putPublisherFirstPartyIdEnabled", requestConfiguration.PublisherFirstPartyIdEnabled.Value);
            }

            AndroidJavaObject requestConfigurationAndroidObject = RequestConfigurationClient.BuildRequestConfiguration(requestConfiguration);
            mobileAdsClass.CallStatic("setRequestConfiguration", requestConfigurationAndroidObject);
        }

        public RequestConfiguration GetRequestConfiguration()
        {
            AndroidJavaClass mobileAdsClass = new AndroidJavaClass(Utils.MobileAdsClassName);
            AndroidJavaObject androidRequestConfiguration = mobileAdsClass.CallStatic<AndroidJavaObject>("getRequestConfiguration");
            RequestConfiguration requestConfiguration = RequestConfigurationClient.GetRequestConfiguration(androidRequestConfiguration);
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
                        new AndroidJavaClass(Utils.UnityAdInspectorClassName);
            AdInspectorListener listener = new AdInspectorListener(onAdInspectorClosed);
            adInspectorClass.CallStatic("openAdInspector", activity, listener);
        }

        public void Preload(List<PreloadConfiguration> configurations,
                            Action<PreloadConfiguration> onAdAvailable,
                            Action<PreloadConfiguration> onAdsExhausted)
        {
            AndroidJavaObject activity = Utils.GetCurrentActivityAndroidJavaObject();
            PreloadListener listener = new PreloadListener(onAdAvailable, onAdsExhausted);
            var configurationsArrayList = new AndroidJavaObject("java.util.ArrayList");
            foreach (PreloadConfiguration configuration in configurations)
            {
                var preloadConfigAndroidJavaObject =
                        Utils.GetPreloadConfigurationJavaObject(configuration);
                configurationsArrayList.Call<bool>("add", preloadConfigAndroidJavaObject);
            }
            AndroidJavaClass mobileAdsClass = new AndroidJavaClass(Utils.MobileAdsClassName);
            mobileAdsClass.CallStatic("startPreload", activity, configurationsArrayList, listener);
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
            AndroidJavaClass mobileAdsClass = new AndroidJavaClass(Utils.MobileAdsClassName);
            AndroidJavaObject androidSDKVersion =
                mobileAdsClass.CallStatic<AndroidJavaObject>("getVersion");
            string versionString = androidSDKVersion.Call<string>("toString");
            return new Version(versionString);
        }

        #region Callbacks from OnInitializationCompleteListener.

        public void onInitializationComplete(AndroidJavaObject initStatus)
        {
            if (initCompleteAction != null)
            {
                IInitializationStatusClient statusClient = new InitializationStatusClient(initStatus);
                initCompleteAction(statusClient);
            }
            string nativePluginVersion = "";
            try
            {
                Assembly assembly = Assembly.Load("GoogleMobileAdsNative.Common");
                Version assemblyVersion = assembly.GetName().Version;
                nativePluginVersion = string.Format("{0}.{1}.{2}",
                        assemblyVersion.Major,
                        assemblyVersion.Minor,
                        assemblyVersion.Revision);
            }
            catch (Exception) {}
            string versionString = AdRequest.BuildVersionString(nativePluginVersion);
            AndroidJavaClass mobileAdsClass = new AndroidJavaClass(Utils.MobileAdsClassName);
            try
            {
                mobileAdsClass.CallStatic("setPlugin", versionString);
            }
            catch (AndroidJavaException) {}
        }

        #endregion

    }
}
