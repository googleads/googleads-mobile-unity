using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
    [Serializable]
    public class Insight
    {
        private const string NativePluginAssemblyName = "GoogleMobileAdsNative.Common";
        private const string AndroidNextGenMobileAdsClassName =
            "com.google.android.libraries.ads.mobile.sdk.MobileAds";
        private const string AndroidNextGenUnityMobileAdsClassName =
            "com.google.unity.ads.nextgen.UnityMobileAds";
        private const string AndroidLegacyUnityMobileAdsClassName =
            "com.google.unity.ads.UnityMobileAds";

        /// <summary>
        /// Used to calculate timestamps since the Unix Epoch for .NET Framework 3.5+.
        /// </summary>
        internal static readonly DateTime UnixEpoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static AdPlatform _platform;
        private static string _appId;
        private static string _appVersionName;
        private static string _unityVersion;
        private static string _osVersion;
        private static string _deviceModel;

        internal static AdSdk CachedSdk { get; private set; }
        internal static string CachedSdkVersion { get; private set; }
        internal static string CachedRequestAgent { get; private set; }

        private static string ResolveRequestAgent()
        {
            string nativePluginVersion = null;

            try
            {
                // Include the native plugin version only when its assembly is present at runtime.
                Version version = Assembly.Load(NativePluginAssemblyName)?.GetName().Version;
                if (version != null)
                {
                    nativePluginVersion =
                        string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
                }
            }
            catch (Exception)
            {
            }

            return AdRequest.BuildVersionString(nativePluginVersion);
        }

        // Ensure it runs on the main thread before any scene is loaded.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        internal static void CacheBaseProperties()
        {
            CachedSdk = AdSdk.Unknown;
            CachedSdkVersion = null;
            CachedRequestAgent = ResolveRequestAgent();
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    _platform = AdPlatform.Android;
                    bool isNextGen = false;
                    try
                    {
                        using (var _ = new AndroidJavaClass(
                            AndroidNextGenMobileAdsClassName))
                        {
                            isNextGen = true;
                        }
                    }
                    catch
                    {
                        isNextGen = false;
                    }

                    if (isNextGen)
                    {
                        CachedSdk = AdSdk.AndroidNextGen;
                        try
                        {
                            using (var unityMobileAds = new AndroidJavaClass(
                                AndroidNextGenUnityMobileAdsClassName))
                            {
                                CachedSdkVersion =
                                    unityMobileAds.CallStatic<string>("getSdkVersionString");
                            }
                        }
                        catch
                        {
                            CachedSdkVersion = null;
                        }
                    }
                    else
                    {
                        CachedSdk = AdSdk.AndroidLegacy;
                        try
                        {
                            using (var unityMobileAds = new AndroidJavaClass(
                                AndroidLegacyUnityMobileAdsClassName))
                            {
                                CachedSdkVersion =
                                    unityMobileAds.CallStatic<string>("getSdkVersionString");
                            }
                        }
                        catch
                        {
                            CachedSdkVersion = null;
                        }
                    }
                    break;
                case RuntimePlatform.IPhonePlayer:
                    _platform = AdPlatform.Ios;
                    CachedSdk = AdSdk.Ios;
                    try
                    {
                        Type externsType =
                            Type.GetType("GoogleMobileAds.iOS.Externs, GoogleMobileAds.iOS");
                        MethodInfo versionMethod = externsType?.GetMethod(
                            "GADUMobileAdsVersion",
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        CachedSdkVersion = versionMethod?.Invoke(null, null) as string;
                    }
                    catch
                    {
                        CachedSdkVersion = null;
                    }
                    break;
                case RuntimePlatform.OSXPlayer:
                    _platform = AdPlatform.Ios;
                    CachedSdk = AdSdk.Unknown;
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
            RequestAgent = CachedRequestAgent;
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
            PictureInPicture = 7,
            SqueezeBack = 8,
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
            AndroidNextGen = 1,
            AndroidLegacy = 2,
            Ios = 3,
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

        // The request agent identifying the Unity and optional native plugin versions, e.g.,
        // 'unity-11.6.0' or 'unity-11.6.0-native-11.2.0'.
        public string RequestAgent;

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
                "RequestAgent='{9}', AppVersionName='{10}', UnityVersion='{11}', " +
                "OSVersion='{12}', DeviceModel='{13}', Tags='{14}', " +
                "Tracing[OperationName='{15}', Id='{16}', ParentId='{17}', " +
                "DurationMillis={18}, HasEnded={19}], Details='{20}']",
                Name,
                Success,
                StartTimeEpochMillis,
                AppId,
                AdUnitId,
                Format,
                Platform,
                Sdk,
                SdkVersion,
                RequestAgent,
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
