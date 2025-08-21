#if UNITY_6000_0_OR_NEWER || UNITY_2023 || UNITY_2022 || UNITY_2021_3_55 || UNITY_2021_3_54 || UNITY_2021_3_53 || UNITY_2021_3_52 || UNITY_2021_3_51 || UNITY_2021_3_50 || UNITY_2021_3_49 || UNITY_2021_3_48 || UNITY_2021_3_47 || UNITY_2021_3_46 || UNITY_2021_3_45 || UNITY_2021_3_44 || UNITY_2021_3_43 || UNITY_2021_3_42 || UNITY_2021_3_41
#define ANDROID_GRADLE_BUILD_PRE_PROCESSOR_ENABLED
#endif

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
    SerializedProperty _enableGradleBuildPreProcessor;
    SerializedProperty _enableKotlinXCoroutinesPackagingOption;
    SerializedProperty _disableOptimizeInitialization;
    SerializedProperty _disableOptimizeAdLoading;
    SerializedProperty _userLanguage;
    SerializedProperty _userTrackingUsageDescription;

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
      _enableGradleBuildPreProcessor =
          serializedObject.FindProperty("enableGradleBuildPreProcessor");
      _enableKotlinXCoroutinesPackagingOption =
          serializedObject.FindProperty("enableKotlinXCoroutinesPackagingOption");
      _disableOptimizeInitialization = serializedObject.FindProperty("disableOptimizeInitialization");
      _disableOptimizeAdLoading = serializedObject.FindProperty("disableOptimizeAdLoading");
      _userLanguage = serializedObject.FindProperty("userLanguage");
      _userTrackingUsageDescription =
          serializedObject.FindProperty("userTrackingUsageDescription");

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

      EditorLocalization localization = new EditorLocalization();
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

#if ANDROID_GRADLE_BUILD_PRE_PROCESSOR_ENABLED
      EditorGUILayout.PropertyField(
          _enableGradleBuildPreProcessor,
          new GUIContent(
              localization.ForKey("ENABLE_GRADLE_BUILD_PRE_PROCESSOR_SETTING")));

      if (settings.EnableGradleBuildPreProcessor)
      {
        EditorGUILayout.HelpBox(
            localization.ForKey("ENABLE_GRADLE_BUILD_PRE_PROCESSOR_HELPBOX"),
            MessageType.Info);
      }
#endif

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
          _disableOptimizeInitialization,
          new GUIContent(localization.ForKey("DISABLE_OPTIMIZE_INITIALIZATION_SETTING")));
      if (settings.DisableOptimizeInitialization)
      {
        EditorGUILayout.HelpBox(localization.ForKey("DISABLE_OPTIMIZE_INITIALIZATION_HELPBOX"),
                                MessageType.Info);
      }

      EditorGUILayout.PropertyField(
          _disableOptimizeAdLoading,
          new GUIContent(localization.ForKey("DISABLE_OPTIMIZE_AD_LOADING_SETTING")));

      if (settings.DisableOptimizeAdLoading)
      {
        EditorGUILayout.HelpBox(localization.ForKey("DISABLE_OPTIMIZE_AD_LOADING_HELPBOX"),
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
