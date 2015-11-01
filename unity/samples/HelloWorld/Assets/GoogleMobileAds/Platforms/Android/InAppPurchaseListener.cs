#if UNITY_ANDROID

using UnityEngine;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Android
{
    internal class InAppPurchaseListener : AndroidJavaProxy
    {
        private IInAppPurchaseListener listener;
        internal InAppPurchaseListener(IInAppPurchaseListener listener)
            : base(Utils.PlayStorePurchaseListenerClassName)
        {
            this.listener = listener;
        }

        bool isValidPurchase(string sku) {
            return listener.FireIsValidPurchase(sku);
        }

        void onInAppPurchaseFinished(AndroidJavaObject result) {
            InAppPurchaseResult wrappedResult = new InAppPurchaseResult(result);
            if (wrappedResult.IsSuccessful && wrappedResult.IsVerified) {
                listener.FireOnInAppPurchaseFinished(wrappedResult);
            }
            else
            {
                Debug.Log("InAppPurhase Failed!");
            }
        }
    }
}

#endif
