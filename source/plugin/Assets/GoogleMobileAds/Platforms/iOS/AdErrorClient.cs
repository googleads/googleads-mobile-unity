#if UNITY_IOS
// Copyright (C) 2020 Google, LLC
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
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    internal class AdErrorClient : IAdErrorClient
    {
        private int code;
        private string domain;
        private string message;
        private string description;
        private AdErrorClient inner;

        private AdErrorClient(IntPtr error)
        {
            // Because Native objects are weak references and may be released,
            // we cache the members for use in case we access this object later.
            this.code = Externs.GADUGetAdErrorCode(error);
            this.domain = Externs.GADUGetAdErrorDomain(error);
            this.message = Externs.GADUGetAdErrorMessage(error);
            this.description = Externs.GADUGetAdErrorDescription(error);

            IntPtr cause = Externs.GADUGetAdErrorUnderLyingError(error);
            if (cause != IntPtr.Zero)
            {
              this.inner = new AdErrorClient(cause);
            }
        }

        public int GetCode()
        {
           return code;
        }

        public string GetDomain()
        {
            return domain;
        }

        public string GetMessage()
        {
            return message;
        }

        public IAdErrorClient GetCause()
        {
            return inner;
        }

        public override string ToString()
        {
            return description;
        }
    }
}
#endif
