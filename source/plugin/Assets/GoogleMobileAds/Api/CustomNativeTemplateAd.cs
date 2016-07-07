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
using System.Collections.Generic;

using GoogleMobileAds.Common;
using UnityEngine;

namespace GoogleMobileAds.Api
{
    public class CustomNativeTemplateAd
    {
        private ICustomNativeTemplateClient client;

        internal CustomNativeTemplateAd(ICustomNativeTemplateClient client)
        {
            this.client = client;
        }

        public List<string> GetAvailableAssetNames()
        {
            return this.client.GetAvailableAssetNames();
        }

        public string GetCustomTemplateId()
        {
            return this.client.GetTemplateId();
        }

        // Get image asset corresponding to the key parameter of custom native template ad as a
        // Texture2D. If the asset key does not map to an existing asset, a null object will be
        // returned.
        public Texture2D GetTexture2D(string key)
        {
            byte[] imageAssetAsByteArray = this.client.GetImageByteArray(key);
            if (imageAssetAsByteArray == null)
            {
                return null;
            }

            return Utils.GetTexture2DFromByteArray(imageAssetAsByteArray);
        }

        // Get text asset corresponding to the key parameter of custom native template ad as a
        // string. If the asset key does not map to an existing asset, a null object will be
        // returned.
        public string GetText(string key)
        {
            return this.client.GetText(key);
        }

        public void PerformClick(string assetName)
        {
            this.client.PerformClick(assetName);
        }

        public void RecordImpression()
        {
            this.client.RecordImpression();
        }
    }
}
