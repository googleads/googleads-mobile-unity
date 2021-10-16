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

        private SerializedProperty adMobAndroidAppId;
        private SerializedProperty adMobIOSAppId;
        private SerializedProperty delayAppMeasurementInit;

        private void OnEnable()
        {
            adMobAndroidAppId = serializedObject.FindProperty("adMobAndroidAppId");
            adMobIOSAppId = serializedObject.FindProperty("adMobIOSAppId");
            delayAppMeasurementInit = serializedObject.FindProperty("delayAppMeasurementInit");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.LabelField("Google Mobile Ads App ID", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUI.BeginChangeCheck();

            adMobAndroidAppId.stringValue = EditorGUILayout.TextField("Android" , adMobAndroidAppId.stringValue);            
            adMobIOSAppId.stringValue = EditorGUILayout.TextField("iOS" , adMobIOSAppId.stringValue);
            EditorGUILayout.HelpBox(
                    "Google Mobile  Ads App ID will look similar to this sample ID: ca-app-pub-3940256099942544~3347511713",
                    MessageType.Info);
            EditorGUI.indentLevel--;
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("AdMob-specific settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            delayAppMeasurementInit.boolValue = EditorGUILayout.Toggle(new GUIContent("Delay app measurement") , delayAppMeasurementInit.boolValue);

            if (delayAppMeasurementInit.boolValue) {
                EditorGUILayout.HelpBox(
                        "Delays app measurement until you explicitly initialize the Mobile Ads SDK or load an ad.",
                        MessageType.Info);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Separator();
            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                GoogleMobileAdsSettings.Instance.WriteSettingsToFile();
            }
        }
    }
}
