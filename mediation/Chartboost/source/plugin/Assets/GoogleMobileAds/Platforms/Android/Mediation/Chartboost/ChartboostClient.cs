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

#if UNITY_ANDROID

using UnityEngine;
using System.Reflection;

using GoogleMobileAds.Common.Mediation.Chartboost;

namespace GoogleMobileAds.Android.Mediation.Chartboost
{
    public class ChartboostClient : IChartboostClient
    {
        private static ChartboostClient instance = new ChartboostClient();
        private ChartboostClient() {}

        public static ChartboostClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void RestrictDataCollection(bool shouldRestrict)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
            AndroidJavaClass chartboost = new AndroidJavaClass ("com.chartboost.sdk.Chartboost");

            string parameterString = (shouldRestrict == true ? "true" : "false");
            MonoBehaviour.print ("Calling 'Chartboost.restrictDataCollection()' with argument: " + parameterString);
            chartboost.CallStatic ("restrictDataCollection", currentActivity, shouldRestrict);
        }
    }
}

#endif
