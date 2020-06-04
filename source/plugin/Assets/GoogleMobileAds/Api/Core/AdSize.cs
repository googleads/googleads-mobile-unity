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

    internal enum Orientation {
        Current = 0,
        Landscape = 1,
        Portrait = 2
    }

    public class AdSize
    {
        public enum Type {
            Standard = 0,
            SmartBanner = 1,
            AnchoredAdaptive = 2
        }
        private Type type;
        private Orientation orientation;
        private int width;
        private int height;

        public static readonly AdSize Banner = new AdSize(320, 50);
        public static readonly AdSize MediumRectangle = new AdSize(300, 250);
        public static readonly AdSize IABBanner = new AdSize(468, 60);
        public static readonly AdSize Leaderboard = new AdSize(728, 90);
        public static readonly AdSize SmartBanner = new AdSize(0, 0, Type.SmartBanner);
        public static readonly int FullWidth = -1;

        public AdSize(int width, int height)
        {
            this.type = Type.Standard;
            this.width = width;
            this.height = height;
            this.orientation = Orientation.Current;
        }

        private AdSize(int width, int height, Type type) : this(width, height)
        {
            this.type = type;
        }

        private static AdSize CreateAnchoredAdaptiveAdSize(int width, Orientation orientation)
        {
            AdSize adSize = new AdSize(width, 0, Type.AnchoredAdaptive);
            adSize.orientation = orientation;
            return adSize;
        }

        public static AdSize GetLandscapeAnchoredAdaptiveBannerAdSizeWithWidth(int width) {
          return CreateAnchoredAdaptiveAdSize(width, Orientation.Landscape);
        }

        public static AdSize GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(int width) {
            return CreateAnchoredAdaptiveAdSize(width, Orientation.Portrait);
        }

        public static AdSize GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(int width) {
            return CreateAnchoredAdaptiveAdSize(width, Orientation.Current);
        }

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

        public Type AdType
        {
            get
            {
                return type;
            }
        }

        internal Orientation Orientation
        {
            get
            {
              return orientation;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            AdSize other = (AdSize)obj;
            return (width == other.width) && (height == other.height)
            && (type == other.type) && (orientation == other.orientation);
        }

        public static bool operator ==(AdSize a, AdSize b)
        {
            if ((object)a == null)
            {
                return (object)b == null;
            }

            return a.Equals(b);
        }

        public static bool operator !=(AdSize a, AdSize b)
        {
            if ((object)a == null)
            {
                return (object)b != null;
            }

            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            int hashBase = 71;
            int hashMultiplier = 11;

            int hash = hashBase;
            hash = (hash * hashMultiplier) ^ width.GetHashCode();
            hash = (hash * hashMultiplier) ^ height.GetHashCode();
            hash = (hash * hashMultiplier) ^ type.GetHashCode();
            hash = (hash * hashMultiplier) ^ orientation.GetHashCode();
            return hash;
        }
    }
}
