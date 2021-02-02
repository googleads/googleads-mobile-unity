// Copyright (C) 2020 Google LLC
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
using GoogleMobileAds.Placement;

namespace GoogleMobileAds.Editor
{
    [CustomEditor(typeof(RewardedInterstitialAdGameObject))]
    [CanEditMultipleObjects]
    public class RewardedInterstitialAdGameObjectEditor : AdGameObjectEditor
    {

        public override void OnEnable()
        {
            serializedObject.FindProperty("adType").enumValueIndex = (int)AdPlacement.AdType.RewardedInterstitial;
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
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onAdFailedToPresentFullScreenContent"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onAdDidPresentFullScreenContent"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onUserEarnedReward"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onAdDidDismissFullScreenContent"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
