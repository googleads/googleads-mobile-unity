// Copyright 2018 Google LLC
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

using GoogleMobileAds.Common.Mediation.MoPub;
using GoogleMobileAds.Mediation;

namespace GoogleMobileAds.Api.Mediation.MoPub
{
    public class MoPub
    {
        public static readonly IMoPubClient client = GetMoPubClient();

        private static IMoPubClient GetMoPubClient()
        {
            return MoPubClientFactory.MoPubInstance();
        }

        public static void Initialize(string moPubAdUnitID)
        {
            if (string.IsNullOrEmpty(moPubAdUnitID))
            {
                MonoBehaviour.print("Unable to Initialize MoPub: " +
                                    "Ad Unit ID is null");
                return;
            }

            client.Initialize(moPubAdUnitID);
        }

        public static bool IsInitialized()
        {
            return client.IsInitialized();
        }
    }
}
