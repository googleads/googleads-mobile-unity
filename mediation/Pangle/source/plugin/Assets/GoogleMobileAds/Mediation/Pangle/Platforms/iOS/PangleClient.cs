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

#if UNITY_IOS

using UnityEngine;

using GoogleMobileAds.Mediation.Pangle.Common;

namespace GoogleMobileAds.Mediation.Pangle.iOS
{
    public class PangleClient : IPangleClient
    {
        private static PangleClient instance = new PangleClient();
        private PangleClient() { }

        public static PangleClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void SetGDPRConsent(int gdpr)
        {
            Externs.GADUMPangleSetGDPRConsent(gdpr);
        }

        public void SetDoNotSell(int doNotSell)
        {
            Externs.GADUMPangleSetDoNotSell(doNotSell);
        }
    }
}

#endif
