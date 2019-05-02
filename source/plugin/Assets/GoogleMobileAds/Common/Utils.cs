// Copyright (C) 2017 Google, Inc.
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
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace GoogleMobileAds.Common
{
    internal class Utils
    {
        public static void CheckInitialization()
        {
            if (!MobileAdsEventExecutor.IsActive())
            {
                Debug.Log("You intitialized an ad object but have not yet called MobileAds.Initialize(). We " +
                    "highly recommend you call MobileAds.Initialize() before interacting with the Google " +
                    "Mobile Ads SDK.");
            }
            MobileAdsEventExecutor.Initialize();
        }

        public static Texture2D GetTexture2DFromByteArray(byte[] img)
        {
            // Create a texture. Texture size does not matter, since
            // LoadImage will replace with with incoming image size.
            Texture2D nativeAdTexture = new Texture2D(1, 1);
            if (!nativeAdTexture.LoadImage(img))
            {
                throw new InvalidOperationException(@"Could not load custom native template
                        image asset as texture");
            }

            return nativeAdTexture;
        }
    }
}
