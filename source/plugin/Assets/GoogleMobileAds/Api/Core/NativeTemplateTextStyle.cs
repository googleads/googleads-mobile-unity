// <copyright file="NativeTemplateTextStyle.cs" company="Google LLC">
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
// </copyright>

namespace GoogleMobileAds.Api
{
    using System;
    using UnityEngine;

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
        /// Bold text.
        /// </summary>
        Bold = 1,

        /// <summary>
        /// Italic text.
        /// </summary>
        Italic = 2,

        /// <summary>
        /// Monospace text.
        /// </summary>
        Monospace = 3
    }

    /// <summary>
    /// Text style options for native templates.
    /// </summary>
    [Serializable]
    public class NativeTemplateTextStyle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NativeTemplateTextStyle"/> class.
        /// </summary>
        public NativeTemplateTextStyle()
        {
        }

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the Text to be rendered.
        /// </summary>
        public Color TextColor { get; set; }

        /// <summary>
        /// Gets or sets the size of the Text to be displayed.
        /// </summary>
        public int FontSize { get; set; }

        /// <summary>
        /// Gets or sets the FontStyle for the text.
        /// </summary>
        public NativeTemplateFontStyle Style { get; set; }
    }
}
