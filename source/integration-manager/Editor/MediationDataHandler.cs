using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace GoogleMobileAds.Editor.IntegrationManager
{
    public class MediationDataHandler
    {
        internal const string MediationPackagePrefix = "com.google.ads.mobile.mediation.";
        internal const string GmaSdkPackageName = "com.google.ads.mobile";
        internal const string DefaultGmaSdkChangelogUrl = "https://github.com/googleads/googleads-mobile-unity/releases";
        internal const string DefaultMediationChangelogUrl = "https://developers.google.com/admob/unity/mediation";
        internal const string GmaSdkQuickStartUrl = "https://developers.google.com/admob/unity/quick-start";
        private List<MediationNetworkModel> _networks;
        private readonly Action _onDataLoaded;
        private readonly IPackageManagerClient _packageManagerClient;

        public bool IsLoading { get; internal set; }
        public MediationNetworkModel GmaSdkModel => _networks.FirstOrDefault(n => n.NetworkName == GmaSdkPackageName);
        public string InstalledGmaSdkVersion => GmaSdkModel?.InstalledVersion;
        public string LatestGmaSdkVersion => GmaSdkModel?.LatestVersion;
        public string GmaSdkChangelogUrl => GmaSdkModel?.ChangelogUrl ?? DefaultGmaSdkChangelogUrl;

        public MediationDataHandler(
            Action onDataLoaded,
            List<MediationNetworkModel> existingNetworks,
            IPackageManagerClient packageManagerClient = null)
        {
            _onDataLoaded = onDataLoaded;
            _packageManagerClient = packageManagerClient ?? new UpmPackageManagerClient();

            if (existingNetworks != null && existingNetworks.Count > 0)
            {
                _networks = existingNetworks;
                IsLoading = false;
            }
            else
            {
                _networks = new List<MediationNetworkModel>();
                IsLoading = true;
            }

            // Start async load
            LoadData();
        }

        public List<MediationNetworkModel> GetNetworks()
        {
            return _networks.Where(n => n.NetworkName.StartsWith(MediationPackagePrefix)).ToList();
        }

        public List<MediationNetworkModel> GetAllNetworkModels()
        {
            return _networks;
        }

        private async void LoadData()
        {
            var installedPackages = await _packageManagerClient.GetInstalledPackagesAsync();
            var availablePackages = await _packageManagerClient.SearchAvailablePackagesAsync();

            var allDiscoveredPackageNames = new HashSet<string>();
            foreach (var key in installedPackages.Keys) allDiscoveredPackageNames.Add(key);
            foreach (var key in availablePackages.Keys) allDiscoveredPackageNames.Add(key);

            foreach (var pkgName in allDiscoveredPackageNames)
            {

                var existing = _networks.FirstOrDefault(n => n.NetworkName == pkgName);
                if (existing == null)
                {
                    installedPackages.TryGetValue(pkgName, out var instVersion);
                    availablePackages.TryGetValue(pkgName, out var availInfo);
                    _networks.Add(new MediationNetworkModel
                    {
                        NetworkName = pkgName,
                        DisplayName = GetFriendlyDisplayName(pkgName, availInfo?.DisplayName),
                        InstalledVersion = null,
                        LatestVersion = availInfo != null ? availInfo.LatestVersion : null,
                        ChangelogUrl = availInfo?.ChangelogUrl,
                        AvailableVersions = availInfo?.AvailableVersions ?? new List<string>(),
                        IsInstalled = false,
                        IsFetchingLatest = availInfo == null
                    });
                }
            }

            var existingDirectImportFolders = new List<string>();
            string mediationFolderPath = Path.Combine(Application.dataPath, "GoogleMobileAds", "Mediation");
            if (Directory.Exists(mediationFolderPath))
            {
                string[] directories = Directory.GetDirectories(mediationFolderPath);
                foreach (string dir in directories)
                {
                    existingDirectImportFolders.Add(Path.GetFileName(dir));
                }
            }

            foreach (var network in _networks)
            {
                bool isInstalledInUPM = installedPackages.TryGetValue(network.NetworkName, out var installedVersion);
                UpdateNetworkStateFromUPM(network, isInstalledInUPM, installedVersion);
                if (availablePackages.TryGetValue(network.NetworkName, out var availInfo) && availInfo != null)
                {
                    if (availInfo.AvailableVersions != null && availInfo.AvailableVersions.Count > 0)
                    {
                        network.AvailableVersions = availInfo.AvailableVersions;
                    }
                    if (string.IsNullOrEmpty(network.DisplayName) || network.DisplayName == network.NetworkName)
                    {
                        network.DisplayName = GetFriendlyDisplayName(network.NetworkName, availInfo.DisplayName);
                    }
                }
                if (string.IsNullOrEmpty(network.ChangelogUrl) && availablePackages.TryGetValue(network.NetworkName, out var pkgSummary) && pkgSummary != null)
                {
                    network.ChangelogUrl = pkgSummary.ChangelogUrl;
                }

                network.IsDirectAssetImport = IsDirectAssetImportDetected(network, existingDirectImportFolders);
            }

            _networks.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase));

            // Notify that installed states and available packages are determined
            IsLoading = false;
            _onDataLoaded?.Invoke();

            // Fetch latest versions for any networks that still need it
            var fetchTasks = _networks.Where(n => n.IsFetchingLatest || string.IsNullOrEmpty(n.LatestVersion)).Select(async network =>
            {
                if (availablePackages.TryGetValue(network.NetworkName, out var availInfo) && availInfo != null)
                {
                    network.LatestVersion = availInfo.LatestVersion;
                }
                else
                {
                    network.LatestVersion = await _packageManagerClient.FetchLatestVersionAsync(network.NetworkName);
                }
                network.IsFetchingLatest = false;
            }).ToList();

            await Task.WhenAll(fetchTasks);

            _networks.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase));
            _onDataLoaded?.Invoke();
        }

        private void UpdateNetworkStateFromUPM(
            MediationNetworkModel network,
            bool isInstalledInUPM,
            string installedVersion)
        {
            string expectedVersion = !string.IsNullOrEmpty(network.TargetVersion) ? network.TargetVersion : network.LatestVersion;

            if (network.CurrentTransitionState == MediationNetworkModel.TransitionState.Installing)
            {
                if (isInstalledInUPM && (string.IsNullOrEmpty(expectedVersion) || installedVersion == expectedVersion))
                {
                    if (!string.IsNullOrEmpty(network.InstalledVersion) && network.InstalledVersion != installedVersion)
                    {
                        network.PreviousInstalledVersion = network.InstalledVersion;
                    }
                    network.InstalledVersion = installedVersion;
                    network.IsInstalled = true;
                    network.CurrentTransitionState = MediationNetworkModel.TransitionState.None;
                    network.TargetVersion = null;
                }
                else
                {
                    // Still installing, keep transition state
                    network.IsInstalled = false;
                    network.InstalledVersion = null;
                }
            }
            else if (network.CurrentTransitionState == MediationNetworkModel.TransitionState.Updating)
            {
                if (isInstalledInUPM && (string.IsNullOrEmpty(expectedVersion) || installedVersion == expectedVersion))
                {
                    if (!string.IsNullOrEmpty(network.InstalledVersion) && network.InstalledVersion != installedVersion)
                    {
                        network.PreviousInstalledVersion = network.InstalledVersion;
                    }
                    network.InstalledVersion = installedVersion;
                    network.IsInstalled = true;
                    network.CurrentTransitionState = MediationNetworkModel.TransitionState.None;
                    network.TargetVersion = null;
                }
                else
                {
                    // Still updating
                    network.IsInstalled = true;
                    // Keep old installed version for display during transition if possible,
                    // but since we reuse the model, it should already have it.
                }
            }
            else if (network.CurrentTransitionState == MediationNetworkModel.TransitionState.Removing)
            {
                if (!isInstalledInUPM)
                {
                    network.InstalledVersion = null;
                    network.IsInstalled = false;
                    network.CurrentTransitionState = MediationNetworkModel.TransitionState.None;
                    network.TargetVersion = null;
                }
                else
                {
                    // Still removing
                    network.IsInstalled = true;
                    network.InstalledVersion = installedVersion;
                }
            }
            else
            {
                // Normal state
                if (isInstalledInUPM)
                {
                    if (!string.IsNullOrEmpty(network.InstalledVersion) && network.InstalledVersion != installedVersion)
                    {
                        network.PreviousInstalledVersion = network.InstalledVersion;
                    }
                    network.InstalledVersion = installedVersion;
                    network.IsInstalled = true;
                }
                else
                {
                    network.InstalledVersion = null;
                    network.IsInstalled = false;
                }
            }
        }

        internal static string GetFriendlyDisplayName(string packageName, string upmDisplayName)
        {
            if (packageName == GmaSdkPackageName)
            {
                return !string.IsNullOrEmpty(upmDisplayName) ? upmDisplayName : "Google Mobile Ads";
            }
            if (!string.IsNullOrEmpty(upmDisplayName))
            {
                string name = upmDisplayName;
                const string prefix = "Google Mobile Ads ";
                const string suffix = " Mediation";
                if (name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    name = name.Substring(prefix.Length);
                }
                if (name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                {
                    name = name.Substring(0, name.Length - suffix.Length);
                }
                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name.Trim();
                }
            }

            if (packageName.StartsWith(MediationPackagePrefix, StringComparison.OrdinalIgnoreCase))
            {
                string raw = packageName.Substring(MediationPackagePrefix.Length);
                if (raw.Length > 0)
                {
                    return char.ToUpper(raw[0]) + raw.Substring(1);
                }
            }
            return packageName;
        }

        public async Task InstallNetwork(MediationNetworkModel network, string targetVersion = null)
        {
            string versionToInstall = targetVersion ?? network.LatestVersion;
            if (network.IsInstalled && !string.IsNullOrEmpty(network.InstalledVersion) && network.InstalledVersion != versionToInstall)
            {
                network.PreviousInstalledVersion = network.InstalledVersion;
            }
            network.TargetVersion = versionToInstall;
            await _packageManagerClient.InstallOrUpdatePackageAsync(network.NetworkName, versionToInstall);
        }

        public async Task UpdateNetwork(MediationNetworkModel network, string targetVersion = null)
        {
            string versionToInstall = targetVersion ?? network.LatestVersion;
            if (network.IsInstalled && !string.IsNullOrEmpty(network.InstalledVersion) && network.InstalledVersion != versionToInstall)
            {
                network.PreviousInstalledVersion = network.InstalledVersion;
            }
            network.TargetVersion = versionToInstall;
            await _packageManagerClient.InstallOrUpdatePackageAsync(network.NetworkName, versionToInstall);
        }

        public async Task RemoveNetwork(MediationNetworkModel network)
        {
            await _packageManagerClient.RemovePackageAsync(network.NetworkName);
        }

        public async Task InstallOrUpdateGmaSdk(string targetVersion = null)
        {
            string versionToInstall = targetVersion ?? LatestGmaSdkVersion;
            if (!string.IsNullOrEmpty(versionToInstall))
            {
                var gmaModel = GmaSdkModel;
                if (gmaModel != null)
                {
                    if (gmaModel.IsInstalled && !string.IsNullOrEmpty(gmaModel.InstalledVersion) && gmaModel.InstalledVersion != versionToInstall)
                    {
                        gmaModel.PreviousInstalledVersion = gmaModel.InstalledVersion;
                    }
                    gmaModel.CurrentTransitionState = gmaModel.IsInstalled
                        ? MediationNetworkModel.TransitionState.Updating
                        : MediationNetworkModel.TransitionState.Installing;
                    gmaModel.TargetVersion = versionToInstall;
                }
                await _packageManagerClient.InstallOrUpdatePackageAsync(GmaSdkPackageName, versionToInstall);
            }
        }

        internal static bool IsDirectAssetImportDetected(MediationNetworkModel network, IEnumerable<string> existingDirectImportFolders)
        {
            if (network == null || string.IsNullOrEmpty(network.NetworkName))
            {
                return false;
            }

            if (network.NetworkName == GmaSdkPackageName)
            {
                return EditorUtilityHelper.IsValidFolderSafe("GoogleMobileAds/Editor");
            }

            if (existingDirectImportFolders == null)
            {
                return false;
            }

            string pkgSuffix = network.NetworkName;
            if (pkgSuffix.StartsWith(MediationPackagePrefix, StringComparison.OrdinalIgnoreCase))
            {
                pkgSuffix = pkgSuffix.Substring(MediationPackagePrefix.Length);
            }

            foreach (string folder in existingDirectImportFolders)
            {
                if (string.IsNullOrEmpty(folder))
                {
                    continue;
                }

                // Match against the exact package suffix (e.g., "AppLovin" vs "applovin") or with an "Ads" suffix
                // to account for networks like Unity Ads where the folder is named "UnityAds" but the UPM suffix is "unity".
                if (string.Equals(folder, pkgSuffix, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(folder, pkgSuffix + "Ads", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
