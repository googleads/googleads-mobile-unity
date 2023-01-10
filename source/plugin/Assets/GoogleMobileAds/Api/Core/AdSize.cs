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
    /// <summary>
    /// Defines the different orientations that a layout or device can have.
    /// </summary>
        internal enum Orientation {
        Current = 0,
        /// <summary>
        /// Oriented landscape.
        /// </summary>
        Landscape = 1,
        /// <summary>
        /// Oriented portrait.
        /// </summary>
        Portrait = 2
    }

    /// <summary>
    /// The size of a banner ad.
    /// <seealso cref="https://developers.google.com/admob/unity/banner#banner_sizes">
    /// Banner sizes.</seealso>
    /// </summary>
    public class AdSize
    {
        /// <summary>
        /// The type of banner ad.
        /// </summary>
        public enum Type
        {
            Standard = 0,
            [System.Obsolete("Deprecated. Use AnchoredAdaptive.")]
            SmartBanner = 1,
            AnchoredAdaptive = 2
        }

        private Type _type;
        private Orientation _orientation;
        private int _width;
        private int _height;

        /// <summary>
        /// Interactive Advertising Bureau (IAB) banner ad size (320x50 density-independent pixels).
        /// </summary>
        public static readonly AdSize Banner = new AdSize(320, 50);

        /// <summary>
        /// Interactive Advertising Bureau (IAB) medium rectangle ad size (300x250
        /// density-independent pixels).
        /// </summary>
        public static readonly AdSize MediumRectangle = new AdSize(300, 250);

        /// <summary>
        /// Interactive Advertising Bureau (IAB) full banner ad size
        /// (468x60 density-independent pixels).
        /// </summary>
        public static readonly AdSize IABBanner = new AdSize(468, 60);

        /// <summary>
        /// Interactive Advertising Bureau (IAB) leaderboard ad size
        /// (728x90 density-independent pixels).
        /// </summary>
        public static readonly AdSize Leaderboard = new AdSize(728, 90);

        /// <summary>
        /// A dynamically sized banner that is full-width and auto-height.
        /// </summary>
        [System.Obsolete("Deprecated. Use AnchoredAdaptive.")]
        public static readonly AdSize SmartBanner = new AdSize(0, 0, Type.SmartBanner);

        public static readonly int FullWidth = -1;

        /// <summary>
        /// Creates a new <see cref="AdSize"/>.
        /// </summary>
        /// <param name="width">The width of the ad in density-independent pixels.</param>
        /// <param name="height">The height of the ad in density-independent pixels.</param>
        public AdSize(int width, int height)
        {
            _type = Type.Standard;
            _width = width;
            _height = height;
            _orientation = Orientation.Current;
        }

        private AdSize(int width, int height, Type type) : this(width, height)
        {
            _type = type;
        }

        private static AdSize CreateAnchoredAdaptiveAdSize(int width, Orientation orientation)
        {
            AdSize adSize = new AdSize(width, 0, Type.AnchoredAdaptive);
            adSize._orientation = orientation;
            return adSize;
        }

        public static AdSize GetLandscapeAnchoredAdaptiveBannerAdSizeWithWidth(int width)
        {
            return CreateAnchoredAdaptiveAdSize(width, Orientation.Landscape);
        }

        public static AdSize GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(int width)
        {
            return CreateAnchoredAdaptiveAdSize(width, Orientation.Portrait);
        }

        public static AdSize GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(int width)
        {
            return CreateAnchoredAdaptiveAdSize(width, Orientation.Current);
        }

        /// <summary>
        /// The width of the ad in density-independent pixels.
        /// </summary>
        public int Width
        {
            get
            {
                return _width;
            }
        }

        /// <summary>
        /// The height of the ad in density-independent pixels.
        /// </summary>
        public int Height
        {
            get
            {
                return _height;
            }
        }

        /// <summary>
        /// The type of banner ad.
        /// </summary>
        public Type AdType
        {
            get
            {
                return _type;
            }
        }

        /// <summary>
        /// The orientations of the banner ad. 
        /// </summary>
        internal Orientation Orientation
        {
            get
            {
              return _orientation;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            AdSize other = (AdSize)obj;
            return (_width == other._width) && (_height == other._height)
            && (_type == other._type) && (_orientation == other._orientation);
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
            hash = (hash * hashMultiplier) ^ _width.GetHashCode();
            hash = (hash * hashMultiplier) ^ _height.GetHashCode();
            hash = (hash * hashMultiplier) ^ _type.GetHashCode();
            hash = (hash * hashMultiplier) ^ _orientation.GetHashCode();
            return hash;
        }
    }
}
