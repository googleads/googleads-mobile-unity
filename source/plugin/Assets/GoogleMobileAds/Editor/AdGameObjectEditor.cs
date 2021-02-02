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

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GoogleMobileAds.Placement;

namespace GoogleMobileAds.Editor
{
    public class AdGameObjectEditor : UnityEditor.Editor
    {
        protected bool showCallbacks = false;
        private SerializedProperty propAdType;
        private SerializedProperty propAndroidAdUnitId;

        private SerializedProperty propIOSAdUnitId;

        private SerializedProperty propPersistent;

        private SerializedProperty propAutoLoadEnabled;
        private SerializedProperty propSelectedPlacementIndex;
        private List<AdPlacement> allAdPlacements;
        private List<string> placementNames;
        private bool shouldUpdateAdPlacement = true;
        private GUIStyle styleSubtitle;

        public virtual void OnEnable()
        {
            propAdType = serializedObject.FindProperty("adType");
            propAndroidAdUnitId = serializedObject.FindProperty("androidAdUnitId");
            propIOSAdUnitId = serializedObject.FindProperty("iOSAdUnitId");
            propPersistent = serializedObject.FindProperty("persistent");
            propAutoLoadEnabled = serializedObject.FindProperty("autoLoadEnabled");
            propSelectedPlacementIndex = serializedObject.FindProperty("selectedPlacementIndex");

            // Setup styles
            styleSubtitle = new GUIStyle();
            styleSubtitle.fontSize = 10;
            styleSubtitle.normal.textColor = Color.gray;
            styleSubtitle.padding.right = 8;
            styleSubtitle.alignment = TextAnchor.MiddleRight;

            UpdateAdList();
        }

        public override void OnInspectorGUI()
        {
            // Update this object because the data may have been changed from AdMobConfig asset
            serializedObject.Update();

            if (allAdPlacements != null && allAdPlacements.Count > 0)
            {
                if (shouldUpdateAdPlacement)
                {
                    UpdateAdList();
                    if (propSelectedPlacementIndex.intValue > allAdPlacements.Count)
                    {
                        propSelectedPlacementIndex.intValue = 0;
                    }
                    // Get the configuration from selected index
                    AdPlacement selectedPlacement = allAdPlacements[propSelectedPlacementIndex.intValue];
                    propAndroidAdUnitId.stringValue = selectedPlacement.androidAdUnitId;
                    propIOSAdUnitId.stringValue = selectedPlacement.iOSAdUnitId;
                    propPersistent.boolValue = selectedPlacement.persistent;
                    propAutoLoadEnabled.boolValue = selectedPlacement.autoLoadEnabled;
                    serializedObject.ApplyModifiedProperties();
                    shouldUpdateAdPlacement = false;
                }
            }
            else
            {
                EditorGUILayout.Separator();
                EditorGUILayout.HelpBox("No Ad Placements set.", MessageType.Error);
                if (GUILayout.Button("Create New Ad Placement"))
                {
                    GoogleMobileAdsPlacementsEditor.SelectAdPlacementsAsset();
                }
                return;
            }

            // Ad Placement Selection Dropdown
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Separator();
            int selected = EditorGUILayout.Popup("Ad Placement", propSelectedPlacementIndex.intValue, placementNames.ToArray());
            if (selected != propSelectedPlacementIndex.intValue)
            {
                // Update configuration only when a new placement is selected
                Undo.RecordObject(target, "Selected Ad Placement");
                propSelectedPlacementIndex.intValue = selected;
                shouldUpdateAdPlacement = true;
            }

            EditorGUILayout.LabelField("Ad Type: " + propAdType.enumNames[propAdType.enumValueIndex], styleSubtitle);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        void UpdateAdList()
        {
            if (shouldUpdateAdPlacement)
            {
                allAdPlacements = GoogleMobileAdsPlacements.Instance.allPlacements.FindAll(
                  placement => (int)placement.adType == propAdType.enumValueIndex
                );

                placementNames = new List<string>();

                for (int i = 0; i < allAdPlacements.Count; i++)
                {
                    placementNames.Add(allAdPlacements[i].placementName);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
