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

#if UNITY_ANDROID

using UnityEngine;

using GoogleMobileAds.Common.Mediation.Pangle;

namespace GoogleMobileAds.Android.Mediation.Pangle
{
    public class PangleClient : IPangleClient
    {
        private static PangleClient instance = new PangleClient();
        private PangleClient() { }

        private const string pangleMediationAdapterClassName =
            "com.google.ads.mediation.pangle.PangleMediationAdapter";

        public static PangleClient Instance
        {
            get { return instance; }
        }

        public void SetGDPR(int gdpr)
        {
            AndroidJavaClass PangleMediationAdapter =
                new AndroidJavaClass(pangleMediationAdapterClassName);
            PangleMediationAdapter.CallStatic("setGdpr", gdpr);
        }

        public void SetCCPA(int ccpa)
        {
            AndroidJavaClass PangleMediationAdapter =
                new AndroidJavaClass(pangleMediationAdapterClassName);
            PangleMediationAdapter.CallStatic("setCcpa", ccpa);
        }
    }
}

#endif
