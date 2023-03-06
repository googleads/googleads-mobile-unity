// Copyright 2022 Google LLC
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

using GoogleMobileAds.Mediation.Pangle;
using GoogleMobileAds.Mediation.Pangle.Common;

namespace GoogleMobileAds.Mediation.Pangle.Api
{
    public class Pangle
    {
        public static readonly IPangleClient client = GetPangleClient();

        public static void SetGDPRConsent(int gdpr)
        {
            client.SetGDPRConsent(gdpr);
        }

        public static void SetDoNotSell(int doNotSell)
        {
            client.SetDoNotSell(doNotSell);
        }

        internal static IPangleClient GetPangleClient()
        {
            return PangleClientFactory.PangleInstance();
        }
    }
}

namespace GoogleMobileAds.Api.Mediation.Pangle
{
    [System.Obsolete("Use `GoogleMobileAds.Mediation.Pangle.Api.Pangle` instead.")]
    public class Pangle
    {
        public static void SetGDPRConsent(int gdpr)
        {
            GoogleMobileAds.Mediation.Pangle.Api.Pangle.SetGDPRConsent(gdpr);
        }

        public static void SetDoNotSell(int doNotSell)
        {
            GoogleMobileAds.Mediation.Pangle.Api.Pangle.SetDoNotSell(doNotSell);
        }

        private static IPangleClient GetPangleClient()
        {
            return GoogleMobileAds.Mediation.Pangle.Api.Pangle.GetPangleClient();
        }
    }
}
