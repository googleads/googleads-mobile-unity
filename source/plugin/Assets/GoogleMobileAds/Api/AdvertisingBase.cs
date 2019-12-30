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
        public abstract string MediationAdapterClassName();

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