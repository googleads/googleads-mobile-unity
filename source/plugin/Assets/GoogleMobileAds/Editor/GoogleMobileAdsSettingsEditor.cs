using UnityEditor;
using UnityEngine;

namespace GoogleMobileAds.Editor
{

    [InitializeOnLoad]
    [CustomEditor(typeof(GoogleMobileAdsSettings))]
    public class GoogleMobileAdsSettingsEditor : UnityEditor.Editor
    {

        SerializedProperty _appIdAndroid;
        SerializedProperty _appIdiOS;
        SerializedProperty _delayAppMeasurement;

        [MenuItem("Assets/Google Mobile Ads/Settings...")]
        public static void OpenInspector()
        {
            Selection.activeObject = GoogleMobileAdsSettings.LoadInstance();
        }

        public void OnEnable()
        {
            _appIdAndroid = serializedObject.FindProperty("adMobAndroidAppId");
            _appIdiOS = serializedObject.FindProperty("adMobIOSAppId");
            _delayAppMeasurement = serializedObject.FindProperty("delayAppMeasurementInit");
        }

        public override void OnInspectorGUI()
        {
            // Make sure the Settings object has all recent changes.
            serializedObject.Update();

            var settings = (GoogleMobileAdsSettings)target;

            if(settings == null)
            {
              UnityEngine.Debug.LogError("GoogleMobileAdsSettings is null.");
              return;
            }

            EditorGUILayout.LabelField("Google Mobile Ads App ID", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(_appIdAndroid, new GUIContent("Android"));

            EditorGUILayout.PropertyField(_appIdiOS, new GUIContent("iOS"));

            EditorGUILayout.HelpBox(
                    "Google Mobile  Ads App ID will look similar to this sample ID: ca-app-pub-3940256099942544~3347511713",
                    MessageType.Info);

            EditorGUI.indentLevel--;
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("AdMob-specific settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(_delayAppMeasurement,
                new GUIContent("Delay app measurement"));

            if (settings.DelayAppMeasurementInit) {
                EditorGUILayout.HelpBox(
                        "Delays app measurement until you explicitly initialize the Mobile Ads SDK or load an ad.",
                        MessageType.Info);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Separator();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
