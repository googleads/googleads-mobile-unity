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
using UnityEngine;

namespace GoogleMobileAds.Api
{

  /// <summary>
  /// Defines ID constants for templates to render native ads.
  /// </summary>
  public class NativeTemplateId
  {
    /// <summary>
    /// Small Layout.
    /// </summary>
    public const string Small = "small";

    /// <summary>
    /// Medium Layout.
    /// </summary>
    public const string Medium = "medium";
  }

  /// <summary>
  /// Style options for native templates.
  /// </summary>
  public class NativeTemplateStyle
  {
    /// <summary>
    /// An identifier representing the native template to render.
    /// </summary>
    public string TemplateId;

    /// <summary>
    // The background color.
    /// </summary>
    public Color MainBackgroundColor;

    /// <summary>
    // The NativeTemplateTextStyle for the primary text.
    /// </summary>
    public NativeTemplateTextStyle PrimaryText;

    /// <summary>
    /// The NativeTemplateTextStyle for the second row of text in the template.
    /// <summary>
    public NativeTemplateTextStyle SecondaryText;

    /// <summary>
    /// The NativeTemplateTextStyle for the third row of text in the template.
    /// <summary>
    public NativeTemplateTextStyle TertiaryText;

    /// <summary>
    /// The NativeTemplateTextStyle for the call to action.
    /// <summary>
    public NativeTemplateTextStyle CallToActionText;

    public NativeTemplateStyle()
    {
      // Default to using the small template.
      TemplateId = NativeTemplateId.Small;
    }

    public NativeTemplateStyle(NativeTemplateStyle templateStyle)
    {
      TemplateId = templateStyle.TemplateId;
      MainBackgroundColor = templateStyle.MainBackgroundColor;
      PrimaryText = templateStyle.PrimaryText;
      SecondaryText = templateStyle.SecondaryText;
      TertiaryText = templateStyle.TertiaryText;
      CallToActionText = templateStyle.CallToActionText;
    }
  }
}