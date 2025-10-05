// Copyright (C) 2019 Google, Inc.
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
    /// Options for rewarded ad types server-side verification callbacks.
    /// Options are created using <see cref="ServerSideVerificationOptions.Builder"/>.
    /// </summary>
    [Serializable]
    public class ServerSideVerificationOptions
    {
        /// <summary>
        /// The user id of the current user.
        /// </summary>
        public string UserId;

        /// <summary>
        /// Custom data of the current user.
        /// </summary>
        public string CustomData;

        public ServerSideVerificationOptions() {}

        public ServerSideVerificationOptions(ServerSideVerificationOptions options)
        {
            UserId = options.UserId;
            CustomData = options.CustomData;
        }
    }
}
