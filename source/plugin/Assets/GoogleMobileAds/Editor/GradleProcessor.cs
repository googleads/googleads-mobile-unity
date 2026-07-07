using System;
using System.IO;
using UnityEditor.Android;

namespace GoogleMobileAds.Editor {
  public class GradleProcessor : IPostGenerateGradleAndroidProject {
    public int callbackOrder {
      get { return 0; }
    }

    private const string ApplyFrom = "apply from: ";
    private const string GmaPluginPath = "GoogleMobileAdsPlugin.androidlib/";
    private const string LauncherPath = "../unityLibrary/" + GmaPluginPath;

    private const string PackagingOptionsGradle = "packaging_options.gradle";
    private const string NextGenExclusionsGradle = "next_gen_exclusions.gradle";
    private const string NextGenResolutionStrategyGradle = "next_gen_resolution_strategy.gradle";
    private const string GmaValidateDependenciesGradle = "validate_dependencies.gradle";

    private const string GmaPackagingOptionsLauncher =
        ApplyFrom + "'" + LauncherPath + PackagingOptionsGradle + "'";
    private const string GmaPackagingOptions =
        ApplyFrom + "'" + GmaPluginPath + PackagingOptionsGradle + "'";
    private const string GmaNextGenExclusions =
        ApplyFrom + "'" + GmaPluginPath + NextGenExclusionsGradle + "'";
    private const string GmaNextGenResolutionStrategy =
        ApplyFrom + "'" + GmaPluginPath + NextGenResolutionStrategyGradle + "'";
    private const string GmaNextGenResolutionStrategyLauncher =
        ApplyFrom + "'" + LauncherPath + NextGenResolutionStrategyGradle + "'";

    // The newer versions of webkit (1.15.0+) require AGP 8.1.0+. Thus we are downgrading the
    // webkit and error_prone_annotations dependencies in Apps using older AGP versions.
    private static readonly Version gmaAgpDowngradeVersion = new Version("8.1.0");

    public void OnPostGenerateGradleAndroidProject(string path) {
      var rootDirinfo = new DirectoryInfo(path);
      var rootPath = rootDirinfo.Parent.FullName;
      var gradleList = Directory.GetFiles(rootPath, "build.gradle", SearchOption.AllDirectories);

      var packagingOptionsLauncher = GmaPackagingOptionsLauncher;
      var packagingOptionsUnityLibrary = GmaPackagingOptions;
      var nextGenExclusionsUnityLibrary = GmaNextGenExclusions;
      var nextGenResolutionStrategyLauncher = GmaNextGenResolutionStrategyLauncher;
      var nextGenResolutionStrategyUnityLibrary = GmaNextGenResolutionStrategy;

      // Windows path requires '\\'
#if UNITY_EDITOR_WIN
      packagingOptionsLauncher = packagingOptionsLauncher.Replace("/", "\\\\");
      packagingOptionsUnityLibrary = packagingOptionsUnityLibrary.Replace("/", "\\\\");
      nextGenExclusionsUnityLibrary = nextGenExclusionsUnityLibrary.Replace("/", "\\\\");
      nextGenResolutionStrategyLauncher = nextGenResolutionStrategyLauncher.Replace("/", "\\\\");
      nextGenResolutionStrategyUnityLibrary =
          nextGenResolutionStrategyUnityLibrary.Replace("/", "\\\\");
#endif

      Version agpVersion = GoogleMobileAds.Editor.Utils.AndroidGradlePluginVersion;
      foreach (var gradlepath in gradleList) {
        if (!gradlepath.Contains("unityLibrary/build.gradle") &&
            !gradlepath.Contains("launcher/build.gradle") &&
            !gradlepath.Contains("unityLibrary\\build.gradle") &&
            !gradlepath.Contains("launcher\\build.gradle")) {
          continue;
        }

        var contents = File.ReadAllText(gradlepath);
        bool modified = false;

        // 1. Clear any previously injected gradle script references (to start with a clean
        // slate).
        if (contents.Contains(PackagingOptionsGradle)) {
          contents = DeleteLineContainingSubstring(contents, PackagingOptionsGradle);
          modified = true;
        }

        if (contents.Contains(NextGenExclusionsGradle)) {
          contents = DeleteLineContainingSubstring(contents, NextGenExclusionsGradle);
          modified = true;
        }

        if (contents.Contains(NextGenResolutionStrategyGradle)) {
          contents = DeleteLineContainingSubstring(contents, NextGenResolutionStrategyGradle);
          modified = true;
        }

        // 2. Remove validate_dependencies.gradle script reference if present. This gradle file
        // is no longer needed since the offending AndroidManifest.xml tags have been removed.
        // This check will also be removed in the next major plugin release (v12.0.0).
        if (contents.Contains(GmaValidateDependenciesGradle)) {
          contents = DeleteLineContainingSubstring(contents, GmaValidateDependenciesGradle);
          modified = true;
        }

        // 3. Apply packaging options if enabled.
        if (GoogleMobileAdsSettings.LoadInstance().EnableKotlinXCoroutinesPackagingOption) {
          if (gradlepath.Contains("unityLibrary/build.gradle") ||
              gradlepath.Contains("unityLibrary\\build.gradle")) {
            contents += Environment.NewLine + packagingOptionsUnityLibrary;
          } else if (gradlepath.Contains("launcher/build.gradle") ||
                     gradlepath.Contains("launcher\\build.gradle")) {
            contents += Environment.NewLine + packagingOptionsLauncher;
          }
          modified = true;
        }

        // 4. Apply Next-Gen resolution strategy if target is NextGen SDK and AGP version is
        // less than 8.1.0.
        if (GoogleMobileAdsSettings.LoadInstance().EffectiveGmaAndroidSdk ==
            GoogleMobileAdsSettings.GmaAndroidSdk.NextGen) {
          if (gradlepath.Contains("unityLibrary/build.gradle") ||
              gradlepath.Contains("unityLibrary\\build.gradle")) {
            contents += Environment.NewLine + nextGenExclusionsUnityLibrary;
            modified = true;
          }

          if (agpVersion < gmaAgpDowngradeVersion) {
            if (gradlepath.Contains("unityLibrary/build.gradle") ||
                gradlepath.Contains("unityLibrary\\build.gradle")) {
              contents += Environment.NewLine + nextGenResolutionStrategyUnityLibrary;
              modified = true;
            } else if (gradlepath.Contains("launcher/build.gradle") ||
                       gradlepath.Contains("launcher\\build.gradle")) {
              contents += Environment.NewLine + nextGenResolutionStrategyLauncher;
              modified = true;
            }
          }
        }
        if (modified) {
          File.WriteAllText(gradlepath, contents);
        }
      }
    }

    private string DeleteLineContainingSubstring(string file, string substring) {
      string escapedSubstring = System.Text.RegularExpressions.Regex.Escape(substring);
      string pattern = @"^.*" + escapedSubstring + @".*(?:\r\n|\r|\n|$)";
      return System.Text.RegularExpressions.Regex.Replace(
          file, pattern, "", System.Text.RegularExpressions.RegexOptions.Multiline);
    }
  }
}
