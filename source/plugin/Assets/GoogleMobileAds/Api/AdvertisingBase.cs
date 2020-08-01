using System;
using GoogleMobileAds.Common;
using UnityEngine;

namespace GoogleMobileAds.Api
{
    public abstract class AdvertisingBase
    {
        public abstract void LoadAd(AdRequest request);
        public abstract bool IsLoaded();
        public abstract void Show();
        /// <summary>
        /// Returns the mediation adapter class name.
        /// </summary>
        /// <returns></returns>
        [Obsolete("MediationAdapterClassName() is deprecated, use GetResponseInfo.MediationAdapterClassName() instead.")]
        public abstract string MediationAdapterClassName();
        /// <summary>
        /// Returns ad request response info.
        /// </summary>
        /// <returns></returns>
        public abstract ResponseInfo GetResponseInfo();

        protected void ExecuteEvent<T>(object sender, EventHandler<T> handler, T args) 
        {
            MobileAdsEventExecutor.ExecuteInUpdate(() => 
            {
                handler?.Invoke(sender, args);
                if (MobileAds.IsDebugging && handler != null)
                    Debug.Log($"Event executed: {handler.Method.Name}");
            });
        }
    }
}