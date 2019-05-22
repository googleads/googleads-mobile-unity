using System.IO;

using UnityEditor;
using UnityEngine;

namespace GoogleMobileAds.Editor
{

    [InitializeOnLoad]
    [CustomEditor(typeof(GoogleMobileAdsSettings))]
    public class GoogleMobileAdsSettingsEditor : UnityEditor.Editor
    {
        [MenuItem("Assets/Google Mobile Ads/Settings...")]
        public static void OpenInspector()
        {
            Selection.activeObject = GoogleMobileAdsSettings.Instance;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Google Ad Manager", EditorStyles.boldLabel);
            GoogleMobileAdsSettings.Instance.IsAdManagerEnabled =
                    EditorGUILayout.Toggle(new GUIContent("Enabled"),
                            GoogleMobileAdsSettings.Instance.IsAdManagerEnabled);

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Google AdMob", EditorStyles.boldLabel);
            GoogleMobileAdsSettings.Instance.IsAdMobEnabled =
                    EditorGUILayout.Toggle(new GUIContent("Enabled"),
                            GoogleMobileAdsSettings.Instance.IsAdMobEnabled);

            EditorGUILayout.Separator();

            EditorGUI.BeginDisabledGroup(!GoogleMobileAdsSettings.Instance.IsAdMobEnabled);

            EditorGUILayout.LabelField("AdMob App ID");

            GoogleMobileAdsSettings.Instance.AdMobAndroidAppId =
                    EditorGUILayout.TextField("Android",
                            GoogleMobileAdsSettings.Instance.AdMobAndroidAppId);

            GoogleMobileAdsSettings.Instance.AdMobIOSAppId =
                    EditorGUILayout.TextField("iOS",
                            GoogleMobileAdsSettings.Instance.AdMobIOSAppId);

            if (GoogleMobileAdsSettings.Instance.IsAdMobEnabled)
            {
                EditorGUILayout.HelpBox(
                        "AdMob App ID will look similar to this sample ID: ca-app-pub-3940256099942544~3347511713",
                        MessageType.Info);
            }

            EditorGUILayout.Separator();

            GoogleMobileAdsSettings.Instance.DelayAppMeasurementInit =
                    EditorGUILayout.Toggle(new GUIContent("Delay app measurement"),
                    GoogleMobileAdsSettings.Instance.DelayAppMeasurementInit);
            if (GoogleMobileAdsSettings.Instance.DelayAppMeasurementInit) {
                    EditorGUILayout.HelpBox(
                            "Delays app measurement until you explicitly initialize the Mobile Ads SDK or load an ad.",
                            MessageType.Info);
            }
            EditorGUI.EndDisabledGroup();

            if (GUI.changed)
            {
                OnSettingsChanged();
            }
        }

        private void OnSettingsChanged()
        {
            EditorUtility.SetDirty((GoogleMobileAdsSettings) target);
            GoogleMobileAdsSettings.Instance.WriteSettingsToFile();
        }
    }
}
