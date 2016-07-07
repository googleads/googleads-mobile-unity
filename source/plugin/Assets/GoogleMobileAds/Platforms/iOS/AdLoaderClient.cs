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

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;

namespace GoogleMobileAds.iOS
{
    public struct NativeAdTypes
    {
        public bool CustomTemplateAd;
        public bool AppInstallAd;
        public bool ContentAd;
    }

    internal class AdLoaderClient : IAdLoaderClient
    {
        private IntPtr adLoaderPtr;

        private Dictionary<string, Action<CustomNativeTemplateAd, string>>
            customNativeTemplateCallbacks;

        public AdLoaderClient(AdLoader unityAdLoader)
        {
            IntPtr adLoaderClientPtr = (IntPtr)GCHandle.Alloc(this);

            this.customNativeTemplateCallbacks = unityAdLoader.CustomNativeTemplateClickHandlers;
            string[] templateIdsArray = new string[unityAdLoader.TemplateIds.Count];
            unityAdLoader.TemplateIds.CopyTo(templateIdsArray);

            NativeAdTypes adTypes = new NativeAdTypes();
            if (unityAdLoader.AdTypes.Contains(NativeAdType.CustomTemplate))
            {
                adTypes.CustomTemplateAd = true;
            }

            this.AdLoaderPtr = Externs.GADUCreateAdLoader(
                adLoaderClientPtr,
                unityAdLoader.AdUnitId,
                templateIdsArray,
                templateIdsArray.Length,
                ref adTypes);

            Externs.GADUSetAdLoaderCallbacks(
                this.AdLoaderPtr,
                AdLoaderDidReceiveNativeCustomTemplateAdCallback,
                AdLoaderDidFailToReceiveAdWithErrorCallback);
        }

        internal delegate void GADUAdLoaderDidReceiveNativeCustomTemplateAdCallback(
            IntPtr adLoader, IntPtr nativeCustomTemplateAd, string templateID);

        internal delegate void GADUAdLoaderDidFailToReceiveAdWithErrorCallback(
            IntPtr AdLoader, string error);

        public event EventHandler<CustomNativeEventArgs> OnCustomNativeTemplateAdLoaded;

        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

        // This property should be used when setting the adLoaderPtr.
        private IntPtr AdLoaderPtr
        {
            get
            {
                return this.adLoaderPtr;
            }

            set
            {
                Externs.GADURelease(this.adLoaderPtr);
                this.adLoaderPtr = value;
            }
        }

        public void LoadAd(AdRequest request)
        {
            IntPtr requestPtr = Utils.BuildAdRequest(request);
            Externs.GADURequestNativeAd(this.AdLoaderPtr, requestPtr);
            Externs.GADURelease(requestPtr);
        }

        [MonoPInvokeCallback(typeof(GADUAdLoaderDidReceiveNativeCustomTemplateAdCallback))]
        private static void AdLoaderDidReceiveNativeCustomTemplateAdCallback(
            IntPtr adLoader, IntPtr nativeCustomTemplateAd, string templateID)
        {
            AdLoaderClient client = IntPtrToAdLoaderClient(adLoader);
            Action<CustomNativeTemplateAd, string> clickHandler =
                    client.customNativeTemplateCallbacks.ContainsKey(templateID) ?
                    client.customNativeTemplateCallbacks[templateID] : null;

            CustomNativeEventArgs args = new CustomNativeEventArgs()
            {
                nativeAd = new CustomNativeTemplateAd(new CustomNativeTemplateClient(
                    nativeCustomTemplateAd, clickHandler))
            };
            client.OnCustomNativeTemplateAdLoaded(client, args);
        }

        [MonoPInvokeCallback(typeof(GADUAdLoaderDidFailToReceiveAdWithErrorCallback))]
        private static void AdLoaderDidFailToReceiveAdWithErrorCallback(
            IntPtr adLoader, string error)
        {
            AdLoaderClient client = IntPtrToAdLoaderClient(adLoader);
            AdFailedToLoadEventArgs args = new AdFailedToLoadEventArgs()
            {
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
