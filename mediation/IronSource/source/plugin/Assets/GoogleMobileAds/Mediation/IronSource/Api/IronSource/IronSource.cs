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

using GoogleMobileAds.Mediation.IronSource;
using GoogleMobileAds.Mediation.IronSource.Common;

namespace GoogleMobileAds.Mediation.IronSource.Api
{
    public class IronSource
    {
        internal static readonly IIronSourceClient client =
                IronSourceClientFactory.CreateIronSourceClient();

        public static void SetConsent(bool consent)
        {
            client.SetConsent(consent);
        }

        public static void SetMetaData(string key, string metaDataValue)
        {
            client.SetMetaData(key, metaDataValue);
        }
    }
}

namespace GoogleMobileAds.Api.Mediation.IronSource
{
    [System.Obsolete("Use `GoogleMobileAds.Mediation.IronSource.Api.IronSource` instead.")]
    public class IronSource
    {
        public static void SetConsent(bool consent)
        {
            GoogleMobileAds.Mediation.IronSource.Api.IronSource.SetConsent(consent);
        }

        public static void SetMetaData(string key, string metaDataValue)
        {
            GoogleMobileAds.Mediation.IronSource.Api.IronSource.SetMetaData(key, metaDataValue);
        }
    }
}
