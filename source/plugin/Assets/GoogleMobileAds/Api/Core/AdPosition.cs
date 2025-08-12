// <copyright file="AdPosition.cs" company="Google LLC">
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
    /// The position of the ad in screen space.
    /// </summary>
    public enum AdPosition
    {

        /// <summary>
        /// Top of the screen, center aligned.
        /// </summary>
        Top = 0,

        /// <summary>
        /// Bottom of the screen, center aligned.
        /// </summary>
        Bottom = 1,

        /// <summary>
        /// Top left corner of the screen.
        /// </summary>
        TopLeft = 2,

        /// <summary>
        /// Top right corner of the screen.
        /// </summary>
        TopRight = 3,

        /// <summary>
        /// Bottom left corner of the screen.
        /// </summary>
        BottomLeft = 4,

        /// <summary>
        /// Bottom right corner of the screen.
        /// </summary>
        BottomRight = 5,

        /// <summary>
        /// Centered on the screen both horizontally and vertically.
        /// </summary>
        Center = 6
    }
}
