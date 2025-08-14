using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;

namespace GoogleMobileAds.Editor {
  public class BuildPostProcessor : IPostprocessBuildWithReport {
    public int callbackOrder {
      get { return 1; }
    }

    public void OnPostprocessBuild(BuildReport report) {
      if (report.summary.platform != BuildTarget.iOS) {
        return;
      }

      string projectPath = PBXProject.GetPBXProjectPath(report.summary.outputPath);
      var project = new PBXProject();
      project.ReadFromFile(projectPath);
      string unityFrameworkTargetGuid = project.GetUnityFrameworkTargetGuid();
      string scriptingDefineSymbols =
          PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
      bool isPreviewEnabled = scriptingDefineSymbols.Contains("GMA_PREVIEW_FEATURES");

      if (isPreviewEnabled) {
        // This is equivalent to adding `-DGMA_PREVIEW_FEATURES=1` to the compiler flags.
        project.AddBuildProperty(unityFrameworkTargetGuid, "GCC_PREPROCESSOR_DEFINITIONS",
                                 "GMA_PREVIEW_FEATURES=1");
      }

      project.WriteToFile(projectPath);
    }
  }
}
