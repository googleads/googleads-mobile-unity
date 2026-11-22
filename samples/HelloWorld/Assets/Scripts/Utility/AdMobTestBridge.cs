// Copyright 2026 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GoogleMobileAds.Samples.Utility
{
    /// <summary>
    /// Test bridge exposing UIKit accessibility elements and test event tracking for automated tests.
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/Utility/AdMobTestBridge")]
    public class AdMobTestBridge : MonoBehaviour
    {
        private static AdMobTestBridge _instance;
        private static readonly Dictionary<string, Button> _registeredButtons =
            new Dictionary<string, Button>();

#if UNITY_IPHONE && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void AdMobTest_Init();

        [DllImport("__Internal")]
        private static extern void AdMobTest_SetLastEvent(string eventName);

        [DllImport("__Internal")]
        private static extern void AdMobTest_RegisterButton(
            string buttonId, float normX, float normY, float normWidth, float normHeight);

        [DllImport("__Internal")]
        private static extern void AdMobTest_ClearButtons();
#endif

        /// <summary>
        /// Automatically initializes the bridge before any scene loads.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            Init();
        }

        /// <summary>
        /// Initializes the test bridge singleton.
        /// </summary>
        public static void Init()
        {
            if (_instance != null)
            {
                return;
            }

            var go = new GameObject("AdMobTestBridge");
            _instance = go.AddComponent<AdMobTestBridge>();
            DontDestroyOnLoad(go);

#if UNITY_IPHONE && !UNITY_EDITOR
            AdMobTest_Init();
#endif
            Debug.Log("[AdMobTestBridge] Initialized successfully.");
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Start()
        {
            StartCoroutine(PeriodicSyncButtons());
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StartCoroutine(SyncButtonsDelayed());
        }

        /// <summary>
        /// Records a test event to the native UIKit accessibility status label.
        /// </summary>
        public static void RecordEvent(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return;
            }

            Debug.Log("[AdMobTestBridge] Test event: " + eventName);

#if UNITY_IPHONE && !UNITY_EDITOR
            AdMobTest_SetLastEvent(eventName);
#endif
        }

        /// <summary>
        /// Dispatches a button click initiated from native XCUITest overlay.
        /// </summary>
        public void TriggerButtonClick(string buttonId)
        {
            if (string.IsNullOrEmpty(buttonId))
            {
                return;
            }

            Debug.Log("[AdMobTestBridge] TriggerButtonClick received for: " + buttonId);

            if (_registeredButtons.TryGetValue(buttonId, out Button button) && button != null)
            {
                if (!button.gameObject.activeInHierarchy || !button.interactable)
                {
                    Debug.LogWarning(
                        "[AdMobTestBridge] Button is inactive or non-interactable: " + buttonId);
                    return;
                }

                if (EventSystem.current != null)
                {
                    var data = new PointerEventData(EventSystem.current);
                    ExecuteEvents.Execute(
                        button.gameObject, data, ExecuteEvents.pointerClickHandler);
                }

                button.onClick.Invoke();

                if (button is SceneMenuButton sceneMenuBtn &&
                    !string.IsNullOrEmpty(sceneMenuBtn.SceneToLoadName))
                {
                    SceneManager.LoadScene(sceneMenuBtn.SceneToLoadName);
                }
            }
            else
            {
                Debug.LogWarning("[AdMobTestBridge] Button not registered: " + buttonId);
            }
        }

        /// <summary>
        /// Manually triggers a synchronization of active buttons with the native bridge.
        /// </summary>
        public static void SyncButtons()
        {
            if (_instance != null)
            {
                _instance.StartCoroutine(_instance.SyncButtonsDelayed());
            }
        }

        private IEnumerator PeriodicSyncButtons()
        {
            var wait = new WaitForSeconds(0.5f);
            while (true)
            {
                yield return wait;
                RegisterAllActiveButtons();
            }
        }

        private IEnumerator SyncButtonsDelayed()
        {
            yield return null;
            yield return new WaitForEndOfFrame();
            RegisterAllActiveButtons();
        }

        private void RegisterAllActiveButtons()
        {
            _registeredButtons.Clear();

#if UNITY_IPHONE && !UNITY_EDITOR
            AdMobTest_ClearButtons();
#endif

            Button[] buttons = FindObjectsOfType<Button>();
            if (buttons == null || buttons.Length == 0)
            {
                return;
            }

            int screenWidth = Screen.width;
            int screenHeight = Screen.height;
            if (screenWidth <= 0 || screenHeight <= 0)
            {
                return;
            }

            var corners = new Vector3[4];
            foreach (Button button in buttons)
            {
                if (button == null || !button.gameObject.activeInHierarchy)
                {
                    continue;
                }

                var rectTransform = button.GetComponent<RectTransform>();
                if (rectTransform == null)
                {
                    continue;
                }

                rectTransform.GetWorldCorners(corners);
                Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
                Camera cam = (canvas != null &&
                              canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                    ? canvas.worldCamera
                    : null;

                Vector2 bottomLeft = RectTransformUtility.WorldToScreenPoint(cam, corners[0]);
                Vector2 topRight = RectTransformUtility.WorldToScreenPoint(cam, corners[2]);

                float width = topRight.x - bottomLeft.x;
                float height = topRight.y - bottomLeft.y;
                if (width <= 0 || height <= 0)
                {
                    continue;
                }

                float normX = bottomLeft.x / (float)screenWidth;
                float normY = ((float)screenHeight - topRight.y) / (float)screenHeight;
                float normW = width / (float)screenWidth;
                float normH = height / (float)screenHeight;

                RegisterButton(button.gameObject.name, button, normX, normY, normW, normH);

                Text text = button.GetComponentInChildren<Text>();
                if (text != null && !string.IsNullOrEmpty(text.text))
                {
                    RegisterButton(text.text.Trim(), button, normX, normY, normW, normH);
                }
            }
        }

        private void RegisterButton(
            string id, Button button, float normX, float normY, float normW, float normH)
        {
            if (string.IsNullOrEmpty(id) || button == null)
            {
                return;
            }

            _registeredButtons[id] = button;

#if UNITY_IPHONE && !UNITY_EDITOR
            AdMobTest_RegisterButton(id, normX, normY, normW, normH);
#endif
        }
    }
}
