// Copyright (C) 2022 Google LLC.
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

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Unity
{
    public class AdError : IAdErrorClient, ILoadAdErrorClient
    {
        public int Code { get { return -1; } }
        public string Domain { get { return "Google Mobile Ads"; } }
        public string Message { get { return "Prefab Ad is Null"; } }
        public string Description { get { return Domain + " " + Message; } }
        public IAdErrorClient Cause { get { return null; } }
        public IResponseInfoClient Response { get { return null; } }
    }
}
