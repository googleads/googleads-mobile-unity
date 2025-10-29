using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoogleMobileAds.Common
{
    [Serializable]
    public class Insight
    {
        private static AdPlatform _platform;
        private static string _appId;
        private static string _appVersionName;
        private static string _unityVersion;
        private static string _osVersion;
        private static string _deviceModel;

        // Ensure it runs on the main thread before any scene is loaded.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void CacheBaseProperties()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    _platform = AdPlatform.Android;
                    break;
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.OSXPlayer:
                    _platform = AdPlatform.Ios;
                    break;
                default:
                    _platform = AdPlatform.Unity;
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
            StartTimeEpochMillis = (long)DateTime.UtcNow
                .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds;
            Platform = _platform;
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
            AdFailedToLoad = 4,
            AdShown = 5,
            AdClosed = 6,
            AdClicked = 7,
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

        // The name of the insight, commonly referred as a CUI (Critical User Interaction).
        public CuiName Name;

        // If the event associated with the insight succeeded or failed.
        public bool Success;

        // The Epoch time in milliseconds when the insight started.
        public long StartTimeEpochMillis;

        // The GMA SDK version.
        public string SdkVersion;

        // The AdMob / Google Ad Manager app id, or by default the application identifier at
        // runtime.
        public string AppId;

        // The ad unit ID.
        public string AdUnitId;

        // The format of the ad.
        public AdFormat Format;

        // The platform on which the CUI was performed.
        public AdPlatform Platform;

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
                "Insight[Name={0}, Success={1}, StartTimeEpochMillis={2}, SdkVersion='{3}', " +
                "AppId='{4}', AdUnitId='{5}', Format={6}, Platform={7}, AppVersionName='{8}', " +
                "UnityVersion='{9}', OSVersion='{10}', DeviceModel='{11}', Tags='{12}', " +
                "Tracing[OperationName='{13}', Id='{14}', ParentId='{15}', DurationMillis={16}, " +
                "HasEnded={17}], Details='{18}']",
                Name,
                Success,
                StartTimeEpochMillis,
                SdkVersion,
                AppId,
                AdUnitId,
                Format,
                Platform,
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
