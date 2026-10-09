using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using GooglePlayServices;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace GoogleMobileAds.Editor
{
    /// <summary>
    /// Pre-processor that performs common setup tasks for all platforms before a build.
    /// </summary>
    public class BuildPreProcessor : IPreprocessBuildWithReport
    {
        // Set the callback order to be before EDM4U.
        // https://github.com/googlesamples/unity-jar-resolver/blob/master/source/AndroidResolver/src/PlayServicesPreBuild.cs#L39
        public int callbackOrder { get { return -1; } }

        private readonly static string _linkXmlAssetsPath =
                Path.Combine(Application.dataPath, "GoogleMobileAds", "link.xml");

        public void OnPreprocessBuild(BuildReport report)
        {
            // Unity's managed code stripping process does not inherently process `link.xml` files
            // in UPM packages. This pre-processor copies the `link.xml` file from the UPM package
            // to the Unity project's `Assets/GoogleMobileAds` directory if it does not exist.
            if (!File.Exists(_linkXmlAssetsPath))
            {
                CopyLinkXml();
            }
        }

        private static void CopyLinkXml()
        {
            if (!AssetDatabase.IsValidFolder(Path.Combine("Assets", "GoogleMobileAds")))
            {
                AssetDatabase.CreateFolder("Assets", "GoogleMobileAds");
            }
            var pathUtils = ScriptableObject.CreateInstance<EditorPathUtils>();
            if (pathUtils.IsPackageRootPath())
            {
                string parentDirectoryPath = pathUtils.GetParentDirectoryAssetPath();
                string linkXmlPackagePath = Path.Combine(parentDirectoryPath, "link.xml");
                if(String.IsNullOrEmpty(linkXmlPackagePath))
                {
                    Debug.LogWarning("link.xml not found in the package.");
                    return;
                }
                AssetDatabase.CopyAsset(linkXmlPackagePath, _linkXmlAssetsPath);
            }
            AssetDatabase.Refresh();
        }
    }
}
