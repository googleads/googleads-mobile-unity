// Copyright (C) 2015 Google, Inc.
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

using GoogleMobileAds.Api.Mediation;

namespace GoogleMobileAds.Api
{
    public class AdRequest
    {
        public const string Version = "6.1.0";
        public const string TestDeviceSimulator = "SIMULATOR";

        private AdRequest(Builder builder)
        {
            this.Keywords = new HashSet<string>(builder.Keywords);
            this.Extras = new Dictionary<string, string>(builder.Extras);
            this.MediationExtras = builder.MediationExtras;
        }

        public HashSet<string> Keywords { get; private set; }

        public Dictionary<string, string> Extras { get; private set; }

        public List<MediationExtras> MediationExtras { get; private set; }

        public class Builder
        {
            public Builder()
            {
                this.Keywords = new HashSet<string>();
                this.Extras = new Dictionary<string, string>();
                this.MediationExtras = new List<MediationExtras>();
            }

            internal HashSet<string> Keywords { get; private set; }

            internal Dictionary<string, string> Extras { get; private set; }

            internal List<MediationExtras> MediationExtras { get; private set; }

            public Builder AddKeyword(string keyword)
            {
                this.Keywords.Add(keyword);
                return this;
            }

            public AdRequest Build()
            {
                return new AdRequest(this);
            }

            public Builder AddMediationExtras(MediationExtras extras)
            {
                this.MediationExtras.Add(extras);
                return this;
            }

            public Builder AddExtra(string key, string value)
            {
                this.Extras.Add(key, value);
                return this;
            }
        }
    }
}
