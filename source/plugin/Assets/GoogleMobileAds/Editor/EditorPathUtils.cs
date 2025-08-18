// Copyright (C) 2023 Google LLC
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

using System.IO;
using UnityEditor;
using UnityEngine;

/*
 * EditorPathUtils class finds and processes the AssetPath for
 * EditorPathUtils.cs within unity asset database.
 */
public class EditorPathUtils : ScriptableObject
{
    /*
     * Returns the asset path of EditorPathUtils.cs
     */
    private string GetFilePath()
    {
        return AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
    }

    /*
     * Returns the asset directory path of EditorPathUtils.cs
     */
    public string GetDirectoryAssetPath()
    {
        return Path.GetDirectoryName(GetFilePath());
    }

    /*
     * Returns the parent asset directory path of EditorPathUtils.cs
     */
    public string GetParentDirectoryAssetPath()
    {
        return Path.GetDirectoryName(GetDirectoryAssetPath());
    }

    /*
     * Returns true if GMA import is done via unity package manager,
     * false otherwise.
     */
    public bool IsPackageRootPath()
    {
        return GetFilePath().StartsWith("Packages");
    }
}
