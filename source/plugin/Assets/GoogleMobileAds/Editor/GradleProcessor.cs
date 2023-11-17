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

    private const string GMA_VALIDATE_GRADLE_DEPENDENCIES =
      "apply from: 'GoogleMobileAdsPlugin.androidlib/validate_dependencies.gradle'";

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        var rootDirinfo = new DirectoryInfo(path);
        var rootPath = rootDirinfo.Parent.FullName;
        var gradleList = Directory.GetFiles(rootPath, "build.gradle", SearchOption.AllDirectories);

        foreach (var gradlepath in gradleList)
        {
            if (!gradlepath.Contains("unityLibrary/build.gradle") &&
                !gradlepath.Contains("launcher/build.gradle"))
            {
                continue;
            }

            var contents = File.ReadAllText(gradlepath);
            // Delete existing packaging_options and then set it if enabled.
            if (contents.Contains("GoogleMobileAdsPlugin.androidlib/packaging_options"))
            {
                contents = DeleteLineContainingSubstring(
                    contents, "GoogleMobileAdsPlugin.androidlib/packaging_options");
            }

            if (!GoogleMobileAdsSettings.LoadInstance().EnableKotlinXCoroutinesPackagingOption)
            {
                File.WriteAllText(gradlepath, contents);
                continue;
            }

            if (gradlepath.Contains("unityLibrary/build.gradle"))
            {
                contents += Environment.NewLine + GMA_PACKAGING_OPTIONS;
            }
            else if (gradlepath.Contains("launcher/build.gradle"))
            {
                contents += Environment.NewLine + GMA_PACKAGING_OPTIONS_LAUNCHER;
            }
            File.WriteAllText(gradlepath, contents);
        }

        // TODO (b/311555203) Use delete then write approach above to update this Gradle script too.
        var unityLibraryGradle = Directory.GetFiles(rootPath, "unityLibrary/build.gradle",
                    SearchOption.TopDirectoryOnly);

        foreach (var gradlePath in unityLibraryGradle)
        {
            var contents = File.ReadAllText(gradlePath);
            if (GoogleMobileAdsSettings.LoadInstance().ValidateGradleDependencies)
            {
                if (!contents.Contains(GMA_VALIDATE_GRADLE_DEPENDENCIES))
                {
                    contents += Environment.NewLine + GMA_VALIDATE_GRADLE_DEPENDENCIES;
                    File.WriteAllText(gradlePath, contents);
                }
            }
            else
            {
                contents = DeleteLineContainingSubstring(
                        contents, GMA_VALIDATE_GRADLE_DEPENDENCIES);
                File.WriteAllText(gradlePath, contents);
            }
        }
    }

    private string DeleteLineContainingSubstring(string file, string substring)
    {
        string newFile = "";
        var lines = file.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        foreach (var line in lines)
        {
            if (!line.Contains(substring))
            {
                newFile += line + Environment.NewLine;
            }
        }
        return newFile;
    }
}
