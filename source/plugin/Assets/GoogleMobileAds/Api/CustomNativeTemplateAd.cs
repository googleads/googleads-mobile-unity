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
using UnityEngine;

using GoogleMobileAds.Common;

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
            return client.GetAvailableAssetNames();
        }

        public string GetCustomTemplateId()
        {
            return client.GetTemplateId();
        }

        public Texture2D GetTexture2D(string key)
        {
            return Utils.GetTexture2DFromByteArray(client.GetImageByteArray(key));
        }

        public string GetText(string key)
        {
            return client.GetText(key);
        }

        public void PerformClick(string assetName)
        {
            client.PerformClick(assetName);
        }

        public void RecordImpression()
        {
            client.RecordImpression();
        }
    }
}
