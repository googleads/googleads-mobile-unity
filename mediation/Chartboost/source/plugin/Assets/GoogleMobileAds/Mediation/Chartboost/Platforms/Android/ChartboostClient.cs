// Copyright 2018 Google LLC
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

#if UNITY_ANDROID

using UnityEngine;

using GoogleMobileAds.Mediation.Chartboost.Api;
using GoogleMobileAds.Mediation.Chartboost.Common;

namespace GoogleMobileAds.Mediation.Chartboost.Android
{
    public class ChartboostClient : IChartboostClient
    {
        private const string chartboostSDKClass = "com.chartboost.sdk.Chartboost";
        private const string chartboostCustomPrivacyClass =
                "com.chartboost.sdk.privacy.model.Custom";
        private const string chartboostGDPRModelClass = "com.chartboost.sdk.privacy.model.GDPR";
        private const string chartboostGDPRModelEnum =
                "com.chartboost.sdk.privacy.model.GDPR$GDPR_CONSENT";
        private const string chartboostCCPAModelClass = "com.chartboost.sdk.privacy.model.CCPA";
        private const string chartboostCCPAModelEnum =
                "com.chartboost.sdk.privacy.model.CCPA$CCPA_CONSENT";

        private static ChartboostClient instance = new ChartboostClient();
        private ChartboostClient() {}

        public static ChartboostClient Instance
        {
            get
            {
                return instance;
            }
        }

        public void AddDataUseConsent(CBGDPRDataUseConsent gdprConsent)
        {
            AndroidJavaClass chartboostGDPRConsentEnum =
                    new AndroidJavaClass(chartboostGDPRModelEnum);
            AndroidJavaObject gdprConsentObject;
            if (gdprConsent == CBGDPRDataUseConsent.NonBehavioral)
            {
                gdprConsentObject =
                        chartboostGDPRConsentEnum.GetStatic<AndroidJavaObject>("NON_BEHAVIORAL");
            }
            else if (gdprConsent == CBGDPRDataUseConsent.Behavioral)
            {
                gdprConsentObject =
                        chartboostGDPRConsentEnum.GetStatic<AndroidJavaObject>("BEHAVIORAL");
            }
            else
            {
                MonoBehaviour.print("Invalid Chartboost GDPR consent configuration.");
                return;
            }

            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity =
                    unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject dataUseConsent =
                    new AndroidJavaObject(chartboostGDPRModelClass, gdprConsentObject);
            AndroidJavaClass chartboost = new AndroidJavaClass(chartboostSDKClass);
            chartboost.CallStatic("addDataUseConsent", currentActivity, dataUseConsent);
        }

        public void AddDataUseConsent(CBCCPADataUseConsent ccpaConsent)
        {
            AndroidJavaClass chartboostCCPAConsentEnum =
                    new AndroidJavaClass(chartboostCCPAModelEnum);
            AndroidJavaObject ccpaConsentObject;
            if (ccpaConsent == CBCCPADataUseConsent.OptOutSale)
            {
                ccpaConsentObject =
                        chartboostCCPAConsentEnum.GetStatic<AndroidJavaObject>("OPT_OUT_SALE");
            }
            else if (ccpaConsent == CBCCPADataUseConsent.OptInSale)
            {
                ccpaConsentObject =
                        chartboostCCPAConsentEnum.GetStatic<AndroidJavaObject>("OPT_IN_SALE");
            }
            else
            {
                MonoBehaviour.print("Invalid Chartboost CCPA consent configuration.");
                return;
            }

            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity =
                    unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject dataUseConsent =
                    new AndroidJavaObject(chartboostCCPAModelClass, ccpaConsentObject);
            AndroidJavaClass chartboost = new AndroidJavaClass(chartboostSDKClass);
            chartboost.CallStatic("addDataUseConsent", currentActivity, dataUseConsent);
        }

        public void AddDataUseConsent(string customConsentName, string customConsentValue)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity =
                    unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject dataUseConsent =
                    new AndroidJavaObject(chartboostCustomPrivacyClass, customConsentName,
                            customConsentValue);
            AndroidJavaClass chartboost = new AndroidJavaClass(chartboostSDKClass);
            chartboost.CallStatic("addDataUseConsent", currentActivity, dataUseConsent);
        }
    }
}

#endif
