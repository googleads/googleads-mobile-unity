using System.Text.RegularExpressions;

namespace GoogleMobileAds.Editor
{
    /// <summary>
    /// Decides what <see cref="EdmDependencyInstaller"/> should do with Unity's External Dependency
    /// Manager package, based on the project's `Packages/manifest.json` and the publisher's
    /// opt-out. The Unity Editor APIs that the installer calls are static and can't be mocked
    /// without wrappers, so the rules are kept free of them and unit tested directly.
    /// </summary>
    internal static class EdmInstallPolicy
    {
        /// <summary>
        /// The action to take for the EDM package. Defaults to <see cref="Install"/>: EDM is
        /// installed unless the publisher opts out by removing it.
        /// </summary>
        internal enum Action
        {
            // EDM has not been installed yet or needs to be updated to the required version. This
            // is the default value, so keep it first.
            Install,
            // EDM was seen before and is now missing: the publisher removed it, so opt out.
            OptOut,
            // Leave the project as is.
            None
        }

        /// <summary>
        /// The state of the project that <see cref="Decide"/> bases its decision on.
        /// </summary>
        internal struct ProjectState
        {
            // Whether EDM is declared in `Packages/manifest.json`.
            internal bool IsDeclared;
            // Whether Unity's External Dependency Manager is enabled.
            internal bool EnableEdm;
            // Whether EDM has been seen in this project before.
            internal bool PackageDetected;
            // Whether the plugin already added the EDM version this release requires.
            internal bool EdmVersionAdded;
        }

        /// <summary>
        /// Returns the action to take for the EDM package.
        /// </summary>
        internal static Action Decide(ProjectState state)
        {
            // An opted-out project is never modified, including on re-import of the plugin.
            if (!state.EnableEdm)
            {
                return Action.None;
            }

            if (!state.IsDeclared)
            {
                return state.PackageDetected ? Action.OptOut : Action.Install;
            }

            return state.EdmVersionAdded ? Action.None : Action.Install;
        }

        /// <summary>
        /// Returns whether <paramref name="packageName"/> is declared in the contents of a
        /// `Packages/manifest.json` file.
        /// </summary>
        internal static bool IsDeclared(string manifestJson, string packageName)
        {
            if (string.IsNullOrEmpty(manifestJson) || string.IsNullOrEmpty(packageName))
            {
                return false;
            }

            // Matches an entry such as `"com.unity.external-dependency-manager": "2.1.0"`.
            // JSON allows whitespace, or none, on either side of the colon.
            return Regex.IsMatch(
                manifestJson, "\"" + Regex.Escape(packageName) + "\"\\s*:\\s*\"[^\"]+\"");
        }
    }
}
