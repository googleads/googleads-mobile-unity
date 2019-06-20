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

namespace GoogleMobileAds.Api
{

// copybara_strip_begin
    internal enum Orientation {
         Current = 0,
         Landscape = 1,
         Portrait = 2
    }
// copybara_strip_end

    public class AdSize
    {
// copybara_strip_begin
        private bool isAdaptiveBanner;
        private Orientation orientation;
// copybara_strip_end
        private bool isSmartBanner;
        private int width;
        private int height;

        public static readonly AdSize Banner = new AdSize(320, 50);
        public static readonly AdSize MediumRectangle = new AdSize(300, 250);
        public static readonly AdSize IABBanner = new AdSize(468, 60);
        public static readonly AdSize Leaderboard = new AdSize(728, 90);
        public static readonly AdSize SmartBanner = new AdSize(true);
        public static readonly int FullWidth = -1;

        public AdSize(int width, int height)
        {
            isSmartBanner = false;
            this.width = width;
            this.height = height;
// copybara_strip_begin
            isAdaptiveBanner = false;
            this.orientation = Orientation.Current;
// copybara_strip_end
        }

        private AdSize(bool isSmartBanner) : this(0, 0)
        {
            this.isSmartBanner = isSmartBanner;
        }
// copybara_strip_begin
        private static AdSize CreateAdaptiveAdSize(int width, Orientation orientation)
        {
            AdSize adSize = new AdSize(width, 0);
            adSize.isAdaptiveBanner = true;
            adSize.orientation = orientation;
            return adSize;
        }


        public static AdSize GetLandscapeBannerAdSizeWithWidth(int width) {
          return CreateAdaptiveAdSize(width, Orientation.Landscape);
        }

        public static AdSize GetPortraitBannerAdSizeWithWidth(int width) {
            return CreateAdaptiveAdSize(width, Orientation.Portrait);
        }

        public static AdSize GetCurrentOrientationBannerAdSizeWithWidth(int width) {
            return CreateAdaptiveAdSize(width, Orientation.Current);
        }
// copybara_strip_end
        public int Width
        {
            get
            {
                return width;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }

        public bool IsSmartBanner
        {
            get
            {
                return isSmartBanner;
            }
        }
// copybara_strip_begin
        public bool IsAdaptiveBanner
        {
            get
            {
                return isAdaptiveBanner;
            }
        }

        internal Orientation Orientation
        {
            get
            {
              return orientation;
            }
        }
// copybara_strip_end

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            AdSize other = (AdSize)obj;
            return (width == other.width) && (height == other.height)
// copybara_strip_begin
            && (isAdaptiveBanner == other.isAdaptiveBanner) && (orientation == other.orientation)
// copybara_strip_end
            && (isSmartBanner == other.isSmartBanner);
        }

        public static bool operator ==(AdSize a, AdSize b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(AdSize a, AdSize b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            int hashBase = 71;
            int hashMultiplier = 11;

            int hash = hashBase;
            hash = (hash * hashMultiplier) ^ width.GetHashCode();
            hash = (hash * hashMultiplier) ^ height.GetHashCode();
            hash = (hash * hashMultiplier) ^ isSmartBanner.GetHashCode();
// copybara_strip_begin
            hash = (hash * hashMultiplier) ^ isAdaptiveBanner.GetHashCode();
            hash = (hash * hashMultiplier) ^ orientation.GetHashCode();
// copybara_strip_end
            return hash;
        }
    }
}
