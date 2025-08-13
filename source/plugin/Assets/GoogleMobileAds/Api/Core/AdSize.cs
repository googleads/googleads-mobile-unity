// <copyright file="AdSize.cs" company="Google LLC">
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
// </copyright>

namespace GoogleMobileAds.Api
{
    /// <summary>
    /// The size of a banner ad.
    /// <seealso cref="https://developers.google.com/admob/unity/banner#banner_sizes">
    /// Banner sizes.</seealso>
    /// </summary>
    public class AdSize
    {
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

        /// <summary>
        /// Constant for full width.
        /// </summary>
        public static readonly int FullWidth = -1;

        private Type _type;
        private Orientation _orientation;
        private int _width;
        private int _height;

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

        /// <summary>
        /// Creates a new <see cref="AdSize"/> for an anchored adaptive banner.
        /// </summary>
        /// <param name="width">The width of the ad in density-independent pixels.</param>
        /// <param name="height">The height of the ad in density-independent pixels.</param>
        /// <param name="type">The type of banner ad.</param>
        private AdSize(int width, int height, Type type) : this(width, height)
        {
            _type = type;
        }

        /// <summary>
        /// The type of banner ad.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// Standard banner ad type.
            /// </summary>
            Standard = 0,

            /// <summary>
            /// Smart banner ad type.
            /// </summary>
            [System.Obsolete("Deprecated. Use AnchoredAdaptive.")]
            SmartBanner = 1,

            /// <summary>
            /// Anchored adaptive banner ad type.
            /// </summary>
            AnchoredAdaptive = 2
        }

        /// <summary>
        /// Gets the width of the ad in density-independent pixels.
        /// </summary>
        public int Width
        {
            get
            {
                return _width;
            }
        }

        /// <summary>
        /// Gets the height of the ad in density-independent pixels.
        /// </summary>
        public int Height
        {
            get
            {
                return _height;
            }
        }

        /// <summary>
        /// Gets the type of banner ad.
        /// </summary>
        public Type AdType
        {
            get
            {
                return _type;
            }
        }

        /// <summary>
        /// Gets the orientations of the banner ad.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return _orientation;
            }
        }

        /// <summary>
        /// Gets an anchored adaptive <see cref="AdSize"/> for the landscape orientation with the given width.
        /// </summary>
        /// <param name="width">The width of the ad in density-independent pixels.</param>
        /// <returns>An AdSize object for landscape orientation.</returns>
        public static AdSize GetLandscapeAnchoredAdaptiveBannerAdSizeWithWidth(int width)
        {
            return CreateAnchoredAdaptiveAdSize(width, Orientation.Landscape);
        }

        /// <summary>
        /// Gets an anchored adaptive <see cref="AdSize"/> for the portrait orientation with the given width.
        /// </summary>
        /// <param name="width">The width of the ad in density-independent pixels.</param>
        /// <returns>An AdSize object for portrait orientation.</returns>
        public static AdSize GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(int width)
        {
            return CreateAnchoredAdaptiveAdSize(width, Orientation.Portrait);
        }

        /// <summary>
        /// Gets an anchored adaptive <see cref="AdSize"/> for the current orientation with the given width.
        /// </summary>
        /// <param name="width">The width of the ad in density-independent pixels.</param>
        /// <returns>An AdSize object for the current orientation.</returns>
        public static AdSize GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(int width)
        {
            return CreateAnchoredAdaptiveAdSize(width, Orientation.Current);
        }

        /// <summary>
        /// Compares two AdSize objects for equality.
        /// </summary>
        /// <param name="a">The first AdSize to compare.</param>
        /// <param name="b">The second AdSize to compare.</param>
        /// <returns>True if the AdSize objects are equal, false otherwise.</returns>
        public static bool operator ==(AdSize a, AdSize b)
        {
            if ((object)a == null)
            {
                return (object)b == null;
            }

            return a.Equals(b);
        }

        /// <summary>
        /// Compares two AdSize objects for inequality.
        /// </summary>
        /// <param name="a">The first AdSize to compare.</param>
        /// <param name="b">The second AdSize to compare.</param>
        /// <returns>True if the AdSize objects are not equal, false otherwise.</returns>
        public static bool operator !=(AdSize a, AdSize b)
        {
            if ((object)a == null)
            {
                return (object)b != null;
            }

            return !a.Equals(b);
        }

        /// <summary>
        /// Checks if the given object is equal to the current AdSize.
        /// </summary>
        /// <param name="obj">The object to compare with the current AdSize.</param>
        /// <returns>
        /// True if the given object is equal to the current AdSize, false otherwise.
        /// </returns>
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

        /// <summary>
        /// Gets the hash code for the current AdSize.
        /// </summary>
        /// <returns>The hash code for the current AdSize.</returns>
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

        /// <summary>
        /// Creates an anchored adaptive <see cref="AdSize"/> for the given orientation and width.
        /// </summary>
        /// <param name="width">The width of the ad in density-independent pixels.</param>
        /// <param name="orientation">The orientation of the banner ad.</param>
        /// <returns>An AdSize object for the given orientation and width.</returns>
        private static AdSize CreateAnchoredAdaptiveAdSize(int width, Orientation orientation)
        {
            AdSize adSize = new AdSize(width, 0, Type.AnchoredAdaptive);
            adSize._orientation = orientation;
            return adSize;
        }
    }
}
