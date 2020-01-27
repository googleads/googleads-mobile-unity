// Copyright 2019 Google LLC
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

using GoogleMobileAds.Common.Mediation.VerizonMedia;

namespace GoogleMobileAds.Mediation
{
    public class VerizonMediaClientFactory
    {
        public static IVerizonMediaClient CreateVerizonMediaClient()
        {
            #if UNITY_EDITOR
            return new GoogleMobileAds.Common.Mediation.VerizonMedia.DummyClient();
            #elif UNITY_ANDROID
            return GoogleMobileAds.Android.Mediation.VerizonMedia.VerizonMediaClient.Instance;
            #elif UNITY_IPHONE || UNITY_IOS
            return GoogleMobileAds.iOS.Mediation.VerizonMedia.VerizonMediaClient.Instance;
            #else
            return new GoogleMobileAds.Common.Mediation.VerizonMedia.DummyClient();
            #endif
        }
    }
}
