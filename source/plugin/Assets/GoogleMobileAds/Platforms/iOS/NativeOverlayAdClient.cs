#if UNITY_IOS
// Copyright 2024 Google LLC
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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    public class NativeOverlayAdClient : INativeOverlayAdClient, IDisposable
    {
        private IntPtr nativeClientPtr;
        private IntPtr nativePtr;

#region native ad callback types
        internal delegate void GADUNativeAdLoadedCallback(IntPtr nativeClient);

        internal delegate void GADUNativeAdFailedToLoadCallback(IntPtr nativeClient, IntPtr error);

        internal delegate void GADUNativePaidEventCallback(
            IntPtr nativeClient, int precision, long value, string currencyCode);

        internal delegate void GADUNativeAdDidRecordImpressionCallback(IntPtr nativeClient);

        internal delegate void GADUNativeAdDidRecordClickCallback(IntPtr nativeClient);

        internal delegate void GADUNativeAdWillPresentScreenCallback(IntPtr nativeClient);

        internal delegate void GADUNativeAdDidDismissScreenCallback(IntPtr nativeClient);
#endregion

        public event EventHandler<EventArgs> OnAdLoaded;
        // Ad event fired when the rewarded ad has failed to load.
        public event EventHandler<LoadAdErrorClientEventArgs> OnAdFailedToLoad;
        // Ad event fired when an ad impression has been recorded.
        public event EventHandler<EventArgs> OnAdDidRecordImpression;
        // Ad event fired when an ad has been clicked.
        public event Action OnAdClicked;
        // Ad event fired when the full screen content has been presented.
        public event EventHandler<EventArgs> OnAdDidPresentFullScreenContent;
        // Ad event fired when the full screen content has been dismissed.
        public event EventHandler<EventArgs> OnAdDidDismissFullScreenContent;

        public event Action<AdValue> OnPaidEvent;

        private static NativeOverlayAdClient IntPtrToNativeClient(IntPtr nativeClient)
        {
            GCHandle handle = (GCHandle)nativeClient;
            return handle.Target as NativeOverlayAdClient;
        }

        private IntPtr NativePtr
        {
            get
            {
                return this.nativePtr;
            }

            set
            {
                Externs.GADURelease(this.nativePtr);
                this.nativePtr = value;
            }
        }

#region INativeOverlayAdClient implementation
        // Loads a native ad
        public void Load(string adUnitID, AdRequest request, NativeAdOptions nativeOptions)
        {
            this.nativeClientPtr = (IntPtr)GCHandle.Alloc(this);
            this.NativePtr = Externs.GADUCreateNativeTemplateAd(this.nativeClientPtr);
            Externs.GADUSetNativeTemplateAdCallbacks(
                this.NativePtr,
                NativeLoadedCallback,
                NativeFailedToLoadCallback,
                AdDidRecordImpressionCallback,
                AdDidRecordClickCallback,
                NativePaidEventCallback,
                NativeAdWillPresentScreenCallback,
                NativeAdDidDismissScreenCallback);

            IntPtr requestPtr = Utils.BuildAdRequest(request);
            IntPtr nativeOptionsPtr = BuildNativeAdOptions(nativeOptions);
            Externs.GADULoadNativeTemplateAd(this.NativePtr, adUnitID, nativeOptionsPtr, requestPtr);
            Externs.GADURelease(requestPtr);
            Externs.GADURelease(nativeOptionsPtr);
        }

        // Hides the native overlay ad from the screen.
        public void Hide()
        {
            Externs.GADUHideNativeTemplateAd(this.NativePtr);
        }

        // Shows the native overlay ad on the screen.
        public void Show()
        {
            Externs.GADUDisplayNativeTemplateAd(this.NativePtr);
        }

        // Renders the native overlay ad on the screen given ad size and position.
        public void Render(NativeTemplateStyle templateViewStyle, AdSize adSize,
                           AdPosition adPosition)
        {
            IntPtr templateStyles = BuildNativeTemplateStyles(templateViewStyle);
            Externs.GADUShowNativeTemplateAd(this.NativePtr, templateStyles, adSize.Height,
                                             adSize.Width);
            Externs.GADUSetNativeTemplateAdPosition(this.NativePtr, (int)adPosition);
        }

        // Renders the native overlay ad on the screen given ad size and custom position.
        public void Render(NativeTemplateStyle templateViewStyle, AdSize adSize, int x, int y)
        {
            IntPtr templateStyles = BuildNativeTemplateStyles(templateViewStyle);
            Externs.GADUShowNativeTemplateAd(this.NativePtr, templateStyles, adSize.Height,
                                                adSize.Width);
          Externs.GADUSetNativeTemplateAdCustomPosition(this.NativePtr, x, y);
        }

        // Renders the native overlay ad on the screen using default sizing and position.
        public void Render(NativeTemplateStyle templateViewStyle, AdPosition adPosition)
        {
          IntPtr templateStyles = BuildNativeTemplateStyles(templateViewStyle);
          Externs.GADUShowDefaultNativeTemplateAd(this.NativePtr, templateStyles);
          Externs.GADUSetNativeTemplateAdPosition(this.NativePtr, (int)adPosition);
        }

        // Renders the native overlay ad on the screen using default sizing and custom position.
        public void Render(NativeTemplateStyle templateViewStyle, int x, int y)
        {
          IntPtr templateStyles = BuildNativeTemplateStyles(templateViewStyle);
          Externs.GADUShowDefaultNativeTemplateAd(this.NativePtr, templateStyles);
          Externs.GADUSetNativeTemplateAdCustomPosition(this.NativePtr, x, y);
        }

        // Set the position of the native overlay ad using standard position.
        public void SetPosition(AdPosition adPosition)
        {
          Externs.GADUSetNativeTemplateAdPosition(this.NativePtr, (int)adPosition);
        }

        // Set the position of the native overlay ad using custom position.
        public void SetPosition(int x, int y)
        {
          Externs.GADUSetNativeTemplateAdCustomPosition(this.NativePtr, x, y);
        }

        // Returns the height of the native overlay in pixels
        public float GetHeightInPixels()
        {
          return Externs.GADUGetNativeTemplateAdHeightInPixels(this.NativePtr);
        }

        // Returns the width of the NativeTemplateView in pixels.
        public float GetWidthInPixels()
        {
          return Externs.GADUGetNativeTemplateAdWidthInPixels(this.NativePtr);
        }

        // Returns the ad request response info
        public IResponseInfoClient GetResponseInfoClient()
        {
          return new ResponseInfoClient(ResponseInfoClientType.AdLoaded, this.NativePtr);
        }

        // Destroys the native overlay ad.
        public void DestroyAd()
        {
            Externs.GADUDestroyNativeTemplateAd(this.NativePtr);
            this.nativePtr = IntPtr.Zero;
        }

        public void Dispose()
        {
            this.DestroyAd();
            ((GCHandle)this.nativeClientPtr).Free();
        }

        ~NativeOverlayAdClient()
        {
            this.Dispose();
        }
#endregion

#region native ad callback methods

        [MonoPInvokeCallback(typeof(GADUNativeAdLoadedCallback))]
        private static void NativeLoadedCallback(IntPtr nativeClient)
        {
            NativeOverlayAdClient client = IntPtrToNativeClient(nativeClient);
            if (client.OnAdLoaded != null)
            {
                client.OnAdLoaded(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUNativeAdFailedToLoadCallback))]
        private static void NativeFailedToLoadCallback(IntPtr nativeClient, IntPtr error)
        {
            NativeOverlayAdClient client = IntPtrToNativeClient(nativeClient);
            if (client.OnAdFailedToLoad != null)
            {
                LoadAdErrorClientEventArgs args = new LoadAdErrorClientEventArgs()
                {
                    LoadAdErrorClient = new LoadAdErrorClient(error)
                };
                client.OnAdFailedToLoad(client, args);
            }
        }

        [MonoPInvokeCallback(typeof(GADUNativePaidEventCallback))]
        private static void NativePaidEventCallback(IntPtr nativeClient, int precision, long value,
                                                    string currencyCode)
        {
            NativeOverlayAdClient client = IntPtrToNativeClient(nativeClient);
            if (client.OnPaidEvent != null)
            {
                AdValue adValue = new AdValue()
                {
                    Precision = (AdValue.PrecisionType)precision,
                    Value = value,
                    CurrencyCode = currencyCode
                };

                client.OnPaidEvent(adValue);
            }
        }

        [MonoPInvokeCallback(typeof(GADUNativeAdDidRecordImpressionCallback))]
        private static void AdDidRecordImpressionCallback(IntPtr nativeClient)
        {
            NativeOverlayAdClient client = IntPtrToNativeClient(nativeClient);
            if (client.OnAdDidRecordImpression != null)
            {
                client.OnAdDidRecordImpression(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUNativeAdDidRecordClickCallback))]
        private static void AdDidRecordClickCallback(IntPtr nativeClient)
        {
            NativeOverlayAdClient client = IntPtrToNativeClient(nativeClient);
            if (client.OnAdClicked != null)
            {
                client.OnAdClicked();
            }
        }

        [MonoPInvokeCallback(typeof(GADUNativeAdLoadedCallback))]
        private static void NativeAdWillPresentScreenCallback(IntPtr nativeClient)
        {
            NativeOverlayAdClient client = IntPtrToNativeClient(nativeClient);
            if (client.OnAdDidPresentFullScreenContent != null)
            {
                client.OnAdDidPresentFullScreenContent(client, EventArgs.Empty);
            }
        }

        [MonoPInvokeCallback(typeof(GADUNativeAdLoadedCallback))]
        private static void NativeAdDidDismissScreenCallback(IntPtr nativeClient)
        {
            NativeOverlayAdClient client = IntPtrToNativeClient(nativeClient);
            if (client.OnAdDidDismissFullScreenContent != null)
            {
                client.OnAdDidDismissFullScreenContent(client, EventArgs.Empty);
            }
        }
#endregion

#region native ad utilities
        public static IntPtr BuildNativeAdOptions(NativeAdOptions options)
        {
            IntPtr videoOptions = Externs.GADUCreateVideoOptions(
                options.VideoOptions.StartMuted, options.VideoOptions.ClickToExpandRequested,
                options.VideoOptions.CustomControlsRequested);
            IntPtr requestPtr = Externs.GADUCreateNativeAdOptions(
                (int)options.AdChoicesPlacement, (int)options.MediaAspectRatio, videoOptions);
            return requestPtr;
        }

        public static IntPtr BuildNativeTemplateStyles(NativeTemplateStyle templateStyle)
        {
            IntPtr templateStyles = Externs.GADUCreateNativeTemplateStyle(templateStyle.TemplateId);

            if (!templateStyle.MainBackgroundColor.Equals(Color.clear))
            {
                IntPtr mainBackgroundColor = Externs.GADUCreateUIColor(
                    templateStyle.MainBackgroundColor.a, templateStyle.MainBackgroundColor.r,
                    templateStyle.MainBackgroundColor.g, templateStyle.MainBackgroundColor.b);
                templateStyles = Externs.GADUSetNativeTemplateStyleBackgroundColor(templateStyles,
                                                                               mainBackgroundColor);
            }

            if (templateStyle.PrimaryText != null)
            {
                IntPtr primaryTemplateTextStyle =
                    BuildNativeTemplateTextStyle(templateStyle.PrimaryText);
                templateStyles = Externs.GADUSetNativeTemplateStyleText(templateStyles, "primary",
                                                                        primaryTemplateTextStyle);
            }

            if (templateStyle.SecondaryText != null)
            {
                IntPtr secondaryTemplateTextStyle =
                    BuildNativeTemplateTextStyle(templateStyle.SecondaryText);
                templateStyles = Externs.GADUSetNativeTemplateStyleText(templateStyles, "secondary",
                                                                        secondaryTemplateTextStyle);
            }

            if (templateStyle.TertiaryText != null)
            {
                IntPtr tertiaryTemplateTextStyle =
                    BuildNativeTemplateTextStyle(templateStyle.TertiaryText);
                templateStyles = Externs.GADUSetNativeTemplateStyleText(templateStyles, "tertiary",
                                                                        tertiaryTemplateTextStyle);
            }

            if (templateStyle.CallToActionText != null)
            {
                IntPtr c2aTemplateTextStyle =
                    BuildNativeTemplateTextStyle(templateStyle.CallToActionText);
                templateStyles = Externs.GADUSetNativeTemplateStyleText(templateStyles,
                                                            "callToAction", c2aTemplateTextStyle);
            }
            return templateStyles;
        }

        public static IntPtr BuildNativeTemplateTextStyle(NativeTemplateTextStyle textStyle)
        {
            IntPtr textPtr = Externs.GADUCreateNativeTemplateTextStyle();

            if (!textStyle.TextColor.Equals(Color.clear))
            {
                IntPtr textColor =
                    Externs.GADUCreateUIColor(textStyle.TextColor.a, textStyle.TextColor.r,
                                              textStyle.TextColor.g, textStyle.TextColor.b);
                textPtr = Externs.GADUSetNativeTemplateTextColor(textPtr, textColor);
            }

            if (!textStyle.BackgroundColor.Equals(Color.clear))
            {
                IntPtr bgColor =
                Externs.GADUCreateUIColor(textStyle.BackgroundColor.a, textStyle.BackgroundColor.r,
                                          textStyle.BackgroundColor.g, textStyle.BackgroundColor.b);
                textPtr = Externs.GADUSetNativeTemplateTextBackgroundColor(textPtr, bgColor);
            }

            textPtr = Externs.GADUSetNativeTemplateTextFontStyle(textPtr, (int)textStyle.Style);

            if (textStyle.FontSize > 0)
            {
                textPtr = Externs.GADUSetNativeTemplateTextFontSize(textPtr, textStyle.FontSize);
            }
            return textPtr;
        }
#endregion
    }
}
#endif
