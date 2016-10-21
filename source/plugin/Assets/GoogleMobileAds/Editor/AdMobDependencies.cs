// Copyright (C) 2016 Google, Inc.
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
using UnityEditor;

/// AdMob dependencies file.
[InitializeOnLoad]
public class AdMobDependencies : AssetPostprocessor
{
    /// Initializes static members of the class.
    static AdMobDependencies() { SetupDeps(); }

    static void SetupDeps() {
#if UNITY_ANDROID
        // Setup the resolver using reflection as the module may not be
        // available at compile time.
        Type playServicesSupport = Google.VersionHandler.FindClass(
            "Google.JarResolver", "Google.JarResolver.PlayServicesSupport");
        if (playServicesSupport == null) {
            return;
        }
        object svcSupport = Google.VersionHandler.InvokeStaticMethod(
            playServicesSupport, "CreateInstance",
            new object[] {
                "AdMobUnity",
                EditorPrefs.GetString("AndroidSdkRoot"),
                "ProjectSettings"
            });

        Google.VersionHandler.InvokeInstanceMethod(
            svcSupport, "DependOn",
            new object[] { "com.google.android.gms", "play-services-ads",
                           "LATEST" },
            namedArgs: new Dictionary<string, object>() {
                {"packageIds", new string[] {
                        "extra-google-m2repository",
                        "extra-android-m2repository"} }
            });
#elif UNITY_IOS
        Type iosResolver = Google.VersionHandler.FindClass(
            "Google.IOSResolver", "Google.IOSResolver");
        if (iosResolver == null) {
            return;
        }
        Google.VersionHandler.InvokeStaticMethod(
            iosResolver, "AddPod",
            new object[] { "Google-Mobile-Ads-SDK" },
            namedArgs: new Dictionary<string, object>() {
                { "version", "7.13+" }
            });
#endif  // UNITY_IOS
    }

    // Handle delayed loading of the dependency resolvers.
    private static void OnPostprocessAllAssets(
            string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromPath) {
        foreach (string asset in importedAssets) {
            if (asset.Contains("IOSResolver") ||
                asset.Contains("JarResolver")) {
                SetupDeps();
                break;
            }
        }
    }
}

