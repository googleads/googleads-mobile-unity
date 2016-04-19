using Google.JarResolver;
using UnityEditor;

/// AdMob dependencies file.
[InitializeOnLoad]
public static class AdMobDependencies
{
    /// The name of the plugin.
    private static readonly string PluginName = "AdMobUnity";

    /// Initializes static members of the class.
    static AdMobDependencies()
    {
        PlayServicesSupport svcSupport =
            PlayServicesSupport.CreateInstance(PluginName, EditorPrefs.GetString("AndroidSdkRoot"),
                    "ProjectSettings");

        svcSupport.DependOn("com.google.android.gms", "play-services-ads", "LATEST");

        // Marshmallow permissions requires app-compat.
        svcSupport.DependOn("com.android.support", "appcompat-v7", "23.1.0+");
    }
}
