// Copyright (C) 2022 Google, LLC
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
    /// Event interface for user rewarding ad formats.
    /// </summary>
    public interface IBaseRewardedAd
    {
        /// <summary>
        /// The reward item for the loaded rewarded ad.
        /// Returns null if the ad is not loaded.
        /// </summary>
        Reward GetRewardItem();

        /// <summary>
        /// Shows a rewarded ad.
        /// </summary>
        /// <param name="userRewardEarnedCallback">
        /// Called when the user earns a reward.
        /// </param>
        void Show(Action<Reward> userRewardEarnedCallback);

        /// <summary>
        /// Sets the server side verification options.
        /// </summary>
        void SetServerSideVerificationOptions(ServerSideVerificationOptions options);
    }
}
