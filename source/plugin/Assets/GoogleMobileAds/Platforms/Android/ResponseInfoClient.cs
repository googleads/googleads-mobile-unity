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

namespace GoogleMobileAds.Android
{
    internal class ResponseInfoClient : IResponseInfoClient
    {
        private AndroidJavaObject _androidResponseInfo;
        private JsonObject _jsonObject;

        public ResponseInfoClient(ResponseInfoClientType type, AndroidJavaObject androidJavaObject)
        {
            // Too much work (I/O) in the constructor
            try
            {
                _androidResponseInfo = androidJavaObject.Call<AndroidJavaObject>("getResponseInfo");
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
        }

        public IAdapterResponseInfoClient[] GetAdapterResponses()
        {
            var listJson = GetJson().GetJsonObjectList("Adapter Responses");
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
            return GetJson().GetValue<string>("Mediation Adapter Class Name");
        }

        public Dictionary<string, string> GetResponseExtras()
        {
            return GetJson().GetDictionary<string>("Response Extras");
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
            if (_androidResponseInfo == null)
            {
                _jsonObject = new JsonObject("");
                return _jsonObject;
            }

            var jsonString = _androidResponseInfo.Call<string>("toString");
            _jsonObject = new JsonObject(jsonString);
            return _jsonObject;
        }
    }
}
