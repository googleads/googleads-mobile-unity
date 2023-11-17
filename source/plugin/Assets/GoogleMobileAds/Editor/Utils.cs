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

/*
 * Utils class that contains helper methods.
 */
public static class Utils
{
    /// <summary>
    /// Path for the Gradle template files.
    /// </summary>
    internal static string GradleTemplatePath =
            Path.Combine(AndroidPluginsDir, "baseProjectTemplate.gradle");

    //  Android library plugins directory that contains custom gradle templates.
    internal const string AndroidPluginsDir = "Assets/Plugins/Android";

    // Extracts an Android Gradle Plugin version number from the contents of a *.gradle file.
    // This should work for Unity 2022.1 and below.
    // Ex.
    //   classpath 'com.android.tools.build:gradle:4.0.1'
    private static Regex androidGradlePluginVersionExtract_legacy = new Regex(
        @"^\s*classpath\s+['""]com\.android\.tools\.build:gradle:([^'""]+)['""]$");

    // Extracts an Android Gradle Plugin version number from the contents of a *.gradle file for
    // Unity 2022.2+ or 2023.1+.
    // Ex.
    //   id 'com.android.application' version '7.1.2' apply false
    private static Regex androidGradlePluginVersionExtract = new Regex(
        @"^\s*id\s+['""]com\.android\.application['""] version ['""]([^'""]+)['""]");

    /// <summary>
    /// Get the Android Gradle Plugin version used by the Unity project.
    /// </summary>
    public static string AndroidGradlePluginVersion {
        private set {}
        get {
            if (!Directory.Exists(AndroidPluginsDir) || !File.Exists(GradleTemplatePath)) {
                return DefaultAndroidGradlePlugin();
            }
            var gradleTemplates = Directory.GetFiles(AndroidPluginsDir, "*.gradle",
                                                        SearchOption.TopDirectoryOnly);
            foreach (var path in gradleTemplates) {
                foreach (var line in File.ReadAllLines(path)) {
                    var match = androidGradlePluginVersionExtract_legacy.Match(line);
                    if (match != null && match.Success) {
                        return match.Result("$1");
                    }
                    match = androidGradlePluginVersionExtract.Match(line);
                    if (match != null && match.Success) {
                        return  match.Result("$1");
                    }
                }
            }
            // Fallback to the gradle templates in Unity installation folder with EDM4U.
            return DefaultAndroidGradlePlugin();
        }
    }

    private static string DefaultAndroidGradlePlugin()
    {
#if UNITY_2022_3_OR_NEWER
        return "7.1.2";
#else
        return "4.0.1";
#endif
    }
}
