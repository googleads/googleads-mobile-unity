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
            int registriesIndex = manifestJson.IndexOf("\"scopedRegistries\"");
            if (registriesIndex >= 0)
            {
                MatchCollection registryMatches = Regex.Matches(
                    manifestJson.Substring(registriesIndex), @"\{[^{}]*\}");

                foreach (Match regMatch in registryMatches)
                {
                    string regText = regMatch.Value;
                    if (IsOpenUpmRegistryBlock(regText))
                    {
                        // OpenUPM registry found! Add com.google to its scopes array.
                        Match scopeMatch = Regex.Match(regText, @"\""scopes\""\s*:\s*\[([^\]]*)\]");
                        if (scopeMatch.Success)
                        {
                            Group scopesGroup = scopeMatch.Groups[1];
                            int absoluteScopesIndex = registriesIndex + regMatch.Index + scopesGroup.Index;
                            string existingScopes = scopesGroup.Value;
                            string newScopes;
                            if (string.IsNullOrWhiteSpace(existingScopes))
                            {
                                newScopes = "\n        \"" + OpenUpmGoogleScope + "\"\n      ";
                            }
                            else
                            {
                                newScopes = existingScopes.TrimEnd() + ",\n        \"" +
                                    OpenUpmGoogleScope + "\"\n      ";
                            }

                            return manifestJson.Substring(0, absoluteScopesIndex) +
                                   newScopes +
                                   manifestJson.Substring(absoluteScopesIndex + scopesGroup.Length);
                        }
                    }
                }
            }

            // Case B: scopedRegistries exists, but OpenUPM registry is not declared yet.
            if (registriesIndex >= 0)
            {
                int openBracket = manifestJson.IndexOf('[', registriesIndex);
                if (openBracket >= 0)
                {
                    string entry = "\n" + BuildOpenUpmRegistryEntry("    ");
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
                                       BuildOpenUpmRegistryEntry("    ") + "\n" +
                                       "  ],\n";
                return manifestJson.Substring(0, dependenciesIndex) +
                       registryBlock +
                       manifestJson.Substring(dependenciesIndex);
            }

            // Fallback: Insert right after the opening brace of the root object.
            int rootBrace = manifestJson.IndexOf('{');
            if (rootBrace >= 0)
            {
                int closingBrace = manifestJson.LastIndexOf('}');
                string rest = closingBrace > rootBrace
                    ? manifestJson.Substring(rootBrace + 1, closingBrace - rootBrace - 1).Trim()
                    : "";
                bool hasSubsequentProperties = !string.IsNullOrEmpty(rest);

                string registryBlock = "\n  \"scopedRegistries\": [\n" +
                                       BuildOpenUpmRegistryEntry("    ") + "\n" +
                                       "  ]" + (hasSubsequentProperties ? "," : "") + "\n";
                return manifestJson.Substring(0, rootBrace + 1) +
                       registryBlock +
                       manifestJson.Substring(rootBrace + 1);
            }

            return manifestJson;
        }

        private static string BuildOpenUpmRegistryEntry(string indent = "    ")
        {
            return indent + "{\n" +
                   indent + "  \"name\": \"" + OpenUpmRegistryName + "\",\n" +
                   indent + "  \"url\": \"" + OpenUpmRegistryUrl + "\",\n" +
                   indent + "  \"scopes\": [\n" +
                   indent + "    \"" + OpenUpmGoogleScope + "\"\n" +
                   indent + "  ]\n" +
                   indent + "}";
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
                    string prefix = manifestJson.Substring(0, lastBrace).TrimEnd();
                    var sb = new StringBuilder();
                    sb.Append(prefix);
                    if (!prefix.EndsWith("{") && !prefix.EndsWith(","))
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
                string pkgPattern = BuildPackageDependencyPattern(pair.Key);
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
            if (string.IsNullOrEmpty(manifestJson))
            {
                return false;
            }

            int registriesIndex = manifestJson.IndexOf("\"scopedRegistries\"");
            if (registriesIndex < 0)
            {
                return false;
            }

            MatchCollection registryMatches = Regex.Matches(
                manifestJson.Substring(registriesIndex), @"\{[^{}]*\}");

            foreach (Match regMatch in registryMatches)
            {
                string regText = regMatch.Value;
                if (IsOpenUpmRegistryBlock(regText))
                {
                    Match scopeMatch = Regex.Match(regText, @"\""scopes\""\s*:\s*\[([^\]]*)\]");
                    if (scopeMatch.Success)
                    {
                        string scopesContent = scopeMatch.Groups[1].Value;
                        if (Regex.IsMatch(scopesContent, @"\""" + Regex.Escape(OpenUpmGoogleScope) + @"\"""))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if <paramref name="packageName"/> is declared inside the
        /// "dependencies" block of <paramref name="manifestJson"/>.
        /// </summary>
        public static bool HasDependency(string manifestJson, string packageName)
        {
            if (string.IsNullOrEmpty(manifestJson) || string.IsNullOrEmpty(packageName))
            {
                return false;
            }

            Match depMatch = Regex.Match(manifestJson, @"\""dependencies\""\s*:\s*\{");
            if (!depMatch.Success)
            {
                return false;
            }

            int openBrace = depMatch.Index + depMatch.Length - 1;
            int closeBrace = FindObjectClosingBrace(manifestJson, openBrace);
            if (closeBrace <= openBrace)
            {
                return false;
            }

            string depsBody = manifestJson.Substring(openBrace, closeBrace - openBrace + 1);
            return Regex.IsMatch(depsBody, BuildPackageDependencyPattern(packageName));
        }

        private static bool IsOpenUpmRegistryBlock(string registryBlockJson)
        {
            return !string.IsNullOrEmpty(registryBlockJson) &&
                   Regex.IsMatch(
                       registryBlockJson,
                       @"\""url\""\s*:\s*\""" + Regex.Escape(OpenUpmRegistryUrl) + @"/?\""");
        }

        private static string BuildPackageDependencyPattern(string packageName)
        {
            return @"\""" + Regex.Escape(packageName) + @"\""\s*:\s*\""[^\""]+\""";
        }

        private static int FindObjectClosingBrace(string json, int openBraceIndex)
        {
            int depth = 0;
            bool inString = false;
            for (int i = openBraceIndex; i < json.Length; i++)
            {
                char c = json[i];
                if (c == '\\' && inString)
                {
                    i++;
                    continue;
                }
                if (c == '"')
                {
                    inString = !inString;
                    continue;
                }
                if (inString)
                {
                    continue;
                }
                if (c == '{')
                {
                    depth++;
                }
                else if (c == '}')
                {
                    depth--;
                    if (depth == 0)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
    }
}
