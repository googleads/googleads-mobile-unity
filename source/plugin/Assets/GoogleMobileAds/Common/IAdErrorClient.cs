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
using UnityEngine;

namespace GoogleMobileAds.Common
{
    /// <summary>
    /// Gets error information about why an ad operation failed.
    /// </summary>
    /// <returns>Ad Error Object with Code, Message, Domain, and Cause.</returns>
    public interface IAdErrorClient
    {
        /// <summary>
        /// Error code.
        /// See https://support.google.com/admob/thread/3494603/admob-error-codes-logs
        /// for explanations of common errors.
        /// </summary>
        int Code { get; }

        /// <summary>
        /// The domain from which the error came.
        /// </summary>
        /// <returns>Ad Error Domain. </returns>
        string Domain { get; }

        /// <summary>
        /// The error message.
        /// See https://support.google.com/admob/answer/9905175
        /// for explanations of common errors.
        /// <summary>
        string Message { get; }

        /// <summary>
        /// The underlying error cause if exists.
        /// <summary>
        IAdErrorClient Cause { get; }

        /// <summary>
        /// A log friendly string of the ad error.
        /// </summary>
        string Description { get; }
    }
}
