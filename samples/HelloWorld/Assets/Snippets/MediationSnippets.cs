using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Snippets
{
    internal class MediationSnippets
    {
        void Initialize()
        {
            // [START initialize_sdk]
            // Initialize the Mobile Ads SDK.
            MobileAds.Initialize((InitializationStatus skdStatus) =>
            {
                Dictionary<string, AdapterStatus> map = skdStatus.getAdapterStatusMap();
                foreach (KeyValuePair<string, AdapterStatus> keyValuePair in map)
                {
                    string className = keyValuePair.Key;
                    AdapterStatus status = keyValuePair.Value;
                    switch (status.InitializationState)
                    {
                        case AdapterState.NotReady:
                            // The adapter initialization did not complete.
                            Debug.Log($"Adapter: {className} is not ready.");
                            break;
                        case AdapterState.Ready:
                            // The adapter was successfully initialized.
                            Debug.Log($"Adapter: {className} is initialized.");
                            break;
                    }
                }
            });
            // [END initialize_sdk]
        }
    }
}