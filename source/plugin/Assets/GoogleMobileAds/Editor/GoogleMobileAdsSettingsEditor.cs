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

            if (GoogleMobileAdsSettings.Instance.DelayAppMeasurementInit) {
                EditorGUILayout.HelpBox(
                        "Delays app measurement until you explicitly initialize the Mobile Ads SDK or load an ad.",
                        MessageType.Info);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Separator();

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
