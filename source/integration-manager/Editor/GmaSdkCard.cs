using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GoogleMobileAds.Editor.IntegrationManager
{
    internal class GmaSdkCard : VisualElement
    {
        private readonly string _changelogUrl;
        private readonly Action<VisualElement, List<MediationNetworkRow.MenuItemInfo>> _onShowOptions;

        public GmaSdkCard(
            string installedVersion,
            string latestVersion,
            Action onUpdate,
            Action<string> onInstallVersion = null,
            List<string> availableVersions = null,
            string changelogUrl = null,
            Action<VisualElement, List<MediationNetworkRow.MenuItemInfo>> onShowOptions = null,
            bool isDirectAssetImport = false,
            MediationNetworkModel.TransitionState transitionState = MediationNetworkModel.TransitionState.None)
        {
            _changelogUrl = changelogUrl;
            _onShowOptions = onShowOptions;
            style.marginTop = 16;
            style.marginBottom = 24;

            CreateUI(installedVersion, latestVersion, onUpdate, onInstallVersion, availableVersions, isDirectAssetImport, transitionState);
        }

        private void CreateUI(
            string installedVersion,
            string latestVersion,
            Action onUpdate,
            Action<string> onInstallVersion,
            List<string> availableVersions,
            bool isDirectAssetImport,
            MediationNetworkModel.TransitionState transitionState)
        {
            VisualElement header = UIStyles.CreateHeader("Google Mobile Ads SDK", "Update the latest version of Google Mobile Ads SDK. Available on Android and iOS.");
            header.style.marginBottom = 24;
            Add(header);

            VisualElement tableContainer = new VisualElement();
            tableContainer.style.flexDirection = FlexDirection.Column;
            tableContainer.style.backgroundColor = UIStyles.TableBgColor;
            UIStyles.ZeroBorders(tableContainer.style);
            tableContainer.Add(CreateTableHeaders());
            tableContainer.Add(CreateCombinedRow(installedVersion, latestVersion, onUpdate, onInstallVersion, availableVersions, isDirectAssetImport, transitionState));
            Add(tableContainer);
        }

        private VisualElement CreateTableHeaders()
        {
            return UIStyles.CreateTableHeaderRow("SDK component", "Installed version", "Latest version", "");
        }

        private VisualElement CreateCombinedRow(
            string installedVersion,
            string latestVersion,
            Action onUpdate,
            Action<string> onInstallVersion,
            List<string> availableVersions,
            bool isDirectAssetImport,
            MediationNetworkModel.TransitionState transitionState)
        {
            VisualElement rowContainer = new VisualElement();
            rowContainer.style.flexDirection = FlexDirection.Column;
            rowContainer.style.borderBottomWidth = 1;
            rowContainer.style.borderBottomColor = UIStyles.BorderColor;

            VisualElement mainRow = new VisualElement();
            UIStyles.ApplyRowStyle(mainRow, Color.clear);
            mainRow.style.paddingTop = 12;
            mainRow.style.paddingBottom = 12;
            mainRow.style.paddingLeft = 24;
            mainRow.style.paddingRight = 24;

            Label componentLabel = new Label(UIStyles.GmaSdkPluginDisplayName);
            UIStyles.ApplyColumnTextStyle(componentLabel, 0.4f, isBold: true);
            componentLabel.style.fontSize = 14;
            componentLabel.style.color = UIStyles.HeaderTextColor;
            mainRow.Add(componentLabel);

            Label installedLabel = new Label(string.IsNullOrEmpty(installedVersion) ? "Not installed" : installedVersion);
            UIStyles.ApplyColumnTextStyle(installedLabel, 0.2f, isBold: false);
            installedLabel.style.fontSize = 13;
            installedLabel.style.color = UIStyles.DimmedTextColor;
            mainRow.Add(installedLabel);

            // Latest Version Column
            bool isNotInstalled = string.IsNullOrEmpty(installedVersion) || installedVersion == "Not installed" || installedVersion == "Loading...";
            bool hasUpdate = isNotInstalled || IsNewerVersion(latestVersion, installedVersion);
            mainRow.Add(CreateLatestVersionColumn(latestVersion, hasUpdate));

            // Action Button Column
            mainRow.Add(CreateActionColumn(isNotInstalled, hasUpdate, onUpdate, isDirectAssetImport, transitionState));

            // Options Menu Column
            mainRow.Add(CreateOptionsMenuCell(installedVersion, onInstallVersion, availableVersions, isDirectAssetImport, transitionState));

            rowContainer.Add(mainRow);
            rowContainer.Add(CreateHelpBox());

            if (isDirectAssetImport)
            {
                rowContainer.Add(UIStyles.CreateNoticeBanner(
                    "Direct asset import detected for Google Mobile Ads Unity Plugin. Please remove the folder from Assets/GoogleMobileAds/ before installing or updating via the Integration Manager.",
                    new Color(0.35f, 0.25f, 0.05f, 0.6f),
                    new Color(0.9f, 0.7f, 0.1f, 1f),
                    "⚠️",
                    Color.yellow,
                    leftBorderOnly: true));
            }

            return rowContainer;
        }

        private VisualElement CreateOptionsMenuCell(
            string installedVersion,
            Action<string> onInstallVersion,
            List<string> availableVersions,
            bool isDirectAssetImport,
            MediationNetworkModel.TransitionState transitionState)
        {
            if (transitionState != MediationNetworkModel.TransitionState.None)
            {
                return UIStyles.CreateOptionsMenuButton(null, null);
            }
            string url = !string.IsNullOrEmpty(_changelogUrl) ? _changelogUrl : MediationDataHandler.DefaultGmaSdkChangelogUrl;
            VisualElement optionsContainer = null;
            optionsContainer = UIStyles.CreateOptionsMenuButton(() => {
                var menuItems = new List<MediationNetworkRow.MenuItemInfo>();
                menuItems.Add(new MediationNetworkRow.MenuItemInfo
                {
                    Icon = UIStyles.ChangeLogMenuIcon,
                    Text = UIStyles.ChangeLogMenuText,
                    IsDisabled = false,
                    Action = () => { try { UnityEngine.Application.OpenURL(url); } catch { } }
                });

                var prevVersions = GetPreviousVersions(installedVersion, availableVersions);
                if (!isDirectAssetImport && prevVersions != null && prevVersions.Count > 0)
                {
                    var subItems = new List<MediationNetworkRow.MenuItemInfo>();
                    foreach (var ver in prevVersions)
                    {
                        string v = ver;
                        subItems.Add(new MediationNetworkRow.MenuItemInfo
                        {
                            Text = v,
                            IsDisabled = false,
                            Action = () => onInstallVersion?.Invoke(v)
                        });
                    }
                    menuItems.Add(new MediationNetworkRow.MenuItemInfo
                    {
                        Icon = UIStyles.InstallSpecificVersionMenuIcon,
                        Text = UIStyles.InstallSpecificVersionMenuText,
                        IsDisabled = false,
                        SubMenu = subItems
                    });
                }
                else
                {
                    menuItems.Add(new MediationNetworkRow.MenuItemInfo
                    {
                        Icon = UIStyles.InstallSpecificVersionMenuIcon,
                        Text = UIStyles.InstallSpecificVersionMenuText,
                        IsDisabled = true,
                        Action = null
                    });
                }

                if (_onShowOptions != null)
                {
                    _onShowOptions.Invoke(optionsContainer, menuItems);
                }
                else
                {
                    GenericMenu menu = new GenericMenu();
                    foreach (var item in menuItems)
                    {
                        if (item.SubMenu != null && item.SubMenu.Count > 0)
                        {
                            foreach (var sub in item.SubMenu)
                            {
                                menu.AddItem(new GUIContent($"{item.Text}/{sub.Text}"), false, new GenericMenu.MenuFunction(sub.Action));
                            }
                        }
                        else if (item.IsDisabled || item.Action == null)
                        {
                            menu.AddDisabledItem(new GUIContent(item.Text));
                        }
                        else
                        {
                            menu.AddItem(new GUIContent(item.Text), false, new GenericMenu.MenuFunction(item.Action));
                        }
                    }
                    menu.ShowAsContext();
                }
            }, $"Options for {UIStyles.GmaSdkPluginDisplayName}");
            return optionsContainer;
        }

        private List<string> GetPreviousVersions(string installedVersion, List<string> availableVersions)
        {
            var results = new List<string>();
            if (availableVersions != null && availableVersions.Count > 0)
            {
                results = availableVersions
                    .Where(v => v != installedVersion)
                    .OrderByDescending(v => System.Version.TryParse(v, out var cand) ? cand : new System.Version(0, 0))
                    .ToList();
            }
            return results.Distinct().ToList();
        }

        private VisualElement CreateLatestVersionColumn(string latestVersion, bool hasUpdate)
        {
            VisualElement latestContainer = new VisualElement();
            UIStyles.ApplyColumnContainerStyle(latestContainer, 0.2f, Align.Center, Justify.FlexStart);

            Label latestLabel = new Label(latestVersion);
            latestLabel.style.fontSize = 13;
            latestLabel.style.color = hasUpdate ? UIStyles.HeaderTextColor : UIStyles.DimmedTextColor;
            latestContainer.Add(latestLabel);

            if (hasUpdate)
            {
                latestContainer.Add(UIStyles.CreateBlueDot());
            }
            return latestContainer;
        }

        private VisualElement CreateActionColumn(
            bool isNotInstalled,
            bool hasUpdate,
            Action onUpdate,
            bool isDirectAssetImport,
            MediationNetworkModel.TransitionState transitionState)
        {
            VisualElement actionContainer = new VisualElement();
            UIStyles.ApplyColumnContainerStyle(actionContainer, 0.2f, Align.Center, Justify.FlexEnd);

            if (transitionState == MediationNetworkModel.TransitionState.Installing)
            {
                Label l = new Label("Installing...");
                l.style.color = UIStyles.DimmedTextColor;
                l.style.unityFontStyleAndWeight = FontStyle.Italic;
                actionContainer.Add(l);
                return actionContainer;
            }

            if (transitionState == MediationNetworkModel.TransitionState.Updating)
            {
                Label l = new Label("Updating...");
                l.style.color = UIStyles.DimmedTextColor;
                l.style.unityFontStyleAndWeight = FontStyle.Italic;
                actionContainer.Add(l);
                return actionContainer;
            }

            if (isDirectAssetImport)
            {
                Button disabledBtn = new Button();
                disabledBtn.text = isNotInstalled ? UIStyles.InstallButtonText : (hasUpdate ? UIStyles.UpdateButtonText : UIStyles.InstalledButtonText);
                UIStyles.ApplyActionButtonStyle(disabledBtn, UIStyles.DisabledButtonBgColor, UIStyles.DisabledButtonTextColor, isBold: false);
                disabledBtn.SetEnabled(false);
                actionContainer.Add(disabledBtn);
            }
            else if (isNotInstalled)
            {
                Button installButton = new Button(onUpdate);
                installButton.text = UIStyles.InstallButtonText;
                UIStyles.ApplyActionButtonStyle(installButton, UIStyles.InstallButtonBgColor, UIStyles.InstallButtonTextColor);
                actionContainer.Add(installButton);
            }
            else if (hasUpdate)
            {
                Button updateButton = new Button(onUpdate);
                updateButton.text = UIStyles.UpdateButtonText;
                UIStyles.ApplyActionButtonStyle(updateButton, UIStyles.UpdateButtonColor, Color.white);
                actionContainer.Add(updateButton);
            }
            else
            {
                Button installedBtn = new Button();
                installedBtn.text = UIStyles.InstalledButtonText;
                UIStyles.ApplyActionButtonStyle(installedBtn, UIStyles.DisabledButtonBgColor, UIStyles.DisabledButtonTextColor, isBold: false);
                installedBtn.SetEnabled(false);
                actionContainer.Add(installedBtn);
            }
            return actionContainer;
        }

        private VisualElement CreateHelpBox()
        {
            VisualElement textContainer = new VisualElement();
            textContainer.style.flexDirection = FlexDirection.Row;
            textContainer.style.flexWrap = Wrap.Wrap;

            Label messageLabel = new Label("Google Mobile Ads Unity is required to show ads on your app. Updating it will also update the Android and iOS SDKs. ");
            messageLabel.style.color = UIStyles.DimmedTextColor;
            messageLabel.style.fontSize = 12;
            messageLabel.style.whiteSpace = WhiteSpace.Normal;
            textContainer.Add(messageLabel);

            Label learnMoreLabel = new Label("Learn more ↗");
            learnMoreLabel.style.color = UIStyles.LinkColor;
            learnMoreLabel.style.fontSize = 12;
            learnMoreLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            learnMoreLabel.RegisterCallback<ClickEvent>(evt => {
                Application.OpenURL(MediationDataHandler.GmaSdkQuickStartUrl);
            });
            textContainer.Add(learnMoreLabel);

            VisualElement helpBox = UIStyles.CreateNoticeBanner(
                textContainer,
                new Color(0.16f, 0.17f, 0.18f, 0.5f),
                UIStyles.BorderColor,
                "ⓘ",
                UIStyles.DimmedTextColor);
            helpBox.style.marginLeft = 24;
            helpBox.style.marginRight = 24;
            helpBox.style.marginTop = 4;
            helpBox.style.marginBottom = 12;
            helpBox.style.paddingLeft = 12;
            return helpBox;
        }

        private bool IsNewerVersion(string latestVersion, string installedVersion)
        {
            if (System.Version.TryParse(latestVersion, out System.Version latest) &&
                System.Version.TryParse(installedVersion, out System.Version installed))
            {
                return latest > installed;
            }
            return false;
        }
    }
}
