// Copyright (C) 2022 Google LLC
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
using GoogleMobileAds.Common;
using UnityEngine;

namespace GoogleMobileAds.Api
{
    /// <summary>
    /// Notifies changes in app foreground/background.
    /// Subscribe to <see cref="AppStateEventNotifier.AppStateChanged"/>
    /// to get notified of changes in <see cref="AppState"/>.
    /// </summary>
    public class AppStateEventNotifier
    {
        /// <summary>
        /// Raised when the app enters the background/foreground.
        /// </summary>
        public static event Action<AppState> AppStateChanged
        {
            add
            {
                Client.AppStateChanged += value;
            }
            remove
            {
                Client.AppStateChanged -= value;
            }
        }

        private static IAppStateEventClient client;

        private static IAppStateEventClient Client
        {
            get
            {
                // In the Unity Editor the client is a MonoBehaviour that is destroyed when play
                // mode exits. With "Enter Play Mode Options" (domain reload disabled) this static
                // survives into the next play session, so re-create the client if it is gone.
                if (client == null ||
                    (client is UnityEngine.Object && (UnityEngine.Object)client == null))
                {
                    client = MobileAds.GetClientFactory().BuildAppStateEventClient();
                }
                return client;
            }
        }

        // Resets static state at the start of every play session, so behavior is the same
        // whether or not domain reload is enabled.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticState()
        {
            client = null;
        }
    }
}
