﻿// Copyright (C) 2022 Google LLC
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

using System.Collections.Generic;

namespace GoogleMobileAds.Ump
{
    /// <summary>
    /// Debug settings for testing User Messaging Platform.
    /// </summary>
    public class ConsentDebugSettings
    {
        /// <summary>
        /// Debug values for testing geography.
        /// </summary>
        public enum DebugGeography
        {
            /// <summary>
            /// Debug geography disabled.
            /// </summary>
            DEBUG_GEOGRAPHY_DISABLED = 0,
            /// <summary>
            /// Geography appears as in EEA for debug devices.
            /// </summary>
            DEBUG_GEOGRAPHY_EEA = 1,
            /// <summary>
            ///  Geography appears as not in EEA for debug devices.
            /// </summary>
            DEBUG_GEOGRAPHY_NOT_EEA = 2,
        }

        /// <summary>
        /// The debug geography for testing purposes.
        /// </summary>
        public DebugGeography TestDebugGeography { get; set; }
        public List<string> TestDeviceHashedIds { get; set; }

        public ConsentDebugSettings(DebugGeography debugGeography, List<string> testDeviceHashedIds)
        {
            TestDebugGeography = debugGeography;
            TestDeviceHashedIds = testDeviceHashedIds;
        }
    }
}
