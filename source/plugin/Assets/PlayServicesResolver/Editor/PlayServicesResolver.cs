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
    using System.Collections.Generic;
    using System.IO;
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
        /// Gets a value indicating whether this <see cref="GooglePlayServices.PlayServicesResolver"/> supports aar files.
        /// </summary>
        /// <value><c>true</c> if supports aar files; otherwise, <c>false</c>.</value>
        static bool SupportsAarFiles
        {
            get
            {
                // Get the version number.
                string majorVersion = Application.unityVersion.Split('.')[0];
                int ver;
                if (!int.TryParse(majorVersion, out ver))
                {
                    #if UNITY_4
                        ver = 4;
                    #else
                    ver = 5;
                    #endif
                }

                return ver >= 5;
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
            DoResolution();
            AssetDatabase.Refresh();
            Debug.Log("Android Jar Dependencies: Resolution Complete");
        }

        /// <summary>
        /// Add a menu item for resolving the jars manually.
        /// </summary>
        [MenuItem("Assets/Google Play Services/Resolve Client Jars")]
        public static void MenuResolve()
        {
             DoResolution();

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Android Jar Dependencies",
                "Resolution Complete", "OK");
        }

        /// <summary>
        /// Perform the resolution and the exploding/cleanup as needed.
        /// </summary>
        static void DoResolution()
        {
            // Get the collection of dependencies that need to be copied.
            Dictionary<string, Dependency> deps =
                svcSupport.ResolveDependencies(true);

            // Copy the list
            svcSupport.CopyDependencies(deps,
                "Assets/Plugins/Android",
                HandleOverwriteConfirmation);

            // If aar files are not supported, explode them into directories.
            // otherwise clean up any exploded directories in favor of the aar files.
            if (!SupportsAarFiles)
            {
                Debug.Log("Exploding");
                // need to explode the .aar file in place.
                ExplodeAarFiles("Assets/Plugins/Android");
            }
            else
            {
                Debug.Log("Cleaning up exploded aars...");
                CleanupExploded("Assets/Plugins/Android");
            }
        }

        /// <summary>
        /// Handles the overwrite confirmation.
        /// </summary>
        /// <returns><c>true</c>, if overwrite confirmation was handled, <c>false</c> otherwise.</returns>
        /// <param name="oldDep">Old dependency.</param>
        /// <param name="newDep">New dependency replacing old.</param>
        static bool HandleOverwriteConfirmation(Dependency oldDep, Dependency newDep)
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

        /// <summary>
        /// Clean ups the exploded aar files by deleting the directory of the
        /// same name.
        /// </summary>
        /// <param name="dir">Directory to look for the aar files.</param>
        static void CleanupExploded(string dir)
        {
            string[] files = Directory.GetFiles(dir, "*.aar");
            foreach (string f in files)
            {
                string baseName = Path.GetFileNameWithoutExtension(f);
                if (Directory.Exists(Path.Combine(dir, baseName)))
                {
                    DeleteFully(Path.Combine(dir, baseName));
                }
            }
        }

        /// <summary>
        /// Deletes the directory fully.
        /// </summary>
        /// <param name="dir">Directory to delete.</param>
        static void DeleteFully(string dir)
        {
            string[] files = Directory.GetFiles(dir);
            string[] dirs = Directory.GetDirectories(dir);

            foreach (string f in files)
            {
                File.Delete(f);
            }

            foreach (string d in dirs)
            {
                DeleteFully(d);
            }

            Directory.Delete(dir);
        }

        /// <summary>
        /// Explodes the aar files into directories and deletes the aar file.
        /// </summary>
        /// <param name="dir">Dir.</param>
        static void ExplodeAarFiles(string dir)
        {
            string[] files = Directory.GetFiles(dir, "*.aar");
            foreach (string f in files)
            {
                ProcessAar(Path.GetFullPath(dir), f);
            }
        }

        /// <summary>
        /// Explodes a single aar file.  This is done by calling the
        /// JDK "jar" command, then moving the classes.jar file.
        /// </summary>
        /// <param name="dir">the parent directory of the plugin.</param>
        /// <param name="aarFile">Aar file to explode.</param>
        static void ProcessAar(string dir, string aarFile)
        {
            string file = Path.GetFileNameWithoutExtension(aarFile);
            string workingDir = Path.Combine(dir, file);
            Directory.CreateDirectory(workingDir);
            try
            {
				string exe = "jar";
				if(RuntimePlatform.WindowsEditor == Application.platform)
				{
					exe = Path.Combine( System.Environment.GetEnvironmentVariable("JAVA_HOME"), Path.Combine("bin","jar.exe"));
				}

                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.Arguments = "xvf " + Path.GetFullPath(aarFile);
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardOutput = false;
                p.StartInfo.RedirectStandardError = true;
				p.StartInfo.FileName = exe;
                p.StartInfo.WorkingDirectory = workingDir;
                p.Start();

                // To avoid deadlocks, always read the output stream first and then wait.
                string stderr = p.StandardError.ReadToEnd();

                p.WaitForExit();

                if (p.ExitCode == 0)
                {

                    // move the classes.jar file to libs.
                    string libDir = Path.Combine(workingDir, "libs");
                    if (!Directory.Exists(libDir))
                    {
                        Directory.CreateDirectory(libDir);
                    }
                    if (File.Exists(Path.Combine(libDir, "classes.jar")))
                    {
                        File.Delete(Path.Combine(libDir, "classes.jar"));
                    }
                    if (File.Exists(Path.Combine(workingDir, "classes.jar")))
                    {
                        File.Move(Path.Combine(workingDir, "classes.jar"),
                            Path.Combine(libDir, "classes.jar"));
                    }

                    // Create the project.properties file which indicates to
                    // Unity that this directory is a plugin.
                    if (!File.Exists(Path.Combine(workingDir, "project.properties")))
                    {
                        // write out project.properties
                        string[] props =
                            {
                        "# Project target.",
                        "target=android-9",
                        "android.library=true"
                    };

                    File.WriteAllLines(Path.Combine(workingDir,"project.properties"),
                        props);
                    }

                    // Clean up the aar file.
                    File.Delete(Path.GetFullPath(aarFile));

                    Debug.Log(aarFile + " expanded successfully");
                }
                else
                {
                    Debug.LogError("Error expanding " +
                        Path.GetFullPath(aarFile) +
                        " err: " + p.ExitCode + ": " + stderr);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw e;
            }
        }
    }
}
#endif
