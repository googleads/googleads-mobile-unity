// Copyright (C) 2017 Google, Inc.
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

#if UNITY_IPHONE || UNITY_IOS

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

namespace GoogleMobileAds.Common.Mediation.AppLovin
{
    public class AppLovinPostProcessBuild
    {
        [PostProcessBuild]
        public static void ChangeXcodePlist(BuildTarget buildTarget,
                                            string pathToBuiltProject)
        {
            string plistPath = pathToBuiltProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            PlistElementDict rootDict = plist.root;

            rootDict.SetString("AppLovinSdkKey",
                               "INSERT_APP_LOVIN_SDK_KEY_HERE");
            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}

#endif
