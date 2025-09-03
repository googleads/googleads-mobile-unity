using System;
using System.Collections.Generic;
using UnityEngine;

// LINT.IfChange
namespace GoogleMobileAds.Common
{
    // Protos are not supported in g3 for C#: http://yaqs/6221611834537934848. Instead, we use
    // JSON Lines (http://jsonlines.org) to serialize the insights.
    [Serializable]
    public class Insight
    {
        public Insight()
        {
            StartTimeMillis = (long)DateTime.UtcNow
                .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds;
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

        // The Epoch time in milliseconds when the CUI started.
        public long StartTimeMillis;

        // How long the operation took to complete in milliseconds.
        public long LatencyMillis;

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

        // Any additional details about the insight.
        public string Details;

        // Returns a string representation of the insight.
        public override string ToString()
        {
            return string.Format(
                "Insight[Name={0}, Success={1}, StartTimeMillis={2}, LatencyMillis={3}, " +
                "SdkVersion='{4}', AppId='{5}', AdUnitId='{6}', Format={7}, Platform={8}, " +
                "Tags='{9}', Details='{10}']",
                Name,
                Success,
                StartTimeMillis,
                LatencyMillis,
                SdkVersion,
                AppId,
                AdUnitId,
                Format,
                Platform,
                string.Join(",", Tags),
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
