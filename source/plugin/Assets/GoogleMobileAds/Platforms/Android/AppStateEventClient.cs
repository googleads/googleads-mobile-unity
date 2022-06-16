// Copyright (C) 2022 Google LLC.
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

using System;
using UnityEngine;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Android
{
    public class AppStateEventClient : AndroidJavaProxy, IAppStateEventClient
    {
        private event Action<AppState> appStateChanged;
        private AndroidJavaObject appStateEventNotifer;

        public event Action<AppState> AppStateChanged
        {
            add
            {
                if (value == null)
                {
                    return;
                }
                if (appStateChanged == null)
                {
                    appStateEventNotifer.Call("startListening");
                }
                appStateChanged += value;
            }
            remove
            {
                appStateChanged -= value;
                if (appStateChanged == null)
                {
                    appStateEventNotifer.Call("stopListening");
                }
            }
        }

        public AppStateEventClient() : base(Utils.UnityAppStateEventCallbackClassName)
        {
            var playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            var activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            appStateEventNotifer =
                  new AndroidJavaObject(Utils.UnityAppStateEventNotifierClassName, activity, this);
        }

        private void onAppStateChanged(bool isBackground)
        {
            if (appStateChanged != null)
            {
                appStateChanged(isBackground ? AppState.Background : AppState.Foreground);
            }
        }
    }
}
