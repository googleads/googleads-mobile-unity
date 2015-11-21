#if UNITY_ANDROID

using UnityEngine;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Android
{
    internal class AdListener : AndroidJavaProxy
    {
        private IAdListener listener;
        internal AdListener(IAdListener listener)
            : base(Utils.UnityAdListenerClassName)
        {
            this.listener = listener;
        }

        void onAdLoaded() {
            listener.FireAdLoaded();
        }

        void onAdFailedToLoad(string errorReason) {
            listener.FireAdFailedToLoad(errorReason);
        }

        void onAdOpened() {
            listener.FireAdOpened();
        }

        void onAdClosed() {
            listener.FireAdClosing();
            listener.FireAdClosed();
        }

        void onAdLeftApplication() {
            listener.FireAdLeftApplication();
        }
    }
}

#endif
