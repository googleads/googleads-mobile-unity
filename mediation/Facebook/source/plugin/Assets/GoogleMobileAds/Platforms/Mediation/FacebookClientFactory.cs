// Copyright 2021 Google LLC
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

using GoogleMobileAds.Common.Mediation.Facebook;

namespace GoogleMobileAds.Mediation
{
    public class FacebookClientFactory
    {
        public static IFacebookClient FacebookInstance()
        {
            #if UNITY_EDITOR
            return new GoogleMobileAds.Common.Mediation.Facebook.DummyClient();
            #elif UNITY_ANDROID
            return GoogleMobileAds.Android.Mediation.Facebook.FacebookClient.Instance;
            #elif UNITY_IPHONE || UNITY_IOS
            return GoogleMobileAds.iOS.Mediation.Facebook.FacebookClient.Instance;
            #else
            return new GoogleMobileAds.Common.Mediation.Facebook.DummyClient();
            #endif
        }
    }
}
