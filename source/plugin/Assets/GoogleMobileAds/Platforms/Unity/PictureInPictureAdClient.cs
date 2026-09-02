// Copyright 2026 Google LLC
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Unity
{
    public class PictureInPictureAdClient : BaseAdClient, IPictureInPictureAdClient
    {
        #region Constants

        private const int MaxCanvasSortingOrder = 32767;
        private const float CardWidth = 308f;
        private const float CardHeight = 286f;
        private const float AdWidth = 300f;
        private const float AdHeight = 250f;
        private const float TopPadding = 4f;
        private const float BottomPadding = 32f;
        internal const float CornerPadding = 20f;
        private const float BadgeSize = 24f;
        private const float BadgeOffsetX = 8f;
        private const float BadgeOffsetY = 4f;
        internal const float DragThreshold = 10f;
        internal const float SnapDuration = 0.5f;
        internal const float CountdownIntervalSeconds = 1.0f;
        private const int OuterCardRadius = 12;
        private const int InnerMaskRadius = 8;
        private static readonly Color CardBackgroundColor = new Color(115f / 255f, 115f / 255f, 115f / 255f, 1f); // #737373

        #endregion

        #region Events

        public event Action<AdValue> OnPaidEvent;
        public event Action OnAdClicked;
        public event Action OnAdDidRecordImpression;
        public event Action OnAdLoaded;
        public event Action<LoadAdErrorClientEventArgs> OnAdFailedToLoad;
        public event Action OnAdShown;
        public event Action OnAdHidden;
        public event Action<AdErrorClientEventArgs> OnAdFailedToPresentFullScreenContent;
        public event Action OnAdDidPresentFullScreenContent;
        public event Action OnAdDidDismissFullScreenContent;

        #endregion

        #region Fields

        private GameObject _gameObject;
        private RectTransform _cardRect;
        private PipSimulatorComponent _simulatorComponent;
        private PictureInPictureAdPosition _currentPosition = PictureInPictureAdPosition.Default;
        private bool _isLoaded = false;
        private bool _didImpress = false;

        private static readonly Dictionary<BadgeType, Sprite> _badgeSprites = new Dictionary<BadgeType, Sprite>();
        private static Sprite _roundedCardSprite;
        private static Sprite _roundedMaskSprite;

        #endregion

        #region IPictureInPictureAdClient Implementation

        public void CreatePictureInPictureAd()
        {
            // No-op for Unity Editor simulator.
        }

        public void LoadAd(string adUnitId, AdRequest request)
        {
            _adUnitId = adUnitId;
            if (string.IsNullOrEmpty(adUnitId) || request == null)
            {
                RaiseAdFailedToLoad();
                return;
            }

            GameObject prefab = Resources.Load("PlaceholderAds/Banners/MEDIUM_RECTANGLE") as GameObject;
            if (prefab == null)
            {
                prefab = Resources.Load("PlaceholderAds/Banners/BANNER") as GameObject;
            }
            if (prefab == null)
            {
                Debug.LogError("No PiP placeholder prefab found in Resources.");
                RaiseAdFailedToLoad();
                return;
            }

            _gameObject = GameObject.Instantiate(prefab);
            Canvas canvas = _gameObject.GetComponent<Canvas>();
            if (canvas != null)
            {
                // Set to maximum sorting order to render on top of scene UI.
                canvas.sortingOrder = MaxCanvasSortingOrder;
            }

            // Remove existing Button component on prefab so dragging does not trigger button click.
            Button existingButton = _gameObject.GetComponentInChildren<Button>();
            Image bannerImage = _gameObject.GetComponentInChildren<Image>();
            Text bannerText = _gameObject.GetComponentInChildren<Text>();

            if (bannerText != null)
            {
                bannerText.text = "Picture-in-Picture Test Ad";
            }

            if (existingButton != null)
            {
                GameObject.Destroy(existingButton);
            }

            // Create outer card container (#737373 gray frame with 12px rounded corners matching Android outerClippingRadiusDp).
            // 300x250 ad + 4px padding (top/left/right) + 32px padding (bottom) -> Size: 308 x 286.
            GameObject cardObject = new GameObject("PipCardFrame");
            cardObject.transform.SetParent(_gameObject.transform, false);
            _cardRect = cardObject.AddComponent<RectTransform>();
            _cardRect.sizeDelta = new Vector2(CardWidth, CardHeight);

            Image cardBg = cardObject.AddComponent<Image>();
            cardBg.sprite = GetOrCreateRoundedCardSprite(OuterCardRadius);
            cardBg.type = Image.Type.Sliced;
            cardBg.color = Color.white;

            // Parent banner image inside an inner rounded mask (8px corner radius matching innerClippingRadiusDp).
            // This ensures the ad image corners do not poke out of the outer card's rounded corners.
            if (bannerImage != null)
            {
                GameObject maskObject = new GameObject("PipAdMaskContainer");
                maskObject.transform.SetParent(_cardRect, false);
                RectTransform maskRect = maskObject.AddComponent<RectTransform>();
                maskRect.anchorMin = new Vector2(0.5f, 1f);
                maskRect.anchorMax = new Vector2(0.5f, 1f);
                maskRect.pivot = new Vector2(0.5f, 1f);
                maskRect.sizeDelta = new Vector2(AdWidth, AdHeight);
                maskRect.anchoredPosition = new Vector2(0f, -TopPadding);

                Image maskImage = maskObject.AddComponent<Image>();
                maskImage.sprite = GetOrCreateRoundedMaskSprite(InnerMaskRadius);
                maskImage.type = Image.Type.Sliced;

                Mask mask = maskObject.AddComponent<Mask>();
                mask.showMaskGraphic = false;

                RectTransform bannerRect = bannerImage.GetComponent<RectTransform>();
                bannerRect.SetParent(maskRect, false);
                bannerRect.anchorMin = Vector2.zero;
                bannerRect.anchorMax = Vector2.one;
                bannerRect.pivot = new Vector2(0.5f, 0.5f);
                bannerRect.sizeDelta = Vector2.zero;
                bannerRect.anchoredPosition = Vector2.zero;
            }

            // Create bottom-left countdown & close button widget in the bottom padding strip.
            GameObject badgeObject = new GameObject("PipCloseCountdownBadge");
            badgeObject.transform.SetParent(_cardRect, false);
            RectTransform badgeRect = badgeObject.AddComponent<RectTransform>();
            badgeRect.anchorMin = Vector2.zero;
            badgeRect.anchorMax = Vector2.zero;
            badgeRect.pivot = Vector2.zero;
            badgeRect.sizeDelta = new Vector2(BadgeSize, BadgeSize);
            badgeRect.anchoredPosition = new Vector2(BadgeOffsetX, BadgeOffsetY);

            Image badgeBg = badgeObject.AddComponent<Image>();
            badgeBg.sprite = GetOrCreateBadgeSprite(BadgeType.Countdown3);
            badgeBg.color = Color.white;

            Button closeButton = badgeObject.AddComponent<Button>();
            closeButton.interactable = false;

            // Attach simulator component to handle dragging, click separation, smooth snapping, and countdown.
            _simulatorComponent = cardObject.AddComponent<PipSimulatorComponent>();
            _simulatorComponent.Initialize(this, badgeBg, closeButton);
            _simulatorComponent.OnCornerSnapped = (snappedCorner) =>
            {
                _currentPosition = snappedCorner;
            };

            _gameObject.SetActive(false);
            _isLoaded = true;
            _currentPosition = PictureInPictureAdPosition.Default;
            OnAdLoaded?.Invoke();
        }

        public void Show(PictureInPictureAdPosition position)
        {
            if (!_isLoaded || _gameObject == null)
            {
                Debug.LogWarning("PictureInPictureAd is not loaded.");
                OnAdFailedToPresentFullScreenContent?.Invoke(new AdErrorClientEventArgs
                {
                    AdErrorClient = new AdErrorClient()
                });
                return;
            }

            PictureInPictureAdPosition resolvedPosition;
            if (position == PictureInPictureAdPosition.Default)
            {
                resolvedPosition = (_currentPosition != PictureInPictureAdPosition.Default)
                    ? _currentPosition
                    : PictureInPictureAdPosition.BottomRight;
            }
            else
            {
                resolvedPosition = position;
            }

            _currentPosition = resolvedPosition;
            AnchorCardToCorner(resolvedPosition);

            _gameObject.SetActive(true);

            if (_simulatorComponent != null)
            {
                _simulatorComponent.StartCountdown();
            }

            OnAdShown?.Invoke();

            if (!_didImpress)
            {
                _didImpress = true;
                OnAdDidRecordImpression?.Invoke();
            }
        }

        public void Hide()
        {
            if (_simulatorComponent != null)
            {
                _simulatorComponent.StopCountdown();
            }
            if (_gameObject != null)
            {
                _gameObject.SetActive(false);
            }
            OnAdHidden?.Invoke();
        }

        public void Destroy()
        {
            if (_simulatorComponent != null)
            {
                _simulatorComponent.StopCountdown();
            }
            if (_gameObject != null)
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(_gameObject);
                }
                else
                {
                    GameObject.DestroyImmediate(_gameObject);
                }
                _gameObject = null;
            }
            _isLoaded = false;
            _currentPosition = PictureInPictureAdPosition.Default;
        }

        public PictureInPictureAdPosition GetAdPosition()
        {
            return _currentPosition;
        }

        #endregion

        #region Helper Methods

        private void RaiseAdFailedToLoad()
        {
            OnAdFailedToLoad?.Invoke(new LoadAdErrorClientEventArgs
            {
                LoadAdErrorClient = new LoadAdErrorClient()
            });
        }

        internal void RaiseAdClicked()
        {
            OnAdClicked?.Invoke();
            OnAdDidPresentFullScreenContent?.Invoke();
            Application.OpenURL("https://google.com");
            OnPaidEvent?.Invoke(new AdValue
            {
                Value = 2000,
                CurrencyCode = "USD",
                Precision = AdValue.PrecisionType.PublisherProvided
            });
            OnAdDidDismissFullScreenContent?.Invoke();
        }

        internal static void GetCornerAnchorsAndPosition(
            PictureInPictureAdPosition position,
            Vector2 sizeDelta,
            out Vector2 anchor,
            out Vector2 anchoredPosition)
        {
            float offsetX = sizeDelta.x / 2f + CornerPadding;
            float offsetY = sizeDelta.y / 2f + CornerPadding;

            switch (position)
            {
                case PictureInPictureAdPosition.TopLeft:
                    anchor = new Vector2(0f, 1f);
                    anchoredPosition = new Vector2(offsetX, -offsetY);
                    break;
                case PictureInPictureAdPosition.TopRight:
                    anchor = new Vector2(1f, 1f);
                    anchoredPosition = new Vector2(-offsetX, -offsetY);
                    break;
                case PictureInPictureAdPosition.BottomLeft:
                    anchor = new Vector2(0f, 0f);
                    anchoredPosition = new Vector2(offsetX, offsetY);
                    break;
                case PictureInPictureAdPosition.BottomRight:
                case PictureInPictureAdPosition.Default:
                default:
                    anchor = new Vector2(1f, 0f);
                    anchoredPosition = new Vector2(-offsetX, offsetY);
                    break;
            }
        }

        private void AnchorCardToCorner(PictureInPictureAdPosition position)
        {
            if (_cardRect == null)
            {
                return;
            }

            GetCornerAnchorsAndPosition(position, _cardRect.sizeDelta, out Vector2 anchor, out Vector2 anchoredPosition);
            _cardRect.pivot = new Vector2(0.5f, 0.5f);
            _cardRect.anchorMin = anchor;
            _cardRect.anchorMax = anchor;
            _cardRect.anchoredPosition = anchoredPosition;
        }

        #endregion

        #region Sprite Generation

        internal enum BadgeType
        {
            Countdown3,
            Countdown2,
            Countdown1,
            CloseCross
        }

        private static Sprite CreateRoundedSprite(int radius, Color baseColor)
        {
            int size = radius * 2 + 8;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color transparent = new Color(0f, 0f, 0f, 0f);
            Color[] pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float cx = (x < radius) ? radius : ((x >= size - radius) ? (size - 1 - radius) : x);
                    float cy = (y < radius) ? radius : ((y >= size - radius) ? (size - 1 - radius) : y);

                    float dx = x - cx;
                    float dy = y - cy;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    if (dist > radius + 1f)
                    {
                        pixels[y * size + x] = transparent;
                    }
                    else
                    {
                        float alpha = Mathf.Clamp01(radius + 1f - dist);
                        pixels[y * size + x] = new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a * alpha);
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            Vector4 border = new Vector4(radius, radius, radius, radius);
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, border);
        }

        internal static Sprite GetOrCreateRoundedCardSprite(int radius = OuterCardRadius)
        {
            if (_roundedCardSprite == null)
            {
                _roundedCardSprite = CreateRoundedSprite(radius, CardBackgroundColor);
            }
            return _roundedCardSprite;
        }

        internal static Sprite GetOrCreateRoundedMaskSprite(int radius = InnerMaskRadius)
        {
            if (_roundedMaskSprite == null)
            {
                _roundedMaskSprite = CreateRoundedSprite(radius, Color.white);
            }
            return _roundedMaskSprite;
        }

        internal static Sprite GetOrCreateBadgeSprite(BadgeType type)
        {
            Sprite cachedSprite;
            if (_badgeSprites.TryGetValue(type, out cachedSprite) && cachedSprite != null)
            {
                return cachedSprite;
            }

            int size = 64;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color transparent = new Color(0f, 0f, 0f, 0f);
            Color whiteCircle = new Color(1f, 1f, 1f, 0.95f);
            Color blackInk = new Color(0.1f, 0.1f, 0.1f, 1f);

            float center = (size - 1) / 2f;
            float radius = (size / 2f) - 2f;

            Color[] pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - center;
                    float dy = y - center;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    if (dist > radius + 1f)
                    {
                        pixels[y * size + x] = transparent;
                    }
                    else
                    {
                        float alpha = Mathf.Clamp01(radius + 1f - dist);
                        Color pixel = new Color(whiteCircle.r, whiteCircle.g, whiteCircle.b, whiteCircle.a * alpha);

                        bool isInk = false;
                        if (type == BadgeType.CloseCross)
                        {
                            float dist1 = Mathf.Abs(dx - dy) / 1.41421356f;
                            float dist2 = Mathf.Abs(dx + dy) / 1.41421356f;
                            float maxCoord = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));
                            float crossWidth = size * 0.08f;
                            float crossLength = size * 0.28f;

                            if (maxCoord <= crossLength && (dist1 <= crossWidth || dist2 <= crossWidth))
                            {
                                float crossDist = Mathf.Min(dist1, dist2);
                                float crossAlpha = Mathf.Clamp01(crossWidth + 0.5f - crossDist);
                                pixel = Color.Lerp(pixel, blackInk, crossAlpha * alpha);
                            }
                        }
                        else if (type == BadgeType.Countdown3)
                        {
                            if ((dy >= 14 && dy <= 20 && dx >= -8 && dx <= 8) ||
                                (dy >= 0 && dy <= 18 && dx >= 2 && dx <= 8) ||
                                (dy >= -3 && dy <= 3 && dx >= -2 && dx <= 8) ||
                                (dy >= -18 && dy <= 0 && dx >= 2 && dx <= 8) ||
                                (dy >= -20 && dy <= -14 && dx >= -8 && dx <= 8))
                            {
                                isInk = true;
                            }
                        }
                        else if (type == BadgeType.Countdown2)
                        {
                            if ((dy >= 14 && dy <= 20 && dx >= -8 && dx <= 8) ||
                                (dy >= 0 && dy <= 18 && dx >= 2 && dx <= 8) ||
                                (dy >= -3 && dy <= 3 && dx >= -8 && dx <= 8) ||
                                (dy >= -18 && dy <= 0 && dx >= -8 && dx <= -2) ||
                                (dy >= -20 && dy <= -14 && dx >= -8 && dx <= 8))
                            {
                                isInk = true;
                            }
                        }
                        else if (type == BadgeType.Countdown1)
                        {
                            if ((dy >= -18 && dy <= 20 && dx >= -2 && dx <= 4) ||
                                (dy >= 10 && dy <= 18 && dx >= -8 && dx <= 0) ||
                                (dy >= -20 && dy <= -14 && dx >= -8 && dx <= 10))
                            {
                                isInk = true;
                            }
                        }

                        if (isInk)
                        {
                            pixel = Color.Lerp(pixel, blackInk, alpha);
                        }

                        pixels[y * size + x] = pixel;
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
            _badgeSprites[type] = sprite;
            return sprite;
        }

        #endregion
    }

    internal class PipSimulatorComponent : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        public Action<PictureInPictureAdPosition> OnCornerSnapped;
        private PictureInPictureAdClient _client;
        private Image _badgeImage;
        private Button _closeButton;
        private RectTransform _rectTransform;
        private Canvas _canvas;
        private Vector2 _pointerDownPos;
        private bool _isDragging = false;
        private Coroutine _countdownCoroutine;
        private Coroutine _snapCoroutine;

        public void Initialize(PictureInPictureAdClient client, Image badgeImage, Button closeButton)
        {
            _client = client;
            _badgeImage = badgeImage;
            _closeButton = closeButton;
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();

            if (_closeButton != null)
            {
                _closeButton.onClick.AddListener(() =>
                {
                    if (_client != null)
                    {
                        _client.Hide();
                    }
                });
            }
        }

        public void StartCountdown()
        {
            StopCountdown();
            _countdownCoroutine = StartCoroutine(CountdownRoutine());
        }

        public void StopCountdown()
        {
            if (_countdownCoroutine != null)
            {
                StopCoroutine(_countdownCoroutine);
                _countdownCoroutine = null;
            }
        }

        private static readonly PictureInPictureAdClient.BadgeType[] CountdownSteps = new[]
        {
            PictureInPictureAdClient.BadgeType.Countdown3,
            PictureInPictureAdClient.BadgeType.Countdown2,
            PictureInPictureAdClient.BadgeType.Countdown1
        };

        private IEnumerator CountdownRoutine()
        {
            if (_closeButton != null)
            {
                _closeButton.interactable = false;
            }

            var waitInterval = new WaitForSeconds(PictureInPictureAdClient.CountdownIntervalSeconds);
            foreach (var badge in CountdownSteps)
            {
                if (_badgeImage != null)
                {
                    _badgeImage.sprite = PictureInPictureAdClient.GetOrCreateBadgeSprite(badge);
                }
                yield return waitInterval;
            }

            if (_badgeImage != null)
            {
                _badgeImage.sprite = PictureInPictureAdClient.GetOrCreateBadgeSprite(PictureInPictureAdClient.BadgeType.CloseCross);
            }
            if (_closeButton != null)
            {
                _closeButton.interactable = true;
            }
            _countdownCoroutine = null;
        }

        public void StopSnapAnimation()
        {
            if (_snapCoroutine != null)
            {
                StopCoroutine(_snapCoroutine);
                _snapCoroutine = null;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _pointerDownPos = eventData.position;
            _isDragging = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;
            StopSnapAnimation();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (Vector2.Distance(eventData.position, _pointerDownPos) > PictureInPictureAdClient.DragThreshold)
            {
                _isDragging = true;
            }

            if (_rectTransform != null && _canvas != null)
            {
                _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_rectTransform == null)
            {
                return;
            }

            Vector2 center = _rectTransform.position;
            float screenMidX = Screen.width / 2f;
            float screenMidY = Screen.height / 2f;

            PictureInPictureAdPosition targetPosition;
            if (center.x > screenMidX && center.y < screenMidY)
            {
                targetPosition = PictureInPictureAdPosition.BottomRight;
            }
            else if (center.x <= screenMidX && center.y < screenMidY)
            {
                targetPosition = PictureInPictureAdPosition.BottomLeft;
            }
            else if (center.x <= screenMidX && center.y >= screenMidY)
            {
                targetPosition = PictureInPictureAdPosition.TopLeft;
            }
            else
            {
                targetPosition = PictureInPictureAdPosition.TopRight;
            }

            StopSnapAnimation();
            _snapCoroutine = StartCoroutine(SmoothSnapRoutine(targetPosition));
        }

        private IEnumerator SmoothSnapRoutine(PictureInPictureAdPosition targetPosition)
        {
            if (_rectTransform == null)
            {
                yield break;
            }

            PictureInPictureAdClient.GetCornerAnchorsAndPosition(
                targetPosition,
                _rectTransform.sizeDelta,
                out Vector2 targetAnchor,
                out Vector2 targetAnchoredPos);

            // Convert current world position to target anchor local coordinate.
            Vector3 worldPos = _rectTransform.position;
            _rectTransform.anchorMin = targetAnchor;
            _rectTransform.anchorMax = targetAnchor;
            _rectTransform.position = worldPos;

            Vector2 startPos = _rectTransform.anchoredPosition;
            float elapsed = 0f;

            while (elapsed < PictureInPictureAdClient.SnapDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / PictureInPictureAdClient.SnapDuration);
                // S-curve smoothstep for natural, gradual deceleration.
                float smoothT = Mathf.SmoothStep(0f, 1f, t);
                _rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetAnchoredPos, smoothT);
                yield return null;
            }

            _rectTransform.anchoredPosition = targetAnchoredPos;
            _snapCoroutine = null;
            OnCornerSnapped?.Invoke(targetPosition);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Do not fire ad click if user was dragging or clicked on the close button.
            if (_isDragging)
            {
                return;
            }

            if (_closeButton != null && eventData.pointerPressRaycast.gameObject == _closeButton.gameObject)
            {
                return;
            }

            if (_client != null)
            {
                _client.RaiseAdClicked();
            }
        }
    }
}
