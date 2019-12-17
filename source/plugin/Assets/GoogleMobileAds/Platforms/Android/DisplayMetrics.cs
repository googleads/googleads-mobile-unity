// Copyright (C) 2019 Google LLC.
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
using System;
using System.Collections.Generic;

namespace GoogleMobileAds.Android
{
    public class DisplayMetrics {
        // The logical density of the display.
        public static float Density { get; protected set; }

        // The absolute height of the display in pixels
        public static int HeightPixels { get; protected set; }

        // The absolute width of the display in pixels
        public static int WidthPixels { get; protected set; }

        static DisplayMetrics() {
            using (
                AndroidJavaClass unityPlayerClass = new AndroidJavaClass(Utils.UnityActivityClassName),
                metricsClass = new AndroidJavaClass(Utils.DisplayMetricsClassName)
            )
            {
                using (
                    AndroidJavaObject metricsInstance = new AndroidJavaObject(Utils.DisplayMetricsClassName),
                    activityInstance = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"),
                    windowManagerInstance = activityInstance.Call<AndroidJavaObject>("getWindowManager"),
                    displayInstance = windowManagerInstance.Call<AndroidJavaObject>("getDefaultDisplay")
                )
                {
                    displayInstance.Call("getMetrics", metricsInstance);
                    Density = metricsInstance.Get<float>("density");
                    HeightPixels = metricsInstance.Get<int>("heightPixels");
                    WidthPixels = metricsInstance.Get<int>("widthPixels");
                }
            }
        }
    }
}

#endif
