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
    [Serializable]
    internal class JsonAdErrorClient : IAdErrorClient,
                                       IAdInspectorErrorClient,
                                       ILoadAdErrorClient
    {
        [SerializeField]
        protected string cause;
        [SerializeField]
        protected int code;
        [SerializeField]
        protected string description;
        [SerializeField]
        protected string domain;
        [SerializeField]
        protected string message;
        [SerializeField]
        protected JsonResponseInfoClient responseInfo;

        public JsonAdErrorClient()
        {
        }

        public JsonAdErrorClient(string json)
        {
            UnityEngine.JsonUtility.FromJsonOverwrite(json, this);
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
            // Serialize as string due to Unity nesting limits. b/243737332
            return JsonUtility.FromJson<JsonAdErrorClient>(cause);
        }

        public override string ToString()
        {
            return description;
        }

        public IResponseInfoClient GetResponseInfoClient()
        {
            return responseInfo;
        }
    }
}
