#if UNITY_IOS
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
using System.Runtime.InteropServices;
using System.Threading;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    public class BaseAdClient : IDisposable
    {
        #region iOS externs

        [DllImport("__Internal")]
        internal static extern void GADUBaseAdSetCallbacks(
            IntPtr adBridgeRef,
            GADUBaseAdCallback adLoadCallback,
            GADUBaseAdErrorCallback adLoadFailedCallback,
            GADUBaseAdCallback adFullScreenOpenedCallback,
            GADUBaseAdErrorCallback adFullScreenFailedCallback,
            GADUBaseAdCallback adFullScreenClosedCallback,
            GADUBaseAdCallback adImpressionCallback,
            GADUBaseAdCallback adClickedCallback,
            GADUBaseAdPaidCallback adPaidCallback,
            GADUBaseAdRewardedCallback adUserEarnedRewardCallback
        );

        [DllImport("__Internal")]
        internal static extern IntPtr GADUBaseAdGetResponseInfo(IntPtr adBridgeRef);

        internal delegate void GADUBaseAdCallback(IntPtr adClientRef);

        internal delegate void GADUBaseAdErrorCallback(IntPtr adClientRef,
                                                       IntPtr error);

        internal delegate void GADUBaseAdPaidCallback(IntPtr adClientRef,
                                                      int precision,
                                                      long value,
                                                      string currencyCode);

        internal delegate void GADUBaseAdRewardedCallback(IntPtr adClientRef,
                                                          string rewardType,
                                                          double rewardAmount);

        #endregion

        public event Action OnAdFullScreenContentOpened = delegate { };
        public event Action OnAdFullScreenContentClosed = delegate { };
        public event Action<IAdErrorClient> OnAdFullScreenContentFailed = delegate { };
        public event Action<AdValue> OnAdPaid = delegate { };
        public event Action OnAdClickRecorded = delegate { };
        public event Action OnAdImpressionRecorded = delegate { };

        public Reward RewardItem { get; private set; }

        public bool IsDestroyed { get { return AdBridgePtr == IntPtr.Zero; } }

        /// <summary>
        /// Pointer to this C# object.
        /// </summary>
        protected IntPtr AdClientPtr
        {
            get
            {
                return _adClientPtr;
            }
            set
            {
                if (_adClientPtr != IntPtr.Zero)
                {
                    ((GCHandle)_adClientPtr).Free();
                }
                _adClientPtr = value;
            }
        }

        /// <summary>
        /// Pointer to the iOS Shim object this C# object communicates with.
        /// </summary>
        protected IntPtr AdBridgePtr
        {
            get
            {
                return _adBridgePtr;
            }
            set
            {
                if (_adBridgePtr != IntPtr.Zero)
                {
                    ((GCHandle)_adBridgePtr).Free();
                    Externs.GADURelease(_adBridgePtr);
                }
                _adBridgePtr = value;
            }
        }

        private IResponseInfoClient _response;
        private IntPtr _adClientPtr = IntPtr.Zero;
        private IntPtr _adBridgePtr = IntPtr.Zero;

        ~BaseAdClient()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            AdBridgePtr = IntPtr.Zero;
            AdClientPtr = IntPtr.Zero;
        }

        public IResponseInfoClient GetResponseInfoClient()
        {
            if (_response == null)
            {
                IntPtr responseRef = GADUBaseAdGetResponseInfo(AdBridgePtr);
                _response = new ResponseInfoClient(responseRef);
            }
            return _response;
        }

        public Reward GetRewardItem()
        {
            return RewardItem;
        }

        /// <summary>
        /// Binds event callbacks to the iOS bridge.
        /// </summary>
        protected void SetClientCallbacks()
        {

            GADUBaseAdSetCallbacks(AdBridgePtr,
                                   OnAdLoadCallback,
                                   OnAdLoadFailedCallback,
                                   OnAdFullScreenContentOpenedCallback,
                                   OnAdFullScreenContentFailedCallback,
                                   OnAdFullScreenContentClosedCallback,
                                   OnAdImpressionRecordedCallback,
                                   OnAdClickRecordedCallback,
                                   OnAdPaidCallback,
                                   OnUserEarnedRewardCallback);
        }

        /// <summary>
        /// Helper for casting IntPtr to a BaseAdClient type
        /// </summary>
        protected static BaseAdClient IntPtrToBaseAdClient(IntPtr adClientRef)
        {
            GCHandle handle = (GCHandle)adClientRef;
            return handle.Target as BaseAdClient;
        }

        /// <summary>
        /// Helper for casting IntPtr to a BaseAdClient type
        /// </summary>
        protected static T IntPtrToClient<T>(IntPtr adClientRef) where T : BaseAdClient
        {
            GCHandle handle = (GCHandle)adClientRef;
            return handle.Target as T;
        }

        /// <summary>
        /// Raised when the AdLoad operation completes.
        /// Use this to event to raise any load callback.
        /// </summary>
        protected virtual void OnAdLoaded()
        {
        }

        /// <summary>
        /// Raised when the AdLoad operation completes.
        /// Use this to event to raise any load failed callback.
        /// </summary>
        protected virtual void OnAdLoadFailed(ILoadAdErrorClient error)
        {
        }

        /// <summary>
        /// Raised when the Show() operation completes.
        /// Use this to event to raise any user earned reward callback.
        /// </summary>
        protected virtual void OnUserEarnedReward(Reward reward)
        {
        }

        [MonoPInvokeCallback(typeof(GADUBaseAdCallback))]
        private static void OnAdLoadCallback(IntPtr adClientRef)
        {
            BaseAdClient client = IntPtrToBaseAdClient(adClientRef);
            client.OnAdLoaded();
        }

        [MonoPInvokeCallback(typeof(GADUBaseAdErrorCallback))]
        private static void OnAdLoadFailedCallback(IntPtr adClientRef, IntPtr errorRef)
        {
            BaseAdClient client = IntPtrToBaseAdClient(adClientRef);
            ILoadAdErrorClient errorClient = new LoadAdErrorClient(errorRef);
            client.OnAdLoadFailed(errorClient);
        }

        [MonoPInvokeCallback(typeof(GADUBaseAdCallback))]
        private static void OnAdFullScreenContentOpenedCallback(IntPtr adClientRef)
        {
            BaseAdClient client = IntPtrToBaseAdClient(adClientRef);
            client.OnAdFullScreenContentOpened();
        }

        [MonoPInvokeCallback(typeof(GADUBaseAdCallback))]
        private static void OnAdFullScreenContentClosedCallback(IntPtr adClientRef)
        {
            BaseAdClient client = IntPtrToBaseAdClient(adClientRef);
            client.OnAdFullScreenContentClosed();
        }

        [MonoPInvokeCallback(typeof(GADUBaseAdErrorCallback))]
        private static void OnAdFullScreenContentFailedCallback(IntPtr adClientRef, IntPtr errorRef)
        {
            BaseAdClient client = IntPtrToBaseAdClient(adClientRef);
            client.OnAdFullScreenContentFailed(new AdErrorClient(errorRef));
        }

        [MonoPInvokeCallback(typeof(GADUBaseAdPaidCallback))]
        private static void OnAdPaidCallback(IntPtr adClientRef,
            int precision, long value, string currencyCode)
        {
            BaseAdClient client = IntPtrToBaseAdClient(adClientRef);
            client.OnAdPaid(new AdValue
            {
                Precision = (AdValue.PrecisionType)precision,
                Value = value,
                CurrencyCode = currencyCode
            });
        }

        [MonoPInvokeCallback(typeof(GADUBaseAdCallback))]
        private static void OnAdClickRecordedCallback(IntPtr adClientRef)
        {
            BaseAdClient client = IntPtrToBaseAdClient(adClientRef);
            client.OnAdClickRecorded();
        }

        [MonoPInvokeCallback(typeof(GADUBaseAdCallback))]
        private static void OnAdImpressionRecordedCallback(IntPtr adClientRef)
        {
            BaseAdClient client = IntPtrToBaseAdClient(adClientRef);
            client.OnAdImpressionRecorded();
        }

        [MonoPInvokeCallback(typeof(GADUBaseAdRewardedCallback))]
        private static void OnUserEarnedRewardCallback(IntPtr adClientRef,
            string rewardType, double rewardAmount)
        {
            BaseAdClient client = IntPtrToBaseAdClient(adClientRef);
            client.RewardItem = new Reward
            {
                Type = rewardType,
                Amount = rewardAmount,
            };
            client.OnUserEarnedReward(client.RewardItem);
        }
    }
}
#endif
