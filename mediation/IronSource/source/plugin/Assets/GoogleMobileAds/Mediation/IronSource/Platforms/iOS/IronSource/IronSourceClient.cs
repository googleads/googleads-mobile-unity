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

using GoogleMobileAds.Mediation.IronSource.Common;

namespace GoogleMobileAds.Mediation.IronSource.iOS
{
    public class IronSourceClient : IIronSourceClient
    {
        private static IronSourceClient instance = new IronSourceClient();
        private IronSourceClient() { }

        public static IronSourceClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void SetConsent(bool consent)
        {
            Externs.GADUMIronSourceSetConsent(consent);
        }

        public void SetMetaData(string key, string metaDataValue)
        {
            Externs.GADUMIronSourceSetMetaData(key, metaDataValue);
        }
    }
}

#endif
