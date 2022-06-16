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

namespace GoogleMobileAds.Common
{
    /// <summary>
    /// The app foreground/background state.
    /// </summary>
    public enum AppState
    {
        /// <summary>
        /// The app is backgrounded.
        /// </summary>
        Background = 0,
        /// <summary>
        /// The app is foregrounded.
        /// </summary>
        Foreground = 1
    }

    /// <summary>
    /// Notifies changes in app foreground/background.
    /// </summary>
    public interface IAppStateEventClient
    {
        /// <summary>
        /// Raised when the app enters the background/foreground.
        /// </summary>
        event Action<AppState> AppStateChanged;
    }
}
