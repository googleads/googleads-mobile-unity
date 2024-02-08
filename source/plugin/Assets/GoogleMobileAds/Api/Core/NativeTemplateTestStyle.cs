// Copyright (C) 2024 Google LLC
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

namespace GoogleMobileAds.Api {

  /// <summary>
  /// Font Types for native templates.
  /// </summary>
  public enum NativeTemplateFontStyle
  {
    /// <summary>
    /// Default text.
    /// </summary>
    Normal = 0,
    /// <summary>
    /// Bold.
    /// </summary>
    Bold = 1,
    /// <summary>
    /// Italic.
    /// </summary>
    Italic = 2,
    /// <summary>
    /// Monospace.
    /// </summary>
    Monospace = 3
  }

  /// <summary>
  /// Text style options for native templates.
  /// </summary>
  [Serializable]
  public class NativeTemplateTextStyle
  {
    /// Background color
    public Color BackgroundColor { get; set; }

    /// Color of the Text to be rendered.
    public Color TextColor { get; set; }

    /// Size of the Text to be displayed.
    public int FontSize { get; set; }

    /// FontStyle for the text.
    public NativeTemplateFontStyle Style { get; set; }

    public NativeTemplateTextStyle() {}
  }
}
