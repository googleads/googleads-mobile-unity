// Copyright (C) 2023 Google LLC
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
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace GoogleMobileAds.Editor
{
    /*
    * Utils class that contains helper methods.
    */
    public static class Utils
    {
        //  Android library plugins directory that contains custom gradle templates.
        internal const string AndroidPluginsDir = "Assets/Plugins/Android";

        // Extracts an Android Gradle Plugin version number from the contents of a *.gradle file.
        // This should work for Unity 2022.1 and below.
        // Ex.
        //   classpath 'com.android.tools.build:gradle:4.0.1'
        private static Regex androidGradlePluginVersionExtract_legacy =
            new Regex(@"^\s*classpath\s+['""]com\.android\.tools\.build:gradle:([^'""]+)['""]$");

        // Extracts an Android Gradle Plugin version number from the contents of a *.gradle file for
        // Unity 2022.2+ or 2023.1+.
        // Ex.
        //   id 'com.android.application' version '7.1.2' apply false
        private static Regex androidGradlePluginVersionExtract =
            new Regex(@"^\s*id\s+['""]com\.android\.application['""] version ['""]([^'""]+)['""]");

        // Extracts major.minor[.patch] version numbers from a string.
        private static readonly Regex versionParseRegex =
            new Regex(@"^(\d+)\.(\d+)(?:\.(\d+))?", RegexOptions.Compiled);

        /// <summary>
        /// Get the Android Gradle Plugin version used by the Unity project.
        /// </summary>
        public static Version AndroidGradlePluginVersion
        {
            get
            {
                return ParseVersion(GetAndroidGradlePluginVersionString());
            }
        }

        private static string GetAndroidGradlePluginVersionString()
        {
            if (!Directory.Exists(AndroidPluginsDir))
            {
                return DefaultAndroidGradlePlugin();
            }
            var gradleTemplates = Directory.GetFiles(AndroidPluginsDir, "*.gradle",
                                                     SearchOption.TopDirectoryOnly);
            foreach (var path in gradleTemplates)
            {
                foreach (var line in File.ReadLines(path))
                {
                    var match = androidGradlePluginVersionExtract_legacy.Match(line);
                    if (match != null && match.Success)
                    {
                        return match.Result("$1");
                    }
                    match = androidGradlePluginVersionExtract.Match(line);
                    if (match != null && match.Success)
                    {
                        return match.Result("$1");
                    }
                }
            }
            return DefaultAndroidGradlePlugin();
        }

        private static Version ParseVersion(string versionStr)
        {
            var match = versionParseRegex.Match(versionStr);
            if (match.Success)
            {
                int major = int.Parse(match.Groups[1].Value);
                int minor = int.Parse(match.Groups[2].Value);
                int patch = match.Groups[3].Success ? int.Parse(match.Groups[3].Value) : 0;
                return new Version(major, minor, patch);
            }
            return new Version(0, 0, 0);
        }

        // These values are based on the Unity documentation for the latest version of Unity. See
        // https://docs.unity3d.com/2023.2/Documentation/Manual/android-gradle-overview.html
        // https://docs.unity3d.com/6000.0/Documentation/Manual/android-gradle-version-compatibility.html
        private static string DefaultAndroidGradlePlugin()
        {
#if UNITY_6000_0_OR_NEWER
            return "8.3.0";
#elif UNITY_2023_2_OR_NEWER
            return "7.3.1";
#elif UNITY_2022_2_OR_NEWER
            return "7.1.2";
#else
            return "4.0.1";
#endif
        }
    }
}