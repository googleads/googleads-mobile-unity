// <copyright file="SettingsDialog.cs" company="Google Inc.">
// Copyright (C) 2015 Google Inc. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>
#if UNITY_ANDROID

namespace GooglePlayServices
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Settings dialog for PlayServices Resolver.
    /// </summary>
    public class SettingsDialog : EditorWindow
    {

        bool mEnableAutoResolution;

        public void OnEnable()
        {
            mEnableAutoResolution =
                EditorPrefs.GetBool("GooglePlayServices.AutoResolverEnabled", true);
        }

        /// <summary>
        /// Called when the GUI should be rendered.
        /// </summary>
        public void OnGUI()
        {
            GUI.skin.label.wordWrap = true;
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Enable Background resolution", EditorStyles.boldLabel);
            mEnableAutoResolution = EditorGUILayout.Toggle(mEnableAutoResolution);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button("OK"))
            {
                EditorPrefs.SetBool(
                    "GooglePlayServices.AutoResolverEnabled",
                    mEnableAutoResolution);
                Close();
            }
        }
    }
}
#endif
