// Copyright (C) 2023 Google, Inc.
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

namespace GoogleMobileAds.Api
{
  /// <summary>
  /// Options for controlling video playback in supported ad formats.
  /// </summary>
  [Serializable]
  public class VideoOptions
  {
    /// <summary>
    /// Indicates whether the requested video should have the click to expand behavior.
    /// </summary>
    public bool ClickToExpandRequested;

    /// <summary>
    /// Indicates whether the requested video should have custom controls enabled for
    /// play/pause/mute/unmute.
    /// </summary>
    public bool CustomControlsRequested;

    /// <summary>
    /// Indicates whether videos should start muted.
    /// </summary>
    public bool StartMuted;

    /// <summary>
    /// Create a new VideoOptions object.
    /// </summary>
    public VideoOptions()
    {
      StartMuted = true;
    }
  }
}
