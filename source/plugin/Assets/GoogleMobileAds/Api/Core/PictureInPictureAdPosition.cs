// Copyright 2026 Google LLC
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
    /// The screen corner position where a Picture-in-Picture ad is displayed.
    /// </summary>
    public enum PictureInPictureAdPosition
    {
        /// <summary>
        /// Default placement on the screen (typically bottom right).
        /// </summary>
        Default = 0,

        /// <summary>
        /// Bottom right corner of the screen.
        /// </summary>
        BottomRight = 1,

        /// <summary>
        /// Bottom left corner of the screen.
        /// </summary>
        BottomLeft = 2,

        /// <summary>
        /// Top left corner of the screen.
        /// </summary>
        TopLeft = 3,

        /// <summary>
        /// Top right corner of the screen.
        /// </summary>
        TopRight = 4
    }
}
