#if UNITY_IOS
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
using System.Runtime.InteropServices;

using GoogleMobileAds.Api;
using GoogleMobileAds.Api.AdManager;
using GoogleMobileAds.Api.Mediation;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    internal class Utils
    {
        /// <summary>
        /// Converts the plugin AdRequest object to a native ios proxy object for use by the sdk.
        /// </summary>
        /// <param name="AdRequest">the AdRequest from the unity plugin.</param>
        /// <param name="nativePluginVersion">the version string of the native plugin.</param>
        public static IntPtr BuildAdRequest(AdRequest request, string nativePluginVersion = null)
        {
            IntPtr requestPtr = Externs.GADUCreateRequest();
            foreach (string keyword in request.Keywords)
            {
                Externs.GADUAddKeyword(requestPtr, keyword);
            }

            foreach (KeyValuePair<string, string> entry in request.Extras)
            {
                Externs.GADUSetExtra(requestPtr, entry.Key, entry.Value);
            }

            Externs.GADUSetExtra(requestPtr, "is_unity", "1");

            // Makes ads that contain WebP ad assets ineligible.
            Externs.GADUSetExtra(requestPtr, "adw", "true");

            foreach (MediationExtras mediationExtra in request.MediationExtras)
            {
                IntPtr mutableDictionaryPtr = Externs.GADUCreateMutableDictionary();
                if (mutableDictionaryPtr != IntPtr.Zero)
                {
                    foreach (KeyValuePair<string, string> entry in mediationExtra.Extras)
                    {
                        Externs.GADUMutableDictionarySetValue(
                                mutableDictionaryPtr,
                                entry.Key,
                                entry.Value);
                    }

                    Externs.GADUSetMediationExtras(
                            requestPtr,
                            mutableDictionaryPtr,
                            mediationExtra.IOSMediationExtraBuilderClassName);
                }
            }

            Externs.GADUSetRequestAgent(requestPtr,
                    AdRequest.BuildVersionString(nativePluginVersion));
            return requestPtr;
        }

        public static IntPtr BuildAdManagerAdRequest(AdRequest request,
                                                     string nativePluginVersion = null)
        {
            if (!typeof(AdManagerAdRequest).IsInstanceOfType(request))
            {
                return BuildAdRequest(request, nativePluginVersion);
            }
            AdManagerAdRequest adManagerAdRequest = (AdManagerAdRequest)request;
            IntPtr requestPtr = Externs.GAMUCreateRequest();

            foreach (string keyword in adManagerAdRequest.Keywords)
            {
                Externs.GADUAddKeyword(requestPtr, keyword);
            }

            foreach (KeyValuePair<string, string> entry in adManagerAdRequest.Extras)
            {
                Externs.GADUSetExtra(requestPtr, entry.Key, entry.Value);
            }

            Externs.GADUSetExtra(requestPtr, "is_unity", "1");

            // Makes ads that contain WebP ad assets ineligible.
            Externs.GADUSetExtra(requestPtr, "adw", "true");

            foreach (MediationExtras mediationExtra in adManagerAdRequest.MediationExtras)
            {
                IntPtr mutableDictionaryPtr = Externs.GADUCreateMutableDictionary();
                if (mutableDictionaryPtr != IntPtr.Zero)
                {
                    foreach (KeyValuePair<string, string> entry in mediationExtra.Extras)
                    {
                        Externs.GADUMutableDictionarySetValue(
                                mutableDictionaryPtr,
                                entry.Key,
                                entry.Value);
                    }

                    Externs.GADUSetMediationExtras(
                            requestPtr,
                            mutableDictionaryPtr,
                            mediationExtra.IOSMediationExtraBuilderClassName);
                }
            }

            Externs.GADUSetRequestAgent(requestPtr,
                                        AdRequest.BuildVersionString(nativePluginVersion));

            Externs.GAMUSetPublisherProvidedID(requestPtr, adManagerAdRequest.PublisherProvidedId);
            foreach (string category in adManagerAdRequest.CategoryExclusions)
            {
                Externs.GAMUAddCategoryExclusion(requestPtr, category);
            }
            foreach (KeyValuePair<string, string> entry in adManagerAdRequest.CustomTargeting)
            {
                Externs.GAMUSetCustomTargeting(requestPtr, entry.Key, entry.Value);
            }
            return requestPtr;
        }

        public static IntPtr BuildServerSideVerificationOptions(ServerSideVerificationOptions options)
        {
            IntPtr optionsPtr = Externs.GADUCreateServerSideVerificationOptions();
            Externs.GADUServerSideVerificationOptionsSetUserId(optionsPtr, options.UserId);
            Externs.GADUServerSideVerificationOptionsSetCustomRewardString(optionsPtr, options.CustomData);

            return optionsPtr;
        }

        public static string PtrToString(IntPtr stringPtr) {
            string managedString = Marshal.PtrToStringAnsi(stringPtr);
            Marshal.FreeHGlobal(stringPtr);
            return managedString;
        }

        public static List<string> PtrArrayToManagedList(IntPtr arrayPtr, int numOfAssets) {
            IntPtr[] intPtrArray = new IntPtr[numOfAssets];
            string[] managedAssetArray = new string[numOfAssets];
            Marshal.Copy(arrayPtr, intPtrArray, 0, numOfAssets);

            for (int i = 0; i < numOfAssets; i++)
            {
                managedAssetArray[i] = Marshal.PtrToStringAuto(intPtrArray[i]);
                Marshal.FreeHGlobal(intPtrArray[i]);
            }

            Marshal.FreeHGlobal(arrayPtr);
            return new List<string>(managedAssetArray);
        }
    }
}
#endif
