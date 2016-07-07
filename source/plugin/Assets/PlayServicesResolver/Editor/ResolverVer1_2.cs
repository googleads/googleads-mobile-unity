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
    using UnityEngine;
    using UnityEditor;

    [InitializeOnLoad]
    public class ResolverVer1_2 : ResolverVer1_1
    {

        private const int MajorVersion = 1;
        private const int MinorVersion = 2;
        private const int PointVersion = 0;

        static ResolverVer1_2()
        {
            PlayServicesResolver.RegisterResolver(new ResolverVer1_2());
            Debug.Log("Registering resolver version " + MajorVersion + "." +
                MinorVersion + "." + PointVersion);
            PlayServicesResolver.RegisterResolver(new ResolverVer1_2());

        }
        /// <summary>
        /// Version of the resolver.
        /// </summary>
        /// <remarks>The resolver with the greatest version is used when resolving.
        /// The value of the verison is calcuated using MakeVersion in DefaultResolver</remarks>
        /// <seealso cref="DefaultResolver.MakeVersionNumber"></seealso>
        public override int Version()
        {
            return MakeVersionNumber(MajorVersion, MinorVersion, PointVersion);
        }

        /// <summary>
        /// Shoulds the explode the aar file.
        /// </summary>
        /// <returns><c>true</c>, if explode was shoulded, <c>false</c> otherwise.</returns>
        /// <param name="aarFile">The aar file.</param>
        internal override bool ShouldExplode(string aarFile)
        {
            return base.ShouldExplode(aarFile) ||
            aarFile.Contains("firebase-iid") ||
            aarFile.Contains("firebase-common") ||
            aarFile.Contains("firebase-measurement") ||
            !SupportsAarFiles;
        }
    }
}
#endif