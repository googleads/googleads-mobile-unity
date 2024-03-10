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
            _delayAppMeasurement = serializedObject.FindProperty("delayAppMeasurementInit");
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

            if (settings == null)
            {
                UnityEngine.Debug.LogError("GoogleMobileAdsSettings is null.");
                return;
            }

            EditorLocalization localization = new();

            EditorGUILayout.LabelField(localization.ForKey("GMA_APP_ID_LABEL"), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(_appIdAndroid, new GUIContent("Android"));

            EditorGUILayout.PropertyField(_appIdiOS, new GUIContent("iOS"));

            EditorGUILayout.HelpBox(
                    localization.ForKey("GMA_APP_ID_HELPBOX"),
                    MessageType.Info);

            EditorGUI.indentLevel--;
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField(localization.ForKey("ANDROID_SETTINGS_LABEL"), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(_enableKotlinXCoroutinesPackagingOption,
                              new GUIContent(localization.ForKey("ENABLE_KOTLINX_COROUTINES_PACKAGING_OPTION_SETTING")));

            if (settings.EnableKotlinXCoroutinesPackagingOption)
            {
                EditorGUILayout.HelpBox(
                        localization.ForKey("ENABLE_KOTLINX_COROUTINES_PACKAGING_OPTION_HELPBOX"),
                        MessageType.Info);
            }

            EditorGUILayout.PropertyField(_validateGradleDependencies,
                              new GUIContent(localization.ForKey("VALIDATE_GRADLE_DEPENDENCIES_SETTING")));

            if (settings.ValidateGradleDependencies)
            {
                EditorGUILayout.HelpBox(
                    localization.ForKey("VALIDATE_GRADLE_DEPENDENCIES_HELPBOX"),
                    MessageType.Info);
            }

            EditorGUILayout.PropertyField(_optimizeInitialization,
                                          new GUIContent(localization.ForKey("OPTIMIZE_INITIALIZATION_SETTING")));
            if (settings.OptimizeInitialization)
            {
                EditorGUILayout.HelpBox(
                        localization.ForKey("OPTIMIZE_INITIALIZATION_HELPBOX"),
                        MessageType.Info);
            }

            EditorGUILayout.PropertyField(_optimizeAdLoading,
                                          new GUIContent(localization.ForKey("OPTIMIZE_AD_LOADING_SETTING")));

            if (settings.OptimizeAdLoading)
            {
                EditorGUILayout.HelpBox(
                        localization.ForKey("OPTIMIZE_AD_LOADING_HELPBOX"),
                        MessageType.Info);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField(localization.ForKey("ADMOB_SPECIFIC_SETTINGS_LABEL"), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(_delayAppMeasurement,
                                          new GUIContent(localization.ForKey("DELAY_APP_MEASUREMENT_SETTING")));

            if (settings.DelayAppMeasurementInit)
            {
                EditorGUILayout.HelpBox(
                        localization.ForKey("DELAY_APP_MEASUREMENT_INIT_HELPBOX"),
                        MessageType.Info);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField(localization.ForKey("UMP_SPECIFIC_SETTINGS_LABEL"), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(_userTrackingUsageDescription,
                                          new GUIContent(localization.ForKey("USER_TRACKING_USAGE_DESCRIPTION_SETTING")));

            EditorGUILayout.HelpBox(
                    localization.ForKey("USER_TRACKING_USAGE_DESCRIPTION_HELPBOX"), MessageType.Info);

            EditorGUI.indentLevel--;
            EditorGUILayout.Separator();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
