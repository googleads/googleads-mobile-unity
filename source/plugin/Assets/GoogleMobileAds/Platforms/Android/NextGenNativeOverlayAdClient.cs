// Copyright (C) 2026 Google, LLC
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
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Android
{
    public class NextGenNativeOverlayAdClient : AndroidJavaProxy, INativeOverlayAdClient
    {
        private readonly IInsightsEmitter _insightsEmitter = InsightsEmitter.Instance;
        private const Insight.AdFormat NativeFormat = Insight.AdFormat.Native;

        private AndroidJavaObject nativeOverlayAd;
        private string _adUnitId;

        public NextGenNativeOverlayAdClient() : base(NextGenUtils.UnityNativeTemplateAdCallbackClassName)
        {
            AndroidJavaClass playerClass = new AndroidJavaClass(Utils.UnityActivityClassName);
            AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            this.nativeOverlayAd =
                new AndroidJavaObject(NextGenUtils.UnityNativeTemplateAdClassName, activity, this);
        }

#region INativeOverlayAdClient implementation
        // Ad event fired when the native ad has loaded.
        public event EventHandler<EventArgs> OnAdLoaded;
        // Ad event fired when the native ad has failed to load.
        public event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;
        // Ad event fired when an ad impression has been recorded.
        public event EventHandler<EventArgs> OnAdDidRecordImpression;
        // Ad event fired when the full screen content has been presented.
        public event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;
        // Ad event fired when the full screen content has been dismissed.
        public event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;
        // Ad event fired when an ad has been clicked.
        public event Action OnAdClicked;

        public event Action<AdValue> OnPaidEvent;

        // A long integer provided by the AdMob UI for the configured placement.
        public long PlacementId
        {
            get
            {
                return this.nativeOverlayAd.Call<long>("getPlacementId");
            }
            set
            {
                this.nativeOverlayAd.Call("setPlacementId", value);
            }
        }

        // Loads a native ad
        public void Load(string adUnitId, AdRequest request, NativeAdOptions options)
        {
            _adUnitId = adUnitId;
            AndroidJavaObject nativeAdOptionsJava = new AndroidJavaObject(
                NextGenUtils.NativeAdOptionsClassName,
                (int)options.MediaAspectRatio,
                (int)options.AdChoicesPlacement);

            if (options.VideoOptions != null)
            {
                AndroidJavaObject videoOptionsBuilder = new AndroidJavaObject("com.google.android.libraries.ads.mobile.sdk.common.VideoOptions$Builder");
                videoOptionsBuilder.Call<AndroidJavaObject>("setStartMuted", (bool)options.VideoOptions.StartMuted);
                videoOptionsBuilder.Call<AndroidJavaObject>("setCustomControlsRequested", (bool)options.VideoOptions.CustomControlsRequested);
                videoOptionsBuilder.Call<AndroidJavaObject>("setClickToExpandRequested", (bool)options.VideoOptions.ClickToExpandRequested);
                AndroidJavaObject videoOptionsJava = videoOptionsBuilder.Call<AndroidJavaObject>("build");
                nativeAdOptionsJava.Call("setVideoOptions", videoOptionsJava);
            }

            this.nativeOverlayAd.Call("loadAd", adUnitId, nativeAdOptionsJava,
                                      NextGenUtils.GetAdRequestJavaObject(request, adUnitId));
        }

        // Hides the native overlay from the screen.
        public void Hide()
        {
            this.nativeOverlayAd.Call("hide");
        }

        // Shows the Native overlay on the screen.
        public void Show()
        {
            this.nativeOverlayAd.Call("show");
        }

        // Set the position of the Native overlay using standard position.
        public void SetPosition(AdPosition position)
        {
            this.nativeOverlayAd.Call("setPositionCode", (int)position);
        }

        // Set the position of the Native overlay using custom position.
        public void SetPosition(int x, int y)
        {
            this.nativeOverlayAd.Call("setPosition", x, y);
        }

        // Renders the Native overlay ad on the screen using the provided style, size and
        // AdPosition.
        public void Render(NativeTemplateStyle templateViewStyle, AdSize adSize,
                         AdPosition adPosition)
        {
            this.nativeOverlayAd.Call("renderCustomSizeAtPositionCode",
                                      GetNativeTemplateStyleJavaObject(templateViewStyle),
                                      NextGenUtils.GetAdSizeJavaObject(adSize), (int)adPosition);
        }

        // Renders the Native overlay ad on the screen using the provided style, size and
        // coordinates.
        public void Render(NativeTemplateStyle templateViewStyle, AdSize adSize, int x, int y)
        {
            this.nativeOverlayAd.Call("renderCustomSizeAtPosition",
                                      GetNativeTemplateStyleJavaObject(templateViewStyle),
                                      NextGenUtils.GetAdSizeJavaObject(adSize), x, y);
        }

        // Renders the Native overlay ad on the screen using default size at preset position.
        public void Render(NativeTemplateStyle templateViewStyle, AdPosition adPosition)
        {
            this.nativeOverlayAd.Call("renderDefaultSizeAtPositionCode",
                                      GetNativeTemplateStyleJavaObject(templateViewStyle),
                                      (int)adPosition);
        }

        // Renders the Native overlay ad on the screen using default size at (x,y) coordinates.
        public void Render(NativeTemplateStyle templateViewStyle, int x, int y)
        {
            this.nativeOverlayAd.Call("renderDefaultSizeAtPosition",
                                      GetNativeTemplateStyleJavaObject(templateViewStyle), x,
                                      y);
        }

        // Destroys the native ad.
        public void DestroyAd()
        {
            this.nativeOverlayAd.Call("destroy");
        }

        // Returns ad request Response info client.
        public IResponseInfoClient GetResponseInfoClient()
        {
            var responseInfoJavaObject = nativeOverlayAd.Call<AndroidJavaObject>("getResponseInfo");
            return new NextGenResponseInfoClient(responseInfoJavaObject);
        }

        // Returns the height of the NativeTemplateView in pixels.
        public float GetHeightInPixels()
        {
            return this.nativeOverlayAd.Call<float>("getHeightInPixels");
        }

        // Returns the width of the NativeTemplateView in pixels.
        public float GetWidthInPixels()
        {
            return this.nativeOverlayAd.Call<float>("getWidthInPixels");
        }

#endregion

#region Callbacks from UnityNativeTemplateAdCallback.
        void onNativeAdLoaded()
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdLoaded,
                Format = NativeFormat,
                AdUnitId = _adUnitId,
            });

            if (this.OnAdLoaded != null)
            {
                this.OnAdLoaded(this, EventArgs.Empty);
            }
        }

        void onNativeAdFailedToLoad(AndroidJavaObject error)
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdLoaded,
                Format = NativeFormat,
                AdUnitId = _adUnitId,
                Success = false,
            });

            if (this.OnAdFailedToLoad != null)
            {
                LoadAdErrorClientEventArgs args = new LoadAdErrorClientEventArgs()
                {
                    LoadAdErrorClient = new NextGenLoadAdErrorClient(error)
                };
                this.OnAdFailedToLoad(this, args);
            }
        }

        void onAdImpression()
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdShown,
                Format = NativeFormat,
                AdUnitId = _adUnitId,
            });

            if (this.OnAdDidRecordImpression != null)
            {
                this.OnAdDidRecordImpression(this, EventArgs.Empty);
            }
        }

        void onAdClicked()
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdClicked,
                Format = NativeFormat,
                AdUnitId = _adUnitId,
            });

            if (this.OnAdClicked != null)
            {
                this.OnAdClicked();
            }
        }

        void onAdShowedFullScreenContent()
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdShowedFullScreenContent,
                Format = NativeFormat,
                AdUnitId = _adUnitId,
            });

            if (this.OnAdDidPresentFullScreenContent != null)
            {
                this.OnAdDidPresentFullScreenContent(this, EventArgs.Empty);
            }
        }

        void onAdDismissedFullScreenContent()
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdDismissedFullScreenContent,
                Format = NativeFormat,
                AdUnitId = _adUnitId,
            });

            if (this.OnAdDidDismissFullScreenContent != null)
            {
                this.OnAdDidDismissFullScreenContent(this, EventArgs.Empty);
            }
        }

        void onAdFailedToShowFullScreenContent(AndroidJavaObject error)
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdShowedFullScreenContent,
                Format = NativeFormat,
                AdUnitId = _adUnitId,
                Success = false,
            });

            // No-op
        }

        void onPaidEvent(int precision, long valueInMicros, string currencyCode)
        {
            _insightsEmitter.Emit(new Insight()
            {
                Name = Insight.CuiName.AdPaid,
                Format = NativeFormat,
                AdUnitId = _adUnitId,
            });

            if (this.OnPaidEvent != null)
            {
                AdValue adValue = new AdValue()
                {
                    Precision = (AdValue.PrecisionType)precision,
                    Value = valueInMicros,
                    CurrencyCode = currencyCode
                };
                this.OnPaidEvent(adValue);
            }
        }
