using System;
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
    SerializedProperty _userLanguage;
    SerializedProperty _userTrackingUsageDescription;
    SerializedProperty _validateGradleDependencies;

    // Using an ordered list of languages is computationally expensive when trying to create an
    // array out of them for purposes of showing a dropdown menu. Care should be taken to ensure
    // these arrays are kept in sync.
    string[] availableLanguages = new string[] { "English", "French"};
    string[] languageCodes = new string[] { "en", "fr" };
    int selectedIndex = 0;

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
      _userLanguage = serializedObject.FindProperty("userLanguage");
      _userTrackingUsageDescription =
          serializedObject.FindProperty("userTrackingUsageDescription");
      _validateGradleDependencies =
          serializedObject.FindProperty("validateGradleDependencies");

      selectedIndex = Array.IndexOf(languageCodes, _userLanguage.stringValue);
      selectedIndex = selectedIndex >= 0 ? selectedIndex : 0;
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
      EditorGUI.BeginChangeCheck();
      selectedIndex = EditorGUILayout.Popup("Language", selectedIndex, availableLanguages);
      if (EditorGUI.EndChangeCheck())
      {
        _userLanguage.stringValue = languageCodes[selectedIndex];
      }


      EditorGUIUtility.labelWidth = 60.0f;
      EditorGUILayout.LabelField(localization.ForKey("GMA_APP_ID_LABEL"),
                                 EditorStyles.boldLabel);
      EditorGUI.indentLevel++;

      EditorGUILayout.PropertyField(_appIdAndroid, new GUIContent("Android"));

      EditorGUILayout.PropertyField(_appIdiOS, new GUIContent("iOS"));

      EditorGUILayout.HelpBox(localization.ForKey("GMA_APP_ID_HELPBOX"), MessageType.Info);

      EditorGUI.indentLevel--;
      EditorGUILayout.Separator();

      EditorGUIUtility.labelWidth = 325.0f;
      EditorGUILayout.LabelField(localization.ForKey("ANDROID_SETTINGS_LABEL"),
                                 EditorStyles.boldLabel);
      EditorGUI.indentLevel++;

      EditorGUI.BeginChangeCheck();

      EditorGUILayout.PropertyField(
          _enableKotlinXCoroutinesPackagingOption,
          new GUIContent(
              localization.ForKey("ENABLE_KOTLINX_COROUTINES_PACKAGING_OPTION_SETTING")));

      if (settings.EnableKotlinXCoroutinesPackagingOption)
      {
        EditorGUILayout.HelpBox(
            localization.ForKey("ENABLE_KOTLINX_COROUTINES_PACKAGING_OPTION_HELPBOX"),
            MessageType.Info);
      }

      EditorGUILayout.PropertyField(
          _validateGradleDependencies,
          new GUIContent(localization.ForKey("VALIDATE_GRADLE_DEPENDENCIES_SETTING")));

      if (settings.ValidateGradleDependencies)
      {
        EditorGUILayout.HelpBox(localization.ForKey("VALIDATE_GRADLE_DEPENDENCIES_HELPBOX"),
                                MessageType.Info);
      }

      EditorGUILayout.PropertyField(
          _optimizeInitialization,
          new GUIContent(localization.ForKey("OPTIMIZE_INITIALIZATION_SETTING")));
      if (settings.OptimizeInitialization)
      {
        EditorGUILayout.HelpBox(localization.ForKey("OPTIMIZE_INITIALIZATION_HELPBOX"),
                                MessageType.Info);
      }

      EditorGUILayout.PropertyField(
          _optimizeAdLoading,
          new GUIContent(localization.ForKey("OPTIMIZE_AD_LOADING_SETTING")));

      if (settings.OptimizeAdLoading)
      {
        EditorGUILayout.HelpBox(localization.ForKey("OPTIMIZE_AD_LOADING_HELPBOX"),
                                MessageType.Info);
      }

      EditorGUI.indentLevel--;
      EditorGUILayout.Separator();

      EditorGUIUtility.labelWidth = 300.0f;
      EditorGUILayout.LabelField(localization.ForKey("UMP_SPECIFIC_SETTINGS_LABEL"),
                                 EditorStyles.boldLabel);
      EditorGUI.indentLevel++;

      EditorGUILayout.PropertyField(
          _userTrackingUsageDescription,
          new GUIContent(localization.ForKey("USER_TRACKING_USAGE_DESCRIPTION_SETTING")));

      EditorGUILayout.HelpBox(localization.ForKey("USER_TRACKING_USAGE_DESCRIPTION_HELPBOX"),
                              MessageType.Info);

      EditorGUI.indentLevel--;
      EditorGUILayout.Separator();

      serializedObject.ApplyModifiedProperties();
    }
  }
}
