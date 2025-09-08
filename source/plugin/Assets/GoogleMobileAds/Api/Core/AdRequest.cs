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

using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

using GoogleMobileAds.Api.Mediation;

namespace GoogleMobileAds.Api
{
    /// <summary>
    /// An <see cref="AdRequest"/> contains targeting information used to fetch an ad.
    /// Ad requests are created using <see cref="AdRequest.Builder"/>.
    /// </summary>
    [Serializable]
    public class AdRequest
    {
        /// <summary>
        /// Request version string.
        /// </summary>
        public static string Version { get; private set; }

        /// <summary>
        /// Test device ID used to load test ads.
        /// <seealso href="https://developers.google.com/admob/unity/test-ads"/>.
        /// </summary>
        public const string TestDeviceSimulator = "SIMULATOR";

        /// <summary>
        /// The custom targeting parameters.
        /// </summary>
        public Dictionary<string, string> CustomTargeting = new Dictionary<string, string>();

        static AdRequest()
        {
            Version version = typeof(AdRequest).Assembly.GetName().Version;
            Version = string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }

        public AdRequest() {}

        public AdRequest(AdRequest request)
        {
            Keywords = request.Keywords;
            Extras = request.Extras;
            MediationExtras = request.MediationExtras;
            CustomTargeting = request.CustomTargeting;
        }

        /// <summary>
        /// Returns targeting information keywords. Returns an empty set if no keywords were added.
        /// </summary>
        public HashSet<string> Keywords = new HashSet<string>();

        /// <summary>
        /// Returns extra parameters to be sent in the ad request.
        /// </summary>
        public Dictionary<string, string> Extras = new Dictionary<string, string>();

        /// <summary>
        /// A long integer provided by the AdMob UI for the configured placement.
        /// </summary>
        public long PlacementID;

        /// <summary>
        /// Returns extra parameters to be sent to a specific ad partner in the ad request.
        /// </summary>
        public List<MediationExtras> MediationExtras = new List<MediationExtras>();

        internal static string BuildVersionString(string nativePluginVersion = null)
        {
            var versionBuilder = new StringBuilder("unity-");
            versionBuilder.Append(AdRequest.Version);
            if (!string.IsNullOrEmpty(nativePluginVersion)) {
                versionBuilder.Append("-native-");
                versionBuilder.Append(nativePluginVersion);
            }
            return versionBuilder.ToString();
        }
    }
}
