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

using GoogleMobileAds.Common;
using System.Collections.Generic;
using System.Linq;

namespace GoogleMobileAds.Api
{
    /// <summary>
    /// Information about an ad response.
    /// </summary>
    public class ResponseInfo
    {
        private IResponseInfoClient client;

        public ResponseInfo(IResponseInfoClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Returns the AdapterResponseInfo of the adapter that loaded the ad.
        /// Returns null if the ad failed to load.
        /// </summary>
        public AdapterResponseInfo GetLoadedAdapterResponseInfo()
        {
            return new AdapterResponseInfo(client.GetLoadedAdapterResponseInfo());
        }

        /// <summary>
        /// Returns the list of AdapterResponseInfo containing metadata for each adapter included
        /// in the ad response. Can be used to debug the mediation waterfall execution.
        /// </summary>
        public AdapterResponseInfo[] GetAdapterResponses()
        {
            var responses = client.GetAdapterResponses();
            if (responses == null || responses.Length == 0)
            {
                return new AdapterResponseInfo[0];
            }
            return responses.Select(o => new AdapterResponseInfo(o)).ToArray();
        }

        /// <summary>
        /// Returns extra information about the ad response.
        /// </summary>
        public Dictionary<string, string> GetResponseExtras()
        {
            return client.GetResponseExtras();
        }

        /// <summary>
        /// Returns the mediation adapter class name of the ad network that loaded the ad.
        ///
        /// In the case of a mediated ad response, this is the name of the class that was
        /// responsible for performing the ad request and rendering the ad.
        ///
        /// See https://developers.google.com/admob/unity/response-info for possible return values.
        /// Returns null if the ad failed to load.
        /// </summary>
        public string GetMediationAdapterClassName()
        {
            return client.GetMediationAdapterClassName();
        }

        /// <summary>
        /// Returns the response ID for the loaded ad.
        /// Can be used to look up ads in the Ad Review Center.
        /// Returns null if the ad failed to load.
        /// </summary>
        public string GetResponseId()
        {
            return client.GetResponseId();
        }

        /// <summary>
        /// Returns a log friendly string version of this object.
        /// </summary>
        public override string ToString()
        {
            return client.ToString();
        }
    }
}
