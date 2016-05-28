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
    public class CustomNativeTemplateClient : ICustomNativeTemplateClient
    {
        [System.Serializable]
        public struct AssetNamesArrayWrapper
        {
            public string[] assets;
        }

        private IntPtr customNativeAd;
        private Action<CustomNativeTemplateAd, string> clickHandler;

        internal delegate void GADUNativeCustomTemplateDidReceiveClick(
            IntPtr nativeCustomTemplateAd, string error);

        public CustomNativeTemplateClient(
            IntPtr customNativeAd, Action<CustomNativeTemplateAd, string> clickHandler)
        {
            this.customNativeAd = customNativeAd;
            this.clickHandler = clickHandler;

            IntPtr customNativeCustomTemplateAdClientPtr = (IntPtr)GCHandle.Alloc(this);

            Externs.GADUSetNativeCustomTemplateAdUnityClient(customNativeAd,
                                                             customNativeCustomTemplateAdClientPtr);

            Externs.GADUSetNativeCustomTemplateAdCallbacks(customNativeAd,
                NativeCustomTemplateDidReceiveClickCallback);
        }

        public List<string> GetAvailableAssetNames()
        {
            string encodedJson = Externs.GADUNativeCustomTemplateAdAvailableAssetKeys(
                    customNativeAd);
            string decodedJson = System.Text.Encoding.UTF8.GetString(
                    System.Convert.FromBase64String(encodedJson));
            AssetNamesArrayWrapper assetNamesWrapper = JsonUtility.FromJson<AssetNamesArrayWrapper>(
                    decodedJson);
            return new List<string>(assetNamesWrapper.assets);
        }

        public string GetTemplateId()
        {
            return Externs.GADUNativeCustomTemplateAdTemplateID(customNativeAd);
        }

        public byte[] GetImageByteArray(string key)
        {
            string bytesString = Externs.GADUNativeCustomTemplateAdImageAsBytesForKey(
                customNativeAd, key);
            return System.Convert.FromBase64String(bytesString);
        }

        public string GetText(string key)
        {
            return Externs.GADUNativeCustomTemplateAdStringForKey(customNativeAd, key);
        }

        public void PerformClick(string assetName)
        {
            bool customClickAction = this.clickHandler != null;
            Externs.GADUNativeCustomTemplateAdPerformClickOnAssetWithKey(
                customNativeAd, assetName, customClickAction);
        }

        [MonoPInvokeCallback(typeof(GADUNativeCustomTemplateDidReceiveClick))]
        private static void NativeCustomTemplateDidReceiveClickCallback(
            IntPtr nativeCustomAd, string assetName)
        {
            CustomNativeTemplateClient client = IntPtrToAdLoaderClient(nativeCustomAd);
            CustomNativeTemplateAd nativeAd = new CustomNativeTemplateAd(client);
            client.clickHandler(nativeAd, assetName);
        }

        public void RecordImpression()
        {
            Externs.GADUNativeCustomTemplateAdRecordImpression(customNativeAd);
        }

        private static CustomNativeTemplateClient IntPtrToAdLoaderClient(
                IntPtr customNativeTemplateAd)
        {
            GCHandle handle = (GCHandle)customNativeTemplateAd;
            return handle.Target as CustomNativeTemplateClient;
        }
    }
}
