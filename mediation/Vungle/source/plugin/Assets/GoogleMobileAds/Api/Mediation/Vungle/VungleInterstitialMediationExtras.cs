using System;
using System.Collections.Generic;

namespace GoogleMobileAds.Api.Mediation.Vungle
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
