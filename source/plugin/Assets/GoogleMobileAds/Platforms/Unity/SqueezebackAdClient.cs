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
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Unity
{
    public class SqueezebackAdClient : BaseAdClient, ISqueezebackAdClient
    {
        #region Constants

        internal const int BackdropSortingOrder = -100;
        internal const int AnimatingControlsSortingOrder = -99;
        internal const int ActiveControlsSortingOrder = 32767;
        internal const float ContentScaleFactor = 0.75f;
        internal const float TopOffsetNormalized = 0.035f;
        internal const float AnimationDurationSeconds = 0.4f;
        internal const float CountdownIntervalSeconds = 1.0f;
        private const int PillCornerRadius = 20;

        private static readonly Color InitialCtaColor = new Color(0.32f, 0.34f, 0.37f, 1f);
        private static Color _dominantCreativeColor = new Color(0.67f, 0.70f, 0.11f, 1f);

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

        private static SqueezebackAdClient _activeShowingClient;

        private GameObject _rootObject;
        private SqueezebackSimulatorComponent _simulatorComponent;
        private bool _isLoaded = false;
        private bool _isShowing = false;
        private bool _isBeingDestroyed = false;
        private bool _isClosedByUser = false;
        private bool _didImpress = false;

        private static Sprite _appRoundedMaskSprite;
        private static Sprite _pillWhiteSprite;
        private static Sprite _adBadgeSprite;
        private static Sprite _gradientBackdropSprite;
        private static Sprite _floodItIconSprite;
        private static Sprite _googlePlayIconSprite;
        private static Sprite _adChoicesIconSprite;
        private static readonly Dictionary<PictureInPictureAdClient.BadgeType, Sprite> _squeezebackBadgeSprites =
            new Dictionary<PictureInPictureAdClient.BadgeType, Sprite>();

        #endregion

        #region ISqueezebackAdClient Implementation

        public void LoadAd(string adUnitId, AdRequest request)
        {
            _adUnitId = adUnitId;
            if (string.IsNullOrEmpty(adUnitId) || request == null)
            {
                RaiseAdFailedToLoad();
                return;
            }

            CleanupRootObject();

            _rootObject = new GameObject("SqueezebackAdSimulator");
            _simulatorComponent = _rootObject.AddComponent<SqueezebackSimulatorComponent>();
            _simulatorComponent.BuildUi(this);

            _rootObject.SetActive(false);
            _isLoaded = true;
            _isShowing = false;
            _isBeingDestroyed = false;
            _isClosedByUser = false;
            _didImpress = false;

            OnAdLoaded?.Invoke();
        }

        public void Show()
        {
            if (!_isLoaded || _rootObject == null || _isBeingDestroyed)
            {
                Debug.LogWarning("SqueezebackAd is not loaded or has been destroyed.");
                RaiseAdFailedToPresent();
                return;
            }

            if (_isClosedByUser)
            {
                Debug.LogWarning("SqueezebackAd was closed by the user and cannot be shown again. Please load a new ad.");
                RaiseAdFailedToPresent();
                return;
            }

            if (_activeShowingClient != null)
            {
                Debug.LogWarning("Failed to show because a SqueezebackAd is already showing. Only one SqueezebackAd can be displayed at a time.");
                RaiseAdFailedToPresent();
                return;
            }

            _isShowing = true;
            _activeShowingClient = this;
            _rootObject.SetActive(true);

            if (_simulatorComponent != null)
            {
                _simulatorComponent.PlayShowAnimation(() =>
                {
                    OnAdShown?.Invoke();

                    if (!_didImpress)
                    {
                        _didImpress = true;
                        OnAdDidRecordImpression?.Invoke();
                        OnPaidEvent?.Invoke(new AdValue
                        {
                            Value = 2500,
                            CurrencyCode = "USD",
                            Precision = AdValue.PrecisionType.PublisherProvided
                        });
                    }
                });
            }
        }

        public void Hide()
        {
            if (_isBeingDestroyed)
            {
                return;
            }

            if (!_isShowing)
            {
                Debug.LogWarning("SqueezebackAd is not currently showing.");
                return;
            }

            if (_simulatorComponent != null && _rootObject != null && _rootObject.activeInHierarchy)
            {
                _simulatorComponent.PlayHideAnimation(() =>
                {
                    if (_rootObject != null)
                    {
                        _rootObject.SetActive(false);
                    }
                    CompleteHide();
                });
            }
            else
            {
                CompleteHide();
            }
        }

        internal void HideFromUserClose()
        {
            _isClosedByUser = true;
            Hide();
        }

        private void CompleteHide()
        {
            _isShowing = false;
            if (_activeShowingClient == this)
            {
                _activeShowingClient = null;
            }
            OnAdHidden?.Invoke();
        }

        public void Destroy()
        {
            if (_isBeingDestroyed)
            {
                return;
            }
            _isBeingDestroyed = true;

            bool wasShowing = _isShowing;
            _isShowing = false;
            _isLoaded = false;

            if (_activeShowingClient == this)
            {
                _activeShowingClient = null;
            }

            if (_simulatorComponent != null)
            {
                _simulatorComponent.RestoreSceneImmediately();
            }

            CleanupRootObject();

            if (wasShowing)
            {
                OnAdHidden?.Invoke();
            }
        }

        #endregion

        #region Helper & Sprite Methods

        internal static void DestroyObject(UnityEngine.Object obj)
        {
            if (obj == null)
            {
                return;
            }
            if (Application.isPlaying)
            {
                GameObject.Destroy(obj);
            }
            else
            {
                GameObject.DestroyImmediate(obj);
            }
        }

        private void CleanupRootObject()
        {
            if (_rootObject != null)
            {
                DestroyObject(_rootObject);
                _rootObject = null;
                _simulatorComponent = null;
            }
        }

        private void RaiseAdFailedToLoad()
        {
            OnAdFailedToLoad?.Invoke(new LoadAdErrorClientEventArgs
            {
                LoadAdErrorClient = new LoadAdErrorClient()
            });
        }

        private void RaiseAdFailedToPresent()
        {
            OnAdFailedToPresentFullScreenContent?.Invoke(new AdErrorClientEventArgs
            {
                AdErrorClient = new AdErrorClient()
            });
        }

        internal void RaiseAdClicked()
        {
            OnAdClicked?.Invoke();
            OnAdDidPresentFullScreenContent?.Invoke();
            Application.OpenURL("https://google.com");
            OnAdDidDismissFullScreenContent?.Invoke();
        }

        private static float ComputeRoundedCornerAlpha(int x, int y, int width, int height, int radius)
        {
            float dx = x < radius ? radius - (x + 0.5f) : (x >= width - radius ? (x + 0.5f) - (width - radius) : 0f);
            float dy = y < radius ? radius - (y + 0.5f) : (y >= height - radius ? (y + 0.5f) - (height - radius) : 0f);
            return Mathf.Clamp01(radius + 0.5f - Mathf.Sqrt(dx * dx + dy * dy));
        }

        internal static Sprite GetOrCreateAppRoundedMaskSprite()
        {
            if (_appRoundedMaskSprite != null)
            {
                return _appRoundedMaskSprite;
            }

            const int size = 128;
            const int radius = 26;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp
            };

            Color[] pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    pixels[y * size + x] = new Color(1f, 1f, 1f, ComputeRoundedCornerAlpha(x, y, size, size, radius));
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            float border = radius + 2f;
            _appRoundedMaskSprite = Sprite.Create(
                tex,
                new Rect(0, 0, size, size),
                new Vector2(0.5f, 0.5f),
                100f,
                0,
                SpriteMeshType.FullRect,
                new Vector4(border, border, border, border));
            return _appRoundedMaskSprite;
        }

        internal static Sprite GetOrCreatePillSprite()
        {
            if (_pillWhiteSprite == null)
            {
                _pillWhiteSprite = PictureInPictureAdClient.GetOrCreateRoundedMaskSprite(PillCornerRadius);
            }
            return _pillWhiteSprite;
        }

        internal static Sprite GetOrCreateAdBadgeSprite()
        {
            if (_adBadgeSprite != null)
            {
                return _adBadgeSprite;
            }

            // 92x40 rectangular badge sprite (2x resolution of 46x20) with a small 7px corner radius (~3.5px on screen).
            const int width = 92;
            const int height = 40;
            const int radius = 7;
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            Color leftBg = new Color(240f / 255f, 240f / 255f, 240f / 255f, 1f);
            Color rightBg = new Color(235f / 255f, 235f / 255f, 235f / 255f, 1f);
            Color seamColor = new Color(226f / 255f, 226f / 255f, 226f / 255f, 1f);

            Color[] pixels = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float alpha = ComputeRoundedCornerAlpha(x, y, width, height, radius);
                    Color baseCol = x < 52 ? leftBg : (x == 52 ? seamColor : rightBg);
                    pixels[y * width + x] = new Color(baseCol.r, baseCol.g, baseCol.b, alpha);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            _adBadgeSprite = Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
            return _adBadgeSprite;
        }

        private static void ApplySeparableGaussianBlur(Color[] buffer, int width, int height, int radius, int passes)
        {
            Color[] temp = new Color[width * height];
            for (int p = 0; p < passes; p++)
            {
                // Horizontal pass
                for (int y = 0; y < height; y++)
                {
                    int rowOffset = y * width;
                    for (int x = 0; x < width; x++)
                    {
                        float r = 0f, g = 0f, b = 0f;
                        int minX = Mathf.Max(0, x - radius);
                        int maxX = Mathf.Min(width - 1, x + radius);
                        for (int kx = minX; kx <= maxX; kx++)
                        {
                            Color c = buffer[rowOffset + kx];
                            r += c.r;
                            g += c.g;
                            b += c.b;
                        }
                        float inv = 1f / (maxX - minX + 1);
                        temp[rowOffset + x] = new Color(r * inv, g * inv, b * inv, 1f);
                    }
                }

                // Vertical pass
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        float r = 0f, g = 0f, b = 0f;
                        int minY = Mathf.Max(0, y - radius);
                        int maxY = Mathf.Min(height - 1, y + radius);
                        for (int ky = minY; ky <= maxY; ky++)
                        {
                            Color c = temp[ky * width + x];
                            r += c.r;
                            g += c.g;
                            b += c.b;
                        }
                        float inv = 1f / (maxY - minY + 1);
                        buffer[y * width + x] = new Color(r * inv, g * inv, b * inv, 1f);
                    }
                }
            }
        }

        private static float SmoothStep(float edge0, float edge1, float x)
        {
            float t = Mathf.Clamp01((x - edge0) / (edge1 - edge0));
            return t * t * (3f - 2f * t);
        }

        private static Color GetFloodItPaletteColor(int code)
        {
            switch (code)
            {
                case 1: return new Color(67f / 255f, 173f / 255f, 226f / 255f, 1f);  // Sky Blue (#43ADE2)
                case 2: return new Color(120f / 255f, 152f / 255f, 30f / 255f, 1f);  // Olive Green (#78981E)
                case 3: return new Color(237f / 255f, 238f / 255f, 25f / 255f, 1f);  // Bright Yellow (#EDEE19)
                case 4: return new Color(220f / 255f, 75f / 255f, 26f / 255f, 1f);   // Orange-Red (#DC4B1A)
                case 5: return new Color(234f / 255f, 106f / 255f, 158f / 255f, 1f); // Pink (#EA6A9E)
                case 6: return new Color(96f / 255f, 92f / 255f, 168f / 255f, 1f);   // Slate Purple (#605CA8)
                default: return new Color(30f / 255f, 31f / 255f, 34f / 255f, 1f);  // Dark Charcoal (#1E1F22)
            }
        }

        internal static Sprite GetOrCreateFloodItIconSprite()
        {
            if (_floodItIconSprite != null)
            {
                return _floodItIconSprite;
            }

            const int size = 64;
            const int radius = 14;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            int[,] grid = new int[4, 4]
            {
                { 4, 5, 1, 2 }, // bottom row
                { 6, 3, 4, 5 },
                { 2, 6, 3, 1 },
                { 3, 5, 1, 4 }  // top row
            };

            Color[] pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                int row = Mathf.Clamp((y * 4) / size, 0, 3);
                for (int x = 0; x < size; x++)
                {
                    int col = Mathf.Clamp((x * 4) / size, 0, 3);
                    float alpha = ComputeRoundedCornerAlpha(x, y, size, size, radius);
                    Color tileColor = GetFloodItPaletteColor(grid[row, col]);
                    pixels[y * size + x] = new Color(tileColor.r, tileColor.g, tileColor.b, alpha);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            _floodItIconSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
            return _floodItIconSprite;
        }

        private static Color ComputeDominantNonDarkColor(Color[] pixels, int count)
        {
            float sumR = 0f, sumG = 0f, sumB = 0f, totalWeight = 0f;

            for (int i = 0; i < count; i++)
            {
                Color c = pixels[i];
                if (Mathf.Max(c.r, Mathf.Max(c.g, c.b)) <= 0.25f)
                {
                    continue;
                }

                float luminance = 0.299f * c.r + 0.587f * c.g + 0.114f * c.b;
                float yellowOliveAffinity = Mathf.Clamp01((c.r + c.g) * 0.65f - c.b * 0.85f);
                float weight = luminance * (0.35f + 2.4f * yellowOliveAffinity * yellowOliveAffinity);

                sumR += c.r * weight;
                sumG += c.g * weight;
                sumB += c.b * weight;
                totalWeight += weight;
            }

            if (totalWeight <= 0.0001f)
            {
                return new Color(0.66f, 0.69f, 0.11f, 1f);
            }

            Color avg = new Color(sumR / totalWeight, sumG / totalWeight, sumB / totalWeight, 1f);
            return Color.Lerp(avg, new Color(0.67f, 0.70f, 0.11f, 1f), 0.55f);
        }

        internal static Sprite GetOrCreateBackdropSprite()
        {
            if (_gradientBackdropSprite != null)
            {
                return _gradientBackdropSprite;
            }

            // 220 x 476 matches a 9:19.5 portrait screen aspect ratio so 1x1 pixel tiles are unstretched squares.
            const int width = 220;
            const int height = 476;
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            // Flood-It! dark splash mosaic pattern (6 rows x 9 cols of 21x21 square tiles).
            int[,] mosaic = new int[6, 9]
            {
                { 0, 2, 1, 1, 2, 3, 3, 6, 5 }, // row 0 (bottom)
                { 0, 6, 5, 2, 3, 4, 5, 5, 1 }, // row 1
                { 0, 0, 4, 2, 2, 6, 2, 5, 0 }, // row 2
                { 0, 0, 3, 0, 0, 0, 4, 0, 0 }, // row 3
                { 0, 0, 0, 6, 0, 5, 0, 0, 6 }, // row 4
                { 0, 1, 0, 0, 2, 0, 0, 3, 0 }  // row 5 (near bottom edge of squeezed app)
            };

            const int cols = 9;
            const int rows = 6;
            const int tileSize = 21;
            int gridOffsetX = (width - cols * tileSize) / 2;
            const int gridOffsetY = 4;
            int bottomCreativeHeight = Mathf.RoundToInt(height * 0.275f);
            Color deepBlack = new Color(0.03f, 0.03f, 0.035f, 1f);

            Color[] rawCreativePixels = new Color[width * bottomCreativeHeight];
            Color[] sharpBottomBuffer = new Color[width * height];

            for (int y = 0; y < bottomCreativeHeight; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color rawTile = deepBlack;
                    int gx = x - gridOffsetX;
                    if (y >= gridOffsetY && gx >= 0 && gx < cols * tileSize)
                    {
                        int col = gx / tileSize;
                        int row = (y - gridOffsetY) / tileSize;
                        if (row >= 0 && row < rows && mosaic[row, col] != 0)
                        {
                            rawTile = GetFloodItPaletteColor(mosaic[row, col]);
                        }
                    }

                    rawCreativePixels[y * width + x] = rawTile;
                    sharpBottomBuffer[y * width + x] = Color.Lerp(rawTile, deepBlack, 0.44f);
                }
            }

            Color dominantColor = ComputeDominantNonDarkColor(rawCreativePixels, rawCreativePixels.Length);
            _dominantCreativeColor = dominantColor;

            Color[] upperBlurBuffer = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                float ny = y / (float)(height - 1);
                float riseFromSeam = SmoothStep(0.26f, 0.36f, ny);
                float topVignette = 1f - SmoothStep(0.68f, 1.0f, ny) * 0.44f;
                float verticalGlow = riseFromSeam * topVignette;

                for (int x = 0; x < width; x++)
                {
                    float nx = x / (float)(width - 1);
                    float edgeVignette = 1f - SmoothStep(0.32f, 0.50f, Mathf.Abs(nx - 0.5f)) * 0.18f;
                    upperBlurBuffer[y * width + x] = Color.Lerp(deepBlack, dominantColor, verticalGlow * edgeVignette);

                    if (y >= bottomCreativeHeight)
                    {
                        sharpBottomBuffer[y * width + x] = deepBlack;
                    }
                }
            }

            ApplySeparableGaussianBlur(upperBlurBuffer, width, height, radius: 14, passes: 3);

            Color[] softBottomBuffer = (Color[])sharpBottomBuffer.Clone();
            ApplySeparableGaussianBlur(softBottomBuffer, width, height, radius: 4, passes: 2);

            Color[] finalPixels = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                float ny = y / (float)(height - 1);
                float toSoftBottom = SmoothStep(0.20f, 0.27f, ny);
                float toUpperDominantBlur = SmoothStep(0.24f, 0.31f, ny);

                for (int x = 0; x < width; x++)
                {
                    int idx = y * width + x;
                    Color bottomCol = Color.Lerp(sharpBottomBuffer[idx], softBottomBuffer[idx], toSoftBottom);
                    Color c = Color.Lerp(bottomCol, upperBlurBuffer[idx], toUpperDominantBlur);
                    c.a = 1f;
                    finalPixels[idx] = c;
                }
            }

            tex.SetPixels(finalPixels);
            tex.Apply();
            _gradientBackdropSprite = Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
            return _gradientBackdropSprite;
        }

        internal static Sprite GetOrCreateGooglePlayIconSprite()
        {
            if (_googlePlayIconSprite != null)
            {
                return _googlePlayIconSprite;
            }

            const int size = 32;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp
            };

            Color blue = new Color(66f / 255f, 133f / 255f, 244f / 255f, 1f);
            Color green = new Color(52f / 255f, 168f / 255f, 83f / 255f, 1f);
            Color red = new Color(234f / 255f, 67f / 255f, 53f / 255f, 1f);
            Color yellow = new Color(251f / 255f, 188f / 255f, 5f / 255f, 1f);

            Color[] pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                float ny = (y + 0.5f) / size;
                float dy = Mathf.Abs(ny - 0.5f);
                for (int x = 0; x < size; x++)
                {
                    float nx = (x + 0.5f) / size;
                    float maxHalfHeight = (0.90f - nx) * 0.56f;
                    if (nx >= 0.12f && nx <= 0.90f && dy <= maxHalfHeight)
                    {
                        float diag1 = ny - nx * 0.85f;
                        float diag2 = (1f - ny) - nx * 0.85f;
                        pixels[y * size + x] = nx > 0.62f
                            ? yellow
                            : (diag1 > 0.04f && diag2 > 0.04f ? blue : (ny >= 0.5f ? green : red));
                    }
                    else
                    {
                        pixels[y * size + x] = Color.clear;
                    }
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            _googlePlayIconSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
            return _googlePlayIconSprite;
        }

        private static float DistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
        {
            Vector2 ab = b - a;
            float lenSq = ab.sqrMagnitude;
            if (lenSq < 0.000001f)
            {
                return Vector2.Distance(p, a);
            }
            float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / lenSq);
            return Vector2.Distance(p, a + t * ab);
        }

        internal static Sprite GetOrCreateAdChoicesIconSprite()
        {
            if (_adChoicesIconSprite != null)
            {
                return _adChoicesIconSprite;
            }

            const int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            Color cyan = new Color(0f / 255f, 174f / 255f, 205f / 255f, 1f);

            Vector2 topLeft = new Vector2(0.18f, 0.82f);
            Vector2 bottomLeft = new Vector2(0.18f, 0.18f);
            Vector2 bottomHook = new Vector2(0.27f, 0.22f);
            Vector2 rightApex = new Vector2(0.84f, 0.50f);
            Vector2 stemBottom = new Vector2(0.44f, 0.30f);
            Vector2 stemTop = new Vector2(0.44f, 0.48f);
            Vector2 dotCenter = new Vector2(0.44f, 0.62f);

            const float strokeRadius = 0.052f;
            const float dotRadius = 0.058f;
            const float aaWidth = 1.25f / size;

            Color[] pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                float ny = (y + 0.5f) / size;
                for (int x = 0; x < size; x++)
                {
                    Vector2 p = new Vector2((x + 0.5f) / size, ny);
                    float minStrokeDist = Mathf.Min(
                        DistanceToSegment(p, topLeft, bottomLeft),
                        Mathf.Min(
                            DistanceToSegment(p, bottomLeft, bottomHook),
                            Mathf.Min(
                                DistanceToSegment(p, topLeft, rightApex),
                                Mathf.Min(
                                    DistanceToSegment(p, rightApex, stemBottom),
                                    DistanceToSegment(p, stemBottom, stemTop))))) - strokeRadius;

                    float sdf = Mathf.Min(minStrokeDist, Vector2.Distance(p, dotCenter) - dotRadius);
                    pixels[y * size + x] = new Color(cyan.r, cyan.g, cyan.b, Mathf.Clamp01(0.5f - sdf / aaWidth));
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            _adChoicesIconSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
            return _adChoicesIconSprite;
        }

        internal static Sprite GetOrCreateSqueezebackBadgeSprite(PictureInPictureAdClient.BadgeType type)
        {
            if (_squeezebackBadgeSprites.TryGetValue(type, out Sprite cachedSprite) && cachedSprite != null)
            {
                return cachedSprite;
            }

            const int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            Color darkCircle = new Color(54f / 255f, 55f / 255f, 58f / 255f, 0.94f);
            Color whiteInk = new Color(0.97f, 0.97f, 0.97f, 1f);

            const float center = (size - 1) / 2f;
            const float radius = 27.5f;
            const float aaWidth = 1.25f / size;

            Vector2 crossA1 = new Vector2(0.32f, 0.32f);
            Vector2 crossB1 = new Vector2(0.68f, 0.68f);
            Vector2 crossA2 = new Vector2(0.32f, 0.68f);
            Vector2 crossB2 = new Vector2(0.68f, 0.32f);
            const float crossStrokeRadius = 0.036f;

            Color[] pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                float ny = (y + 0.5f) / size;
                float dy = y - center;
                for (int x = 0; x < size; x++)
                {
                    float nx = (x + 0.5f) / size;
                    float dx = x - center;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    if (dist > radius + 3.0f)
                    {
                        pixels[y * size + x] = Color.clear;
                        continue;
                    }

                    float circleAlpha = Mathf.Clamp01(radius + 0.6f - dist);
                    float shadowAlpha = Mathf.Clamp01((radius + 2.8f - dist) / 2.8f) * 0.32f;

                    Color baseCol;
                    if (circleAlpha > 0f)
                    {
                        float outA = circleAlpha * darkCircle.a + (1f - circleAlpha) * shadowAlpha;
                        float factor = outA > 0.0001f ? (circleAlpha * darkCircle.a) / outA : 0f;
                        baseCol = new Color(darkCircle.r * factor, darkCircle.g * factor, darkCircle.b * factor, outA);
                    }
                    else
                    {
                        baseCol = new Color(0f, 0f, 0f, shadowAlpha);
                    }

                    float inkAlpha = 0f;
                    if (type == PictureInPictureAdClient.BadgeType.CloseCross)
                    {
                        Vector2 p = new Vector2(nx, ny);
                        float d1 = DistanceToSegment(p, crossA1, crossB1) - crossStrokeRadius;
                        float d2 = DistanceToSegment(p, crossA2, crossB2) - crossStrokeRadius;
                        inkAlpha = Mathf.Clamp01(0.5f - Mathf.Min(d1, d2) / aaWidth);
                    }
                    else if (type == PictureInPictureAdClient.BadgeType.Countdown3)
                    {
                        if ((dy >= 11 && dy <= 16 && dx >= -7 && dx <= 7) ||
                            (dy >= 0 && dy <= 15 && dx >= 3 && dx <= 7) ||
                            (dy >= -2 && dy <= 2 && dx >= -2 && dx <= 7) ||
                            (dy >= -15 && dy <= 0 && dx >= 3 && dx <= 7) ||
                            (dy >= -16 && dy <= -11 && dx >= -7 && dx <= 7))
                        {
                            inkAlpha = 1f;
                        }
                    }
                    else if (type == PictureInPictureAdClient.BadgeType.Countdown2)
                    {
                        if ((dy >= 11 && dy <= 16 && dx >= -7 && dx <= 7) ||
                            (dy >= 0 && dy <= 15 && dx >= 3 && dx <= 7) ||
                            (dy >= -2 && dy <= 2 && dx >= -7 && dx <= 7) ||
                            (dy >= -15 && dy <= 0 && dx >= -7 && dx <= -3) ||
                            (dy >= -16 && dy <= -11 && dx >= -7 && dx <= 7))
                        {
                            inkAlpha = 1f;
                        }
                    }
                    else if (type == PictureInPictureAdClient.BadgeType.Countdown1)
                    {
                        if ((dy >= -15 && dy <= 16 && dx >= -2 && dx <= 3) ||
                            (dy >= 8 && dy <= 15 && dx >= -7 && dx <= 0) ||
                            (dy >= -16 && dy <= -11 && dx >= -7 && dx <= 8))
                        {
                            inkAlpha = 1f;
                        }
                    }

                    if (inkAlpha > 0f && circleAlpha > 0f)
                    {
                        Color blendedRgb = Color.Lerp(baseCol, whiteInk, inkAlpha);
                        baseCol = new Color(blendedRgb.r, blendedRgb.g, blendedRgb.b, baseCol.a);
                    }

                    pixels[y * size + x] = baseCol;
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
            _squeezebackBadgeSprites[type] = sprite;
            return sprite;
        }

        internal static Color GetInitialCtaColor() => InitialCtaColor;
        internal static Color GetActiveCtaColor() => _dominantCreativeColor;

        #endregion
    }

    internal class SqueezebackSimulatorComponent : MonoBehaviour
    {
        private class ModifiedImageState
        {
            public Image Image;
            public Sprite OriginalSprite;
            public Image.Type OriginalType;
        }

        private class WrappedCanvasState
        {
            public Canvas Canvas;
            public RectTransform WrapperRect;
            public List<Transform> WrappedChildren = new List<Transform>();
            public List<ModifiedImageState> ModifiedBackgroundImages = new List<ModifiedImageState>();
        }

        private class WrappedCameraState
        {
            public Camera Camera;
            public Rect OriginalRect;
        }

        private SqueezebackAdClient _client;
        private Canvas _backdropCanvas;
        private Canvas _controlsCanvas;
        private RectTransform _appShadowFrameRect;
        private Image _badgeImage;
        private Button _closeButton;
        private Image _ctaBackground;
        private Text _ctaText;
        private Button _cardButton;
        private Button _ctaButton;

        private readonly List<WrappedCanvasState> _wrappedCanvases = new List<WrappedCanvasState>();
        private readonly List<WrappedCameraState> _wrappedCameras = new List<WrappedCameraState>();

        private Coroutine _squeezeCoroutine;
        private Coroutine _countdownCoroutine;
        private float _currentScale = 1.0f;
        private float _currentTopNorm = 0f;
        private bool _isSceneWrapped = false;
        private Scene _shownScene;

        private void OnEnable()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            RestoreSceneImmediately();
        }

        private void OnSceneUnloaded(Scene scene)
        {
            if (_isSceneWrapped && scene == _shownScene)
            {
                RestoreSceneImmediately();
                _client?.Destroy();
            }
        }

        private static object GetDefaultFontObject()
        {
            try
            {
                GameObject bannerPrefab = Resources.Load("PlaceholderAds/Banners/BANNER") as GameObject;
                if (bannerPrefab != null)
                {
                    Text prefabText = bannerPrefab.GetComponentInChildren<Text>(true);
                    object fontVal = prefabText != null ? typeof(Text).GetProperty("font")?.GetValue(prefabText, null) : null;
                    if (fontVal != null)
                    {
                        return fontVal;
                    }
                }

                Type fontType = Type.GetType("UnityEngine.Font, UnityEngine.TextRenderingModule")
                    ?? Type.GetType("UnityEngine.Font, UnityEngine");
                var getBuiltin = fontType != null
                    ? typeof(Resources).GetMethod("GetBuiltinResource", new Type[] { typeof(Type), typeof(string) })
                    : null;
                if (getBuiltin != null)
                {
                    return getBuiltin.Invoke(null, new object[] { fontType, "LegacyRuntime.ttf" })
                        ?? getBuiltin.Invoke(null, new object[] { fontType, "Arial.ttf" });
                }
            }
            catch (Exception)
            {
                // Ignore reflection fallback errors in batch/headless environments.
            }
            return null;
        }

        private static void ConfigureText(
            Text label,
            object defaultFont,
            string content,
            int size,
            string fontStyleName,
            string alignmentName,
            Color textColor,
            bool addDropShadow = false)
        {
            label.text = content;
            label.fontSize = size;
            label.color = textColor;
            label.raycastTarget = false;

            if (addDropShadow)
            {
                Shadow shadow = label.gameObject.GetComponent<Shadow>() ?? label.gameObject.AddComponent<Shadow>();
                shadow.effectColor = new Color(0f, 0f, 0f, 0.78f);
                shadow.effectDistance = new Vector2(0f, -1.2f);
            }

            try
            {
                if (defaultFont != null)
                {
                    typeof(Text).GetProperty("font")?.SetValue(label, defaultFont, null);
                }

                var styleProp = typeof(Text).GetProperty("fontStyle");
                if (styleProp != null && !string.IsNullOrEmpty(fontStyleName))
                {
                    styleProp.SetValue(label, Enum.Parse(styleProp.PropertyType, fontStyleName), null);
                }

                var alignProp = typeof(Text).GetProperty("alignment");
                if (alignProp != null && !string.IsNullOrEmpty(alignmentName))
                {
                    alignProp.SetValue(label, Enum.Parse(alignProp.PropertyType, alignmentName), null);
                }
            }
            catch (Exception)
            {
                // Ignore reflection errors if properties are unavailable.
            }
        }

        private Canvas CreateOverlayCanvas(string name, int sortingOrder, bool addRaycaster)
        {
            GameObject canvasObj = new GameObject(name);
            canvasObj.transform.SetParent(transform, false);

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(390f, 844f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            if (addRaycaster)
            {
                canvasObj.AddComponent<GraphicRaycaster>();
            }
            return canvas;
        }

        private static RectTransform CreateRect(
            string name,
            Transform parent,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 anchoredPosition,
            Vector2 sizeDelta)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
            return rect;
        }

        private static RectTransform CreateOffsetRect(
            string name,
            Transform parent,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 offsetMin,
            Vector2 offsetMax)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            return rect;
        }

        public void BuildUi(SqueezebackAdClient client)
        {
            _client = client;
            object defaultFont = GetDefaultFontObject();

            // 1. Backdrop Canvas (renders behind the squeezed scene UI).
            _backdropCanvas = CreateOverlayCanvas(
                "SqueezebackBackdropCanvas",
                SqueezebackAdClient.BackdropSortingOrder,
                addRaycaster: false);

            RectTransform bgRect = CreateRect(
                "GradientBackdrop",
                _backdropCanvas.transform,
                Vector2.zero,
                Vector2.one,
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                Vector2.zero);
            Image bgImage = bgRect.gameObject.AddComponent<Image>();
            bgImage.sprite = SqueezebackAdClient.GetOrCreateBackdropSprite();
            bgImage.raycastTarget = false;

            _appShadowFrameRect = CreateOffsetRect(
                "SqueezedAppShadowFrame",
                _backdropCanvas.transform,
                Vector2.zero,
                Vector2.one,
                new Vector2(0.5f, 1.0f),
                new Vector2(-2f, -2f),
                new Vector2(2f, 2f));
            Image shadowFrameImage = _appShadowFrameRect.gameObject.AddComponent<Image>();
            shadowFrameImage.sprite = SqueezebackAdClient.GetOrCreateAppRoundedMaskSprite();
            shadowFrameImage.type = Image.Type.Sliced;
            shadowFrameImage.color = new Color(0f, 0f, 0f, 0.40f);
            shadowFrameImage.raycastTarget = false;

            // 2. Controls Canvas.
            _controlsCanvas = CreateOverlayCanvas(
                "SqueezebackControlsCanvas",
                SqueezebackAdClient.ActiveControlsSortingOrder,
                addRaycaster: true);

            // Top-left countdown & close button positioned in the left margin next to the squeezed app.
            RectTransform badgeRect = CreateRect(
                "SqueezebackCloseBadge",
                _controlsCanvas.transform,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(10f, -38f),
                new Vector2(28f, 28f));

            _badgeImage = badgeRect.gameObject.AddComponent<Image>();
            _badgeImage.sprite = SqueezebackAdClient.GetOrCreateSqueezebackBadgeSprite(PictureInPictureAdClient.BadgeType.Countdown3);
            _badgeImage.color = Color.white;

            _closeButton = badgeRect.gameObject.AddComponent<Button>();
            _closeButton.transition = Selectable.Transition.None;
            _closeButton.interactable = false;
            _closeButton.onClick.AddListener(() => _client?.HideFromUserClose());

            // Full-width transparent bottom ad overlay container.
            RectTransform bottomOverlayRect = CreateRect(
                "SqueezebackBottomOverlay",
                _controlsCanvas.transform,
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0.5f, 0f),
                Vector2.zero,
                new Vector2(0f, 170f));

            Image overlayHitTarget = bottomOverlayRect.gameObject.AddComponent<Image>();
            overlayHitTarget.color = Color.clear;

            _cardButton = bottomOverlayRect.gameObject.AddComponent<Button>();
            _cardButton.targetGraphic = overlayHitTarget;
            _cardButton.onClick.AddListener(() => _client?.RaiseAdClicked());

            // Row 1 (Top): Rounded Flood-It! app icon + Headline + Rating & Google Play badge.
            RectTransform iconRect = CreateRect(
                "AdIcon",
                bottomOverlayRect,
                Vector2.zero,
                Vector2.zero,
                Vector2.zero,
                new Vector2(22f, 104f),
                new Vector2(44f, 44f));
            Image iconBg = iconRect.gameObject.AddComponent<Image>();
            iconBg.sprite = SqueezebackAdClient.GetOrCreateFloodItIconSprite();
            iconBg.type = Image.Type.Simple;
            iconBg.color = Color.white;
            iconBg.raycastTarget = false;

            RectTransform headlineRect = CreateOffsetRect(
                "HeadlineLabel",
                bottomOverlayRect,
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(76f, 126f),
                new Vector2(-22f, 150f));
            ConfigureText(
                headlineRect.gameObject.AddComponent<Text>(),
                defaultFont,
                "Flood-It!",
                16,
                "Bold",
                "MiddleLeft",
                Color.white,
                addDropShadow: true);

            RectTransform ratingRect = CreateRect(
                "RatingLabel",
                bottomOverlayRect,
                Vector2.zero,
                Vector2.zero,
                Vector2.zero,
                new Vector2(76f, 105f),
                new Vector2(42f, 20f));
            ConfigureText(
                ratingRect.gameObject.AddComponent<Text>(),
                defaultFont,
                "4.9 ★",
                12,
                "Bold",
                "MiddleLeft",
                Color.white,
                addDropShadow: true);

            RectTransform playIconRect = CreateRect(
                "GooglePlayIcon",
                bottomOverlayRect,
                Vector2.zero,
                Vector2.zero,
                Vector2.zero,
                new Vector2(118f, 108f),
                new Vector2(14f, 14f));
            Image playIconImage = playIconRect.gameObject.AddComponent<Image>();
            playIconImage.sprite = SqueezebackAdClient.GetOrCreateGooglePlayIconSprite();
            playIconImage.raycastTarget = false;

            RectTransform storeLabelRect = CreateOffsetRect(
                "GooglePlayLabel",
                bottomOverlayRect,
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(136f, 105f),
                new Vector2(-22f, 125f));
            ConfigureText(
                storeLabelRect.gameObject.AddComponent<Text>(),
                defaultFont,
                "Google Play",
                12,
                "Normal",
                "MiddleLeft",
                Color.white,
                addDropShadow: true);

            // Row 2 (Middle): Full-width white body copy below the icon row.
            RectTransform descRect = CreateOffsetRect(
                "DescriptionLabel",
                bottomOverlayRect,
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(22f, 72f),
                new Vector2(-22f, 96f));
            ConfigureText(
                descRect.gameObject.AddComponent<Text>(),
                defaultFont,
                "Fill the board with a single color! Fun, addictive puzzle game.",
                13,
                "Normal",
                "MiddleLeft",
                Color.white,
                addDropShadow: true);

            // Row 3 (Bottom): Pill-shaped "Install" CTA button (~75% of screen width, centered).
            RectTransform ctaRect = CreateRect(
                "CtaButton",
                bottomOverlayRect,
                new Vector2(0.125f, 0f),
                new Vector2(0.875f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 22f),
                new Vector2(0f, 40f));

            _ctaBackground = ctaRect.gameObject.AddComponent<Image>();
            _ctaBackground.sprite = SqueezebackAdClient.GetOrCreatePillSprite();
            _ctaBackground.type = Image.Type.Sliced;
            _ctaBackground.color = SqueezebackAdClient.GetInitialCtaColor();

            _ctaButton = ctaRect.gameObject.AddComponent<Button>();
            _ctaButton.transition = Selectable.Transition.None;
            _ctaButton.targetGraphic = _ctaBackground;
            _ctaButton.onClick.AddListener(() => _client?.RaiseAdClicked());

            RectTransform ctaTextRect = CreateRect(
                "CtaText",
                ctaRect,
                Vector2.zero,
                Vector2.one,
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                Vector2.zero);
            _ctaText = ctaTextRect.gameObject.AddComponent<Text>();
            ConfigureText(_ctaText, defaultFont, "Install", 15, "Bold", "MiddleCenter", Color.white);

            // Bottom-Right "[Ad ▷]" Attribution + AdChoices Badge flush at bottom-right corner.
            RectTransform attrRect = CreateRect(
                "AdAttributionBadge",
                _controlsCanvas.transform,
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(-2f, 2f),
                new Vector2(46f, 20f));

            Image attrBg = attrRect.gameObject.AddComponent<Image>();
            attrBg.sprite = SqueezebackAdClient.GetOrCreateAdBadgeSprite();
            attrBg.type = Image.Type.Simple;
            attrBg.color = Color.white;
            attrBg.raycastTarget = false;

            RectTransform attrTextRect = CreateRect(
                "AdText",
                attrRect,
                new Vector2(0f, 0f),
                new Vector2(0f, 1f),
                new Vector2(0f, 0.5f),
                new Vector2(3f, 0f),
                new Vector2(23f, 0f));
            ConfigureText(
                attrTextRect.gameObject.AddComponent<Text>(),
                defaultFont,
                "Ad",
                12,
                "Normal",
                "MiddleCenter",
                new Color(0.10f, 0.10f, 0.10f, 1f));

            RectTransform adChoicesIconRect = CreateRect(
                "AdChoicesIcon",
                attrRect,
                new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f),
                new Vector2(-3.5f, 0f),
                new Vector2(14f, 14f));
            Image adChoicesIconImg = adChoicesIconRect.gameObject.AddComponent<Image>();
            adChoicesIconImg.sprite = SqueezebackAdClient.GetOrCreateAdChoicesIconSprite();
            adChoicesIconImg.raycastTarget = false;
        }

        public void PlayShowAnimation(Action onComplete)
        {
            StopActiveCoroutines();
            WrapSceneContentIfNeeded();

            // Place controls behind the app UI canvas (-99, just above backdrop at -100) while squeezing in
            // so the app card smoothly pulls back to reveal the controls underneath.
            if (_controlsCanvas != null)
            {
                _controlsCanvas.sortingOrder = SqueezebackAdClient.AnimatingControlsSortingOrder;
            }
            if (_closeButton != null)
            {
                _closeButton.interactable = false;
            }
            if (_badgeImage != null)
            {
                _badgeImage.sprite = SqueezebackAdClient.GetOrCreateSqueezebackBadgeSprite(PictureInPictureAdClient.BadgeType.Countdown3);
            }
            if (_ctaBackground != null)
            {
                _ctaBackground.color = SqueezebackAdClient.GetInitialCtaColor();
            }
            if (_ctaText != null)
            {
                _ctaText.color = Color.white;
            }

            _squeezeCoroutine = StartCoroutine(AnimateSqueezeRoutine(
                targetScale: SqueezebackAdClient.ContentScaleFactor,
                targetTopNorm: SqueezebackAdClient.TopOffsetNormalized,
                onComplete: () =>
                {
                    if (_controlsCanvas != null)
                    {
                        _controlsCanvas.sortingOrder = SqueezebackAdClient.ActiveControlsSortingOrder;
                    }
                    onComplete?.Invoke();
                    _countdownCoroutine = StartCoroutine(CountdownAndCtaTransitionRoutine());
                }));
        }

        public void PlayHideAnimation(Action onComplete)
        {
            StopActiveCoroutines();

            // Place controls behind the app UI canvas (-99, just above backdrop at -100) before unsqueezing
            // so the expanding app card covers the Install button, Close button, and text as the squeeze is undone.
            if (_controlsCanvas != null)
            {
                _controlsCanvas.sortingOrder = SqueezebackAdClient.AnimatingControlsSortingOrder;
            }
            if (_closeButton != null)
            {
                _closeButton.interactable = false;
            }

            _squeezeCoroutine = StartCoroutine(AnimateSqueezeRoutine(
                targetScale: 1.0f,
                targetTopNorm: 0f,
                onComplete: () =>
                {
                    RestoreSceneImmediately();
                    onComplete?.Invoke();
                }));
        }

        private void StopActiveCoroutines()
        {
            if (_squeezeCoroutine != null)
            {
                StopCoroutine(_squeezeCoroutine);
                _squeezeCoroutine = null;
            }
            if (_countdownCoroutine != null)
            {
                StopCoroutine(_countdownCoroutine);
                _countdownCoroutine = null;
            }
        }

        private static void RecalculateMasking(Component root)
        {
            if (root == null)
            {
                return;
            }
            MaskableGraphic[] maskables = root.GetComponentsInChildren<MaskableGraphic>(true);
            for (int i = 0; i < maskables.Length; i++)
            {
                maskables[i]?.RecalculateMasking();
            }
        }

        /// <summary>
        /// Wraps active scene UI Canvases (the "container view") and captures active scene Cameras (the "hardware layer")
        /// so both layers scale in lockstep frame-by-frame with rounded rectangle clipping throughout the squeeze process.
        /// </summary>
        private void WrapSceneContentIfNeeded()
        {
            if (_isSceneWrapped)
            {
                return;
            }
            _isSceneWrapped = true;
            _shownScene = SceneManager.GetActiveScene();

            _wrappedCanvases.Clear();
            _wrappedCameras.Clear();

            // 1. Capture active scene Cameras (the "SurfaceView / hardware layer").
            Camera[] allCameras = Camera.allCameras;
            for (int i = 0; i < allCameras.Length; i++)
            {
                Camera cam = allCameras[i];
                if (cam != null && cam.targetTexture == null)
                {
                    _wrappedCameras.Add(new WrappedCameraState
                    {
                        Camera = cam,
                        OriginalRect = cam.rect
                    });
                }
            }

            Sprite appRoundedSprite = SqueezebackAdClient.GetOrCreateAppRoundedMaskSprite();

            // 2. Wrap active root Canvases (the "contentWrapper / container view").
            Canvas[] allCanvases = GameObject.FindObjectsOfType<Canvas>();
            for (int i = 0; i < allCanvases.Length; i++)
            {
                Canvas canvas = allCanvases[i];
                if (canvas == null || !canvas.isRootCanvas || !canvas.gameObject.activeInHierarchy ||
                    canvas == _backdropCanvas || canvas == _controlsCanvas || canvas.transform.IsChildOf(transform))
                {
                    continue;
                }

                // Collect UI RectTransform children (skipping non-UI transforms like Main Camera or EventSystem).
                List<Transform> uiChildren = new List<Transform>();
                int childCount = canvas.transform.childCount;
                for (int c = 0; c < childCount; c++)
                {
                    Transform child = canvas.transform.GetChild(c);
                    if (child is RectTransform &&
                        child.GetComponent<Camera>() == null &&
                        child.GetComponent<UnityEngine.EventSystems.EventSystem>() == null)
                    {
                        uiChildren.Add(child);
                    }
                }

                if (uiChildren.Count == 0)
                {
                    continue;
                }

                RectTransform wrapperRect = CreateOffsetRect(
                    "SqueezebackContentWrapper",
                    canvas.transform,
                    Vector2.zero,
                    Vector2.one,
                    new Vector2(0.5f, 1.0f),
                    Vector2.zero,
                    Vector2.zero);
                wrapperRect.gameObject.layer = canvas.gameObject.layer;
                wrapperRect.localScale = Vector3.one;

                // Clip the squeezed app UI to a 9-sliced rounded rectangle throughout the entire squeeze animation.
                Image wrapperMaskImage = wrapperRect.gameObject.AddComponent<Image>();
                wrapperMaskImage.sprite = appRoundedSprite;
                wrapperMaskImage.type = Image.Type.Sliced;
                wrapperMaskImage.color = Color.white;
                wrapperMaskImage.raycastTarget = false;

                Mask wrapperMask = wrapperRect.gameObject.AddComponent<Mask>();
                wrapperMask.showMaskGraphic = false;

                List<ModifiedImageState> modifiedImages = new List<ModifiedImageState>();
                for (int c = 0; c < uiChildren.Count; c++)
                {
                    Transform child = uiChildren[c];
                    Image childImg = child.GetComponent<Image>();
                    if (childImg != null && childImg.sprite == null)
                    {
                        modifiedImages.Add(new ModifiedImageState
                        {
                            Image = childImg,
                            OriginalSprite = childImg.sprite,
                            OriginalType = childImg.type
                        });
                        childImg.sprite = appRoundedSprite;
                        childImg.type = Image.Type.Sliced;
                    }
                    child.SetParent(wrapperRect, false);
                }

                RecalculateMasking(wrapperRect);

                _wrappedCanvases.Add(new WrappedCanvasState
                {
                    Canvas = canvas,
                    WrapperRect = wrapperRect,
                    WrappedChildren = uiChildren,
                    ModifiedBackgroundImages = modifiedImages
                });
            }
        }

        private static void ApplySqueezeToRect(RectTransform targetRect, Canvas parentCanvas, float curScale, float curTopNorm)
        {
            if (targetRect == null || parentCanvas == null)
            {
                return;
            }

            RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();
            float canvasHeight = (canvasRect != null && canvasRect.rect.height > 0f)
                ? canvasRect.rect.height
                : Screen.height;

            targetRect.pivot = new Vector2(0.5f, 1.0f);
            targetRect.localScale = new Vector3(curScale, curScale, 1f);
            targetRect.anchoredPosition = new Vector2(0f, -curTopNorm * canvasHeight);
        }

        /// <summary>
        /// Updates both the container view (`WrapperRect`) and the hardware layer (`Camera.rect`) on the same frame.
        /// </summary>
        private void UpdateSqueezeFrame(float curScale, float curTopNorm)
        {
            _currentScale = curScale;
            _currentTopNorm = curTopNorm;

            for (int i = 0; i < _wrappedCanvases.Count; i++)
            {
                ApplySqueezeToRect(_wrappedCanvases[i].WrapperRect, _wrappedCanvases[i].Canvas, curScale, curTopNorm);
            }

            ApplySqueezeToRect(_appShadowFrameRect, _backdropCanvas, curScale, curTopNorm);

            float viewportX = (1f - curScale) * 0.5f;
            float viewportY = Mathf.Clamp01(1f - curTopNorm - curScale);
            Rect squeezedViewport = new Rect(viewportX, viewportY, curScale, curScale);

            for (int i = 0; i < _wrappedCameras.Count; i++)
            {
                if (_wrappedCameras[i].Camera != null)
                {
                    _wrappedCameras[i].Camera.rect = squeezedViewport;
                }
            }
        }

        public void RestoreSceneImmediately()
        {
            StopActiveCoroutines();

            if (!_isSceneWrapped)
            {
                return;
            }
            _isSceneWrapped = false;
            _currentScale = 1.0f;
            _currentTopNorm = 0f;

            for (int i = 0; i < _wrappedCanvases.Count; i++)
            {
                WrappedCanvasState state = _wrappedCanvases[i];
                for (int m = 0; m < state.ModifiedBackgroundImages.Count; m++)
                {
                    ModifiedImageState imgState = state.ModifiedBackgroundImages[m];
                    if (imgState.Image != null)
                    {
                        imgState.Image.sprite = imgState.OriginalSprite;
                        imgState.Image.type = imgState.OriginalType;
                    }
                }

                if (state.Canvas != null && state.WrapperRect != null)
                {
                    state.WrapperRect.localScale = Vector3.one;
                    state.WrapperRect.anchoredPosition = Vector2.zero;
                    for (int c = 0; c < state.WrappedChildren.Count; c++)
                    {
                        Transform child = state.WrappedChildren[c];
                        if (child != null)
                        {
                            child.SetParent(state.Canvas.transform, false);
                            RecalculateMasking(child);
                        }
                    }
                }
                if (state.WrapperRect != null)
                {
                    SqueezebackAdClient.DestroyObject(state.WrapperRect.gameObject);
                }
            }
            _wrappedCanvases.Clear();

            for (int i = 0; i < _wrappedCameras.Count; i++)
            {
                if (_wrappedCameras[i].Camera != null)
                {
                    _wrappedCameras[i].Camera.rect = _wrappedCameras[i].OriginalRect;
                }
            }
            _wrappedCameras.Clear();
        }

        private IEnumerator AnimateSqueezeRoutine(float targetScale, float targetTopNorm, Action onComplete)
        {
            float startScale = _currentScale;
            float startTopNorm = _currentTopNorm;
            float elapsed = 0f;

            if (targetScale < 0.99f)
            {
                SetAdInteractionsEnabled(false);
            }

            while (elapsed < SqueezebackAdClient.AnimationDurationSeconds)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / SqueezebackAdClient.AnimationDurationSeconds);
                float inv = 1f - t;
                float eased = 1f - (inv * inv * inv);

                UpdateSqueezeFrame(
                    Mathf.Lerp(startScale, targetScale, eased),
                    Mathf.Lerp(startTopNorm, targetTopNorm, eased));
                yield return null;
            }

            UpdateSqueezeFrame(targetScale, targetTopNorm);
            _squeezeCoroutine = null;
            onComplete?.Invoke();
        }

        private void SetAdInteractionsEnabled(bool enabled)
        {
            if (_cardButton != null)
            {
                _cardButton.interactable = enabled;
            }
            if (_ctaButton != null)
            {
                _ctaButton.interactable = enabled;
            }
        }

        private static readonly PictureInPictureAdClient.BadgeType[] CountdownSteps = new[]
        {
            PictureInPictureAdClient.BadgeType.Countdown3,
            PictureInPictureAdClient.BadgeType.Countdown2,
            PictureInPictureAdClient.BadgeType.Countdown1
        };

        private IEnumerator CountdownAndCtaTransitionRoutine()
        {
            if (_closeButton != null)
            {
                _closeButton.interactable = false;
            }

            for (int i = 0; i < CountdownSteps.Length; i++)
            {
                if (_badgeImage != null)
                {
                    _badgeImage.sprite = SqueezebackAdClient.GetOrCreateSqueezebackBadgeSprite(CountdownSteps[i]);
                }

                yield return new WaitForSecondsRealtime(SqueezebackAdClient.CountdownIntervalSeconds);

                // Re-enable ad card click interactions after the first 1.0s (AD_INTERACTION_DISABLED_MS = 1000).
                if (i == 0)
                {
                    SetAdInteractionsEnabled(true);
                }
            }

            if (_badgeImage != null)
            {
                _badgeImage.sprite = SqueezebackAdClient.GetOrCreateSqueezebackBadgeSprite(PictureInPictureAdClient.BadgeType.CloseCross);
            }
            if (_closeButton != null)
            {
                _closeButton.interactable = true;
            }

            // Smoothly transition CTA button color from grey directly to the Dominant Non-Dark Color
            // and font color from white to black at the 3-second mark over 300ms.
            if (_ctaBackground != null)
            {
                Color startColor = SqueezebackAdClient.GetInitialCtaColor();
                Color endColor = SqueezebackAdClient.GetActiveCtaColor();
                Color startTextColor = _ctaText != null ? _ctaText.color : Color.white;
                Color endTextColor = Color.black;
                float elapsed = 0f;
                const float ctaAnimDuration = 0.3f;
                while (elapsed < ctaAnimDuration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    float t = Mathf.Clamp01(elapsed / ctaAnimDuration);
                    _ctaBackground.color = Color.Lerp(startColor, endColor, t);
                    if (_ctaText != null)
                    {
                        _ctaText.color = Color.Lerp(startTextColor, endTextColor, t);
                    }
                    yield return null;
                }
                _ctaBackground.color = endColor;
                if (_ctaText != null)
                {
                    _ctaText.color = endTextColor;
                }
            }

            _countdownCoroutine = null;
        }
    }
}
