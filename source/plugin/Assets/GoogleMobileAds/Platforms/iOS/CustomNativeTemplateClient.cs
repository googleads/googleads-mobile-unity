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

#if UNITY_IOS

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;

namespace GoogleMobileAds.iOS
{
    internal class CustomNativeTemplateClient : ICustomNativeTemplateClient, IDisposable
    {
        private IntPtr customNativeAdPtr;
        private IntPtr customNativeTemplateAdClientPtr;
        private Action<CustomNativeTemplateAd, string> clickHandler;

        // This property should be used when setting the customNativeAdPtr.
        private IntPtr CustomNativeAdPtr
        {
            get
            {
                return this.customNativeAdPtr;
            }

            set
            {
                Externs.GADURelease(this.customNativeAdPtr);
                this.customNativeAdPtr = value;
            }
        }

        public CustomNativeTemplateClient(
            IntPtr customNativeAd, Action<CustomNativeTemplateAd, string> clickHandler)
        {
            this.customNativeAdPtr = customNativeAd;
            this.clickHandler = clickHandler;

            this.customNativeTemplateAdClientPtr = (IntPtr)GCHandle.Alloc(this);

            Externs.GADUSetNativeCustomTemplateAdUnityClient(
                    customNativeAd,
                    this.customNativeTemplateAdClientPtr);

            Externs.GADUSetNativeCustomTemplateAdCallbacks(
                    customNativeAd,
                    NativeCustomTemplateDidReceiveClickCallback);
        }

        internal delegate void GADUNativeCustomTemplateDidReceiveClick(
            IntPtr nativeCustomTemplateAd, string error);

        public List<string> GetAvailableAssetNames()
        {
            IntPtr unmanagedAssetArray =
                    Externs.GADUNativeCustomTemplateAdAvailableAssetKeys(this.CustomNativeAdPtr);
            int numOfAssets =
                    Externs.GADUNativeCustomTemplateAdNumberOfAvailableAssetKeys(
                            this.CustomNativeAdPtr);

            return Utils.PtrArrayToManagedList(unmanagedAssetArray, numOfAssets);
        }

        public string GetTemplateId()
        {
            return Externs.GADUNativeCustomTemplateAdTemplateID(this.CustomNativeAdPtr);
        }

        public byte[] GetImageByteArray(string key)
        {
            string bytesString = Externs.GADUNativeCustomTemplateAdImageAsBytesForKey(
                this.CustomNativeAdPtr, key);
            if (bytesString == null)
            {
                return null;
            }

            return System.Convert.FromBase64String(bytesString);
        }

        public string GetText(string key)
        {
            return Externs.GADUNativeCustomTemplateAdStringForKey(this.CustomNativeAdPtr, key);
        }

        public void PerformClick(string assetName)
        {
            bool customClickAction = this.clickHandler != null;
            Externs.GADUNativeCustomTemplateAdPerformClickOnAssetWithKey(
                this.CustomNativeAdPtr, assetName, customClickAction);
        }

        public void RecordImpression()
        {
            Externs.GADUNativeCustomTemplateAdRecordImpression(this.CustomNativeAdPtr);
        }

        public void DestroyCustomNativeTemplateAd()
        {
            this.CustomNativeAdPtr = IntPtr.Zero;
        }

        public void Dispose()
        {
            this.DestroyCustomNativeTemplateAd();
            ((GCHandle)this.customNativeTemplateAdClientPtr).Free();
        }

        ~CustomNativeTemplateClient()
        {
            this.Dispose();
        }

        [MonoPInvokeCallback(typeof(GADUNativeCustomTemplateDidReceiveClick))]
        private static void NativeCustomTemplateDidReceiveClickCallback(
            IntPtr nativeCustomAd, string assetName)
        {
            CustomNativeTemplateClient client = IntPtrToAdLoaderClient(nativeCustomAd);
            if (client.clickHandler != null)
            {
                CustomNativeTemplateAd nativeAd = new CustomNativeTemplateAd(client);
                client.clickHandler(nativeAd, assetName);
            }

        }

        private static CustomNativeTemplateClient IntPtrToAdLoaderClient(
            IntPtr customNativeTemplateAd)
        {
            GCHandle handle = (GCHandle)customNativeTemplateAd;
            return handle.Target as CustomNativeTemplateClient;
        }
    }
}

#endif
