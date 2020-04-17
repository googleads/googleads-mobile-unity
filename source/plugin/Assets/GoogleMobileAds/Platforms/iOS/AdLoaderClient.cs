#if UNITY_IOS
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
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    [StructLayout(LayoutKind.Sequential),Serializable]
    // The System.Boolean (bool in C#) type is special. A bool within a structure is marshaled in a different format
    // than when passed as an argument to a function (4-byte integer vs 2-byte integer, non zero = true vs -1 = true).
    // Using ints instead for simplicity.
    public struct NativeAdTypes
    {
        public int CustomTemplateAd;
    }

    public class AdLoaderClient : IAdLoaderClient, IDisposable
    {
        private IntPtr adLoaderPtr;
        private IntPtr adLoaderClientPtr;
        private NativeAdTypes adTypes;
        // Dictionary of template Ids and whether they have a click callback registered.
        Dictionary<string, bool> TemplateIds;

        public AdLoaderClient(AdLoaderClientArgs clientArgs)
        {
            this.adLoaderClientPtr = (IntPtr)GCHandle.Alloc(this);
            this.TemplateIds = clientArgs.TemplateIds;
            string[] templateIdsArray = new string[TemplateIds.Keys.Count];
            TemplateIds.Keys.ToList().CopyTo(templateIdsArray);

            this.adTypes = new NativeAdTypes();
            bool configureReturnUrlsForImageAssets = false;

            if (clientArgs.AdTypes.Contains(NativeAdType.CustomTemplate))
            {
                configureReturnUrlsForImageAssets = false;
                adTypes.CustomTemplateAd = 1;
            }
            this.AdLoaderPtr = Externs.GADUCreateAdLoader(
                this.adLoaderClientPtr,
                clientArgs.AdUnitId,
                templateIdsArray,
                templateIdsArray.Length,
                ref adTypes,
                configureReturnUrlsForImageAssets);

            Externs.GADUSetAdLoaderCallbacks(
                this.AdLoaderPtr,
                AdLoaderDidReceiveNativeCustomTemplateAdCallback,
                AdLoaderDidFailToReceiveAdWithErrorCallback);
        }

        internal delegate void GADUAdLoaderDidReceiveNativeCustomTemplateAdCallback(
            IntPtr adLoader, IntPtr nativeCustomTemplateAd, string templateID);

        internal delegate void GADUAdLoaderDidFailToReceiveAdWithErrorCallback(
            IntPtr AdLoader, string error);

        public event EventHandler<CustomNativeClientEventArgs> OnCustomNativeTemplateAdLoaded;

        public event EventHandler<CustomNativeClientEventArgs> OnCustomNativeTemplateAdClicked;

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

        // Destroys the AdLoader.
        public void DestroyAdLoader()
        {
            this.AdLoaderPtr = IntPtr.Zero;
        }

        public void Dispose()
        {
            this.DestroyAdLoader();
            ((GCHandle)this.adLoaderClientPtr).Free();
        }

        ~AdLoaderClient()
        {
            this.Dispose();
        }

        [MonoPInvokeCallback(typeof(GADUAdLoaderDidReceiveNativeCustomTemplateAdCallback))]
        private static void AdLoaderDidReceiveNativeCustomTemplateAdCallback(
            IntPtr adLoader, IntPtr nativeCustomTemplateAd, string templateID)
        {
            AdLoaderClient client = IntPtrToAdLoaderClient(adLoader);
            CustomNativeTemplateClient adClient = new CustomNativeTemplateClient(
                nativeCustomTemplateAd);
            if (client.OnCustomNativeTemplateAdClicked != null &&
                  client.TemplateIds[templateID] == true)
            {
                WeakReference weakClient = new WeakReference(adClient);
                adClient.clickHandler = delegate(string assetName)
                                    {
                                        if (weakClient.IsAlive)
                                        {
                                            CustomNativeTemplateClient strongClient = weakClient.Target as CustomNativeTemplateClient;
                                            CustomNativeClientEventArgs args = new CustomNativeClientEventArgs()
                                            {
                                                nativeAdClient = strongClient,
                                                assetName = assetName
                                            };
                                            client.OnCustomNativeTemplateAdClicked(client, args);
                                        }
                                    };
            }

            if (client.OnCustomNativeTemplateAdLoaded != null)
            {
                CustomNativeClientEventArgs args = new CustomNativeClientEventArgs()
                {
                    nativeAdClient = adClient,
                    assetName = null
                };
                client.OnCustomNativeTemplateAdLoaded(client, args);
            }
        }

        [MonoPInvokeCallback(typeof(GADUAdLoaderDidFailToReceiveAdWithErrorCallback))]
        private static void AdLoaderDidFailToReceiveAdWithErrorCallback(
            IntPtr adLoader, string error)
        {
            AdLoaderClient client = IntPtrToAdLoaderClient(adLoader);
            if (client.OnAdFailedToLoad != null)
            {
                AdFailedToLoadEventArgs args = new AdFailedToLoadEventArgs()
                {
                    Message = error
                };
                client.OnAdFailedToLoad(client, args);
            }
        }

        private static AdLoaderClient IntPtrToAdLoaderClient(IntPtr adLoader)
        {
            GCHandle handle = (GCHandle)adLoader;
            return handle.Target as AdLoaderClient;
        }
    }
}
#endif
