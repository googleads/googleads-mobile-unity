// Copyright (C) 2026 Google LLC
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GoogleMobileAds.Editor
{
    /// <summary>
    /// Manages reading and atomically modifying `Packages/manifest.json` for
    /// OpenUPM scoped registries and package dependencies.
    /// </summary>
    public static class ManifestManager
    {
        public const string OpenUpmRegistryName = "package.openupm.com";
        public const string OpenUpmRegistryUrl = "https://package.openupm.com";
        public const string OpenUpmGoogleScope = "com.google";

        /// <summary>
        /// Reads `Packages/manifest.json` from the project.
        /// </summary>
        public static string ReadManifest(string projectRoot = null)
        {
            string manifestPath = GetManifestPath(projectRoot);
            if (!File.Exists(manifestPath))
            {
                return null;
            }
            return File.ReadAllText(manifestPath);
        }

        /// <summary>
        /// Writes updated contents to `Packages/manifest.json`.
        /// </summary>
        public static void WriteManifest(string manifestJson, string projectRoot = null)
        {
            string manifestPath = GetManifestPath(projectRoot);
            string directory = Path.GetDirectoryName(manifestPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(manifestPath, manifestJson);
        }

        /// <summary>
        /// Returns the full path to `Packages/manifest.json`.
        /// </summary>
        public static string GetManifestPath(string projectRoot = null)
        {
            if (string.IsNullOrEmpty(projectRoot))
            {
                projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            }
            return Path.Combine(projectRoot, "Packages", "manifest.json");
        }

        /// <summary>
        /// Ensures `https://package.openupm.com` with scope `com.google` is in `scopedRegistries`.
        /// Preserves all existing registries and formatting.
        /// </summary>
        public static string EnsureOpenUpmRegistry(string manifestJson)
        {
            if (string.IsNullOrEmpty(manifestJson))
            {
                return manifestJson;
            }

            // Check if https://package.openupm.com with com.google scope is already present.
            if (HasGoogleScopeInOpenUpm(manifestJson))
            {
                return manifestJson;
            }

            // Case A: Scoped registry for openupm exists, but lacks com.google scope.
            string openUpmMatchPattern = @"(""url""\s*:\s*""" +
                Regex.Escape(OpenUpmRegistryUrl) +
                @"[\s\S]*?""scopes""\s*:\s*\[)([^\]]*)(\])";
            Match match = Regex.Match(manifestJson, openUpmMatchPattern);
            if (match.Success)
            {
                string existingScopes = match.Groups[2].Value.Trim();
                string newScopes;
                if (string.IsNullOrEmpty(existingScopes))
                {
                    newScopes = "\n        \"" + OpenUpmGoogleScope + "\"\n      ";
                }
                else
                {
                    newScopes = match.Groups[2].Value.TrimEnd() + ",\n        \"" +
                        OpenUpmGoogleScope + "\"\n      ";
                }
                return manifestJson.Substring(0, match.Groups[2].Index) +
                       newScopes +
                       manifestJson.Substring(match.Groups[2].Index + match.Groups[2].Length);
            }

            // Case B: scopedRegistries exists, but OpenUPM registry is not declared yet.
            int registriesIndex = manifestJson.IndexOf("\"scopedRegistries\"");
            if (registriesIndex >= 0)
            {
                int openBracket = manifestJson.IndexOf('[', registriesIndex);
                if (openBracket >= 0)
                {
                    string entry = "\n    {\n" +
                                   "      \"name\": \"" + OpenUpmRegistryName + "\",\n" +
                                   "      \"url\": \"" + OpenUpmRegistryUrl + "\",\n" +
                                   "      \"scopes\": [\n" +
                                   "        \"" + OpenUpmGoogleScope + "\"\n" +
                                   "      ]\n" +
                                   "    }";
                    // If the array already has elements, append a comma after the new entry
                    int nextCharIdx = openBracket + 1;
                    while (nextCharIdx < manifestJson.Length &&
                           char.IsWhiteSpace(manifestJson[nextCharIdx]))
                    {
                        nextCharIdx++;
                    }
                    if (nextCharIdx < manifestJson.Length && manifestJson[nextCharIdx] != ']')
                    {
                        entry += ",";
                    }

                    return manifestJson.Substring(0, openBracket + 1) +
                           entry +
                           manifestJson.Substring(openBracket + 1);
                }
            }

            // Case C: No scopedRegistries section exists yet. Add it before "dependencies" or at the root.
            int dependenciesIndex = manifestJson.IndexOf("\"dependencies\"");
            if (dependenciesIndex >= 0)
            {
                string registryBlock = "  \"scopedRegistries\": [\n" +
                                       "    {\n" +
                                       "      \"name\": \"" + OpenUpmRegistryName + "\",\n" +
                                       "      \"url\": \"" + OpenUpmRegistryUrl + "\",\n" +
                                       "      \"scopes\": [\n" +
                                       "        \"" + OpenUpmGoogleScope + "\"\n" +
                                       "      ]\n" +
                                       "    }\n" +
                                       "  ],\n";
                return manifestJson.Substring(0, dependenciesIndex) +
                       registryBlock +
                       manifestJson.Substring(dependenciesIndex);
            }

            // Fallback: Insert right after the opening brace of the root object.
            int rootBrace = manifestJson.IndexOf('{');
            if (rootBrace >= 0)
            {
                string registryBlock = "\n  \"scopedRegistries\": [\n" +
                                       "    {\n" +
                                       "      \"name\": \"" + OpenUpmRegistryName + "\",\n" +
                                       "      \"url\": \"" + OpenUpmRegistryUrl + "\",\n" +
                                       "      \"scopes\": [\n" +
                                       "        \"" + OpenUpmGoogleScope + "\"\n" +
                                       "      ]\n" +
                                       "    }\n" +
                                       "  ],";
                return manifestJson.Substring(0, rootBrace + 1) +
                       registryBlock +
                       manifestJson.Substring(rootBrace + 1);
            }

            return manifestJson;
        }

        /// <summary>
        /// Adds or updates dependencies in `Packages/manifest.json`.
        /// </summary>
        public static string AddDependencies(
            string manifestJson,
            IDictionary<string, string> packagesToAdd)
        {
            if (string.IsNullOrEmpty(manifestJson) || packagesToAdd == null ||
                packagesToAdd.Count == 0)
            {
                return manifestJson;
            }

            int depIndex = manifestJson.IndexOf("\"dependencies\"");
            if (depIndex < 0)
            {
                // If dependencies block is missing, create it
                int lastBrace = manifestJson.LastIndexOf('}');
                if (lastBrace >= 0)
                {
                    var sb = new StringBuilder();
                    sb.Append(manifestJson.Substring(0, lastBrace).TrimEnd());
                    if (!sb.ToString().EndsWith(","))
                    {
                        sb.Append(",");
                    }
                    sb.Append("\n  \"dependencies\": {\n");
                    int i = 0;
                    foreach (var pair in packagesToAdd)
                    {
                        sb.AppendFormat("    \"{0}\": \"{1}\"", pair.Key, pair.Value);
                        if (++i < packagesToAdd.Count)
                        {
                            sb.Append(",");
                        }
                        sb.Append("\n");
                    }
                    sb.Append("  }\n}");
                    return sb.ToString();
                }
                return manifestJson;
            }

            int openBrace = manifestJson.IndexOf('{', depIndex);
            if (openBrace < 0)
            {
                return manifestJson;
            }

            string updated = manifestJson;
            foreach (var pair in packagesToAdd)
            {
                string pkgPattern = @"\""" + Regex.Escape(pair.Key) + @"\""\s*:\s*\""[^\""]+\""";
                if (Regex.IsMatch(updated, pkgPattern))
                {
                    // Package is already declared; update its version
                    string replacement = string.Format("\"{0}\": \"{1}\"", pair.Key, pair.Value);
                    updated = Regex.Replace(updated, pkgPattern, replacement);
                }
                else
                {
                    // Insert the new package at the beginning of the dependencies block
                    int currentDepIndex = updated.IndexOf("\"dependencies\"");
                    int currentOpenBrace = updated.IndexOf('{', currentDepIndex);

                    int nextCharIdx = currentOpenBrace + 1;
                    while (nextCharIdx < updated.Length && char.IsWhiteSpace(updated[nextCharIdx]))
                    {
                        nextCharIdx++;
                    }

                    string newEntry = string.Format("\n    \"{0}\": \"{1}\"", pair.Key, pair.Value);
                    if (nextCharIdx < updated.Length && updated[nextCharIdx] != '}')
                    {
                        newEntry += ",";
                    }
                    else
                    {
                        newEntry += "\n  ";
                    }

                    updated = updated.Substring(0, currentOpenBrace + 1) +
                              newEntry +
                              updated.Substring(currentOpenBrace + 1);
                }
            }

            return updated;
        }

        private static bool HasGoogleScopeInOpenUpm(string manifestJson)
        {
            // Matches url: openupm followed by scopes containing com.google
            string patternA = @"(""url""\s*:\s*""" + Regex.Escape(OpenUpmRegistryUrl) +
                @"[\s\S]*?""scopes""\s*:\s*\[[\s\S]*?""" + Regex.Escape(OpenUpmGoogleScope) +
                @"""[\s\S]*?\])";
            // Inverse order: scopes with com.google followed by url: openupm
            string patternB = @"(""scopes""\s*:\s*\[[\s\S]*?""" + Regex.Escape(OpenUpmGoogleScope) +
                @"""[\s\S]*?\][\s\S]*?""url""\s*:\s*""" + Regex.Escape(OpenUpmRegistryUrl) + @""")";

            return Regex.IsMatch(manifestJson, patternA) || Regex.IsMatch(manifestJson, patternB);
        }
    }
}
