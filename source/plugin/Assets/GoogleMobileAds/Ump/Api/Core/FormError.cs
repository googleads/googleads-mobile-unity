// Copyright (C) 2022 Google LLC
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

namespace GoogleMobileAds.Ump.Api
{
    /// <summary>
    /// Error information about why a form operation failed.
    /// </summary>
    public class FormError
    {
        /// <summary>
        /// The error code.
        /// <remarks>
        /// The error code constant values are different depending on the runtime platform.
        /// For iOS <see href="https://developers.google.com/admob/ump/ios/api/reference/Enums/UMPFormErrorCode"/>
        /// For Android <see href="https://developers.google.com/admob/ump/android/api/reference/com/google/android/ump/FormError"/>
        /// </remarks>
        /// </summary>
        public int ErrorCode { get; private set; }

        /// <summary>
        /// The message describing the error.
        /// </summary>
        public string Message { get; private set; }

        internal FormError(int errorCode, string message)
        {
            ErrorCode = errorCode;
            Message = message;
        }
    }
}
