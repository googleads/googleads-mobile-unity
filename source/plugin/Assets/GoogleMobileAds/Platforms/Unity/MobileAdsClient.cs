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
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Unity
{
    public class MobileAdsClient : IMobileAdsClient
    {
        public void DisableMediationInitialization()
        {
            UnityEngine.Debug.Log("Dummy DisableMediationInitialization");
        }

        public int GetDeviceSafeWidth()
        {
            UnityEngine.Debug.Log("Dummy GetDeviceSafeWidth");
            return 0;
        }

        public float GetDeviceScale()
        {
            UnityEngine.Debug.Log("Dummy GetDeviceScale");
            return 0;
        }

        public RequestConfiguration GetRequestConfiguration()
        {
            UnityEngine.Debug.Log("Dummy GetRequestConfiguration");
            return null;
        }

        public void Initialize(Action<IInitializationStatusClient> initCompleteAction)
        {
            UnityEngine.Debug.Log("Dummy Initialize");
            var initStatusClient = new InitializationStatusClient();
            initCompleteAction(initStatusClient);
        }

        public void SetApplicationMuted(bool muted)
        {
            UnityEngine.Debug.Log("Dummy SetApplicationMuted");
        }

        public void SetApplicationVolume(float volume)
        {
            UnityEngine.Debug.Log("Dummy SetApplicationVolume");
        }

        public void SetiOSAppPauseOnBackground(bool pause)
        {
            UnityEngine.Debug.Log("Dummy SetiOSAppPauseOnBackground");
        }

        public void SetRequestConfiguration(RequestConfiguration requestConfiguration)
        {
            UnityEngine.Debug.Log("Dummy RequestConfiguration");
        }

        public void OpenAdInspector(Action<IAdErrorClient> adInspectorClosedAction)
        {
            UnityEngine.Debug.Log("Dummy AdInspector");
            var client = new AdInspectorClient();
            client.OpenAdInspector(adInspectorClosedAction);
        }
    }
}
