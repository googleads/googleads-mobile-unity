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
    public class PictureInPictureAdClient : IPictureInPictureAdClient
    {
        public event EventHandler<EventArgs> OnAdLoaded;
        public event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;
        public event EventHandler<EventArgs> OnAdShown;
        public event EventHandler<EventArgs> OnAdHidden;
        public event EventHandler<EventArgs> OnAdDidRecordImpression;
        public event Action OnAdClicked;
        public event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;
        public event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;
        public event Action<AdValue> OnPaidEvent;

        private GameObject _gameObject;
        private RectTransform _cardRect;
        private PipSimulatorComponent _simulatorComponent;
        private string _adUnitId;
        private PictureInPictureAdPosition _currentPosition = PictureInPictureAdPosition.Default;
        private bool _isLoaded = false;
        private bool _didImpress = false;

        public void CreatePictureInPictureAd()
        {
            // No-op for Unity Editor simulator.
        }

        public void LoadAd(string adUnitId, AdRequest request)
        {
            _adUnitId = adUnitId;
            if (string.IsNullOrEmpty(adUnitId) || request == null)
            {
                if (OnAdFailedToLoad != null)
                {
                    OnAdFailedToLoad.Invoke(this, new LoadAdErrorClientEventArgs
                    {
                        LoadAdErrorClient = new LoadAdErrorClient()
                    });
                }
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
                if (OnAdFailedToLoad != null)
                {
                    OnAdFailedToLoad.Invoke(this, new LoadAdErrorClientEventArgs
                    {
                        LoadAdErrorClient = new LoadAdErrorClient()
                    });
                }
                return;
            }

            _gameObject = GameObject.Instantiate(prefab);
            Canvas canvas = _gameObject.GetComponent<Canvas>();
            if (canvas != null)
            {
                // Set to maximum sorting order to render on top of scene UI.
                canvas.sortingOrder = 32767;
            }

            // Remove existing Button component on prefab so dragging does not trigger button click
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

            // Create outer card container (#737373 gray frame with 12px rounded corners matching Android outerClippingRadiusDp)
            // 300x250 ad + 4px padding (top/left/right) + 32px padding (bottom) -> Size: 308 x 286
            GameObject cardObject = new GameObject("PipCardFrame");
            cardObject.transform.SetParent(_gameObject.transform, false);
            _cardRect = cardObject.AddComponent<RectTransform>();
            _cardRect.sizeDelta = new Vector2(308f, 286f);

            Image cardBg = cardObject.AddComponent<Image>();
            cardBg.sprite = GetOrCreateRoundedCardSprite(12);
            cardBg.type = Image.Type.Sliced;
            cardBg.color = Color.white;

            // Parent banner image inside an inner rounded mask (8px corner radius matching Android innerClippingRadiusDp)
            // This ensures the ad image corners do not poke out of the outer card's rounded corners.
            if (bannerImage != null)
            {
                GameObject maskObject = new GameObject("PipAdMaskContainer");
                maskObject.transform.SetParent(_cardRect, false);
                RectTransform maskRect = maskObject.AddComponent<RectTransform>();
                maskRect.anchorMin = new Vector2(0.5f, 1f);
                maskRect.anchorMax = new Vector2(0.5f, 1f);
                maskRect.pivot = new Vector2(0.5f, 1f);
                maskRect.sizeDelta = new Vector2(300f, 250f);
                maskRect.anchoredPosition = new Vector2(0f, -4f); // 4px padding from top

                Image maskImage = maskObject.AddComponent<Image>();
                maskImage.sprite = GetOrCreateRoundedMaskSprite(8);
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

            // Create bottom-left countdown & close button widget in the 32px bottom padding strip
            GameObject badgeObject = new GameObject("PipCloseCountdownBadge");
            badgeObject.transform.SetParent(_cardRect, false);
            RectTransform badgeRect = badgeObject.AddComponent<RectTransform>();
            badgeRect.anchorMin = new Vector2(0f, 0f);
            badgeRect.anchorMax = new Vector2(0f, 0f);
            badgeRect.pivot = new Vector2(0f, 0f);
            badgeRect.sizeDelta = new Vector2(24f, 24f);
            badgeRect.anchoredPosition = new Vector2(8f, 4f); // Bottom-left alignment in bottom strip

            Image badgeBg = badgeObject.AddComponent<Image>();
            badgeBg.sprite = GetOrCreateBadgeSprite(BadgeType.Countdown3);
            badgeBg.color = Color.white;

            Button closeButton = badgeObject.AddComponent<Button>();
            closeButton.interactable = false;

            // Attach simulator component to handle dragging, click separation, smooth snapping, and countdown
            _simulatorComponent = cardObject.AddComponent<PipSimulatorComponent>();
            _simulatorComponent.Initialize(this, badgeBg, closeButton);
            _simulatorComponent.OnCornerSnapped = (snappedCorner) =>
            {
                _currentPosition = snappedCorner;
            };

            _gameObject.SetActive(false);
            _isLoaded = true;
            _currentPosition = PictureInPictureAdPosition.Default;

            if (OnAdLoaded != null)
            {
                OnAdLoaded.Invoke(this, EventArgs.Empty);
            }
        }

        public void Show(PictureInPictureAdPosition position)
        {
            if (!_isLoaded || _gameObject == null)
            {
                Debug.LogWarning("PictureInPictureAd is not loaded.");
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

            if (OnAdShown != null)
            {
                OnAdShown.Invoke(this, EventArgs.Empty);
            }

            if (!_didImpress)
            {
                _didImpress = true;
                if (OnAdDidRecordImpression != null)
                {
                    OnAdDidRecordImpression.Invoke(this, EventArgs.Empty);
                }
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
            if (OnAdHidden != null)
            {
                OnAdHidden.Invoke(this, EventArgs.Empty);
            }
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

        public IResponseInfoClient GetResponseInfoClient()
        {
            return new ResponseInfoClient();
        }

        internal void RaiseAdClicked()
        {
            if (OnAdClicked != null)
            {
                OnAdClicked();
            }
            if (OnPaidEvent != null)
            {
                OnPaidEvent(new AdValue
                {
                    Value = 0,
                    CurrencyCode = "USD",
                    Precision = AdValue.PrecisionType.PublisherProvided
                });
            }
            Application.OpenURL("https://google.com");
        }

        internal enum BadgeType
        {
            Countdown3,
            Countdown2,
            Countdown1,
            CloseCross
        }

        private static readonly Dictionary<BadgeType, Sprite> _badgeSprites = new Dictionary<BadgeType, Sprite>();
        private static Sprite _roundedCardSprite;
        private static Sprite _roundedMaskSprite;

        internal static Sprite GetOrCreateRoundedCardSprite(int radius = 12)
        {
            if (_roundedCardSprite != null)
            {
                return _roundedCardSprite;
            }

            int size = radius * 2 + 8; // 32x32 texture with 12px corner radius
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color transparent = new Color(0f, 0f, 0f, 0f);
            Color cardColor = new Color(115f / 255f, 115f / 255f, 115f / 255f, 1f); // #737373

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
                        pixels[y * size + x] = new Color(cardColor.r, cardColor.g, cardColor.b, cardColor.a * alpha);
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            Vector4 border = new Vector4(radius, radius, radius, radius);
            _roundedCardSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, border);
            return _roundedCardSprite;
        }

        internal static Sprite GetOrCreateRoundedMaskSprite(int radius = 8)
        {
            if (_roundedMaskSprite != null)
            {
                return _roundedMaskSprite;
            }

            int size = radius * 2 + 8; // 24x24 texture with 8px corner radius
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
                        pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            Vector4 border = new Vector4(radius, radius, radius, radius);
            _roundedMaskSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, border);
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

        private void AnchorCardToCorner(PictureInPictureAdPosition position)
        {
            if (_cardRect == null)
            {
                return;
            }

            float width = _cardRect.sizeDelta.x;
            float height = _cardRect.sizeDelta.y;
            float halfWidth = width / 2f;
            float halfHeight = height / 2f;
            float padding = 20f;

            _cardRect.pivot = new Vector2(0.5f, 0.5f);

            switch (position)
            {
                case PictureInPictureAdPosition.TopLeft:
                    _cardRect.anchorMin = new Vector2(0, 1);
                    _cardRect.anchorMax = new Vector2(0, 1);
                    _cardRect.anchoredPosition = new Vector2(halfWidth + padding, -(halfHeight + padding));
                    break;
                case PictureInPictureAdPosition.TopRight:
                    _cardRect.anchorMin = new Vector2(1, 1);
                    _cardRect.anchorMax = new Vector2(1, 1);
                    _cardRect.anchoredPosition = new Vector2(-(halfWidth + padding), -(halfHeight + padding));
                    break;
                case PictureInPictureAdPosition.BottomLeft:
                    _cardRect.anchorMin = new Vector2(0, 0);
                    _cardRect.anchorMax = new Vector2(0, 0);
                    _cardRect.anchoredPosition = new Vector2(halfWidth + padding, halfHeight + padding);
                    break;
                case PictureInPictureAdPosition.BottomRight:
                case PictureInPictureAdPosition.Default:
                default:
                    _cardRect.anchorMin = new Vector2(1, 0);
                    _cardRect.anchorMax = new Vector2(1, 0);
                    _cardRect.anchoredPosition = new Vector2(-(halfWidth + padding), halfHeight + padding);
                    break;
            }
        }
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

        private IEnumerator CountdownRoutine()
        {
            if (_closeButton != null)
            {
                _closeButton.interactable = false;
            }

            if (_badgeImage != null)
            {
                _badgeImage.sprite = PictureInPictureAdClient.GetOrCreateBadgeSprite(PictureInPictureAdClient.BadgeType.Countdown3);
            }
            yield return new WaitForSeconds(1f);

            if (_badgeImage != null)
            {
                _badgeImage.sprite = PictureInPictureAdClient.GetOrCreateBadgeSprite(PictureInPictureAdClient.BadgeType.Countdown2);
            }
            yield return new WaitForSeconds(1f);

            if (_badgeImage != null)
            {
                _badgeImage.sprite = PictureInPictureAdClient.GetOrCreateBadgeSprite(PictureInPictureAdClient.BadgeType.Countdown1);
            }
            yield return new WaitForSeconds(1f);

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

        public void OnPointerDown(PointerEventData eventData)
        {
            _pointerDownPos = eventData.position;
            _isDragging = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;
            if (_snapCoroutine != null)
            {
                StopCoroutine(_snapCoroutine);
                _snapCoroutine = null;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (Vector2.Distance(eventData.position, _pointerDownPos) > 10f)
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

            if (_snapCoroutine != null)
            {
                StopCoroutine(_snapCoroutine);
            }
            _snapCoroutine = StartCoroutine(SmoothSnapRoutine(targetPosition));
        }

        private IEnumerator SmoothSnapRoutine(PictureInPictureAdPosition targetPosition)
        {
            if (_rectTransform == null)
            {
                yield break;
            }

            float width = _rectTransform.sizeDelta.x;
            float height = _rectTransform.sizeDelta.y;
            float halfWidth = width / 2f;
            float halfHeight = height / 2f;
            float padding = 20f;

            Vector2 targetAnchorMin;
            Vector2 targetAnchorMax;
            Vector2 targetAnchoredPos;

            switch (targetPosition)
            {
                case PictureInPictureAdPosition.TopLeft:
                    targetAnchorMin = new Vector2(0, 1);
                    targetAnchorMax = new Vector2(0, 1);
                    targetAnchoredPos = new Vector2(halfWidth + padding, -(halfHeight + padding));
                    break;
                case PictureInPictureAdPosition.TopRight:
                    targetAnchorMin = new Vector2(1, 1);
                    targetAnchorMax = new Vector2(1, 1);
                    targetAnchoredPos = new Vector2(-(halfWidth + padding), -(halfHeight + padding));
                    break;
                case PictureInPictureAdPosition.BottomLeft:
                    targetAnchorMin = new Vector2(0, 0);
                    targetAnchorMax = new Vector2(0, 0);
                    targetAnchoredPos = new Vector2(halfWidth + padding, halfHeight + padding);
                    break;
                case PictureInPictureAdPosition.BottomRight:
                case PictureInPictureAdPosition.Default:
                default:
                    targetAnchorMin = new Vector2(1, 0);
                    targetAnchorMax = new Vector2(1, 0);
                    targetAnchoredPos = new Vector2(-(halfWidth + padding), halfHeight + padding);
                    break;
            }

            // Convert current world position to target anchor local coordinate
            Vector3 worldPos = _rectTransform.position;
            _rectTransform.anchorMin = targetAnchorMin;
            _rectTransform.anchorMax = targetAnchorMax;
            _rectTransform.position = worldPos;

            Vector2 startPos = _rectTransform.anchoredPosition;
            float duration = 0.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                // S-curve smoothstep for natural, gradual deceleration
                float smoothT = Mathf.SmoothStep(0f, 1f, t);
                _rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetAnchoredPos, smoothT);
                yield return null;
            }

            _rectTransform.anchoredPosition = targetAnchoredPos;
            _snapCoroutine = null;

            if (OnCornerSnapped != null)
            {
                OnCornerSnapped(targetPosition);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Do not fire ad click if user was dragging or clicked on the close button
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
