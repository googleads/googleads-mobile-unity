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

using GoogleMobileAds.Api;

namespace GoogleMobileAds.iOS
{
    internal class Utils
    {
        // Creates an ad request.
        public static IntPtr BuildAdRequest(AdRequest request)
        {
            IntPtr requestPtr = Externs.GADUCreateRequest();
            foreach (string keyword in request.Keywords)
            {
                Externs.GADUAddKeyword(requestPtr, keyword);
            }
            foreach (string deviceId in request.TestDevices)
            {
                Externs.GADUAddTestDevice(requestPtr, deviceId);
            }
            if (request.Birthday.HasValue)
            {
                DateTime birthday = request.Birthday.GetValueOrDefault();
                Externs.GADUSetBirthday(requestPtr, birthday.Year, birthday.Month, birthday.Day);
            }
            if (request.Gender.HasValue)
            {
                Externs.GADUSetGender(requestPtr, (int)request.Gender.GetValueOrDefault());
            }
            if (request.TagForChildDirectedTreatment.HasValue) {
                Externs.GADUTagForChildDirectedTreatment(
                    requestPtr, request.TagForChildDirectedTreatment.GetValueOrDefault());
            }
            foreach (KeyValuePair<string, string> entry in request.Extras)
            {
                Externs.GADUSetExtra(requestPtr, entry.Key, entry.Value);
            }
            Externs.GADUSetRequestAgent(requestPtr, "unity-" + AdRequest.Version);
            return requestPtr;
        }
    }
}

