#if UNITY_IOS
// Copyright 2015-2022 Google LLC
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

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    public class InterstitialAdClient : BaseAdClient, IInterstitialAdClient
    {

        #region iOS externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADUInterstitialAdCreate(IntPtr adClientPtr);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUInterstitialAdLoad(IntPtr adBridgePtr,
                                                             string adUnitId,
                                                             IntPtr request);
        [DllImport("__Internal")]
        internal static extern void GADUInterstitialAdShow(IntPtr adBridgePtr);

        #endregion

        private Action<IInterstitialAdClient, ILoadAdErrorClient> _loadCallback;

        public void LoadInterstitialAd(
            string adUnitId,
            AdRequest request,
            Action<IInterstitialAdClient, ILoadAdErrorClient> callback)
        {
            _loadCallback = callback;

            AdClientPtr = (IntPtr)GCHandle.Alloc(this);
            AdBridgePtr = GADUInterstitialAdCreate(AdClientPtr);

            SetClientCallbacks();

            IntPtr requestPtr = Utils.BuildAdRequest(request);
            GADUInterstitialAdLoad(AdBridgePtr, adUnitId, requestPtr);
            Externs.GADURelease(requestPtr);
        }

        public void Show()
        {
            GADUInterstitialAdShow(AdBridgePtr);
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
