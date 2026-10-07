using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GoogleMobileAds.Common
{
    [Serializable]
    public class Insight
    {
        /// <summary>
        /// Used to calculate timestamps since the Unix Epoch for .NET Framework 3.5+.
        /// </summary>
        internal static readonly DateTime UnixEpoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        [DllImport("__Internal")]
        private static extern string GADUMobileAdsVersion();

        private static AdPlatform _platform;
        private static string _appId;
        private static string _appVersionName;
        private static string _unityVersion;
        private static string _osVersion;
        private static string _deviceModel;

        internal static AdSdk CachedSdk { get; private set; }
        internal static string CachedSdkVersion { get; private set; }

        // Ensure it runs on the main thread before any scene is loaded.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        internal static void CacheBaseProperties()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    _platform = AdPlatform.Android;
                    try
                    {
                        using (var _ = new AndroidJavaClass(
                            "com.google.android.libraries.ads.mobile.sdk.MobileAds"))
                        {
                            CachedSdk = AdSdk.Decagon;
                            using (var unityMobileAds = new AndroidJavaClass(
                                "com.google.unity.ads.nextgen.UnityMobileAds"))
                            {
                                CachedSdkVersion =
                                    unityMobileAds.CallStatic<string>("getSdkVersionString");
                            }
                        }
                    }
                    catch
                    {
                        try
                        {
                            CachedSdk = AdSdk.NonagonAndroid;
                            using (var unityMobileAds = new AndroidJavaClass(
                                "com.google.unity.ads.UnityMobileAds"))
                            {
                                CachedSdkVersion =
                                    unityMobileAds.CallStatic<string>("getSdkVersionString");
                            }
                        }
                        catch
                        {
                            CachedSdk = AdSdk.Unknown;
                        }
                    }
                    break;
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.OSXPlayer:
                    _platform = AdPlatform.Ios;
                    CachedSdk = AdSdk.NonagonIos;
                    try
                    {
                        CachedSdkVersion = GADUMobileAdsVersion();
                    }
                    catch
                    {
                        CachedSdk = AdSdk.Unknown;
                    }
                    break;
                default:
                    _platform = AdPlatform.Unity;
                    CachedSdk = AdSdk.Unknown;
                    break;
            }
            _appId = Application.identifier;
            _appVersionName = Application.version;
            _unityVersion = Application.unityVersion;
            _osVersion = SystemInfo.operatingSystem;
            _deviceModel = SystemInfo.deviceModel;
        }

        public Insight()
        {
            Success = true;
            StartTimeEpochMillis = (long)DateTime.UtcNow
                .Subtract(UnixEpoch)
                .TotalMilliseconds;
            Platform = _platform;
            Sdk = CachedSdk;
            SdkVersion = CachedSdkVersion;
            AppId = _appId;
            AppVersionName = _appVersionName;
            UnityVersion = _unityVersion;
            OSVersion = _osVersion;
            DeviceModel = _deviceModel;
        }

        [Serializable]
        public class TracingActivity
        {
            // The provided activity name.
            public string OperationName;

            // The ID of this trace (following the W3C Trace Context format).
            public string Id;

            // The ID of the parent trace (following the W3C Trace Context format).
            public string ParentId;

            // The duration of this trace in milliseconds.
            public long DurationMillis;

            // False if the trace started and is running, true if it stopped (i.e. ended).
            public bool HasEnded;
        }

        public enum CuiName
        {
            Unknown = 0,
            SdkInitialized = 1,
            AdRequested = 2,
            AdLoaded = 3,
            [Obsolete("Use CuiName.AdLoaded with Success = false instead.")]
            AdFailedToLoad = 4,
            AdShown = 5,
            AdClosed = 6,
            AdClicked = 7,
            AdOpened = 8,
            AdPaid = 9,
            AdShowedFullScreenContent = 10,
            AdDismissedFullScreenContent = 11,
            UserEarnedReward = 12,
        }

        public enum AdFormat
        {
            Unknown = 0,
            Banner = 1,
            Interstitial = 2,
            Rewarded = 3,
            RewardedInterstitial = 4,
            AppOpen = 5,
            Native = 6,
        }

        public enum AdPlatform
        {
            Unknown = 0,
            Android = 1,
            Ios = 2,
            Unity = 3,
        }

        public enum AdSdk
        {
            Unknown = 0,
            NonagonAndroid = 1,
            NonagonIos = 2,
            Decagon = 3,
        }

        // The name of the insight, commonly referred as a CUI (Critical User Interaction).
        public CuiName Name;

        // If the event associated with the insight succeeded or failed.
        public bool Success;

        // The Epoch time in milliseconds when the insight started.
        public long StartTimeEpochMillis;

        // The AdMob / Google Ad Manager app id, or by default the application identifier at
        // runtime.
        public string AppId;

        // The ad unit ID.
        public string AdUnitId;

        // The format of the ad.
        public AdFormat Format;

        // The platform on which the CUI was performed.
        public AdPlatform Platform;

        // The underlying GMA SDK.
        public AdSdk Sdk;

        // The GMA SDK version.
        public string SdkVersion;

        // The application version.
        public string AppVersionName;

        // The Unity version.
        public string UnityVersion;

        // The OS version.
        public string OSVersion;

        // The device model.
        public string DeviceModel;

        // The keywords associated with the insight. They are used to group insights together for
        // analysis.
        public List<string> Tags;

        // The tracing activity associated with the insight.
        public TracingActivity Tracing;

        // Any additional details about the insight.
        public string Details;

        // Returns a string representation of the insight.
        public override string ToString()
        {
            return string.Format(
                "Insight[Name={0}, Success={1}, StartTimeEpochMillis={2}, AppId='{3}', " +
                "AdUnitId='{4}', Format={5}, Platform={6}, Sdk='{7}', SdkVersion='{8}', " +
                "AppVersionName='{9}', UnityVersion='{10}', OSVersion='{11}', " +
                "DeviceModel='{12}', Tags='{13}', Tracing[OperationName='{14}', Id='{15}', " +
                "ParentId='{16}', DurationMillis={17}, HasEnded={18}], Details='{19}']",
                Name,
                Success,
                StartTimeEpochMillis,
                AppId,
                AdUnitId,
                Format,
                Platform,
                Sdk,
                SdkVersion,
                AppVersionName,
                UnityVersion,
                OSVersion,
                DeviceModel,
                Tags != null ? string.Join(",", Tags) : "",
                Tracing != null ? Tracing.OperationName : "",
                Tracing != null ? Tracing.Id : "",
                Tracing != null ? Tracing.ParentId : "",
                Tracing != null ? Tracing.DurationMillis : 0,
                Tracing != null ? Tracing.HasEnded : false,
                Details);
        }

        // Returns a JSON string representation of the insight.
        public string ToJson()
        {
            return JsonUtility.ToJson(this, prettyPrint: true);
        }
    }
}
