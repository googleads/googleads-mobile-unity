// Copyright 2026 Google LLC

/*
// [START adpub_sdk_android]
package com.adpub.android;

public class AdPubSdk
{
    public static void setHasUserConsent(boolean hasUserConsent);
}
// [END adpub_sdk_android]
*/

/*
// [START adpub_sdk_ios]
@interface AdPubSdk : NSObject
+ (void)setHasUserConsent:(BOOL)hasUserConsent;
@end
// [END adpub_sdk_ios]
*/

/*
// [START iadpub_client]
namespace AdPub.Common
{
    public interface IAdPubClient
    {
        ///<summary>
        /// Sets a flag indicating whether the app has user consent for ads.
        ///</summary>
        void SetHasUserConsent(bool hasUserConsent);
    }
}
// [END iadpub_client]
*/

/*
// [START default_client]
namespace AdPub.Common
{
    public class DefaultClient : IAdPubClient
    {
        public void SetHasUserConsent(bool hasUserConsent)
        {
            Debug.Log("SetHasUserConsent was called.");
        }
    }
}
// [END default_client]
*/

/*
// [START ios_adpub_client]
// Wrap this class in a conditional operator to make sure it only runs on iOS.
#if UNITY_IOS

// Reference InteropServices to include the DLLImportAttribute type.
using System.Runtime.InteropServices;

using AdPub.Common;

namespace AdPub.Platforms.iOS
{
    public class iOSAdPubClient : IAdPubClient
    {
        public void SetHasUserConsent(bool hasUserConsent)
        {
            GADUAdPubSetHasUserConsent(hasUserConsent);
        }

        [DllImport("__Internal")]
        internal static extern void
            GADUAdPubSetHasUserConsent(bool hasUserConsent);
    }
}
#endif
// [END ios_adpub_client]
*/

/*
// [START adpub_client_bridge]
#import <AdPubSDK/AdPubSDK.h>

void GADUAdPubSetHasUserConsent(BOOL hasUserConsent) {
  [AdPubSDK setHasUserConsent:hasUserConsent];
}
// [END adpub_client_bridge]
*/

/*
// [START android_adpub_client]
// Wrap this class in a conditional operator to ensure it runs only on Android.
#if UNITY_ANDROID

// Reference the UnityEngine namespace which contains the JNI Helper classes.
using UnityEngine;

using AdPub.Common;

namespace AdPub.Platforms.Android
{
    public class AndroidAdPubClient : IAdPubClient
    {
        public void SetHasUserConsent(bool hasUserConsent)
        {
             // Make a reference to the com.adpub.AdPubSDK.
            AndroidJavaClass adPubSdk =
                new AndroidJavaClass("com.adpub.AdPubSdk");

            // Call the native setHasUserConsent method of com.adpub.AdPubSDK.
            adPubSdk.CallStatic("setHasUserConsent", hasUserConsent);
        }
    }
}
#endif
// [END android_adpub_client]
*/

/*
// [START adpub_client_factory]
namespace AdPub.Common
{
    public class AdPubClientFactory
    {
        // Return the correct platform client.
        public static IAdPubClient GetClient()
        {
#if   !UNITY_EDITOR && UNITY_ANDROID
            return new AdPub.Platforms.Android.AndroidAdPubClient();
#elif !UNITY_EDITOR && UNITY_IOS
            return new AdPub.Platforms.iOS.iOSAdPubClient();
#else
            // Returned for the Unity Editor and unsupported platforms.
            return new DefaultClient();
#endif
        }
    }
}
// [END adpub_client_factory]
*/

/*
// [START adpub_api]
using AdPub.Common;

namespace AdPub
{
    public class AdPubApi
    {
        private static readonly IAdPubClient client = GetAdPubClient();

        // Returns the correct client for the current runtime platform.
        private static IAdPubClient GetAdPubClient()
        {
            return AdPubClientFactory.GetClient();
        }

        // Sets the user consent using the underlying SDK functionality.
        public static void SetHasUserConsent(bool hasUserConsent)
        {
            client.SetHasUserConsent(hasUserConsent);
        }
    }
}
// [END adpub_api]
*/

/*
// [START adpub_controller]
using UnityEngine;
using AdPub;

public class AdPubController : MonoBehaviour
{
    // TODO: Get consent from the user and update this userConsent field.
    public bool userConsent;

    // Called on startup of the GameObject it's assigned to.
    public void Start()
    {
        // Pass the user consent to AdPub.
        AdPubApi.SetHasUserConsent(userConsent);
    }
}
// [END adpub_controller]
*/
