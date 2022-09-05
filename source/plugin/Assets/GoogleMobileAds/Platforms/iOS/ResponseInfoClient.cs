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
using System.Linq;
using UnityEngine;

using GoogleMobileAds.Common;
using System.Collections.Generic;

namespace GoogleMobileAds.iOS
{
    internal class ResponseInfoClient : IResponseInfoClient
    {
        private IntPtr _iosResponseInfo;
        private JsonObject _jsonObject;

        public ResponseInfoClient(ResponseInfoClientType type, IntPtr ptr)
        {
            // Too much work (I/O) in the constructor
            try
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
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
        }

        public ResponseInfoClient(IntPtr _adFormat, IntPtr iOSClient)
        {
            _iosResponseInfo = iOSClient;
        }

        public IAdapterResponseInfoClient[] GetAdapterResponses()
        {
            var listJson = GetJson().GetJsonObjectList("Mediation line items");
            return listJson == null
                    ? new IAdapterResponseInfoClient[0]
                    : listJson.Select(item => new AdapterResponseInfoClient(item)).ToArray();
        }

        public IAdapterResponseInfoClient GetLoadedAdapterResponseInfo()
        {
            var adapterJson = GetJson().GetJsonObject("Loaded Adapter Response");
            return adapterJson == null
                    ? null
                    : new AdapterResponseInfoClient(adapterJson);
        }

        public string GetMediationAdapterClassName()
        {
            return GetJson().GetValue<string>("Network");
        }

        public Dictionary<string, string> GetResponseExtras()
        {
            return GetJson().GetDictionary<string>("Extras Dictionary");
        }

        public string GetResponseId()
        {
            return GetJson().GetValue<string>("Response ID");
        }

        public override string ToString()
        {
            return GetJson().ToString();
        }

        private JsonObject GetJson()
        {
            if (_jsonObject != null)
            {
                return _jsonObject;
            }
            if (_iosResponseInfo == IntPtr.Zero)
            {
                _jsonObject = new JsonObject("");
                return _jsonObject;
            }

            var jsonString = Externs.GADUGetResponseInfoJson(_iosResponseInfo);
            _jsonObject = new JsonObject(jsonString);
            return _jsonObject;
        }
    }
}
#endif
