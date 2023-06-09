
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

namespace GoogleMobileAds.Api
{
    /// <summary>
    /// Event args for a paid event.
    /// </summary>
    /// @deprecated Use @ref AdValue directly.
    public class AdValueEventArgs : EventArgs
    {
        /// <summary>
        /// The monetary value earned from an ad.
        /// </summary>
        public AdValue AdValue { get; set; }
    }
}

