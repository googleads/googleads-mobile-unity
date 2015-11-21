#if UNITY_ANDROID

using UnityEngine;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Android
{
    internal class InAppPurchaseResult : IInAppPurchaseResult
    {
        private UnityEngine.AndroidJavaObject result;
        public InAppPurchaseResult(AndroidJavaObject result)
        {
            this.result = result;
        }

        public void FinishPurchase() {
            result.Call("finishPurchase");
        }

        public string ProductId {
            get { return result.Call<string>("getProductId"); }
        }

        public bool IsSuccessful {
            get {
                AndroidJavaObject pluginUtils = new AndroidJavaObject(Utils.PluginUtilsClassName);
                return pluginUtils.CallStatic<bool>("isResultSuccess", result);
            }
        }

        public bool IsVerified {
            get { return result.Call<bool>("isVerified"); }
        }
    }
}

#endif
