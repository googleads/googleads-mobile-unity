// Copyright (C) 2019 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEditor;
using UnityEngine;

using System.Collections.Generic;

using GoogleMobileAds.Api;
using GoogleMobileAds.Placement;

namespace GoogleMobileAds.Editor
{
    [InitializeOnLoad]
    [CustomEditor(typeof(GoogleMobileAdsPlacements))]
    public class GoogleMobileAdsPlacementsEditor : UnityEditor.Editor
    {
        static GoogleMobileAdsPlacements config;
        private bool refreshInspector = true;
        private SerializedProperty propAllPlacements;
        SerializedProperty elementProperty;
        SerializedProperty propPlacementName;
        SerializedProperty propAndroidAdUnitId;
        SerializedProperty propiOSAdUnitId;
        SerializedProperty propPersistent;
        SerializedProperty propAutoLoadEnabled;
        SerializedProperty propAdType;

        internal GUIStyle styleAdPlacementArea, styleBtnDelete, baseStyle;
        internal Texture2D iconProductBanner, iconDelete, iconFeedback;

        private const string GoogleMobileAdsPlacementsDir = "Assets/GoogleMobileAds/";

        private const string GoogleMobileAdsPlacementsResDir = GoogleMobileAdsPlacementsDir + "Resources/";

        private const string GoogleMobileAdsPlacementsFile = GoogleMobileAdsPlacementsResDir + "GoogleMobileAdsPlacements.asset";

        private const string feedbackUrl = "https://googlemobileadssdk.page.link/unity-feedback";

        [MenuItem("Assets/Google Mobile Ads/Ad Placements")]
        public static void SelectAdPlacementsAsset()
        {
            MaybeCreateAdMobConfig();
        }

        void OnEnable()
        {
            propAllPlacements = serializedObject.FindProperty("allPlacements");

            // Get resources
            // TODO(b/156089995): Update resource loading from EditorGUIUtility.Load<>()
            string logoName = EditorGUIUtility.isProSkin ? "GMALogoLight" : "GMALogoDark";
            string deleteIconName = EditorGUIUtility.isProSkin ? "DeleteIconLight": "DeleteIconDark";
            string feedbackIconName = EditorGUIUtility.isProSkin ? "FeedbackIconLight": "FeedbackIconDark";

            iconProductBanner = Resources.Load<Texture2D>(logoName);
            iconDelete = Resources.Load<Texture2D>(deleteIconName);
            iconFeedback = Resources.Load<Texture2D>(feedbackIconName);

            // MaybeSetupStyles();
            refreshInspector = true;
        }

        void OnDisable()
        {
            // Save the asset whenever moving away from this inspector window
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            MaybeSetupStyles();

            // target.name = "";
            EditorGUI.BeginChangeCheck();
            CreateEditor();
            if (EditorGUI.EndChangeCheck() || refreshInspector)
            {
                serializedObject.ApplyModifiedProperties();
                refreshInspector = false;
            }
        }

        protected override void OnHeaderGUI()
        {
            GUIStyle styleHeader = new GUIStyle();
            styleHeader.padding = new RectOffset(24, 8, 16, 0);
            styleHeader.fixedHeight = 30f;
            styleHeader.alignment = TextAnchor.LowerLeft;

            GUIStyle styleHeaderLabel = new GUIStyle(EditorStyles.label);
            styleHeaderLabel.padding = new RectOffset(0, 0, 12, 0);
            styleHeaderLabel.fontSize = 10;

            GUIStyle styleFeedbackButton = new GUIStyle();
            styleFeedbackButton.padding = new RectOffset(0, 0, 8, 0);
            styleFeedbackButton.fixedHeight = 30f;
            styleFeedbackButton.fixedWidth = 120f;

            GUILayout.BeginHorizontal(styleHeader);
            GUILayout.Label(iconProductBanner, GUILayout.Height(30f), GUILayout.Width(170f));
            GUILayout.Label("v" + AdRequest.Version, styleHeaderLabel);

            GUILayout.FlexibleSpace();
            // Remove button background
            Color bgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.clear;
            if(GUILayout.Button(iconFeedback, styleFeedbackButton)) {
                Application.OpenURL(feedbackUrl);
            }
            // Reset background color
            GUI.backgroundColor = bgColor;

            GUILayout.EndHorizontal();
        }

        void MaybeSetupStyles()
        {
            if (baseStyle == null || styleAdPlacementArea == null || styleBtnDelete == null)
            {
                // Setup GUI Styles
                baseStyle = new GUIStyle();
                baseStyle.padding = new RectOffset(0, 0, 16, 0);

                styleAdPlacementArea = new GUIStyle(EditorStyles.helpBox);
                styleAdPlacementArea.padding = new RectOffset(8, 8, 8, 8);

                // Delete Button Style
                styleBtnDelete = new GUIStyle();
                styleBtnDelete.fixedHeight = 20f;
                styleBtnDelete.fixedWidth = 20f;
            }
        }

