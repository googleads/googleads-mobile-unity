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

using UnityEngine;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    internal class JsonAdErrorClient : IAdErrorClient
    {
        JsonObject _json;

        public JsonAdErrorClient(JsonObject json)
        {
            _json = json;
        }

        public int GetCode()
        {
           return _json.GetValue<int>("code");
        }

        public string GetDomain()
        {
           return _json.GetValue<string>("domain");
        }

        public string GetMessage()
        {
           return _json.GetValue<string>("description");
        }

        public IAdErrorClient GetCause()
        {
           var cause = _json.GetJsonObject("cause");
           return cause == null ? null : new JsonAdErrorClient(cause);
        }

        public override string ToString()
        {
            return _json.ToString();
        }
    }
}