#endregion

#region Native Template Styling Utilities

        private AndroidJavaObject GetNativeTemplateStyleJavaObject(NativeTemplateStyle tmplStyle)
        {
            AndroidJavaClass nativeTemplateTypeClass =
                new AndroidJavaClass(Utils.UnityNativeTemplateTypeClassName);
            AndroidJavaObject nativeTemplateType = null;
            if (tmplStyle.TemplateId == NativeTemplateId.Small)
            {
                nativeTemplateType =
                    nativeTemplateTypeClass.CallStatic<AndroidJavaObject>("fromIntValue", 2);
            }
            else
            {
                nativeTemplateType =
                    nativeTemplateTypeClass.CallStatic<AndroidJavaObject>("fromIntValue", 3);
            }

            AndroidJavaObject mainBgColor = null;
            if (!tmplStyle.MainBackgroundColor.Equals(Color.clear))
            {
                AndroidJavaClass colorClass = new AndroidJavaClass(Utils.ColorClassName);
                int color = colorClass.CallStatic<int>(
                  "argb", (int)(255 * tmplStyle.MainBackgroundColor.a),
                  (int)(255 * tmplStyle.MainBackgroundColor.r),
                  (int)(255 * tmplStyle.MainBackgroundColor.g),
                  (int)(255 * tmplStyle.MainBackgroundColor.b));
                mainBgColor = new AndroidJavaObject(Utils.ColorDrawableClassName, color);
            }

            AndroidJavaObject primaryTextStyle = null;
            if (tmplStyle.PrimaryText != null)
            {
                primaryTextStyle = GetNativeTemplateTextStyleJavaObject(tmplStyle.PrimaryText);
            }

            AndroidJavaObject secondaryTextStyle = null;
            if (tmplStyle.SecondaryText != null)
            {
                secondaryTextStyle = GetNativeTemplateTextStyleJavaObject(tmplStyle.SecondaryText);
            }

            AndroidJavaObject tertiaryTextStyle = null;
            if (tmplStyle.TertiaryText != null)
            {
                tertiaryTextStyle = GetNativeTemplateTextStyleJavaObject(tmplStyle.TertiaryText);
            }

            AndroidJavaObject c2aTextStyle = null;
            if (tmplStyle.CallToActionText != null)
            {
                c2aTextStyle = GetNativeTemplateTextStyleJavaObject(tmplStyle.CallToActionText);
            }

            AndroidJavaObject nativeAdTemplateStyle = new AndroidJavaObject(
                Utils.UnityNativeTemplateStyleClassName, nativeTemplateType, mainBgColor,
                c2aTextStyle, primaryTextStyle, secondaryTextStyle, tertiaryTextStyle);

            return nativeAdTemplateStyle;
        }

        private AndroidJavaObject GetNativeTemplateTextStyleJavaObject(NativeTemplateTextStyle text)
        {
            AndroidJavaClass colorClass = new AndroidJavaClass(Utils.ColorClassName);
            AndroidJavaObject textColorDrawable = null;
            AndroidJavaObject bgColorDrawable = null;

            if (!text.TextColor.Equals(Color.clear))
            {
                int color = colorClass.CallStatic<int>(
                    "argb", (int)(255 * text.TextColor.a), (int)(255 * text.TextColor.r),
                    (int)(255 * text.TextColor.g), (int)(255 * text.TextColor.b));
                textColorDrawable = new AndroidJavaObject(Utils.ColorDrawableClassName, color);
            }

            if (!text.BackgroundColor.Equals(Color.clear))
            {
                int bgColor = colorClass.CallStatic<int>(
                  "argb", (int)(255 * text.BackgroundColor.a), (int)(255 * text.BackgroundColor.r),
                  (int)(255 * text.BackgroundColor.g), (int)(255 * text.BackgroundColor.b));
                bgColorDrawable = new AndroidJavaObject(Utils.ColorDrawableClassName, bgColor);
            }

            AndroidJavaClass fontStyleClass =
                new AndroidJavaClass(Utils.UnityNativeTemplateFontStyleClassName);
            AndroidJavaObject fontStyle = fontStyleClass.CallStatic<AndroidJavaObject>(
                "fromIntValue", (int)text.Style);

            AndroidJavaObject fontSize =
                new AndroidJavaObject(Utils.DoubleClassName, (double)text.FontSize);

            AndroidJavaObject templateTextStyle =
                new AndroidJavaObject(Utils.UnityNativeTemplateTextStyleClassName,
                                      textColorDrawable, bgColorDrawable, fontStyle, fontSize);
            return templateTextStyle;
        }
#endregion
    }
}
