// <copyright file="ResolverVer1_1.cs" company="Google Inc.">
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
    using System.Collections.Generic;
    using Google.JarResolver;
    using System.IO;

    [InitializeOnLoad]
    public class ResolverVer1_1 : DefaultResolver
    {

        private const int MajorVersion = 1;
        private const int MinorVersion = 1;
        private const int PointVersion = 0;

        static ResolverVer1_1()
        {
            PlayServicesResolver.RegisterResolver(new ResolverVer1_1());
        }

        #region IResolver implementation

        /// <summary>
        /// Version of the resolver. - 1.1.0
        /// </summary>
        /// <remarks>The resolver with the greatest version is used when resolving.
        /// The value of the verison is calcuated using MakeVersion in DefaultResolver</remarks>
        /// <seealso cref="DefaultResolver.MakeVersionNumber"></seealso>
        public override int Version()
        {
            return MakeVersionNumber(MajorVersion, MinorVersion, PointVersion);
        }

        /// <summary>
        /// Perform the resolution and the exploding/cleanup as needed.
        /// </summary>
        public override void DoResolution(PlayServicesSupport svcSupport,
                                          string destinationDirectory,
                                          PlayServicesSupport.OverwriteConfirmation handleOverwriteConfirmation)
        {
            // Get the collection of dependencies that need to be copied.
            Dictionary<string, Dependency> deps =
                svcSupport.ResolveDependencies(true);

            // Copy the list
            svcSupport.CopyDependencies(deps,
                destinationDirectory,
                handleOverwriteConfirmation);

            // we want to look at all the .aars to decide to explode or not.
            // Some aars have variables in their AndroidManifest.xml file,
            // e.g. ${applicationId}.  Unity does not understand how to process
            // these, so we handle it here.
            ProcessAars(destinationDirectory);
        }

        #endregion

        /// <summary>
        /// Processes the aars.
        /// </summary>
        /// <remarks>Each aar copied is inspected and determined if it should be
        /// exploded into a directory or not. Unneeded exploded directories are
        /// removed.
        /// <para>
        /// Exploding is needed if the version of Unity is old, or if the artifact
        /// has been explicitly flagged for exploding.  This allows the subsequent
        /// processing of variables in the AndroidManifest.xml file which is not
        /// supported by the current versions of the manifest merging process that
        /// Unity uses.
        /// </para>
        /// <param name="dir">The directory to process.</param>
        void ProcessAars(string dir)
        {
            string[] files = Directory.GetFiles(dir, "*.aar");
            foreach (string f in files)
            {
                if (ShouldExplode(f))
                {
                    string exploded = ProcessAar(Path.GetFullPath(dir), f);
                    ReplaceVariables(exploded);
                }
                else
                {
                    string baseName = Path.GetFileNameWithoutExtension(f);
                    if (Directory.Exists(Path.Combine(dir, baseName)))
                    {
                        DeleteFully(Path.Combine(dir, baseName));
                    }
                }
            }
        }

        /// <summary>
        /// Shoulds the explode the aar file.
        /// </summary>
        /// <returns><c>true</c>, if explode was shoulded, <c>false</c> otherwise.</returns>
        /// <param name="aarFile">The aar file.</param>
        internal virtual bool ShouldExplode(string aarFile)
        {
            return aarFile.Contains("play-services-measurement") ||
            !SupportsAarFiles;
        }

        /// <summary>
        /// Replaces the variables in the AndroidManifest file.
        /// </summary>
        /// <param name="exploded">Exploded.</param>
        void ReplaceVariables(string exploded)
        {
            string manifest = Path.Combine(exploded, "AndroidManifest.xml");
            if (File.Exists(manifest))
            {
                StreamReader sr = new StreamReader(manifest);
                string body = sr.ReadToEnd();
                sr.Close();

                body = body.Replace("${applicationId}", PlayerSettings.bundleIdentifier);

                using (var wr = new StreamWriter(manifest, false))
                {
                    wr.Write(body);
                }
            }
        }
    }
}
#endif
