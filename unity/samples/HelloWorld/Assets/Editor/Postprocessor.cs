// C# example:
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using UnityEditor.iOS.Xcode;
using System.IO;

public class Postprocessor
{
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target == BuildTarget.iOS)
        {
            _AddDeviceCapabilities(pathToBuiltProject);
        }
    }

    static void _AddDeviceCapabilities(string path)
    {
        string pbxprojPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
        
        PBXProject project = new PBXProject();
        project.ReadFromString(File.ReadAllText(pbxprojPath));
        string target = project.TargetGuidByName("Unity-iPhone");

        project.AddFrameworkToProject(target, "AdSupport.framework", false);
        project.AddFrameworkToProject(target, "AudioToolbox.framework", false);
        project.AddFrameworkToProject(target, "AVFoundation.framework", false);
        project.AddFrameworkToProject(target, "CoreGraphics.framework", false);
        project.AddFrameworkToProject(target, "CoreTelephony.framework", false);
        project.AddFrameworkToProject(target, "EventKit.framework", false);
        project.AddFrameworkToProject(target, "EventKitUI.framework", false);
        project.AddFrameworkToProject(target, "MessageUI.framework", false);
        project.AddFrameworkToProject(target, "StoreKit.framework", false);
        project.AddFrameworkToProject(target, "SystemConfiguration.framework", false);

        project.SetBuildProperty(target, "CLANG_ENABLE_MODULES", "YES");

        File.WriteAllText(pbxprojPath, project.WriteToString());

        string infoPlistPath = Path.Combine(path, "./Info.plist");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(infoPlistPath));

        PlistElementDict rootDict = plist.root;
        PlistElementArray deviceCapabilityArray = rootDict.CreateArray("UIRequiredDeviceCapabilities");
        deviceCapabilityArray.AddString("armv7");
        deviceCapabilityArray.AddString("gamekit"); 

        rootDict.SetBoolean("UIRequiresFullScreen", true);

        File.WriteAllText(infoPlistPath, plist.WriteToString());
    }
}