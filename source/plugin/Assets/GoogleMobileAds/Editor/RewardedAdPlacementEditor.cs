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

using GoogleMobileAds.Common;

[CustomEditor(typeof(RewardedAdPlacement))]
[CanEditMultipleObjects]
public class RewardedAdPlacementEditor : Editor
{
    private bool showGeneral = true;

    private bool showCallbacks = false;

    private SerializedProperty propAndroidAdUnitId;

    private SerializedProperty propIOSAdUnitId;

    private SerializedProperty propAdLoaded;

    private SerializedProperty propAdFailedToLoad;

    private SerializedProperty propAdFailedToShow;

    private SerializedProperty propAdOpening;

    private SerializedProperty propUserEanredReward;

    private SerializedProperty propAdClosed;

    void OnEnable()
    {
        propAndroidAdUnitId = serializedObject.FindProperty("androidAdUnitId");
        propIOSAdUnitId = serializedObject.FindProperty("iOSAdUnitId");
        propAdLoaded = serializedObject.FindProperty("AdLoaded");
        propAdFailedToLoad = serializedObject.FindProperty("AdFailedToLoad");
        propAdFailedToShow = serializedObject.FindProperty("AdFailedToShow");
        propAdOpening = serializedObject.FindProperty("AdOpening");
        propUserEanredReward = serializedObject.FindProperty("UserEarnedReward");
        propAdClosed = serializedObject.FindProperty("AdClosed");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Separator();

        showGeneral = EditorGUILayout.Foldout(showGeneral, "Ad placement settings");
        if (showGeneral)
        {
            EditorGUILayout.LabelField("Ad unit ID");
            EditorGUILayout.PropertyField(propAndroidAdUnitId, new GUIContent("  Android"));
            EditorGUILayout.PropertyField(propIOSAdUnitId, new GUIContent("  iOS"));
            if (NoAdUnitIDsSpecified())
            {
                EditorGUILayout.HelpBox("Please specify the ad unit ID for at least one platform.", MessageType.Error);
            }
        }

        EditorGUILayout.Separator();
        showCallbacks = EditorGUILayout.Foldout(showCallbacks, "Callbacks");
        if (showCallbacks)
        {
            EditorGUILayout.PropertyField(propAdLoaded);
            EditorGUILayout.PropertyField(propAdFailedToLoad);
            EditorGUILayout.PropertyField(propAdFailedToShow);
            EditorGUILayout.PropertyField(propAdOpening);
            EditorGUILayout.PropertyField(propUserEanredReward);
            EditorGUILayout.PropertyField(propAdClosed);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private bool NoAdUnitIDsSpecified()
    {
        return String.IsNullOrEmpty(propAndroidAdUnitId.stringValue) &&
            String.IsNullOrEmpty(propIOSAdUnitId.stringValue);
    }
}