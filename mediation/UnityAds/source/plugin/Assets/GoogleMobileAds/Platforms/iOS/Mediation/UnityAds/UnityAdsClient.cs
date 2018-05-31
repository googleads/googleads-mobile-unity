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

#if UNITY_IOS

using UnityEngine;

using GoogleMobileAds.Common.Mediation.UnityAds;

namespace GoogleMobileAds.iOS.Mediation.UnityAds
{
    public class UnityAdsClient : IUnityAdsClient
    {
        private static UnityAdsClient instance = new UnityAdsClient();
        private UnityAdsClient() {}

        public static UnityAdsClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void SetGDPRConsentMetaData(bool consent)
        {
            string parameterString = (consent == true ? "@YES" : "@NO");
            MonoBehaviour.print ("Setting UnityAds metadata with key 'gdpr.consent' to value: " + parameterString);
            Externs.GADUMSetGDPRConsentMetaData (consent);
        }
    }
}

#endif
