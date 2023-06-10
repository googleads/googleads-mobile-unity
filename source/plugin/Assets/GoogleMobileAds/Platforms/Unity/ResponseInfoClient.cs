// Copyright (C) 2020 Google LLC.
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

using System.Collections.Generic;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Unity
{
    public class ResponseInfoClient : IResponseInfoClient
    {
        public string GetMediationAdapterClassName()
        {
            return "Placeholder Mediation Adapter Class Name";
        }

        public string GetResponseId()
        {
            return "Placeholder Response ID";
        }

        public IAdapterResponseInfoClient GetLoadedAdapterResponseInfo()
        {
            return new AdapterResponseInfoClient();
        }

        public List<IAdapterResponseInfoClient> GetAdapterResponses()
        {
            return new List<IAdapterResponseInfoClient>
            {
                new AdapterResponseInfoClient()
            };
        }

        public Dictionary<string, string> GetResponseExtras()
        {
            return new Dictionary<string, string>
            {
                {"Placeholder Key", "Placeholder Value"}
            };
        }
    }
}
