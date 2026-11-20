// Copyright (C) 2022 Google LLC.
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
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

using GoogleMobileAds.Ump.Common;

namespace GoogleMobileAds.Ump.Api
{
    /// <summary>
    /// Utility methods for managing consent information from users.
    /// </summary>
    public class ConsentInformation
    {
        internal static IUmpClientFactory ClientFactory
        {
            get
            {
                if (_clientFactory == null)
                {
                    _clientFactory = Utils.GetClientFactory();
                }
                return _clientFactory;
            }
            set
            {
                _clientFactory = value;
            }
        }
        private static IUmpClientFactory _clientFactory;

        // Runtime settings written to StreamingAssets by the GMA Unity plugin at build time.
        private const string RuntimeSettingsDirectory = "GoogleMobileAds";
        private const string RuntimeSettingsFileName = "GoogleAdsRuntimeSettings.xml";

        /// <summary>
        /// The user's consent status.
        /// This value is cached between app sessions and can be read before
        /// requesting updated parameters.
        /// </summary>
        public static ConsentStatus ConsentStatus
        {
            get
            {
                IConsentInformationClient client = ClientFactory.ConsentInformationClient();
                return (ConsentStatus)client.GetConsentStatus();
            }
        }

        /// <summary>
        /// Privacy options requirement status.
        /// </summary>
        public static PrivacyOptionsRequirementStatus PrivacyOptionsRequirementStatus
        {
            get
            {
                IConsentInformationClient client = ClientFactory.ConsentInformationClient();
                return (PrivacyOptionsRequirementStatus)client.GetPrivacyOptionsRequirementStatus();
            }
        }

        /// <summary>
        /// Requests consent information update.
        /// </summary>
        /// <remarks>
        /// The debug geography and test device hashed IDs configured in the Google Mobile Ads
        /// settings are applied to <c>request.ConsentDebugSettings</c> before the update is
        /// requested.
        /// </remarks>
        /// <param name="request">The request params.</param>
        /// <param name="consentInfoUpdateCallback">Called when client updates ConsentStatus or
        /// returns a FormError argument with ErrorCode and Message. </param>
        public static void Update(ConsentRequestParameters request,
                                  Action<FormError> consentInfoUpdateCallback)
        {
            LoadRuntimeSettingsXml(xml =>
            {
                ApplyRuntimeSettings(request, xml);
                UpdateConsentInfo(request, consentInfoUpdateCallback);
            });
        }

        private static void UpdateConsentInfo(ConsentRequestParameters request,
                                              Action<FormError> consentInfoUpdateCallback)
        {
            IConsentInformationClient client = ClientFactory.ConsentInformationClient();

            client.Update(request, () =>
            {
                if (consentInfoUpdateCallback != null)
                {
                    GoogleMobileAds.Api.MobileAds.RaiseAction(() =>
                    {
                        consentInfoUpdateCallback(null);
                    });
                }
            }, error =>
            {
                if (consentInfoUpdateCallback != null)
                {
                    GoogleMobileAds.Api.MobileAds.RaiseAction(() =>
                    {
                        consentInfoUpdateCallback(error);
                    });
                }
            });
        }

