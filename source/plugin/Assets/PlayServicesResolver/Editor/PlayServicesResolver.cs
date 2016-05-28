// <copyright file="PlayServicesResolver.cs" company="Google Inc.">
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
    using System;
    using Google.JarResolver;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Play services resolver.  This is a background post processor
    /// that copies over the Google play services .aar files that
    /// plugins have declared as dependencies.  If the Unity version is less than
    /// 5, aar files are not supported so this class 'explodes' the aar file into
    /// a plugin directory.  Once the version of Unity is upgraded, the exploded
    /// files are removed in favor of the .aar files.
    /// </summary>
    [InitializeOnLoad]
    public class PlayServicesResolver : AssetPostprocessor
    {
        /// <summary>
        /// The instance to the play services support object.
        /// </summary>
        private static PlayServicesSupport svcSupport;

        /// <summary>
        /// The resolver to use, injected to allow for version updating.
        /// </summary>
        private static IResolver _resolver;

        /// <summary>
        /// Initializes the <see cref="GooglePlayServices.PlayServicesResolver"/> class.
        /// </summary>
        static PlayServicesResolver()
        {
            svcSupport = PlayServicesSupport.CreateInstance(
                "PlayServicesResolver",
                EditorPrefs.GetString("AndroidSdkRoot"),
                "ProjectSettings");
        }

        /// <summary>
        /// Registers the resolver.
        /// </summary>
        /// <remarks>
        /// The resolver with the greatest version number is retained
        /// </remarks>
        /// <returns>The resolver.</returns>
        /// <param name="resolverImpl">Resolver impl.</param>
        public static IResolver RegisterResolver(IResolver resolverImpl)
        {
            if (resolverImpl == null)
            {
                return _resolver;
            }
            if (_resolver == null || _resolver.Version() < resolverImpl.Version())
            {
                _resolver = resolverImpl;
            }
            Debug.Log("Resolver version is now: " + _resolver.Version());
            return _resolver;
        }

        /// <summary>
        /// Gets the resolver.
        /// </summary>
        /// <value>The resolver.</value>
        public static IResolver Resolver
        {
            get
            {
                return _resolver;
            }
        }

        /// <summary>
        /// Called by Unity when all assets have been updated. This
        /// is used to kick off resolving the dependendencies declared.
        /// </summary>
        /// <param name="importedAssets">Imported assets. (unused)</param>
        /// <param name="deletedAssets">Deleted assets. (unused)</param>
        /// <param name="movedAssets">Moved assets. (unused)</param>
        /// <param name="movedFromAssetPaths">Moved from asset paths. (unused)</param>
        static void OnPostprocessAllAssets(string[] importedAssets,
                                           string[] deletedAssets,
                                           string[] movedAssets,
                                           string[] movedFromAssetPaths)
        {
            if (!Resolver.ShouldAutoResolve(importedAssets, deletedAssets,
                    movedAssets, movedFromAssetPaths))
            {
                return;
            }

            Resolver.DoResolution(svcSupport,
                "Assets/Plugins/Android",
                HandleOverwriteConfirmation);

            AssetDatabase.Refresh();
            Debug.Log("Android Jar Dependencies: Resolution Complete");
        }

        /// <summary>
        /// Add a menu item for resolving the jars manually.
        /// </summary>
        [MenuItem("Assets/Google Play Services/Settings")]
        public static void SettingsDialog()
        {
            Resolver.ShowSettingsDialog();
        }

        /// <summary>
        /// Add a menu item for resolving the jars manually.
        /// </summary>
        [MenuItem("Assets/Google Play Services/Resolve Client Jars")]
        public static void MenuResolve()
        {
            Resolver.DoResolution(svcSupport, "Assets/Plugins/Android", HandleOverwriteConfirmation);

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Android Jar Dependencies",
                "Resolution Complete", "OK");
        }

        /// <summary>
        /// Handles the overwrite confirmation.
        /// </summary>
        /// <returns><c>true</c>, if overwrite confirmation was handled, <c>false</c> otherwise.</returns>
        /// <param name="oldDep">Old dependency.</param>
        /// <param name="newDep">New dependency replacing old.</param>
        public static bool HandleOverwriteConfirmation(Dependency oldDep, Dependency newDep)
        {
            // Don't prompt overwriting the same version, just do it.
            if (oldDep.BestVersion != newDep.BestVersion)
            {
                string msg = "Remove or replace " + oldDep.Artifact + " version " +
                             oldDep.BestVersion + " with version " + newDep.BestVersion + "?";
                return EditorUtility.DisplayDialog("Android Jar Dependencies",
                    msg, "OK", "Keep");
            }
            return true;
        }
    }
}
#endif
