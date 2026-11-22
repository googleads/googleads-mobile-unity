using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using GoogleMobileAds.Editor;
using GoogleMobileAds.Editor.IntegrationManager;

namespace GoogleMobileAds.Editor.Tests
{
    [TestFixture]
    public class IntegrationManagerE2ETest
    {
        private const float TestTimeout = 90f;
        private GoogleMobileAdsSettings _settings;
        private MediationDataHandler _dataHandler;

        // Backup variables to restore the real settings asset state after mutation tests
        private string _origAndroidAppId;
        private string _origIosAppId;
        private bool _origOptimizeInit;
        private bool _origOptimizeAdLoading;
        private bool _origKotlinX;
        private bool _origOverrideAndroidSdk;
        private int _origSelectedAndroidSdk;

        private GoogleMobileAdsSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = ScriptableObject.CreateInstance<GoogleMobileAdsSettings>();
                }
                return _settings;
            }
        }

        private MediationDataHandler DataHandler
        {
            get
            {
                if (_dataHandler == null)
                {
                    _dataHandler = new MediationDataHandler(
                        onDataLoaded: () => {},
                        existingNetworks: new List<MediationNetworkModel>()
                    );
                }
                return _dataHandler;
            }
        }

        [SetUp]
        public void SetUp()
        {
            if (UnityEngine.Application.isBatchMode) return;
            // Back up the real settings asset state before every test to guarantee clean runs
            var s = Settings;
            _origAndroidAppId = s.GoogleMobileAdsAndroidAppId;
            _origIosAppId = s.GoogleMobileAdsIOSAppId;
            _origOptimizeInit = s.DisableOptimizeInitialization;
            _origOptimizeAdLoading = s.DisableOptimizeAdLoading;
            _origKotlinX = s.EnableKotlinXCoroutinesPackagingOption;
            _origOverrideAndroidSdk = s.OverrideDefaultGmaAndroidSdk;
            _origSelectedAndroidSdk = s.SelectedGmaAndroidSdk;
        }

        [TearDown]
        public void TearDown()
        {
            if (UnityEngine.Application.isBatchMode) return;
            // Restore the real settings asset back to its original state so the project remains 100% clean
            var s = Settings;
            s.GoogleMobileAdsAndroidAppId = _origAndroidAppId;
            s.GoogleMobileAdsIOSAppId = _origIosAppId;
            s.DisableOptimizeInitialization = _origOptimizeInit;
            s.DisableOptimizeAdLoading = _origOptimizeAdLoading;
            s.EnableKotlinXCoroutinesPackagingOption = _origKotlinX;
            s.OverrideDefaultGmaAndroidSdk = _origOverrideAndroidSdk;
            s.SelectedGmaAndroidSdk = _origSelectedAndroidSdk;
            EditorUtility.SetDirty(s);
        }

        // Helper coroutine to instantiate a fresh MediationDataHandler and wait until its UPM scan completes.
        private IEnumerator RefreshDataHandlerCoroutine()
        {
            bool loaded = false;
            _dataHandler = new MediationDataHandler(
                onDataLoaded: () => { loaded = true; },
                existingNetworks: new List<MediationNetworkModel>()
            );

            double startTime = EditorApplication.timeSinceStartup;
            float scanTimeout = 20f;

            while (!loaded && (EditorApplication.timeSinceStartup - startTime < scanTimeout))
            {
                yield return null;
            }

            while (_dataHandler.GetNetworks().Any(n => n.IsFetchingLatest) && (EditorApplication.timeSinceStartup - startTime < scanTimeout))
            {
                yield return null;
            }
        }

        // Helper to create a real IntegrationManagerView using the real data handler and real settings asset
        private IntegrationManagerView CreateView(int activeTabIndex = 0, System.Action<int> onTabSwitched = null)
        {
            var networks = DataHandler.GetNetworks();
            return new IntegrationManagerView(
                DataHandler,
                Settings,
                networks,
                activeTabIndex,
                (n) => {}, // Mock install callback (not triggered in read-only UI tests)
                (n) => {}, // Mock update callback (not triggered in read-only UI tests)
                (n) => {}, // Mock remove callback (not triggered in read-only UI tests)
                onTabSwitched != null ? onTabSwitched : (tabIndex) => {}
            );
        }

        private void SimulateClick(Button button)
        {
            var clickable = button.clickable;
            if (clickable != null)
            {
                var clickedField = typeof(Clickable).GetField("clicked", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (clickedField != null)
                {
                    var delegateValue = clickedField.GetValue(clickable) as System.Action;
                    if (delegateValue != null)
                    {
                        delegateValue.Invoke();
                        return;
                    }
                }
            }

            Event downSystemEvent = new Event { type = EventType.MouseDown, button = 0 };
            using (var downEvt = MouseDownEvent.GetPooled(downSystemEvent))
            {
                downEvt.target = button;
                button.SendEvent(downEvt);
            }

            Event upSystemEvent = new Event { type = EventType.MouseUp, button = 0 };
            using (var upEvt = MouseUpEvent.GetPooled(upSystemEvent))
            {
                upEvt.target = button;
                button.SendEvent(upEvt);
            }
        }

        private void SimulateValueChange<T>(BaseField<T> field, T newValue)
        {
            T oldValue = field.value;
            field.value = newValue;

            using (var evt = ChangeEvent<T>.GetPooled(oldValue, newValue))
            {
                evt.target = field;

                var currentTargetProp = typeof(EventBase).GetProperty("currentTarget", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                currentTargetProp?.SetValue(evt, field);

                var propagationPhaseProp = typeof(EventBase).GetProperty("propagationPhase", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                propagationPhaseProp?.SetValue(evt, PropagationPhase.BubbleUp);

                var registryField = typeof(CallbackEventHandler).GetField("m_CallbackRegistry", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                if (registryField != null)
                {
                    var registry = registryField.GetValue(field);
                    if (registry != null)
                    {
                        var methods = registry.GetType().GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                        var invokeMethod = methods.FirstOrDefault(m => m.Name == "InvokeCallbacks" && m.GetParameters().Length == 1);
                        if (invokeMethod != null)
                        {
                            invokeMethod.Invoke(registry, new object[] { evt });
                            return;
                        }

                        invokeMethod = methods.FirstOrDefault(m => m.Name == "InvokeCallbacks" && m.GetParameters().Length == 2);
                        if (invokeMethod != null)
                        {
                            invokeMethod.Invoke(registry, new object[] { evt, PropagationPhase.BubbleUp });
                            return;
                        }
                    }
                }

                var handleEventMethod = typeof(CallbackEventHandler).GetMethod(
                    "HandleEvent",
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Public
                );
                handleEventMethod?.Invoke(field, new object[] { evt });
            }
        }

        [UnityTest]
        public IEnumerator TestUpmRegistryConnection_LoadsNetworkData()
        {
            if (UnityEngine.Application.isBatchMode)
            {
                UnityEngine.Debug.Log("[E2E Skip] Skipping E2E test in headless batch mode. Run locally in Unity Editor GUI!");
                yield break;
            }
            yield return RefreshDataHandlerCoroutine();
            var networks = DataHandler.GetNetworks();
            Assert.IsNotNull(networks, "Loaded networks list is null.");
            Assert.IsNotEmpty(networks, "Loaded networks list is empty.");

            var unityAds = networks.Find(n => n.NetworkName == "com.google.ads.mobile.mediation.unity");
            Assert.IsNotNull(unityAds, "Could not find Unity Ads network in metadata mapping.");
            Assert.AreEqual("Unity Ads", unityAds.DisplayName, "Unity Ads display name mapping is incorrect.");

            double startTime = EditorApplication.timeSinceStartup;
            while (unityAds.IsFetchingLatest && (EditorApplication.timeSinceStartup - startTime < TestTimeout))
            {
                yield return null;
            }

            Assert.IsFalse(unityAds.IsFetchingLatest, "Timed out fetching latest version from live UPM registry.");
            Assert.IsNotEmpty(unityAds.LatestVersion, "Latest version fetched from live UPM registry is empty/null.");
            Debug.Log($"[E2E Test] Successfully connected to live UPM registry! Unity Ads latest version: {unityAds.LatestVersion}");
        }

        [UnityTest]
        public IEnumerator TestUpmRegistryConnection_ValidatesAllNetworksMetadata()
        {
            if (UnityEngine.Application.isBatchMode)
            {
                UnityEngine.Debug.Log("[E2E Skip] Skipping E2E test in headless batch mode. Run locally in Unity Editor GUI!");
                yield break;
            }
            yield return RefreshDataHandlerCoroutine();
            var networks = DataHandler.GetNetworks();
            Assert.IsNotNull(networks, "Loaded networks list is null.");
            Assert.AreEqual(18, networks.Count, "The number of supported mediation networks should be exactly 18.");

            double startTime = EditorApplication.timeSinceStartup;
            while (networks.Any(n => n.IsFetchingLatest) && (EditorApplication.timeSinceStartup - startTime < TestTimeout))
            {
                yield return null;
            }

            foreach (var network in networks)
            {
                Assert.IsFalse(network.IsFetchingLatest, $"Timed out fetching latest version for package: {network.NetworkName}");
                Assert.IsNotEmpty(network.NetworkName, "Network package name is empty.");
                Debug.Log($"[E2E Test] Live Package: {network.NetworkName} ({network.DisplayName}) -> Latest Version: {network.LatestVersion}");
                Assert.IsNotEmpty(network.LatestVersion, $"Latest version fetched from live UPM registry is empty/null for package: {network.NetworkName}.");
            }
        }

        [UnityTest]
        public IEnumerator TestRealUi_WindowInitialization()
        {
            if (UnityEngine.Application.isBatchMode)
            {
                UnityEngine.Debug.Log("[E2E Skip] Skipping E2E test in headless batch mode. Run locally in Unity Editor GUI!");
                yield break;
            }
            yield return RefreshDataHandlerCoroutine();
            var view = CreateView();
            Assert.IsNotNull(view);

            Label titleLabel = view.Q<Label>();
            Assert.IsNotNull(titleLabel);
            Assert.AreEqual("Google Integration Manager", titleLabel.text);

            List<Label> labels = view.Query<Label>().ToList();
            Assert.IsTrue(labels.Any(l => l.text == "AppLovin"), "AppLovin row should be rendered in the UI.");
            Assert.IsTrue(labels.Any(l => l.text == "Chartboost"), "Chartboost row should be rendered in the UI.");
            Assert.IsTrue(labels.Any(l => l.text == "Unity Ads"), "Unity Ads row should be rendered in the UI.");
            Debug.Log("[E2E UI Test] Successfully verified real window initialization and row rendering!");
        }

        [UnityTest]
        public IEnumerator TestRealUi_TabSwitching()
        {
            if (UnityEngine.Application.isBatchMode)
            {
                UnityEngine.Debug.Log("[E2E Skip] Skipping E2E test in headless batch mode. Run locally in Unity Editor GUI!");
                yield break;
            }
            yield return RefreshDataHandlerCoroutine();
            int clickedTabIndex = -1;
            var view = CreateView(activeTabIndex: 0, onTabSwitched: (tabIndex) => clickedTabIndex = tabIndex);

            Button setupButton = view.Query<Button>().Where(b => b.text == "Setup SDK").First();
            Assert.IsNotNull(setupButton, "Setup SDK button should be rendered on Tab 0.");

            SimulateClick(setupButton);
            Assert.AreEqual(1, clickedTabIndex, "Clicking 'Setup SDK' should trigger callback to switch to Tab 1 (Settings).");
            Debug.Log("[E2E UI Test] Successfully verified real tab switching callback!");
        }

        [UnityTest]
        public IEnumerator TestRealUi_SettingsRenderingAndMutation()
        {
            if (UnityEngine.Application.isBatchMode)
            {
                UnityEngine.Debug.Log("[E2E Skip] Skipping E2E test in headless batch mode. Run locally in Unity Editor GUI!");
                yield break;
            }
            yield return RefreshDataHandlerCoroutine();

            // Seed our real settings asset in memory with known test values
            var s = Settings;
            s.GoogleMobileAdsAndroidAppId = "ca-app-pub-test-android";
            s.GoogleMobileAdsIOSAppId = "ca-app-pub-test-ios";
            s.DisableOptimizeInitialization = true;
            s.DisableOptimizeAdLoading = false;

            // Create view focused on the Settings tab (tabIndex 1)
            var view = CreateView(activeTabIndex: 1);
            Assert.IsNotNull(view);

            // 1. Verify UI correctly renders values from the real settings asset
            var androidField = view.Query<TextField>().Where(f => f.label == "Android").First();
            Assert.IsNotNull(androidField);
            Assert.AreEqual("ca-app-pub-test-android", androidField.value);

            var iosField = view.Query<TextField>().Where(f => f.label == "iOS").First();
            Assert.IsNotNull(iosField);
            Assert.AreEqual("ca-app-pub-test-ios", iosField.value);

            var initToggle = view.Query<Toggle>().Where(t => t.label == "Disable initialization optimization").First();
            Assert.IsNotNull(initToggle);
            Assert.IsTrue(initToggle.value);

            var adLoadingToggle = view.Query<Toggle>().Where(t => t.label == "Disable ad loading optimization").First();
            Assert.IsNotNull(adLoadingToggle);
            Assert.IsFalse(adLoadingToggle.value);

            // 2. Simulate user typing a new App ID in the UI field
            Debug.Log("[E2E UI Test] Simulating typing new App IDs and toggling optimizations in the UI...");
            SimulateValueChange(androidField, "ca-app-pub-mutated-android");
            SimulateValueChange(iosField, "ca-app-pub-mutated-ios");
            SimulateValueChange(initToggle, false);
            SimulateValueChange(adLoadingToggle, true);

            // 3. Verify the real underlying GoogleMobileAdsSettings asset was successfully mutated in memory!
            Assert.AreEqual("ca-app-pub-mutated-android", Settings.GoogleMobileAdsAndroidAppId, "Android App ID asset value should be mutated.");
            Assert.AreEqual("ca-app-pub-mutated-ios", Settings.GoogleMobileAdsIOSAppId, "iOS App ID asset value should be mutated.");
            Assert.IsFalse(Settings.DisableOptimizeInitialization, "Initialization optimization asset value should be mutated.");
            Assert.IsTrue(Settings.DisableOptimizeAdLoading, "Ad loading optimization asset value should be mutated.");

            Debug.Log("[E2E UI Test] Successfully verified real settings rendering and asset mutation through the UI!");
        }
    }
}
