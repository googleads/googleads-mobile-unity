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
using System.Collections.Generic;
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    internal class ResponseInfoClient : IResponseInfoClient
    {
        private IntPtr _iosResponseInfo;

        public ResponseInfoClient(ResponseInfoClientType type, IntPtr ptr)
        {
            if(type == ResponseInfoClientType.AdLoaded)
            {
                _iosResponseInfo = Externs.GADUGetResponseInfo(ptr);
            }
            else if(type == ResponseInfoClientType.AdError)
            {
                _iosResponseInfo = Externs.GADUGetAdErrorResponseInfo(ptr);
            }
        }

        public ResponseInfoClient(IntPtr adFormat, IntPtr iOSClient)
        {
            _iosResponseInfo = iOSClient;
        }

        public List<IAdapterResponseInfoClient> GetAdapterResponses()
        {
            var list = new List<IAdapterResponseInfoClient>();
            if (_iosResponseInfo == IntPtr.Zero)
            {
                return list;
            }

            int count = Externs.GADUResponseInfoAdNetworkCount(_iosResponseInfo);
            for (int i = 0; i < count; i++)
            {
                var adNetworkRef = Externs.GADUResponseInfoAdNetworkAtIndex(_iosResponseInfo, i);
                list.Add(new AdapterResponseInfoClient(adNetworkRef));
            }
            return list;
        }

        public IAdapterResponseInfoClient GetLoadedAdapterResponseInfo()
        {
            if (_iosResponseInfo == IntPtr.Zero)
            {
                return null;
            }

            var adapterRef = Externs.GADUResponseInfoLoadedAdNetworkResponseInfo(_iosResponseInfo);
            return adapterRef == IntPtr.Zero
                    ? null
                    : new AdapterResponseInfoClient(adapterRef);
        }

        public Dictionary<string, string> GetResponseExtras()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (_iosResponseInfo == IntPtr.Zero)
            {
                return dict;
            }

            int count = Externs.GADUResponseInfoExtrasCount(_iosResponseInfo);
            for (int i = 0; i < count; i++)
            {
                string key = Externs.GADUResponseInfoExtrasKey(_iosResponseInfo, i);
                string val = Externs.GADUResponseInfoExtrasValue(_iosResponseInfo, key);
                dict.Add(key, val);
            }
            return dict;
        }

        public string GetMediationAdapterClassName()
        {
            if (_iosResponseInfo == IntPtr.Zero)
            {
                return string.Empty;
            }

            return Externs.GADUResponseInfoMediationAdapterClassName(_iosResponseInfo);
        }

        public string GetResponseId()
        {
            if (_iosResponseInfo == IntPtr.Zero)
            {
                return string.Empty;
            }

            return Externs.GADUResponseInfoResponseId(_iosResponseInfo);
        }

        public override string ToString()
        {
            if (_iosResponseInfo == IntPtr.Zero)
            {
                return string.Empty;
            }

            return Externs.GADUGetResponseInfoDescription(_iosResponseInfo);
        }
    }
}
#endif
