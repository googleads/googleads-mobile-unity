// Copyright (C) 2020 Google LLC
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

namespace GoogleMobileAds.Api {
    public class LoadAdError : AdError
    {
       ILoadAdErrorClient client;

       public LoadAdError(ILoadAdErrorClient client) : base(client)
       {
          this.client = client;
       }

      /// <summary>
      /// Gets ResponseInfo Object for the failed request.
      /// See https://developers.google.com/admob/unity/response-info
      /// for more inforomation about Response Info.
      /// <summary>
      /// <returns>ResponseInfo Object</returns>
       public ResponseInfo GetResponseInfo()
       {
          return new ResponseInfo(client.GetResponseInfoClient());
       }

       public override string ToString()
       {
            return client.ToString();
        }
    }
}
