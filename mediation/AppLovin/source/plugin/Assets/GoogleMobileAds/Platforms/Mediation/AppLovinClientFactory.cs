using UnityEngine;
using System.Collections;

using GoogleMobileAds.Common.Mediation.AppLovin;

namespace GoogleMobileAds.Mediation
{
    public class AppLovinClientFactory
    {
        public static IAppLovinClient AppLovinInstance()
        {
#if UNITY_EDITOR
        // Testing UNITY_EDITOR first because the editor also responds to the currently
        // selected platform.
            return new GoogleMobileAds.Common.Mediation.AppLovin.DummyClient();
#elif UNITY_ANDROID
            return GoogleMobileAds.Android.Mediation.AppLovin.AppLovinClient.Instance;
#elif (UNITY_5 && UNITY_IOS) || UNITY_IPHONE
            return GoogleMobileAds.iOS.Mediation.AppLovin.AppLovinClient.Instance;
#else
            return new GoogleMobileAds.Common.Mediation.AppLovin.DummyClient();
#endif
        }
    }
}
