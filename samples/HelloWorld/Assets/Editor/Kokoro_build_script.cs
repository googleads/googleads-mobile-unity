using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;


public class Kokoro: MonoBehaviour
{
      [MenuItem("Build/Build Android")]
    public static void MyBuild()
    {

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] {"Assets/Scenes/MainScene.unity"};
        buildPlayerOptions.locationPathName = "AndroidBuildclisep7.apk";
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }

}
