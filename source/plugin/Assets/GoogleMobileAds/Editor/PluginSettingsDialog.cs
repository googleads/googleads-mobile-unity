using UnityEditor;
using UnityEngine;

public class PluginSettingsDialog : EditorWindow 
{
    [MenuItem("Assets/Google Mobile Ads/Plugin/Settings...")]
    public static void OpenInspector() 
    {
        PluginSettingsDialog window = (PluginSettingsDialog)EditorWindow.GetWindow(
            typeof(PluginSettingsDialog), true, "Plugin Settings");
        window.Show();
    }

    private PluginSettings settings;
    
    private void LoadSettings() 
    {
        settings = PluginSettingsProvider.Read();
    }
    
    public void OnEnable() 
    {
        LoadSettings();
    }
    
    public void OnGUI()
    {
        GUILayout.Label("Root plugin directory");
        settings.rootPluginDirectory = GUILayout.TextField(settings.rootPluginDirectory);
        GUILayout.Label("Mobile ads settings directory:");
        settings.mobileAdsSettingsDir = GUILayout.TextField(settings.mobileAdsSettingsDir);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset")) {
            settings = PluginSettingsProvider.CreateDefault();
            PluginSettingsProvider.Write(settings);
        }
        if (GUILayout.Button("Save")) {
            PluginSettingsProvider.Write(settings);
        }
        GUILayout.EndHorizontal();
    }
}