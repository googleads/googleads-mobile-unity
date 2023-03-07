using System;
using System.Collections.Generic;

namespace GoogleMobileAds.Mediation.Vungle.Api
{
    public class VungleInterstitialMediationExtras : VungleMediationExtras
    {
        public VungleInterstitialMediationExtras() : base() { }

        public override string AndroidMediationExtraBuilderClassName
        {
            get { return "com.google.unity.mediation.vungle.VungleUnityInterstitialExtrasBuilder"; }
        }
    }
}

namespace GoogleMobileAds.Api.Mediation.Vungle
{
    [System.Obsolete("Use `GoogleMobileAds.Mediation.Vungle.Api.VungleInterstitialMediationExtras` instead.")]
    public class VungleInterstitialMediationExtras :
                 GoogleMobileAds.Mediation.Vungle.Api.VungleInterstitialMediationExtras
    {
    }
}
