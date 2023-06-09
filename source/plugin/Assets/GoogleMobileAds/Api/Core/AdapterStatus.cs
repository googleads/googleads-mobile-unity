// Copyright (C) 2018 Google, Inc.
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

namespace GoogleMobileAds.Api
{

    /// <summary>
    /// The initialization state of the mediation adapter.
    /// </summary>
    public enum AdapterState
    {
        /// <summary>
        /// The mediation adapter is not yet ready to service ad requests.
        /// Adapters in this state are more likely than usual to no-fill.
        /// </summary>
        NotReady = 0,
        /// <summary>
        /// The mediation adapter is ready to service ad requests.
        /// </summary>
        Ready = 1
    }

    /// <summary>
    /// An immutable snapshot of a mediation adapter's initialization status.
    /// </summary>
    public class AdapterStatus
    {
        /// <summary>
        /// Gets the adapter's initialization state.
        /// </summary>
        public AdapterState InitializationState { get; private set; }

        /// <summary>
        /// Detailed description of the status.
        /// </summary>
        /// <remarks>
        /// This method should only be used for informational purposes, such as logging.
        /// Use <see cref="AdapterState"/> to make decisions regarding an adapter's status.
        /// </remarks>
        public string Description { get; private set; }

        /// <summary>
        /// The adapter's initialization latency in milliseconds. <c>0</c> if initialization
        /// has not yet ended.
        /// </summary>
        public int Latency { get; private set; }

        internal AdapterStatus(AdapterState state, string description, int latency)
        {
            this.InitializationState = state;
            this.Description = description;
            this.Latency = latency;
        }
    }
}
