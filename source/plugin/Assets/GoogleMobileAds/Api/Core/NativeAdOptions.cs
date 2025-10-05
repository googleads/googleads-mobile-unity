// Copyright (C) 2024 Google, Inc.
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
  /// Ad options for configuring the view of native ads.
  /// </summary>
  [Serializable]
  public class NativeAdOptions
  {
    /// <summary>
    /// Image and video aspect ratios. Default is Unknown.
    /// </summary>
    public MediaAspectRatio MediaAspectRatio;

    /// <summary>
    /// Sets the placement of AdChoices icon (if present). Default is TopRightCorner.
    /// </summary>
    public AdChoicesPlacement AdChoicesPlacement;

    /// <summary>
    /// Sets the VideoOptions object.
    /// </summary>
    public VideoOptions VideoOptions;

    /// <summary>
    /// Create a new default NativeAdOptions object.
    /// </summary>
    public NativeAdOptions()
    {
      VideoOptions = new VideoOptions();
    }

    /// <summary>
    /// Create a new NativeAdOptions object.
    /// </summary>
    public NativeAdOptions(NativeAdOptions options)
    {
      MediaAspectRatio = options.MediaAspectRatio;
      AdChoicesPlacement = options.AdChoicesPlacement;
      VideoOptions = options.VideoOptions;
    }
  }
}
