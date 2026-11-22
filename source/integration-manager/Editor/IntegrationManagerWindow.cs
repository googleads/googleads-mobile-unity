using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using System.Linq;

namespace GoogleMobileAds.Editor.IntegrationManager
{
    [InitializeOnLoad]
    public class IntegrationManagerWindow : EditorWindow
    {
        public static bool IsDirectAssetImportDetected =>
            EditorUtilityHelper.IsValidFolderSafe("GoogleMobileAds/Editor");

        [MenuItem("Assets/Google Mobile Ads/Integration Manager")]
        public static void ShowWindow()
        {
            GetWindow<IntegrationManagerWindow>("Integration Manager");
        }
        private MediationDataHandler dataHandler;
        private int _activeTabIndex = 0;
        private bool _isRefreshPending = false;

        [SerializeField]
        private List<MediationNetworkModel> _serializedNetworks = new List<MediationNetworkModel>();

        private Vector2 _scrollOffset = Vector2.zero;

        private void OnEnable()
        {
            this.minSize = new Vector2(850, 400);
            bool isNull = dataHandler == null;
            if (isNull)
            {
                dataHandler = new MediationDataHandler(RefreshUI, _serializedNetworks);
                if (_serializedNetworks.Count == 0)
                {
                    _serializedNetworks = dataHandler.GetAllNetworkModels();
                }
            }

            UnityEditor.PackageManager.Events.registeredPackages += OnPackageRegistrationChanged;
            IntegrationManagerAssetPostprocessor.OnGoogleMobileAdsAssetsChanged += ScheduleDebouncedRefresh;
        }

        private void OnDisable()
        {
            UnityEditor.PackageManager.Events.registeredPackages -= OnPackageRegistrationChanged;
            IntegrationManagerAssetPostprocessor.OnGoogleMobileAdsAssetsChanged -= ScheduleDebouncedRefresh;
        }

        private void OnFocus()
        {
            ScheduleDebouncedRefresh();
        }

        internal void OnPackageRegistrationChanged(UnityEditor.PackageManager.PackageRegistrationEventArgs args)
        {
            var affectedPackages = (args.added ?? Enumerable.Empty<UnityEditor.PackageManager.PackageInfo>())
                .Concat(args.removed ?? Enumerable.Empty<UnityEditor.PackageManager.PackageInfo>())
                .Concat(args.changedFrom ?? Enumerable.Empty<UnityEditor.PackageManager.PackageInfo>())
                .Concat(args.changedTo ?? Enumerable.Empty<UnityEditor.PackageManager.PackageInfo>());

            bool affectsGma = affectedPackages
                .Any(pkg => pkg != null && !string.IsNullOrEmpty(pkg.name) && pkg.name.StartsWith(MediationDataHandler.GmaSdkPackageName));

            if (affectsGma)
            {
                ScheduleDebouncedRefresh();
            }
        }

        private void ScheduleDebouncedRefresh()
        {
            if (_isRefreshPending) return;
            _isRefreshPending = true;
            EditorApplication.delayCall += () =>
            {
                _isRefreshPending = false;
                if (this != null && dataHandler != null)
                {
                    ReinitDataHandler();
                }
            };
        }

        public void CreateGUI()
        {
            if (dataHandler == null) return;

            var settings = SettingsProviderRegistry.Provider ?? ReflectionSettingsProvider.CreateIfAvailable();
            var view = new IntegrationManagerView(
                dataHandler,
                settings,
                _serializedNetworks,
                _activeTabIndex,
                InstallNetwork,
                UpdateNetwork,
                RemoveNetwork,
                SetActiveTab,
                InstallOrUpdateGmaSdk
            );
            view.ScrollOffset = _scrollOffset;

            rootVisualElement.Clear();
            rootVisualElement.Add(view);
        }

        internal void SetActiveTab(int index)
        {
            _activeTabIndex = index;
            RefreshUI();
        }

        private async void InstallOrUpdateGmaSdk(string version = null)
        {
            bool isInstalled = dataHandler != null && dataHandler.GmaSdkModel != null && dataHandler.GmaSdkModel.IsInstalled;
            UpdateLocalNetworkTransitionState(MediationDataHandler.GmaSdkPackageName, isInstalled ? MediationNetworkModel.TransitionState.Updating : MediationNetworkModel.TransitionState.Installing);
            RefreshUI();
            await dataHandler.InstallOrUpdateGmaSdk(version);
            ReinitDataHandler();
        }

        private async void UpdateNetwork(MediationNetworkModel network, string version = null)
        {
            UpdateLocalNetworkTransitionState(network.NetworkName, MediationNetworkModel.TransitionState.Updating);
            RefreshUI();
            await dataHandler.UpdateNetwork(network, version);
            ReinitDataHandler();
        }

        private async void InstallNetwork(MediationNetworkModel network, string version = null)
        {
            UpdateLocalNetworkTransitionState(network.NetworkName, MediationNetworkModel.TransitionState.Installing);
            RefreshUI();
            await dataHandler.InstallNetwork(network, version);
            ReinitDataHandler();
        }

        private async void RemoveNetwork(MediationNetworkModel network)
        {
            UpdateLocalNetworkTransitionState(network.NetworkName, MediationNetworkModel.TransitionState.Removing);
            RefreshUI();
            await dataHandler.RemoveNetwork(network);
            ReinitDataHandler();
        }

        private void UpdateLocalNetworkTransitionState(string packageName, MediationNetworkModel.TransitionState state)
        {
            var match = _serializedNetworks.FirstOrDefault(n => n.NetworkName == packageName);
            if (match != null)
            {
                match.CurrentTransitionState = state;
            }
        }

        private void ReinitDataHandler()
        {
            dataHandler = new MediationDataHandler(RefreshUI, _serializedNetworks);
            _serializedNetworks = dataHandler.GetAllNetworkModels();
            RefreshUI();
        }

        private void RefreshUI()
        {
            IntegrationManagerView view = rootVisualElement.Q<IntegrationManagerView>();
            if (view != null)
            {
                _scrollOffset = view.ScrollOffset;
            }
            rootVisualElement.Clear();
            CreateGUI();
        }
    }
}
