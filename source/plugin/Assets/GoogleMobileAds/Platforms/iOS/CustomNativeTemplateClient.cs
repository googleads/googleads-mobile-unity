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
    public class CustomNativeTemplateClient : ICustomNativeTemplateClient
    {
        private IntPtr customNativeAd;
        private Action<CustomNativeTemplateAd, string> clickHandler;

        public CustomNativeTemplateClient(
            IntPtr customNativeAd, Action<CustomNativeTemplateAd, string> clickHandler)
        {
            this.customNativeAd = customNativeAd;
            this.clickHandler = clickHandler;

            IntPtr customNativeCustomTemplateAdClientPtr = (IntPtr)GCHandle.Alloc(this);

            Externs.GADUSetNativeCustomTemplateAdUnityClient(
                    customNativeAd,
                    customNativeCustomTemplateAdClientPtr);

            Externs.GADUSetNativeCustomTemplateAdCallbacks(
                    customNativeAd,
                    NativeCustomTemplateDidReceiveClickCallback);
        }

        internal delegate void GADUNativeCustomTemplateDidReceiveClick(
            IntPtr nativeCustomTemplateAd, string error);

        public List<string> GetAvailableAssetNames()
        {
            IntPtr unmanagedAssetArray =
                    Externs.GADUNativeCustomTemplateAdAvailableAssetKeys(this.customNativeAd);
            int numOfAssets =
                    Externs.GADUNativeCustomTemplateAdNumberOfAvailableAssetKeys(
                            this.customNativeAd);

            IntPtr[] intPtrArray = new IntPtr[numOfAssets];
            string[] managedAssetArray = new string[numOfAssets];
            Marshal.Copy(unmanagedAssetArray, intPtrArray, 0, numOfAssets);

            for (int i = 0; i < numOfAssets; i++)
            {
                managedAssetArray[i] = Marshal.PtrToStringAuto(intPtrArray[i]);
                Marshal.FreeHGlobal(intPtrArray[i]);
            }

            Marshal.FreeHGlobal(unmanagedAssetArray);
            return new List<string>(managedAssetArray);
        }

        public string GetTemplateId()
        {
            return Externs.GADUNativeCustomTemplateAdTemplateID(this.customNativeAd);
        }

        public byte[] GetImageByteArray(string key)
        {
            string bytesString = Externs.GADUNativeCustomTemplateAdImageAsBytesForKey(
                this.customNativeAd, key);
            if (bytesString == null)
            {
                return null;
            }

            return System.Convert.FromBase64String(bytesString);
        }

        public string GetText(string key)
        {
            return Externs.GADUNativeCustomTemplateAdStringForKey(this.customNativeAd, key);
        }

        public void PerformClick(string assetName)
        {
            bool customClickAction = this.clickHandler != null;
            Externs.GADUNativeCustomTemplateAdPerformClickOnAssetWithKey(
                this.customNativeAd, assetName, customClickAction);
        }

        public void RecordImpression()
        {
            Externs.GADUNativeCustomTemplateAdRecordImpression(this.customNativeAd);
        }

        [MonoPInvokeCallback(typeof(GADUNativeCustomTemplateDidReceiveClick))]
        private static void NativeCustomTemplateDidReceiveClickCallback(
            IntPtr nativeCustomAd, string assetName)
        {
            CustomNativeTemplateClient client = IntPtrToAdLoaderClient(nativeCustomAd);
            CustomNativeTemplateAd nativeAd = new CustomNativeTemplateAd(client);
            client.clickHandler(nativeAd, assetName);
        }

        private static CustomNativeTemplateClient IntPtrToAdLoaderClient(
            IntPtr customNativeTemplateAd)
        {
            GCHandle handle = (GCHandle)customNativeTemplateAd;
            return handle.Target as CustomNativeTemplateClient;
        }

        [System.Serializable]
        public struct AssetNamesArrayWrapper
        {
            public string[] Assets;
        }
    }
}
