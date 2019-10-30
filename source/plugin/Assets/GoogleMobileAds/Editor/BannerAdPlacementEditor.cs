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

using System;

using UnityEditor;
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Editor
{
    [CustomEditor(typeof(BannerAdPlacement))]
    [CanEditMultipleObjects]
    public class BannerAdPlacementEditor : UnityEditor.Editor
    {

        private bool showGeneral = true;

        private bool showAppearance = true;

        private bool showCallbacks = false;

        private SerializedProperty propAndroidAdUnitId;

        private SerializedProperty propIOSAdUnitId;

        private SerializedProperty propAdSize;

        private SerializedProperty propAdPosition;

        private SerializedProperty propAdPostionOffset;

        private SerializedProperty propAutoLoadEnabled;

        private SerializedProperty propDontDestroyOnNewScene;

        void OnEnable()
        {
            propAndroidAdUnitId = serializedObject.FindProperty("androidAdUnitId");
            propIOSAdUnitId = serializedObject.FindProperty("iOSAdUnitId");
            propAdSize = serializedObject.FindProperty("adSize");
            propAdPosition = serializedObject.FindProperty("adPosition");
            propAdPostionOffset = serializedObject.FindProperty("adPositionOffset");
            propAutoLoadEnabled = serializedObject.FindProperty("autoLoadEnabled");
            propDontDestroyOnNewScene = serializedObject.FindProperty("dontDestroyOnNewScene");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Separator();

            showGeneral = EditorGUILayout.Foldout(showGeneral, "Ad placement settings");
            if (showGeneral)
            {
                EditorGUILayout.LabelField("Ad unit ID");
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(propAndroidAdUnitId, new GUIContent("Android"));
                EditorGUILayout.PropertyField(propIOSAdUnitId, new GUIContent("iOS"));
                if (NoAdUnitIDsSpecified())
                {
                    EditorGUILayout.HelpBox("Please specify the ad unit ID for at least one platform.", MessageType.Error);
                }
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Separator();
            showAppearance = EditorGUILayout.Foldout(showAppearance, "Banner configuration");
            if (showAppearance)
            {
                EditorGUILayout.PropertyField(propAdSize);
                EditorGUILayout.PropertyField(propAdPosition);
                
                AdPosition currPosition = (AdPosition) Enum.ToObject(typeof(AdPosition), propAdPosition.intValue);
                if (currPosition == AdPosition.Custom)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(propAdPostionOffset, new GUIContent("Offset (dp)"));
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.PropertyField(propAutoLoadEnabled, new GUIContent("Auto Load Enabled"));
                EditorGUILayout.PropertyField(
                    propDontDestroyOnNewScene, new GUIContent("Don't destroy on new scene"));
            }

            EditorGUILayout.Separator();
            showCallbacks = EditorGUILayout.Foldout(showCallbacks, "Callbacks");
            if (showCallbacks)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("AdLoaded"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("AdFailedToLoad"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("AdOpening"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("AdClosed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("AdLeavingApplication"));
            }

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private bool NoAdUnitIDsSpecified()
        {
            return String.IsNullOrEmpty(propAndroidAdUnitId.stringValue) &&
                String.IsNullOrEmpty(propIOSAdUnitId.stringValue);
        }
    }
}
