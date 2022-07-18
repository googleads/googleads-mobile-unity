#if UNITY_IOS
// Copyright (C) 2021 Google LLC
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
using System.Runtime.InteropServices;

using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System.Threading;

namespace GoogleMobileAds.iOS
{
    public class AppOpenAdClient : BaseAdClient, IAppOpenAdClient
    {
        #region AppOpenAd externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADUAppOpenAdCreate(IntPtr adClientPtr);

        [DllImport("__Internal")]
        internal static extern void GADUAppOpenAdLoad(IntPtr adBridgePtr,
                                                      string adUnitID,
                                                      int orientation,
                                                      IntPtr requestPtr);

        [DllImport("__Internal")]
        internal static extern void GADUAppOpenAdShow(IntPtr adBridgePtr);

        #endregion

        private Action<IAppOpenAdClient, ILoadAdErrorClient> _loadCallback;

        public void LoadAd(string adUnitId,
                           ScreenOrientation orientation,
                           AdRequest request,
                           Action<IAppOpenAdClient, ILoadAdErrorClient> callback)
        {
            _loadCallback = callback;

            AdClientPtr = (IntPtr)GCHandle.Alloc(this);
            AdBridgePtr = GADUAppOpenAdCreate(AdClientPtr);

            SetClientCallbacks();

            IntPtr requestPtr = Utils.BuildAdRequest(request);
            GADUAppOpenAdLoad(AdBridgePtr, adUnitId, (int)orientation, requestPtr);
            Externs.GADURelease(requestPtr);
        }

        public void Show()
        {
            GADUAppOpenAdShow(AdBridgePtr);
        }

        public void Destroy()
        {
            Dispose();
        }

        protected override void OnAdLoaded()
        {
            if (_loadCallback != null)
            {
                _loadCallback(this, null);
                _loadCallback = null;
            }
        }

        protected override void OnAdLoadFailed(ILoadAdErrorClient error)
        {
            if (_loadCallback != null)
            {
                _loadCallback(null, error);
                _loadCallback = null;
            }
        }
    }
}
#endif
