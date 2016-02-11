using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Callbacks;
using UnityEditor;

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

            if(target == iOSBuildTarget)
            {
                runPodUpdate(pathToBuiltProject);
            }
        }

        static void runPodUpdate(string path)
        {
            #if !UNITY_CLOUD_BUILD
                // Copy the podfile into the project.
                string podfile = "Assets/GoogleMobileAds/Editor/Podfile";
                string destpodfile = path + "/Podfile";
                if(!System.IO.File.Exists(destpodfile))
                {
                    FileUtil.CopyFileOrDirectory(podfile, destpodfile);
                }

                try
                {
                    CocoaPodHelper.Update(path);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.Log("Could not create a new Xcode project with CocoaPods: " +
                            e.Message);
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
