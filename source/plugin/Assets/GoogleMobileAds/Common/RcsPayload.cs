using System;
using System.Globalization;
using UnityEngine;

namespace GoogleMobileAds.Common
{
    /// <summary>
    /// Holds all static metadata about the SDK environment. This info is gathered once at startup.
    /// </summary>
    [Serializable]
    public struct StaticMetadata
    {
        public string session_id;
        public string app_id;
        public string app_version_name;
        public string platform;
        public string unity_version;
        public string os_version;
        public string device_model;
        public string country;
        public int total_cpu;
        // JSPB compatibility: 64-bit integers must be sent as strings to avoid precision loss.
        public string total_memory_bytes;
    }

    /// <summary>
    /// Holds all dynamic metadata about the SDK environment. This info is gathered at request time.
    /// </summary>
    [Serializable]
    public struct DynamicMetadata
    {
        public string network_type;
        public string orientation;
    }

    /// <summary>
    /// Helper class for RCS message payloads to retrieve static and dynamic metadata.
    /// </summary>
    public static class RcsPayload
    {
        private static StaticMetadata _staticMetadata;
        private static bool _staticMetadataInitialized = false;

        /// <summary>
        /// Gathers all static device and app info once. Must be called from main thread during
        /// initialization.
        /// </summary>
        public static void InitializeStaticMetadata()
        {
            if (_staticMetadataInitialized)
            {
                return;
            }
            _staticMetadata = new StaticMetadata
            {
                session_id = System.Guid.NewGuid().ToString(),
                app_id = Application.identifier,
                app_version_name = Application.version,
                platform = Application.platform.ToString(),
                unity_version = Application.unityVersion,
                os_version = SystemInfo.operatingSystem,
                device_model = SystemInfo.deviceModel,
                country = RegionInfo.CurrentRegion.TwoLetterISORegionName,
                total_cpu = SystemInfo.processorCount,
                // Convert MB to bytes.
                total_memory_bytes = ((long)SystemInfo.systemMemorySize * 1024 * 1024).ToString(),
            };
            _staticMetadataInitialized = true;
        }

        public static StaticMetadata GetStaticMetadata()
        {
            if (!_staticMetadataInitialized)
            {
                throw new InvalidOperationException("Static metadata not initialized. " +
                    "Call 'InitializeStaticMetadata()' first from the main thread.");
            }
            return _staticMetadata;
        }

        public static DynamicMetadata GetDynamicMetadata()
        {
            return new DynamicMetadata
            {
                network_type = Application.internetReachability.ToString(),
                orientation = Screen.orientation.ToString(),
            };
        }
    }
}