        /// <summary>
        /// Reads the runtime settings file from StreamingAssets and passes its contents to
        /// <paramref name="onLoaded"/>, or null if the file can't be read. Never throws, so that
        /// reading the settings can't break the consent info update.
        /// </summary>
        private static void LoadRuntimeSettingsXml(Action<string> onLoaded)
        {
            string path = null;
            string xml = null;
            UnityWebRequest webRequest = null;
            UnityWebRequestAsyncOperation operation = null;
            try
            {
                path = Path.Combine(Application.streamingAssetsPath,
                                    RuntimeSettingsDirectory,
                                    RuntimeSettingsFileName);

                // On Android, StreamingAssets are packed inside the APK and can only be read
                // with UnityWebRequest. On other platforms they are regular files.
                if (path.Contains("://"))
                {
                    webRequest = UnityWebRequest.Get(path);
                    operation = webRequest.SendWebRequest();
                }
                else if (File.Exists(path))
                {
                    xml = File.ReadAllText(path);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to read " + path + ": " + e.Message);
                if (webRequest != null)
                {
                    webRequest.Dispose();
                }
                webRequest = null;
                operation = null;
            }

            if (operation == null)
            {
                onLoaded(xml);
                return;
            }

            operation.completed += completedOperation =>
            {
                string downloadedXml = null;
                try
                {
                    if (string.IsNullOrEmpty(webRequest.error))
                    {
                        downloadedXml = webRequest.downloadHandler.text;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Failed to read " + path + ": " + e.Message);
                }
                finally
                {
                    webRequest.Dispose();
                }
                onLoaded(downloadedXml);
            };
        }

        /// <summary>
        /// Applies the DebugGeography and TestDeviceHashedIds values from the runtime settings XML
        /// to <c>request.ConsentDebugSettings</c>.
        /// </summary>
        /// <remarks>
        /// <para>DebugGeography is only overridden if the settings value is not Disabled, so that a
        /// debug geography set in code is kept. Test device hashed IDs are added to any IDs already
        /// set in code.</para>
        /// <para>The file has a fixed, flat format generated by the GMA Unity plugin, so it is
        /// parsed with simple string matching to avoid adding a System.Xml dependency to this
        /// assembly.</para>
        /// </remarks>
        internal static void ApplyRuntimeSettings(ConsentRequestParameters request, string xml)
        {
            if (request == null || string.IsNullOrEmpty(xml))
            {
                return;
            }

            if (request.ConsentDebugSettings == null)
            {
                request.ConsentDebugSettings = new ConsentDebugSettings();
            }
            ConsentDebugSettings debugSettings = request.ConsentDebugSettings;

            List<string> debugGeographyValues = GetXmlElementValues(xml, "DebugGeography");
            int debugGeography;
            if (debugGeographyValues.Count > 0 &&
                int.TryParse(debugGeographyValues[0], out debugGeography) &&
                debugGeography != (int)DebugGeography.Disabled)
            {
                debugSettings.DebugGeography = (DebugGeography)debugGeography;
            }

            if (debugSettings.TestDeviceHashedIds == null)
            {
                debugSettings.TestDeviceHashedIds = new List<string>();
            }
            foreach (string id in GetXmlElementValues(xml, "TestDeviceHashedId"))
            {
                if (!string.IsNullOrEmpty(id) && !debugSettings.TestDeviceHashedIds.Contains(id))
                {
                    debugSettings.TestDeviceHashedIds.Add(id);
                }
            }
        }

        // Returns the text values of all <elementName>...</elementName> elements in the XML.
        internal static List<string> GetXmlElementValues(string xml, string elementName)
        {
            List<string> values = new List<string>();
            if (string.IsNullOrEmpty(xml))
            {
                return values;
            }

            string openTag = "<" + elementName + ">";
            string closeTag = "</" + elementName + ">";
            int index = 0;
            while ((index = xml.IndexOf(openTag, index, StringComparison.Ordinal)) >= 0)
            {
                int valueStart = index + openTag.Length;
                int valueEnd = xml.IndexOf(closeTag, valueStart, StringComparison.Ordinal);
                if (valueEnd < 0)
                {
                    break;
                }
                values.Add(UnescapeXml(xml.Substring(valueStart, valueEnd - valueStart).Trim()));
                index = valueEnd + closeTag.Length;
            }
            return values;
        }

        private static string UnescapeXml(string value)
        {
            return value.Replace("&lt;", "<")
                        .Replace("&gt;", ">")
                        .Replace("&quot;", "\"")
                        .Replace("&apos;", "'")
                        .Replace("&amp;", "&");
        }

        /// <summary>
        /// Check if the app has finished all the required consent flow and can request ads now.
        /// </summary>
        public static bool CanRequestAds()
        {
            IConsentInformationClient client = ClientFactory.ConsentInformationClient();
            return client.CanRequestAds();
        }

        /// <summary>
        /// Clears all consent status from persistent storage.
        /// </summary>
        public static void Reset()
        {
            IConsentInformationClient client = ClientFactory.ConsentInformationClient();
            client.Reset();
        }

        /// <summary>
        /// Returns <c>true</c> if a
        /// <see cref="GoogleMobileAds.Ump.Api.ConsentForm">ConsentForm</see>
        /// is available, otherwise returns <c>false</c>.
        /// </summary>
        public static bool IsConsentFormAvailable()
        {
            IConsentInformationClient client = ClientFactory.ConsentInformationClient();
            return client.IsConsentFormAvailable();
        }
    }
}
