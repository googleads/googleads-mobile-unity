// <copyright file="SampleDependencies.cs" company="Google Inc.">
// Copyright (C) 2015 Google Inc. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>

using Google.JarResolver;
using UnityEditor;

/// <summary>
/// Sample dependencies file.  Copy this to a different name specific to your
/// plugin and add the Google Play Services and Android Support components that
/// your plugin depends on.
/// </summary>
[InitializeOnLoad]
public static class SampleDependencies
{
    /// <summary>
    /// The name of your plugin.  This is used to create a settings file
    /// which contains the dependencies specific to your plugin.
    /// </summary>

/*
    private static readonly string PluginName = "your_plugin_name";

    /// <summary>
    /// Initializes static members of the <see cref="SampleDependencies"/> class.
    /// </summary>
    static SampleDependencies()
    {

        PlayServicesSupport svcSupport = PlayServicesSupport.CreateInstance(
                                             PluginName,
                                             EditorPrefs.GetString("AndroidSdkRoot"),
                                             "ProjectSettings");

        // add your dependencies here

        // svcSupport.DependOn("com.google.android.gms", "play-services-base", "8.1+");
    }
*/
}
