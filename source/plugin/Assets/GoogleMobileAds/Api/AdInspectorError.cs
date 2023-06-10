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

using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api {

  /// <summary>
  /// Error information about why the ad inspector failed.
  /// <summary>
  public class AdInspectorError : AdError {

        /// <summary>
        /// Error information about why the ad inspector failed.
        /// <summary>
        public enum AdInspectorErrorCode {
          /// <summary>
          /// Ad inspector had an internal error.
          /// <summary>
          ERROR_CODE_INTERNAL_ERROR = 0,
          /// <summary>
          /// Ad inspector failed to load.
          /// <summary>
          ERROR_CODE_FAILED_TO_LOAD = 1,
          /// <summary>
          /// Ad inspector cannot be opened because the device is not in test mode.
          /// information.
          /// <summary>
          ERROR_CODE_NOT_IN_TEST_MODE = 2,
          /// <summary>
          /// Ad inspector cannot be opened because it is already open.
          /// <summary>
          ERROR_CODE_ALREADY_OPEN = 3
        }

    public AdInspectorError(IAdInspectorErrorClient client) : base(client) {}

    public new AdInspectorErrorCode GetCode() {
      return (AdInspectorErrorCode)base.GetCode();
    }
  }
}
