using System;
using System.Collections.Generic;
using UnityEngine;

// LINT.IfChange
namespace GoogleMobileAds.Common
{
    [Serializable]
    public class Insight
    {
        public Insight()
        {
            StartTimeEpochMillis = (long)DateTime.UtcNow
                .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds;
        }

        [Serializable]
        public class TracingActivity
        {
            // The provided activity name.
            public string OperationName;

            // The UUID of this trace.
            public string Id;

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

        // The AdMob / Google Ad Manager app id.
        public string AppId;

        // The ad unit ID.
        public string AdUnitId;

        // The format of the ad.
        public AdFormat Format;

        // The platform on which the CUI was performed.
        public AdPlatform Platform;

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
                "AppId='{4}', AdUnitId='{5}', Format={6}, Platform={7}, Tags='{8}', " +
                "Tracing[OperationName='{9}', Id='{10}', DurationMillis={11}, HasEnded={12}], " +
                "Details='{13}']",
                Name,
                Success,
                StartTimeEpochMillis,
                SdkVersion,
                AppId,
                AdUnitId,
                Format,
                Platform,
                Tags != null ? string.Join(",", Tags) : "",
                Tracing != null ? Tracing.OperationName : "",
                Tracing != null ? Tracing.Id : "",
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
// LINT.ThenChange(//depot/google3/javatests/com/google/android/apps/internal/admobsdk/mediumtest/unityplugin/Insight.java)
