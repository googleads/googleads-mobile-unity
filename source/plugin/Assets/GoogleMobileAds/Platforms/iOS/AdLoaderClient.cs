// Copyright (C) 2016 Google, Inc.
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
using System.Runtime.InteropServices;
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    public struct NativeAdTypes
    {
        public bool customTemplateAd;
        public bool appInstallAd;
        public bool contentAd;
    }

    internal class AdLoaderClient : IAdLoaderClient
    {
        public event EventHandler<CustomNativeEventArgs> onCustomNativeTemplateAdLoaded;
        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

        internal delegate void GADUAdLoaderDidReceiveNativeCustomTemplateAdCallback(
                IntPtr adLoader, IntPtr nativeCustomTemplateAd, string templateID);

        internal delegate void GADUAdLoaderDidFailToReceiveAdWithErrorCallback(
                IntPtr AdLoader, string error);

        private Dictionary<string, Action<CustomNativeTemplateAd, string>>
                CustomNativeTemplateCallbacks { get; set; }

        private IntPtr adLoaderPtr;

        // This property should be used when setting the adLoaderPtr.
        private IntPtr AdLoaderPtr {
            get
            {
                return adLoaderPtr;
            }
            set
            {
                if(adLoaderPtr != null)
                {
                    Externs.GADURelease(adLoaderPtr);
                }
                adLoaderPtr = value;
            }
        }

        public AdLoaderClient(AdLoader unityAdLoader)
        {
            IntPtr adLoaderClientPtr = (IntPtr)GCHandle.Alloc(this);

            this.CustomNativeTemplateCallbacks = unityAdLoader.CustomNativeTemplateClickHandlers;
            string[] templateIdsArray = new String[unityAdLoader.TemplateIds.Count];
            unityAdLoader.TemplateIds.CopyTo(templateIdsArray);

            NativeAdTypes adTypes = new NativeAdTypes();
            if (unityAdLoader.AdTypes.Contains(NativeAdType.CustomTemplate))
            {
                adTypes.customTemplateAd = true;
            }

            AdLoaderPtr = Externs.GADUCreateAdLoader(
                adLoaderClientPtr, unityAdLoader.AdUnitId, templateIdsArray,
                templateIdsArray.Length, ref adTypes);

            Externs.GADUSetAdLoaderCallbacks(
                AdLoaderPtr,
                AdLoaderDidReceiveNativeCustomTemplateAdCallback,
                AdLoaderDidFailToReceiveAdWithErrorCallback);
        }

        public void LoadAd(AdRequest request)
        {
            IntPtr requestPtr = Utils.BuildAdRequest(request);
            Externs.GADURequestNativeAd(AdLoaderPtr, requestPtr);
            Externs.GADURelease(requestPtr);
        }

        [MonoPInvokeCallback(typeof(GADUAdLoaderDidReceiveNativeCustomTemplateAdCallback))]
        private static void AdLoaderDidReceiveNativeCustomTemplateAdCallback(
            IntPtr adLoader, IntPtr nativeCustomTemplateAd, string templateID)
        {
            AdLoaderClient client = IntPtrToAdLoaderClient(adLoader);
            Action<CustomNativeTemplateAd, string> clickHandler =
                    client.CustomNativeTemplateCallbacks.ContainsKey(templateID) ?
                    client.CustomNativeTemplateCallbacks[templateID] : null;

            CustomNativeEventArgs args = new CustomNativeEventArgs() {
                nativeAd = new CustomNativeTemplateAd(new CustomNativeTemplateClient(
                    nativeCustomTemplateAd, clickHandler))
            };
            client.onCustomNativeTemplateAdLoaded(client, args);
        }

        [MonoPInvokeCallback(typeof(GADUAdLoaderDidFailToReceiveAdWithErrorCallback))]
        private static void AdLoaderDidFailToReceiveAdWithErrorCallback(
            IntPtr adLoader, string error)
        {
            AdLoaderClient client = IntPtrToAdLoaderClient(adLoader);
            AdFailedToLoadEventArgs args = new AdFailedToLoadEventArgs() {
                Message = error
            };
            client.OnAdFailedToLoad(client, args);
        }

        private static AdLoaderClient IntPtrToAdLoaderClient(IntPtr adLoader)
        {
            GCHandle handle = (GCHandle)adLoader;
            return handle.Target as AdLoaderClient;
        }
    }
}

