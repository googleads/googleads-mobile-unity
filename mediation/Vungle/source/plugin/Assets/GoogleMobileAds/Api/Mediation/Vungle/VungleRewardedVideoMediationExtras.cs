using System;
using System.Collections.Generic;

namespace GoogleMobileAds.Api.Mediation.Vungle
{
    public class VungleRewardedVideoMediationExtras : VungleMediationExtras
    {
        public VungleRewardedVideoMediationExtras() : base() { }

        public override string AndroidMediationExtraBuilderClassName
        {
            get { return "com.google.unity.mediation.vungle.VungleUnityRewardedVideoExtrasBuilder"; }
        }
    }
}
