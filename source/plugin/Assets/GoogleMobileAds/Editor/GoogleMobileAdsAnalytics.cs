// Copyright (C) 2020 Google LLC
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
using System.Reflection;

using Google;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Editor
{
    public class GoogleMobileAdsAnalytics
    {
        private const string GA_TRACKING_ID = "UA-180975654-1";

        private const string MEASUREMENT_ID = "com.google.ads.unity";

        private const string PLUGIN_NAME = "Google Mobile Ads";

        private const string DATA_COLLECTION_DESCRIPTION =
                "Help improve your Google Mobile Ads experience in Unity by automatically " +
                "sending usage data to Google. The data collected is general information " +
                "about how you are using the plugin " +
                "(such as ad unit creation and processing errors). " +
                "Enabling and disabling this feature will not affect your ability " +
                "to use Google Ads or your performance.";

        private const string PRIVACY_POLICY = "https://policies.google.com/privacy";

        private const string DATA_USAGE_URL =
                "https://github.com/googleads/googleads-mobile-unity#analytics";

        private const string OPERATION_SETTINGS = "settings";

        private const string OPERATION_MANIFEST_PROCESSOR = "androidmanifest";

        private const string OPERATION_PLIST_PROCESSOR = "iosplist";

        private static ProjectSettings settings = new ProjectSettings("GoogleMobileAds");

        private static Logger logger = new Logger();

        private static EditorMeasurement analytics = new EditorMeasurement(
                settings, logger, GA_TRACKING_ID, MEASUREMENT_ID, PLUGIN_NAME,
                DATA_COLLECTION_DESCRIPTION, PRIVACY_POLICY)
        {
            BasePath = "/googlemobileads/",
            BaseQuery = string.Format("version={0}", AdRequest.Version),
            InstallSourceFilename = Assembly.GetAssembly(typeof(GoogleMobileAdsAnalytics)).Location,
            DataUsageUrl = DATA_USAGE_URL,
        };

        public static void SetAnalyticsEnabled(bool enable)
        {
            if (enable)
            {
                analytics.PromptToEnable(() => {
                    analytics.Enabled = true;
                });
            }
            else
            {
                analytics.Enabled = false;
            }
        }

        public static bool IsAnalyticsEnabled()
        {
            return analytics.Enabled;
        }

        #region GoogleMobileAdsSettingsEditor.cs

        internal static void ReportSettingsOpened()
        {
            analytics.Report("settings/open", "Plugin Settings: Opened inspector");
        }

        internal static void ReportSettingsDelayAppMeasurementEnabled(bool enabled)
        {
            string path = string.Format("{0}/delay_app_measurement/{1}", OPERATION_SETTINGS, enabled ? "enable" : "disable");
            string message = string.Format("Plugin Settings: Delay app measurement {0}", enabled ? "enabled" : "disabled");

            analytics.Report(path, message);
        }

        #endregion

        #region ManifestProcessor.cs

        internal static void ReportProcessManifestStarted()
        {
            string path = string.Format("{0}/process", OPERATION_MANIFEST_PROCESSOR);
            analytics.Report(path, "ManifestProcesspr: Started processing AndroidManifest.xml");
        }

        internal static void ReportProcessManifestFailedMissingFile()
        {
            string path = string.Format("{0}/process/failed/missing_manifest", OPERATION_MANIFEST_PROCESSOR);
            analytics.Report(path, "ManifestProcesspr: Failed to process AndroidManifest.xml (Missing file)");
        }

        internal static void ReportProcessManifestFailedInvalidFile()
        {
            string path = string.Format("{0}/process/failed/invalid_manifest", OPERATION_MANIFEST_PROCESSOR);
            analytics.Report(path, "ManifestProcesspr: Failed to process AndroidManifest.xml (Invalid file)");
        }

        internal static void ReportProcessManifestFailedEmptyGoogleMobileAdsAppId()
        {
            string path = string.Format("{0}/process/failed/no_gma_appId", OPERATION_MANIFEST_PROCESSOR);
            analytics.Report(path, "ManifestProcesspr: Failed to process AndroidManifest.xml (Empty Google Mobile Ads app ID)");
        }

        internal static void ReportProcessManifestSuccessful()
        {
            string path = string.Format("{0}/process/success", OPERATION_MANIFEST_PROCESSOR);
            analytics.Report(path, "ManifestProcesspr: Successfully processed AndroidManifest.xml");
        }

        #endregion

        #region PListProcessor.cs

        internal static void ReportProcessPlistStarted()
        {
            string path = string.Format("{0}/process", OPERATION_PLIST_PROCESSOR);
            analytics.Report(path, "PListProcessor: Started processing Info.plist");
        }

        internal static void ReportProcessPlistFailedEmptyGoogleMobileAdsAppId()
        {
            string path = string.Format("{0}/process/failed/no_gma_appId", OPERATION_PLIST_PROCESSOR);
            analytics.Report(path, "PListProcessor: Failed to process Info.plist (Empty Google Mobile Ads app ID)");
        }

        internal static void ReportProcessPlistFailedMissingSKAdNetworkIds()
        {
            string path = string.Format("{0}/process/failed/missing_skadnetworkids", OPERATION_PLIST_PROCESSOR);
            analytics.Report(path, "PListProcessor: Failed to process Info.plist (Missing GoogleMobileAdsAdNetworkIDs.xml)");
        }

        internal static void ReportProcessPlistFailedSKAdNetworkIdsIoError()
        {
            string path = string.Format("{0}/process/failed/cannot_read_skadnetworkids", OPERATION_PLIST_PROCESSOR);
            analytics.Report(path, "PListProcessor: Failed to process Info.plist (Can't read GoogleMobileAdsAdNetworkIDs.xml)");
        }

        internal static void ReportProcessPlistFailedInvalidSKAdNetworkIds()
        {
            string path = string.Format("{0}/process/failed/invalid_skadnetworkids", OPERATION_PLIST_PROCESSOR);
            analytics.Report(path, "PListProcessor: Failed to process Info.plist (Invalid GoogleMobileAdsAdNetworkIDs.xml)");
        }

        internal static void ReportProcessPlistSuccessful()
        {
            string path = string.Format("{0}/process/success", OPERATION_PLIST_PROCESSOR);
            analytics.Report(path, "PListProcessor: Successfully processed Info.plist");
        }

        #endregion
    }
}
