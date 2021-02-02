// Copyright (C) 2019 Google LLC
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

using UnityEngine;

using GoogleMobileAds.Api;

namespace GoogleMobileAds.Placement
{
    [Serializable]
    public class AdSizeProperty
    {
        public AdSize.Type type;

        public int width;

        public int height;

        public Orientation orientation;

        public AdSize ToAdSize()
        {
            switch (type)
            {
                case AdSize.Type.Standard:
                    return new AdSize(width, height);
                case AdSize.Type.SmartBanner:
                    return AdSize.SmartBanner;
                case AdSize.Type.AnchoredAdaptive:
                    return ToAnchoredAdaptiveAdSize();
                default:
                    throw new ArgumentException("Unexpected type: " + type);
            }
        }

        private AdSize ToAnchoredAdaptiveAdSize()
        {
            int adWidth = width == AdSize.FullWidth ? MobileAds.Utils.GetDeviceSafeWidth() : CalculateAdWidth(width);
            switch (orientation)
            {
                case Orientation.Current:
                    return AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(adWidth);
                case Orientation.Landscape:
                    return AdSize.GetLandscapeAnchoredAdaptiveBannerAdSizeWithWidth(adWidth);
                case Orientation.Portrait:
                    return AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(adWidth);
                default:
                    throw new ArgumentException("Unexpected orientation: " + orientation);
            }
        }

        private static int CalculateAdWidth(int percentScreenWidth)
        {
            if (percentScreenWidth < 50)
            {
                percentScreenWidth = 50;
            }
            else if (percentScreenWidth > 99)
            {
                percentScreenWidth = 99;
            }
            return (int) (MobileAds.Utils.GetDeviceSafeWidth() * percentScreenWidth / 100);
        }
    }
}
