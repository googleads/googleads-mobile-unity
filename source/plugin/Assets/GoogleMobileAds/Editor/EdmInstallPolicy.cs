using System;
using System.Text.RegularExpressions;

namespace GoogleMobileAds.Editor
{
    /// <summary>
    /// Decides what <see cref="EdmDependencyInstaller"/> should do with Unity's External Dependency
    /// Manager package, based on the project's Packages/manifest.json and the publisher's opt-out.
    /// Kept free of Unity Editor APIs so the rules can be unit tested.
    /// </summary>
    internal static class EdmInstallPolicy
    {
        internal enum Action
        {
            // Leave the project as is.
            None,
            // EDM has never been seen in the project: install it.
            Install,
            // EDM is declared with an older version than the plugin requires: upgrade it.
            Upgrade,
            // EDM was seen before and is now missing: the publisher removed it, so opt out.
            OptOut
        }

        // Matches a plain semantic version such as 2.1.0, 2.1.0-pre.1 or 2.1.0+build.
        private static readonly Regex SemVerRegex = new Regex(
            @"^\s*(\d+)\.(\d+)\.(\d+)(?:-([0-9A-Za-z.-]+))?(?:\+[0-9A-Za-z.-]+)?\s*$");

        /// <summary>
        /// Returns the action to take for the EDM package.
        /// </summary>
        /// <param name="isDeclared">Whether EDM is declared in Packages/manifest.json.</param>
        /// <param name="declaredVersion">The declared EDM version, if any.</param>
        /// <param name="enableEdm">Whether External Dependency Manager is enabled.</param>
        /// <param name="packageDetected">Whether EDM has been seen in this project before.</param>
        /// <param name="targetVersion">The EDM version required by this plugin release.</param>
        internal static Action Decide(bool isDeclared, string declaredVersion,
                                      bool enableEdm, bool packageDetected,
                                      string targetVersion)
        {
            // An opted-out project is never modified, including on re-import of the plugin.
            if (!enableEdm)
            {
                return Action.None;
            }

            if (isDeclared)
            {
                return IsOlderVersion(declaredVersion, targetVersion) ? Action.Upgrade
                                                                      : Action.None;
            }

            return packageDetected ? Action.OptOut : Action.Install;
        }

        /// <summary>
        /// Reads the version declared for <paramref name="packageName"/> in the contents of a
        /// Packages/manifest.json file. Returns false if the package is not declared.
        /// </summary>
        internal static bool TryGetDeclaredVersion(string manifestJson, string packageName,
                                                   out string version)
        {
            version = null;
            if (string.IsNullOrEmpty(manifestJson))
            {
                return false;
            }

            Match match = Regex.Match(
                manifestJson, "\"" + Regex.Escape(packageName) + "\"\\s*:\\s*\"([^\"]*)\"");
            if (!match.Success)
            {
                return false;
            }

            version = match.Groups[1].Value;
            return true;
        }

        /// <summary>
        /// Returns true if <paramref name="installedVersion"/> is a semantic version lower than
        /// <paramref name="targetVersion"/>. A pre-release is lower than the release with the same
        /// version number. Values that are not plain semantic versions (such as git URLs or local
        /// file paths) return false, so packages the publisher points at a custom source are left
        /// alone.
        /// </summary>
        internal static bool IsOlderVersion(string installedVersion, string targetVersion)
        {
            Version installedCore;
            string installedPreRelease;
            Version targetCore;
            string targetPreRelease;
            if (!TryParseSemVer(installedVersion, out installedCore, out installedPreRelease) ||
                !TryParseSemVer(targetVersion, out targetCore, out targetPreRelease))
            {
                return false;
            }

            int comparison = installedCore.CompareTo(targetCore);
            if (comparison != 0)
            {
                return comparison < 0;
            }

            return !string.IsNullOrEmpty(installedPreRelease) &&
                   string.IsNullOrEmpty(targetPreRelease);
        }

        private static bool TryParseSemVer(string value, out Version core, out string preRelease)
        {
            core = null;
            preRelease = null;
            if (value == null)
            {
                return false;
            }

            Match match = SemVerRegex.Match(value);
            if (!match.Success)
            {
                return false;
            }

            int major;
            int minor;
            int patch;
            if (!int.TryParse(match.Groups[1].Value, out major) ||
                !int.TryParse(match.Groups[2].Value, out minor) ||
                !int.TryParse(match.Groups[3].Value, out patch))
            {
                return false;
            }

            core = new Version(major, minor, patch);
            preRelease = match.Groups[4].Success ? match.Groups[4].Value : null;
            return true;
        }
    }
}
