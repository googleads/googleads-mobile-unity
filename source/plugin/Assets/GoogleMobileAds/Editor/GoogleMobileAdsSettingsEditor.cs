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
            EditorGUILayout.LabelField("Google Mobile Ads App ID");

            GoogleMobileAdsSettings.Instance.GoogleMobileAdsAndroidAppId =
                    EditorGUILayout.TextField("Android",
                            GoogleMobileAdsSettings.Instance.GoogleMobileAdsAndroidAppId);

            GoogleMobileAdsSettings.Instance.GoogleMobileAdsIOSAppId =
                    EditorGUILayout.TextField("iOS",
                            GoogleMobileAdsSettings.Instance.GoogleMobileAdsIOSAppId);

            EditorGUILayout.HelpBox(
                    "Google Mobile  Ads App ID will look similar to this sample ID: ca-app-pub-3940256099942544~3347511713",
                    MessageType.Info);

            EditorGUILayout.Separator();

            GoogleMobileAdsSettings.Instance.DelayAppMeasurementInit =
                    EditorGUILayout.Toggle(new GUIContent("Delay app measurement (AdMob only)"),
                    GoogleMobileAdsSettings.Instance.DelayAppMeasurementInit);
            if (GoogleMobileAdsSettings.Instance.DelayAppMeasurementInit) {
                    EditorGUILayout.HelpBox(
                            "Delays app measurement until you explicitly initialize the Mobile Ads SDK or load an ad.",
                            MessageType.Info);
            }

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
