using UnityEngine;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Snippets
{
    /// <summary>
    /// Code snippets used for the developer guides.
    /// </summary>
    internal class ReduceFirstImpressionLatencySnippets : MonoBehaviour
    {
        // [START reduce_first_impression_latency]
        private bool _isInitialAdLoaded = false;

        void InitializeWithInitialAd()
        {
            // Load an ad after 5s if SDK initialization is slow.
            Invoke("LoadInitialAd", 5);

            MobileAds.Initialize((InitializationStatus initstatus) =>
            {
                LoadInitialAd();
            });
        }

        void LoadInitialAd()
        {
            if (_isInitialAdLoaded)
            {
                return;
            }
            _isInitialAdLoaded = true;
            LoadAd();
        }
        // [END reduce_first_impression_latency]

        void LoadAd()
        {
            // TODO: Load an ad.
        }
    }
}
