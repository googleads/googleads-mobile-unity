using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GoogleMobileAds.Editor.IntegrationManager
{
    internal class IntegrationManagerView : VisualElement
    {
        private readonly MediationDataHandler _dataHandler;
        private readonly ISettingsProvider _settings;
        private readonly List<MediationNetworkModel> _serializedNetworks;
        private readonly int _activeTabIndex;

        private readonly Action<MediationNetworkModel, string> _onInstallNetwork;
        private readonly Action<MediationNetworkModel, string> _onUpdateNetwork;
        private readonly Action<MediationNetworkModel> _onRemoveNetwork;
        private readonly Action<int> _onTabChanged;
        private readonly Action<string> _onInstallOrUpdateGmaSdk;

        private Vector2 _scrollOffset = Vector2.zero;
        private VisualElement _popupOverlay;
        private VisualElement _currentDropdown;
        private VisualElement _currentSubDropdown;

        public IntegrationManagerView(
            MediationDataHandler dataHandler,
            ISettingsProvider settings,
            List<MediationNetworkModel> serializedNetworks,
            int activeTabIndex,
            Action<MediationNetworkModel> onInstallNetwork,
            Action<MediationNetworkModel> onUpdateNetwork,
            Action<MediationNetworkModel> onRemoveNetwork,
            Action<int> onTabChanged,
            Action<string> onInstallOrUpdateGmaSdk = null)
            : this(
                  dataHandler,
                  settings,
                  serializedNetworks,
                  activeTabIndex,
                  (net, ver) => onInstallNetwork?.Invoke(net),
                  (net, ver) => onUpdateNetwork?.Invoke(net),
                  onRemoveNetwork,
                  onTabChanged,
                  onInstallOrUpdateGmaSdk)
        {
        }

        public IntegrationManagerView(
            MediationDataHandler dataHandler,
            ISettingsProvider settings,
            List<MediationNetworkModel> serializedNetworks,
            int activeTabIndex,
            Action<MediationNetworkModel, string> onInstallNetwork,
            Action<MediationNetworkModel, string> onUpdateNetwork,
            Action<MediationNetworkModel> onRemoveNetwork,
            Action<int> onTabChanged,
            Action<string> onInstallOrUpdateGmaSdk = null)
        {
            _dataHandler = dataHandler;
            _settings = settings;
            _serializedNetworks = serializedNetworks;
            _activeTabIndex = activeTabIndex;

            _onInstallNetwork = onInstallNetwork;
            _onUpdateNetwork = onUpdateNetwork;
            _onRemoveNetwork = onRemoveNetwork;
            _onTabChanged = onTabChanged;
            _onInstallOrUpdateGmaSdk = onInstallOrUpdateGmaSdk;

            InitUI();
        }

        private void InitUI()
        {
            if (_dataHandler == null) return;

            this.Clear();
            UIStyles.ApplyWindowStyle(this);
            this.style.flexGrow = 1;
            this.style.paddingLeft = 0;
            this.style.paddingRight = 0;
            this.style.paddingTop = 0;
            this.style.paddingBottom = 0;

            VisualElement headerContainer = new VisualElement();
            headerContainer.style.paddingLeft = UIStyles.GenerousRootPadding;
            headerContainer.style.paddingRight = UIStyles.GenerousRootPadding;
            headerContainer.style.paddingTop = UIStyles.GenerousRootPadding;

            // Title
            Label titleLabel = new Label("Google Integration Manager");
            UIStyles.ApplyTitleStyle(titleLabel);
            headerContainer.Add(titleLabel);

            headerContainer.Add(CreateTabBar());
            this.Add(headerContainer);

            // ScrollView for Tab Content
            ScrollView scrollView = new ScrollView();
            scrollView.style.flexGrow = 1;
            scrollView.contentContainer.style.paddingLeft = 32;
            scrollView.contentContainer.style.paddingRight = 40;
            scrollView.contentContainer.style.paddingTop = 8;
            scrollView.contentContainer.style.paddingBottom = 24;
            this.Add(scrollView);

            // We don't easily have access to geometry changed event callbacks in a simple way
            // without rendering, but we can keep the scroll offset logic if needed.
            // For unit tests, we might not need to verify scroll offset restoration.
            if (_scrollOffset != Vector2.zero)
            {
                EventCallback<GeometryChangedEvent> onGeometryChanged = null;
                onGeometryChanged = evt => {
                    scrollView.scrollOffset = _scrollOffset;
                    scrollView.contentContainer.UnregisterCallback(onGeometryChanged);
                };
                scrollView.contentContainer.RegisterCallback(onGeometryChanged);
            }

            if (_activeTabIndex == 0)
            {
                DrawMediationTab(scrollView);
            }
            else if (_activeTabIndex == 1)
            {
                DrawSettingsTab(scrollView);
            }

            SetupPopupOverlay();
        }

        public Vector2 ScrollOffset
        {
            get
            {
                ScrollView scrollView = this.Q<ScrollView>();
                return scrollView != null ? scrollView.scrollOffset : Vector2.zero;
            }
            set
            {
                _scrollOffset = value;
                ScrollView scrollView = this.Q<ScrollView>();
                if (scrollView != null)
                {
                    scrollView.scrollOffset = value;
                }
            }
        }

        private VisualElement CreateTabBar()
        {
            VisualElement tabBar = new VisualElement();
            tabBar.style.flexDirection = FlexDirection.Row;
            tabBar.style.borderBottomWidth = 1;
            tabBar.style.borderBottomColor = UIStyles.BorderColor;
            tabBar.style.marginBottom = 24;
            tabBar.style.height = UIStyles.TabBarHeight;
            tabBar.style.flexShrink = 0;

            Button mediationTabButton = CreateTabButton("Manage SDK", () => _onTabChanged?.Invoke(0), 0, UIStyles.TabButtonSpacing);
            Button settingsTabButton = CreateTabButton("Set up SDK", () => _onTabChanged?.Invoke(1), 0, 0);

            StyleTabButton(mediationTabButton, _activeTabIndex == 0);
            StyleTabButton(settingsTabButton, _activeTabIndex == 1);

            tabBar.Add(mediationTabButton);
            tabBar.Add(settingsTabButton);
            return tabBar;
        }

        private Button CreateTabButton(string text, Action onClick, float marginLeft, float marginRight)
        {
            Button button = new Button(onClick) { text = text };
            button.style.width = UIStyles.TabButtonWidth;
            button.style.height = UIStyles.TabButtonHeight;
            button.style.fontSize = 14;
            button.style.backgroundColor = Color.clear;
            button.style.borderTopWidth = 0;
            button.style.borderLeftWidth = 0;
            button.style.borderRightWidth = 0;
            button.style.borderTopLeftRadius = 0;
            button.style.borderTopRightRadius = 0;
            button.style.borderBottomLeftRadius = 0;
            button.style.borderBottomRightRadius = 0;
            button.style.marginLeft = marginLeft;
            button.style.marginRight = marginRight;
            button.style.marginTop = 0;
            button.style.marginBottom = 0;
            button.style.paddingLeft = 16;
            button.style.paddingRight = 16;
            return button;
        }

        private void StyleTabButton(Button button, bool isActive)
        {
            if (isActive)
            {
                button.style.color = Color.white;
                button.style.unityFontStyleAndWeight = FontStyle.Bold;
                button.style.borderBottomWidth = 3;
                button.style.borderBottomColor = UIStyles.PrimaryButtonColor;
            }
            else
            {
                button.style.color = UIStyles.DimmedTextColor;
                button.style.unityFontStyleAndWeight = FontStyle.Normal;
                button.style.borderBottomWidth = 0;
            }
        }

        private void DrawMediationTab(ScrollView scrollView)
        {
            var networks = _dataHandler.GetNetworks();

            var installedNetworks = networks
                .Where(n => n.IsInstalled)
                .OrderBy(n => n.HasUpdate ? 0 : 1)
                .ThenBy(n => n.NetworkName)
                .ToList();

            var uninstalledNetworks = networks
                .Where(n => !n.IsInstalled)
                .OrderBy(n => n.NetworkName)
                .ToList();

            DrawGmaSdkSection(scrollView);
            DrawMediatedNetworksSection(scrollView, installedNetworks, uninstalledNetworks);
        }

        private void DrawGmaSdkSection(ScrollView scrollView)
        {
            var gmaSdkModel = _dataHandler.GmaSdkModel;
            var transitionState = gmaSdkModel?.CurrentTransitionState ?? MediationNetworkModel.TransitionState.None;
            string installedVer = string.IsNullOrEmpty(_dataHandler.InstalledGmaSdkVersion)
                ? (_dataHandler.IsLoading ? "Loading..." : "Not installed")
                : _dataHandler.InstalledGmaSdkVersion;

            string latestVer = string.IsNullOrEmpty(_dataHandler.LatestGmaSdkVersion)
                ? (_dataHandler.IsLoading ? "Loading..." : "Unknown")
                : _dataHandler.LatestGmaSdkVersion;
            var availableVersions = gmaSdkModel?.AvailableVersions;
            bool isDirectAssetImport = gmaSdkModel?.IsDirectAssetImport ?? false;

            GmaSdkCard sdkCard = new GmaSdkCard(
                installedVer,
                latestVer,
                async () => {
                    if (_onInstallOrUpdateGmaSdk != null)
                    {
                        _onInstallOrUpdateGmaSdk.Invoke(null);
                    }
                    else
                    {
                        await _dataHandler.InstallOrUpdateGmaSdk();
                    }
                },
                onInstallVersion: async version => {
                    if (_onInstallOrUpdateGmaSdk != null)
                    {
                        _onInstallOrUpdateGmaSdk.Invoke(version);
                    }
                    else
                    {
                        await _dataHandler.InstallOrUpdateGmaSdk(version);
                    }
                },
                availableVersions: availableVersions,
                changelogUrl: _dataHandler.GmaSdkChangelogUrl,
                onShowOptions: (btn, items) => ShowDropdownMenu(btn, items),
                isDirectAssetImport: isDirectAssetImport,
                transitionState: transitionState);
            scrollView.Add(sdkCard);
        }

        private void DrawMediatedNetworksSection(ScrollView scrollView, List<MediationNetworkModel> installedNetworks, List<MediationNetworkModel> uninstalledNetworks)
        {
            VisualElement section = new VisualElement();
            section.style.flexGrow = 1;
            section.style.marginBottom = 24;
            scrollView.Add(section);

            VisualElement sectionHeader = UIStyles.CreateHeader("Mediated Networks", "Update the latest version of network adapters to allow mediation on your app");
            section.Add(sectionHeader);

            VisualElement table = new VisualElement();
            table.style.flexGrow = 1;
            section.Add(table);

            if (_dataHandler.IsLoading && installedNetworks.Count == 0 && uninstalledNetworks.Count == 0)
            {
                DrawLoadingState(table);
            }
            else if (!_dataHandler.IsLoading && installedNetworks.Count == 0 && uninstalledNetworks.Count == 0)
            {
                DrawEmptyState(table);
            }
            else
            {
                if (installedNetworks.Count > 0)
                {
                    DrawNetworkTable(table, $"Installed Networks ({installedNetworks.Count})", installedNetworks, showUpdateAll: true);
                }

                if (uninstalledNetworks.Count > 0)
                {
                    DrawNetworkTable(table, $"Available Networks ({uninstalledNetworks.Count})", uninstalledNetworks, showUpdateAll: false);
                }
            }
        }

        private void DrawLoadingState(VisualElement container)
        {
            VisualElement card = new VisualElement();
            card.style.flexGrow = 1;
            card.style.minHeight = 360;
            card.style.backgroundColor = UIStyles.CardBgColor;
            card.style.borderTopWidth = 1;
            card.style.borderBottomWidth = 1;
            card.style.borderLeftWidth = 1;
            card.style.borderRightWidth = 1;
            card.style.borderTopColor = UIStyles.BorderColor;
            card.style.borderBottomColor = UIStyles.BorderColor;
            card.style.borderLeftColor = UIStyles.BorderColor;
            card.style.borderRightColor = UIStyles.BorderColor;
            card.style.borderTopLeftRadius = 4;
            card.style.borderTopRightRadius = 4;
            card.style.borderBottomLeftRadius = 4;
            card.style.borderBottomRightRadius = 4;
            card.style.paddingTop = 24;
            card.style.paddingBottom = 24;
            card.style.paddingLeft = 24;
            card.style.paddingRight = 24;
            card.style.alignItems = Align.Center;
            card.style.justifyContent = Justify.Center;

            Label titleLabel = new Label("Loading Mediation Adapters...");
            titleLabel.style.fontSize = 13;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.color = UIStyles.HeaderTextColor;
            titleLabel.style.marginBottom = 6;
            card.Add(titleLabel);

            Label subLabel = new Label("Checking Unity Package Manager for installed and available network adapters.");
            subLabel.style.fontSize = 12;
            subLabel.style.color = UIStyles.DimmedTextColor;
            card.Add(subLabel);

            container.Add(card);
        }

        private void DrawEmptyState(VisualElement container)
        {
            VisualElement card = new VisualElement();
            card.style.flexGrow = 1;
            card.style.minHeight = 360;
            card.style.backgroundColor = UIStyles.CardBgColor;
            card.style.borderTopWidth = 1;
            card.style.borderBottomWidth = 1;
            card.style.borderLeftWidth = 1;
            card.style.borderRightWidth = 1;
            card.style.borderTopColor = UIStyles.BorderColor;
            card.style.borderBottomColor = UIStyles.BorderColor;
            card.style.borderLeftColor = UIStyles.BorderColor;
            card.style.borderRightColor = UIStyles.BorderColor;
            card.style.borderTopLeftRadius = 4;
            card.style.borderTopRightRadius = 4;
            card.style.borderBottomLeftRadius = 4;
            card.style.borderBottomRightRadius = 4;
            card.style.paddingTop = 24;
            card.style.paddingBottom = 24;
            card.style.paddingLeft = 24;
            card.style.paddingRight = 24;
            card.style.alignItems = Align.Center;
            card.style.justifyContent = Justify.Center;

            Label titleLabel = new Label("No Mediation Adapters Discovered");
            titleLabel.style.fontSize = 13;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.color = UIStyles.HeaderTextColor;
            titleLabel.style.marginBottom = 6;
            card.Add(titleLabel);

            Label subLabel = new Label("Verify your internet connection and check that Unity Package Manager is online and accessible.");
            subLabel.style.fontSize = 12;
            subLabel.style.color = UIStyles.DimmedTextColor;
            subLabel.style.marginBottom = 16;
            card.Add(subLabel);

            Button refreshButton = new Button(() => InitUI()) { text = "Refresh" };
            UIStyles.ApplyActionButtonStyle(refreshButton, UIStyles.InstallButtonBgColor, UIStyles.InstallButtonTextColor);
            card.Add(refreshButton);

            container.Add(card);
        }

        private void DrawNetworkTable(VisualElement container, string title, List<MediationNetworkModel> networks, bool showUpdateAll = false)
        {
            VisualElement headerRow = new VisualElement();
            headerRow.style.flexDirection = FlexDirection.Row;
            headerRow.style.justifyContent = Justify.SpaceBetween;
            headerRow.style.alignItems = Align.Center;
            headerRow.style.marginTop = 16;
            headerRow.style.marginBottom = 8;
            headerRow.style.paddingLeft = 24;
            headerRow.style.paddingRight = 24 + UIStyles.TrashColumnWidth + UIStyles.DefaultPadding;

            Label tableTitle = new Label(title);
            UIStyles.ApplySubTitleStyle(tableTitle);
            tableTitle.style.fontSize = 14;
            tableTitle.style.unityFontStyleAndWeight = FontStyle.Bold;
            tableTitle.style.color = UIStyles.HeaderTextColor;
            tableTitle.style.marginBottom = 0;
            headerRow.Add(tableTitle);

            bool isGmaDirectAssetImport = _dataHandler?.GmaSdkModel?.IsDirectAssetImport ?? false;
            if (showUpdateAll)
            {
                var updatableNetworks = networks.Where(n => n.HasUpdate && !n.IsDirectAssetImport && !isGmaDirectAssetImport).ToList();
                if (updatableNetworks.Count > 0)
                {
                    Button updateAllBtn = CreateUpdateAllButton(updatableNetworks);
                    headerRow.Add(updateAllBtn);
                }
            }

            container.Add(headerRow);

            container.Add(new MediationNetworksTable(networks,
                (n, v) => _onInstallNetwork?.Invoke(n, v),
                (n, v) => _onUpdateNetwork?.Invoke(n, v),
                n => _onRemoveNetwork?.Invoke(n),
                (btn, items) => ShowDropdownMenu(btn, items),
                isGmaDirectAssetImport));
        }

        private void DrawSettingsTab(ScrollView scrollView)
        {
            var settings = _settings ?? SettingsProviderRegistry.Provider ?? ReflectionSettingsProvider.CreateIfAvailable();
            bool isInstalled = _dataHandler != null && _dataHandler.GmaSdkModel != null &&
                (_dataHandler.GmaSdkModel.IsInstalled || _dataHandler.GmaSdkModel.IsDirectAssetImport);
            if (isInstalled && settings != null)
            {
                var sdkSettingsCard = new SdkSettingsCard(settings);
                scrollView.Add(sdkSettingsCard);

                var androidSettingsCard = new AndroidSettingsCard(settings);
                scrollView.Add(androidSettingsCard);
            }
            else
            {
                DrawMissingSdkPrompt(scrollView);
            }
        }

        private void DrawMissingSdkPrompt(ScrollView scrollView)
        {
            VisualElement card = new VisualElement();
            UIStyles.ApplyCardStyle(card);
            card.style.minHeight = 320;
            card.style.justifyContent = Justify.Center;
            card.style.alignItems = Align.Center;
            card.style.marginBottom = 32;

            Label titleLabel = new Label("Google Mobile Ads SDK Not Installed");
            titleLabel.style.fontSize = 16;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.color = UIStyles.HeaderTextColor;
            titleLabel.style.marginBottom = 12;
            card.Add(titleLabel);

            Label subLabel = new Label("Please install the Google Mobile Ads SDK from the 'Manage SDK' tab\nto configure App IDs and build settings.");
            subLabel.style.fontSize = 13;
            subLabel.style.color = UIStyles.DimmedTextColor;
            subLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            card.Add(subLabel);

            scrollView.Add(card);
        }

        private void SetupPopupOverlay()
        {
            _popupOverlay = new VisualElement();
            _popupOverlay.style.position = Position.Absolute;
            _popupOverlay.style.top = 0;
            _popupOverlay.style.left = 0;
            _popupOverlay.style.right = 0;
            _popupOverlay.style.bottom = 0;
            _popupOverlay.style.display = DisplayStyle.None;
            _popupOverlay.RegisterCallback<MouseDownEvent>(evt => CloseDropdown());
            this.Add(_popupOverlay);
        }

        public void ShowDropdownMenu(VisualElement targetButton, List<MediationNetworkRow.MenuItemInfo> items)
        {
            CloseDropdown();
            if (_popupOverlay == null) return;

            _popupOverlay.style.display = DisplayStyle.Flex;
            _popupOverlay.BringToFront();

            Vector2 worldPos = targetButton.LocalToWorld(new Vector2(targetButton.layout.width, targetButton.layout.height));
            Vector2 localPos = _popupOverlay.WorldToLocal(worldPos);
            float left = Mathf.Max(10f, localPos.x - 280f);
            float top = localPos.y + 2f;

            _currentDropdown = UIStyles.CreateDropdownPopup(280f, left, top);
            _currentDropdown.RegisterCallback<MouseDownEvent>(evt => evt.StopPropagation());

            foreach (var item in items)
            {
                _currentDropdown.Add(CreateDropdownRow(item));
            }

            _popupOverlay.Add(_currentDropdown);
        }

        private Button CreateUpdateAllButton(List<MediationNetworkModel> updatableNetworks)
        {
            Button updateAllBtn = new Button(() => {
                foreach (var net in updatableNetworks)
                {
                    _onUpdateNetwork?.Invoke(net, net.LatestVersion);
                }
            });
            updateAllBtn.text = $"⚠  Update all ({updatableNetworks.Count})";
            UIStyles.ApplyPrimaryButtonStyle(updateAllBtn);
            updateAllBtn.style.fontSize = 12;
            updateAllBtn.style.paddingLeft = 12;
            updateAllBtn.style.paddingRight = 12;
            updateAllBtn.style.paddingTop = 6;
            updateAllBtn.style.paddingBottom = 6;
            UIStyles.ApplyRoundCorners(updateAllBtn.style, 6f);
            return updateAllBtn;
        }

        private VisualElement CreateDropdownRow(MediationNetworkRow.MenuItemInfo item, bool isSubMenuItem = false)
        {
            VisualElement row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.minHeight = 32;
            row.style.paddingLeft = 14;
            row.style.paddingRight = 14;
            row.style.paddingTop = 4;
            row.style.paddingBottom = 4;

            if (!string.IsNullOrEmpty(item.Icon))
            {
                Label iconLabel = new Label(item.Icon);
                iconLabel.style.width = 24;
                iconLabel.style.fontSize = 14;
                iconLabel.style.color = item.IsDisabled ? UIStyles.DimmedTextColor : UIStyles.HeaderTextColor;
                row.Add(iconLabel);
            }

            Label textLabel = new Label(item.Text);
            textLabel.style.flexGrow = 1;
            textLabel.style.fontSize = 12;
            textLabel.style.whiteSpace = WhiteSpace.Normal;
            textLabel.style.color = item.IsDisabled ? UIStyles.DimmedTextColor : UIStyles.HeaderTextColor;
            row.Add(textLabel);

            if (item.SubMenu != null && item.SubMenu.Count > 0)
            {
                Label arrowLabel = new Label(">");
                arrowLabel.style.fontSize = 12;
                arrowLabel.style.color = item.IsDisabled ? UIStyles.DimmedTextColor : UIStyles.HeaderTextColor;
                row.Add(arrowLabel);
            }

            if (!item.IsDisabled)
            {
                if (item.SubMenu != null && item.SubMenu.Count > 0)
                {
                    row.RegisterCallback<MouseEnterEvent>(evt => {
                        row.style.backgroundColor = new Color(1f, 1f, 1f, 0.05f);
                        ShowSubDropdownMenu(row, item.SubMenu);
                    });
                    row.RegisterCallback<MouseLeaveEvent>(evt => row.style.backgroundColor = Color.clear);
                    row.RegisterCallback<MouseDownEvent>(evt => {
                        evt.StopPropagation();
                        ShowSubDropdownMenu(row, item.SubMenu);
                    });
                }
                else if (item.Action != null)
                {
                    if (!isSubMenuItem)
                    {
                        row.RegisterCallback<MouseEnterEvent>(evt => {
                            row.style.backgroundColor = new Color(1f, 1f, 1f, 0.05f);
                            CloseSubDropdown();
                        });
                    }
                    else
                    {
                        row.RegisterCallback<MouseEnterEvent>(evt => row.style.backgroundColor = new Color(1f, 1f, 1f, 0.05f));
                    }
                    row.RegisterCallback<MouseLeaveEvent>(evt => row.style.backgroundColor = Color.clear);
                    row.RegisterCallback<MouseDownEvent>(evt => {
                        evt.StopPropagation();
                        CloseDropdown();
                        item.Action.Invoke();
                    });
                }
            }
            return row;
        }

        private void ShowSubDropdownMenu(VisualElement targetRow, List<MediationNetworkRow.MenuItemInfo> items)
        {
            CloseSubDropdown();
            if (_popupOverlay == null || _currentDropdown == null) return;

            Vector2 worldPosLeft = targetRow.LocalToWorld(Vector2.zero);
            Vector2 localPosLeft = _popupOverlay.WorldToLocal(worldPosLeft);
            float rootHeight = Mathf.Max(300f, this.layout.height);
            float subMenuWidth = 180f;
            float left = localPosLeft.x - subMenuWidth - 2f;
            float top = localPosLeft.y;

            if (left < 10f)
            {
                Vector2 worldPosRight = targetRow.LocalToWorld(new Vector2(targetRow.layout.width, 0));
                Vector2 localPosRight = _popupOverlay.WorldToLocal(worldPosRight);
                left = localPosRight.x + 2f;
            }

            _currentSubDropdown = UIStyles.CreateDropdownPopup(subMenuWidth, left, top);
            _currentSubDropdown.RegisterCallback<MouseDownEvent>(evt => evt.StopPropagation());

            VisualElement header = new VisualElement();
            header.style.minHeight = 28;
            header.style.justifyContent = Justify.Center;
            header.style.paddingLeft = 14;
            header.style.paddingRight = 14;
            Label headerLabel = new Label("SELECT VERSION");
            headerLabel.style.fontSize = 10;
            headerLabel.style.color = UIStyles.DimmedTextColor;
            header.Add(headerLabel);
            _currentSubDropdown.Add(header);

            ScrollView scrollView = new ScrollView(ScrollViewMode.Vertical);
            float maxScrollHeight = Mathf.Min(240f, Mathf.Max(120f, rootHeight - top - 45f));
            scrollView.style.maxHeight = maxScrollHeight;
            foreach (var item in items)
            {
                scrollView.Add(CreateDropdownRow(item, true));
            }
            _currentSubDropdown.Add(scrollView);

            _popupOverlay.Add(_currentSubDropdown);
        }

        private void CloseSubDropdown()
        {
            if (_currentSubDropdown != null)
            {
                _currentSubDropdown.RemoveFromHierarchy();
                _currentSubDropdown = null;
            }
        }

        public void CloseDropdown()
        {
            CloseSubDropdown();
            if (_currentDropdown != null)
            {
                _currentDropdown.RemoveFromHierarchy();
                _currentDropdown = null;
            }
            if (_popupOverlay != null)
            {
                _popupOverlay.style.display = DisplayStyle.None;
            }
        }
    }
}
