// Copyright (C) 2024 Google LLC
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
    /// Media aspect ratio.
    /// </summary>
    public enum MediaAspectRatio
    {
        /// <summary>
        /// Unknown media aspect ratio.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Any media aspect ratio.
        /// </summary>
        Any = 1,
        /// <summary>
        /// Landscape media aspect ratio.
        /// </summary>
        Landscape = 2,
        /// <summary>
        /// Portrait media aspect ratio.
        /// </summary>
        Portrait = 3,
        /// <summary>
        /// Square media aspect ratio. This is not a strict 1:1 aspect ratio.
        /// </summary>
        Square = 4,
    }
}
