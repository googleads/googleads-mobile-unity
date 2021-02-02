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
using UnityEngine;
using UnityEditor;
using GoogleMobileAds.Placement;

namespace GoogleMobileAds.Editor
{
    [CustomEditor(typeof(InterstitialAdGameObject))]
    [CanEditMultipleObjects]
    public class InterstitialAdGameObjectEditor : AdGameObjectEditor
    {
        private SerializedProperty propAdType;
        public override void OnEnable()
        {
            propAdType = serializedObject.FindProperty("adType");
            propAdType.enumValueIndex = (int)AdPlacement.AdType.Interstitial;
            base.OnEnable();
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Separator();
            showCallbacks = EditorGUILayout.Foldout(showCallbacks, "Callbacks");
            if (showCallbacks)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onAdLoaded"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onAdFailedToLoad"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onAdOpening"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onAdClosed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onAdLeavingApplication"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
