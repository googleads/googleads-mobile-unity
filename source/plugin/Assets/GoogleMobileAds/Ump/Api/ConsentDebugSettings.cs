using System.Collections.Generic;

namespace GoogleMobileAds.Ump.Api
{
    /// <summary>
    /// Debug settings for testing User Messaging Platform.
    /// </summary>
    public class ConsentDebugSettings
    {
        /// <summary>
        /// Debug values for testing geography.
        /// </summary>
        public enum DebugGeography
        {
            /// <summary>
            /// Debug geography disabled.
            /// </summary>
            DEBUG_GEOGRAPHY_DISABLED = 0,
            /// <summary>
            /// Geography appears as in EEA for debug devices.
            /// </summary>
            DEBUG_GEOGRAPHY_EEA = 1,
            /// <summary>
            ///  Geography appears as not in EEA for debug devices.
            /// </summary>
            DEBUG_GEOGRAPHY_NOT_EEA = 2,
        }
        
        /// <summary>
        /// The debug geography for testing purposes.
        /// </summary>
        public DebugGeography DebugGeography  { get; private set; }
        public List<string> TestDeviceHashedIds  { get; private set; }
        
        private ConsentDebugSettings(Builder builder)
        {
            this.DebugGeography = builder.DebugGeography;
            this.TestDeviceHashedIds = builder.TestDeviceHashedIds;
        }

        public Builder ToBuilder()
        {
            Builder builder = new Builder()
                    .SetDebugGeography(this.DebugGeography)
                    .SetTestDeviceHashedIds(this.TestDeviceHashedIds);
            return builder;
        }

        /** Builder of ConsentDebugSettings. */
        public static class Builder
        {

            internal DebugGeography DebugGeography  { get; private set; }
            internal List<string> TestDeviceHashedIds  { get; private set; }

            /// <summary>
            /// Sets the debug geography for testing purposes.
            /// Default value is DebugGeography.DEBUG_GEOGRAPHY_DISABLED.
            /// </summary>
            public Builder SetDebugGeography(DebugGeography DebugGeography)
            {
                this.DebugGeography = DebugGeography;
                return this;
            }

            /// <summary>
            /// Registers a device as a test device. 
            /// Test devices respect debug geography settings to enable easier testing. 
            /// Test devices must be added individually so that debug geography
            /// settings won't accidentally get released to all users.
            /// 
            /// You can access the hashedDeviceId from log once your app calls 
            /// ConsentInformation.
            /// </summary>
            /// <typeparam name="string">hashedId The hashed device ID that 
            /// should be considered a debug device.</typeparam>
            public Builder AddTestDeviceHashedId(String hashedId)
            {
                this.testDeviceHashedIds.add(hashedId);
                return this;
            }

            public Builder SetTestDeviceHashedIds(List<string> testDeviceIds)
            {
                this.testDeviceHashedIds = testDeviceHashedIds;
                return this;
            }

            /// Builds the ConsentDebugSettings.
            public ConsentDebugSettings build() {
                return new ConsentDebugSettings(this);
            }
        }
    }
}