        static void MaybeCreateAdMobConfig()
        {
            if (!AssetDatabase.IsValidFolder(GoogleMobileAdsPlacementsDir))
            {
                AssetDatabase.CreateFolder("Assets", "GoogleMobileAds");
            }

            if (!AssetDatabase.IsValidFolder(GoogleMobileAdsPlacementsResDir))
            {
                AssetDatabase.CreateFolder(GoogleMobileAdsPlacementsDir, "Resources");
            }

            string configGUID = AssetDatabase.AssetPathToGUID(GoogleMobileAdsPlacementsFile);

            if (string.IsNullOrEmpty(configGUID))
            {
                config = ScriptableObject.CreateInstance<GoogleMobileAdsPlacements>();
                AssetDatabase.CreateAsset(config, GoogleMobileAdsPlacementsFile);
                config.allPlacements = new List<AdPlacement>();
            }
            else
            {
                config = (GoogleMobileAdsPlacements)AssetDatabase.LoadAssetAtPath(GoogleMobileAdsPlacementsFile, typeof(GoogleMobileAdsPlacements));
            }
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = config;
        }

        void CreateEditor()
        {
            GUILayout.BeginVertical(baseStyle);
            for (int i = 0; i < propAllPlacements.arraySize; i++)
            {
                DrawAdPlacementRect(i);
            }

            // Add Placement Button
            ShowNewAdPlacementButton();
            GUILayout.EndVertical();
        }

        void DrawAdPlacementRect(int id)
        {
            elementProperty = propAllPlacements.GetArrayElementAtIndex(id);
            propPlacementName = elementProperty.FindPropertyRelative("placementName");
            propAndroidAdUnitId = elementProperty.FindPropertyRelative("androidAdUnitId");
            propiOSAdUnitId = elementProperty.FindPropertyRelative("iOSAdUnitId");
            propPersistent = elementProperty.FindPropertyRelative("persistent");
            propAutoLoadEnabled = elementProperty.FindPropertyRelative("autoLoadEnabled");
            propAdType = elementProperty.FindPropertyRelative("adType");

            GUILayout.Space(5f);
            GUILayout.BeginVertical(styleAdPlacementArea);
            EditorGUILayout.PropertyField(propPlacementName, new GUIContent("Placement Name"));
            GUILayout.Space(5f);

            EditorGUI.BeginChangeCheck();
            propAdType.enumValueIndex = EditorGUILayout.Popup("Ad Format", propAdType.enumValueIndex, propAdType.enumDisplayNames);
            string adFormatDisplayName = propAdType.enumDisplayNames[propAdType.enumValueIndex];

            if (EditorGUI.EndChangeCheck())
            {
            }

            GUILayout.Space(5f);
            EditorGUILayout.PropertyField(propAndroidAdUnitId, new GUIContent("Android Ad Unit ID"));
            GUILayout.Space(5f);
            EditorGUILayout.PropertyField(propiOSAdUnitId, new GUIContent("iOS Ad Unit ID"));
            GUILayout.Space(5f);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propPersistent, new GUIContent("Persistent", "When checked, it will keep the ad alive when a new scene is loaded."));
            if (EditorGUI.EndChangeCheck())
            {
            }

            GUILayout.Space(5f);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propAutoLoadEnabled, new GUIContent("Auto Load Enabled", "When checked, the ad will be loaded when the scene starts."));
            if (EditorGUI.EndChangeCheck())
            {
            }

            GUILayout.Space(5f);

            // Error message for empty Ad Unit ID
            if (string.IsNullOrEmpty(propAndroidAdUnitId.stringValue) &&
                string.IsNullOrEmpty(propiOSAdUnitId.stringValue))
            {
                EditorGUILayout.HelpBox("Please specify the ad unit ID for at least one platform.", MessageType.Error);
            }

            GUILayout.Space(5f);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(iconDelete, styleBtnDelete))
            {
                if(EditorUtility.DisplayDialog("Remove this ad placement?",
                "Removing this ad placement will not affect your code. " +
                "You will need to manually remove any references to " +
                propPlacementName.stringValue,
                "Remove", "Cancel")) {
                    Undo.RecordObject(target, "Removed Ad Placement");
                    RemoveAdPlacement(id);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        void ShowNewAdPlacementButton()
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add New Placement"))
            {
                Undo.RecordObject(target, "Added Placement");
                propAllPlacements.InsertArrayElementAtIndex(propAllPlacements.arraySize);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        void RemoveAdPlacement(int id)
        {
            propAllPlacements.DeleteArrayElementAtIndex(id);
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }
    }
}
