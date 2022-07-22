// Copyright 2015-2021 Google LLC
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

using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
    /// <summary>
    /// A full page ad experience at natural transition points such as a page change, an app launch.
    /// Interstitials use a close button that removes the ad from the user's experience.
    /// </summary>
    public interface IInterstitialClient : IFullScreenAdClient
    {
        /// <summary>
        /// Shows the InterstitialAd on the screen.
        /// </summary>
        void Show();

        /// <summary>
        /// Loads an IInterstitialAd.
        /// </summary>
        void LoadInterstitialAd(string adUnitId,
                                AdRequest request,
                                Action<IInterstitialClient, ILoadAdErrorClient> callback);
    }
}
