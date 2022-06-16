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

namespace GoogleMobileAds.Common
{
    public class AppStateEventClient : MonoBehaviour, IAppStateEventClient
    {
        private static AppStateEventClient instance;

        public static AppStateEventClient Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject gameObject = new GameObject("AppStateEventClient");
                    GameObject.DontDestroyOnLoad(gameObject);
                    instance = gameObject.AddComponent<AppStateEventClient>();
                }
                return instance;
            }
        }

        public event Action<AppState> AppStateChanged = delegate {};

        private void OnApplicationPause(bool isPaused)
        {
            AppStateChanged(isPaused ? AppState.Background : AppState.Foreground);
        }
    }
}
