// <copyright file="MaxAdContentRating.cs" company="Google LLC">
// Copyright (C) 2020 Google LLC
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
    /// The maximum ad content rating. All Google ads will have this content rating or lower.
    /// </summary>
    public class MaxAdContentRating
    {
        private MaxAdContentRating(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets G rating - content suitable for general audiences, including families.
        /// </summary>
        public static MaxAdContentRating G
        {
            get { return new MaxAdContentRating("G"); }
        }

        /// <summary>
        /// Gets PG rating - content suitable for most audiences with parental guidance.
        /// </summary>
        public static MaxAdContentRating PG
        {
            get { return new MaxAdContentRating("PG"); }
        }

        /// <summary>
        /// Gets T rating - content suitable for teen and older audiences.
        /// </summary>
        public static MaxAdContentRating T
        {
            get { return new MaxAdContentRating("T"); }
        }

        /// <summary>
        /// Gets MA rating - content suitable only for mature audiences.
        /// </summary>
        public static MaxAdContentRating MA
        {
            get { return new MaxAdContentRating("MA"); }
        }

        /// <summary>
        /// Gets Unspecified rating - content suitability is unspecified.
        /// </summary>
        public static MaxAdContentRating Unspecified
        {
            get { return new MaxAdContentRating(string.Empty); }
        }

        /// <summary>
        /// Gets or sets the string representation of <see cref="MaxAdContentRating"/>.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Parses <see cref="MaxAdContentRating"/> from a string constant.
        /// </summary>
        /// <param name="value">String constant representing the content rating.</param>
        /// <returns>The parsed <see cref="MaxAdContentRating"/>.</returns>
        public static MaxAdContentRating ToMaxAdContentRating(string value)
        {
            return new MaxAdContentRating(value);
        }
    }
}
