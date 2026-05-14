using System;
using System.IO;
using UnityEditor.Android;

using GoogleMobileAds.Editor;

public class GradleProcessor : IPostGenerateGradleAndroidProject
{
    public int callbackOrder { get { return 0; } }

    private const string GMA_PACKAGING_OPTIONS_LAUNCHER =
      "apply from: '../unityLibrary/GoogleMobileAdsPlugin.androidlib/packaging_options.gradle'";

    private const string GMA_PACKAGING_OPTIONS =
      "apply from: 'GoogleMobileAdsPlugin.androidlib/packaging_options.gradle'";

    private const string GMA_VALIDATE_GRADLE_DEPENDENCIES_FILENAME = "validate_dependencies.gradle";

    private const string GMA_NEXT_GEN_EXCLUSIONS =
      "apply from: 'GoogleMobileAdsPlugin.androidlib/next_gen_exclusions.gradle'";

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        var rootDirinfo = new DirectoryInfo(path);
        var rootPath = rootDirinfo.Parent.FullName;
        var gradleList = Directory.GetFiles(rootPath, "build.gradle", SearchOption.AllDirectories);

        var packagingOptionsLauncher = GMA_PACKAGING_OPTIONS_LAUNCHER;
        var packagingOptionsUnityLibrary = GMA_PACKAGING_OPTIONS;
        var nextGenExclusionsUnityLibrary = GMA_NEXT_GEN_EXCLUSIONS;

        // Windows path requires '\\'
#if UNITY_EDITOR_WIN
        packagingOptionsLauncher = packagingOptionsLauncher.Replace("/","\\\\");
        packagingOptionsUnityLibrary = packagingOptionsUnityLibrary.Replace("/","\\\\");
        nextGenExclusionsUnityLibrary = nextGenExclusionsUnityLibrary.Replace("/","\\\\");
#endif

        foreach (var gradlepath in gradleList)
        {
            if (!gradlepath.Contains("unityLibrary/build.gradle") &&
                !gradlepath.Contains("launcher/build.gradle") &&
                !gradlepath.Contains("unityLibrary\\build.gradle") &&
                !gradlepath.Contains("launcher\\build.gradle"))
            {
                continue;
            }

            var contents = File.ReadAllText(gradlepath);
            bool modified = false;

            // 1. Clear any previously injected gradle script references (to start with a clean
            // baseline)
            if (contents.Contains("packaging_options.gradle"))
            {
                contents = DeleteLineContainingSubstring(contents, "packaging_options.gradle");
                modified = true;
            }

            if (contents.Contains("next_gen_exclusions.gradle"))
            {
                contents = DeleteLineContainingSubstring(contents, "next_gen_exclusions.gradle");
                modified = true;
            }

            // 2. Remove validate_dependencies.gradle script reference if present. This gradle file
            // is no longer needed since the offending AndroidManifest.xml tags have been removed.
            // This check will also be removed in the next major plugin release (v12.0.0).
            if (contents.Contains(GMA_VALIDATE_GRADLE_DEPENDENCIES_FILENAME))
            {
                contents = DeleteLineContainingSubstring(contents, GMA_VALIDATE_GRADLE_DEPENDENCIES_FILENAME);
                modified = true;
            }

            // 3. Apply packaging options if enabled
            if (GoogleMobileAdsSettings.LoadInstance().EnableKotlinXCoroutinesPackagingOption)
            {
                if (gradlepath.Contains("unityLibrary/build.gradle") ||
                    gradlepath.Contains("unityLibrary\\build.gradle"))
                {
                    contents += Environment.NewLine + packagingOptionsUnityLibrary;
                }
                else if (gradlepath.Contains("launcher/build.gradle") ||
                         gradlepath.Contains("launcher\\build.gradle"))
                {
                    contents += Environment.NewLine + packagingOptionsLauncher;
                }
                modified = true;
            }

            // 4. Apply Next-Gen exclusions if target is NextGen SDK.
            if (GoogleMobileAdsSettings.LoadInstance().EffectiveGmaAndroidSdk ==
                GoogleMobileAdsSettings.GmaAndroidSdk.NextGen)
            {
                if (gradlepath.Contains("unityLibrary/build.gradle") ||
                    gradlepath.Contains("unityLibrary\\build.gradle"))
                {
                    contents += Environment.NewLine + nextGenExclusionsUnityLibrary;
                    modified = true;
                }
            }

            if (modified)
            {
                File.WriteAllText(gradlepath, contents);
            }
        }
    }

    private string DeleteLineContainingSubstring(string file, string substring)
    {
      string escapedSubstring = System.Text.RegularExpressions.Regex.Escape(substring);
      string pattern = @"^.*" + escapedSubstring + @".*(?:\r\n|\r|\n|$)";
      return System.Text.RegularExpressions.Regex.Replace(
          file, pattern, "", System.Text.RegularExpressions.RegexOptions.Multiline);
    }
}
