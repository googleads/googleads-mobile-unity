// <copyright file="IResolver.cs" company="Google Inc.">
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
    using Google.JarResolver;

    public interface IResolver
    {
        /// <summary>
        /// Version of the resolver.
        /// </summary>
        /// <remarks>
        /// The resolver with the greatest version is used when resolving.
        /// The value of the verison is calcuated using MakeVersion in DefaultResolver
        /// </remarks>
        /// <seealso cref="DefaultResolver.MakeVersionNumber"/>
        int Version();

        /// <summary>
        /// Returns true if automatic resolution is enabled.
        /// </summary>
        /// <returns><c>true</c>, if resolution is enabled <c>false</c> otherwise.</returns>
        bool AutomaticResolutionEnabled();

        /// <summary>
        /// Sets the automatic resolution flag.
        /// </summary>
        /// <param name="flag">If set to <c>true</c> flag.</param>
        void SetAutomaticResolutionEnabled(bool flag);

        /// <summary>
        /// Checks based on the asset changes, if resolution should occur.
        /// </summary>
        /// <remarks>
        /// Resolution should only happen when needed, and avoid infinite loops
        /// of automatic resolution triggered by resolution actions.
        /// </remarks>
        /// <returns><c>true</c>, if auto resolution should happen, <c>false</c> otherwise.</returns>
        /// <param name="importedAssets">Imported assets.</param>
        /// <param name="deletedAssets">Deleted assets.</param>
        /// <param name="movedAssets">Moved assets.</param>
        /// <param name="movedFromAssetPaths">Moved from asset paths.</param>
        bool ShouldAutoResolve(string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths);

        /// <summary>
        /// Shows the settings dialog.
        /// </summary>
        void ShowSettingsDialog();

        /// <summary>
        /// Does the resolution of the play-services aars.
        /// </summary>
        /// <param name="svcSupport">Svc support.</param>
        /// <param name="destinationDirectory">Destination directory.</param>
        /// <param name="handleOverwriteConfirmation">Handle overwrite confirmation.</param>
        void DoResolution(PlayServicesSupport svcSupport,
                          string destinationDirectory,
                          PlayServicesSupport.OverwriteConfirmation handleOverwriteConfirmation);
    }
}
#endif
