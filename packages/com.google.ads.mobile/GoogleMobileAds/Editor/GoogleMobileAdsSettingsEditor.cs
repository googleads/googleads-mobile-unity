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
        SerializedProperty _enableKotlinXCoroutinesPackagingOption;
        SerializedProperty _optimizeInitialization;
        SerializedProperty _optimizeAdLoading;
        SerializedProperty _userTrackingUsageDescription;
        SerializedProperty _validateGradleDependencies;


        [MenuItem("Assets/Google Mobile Ads/Settings...")]
        public static void OpenInspector()
        {
            Selection.activeObject = GoogleMobileAdsSettings.LoadInstance();
        }

        public void OnEnable()
        {
            _appIdAndroid = serializedObject.FindProperty("adMobAndroidAppId");
            _appIdiOS = serializedObject.FindProperty("adMobIOSAppId");
            _enableKotlinXCoroutinesPackagingOption =
                serializedObject.FindProperty("enableKotlinXCoroutinesPackagingOption");
            _optimizeInitialization = serializedObject.FindProperty("optimizeInitialization");
            _optimizeAdLoading = serializedObject.FindProperty("optimizeAdLoading");
            _userTrackingUsageDescription =
                    serializedObject.FindProperty("userTrackingUsageDescription");
            _validateGradleDependencies =
                    serializedObject.FindProperty("validateGradleDependencies");
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

            EditorGUIUtility.labelWidth = 60.0f;
            EditorGUILayout.LabelField("Google Mobile Ads App ID", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(_appIdAndroid, new GUIContent("Android"));

            EditorGUILayout.PropertyField(_appIdiOS, new GUIContent("iOS"));

            EditorGUILayout.HelpBox(
                    "Google Mobile Ads App ID will look similar to this sample ID: ca-app-pub-3940256099942544~3347511713",
                    MessageType.Info);

            EditorGUI.indentLevel--;
            EditorGUILayout.Separator();

            EditorGUIUtility.labelWidth = 325.0f;
            EditorGUILayout.LabelField("Android settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(_enableKotlinXCoroutinesPackagingOption,
                              new GUIContent("Enable kotlinx.coroutines packaging option."));

            if (settings.EnableKotlinXCoroutinesPackagingOption)
            {
                EditorGUILayout.HelpBox(
                        "Adds instruction to fix a build.gradle build error with message"+
                        " '2 files found with path 'META-INF/kotlinx_coroutines_core.version'."+
                        " For more details see https://developers.google.com/admob/unity/gradle",
                        MessageType.Info);
            }

            EditorGUILayout.PropertyField(_validateGradleDependencies,
                              new GUIContent("Remove property tag from GMA Android SDK"));

            if (settings.ValidateGradleDependencies)
            {
                EditorGUILayout.HelpBox(
                    "This option ensures the GMA Android SDK is compatible with the version of " +
                    "Android Gradle Plugin being used. Enabling this option is required for Unity" +
                    " Projects that use Android Gradle Plugin under version 4.2.2.",
                    MessageType.Info);
            }

            EditorGUILayout.PropertyField(_optimizeInitialization,
                                          new GUIContent("Optimize initialization"));
            if (settings.OptimizeInitialization) {
                EditorGUILayout.HelpBox(
                        "Initialization will be offloaded to a background thread.",
                        MessageType.Info);
            }

            EditorGUILayout.PropertyField(_optimizeAdLoading,
                                          new GUIContent("Optimize ad loading"));

            if (settings.OptimizeAdLoading) {
                EditorGUILayout.HelpBox(
                        "Ad loading tasks will be offloaded to a background thread.",
                        MessageType.Info);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Separator();

            EditorGUIUtility.labelWidth = 205.0f;
            EditorGUILayout.LabelField("UMP-specific settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(_userTrackingUsageDescription,
                                          new GUIContent("User Tracking Usage Description"));

            EditorGUILayout.HelpBox(
                    "A message that informs the user why an iOS app is requesting permission to " +
                    "use data for tracking the user or the device.", MessageType.Info);

            EditorGUI.indentLevel--;
            EditorGUILayout.Separator();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
