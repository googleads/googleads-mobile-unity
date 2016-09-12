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
    using System.Collections.Generic;
    using Google.JarResolver;
    using System.Collections;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml;

    [InitializeOnLoad]
    public class ResolverVer1_1 : DefaultResolver
    {
        // Caches data associated with an aar so that it doesn't need to be queried to determine
        // whether it should be expanded / exploded if it hasn't changed.
        private class AarExplodeData
        {
            // Time the file was modified the last time it was inspected.
            public System.DateTime modificationTime;
            // Whether the AAR file should be expanded / exploded.
            public bool explode = false;
            // Project's bundle ID when this was expanded.
            public string bundleId = "";
            // Path of the target AAR package.
            public string path = "";
        }

        private Dictionary<string, AarExplodeData> aarExplodeData =
            new Dictionary<string, AarExplodeData>();
        // File used to to serialize aarExplodeData.  This is required as Unity will reload classes
        // in the editor when C# files are modified.
        private string aarExplodeDataFile = Path.Combine("ProjectSettings",
                                                         "GoogleAarExplodeCache.xml");

        private const int MajorVersion = 1;
        private const int MinorVersion = 1;
        private const int PointVersion = 0;

        static ResolverVer1_1()
        {
            ResolverVer1_1 resolver = new ResolverVer1_1();
            resolver.LoadAarExplodeCache();
            PlayServicesResolver.RegisterResolver(resolver);
        }

        /// <summary>
        /// Load data cached in aarExplodeDataFile into aarExplodeData.
        /// </summary>
        private void LoadAarExplodeCache()
        {
            if (!File.Exists(aarExplodeDataFile)) return;

            XmlTextReader reader = new XmlTextReader(new StreamReader(aarExplodeDataFile));
            aarExplodeData.Clear();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "aars")
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "explodeData")
                        {
                            string aar = "";
                            AarExplodeData aarData = new AarExplodeData();
                            do
                            {
                                if (!reader.Read()) break;
                                if (reader.NodeType == XmlNodeType.Element)
                                {
                                    string elementName = reader.Name;
                                    if (reader.Read() && reader.NodeType == XmlNodeType.Text)
                                    {
                                        if (elementName == "aar")
                                        {
                                            aar = reader.ReadContentAsString();
                                        }
                                        else if (elementName == "modificationTime")
                                        {
                                            aarData.modificationTime =
                                                reader.ReadContentAsDateTime();
                                        }
                                        else if (elementName == "explode")
                                        {
                                            aarData.explode = reader.ReadContentAsBoolean();
                                        }
                                        else if (elementName == "bundleId")
                                        {
                                            aarData.bundleId = reader.ReadContentAsString();
                                        }
                                        else if (elementName == "path")
                                        {
                                            aarData.path = reader.ReadContentAsString();
                                        }
                                    }
                                }
                            } while (!(reader.Name == "explodeData" &&
                                       reader.NodeType == XmlNodeType.EndElement));
                            if (aar != "") aarExplodeData[aar] = aarData;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save data from aarExplodeData into aarExplodeDataFile.
        /// </summary>
        private void SaveAarExplodeCache()
        {
            if (File.Exists(aarExplodeDataFile))
            {
                File.Delete(aarExplodeDataFile);
            }
            XmlTextWriter writer = new XmlTextWriter(new StreamWriter(aarExplodeDataFile));
            writer.WriteStartElement("aars");
            foreach (KeyValuePair<string, AarExplodeData> kv in aarExplodeData)
            {
                writer.WriteStartElement("explodeData");
                writer.WriteStartElement("aar");
                writer.WriteValue(kv.Key);
                writer.WriteEndElement();
                writer.WriteStartElement("modificationTime");
                writer.WriteValue(kv.Value.modificationTime);
                writer.WriteEndElement();
                writer.WriteStartElement("explode");
                writer.WriteValue(kv.Value.explode);
                writer.WriteEndElement();
                writer.WriteStartElement("bundleId");
                writer.WriteValue(PlayerSettings.bundleIdentifier);
                writer.WriteEndElement();
                writer.WriteStartElement("path");
                writer.WriteValue(kv.Value.path);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// Find a tool in the Android SDK.
        /// </summary>
        /// <param name="svcSupport">PlayServicesSupport instance used to retrieve the SDK
        /// path. </param>
        /// <param name="toolName">Name of the tool to search for.</param>
        /// <returns>String with the path to the tool if found, null otherwise.</returns>
        internal static string FindAndroidSdkTool(PlayServicesSupport svcSupport, string toolName)
        {
            string toolPath = null;
            string sdkPath = svcSupport.SDK;
            if (sdkPath == null || sdkPath == "")
            {
                Debug.LogWarning(PlayServicesSupport.AndroidSdkConfigurationError +
                                 "  Will fallback to searching for " + toolName +
                                 " in the system path.");
            }
            else
            {
                toolPath = Path.Combine(
                    sdkPath, Path.Combine(
                        "tools", toolName + CommandLine.GetExecutableExtension()));
            }
            if (toolPath == null || !File.Exists(toolPath))
            {
                toolPath = CommandLine.FindExecutable(toolName);
            }
            return toolPath;
        }

        /// <summary>
        /// Generate an array from a string collection.
        /// </summary>
        /// <returns>An array of strings.</return>
        private static string[] CollectionToArray(ICollection enumerator)
        {
            return (string[])(new ArrayList(enumerator)).ToArray(typeof(string));
        }

        /// <summary>
        /// Delegate called when GetAvailablePackages() completes.
        /// </summary>
        internal delegate void GetAvailablePackagesComplete(Dictionary<string, bool> packages);

        // Answers Android SDK manager license questions.
        private class LicenseResponder : CommandLine.LineReader
        {
            private const string Question = "Do you accept the license";

            private string response;

            // Initialize the class to respond "yes" or "no" to license questions.
            public LicenseResponder(bool accept)
            {
                LineHandler += CheckAndRespond;
                response = accept ? "yes" : "no";
            }

            // Respond license questions with the "response".
            public void CheckAndRespond(System.Diagnostics.Process process, StreamWriter stdin,
                                        CommandLine.StreamData data)
            {
                if (process.HasExited) return;
                if ((data.data != null && data.text.Contains(Question)) ||
                    CommandLine.LineReader.Aggregate(GetBufferedData(0)).text.Contains(Question))
                {
                    Flush();
                    // Ignore I/O exceptions as this could race with the process exiting.
                    try
                    {
                        foreach (byte b in System.Text.Encoding.UTF8.GetBytes(
                                     response + System.Environment.NewLine))
                        {
                            stdin.BaseStream.WriteByte(b);
                        }
                        stdin.BaseStream.Flush();
                    }
                    catch (System.IO.IOException)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// Get the set of available SDK packages and whether they're installed.
        /// </summary>
        /// <param name="androidTool">Path to the Android SDK manager tool.</param>
        /// <param name="svcSupport">PlayServicesSupport instance used to retrieve the SDK
        /// path.</param>
        /// <param name="packages">Delegate called with a dictionary of package names and whether
        /// they're installed or null if the Android SDK isn't configured correctly.</param>
        internal static void GetAvailablePackages(
            string androidTool, PlayServicesSupport svcSupport,
            GetAvailablePackagesComplete complete)
        {
            CommandLineDialog window = CommandLineDialog.CreateCommandLineDialog(
                "Get Installed Android SDK packages.");
            window.modal = false;
            window.summaryText = "Getting list of installed Android packages.";
            window.progressTitle = window.summaryText;
            window.autoScrollToBottom = true;
            window.RunAsync(
                androidTool, "list sdk -u -e -a",
                (result) => {
                    window.Close();
                    if (result.exitCode != 0)
                    {
                        Debug.LogError("Unable to determine which Android packages are " +
                                       "installed.  Failed to run " + androidTool + ".  " +
                                       result.stderr + " (" + result.exitCode.ToString() + ")");
                        complete(null);
                        return;
                    }
                    Dictionary<string, bool> packages = new Dictionary<string, bool>();
                    string[] lines = Regex.Split(result.stdout, "\r\n|\r|\n");
                    string packageIdentifier = null;
                    foreach (string line in lines)
                    {
                        // Find the start of a package description.
                        Match match = Regex.Match(line, "^id:\\W+\\d+\\W+or\\W+\"([^\"]+)\"");
                        if (match.Success)
                        {
                            packageIdentifier = match.Groups[1].Value;
                            packages[packageIdentifier] = false;
                            continue;
                        }
                        if (packageIdentifier == null)
                        {
                            continue;
                        }
                        // Parse the install path and record whether the package is installed.
                        match = Regex.Match(line, "^\\W+Install[^:]+:\\W+([^ ]+)");
                        if (match.Success)
                        {
                            packages[packageIdentifier] = File.Exists(
                                Path.Combine(Path.Combine(svcSupport.SDK, match.Groups[1].Value),
                                    "source.properties"));
                            packageIdentifier = null;
                        }
                    }
                    complete(packages);
                },
                maxProgressLines: 50);
            window.Show();
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
        public override void DoResolution(
            PlayServicesSupport svcSupport, string destinationDirectory,
            PlayServicesSupport.OverwriteConfirmation handleOverwriteConfirmation,
            System.Action resolutionComplete)
        {
            System.Action resolve = () => {
                DoResolutionNoAndroidPackageChecks(svcSupport, destinationDirectory,
                                                   handleOverwriteConfirmation);
                resolutionComplete();
            };

            // Set of packages that need to be installed.
            Dictionary<string, bool> installPackages = new Dictionary<string, bool>();
            // Retrieve the set of required packages and whether they're installed.
            Dictionary<string, Dictionary<string, bool>> requiredPackages =
                new Dictionary<string, Dictionary<string, bool>>();
            foreach (Dependency dependency in
                     svcSupport.LoadDependencies(true, keepMissing: true).Values)
            {
                if (dependency.PackageIds != null)
                {
                    foreach (string packageId in dependency.PackageIds)
                    {
                        Dictionary<string, bool> dependencySet;
                        if (!requiredPackages.TryGetValue(packageId, out dependencySet))
                        {
                            dependencySet = new Dictionary<string, bool>();
                        }
                        dependencySet[dependency.Key] = false;
                        requiredPackages[packageId] = dependencySet;
                        // If the dependency is missing, add it to the set that needs to be
                        // installed.
                        if (System.String.IsNullOrEmpty(dependency.BestVersionPath))
                        {
                            installPackages[packageId] = false;
                        }
                    }
                }
            }

            // If no packages need to be installed or Android SDK package installation is disabled.
            if (installPackages.Count == 0 || !AndroidPackageInstallationEnabled())
            {
                // Report missing packages as warnings and try to resolve anyway.
                foreach (string pkg in requiredPackages.Keys)
                {
                    string depString = System.String.Join(
                        ", ", CollectionToArray(requiredPackages[pkg].Keys));
                    if (installPackages.ContainsKey(pkg) && depString.Length > 0)
                    {
                        Debug.LogWarning(pkg + " not installed or out of date!  This is " +
                                         "required by the following dependencies " + depString);
                    }
                }
                // Attempt resolution.
                resolve();
                return;
            }

            // Find the Android SDK manager.
            string sdkPath = svcSupport.SDK;
            string androidTool = FindAndroidSdkTool(svcSupport, "android");
            if (androidTool == null || sdkPath == null || sdkPath == "")
            {
                Debug.LogError("Unable to find the Android SDK manager tool.  " +
                               "Required Android packages (" +
                               System.String.Join(", ", CollectionToArray(installPackages.Keys)) +
                               ") can not be installed.  " +
                               PlayServicesSupport.AndroidSdkConfigurationError);
                return;
            }

            // Get the set of available and installed packages.
            GetAvailablePackages(
                androidTool, svcSupport,
                (Dictionary<string, bool> packageInfo) => {
                    if (packageInfo == null)
                    {
                        return;
                    }

                    // Filter the set of packages to install by what is available.
                    foreach (string pkg in requiredPackages.Keys)
                    {
                        bool installed = false;
                        string depString = System.String.Join(
                            ", ", CollectionToArray(requiredPackages[pkg].Keys));
                        if (packageInfo.TryGetValue(pkg, out installed))
                        {
                            if (!installed)
                            {
                                installPackages[pkg] = false;
                                Debug.LogWarning(pkg + " not installed or out of date!  " +
                                                 "This is required by the following " +
                                                 "dependencies " + depString);
                            }
                        }
                        else
                        {
                            Debug.LogWarning(pkg + " referenced by " + depString +
                                             " not available in the Android SDK.  This " +
                                             "package will not be installed.");
                            installPackages.Remove(pkg);
                        }
                    }

                    if (installPackages.Count == 0)
                    {
                        resolve();
                        return;
                    }

                    // Start installation.
                    string installPackagesString = System.String.Join(
                        ",", CollectionToArray(installPackages.Keys));
                    string packagesCommand = "update sdk -a -u -t " + installPackagesString;
                    CommandLineDialog window = CommandLineDialog.CreateCommandLineDialog(
                        "Install Android SDK packages");
                    window.summaryText = "Retrieving licenses...";
                    window.modal = false;
                    window.progressTitle = window.summaryText;
                    window.RunAsync(
                        androidTool, packagesCommand,
                        (CommandLine.Result getLicensesResult) => {
                            // Get the start of the license text.
                            int licenseTextStart = getLicensesResult.stdout.IndexOf("--------");
                            if (getLicensesResult.exitCode != 0 || licenseTextStart < 0)
                            {
                                window.Close();
                                Debug.LogError("Unable to retrieve licenses for packages " +
                                               installPackagesString);
                                return;
                            }

                            // Remove the download output from the string.
                            string licenseText = getLicensesResult.stdout.Substring(
                                licenseTextStart);
                            window.summaryText = ("License agreement(s) required to install " +
                                                  "Android SDK packages");
                            window.bodyText = licenseText;
                            window.yesText = "agree";
                            window.noText = "decline";
                            window.result = false;
                            window.Repaint();
                            window.buttonClicked = (TextAreaDialog dialog) => {
                                if (!dialog.result)
                                {
                                    window.Close();
                                    return;
                                }

                                window.summaryText = "Installing Android SDK packages...";
                                window.bodyText = "";
                                window.yesText = "";
                                window.noText = "";
                                window.buttonClicked = null;
                                window.progressTitle = window.summaryText;
                                window.autoScrollToBottom = true;
                                window.Repaint();
                                // Kick off installation.
                                ((CommandLineDialog)window).RunAsync(
                                    androidTool, packagesCommand,
                                    (CommandLine.Result updateResult) => {
                                        window.Close();
                                        if (updateResult.exitCode == 0)
                                        {
                                            resolve();
                                        }
                                        else
                                        {
                                            Debug.LogError("Android SDK update failed.  " +
                                                           updateResult.stderr + "(" +
                                                           updateResult.exitCode.ToString() + ")");
                                        }
                                    },
                                    ioHandler: (new LicenseResponder(true)).AggregateLine,
                                    maxProgressLines: 500);
                            };
                        },
                        ioHandler: (new LicenseResponder(false)).AggregateLine,
                        maxProgressLines: 250);
                });
        }

        public override void DoResolution(
            PlayServicesSupport svcSupport, string destinationDirectory,
            PlayServicesSupport.OverwriteConfirmation handleOverwriteConfirmation)
        {
            DoResolution(svcSupport, destinationDirectory, handleOverwriteConfirmation,
                         () => {});
        }

        /// <summary>
        /// Called during Update to allow the resolver to check the bundle ID of the application
        /// to see whether resolution should be triggered again.
        /// </summary>
        /// <returns>Array of packages that should be re-resolved if resolution should occur,
        /// null otherwise.</returns>
        public override string[] OnBundleId(string bundleId)
        {
            // Determine which packages need to be updated.
            List<string> packagesToUpdate = new List<string>();
            List<string> aarsToResolve = new List<string>();
            foreach (KeyValuePair<string, AarExplodeData> kv in aarExplodeData)
            {
                if (kv.Value.explode && kv.Value.bundleId != bundleId && kv.Value.path != "")
                {
                    packagesToUpdate.Add(kv.Value.path);
                    aarsToResolve.Add(kv.Key);
                }
            }
            // Remove AARs that will be resolved from the dictionary so the next call to
            // OnBundleId triggers another resolution process.
            foreach (string aar in aarsToResolve) aarExplodeData.Remove(aar);
            return packagesToUpdate.Count > 0 ? packagesToUpdate.ToArray() : null;
        }

        #endregion

        /// <summary>
        /// Perform resolution with no Android package dependency checks.
        /// </summary>
        private void DoResolutionNoAndroidPackageChecks(
            PlayServicesSupport svcSupport, string destinationDirectory,
            PlayServicesSupport.OverwriteConfirmation handleOverwriteConfirmation)
        {
            try
            {
                // Get the collection of dependencies that need to be copied.
                Dictionary<string, Dependency> deps =
                    svcSupport.ResolveDependencies(true);
                // Copy the list
                svcSupport.CopyDependencies(deps,
                                            destinationDirectory,
                                            handleOverwriteConfirmation);

            }
            catch (Google.JarResolver.ResolutionException e)
            {
                Debug.LogError(e.ToString());
                return;
            }

            // we want to look at all the .aars to decide to explode or not.
            // Some aars have variables in their AndroidManifest.xml file,
            // e.g. ${applicationId}.  Unity does not understand how to process
            // these, so we handle it here.
            ProcessAars(destinationDirectory);

            SaveAarExplodeCache();
        }

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
                string dirPath = Path.Combine(dir, Path.GetFileNameWithoutExtension(f));
                string targetPath = Path.Combine(dir, Path.GetFileName(f));
                if (ShouldExplode(f))
                {
                    ReplaceVariables(ProcessAar(Path.GetFullPath(dir), f));
                    targetPath = dirPath;
                }
                else
                {
                    // Clean up previously expanded / exploded versions of the AAR.
                    if (Directory.Exists(dirPath))
                    {
                        PlayServicesSupport.DeleteExistingFileOrDirectory(dirPath);
                    }
                }
                aarExplodeData[f].path = targetPath;
                aarExplodeData[f].bundleId = PlayerSettings.bundleIdentifier;
            }
        }

        /// <summary>
        /// Determined whether an aar file should be exploded (extracted).
        ///
        /// This is required for some aars so that the Unity Jar Resolver can perform variable
        /// expansion on manifests in the package before they're merged by aapt.
        /// </summary>
        /// <returns><c>true</c>, if the aar should be exploded, <c>false</c> otherwise.</returns>
        /// <param name="aarFile">The aar file.</param>
        internal virtual bool ShouldExplode(string aarFile)
        {
            AarExplodeData aarData = null;
            if (!aarExplodeData.TryGetValue(aarFile, out aarData)) aarData = new AarExplodeData();
            bool explode = !SupportsAarFiles;
            if (!explode)
            {
                System.DateTime modificationTime = File.GetLastWriteTime(aarFile);
                if (modificationTime.CompareTo(aarData.modificationTime) <= 0)
                {
                    explode = aarData.explode;
                }
            }
            if (!explode)
            {
                string temporaryDirectory = CreateTemporaryDirectory();
                if (temporaryDirectory == null) return false;
                string manifestFilename = "AndroidManifest.xml";
                try
                {
                    if (ExtractAar(aarFile, new string[] {manifestFilename},
                                   temporaryDirectory))
                    {
                        string manifestPath = Path.Combine(temporaryDirectory,
                                                           manifestFilename);
                        if (File.Exists(manifestPath))
                        {
                            string manifest = File.ReadAllText(manifestPath);
                            explode = manifest.IndexOf("${applicationId}") >= 0;
                        }
                        aarData.modificationTime = File.GetLastWriteTime(aarFile);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.Log("Unable to examine AAR file " + aarFile + ", err: " + e);
                    throw e;
                }
                finally
                {
                    PlayServicesSupport.DeleteExistingFileOrDirectory(temporaryDirectory);
                }
            }
            aarData.explode = explode;
            aarExplodeData[aarFile] = aarData;
            return explode;
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
