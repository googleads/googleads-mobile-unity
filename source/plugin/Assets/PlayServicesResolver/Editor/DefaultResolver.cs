// <copyright file="DefaultResolver.cs" company="Google Inc.">
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
    using Google.JarResolver;
    using System.IO;
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Default resolver base class.
    /// </summary>
    /// <remarks> This class contains the default implementation of the
    /// standard methods used to resolve the play-services dependencies.
    /// The intention is that common, stable methods are implemented here, and
    /// subsequent versions of the resolver would extend this class to modify the
    /// behavior.
    /// </remarks>
    public  abstract class DefaultResolver : IResolver
    {
        #region IResolver implementation

        /// <summary>
        /// Version of the resolver - 1.0.0
        /// </summary>
        public virtual int Version()
        {
            return MakeVersionNumber(1, 0, 0);
        }

        /// <summary>
        /// Enables automatic resolution.
        /// </summary>
        /// <param name="flag">If set to <c>true</c> flag.</param>
        public virtual void SetAutomaticResolutionEnabled(bool flag)
        {
            EditorPrefs.GetBool("GooglePlayServices.AutoResolverEnabled", flag);
        }

        /// <summary>
        /// Returns true if automatic resolution is enabled.
        /// </summary>
        /// <returns><c>true</c>, if resolution enabled was automaticed, <c>false</c> otherwise.</returns>
        public virtual bool AutomaticResolutionEnabled()
        {
            return EditorPrefs.GetBool("GooglePlayServices.AutoResolverEnabled",
                SupportsAarFiles);
        }

        /// <summary>
        /// Returns true if Android package installation is enabled.
        /// </summary>
        /// <returns><c>true</c>, package installation is enabled, <c>false</c> otherwise.
        /// </returns>
        public virtual bool AndroidPackageInstallationEnabled()
        {
            return EditorPrefs.GetBool("GooglePlayServices.AndroidPackageInstallationEnabled",
                                       true);
        }

        /// <summary>
        /// Checks based on the asset changes, if resolution should occur.
        /// </summary>
        /// <remarks>
        /// The resolution only happens if a script file (.cs, or .js) was imported
        /// or if an Android plugin was deleted.  This allows for changes to
        /// assets that do not affect the dependencies to happen without processing.
        /// This also avoids an infinite loop when a version of a dependency is
        /// deleted during resolution.
        /// </remarks>
        /// <returns><c>true</c>, if auto resolution should happen, <c>false</c> otherwise.</returns>
        /// <param name="importedAssets">Imported assets.</param>
        /// <param name="deletedAssets">Deleted assets.</param>
        /// <param name="movedAssets">Moved assets.</param>
        /// <param name="movedFromAssetPaths">Moved from asset paths.</param>
        public virtual bool ShouldAutoResolve(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (AutomaticResolutionEnabled())
            {
                // look for imported scripts
                foreach (string s in importedAssets)
                {
                    if (s.EndsWith(".cs") || s.EndsWith(".js"))
                    {
                        Debug.Log(s + " imported, resolving play-services");
                        return true;
                    }
                }

                // look for deleted android plugins
                foreach (string s in deletedAssets)
                {
                    if (s.StartsWith("Assets/Plugins/Android"))
                    {
                        Debug.Log(s + " deleted, resolving play-services");
                        return true;
                    }
                }
                // don't resolve if assets are moved around.
            }
            return false;
        }

        /// <summary>
        /// Shows the settings dialog.
        /// </summary>
        public virtual void ShowSettingsDialog()
        {

            SettingsDialog window = (SettingsDialog)EditorWindow.GetWindow(
                typeof(SettingsDialog), true, "Play Services Resolver Settings");
            window.Initialize();
            window.Show();
        }

        /// <summary>
        /// Does the resolution of the play-services aars.
        /// </summary>
        /// <param name="svcSupport">Svc support.</param>
        /// <param name="destinationDirectory">Destination directory.</param>
        /// <param name="handleOverwriteConfirmation">Handle overwrite confirmation.</param>
        /// <param name="resolutionComplete">Delegate called when resolution is complete.</param>
        public virtual void DoResolution(
            PlayServicesSupport svcSupport, string destinationDirectory,
            PlayServicesSupport.OverwriteConfirmation handleOverwriteConfirmation,
            System.Action resolutionComplete)
        {
            DoResolution(svcSupport, destinationDirectory, handleOverwriteConfirmation);
            resolutionComplete();
        }

        /// <summary>
        /// Called during Update to allow the resolver to check the bundle ID of the application
        /// to see whether resolution should be triggered again.
        /// </summary>
        /// <returns>Array of packages that should be re-resolved if resolution should occur,
        /// null otherwise.</returns>
        public virtual string[] OnBundleId(string bundleId)
        {
            return null;
        }

        #endregion

        /// <summary>
        /// Compatibility method for synchronous implementations of DoResolution().
        /// </summary>
        /// <param name="svcSupport">Svc support.</param>
        /// <param name="destinationDirectory">Destination directory.</param>
        /// <param name="handleOverwriteConfirmation">Handle overwrite confirmation.</param>
        public abstract void DoResolution(
            PlayServicesSupport svcSupport, string destinationDirectory,
            PlayServicesSupport.OverwriteConfirmation handleOverwriteConfirmation);

        /// <summary>
        /// Makes the version number.
        /// </summary>
        /// <remarks>This combines the major/minor/point version components into
        /// an integer.  If multiple resolvers are registered, then the greatest version
        /// is used.
        /// </remarks>
        /// <returns>The version number.</returns>
        /// <param name="maj">Maj.</param>
        /// <param name="min">Minimum.</param>
        /// <param name="pt">Point.</param>
        internal int MakeVersionNumber(int maj, int min, int pt)
        {
            return maj * 10000 + min + 100 + pt;
        }

        /// <summary>
        /// Gets a value indicating whether this version of Unity supports aar files.
        /// </summary>
        /// <value><c>true</c> if supports aar files; otherwise, <c>false</c>.</value>
        internal bool SupportsAarFiles
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
        /// Find a Java tool.
        /// </summary>
        /// <param name="toolName">Name of the tool to search for.</param>
        private string FindJavaTool(string javaTool)
        {
            string javaHome = Environment.GetEnvironmentVariable("JAVA_HOME");
            string toolPath;
            if (javaHome != null)
            {
                toolPath = Path.Combine(
                    javaHome, Path.Combine(
                        "bin", javaTool + CommandLine.GetExecutableExtension()));
                if (!File.Exists(toolPath))
                {
                    EditorUtility.DisplayDialog("Play Services Dependencies",
                                                "JAVA_HOME environment references a directory (" +
                                                javaHome + ") that does not contain " + javaTool +
                                                " which is required to process Play Services " +
                                                "dependencies.", "OK");
                    throw new Exception("JAVA_HOME references incomplete Java distribution.  " +
                                        javaTool + " not found.");
                }
            } else {
                toolPath = CommandLine.FindExecutable(javaTool);
                if (!File.Exists(toolPath))
                {
                    EditorUtility.DisplayDialog("Play Services Dependencies",
                                                "Unable to find " + javaTool + " in the system " +
                                                "path.  This tool is required to process Play " +
                                                "Services dependencies.  Either set JAVA_HOME " +
                                                "or add " + javaTool + " to the PATH variable " +
                                                "to resolve this error.", "OK");
                    throw new Exception(javaTool + " not found.");
                }
            }
            return toolPath;
        }

        /// <summary>
        /// Create a temporary directory.
        /// </summary>
        /// <returns>If temporary directory creation fails, return null.</returns>
        public static string CreateTemporaryDirectory()
        {
            int retry = 100;
            while (retry-- > 0)
            {
                string temporaryDirectory = Path.Combine(Path.GetTempPath(),
                                                         Path.GetRandomFileName());
                if (File.Exists(temporaryDirectory))
                {
                    continue;
                }
                Directory.CreateDirectory(temporaryDirectory);
                return temporaryDirectory;
            }
            return null;
        }

        // store the AndroidManifest.xml in a temporary directory before processing it.
        /// <summary>
        /// Extract an AAR to the specified directory.
        /// </summary>
        /// <param name="aarFile">Name of the AAR file to extract.</param>
        /// <param name="extract_filenames">List of files to extract from the AAR.  If this array
        /// is empty or null all files are extracted.</param>
        /// <param name="outputDirectory">Directory to extract the AAR file to.</param>
        /// <returns>true if successful, false otherwise.</returns>
        internal virtual bool ExtractAar(string aarFile, string[] extractFilenames,
                                         string outputDirectory)
        {
            try
            {
                string aarPath = Path.GetFullPath(aarFile);
                string extractFilesArg = extractFilenames != null && extractFilenames.Length > 0 ?
                    " \"" + String.Join("\" \"", extractFilenames) + "\"" : "";
                CommandLine.Result result = CommandLine.Run(FindJavaTool("jar"),
                                                            "xvf " + "\"" + aarPath + "\"" +
                                                            extractFilesArg,
                                                            workingDirectory: outputDirectory);
                if (result.exitCode != 0)
                {
                    Debug.LogError("Error expanding " + aarPath + " err: " +
                                   result.exitCode + ": " + result.stderr);
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw e;
            }
            return true;
        }

        /// <summary>
        /// Explodes a single aar file.  This is done by calling the
        /// JDK "jar" command, then moving the classes.jar file.
        /// </summary>
        /// <param name="dir">the parent directory of the plugin.</param>
        /// <param name="aarFile">Aar file to explode.</param>
        /// <returns>The path to the exploded aar.
        internal virtual string ProcessAar(string dir, string aarFile)
        {
            string workingDir = Path.Combine(dir, Path.GetFileNameWithoutExtension(aarFile));
            Directory.CreateDirectory(workingDir);
            if (!ExtractAar(aarFile, null, workingDir)) return workingDir;

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

                File.WriteAllLines(Path.Combine(workingDir, "project.properties"),
                                   props);
            }

            // Clean up the aar file.
            File.Delete(Path.GetFullPath(aarFile));
            Debug.Log(aarFile + " expanded successfully");
            return workingDir;
        }
    }
}
#endif
