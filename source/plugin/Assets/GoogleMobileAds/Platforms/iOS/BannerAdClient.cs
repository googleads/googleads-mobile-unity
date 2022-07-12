#if UNITY_IOS
// Copyright (C) 2015 Google, Inc.
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
    public class BannerAdClient : BaseAdClient, IBannerAdClient
    {
        #region Banner externs

        [DllImport("__Internal")]
        internal static extern IntPtr GADUBannerAdCreate(
            IntPtr adClientPtr,
            string adUnitId,
            int width,
            int height,
            int positionAtTop);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUBannerAdCreateWithCustomPosition(
            IntPtr adClientPtr,
            string adUnitId,
            int width,
            int height,
            int x,
            int y);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUBannerAdCreateSmart(
            IntPtr adClientPtr,
            string adUnitId,
            int positionAtTop);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUBannerAdCreateSmartWithCustomPosition(
            IntPtr adClientPtr,
            string adUnitId,
            int x,
            int y);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUBannerAdCreateAnchoredAdaptive(
            IntPtr adClientPtr,
            string adUnitId,
            int width,
            int orientation,
            int positionAtTop);

        [DllImport("__Internal")]
        internal static extern IntPtr GADUBannerAdCreateAnchoredAdaptiveWithCustomPosition(
            IntPtr adClientPtr,
            string adUnitId,
            int width,
            int orientation,
            int x,
            int y);

        [DllImport("__Internal")]
        internal static extern void GADUBannerAdHide(IntPtr adBridgePtr);

        [DllImport("__Internal")]
        internal static extern void GADUBannerAdShow(IntPtr adBridgePtr);

        [DllImport("__Internal")]
        internal static extern void GADUBannerAdRemove(IntPtr adBridgePtr);

        [DllImport("__Internal")]
        internal static extern void GADUBannerAdRequest(IntPtr adBridgePtr, IntPtr request);

        [DllImport("__Internal")]
        internal static extern float GADUBannerAdGetHeightInPixels(IntPtr adBridgePtr);

        [DllImport("__Internal")]
        internal static extern float GADUBannerAdGetWidthInPixels(IntPtr adBridgePtr);

        [DllImport("__Internal")]
        internal static extern void GADUBannerAdSetPosition(IntPtr adBridgePtr, int position);

        [DllImport("__Internal")]
        internal static extern void GADUBannerAdSetCustomPosition(IntPtr adBridgePtr,
                                                                  int x,
                                                                  int y);
        #endregion

        public event Action OnBannerAdLoaded = delegate { };
        public event Action<ILoadAdErrorClient> OnBannerAdLoadFailed = delegate { };

        public override void Dispose()
        {
            if (AdBridgePtr != IntPtr.Zero)
            {
                GADUBannerAdRemove(AdBridgePtr);
            }
            base.Dispose();
        }

        public void LoadBannerAd(string adUnitId, AdSize adSize, AdPosition position)
        {

            AdClientPtr = (IntPtr)GCHandle.Alloc(this);

            switch (adSize.AdType)
            {
                case AdSize.Type.SmartBanner:
                    AdBridgePtr = GADUBannerAdCreateSmart(
                            AdClientPtr, adUnitId, (int)position);
                    break;
                case AdSize.Type.AnchoredAdaptive:
                    AdBridgePtr = GADUBannerAdCreateAnchoredAdaptive(
                            AdClientPtr,
                            adUnitId,
                            adSize.Width,
                            (int)adSize.Orientation,
                            (int)position);
                    break;
                case AdSize.Type.Standard:
                    AdBridgePtr = GADUBannerAdCreate(
                            AdClientPtr,
                            adUnitId,
                            adSize.Width,
                            adSize.Height,
                            (int)position);
                    break;
                default:
                    throw new ArgumentException("Invalid AdSize.Type provided.");
            }
            SetClientCallbacks();
        }

        public void LoadBannerAd(string adUnitId, AdSize adSize, int x, int y)
        {

            AdClientPtr = (IntPtr)GCHandle.Alloc(this);

            switch (adSize.AdType)
            {
                case AdSize.Type.SmartBanner:
                    AdBridgePtr = GADUBannerAdCreateSmartWithCustomPosition(
                    AdClientPtr,
                    adUnitId,
                    x,
                    y);
                    break;
                case AdSize.Type.AnchoredAdaptive:
                    AdBridgePtr = GADUBannerAdCreateAnchoredAdaptiveWithCustomPosition(
                        AdClientPtr,
                        adUnitId,
                        adSize.Width,
                        (int)adSize.Orientation,
                        x,
                        y);
                    break;
                case AdSize.Type.Standard:
                    AdBridgePtr = GADUBannerAdCreateWithCustomPosition(
                        AdClientPtr,
                        adUnitId,
                        adSize.Width,
                        adSize.Height,
                        x,
                        y);
                    break;
                default:
                    throw new ArgumentException("Invalid AdSize.Type provided.");
            }
            SetClientCallbacks();
        }

        public void LoadAd(AdRequest request)
        {
            IntPtr requestPtr = Utils.BuildAdRequest(request);
            GADUBannerAdRequest(AdBridgePtr, requestPtr);
            Externs.GADURelease(requestPtr);
        }

        public void Show()
        {
            GADUBannerAdShow(AdBridgePtr);
        }

        public void Hide()
        {
            GADUBannerAdHide(AdBridgePtr);
        }

        public void Destroy()
        {
            Dispose();
        }

        public float GetHeightInPixels()
        {
            return GADUBannerAdGetHeightInPixels(AdBridgePtr);
        }

        public float GetWidthInPixels()
        {
            return GADUBannerAdGetWidthInPixels(AdBridgePtr);
        }

        public void SetPosition(AdPosition adPosition)
        {
            GADUBannerAdSetPosition(AdBridgePtr, (int)adPosition);
        }

        public void SetPosition(int x, int y)
        {
            GADUBannerAdSetCustomPosition(AdBridgePtr, x, y);
        }

        protected override void OnAdLoaded()
        {
            OnBannerAdLoaded();
        }

        protected override void OnAdLoadFailed(ILoadAdErrorClient error)
        {
            OnBannerAdLoadFailed(error);
        }
    }
}
#endif
