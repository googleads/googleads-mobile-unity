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

#if UNITY_IOS

using System;
using System.Runtime.InteropServices;
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    public class MobileAdsClient : IMobileAdsClient
    {
        private static MobileAdsClient instance = new MobileAdsClient();
        private Action<InitializationStatus> initCompleteAction;
        private IntPtr mobileAdsClientPtr;
        internal delegate void GADUInitializationCompleteCallback(IntPtr mobileAdsClient, IntPtr initStatusClient);

        private MobileAdsClient()
        {
            this.mobileAdsClientPtr = (IntPtr)GCHandle.Alloc(this);
        }

        public static MobileAdsClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void Initialize(string appId)
        {
            Externs.GADUInitialize(appId);
        }

        public void Initialize(Action<InitializationStatus> initCompleteAction)
        {
            this.initCompleteAction = initCompleteAction;
            Externs.GADUInitializeWithCallback(this.mobileAdsClientPtr, InitializationCompleteCallback);
        }

        public void SetApplicationVolume(float volume)
        {
            Externs.GADUSetApplicationVolume(volume);
        }

        public void SetApplicationMuted(bool muted)
        {
            Externs.GADUSetApplicationMuted(muted);
        }

        public void SetiOSAppPauseOnBackground(bool pause)
        {
            Externs.GADUSetiOSAppPauseOnBackground(pause);
        }

        public float GetDeviceScale()
        {
            return Externs.GADUDeviceScale();
        }

        public int GetDeviceSafeWidth()
        {
            return Externs.GADUDeviceSafeWidth();
        }

        [MonoPInvokeCallback(typeof(GADUInitializationCompleteCallback))]
        private static void InitializationCompleteCallback(IntPtr mobileAdsClient, IntPtr initStatus)
        {
            MobileAdsClient client = IntPtrToMobileAdsClient(mobileAdsClient);
            if (client.initCompleteAction != null)
            {
                InitializationStatus status = new InitializationStatus(new InitializationStatusClient(initStatus));
                client.initCompleteAction(status);
            }
        }

        private static MobileAdsClient IntPtrToMobileAdsClient(IntPtr mobileAdsClient)
        {
            GCHandle handle = (GCHandle)mobileAdsClient;
            return handle.Target as MobileAdsClient;
        }

        public void Dispose()
        {
            ((GCHandle)this.mobileAdsClientPtr).Free();
        }

        ~MobileAdsClient()
        {
            this.Dispose();
        }
    }
}

#endif
