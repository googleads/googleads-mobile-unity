// Copyright (C) 2019 Google LLC
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

namespace GoogleMobileAds.Placement
{
    public class GoogleMobileAdsPlacements : ScriptableObject
    {
        [HideInInspector]
        [SerializeField]
        public List<AdPlacement> allPlacements;

        [SerializeField]
        private static GoogleMobileAdsPlacements instance;

        public static GoogleMobileAdsPlacements Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.LoadAll<GoogleMobileAdsPlacements>("")[0];
                }
                return instance;
            }
        }
    }

    [System.Serializable]
    public class AdPlacement
    {
        public string placementName = "";
        public enum AdType
        {
            Banner = 0,
            Interstitial = 1,
            Rewarded = 2,
            RewardedInterstitial = 3,
        }
        public AdType adType;
        public string androidAdUnitId;
        public string iOSAdUnitId;
        public bool persistent;
        public bool autoLoadEnabled;

        public string AdUnitId
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    return this.androidAdUnitId;
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    return this.iOSAdUnitId;
                }
                else
                {
                    return "unexpected_platform";
                }
            }
        }
    }
}
