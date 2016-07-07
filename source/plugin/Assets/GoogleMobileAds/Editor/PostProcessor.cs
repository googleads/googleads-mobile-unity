using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;

#if (UNITY_5 && UNITY_IOS)
    using UnityEditor.iOS.Xcode;
#endif

namespace GoogleMobileAds
{
    public class Postprocessor
    {
        [PostProcessBuildAttribute(1)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            BuildTarget iOSBuildTarget;
            #if UNITY_5
                iOSBuildTarget = BuildTarget.iOS;
            #else
                iOSBuildTarget = BuildTarget.iPhone;
            #endif

            if (target == iOSBuildTarget)
            {
                RunPodUpdate(pathToBuiltProject);
            }
        }

        public static void RunPodUpdate(string path)
        {
            #if !UNITY_CLOUD_BUILD
                // Copy the podfile into the project.
                string podfile = "Assets/GoogleMobileAds/Editor/Podfile";
                string destPodfile = path + "/Podfile";

                if (!System.IO.File.Exists(podfile))
                {
                    UnityEngine.Debug.LogWarning(@"Could not locate Podfile in
                            Assets/GoogleMobileAds/Editor/");
                    return;
                }

                if (!System.IO.File.Exists(destPodfile))
                {
                    FileUtil.CopyFileOrDirectory(podfile, destPodfile);
                }
                else
                {
                    FileUtil.ReplaceFile(podfile, destPodfile);
                }

                try
                {
                    CocoaPodHelper.Update(path);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogWarning("Could not create a new Xcode project with " +
                            "CocoaPods: " + e.Message);
                }
            #endif

            #if (UNITY_5 && UNITY_IOS)
                string pbxprojPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
                PBXProject project = new PBXProject();
                project.ReadFromString(File.ReadAllText(pbxprojPath));
                string target = project.TargetGuidByName("Unity-iPhone");

                project.SetBuildProperty(target, "CLANG_ENABLE_MODULES", "YES");
                project.AddBuildProperty(target, "OTHER_LDFLAGS", "$(inherited)");

                File.WriteAllText(pbxprojPath, project.WriteToString());
            #else
                UnityEngine.Debug.Log("Unable to modify build settings in XCode project. Build " +
                        "settings must be set manually");
            #endif
        }
    }
}
