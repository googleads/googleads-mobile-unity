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
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    internal class ResponseInfoClient : IResponseInfoClient
    {
        private IntPtr _responsePtr;

        public string MediationAdapterClassName
        {
            get
            {
                return Externs.GADUResponseInfoMediationAdapterClassName(_responsePtr);
            }
        }

        public string ResponseId
        {
            get
            {
                return Externs.GADUResponseInfoResponseId(_responsePtr);
            }
        }

        public string Description
        {
            get
            {
                return Externs.GADUGetResponseInfoDescription(_responsePtr);
            }
        }

        internal ResponseInfoClient(IntPtr responsePtr)
        {
            _responsePtr = responsePtr;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
#endif
