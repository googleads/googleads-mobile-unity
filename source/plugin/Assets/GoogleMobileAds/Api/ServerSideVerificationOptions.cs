// Copyright (C) 2019 Google, Inc.
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

namespace GoogleMobileAds.Api
{
    public class ServerSideVerificationOptions
    {
        public string UserId { get; private set; }
        public string CustomData { get; private set; }

        private ServerSideVerificationOptions(Builder builder)
        {
            UserId = builder.UserId;
            CustomData = builder.CustomData;
        }

        public class Builder
        {
            internal string UserId { get; private set; }
            internal string CustomData { get; private set; }

            public Builder()
            {
            }

            public Builder SetUserId(string userId)
            {
                UserId = userId;
                return this;
            }

            public Builder SetCustomData(string customData)
            {
                CustomData = customData;
                return this;
            }

            public ServerSideVerificationOptions Build()
            {
                return new ServerSideVerificationOptions(this);
            }
        }
    }
}

