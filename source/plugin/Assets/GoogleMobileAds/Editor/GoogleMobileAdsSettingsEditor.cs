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
        SerializedProperty _overrideAapt2;
        SerializedProperty _optimizeInitialization;
        SerializedProperty _optimizeAdLoading;
        SerializedProperty _userTrackingUsageDescription;


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
            _overrideAapt2 =
                serializedObject.FindProperty("overrideAapt2");
            _optimizeInitialization = serializedObject.FindProperty("optimizeInitialization");
            _optimizeAdLoading = serializedObject.FindProperty("optimizeAdLoading");
            _userTrackingUsageDescription =
                    serializedObject.FindProperty("userTrackingUsageDescription");
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
                    "Google Mobile Ads App ID will look similar to this sample ID: ca-app-pub-3940256099942544~3347511713",
                    MessageType.Info);

            EditorGUI.indentLevel--;
            EditorGUILayout.Separator();

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

            EditorGUILayout.PropertyField(_overrideAapt2,
                              new GUIContent("Enable overriding default AAPT2"));

            if (settings.OverrideAapt2)
            {
                EditorGUILayout.HelpBox(
                    "Enabling this option, instructs the Android Gradle Plugin to use the AAPT2"+
                    " library packaged as part of the GMA Unity Plugin. This option is only"+
                    " required for projects that use Android Gradle Plugin beneath version 4.2.2.",
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
