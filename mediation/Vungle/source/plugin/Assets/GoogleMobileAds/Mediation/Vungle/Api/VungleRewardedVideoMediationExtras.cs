using System;
using System.Collections.Generic;

namespace GoogleMobileAds.Mediation.Vungle.Api
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


namespace GoogleMobileAds.Api.Mediation.Vungle
{
    [System.Obsolete("Use `GoogleMobileAds.Mediation.Vungle.Api.VungleRewardedVideoMediationExtras` instead.")]
    public class VungleRewardedVideoMediationExtras :
                 GoogleMobileAds.Mediation.Vungle.Api.VungleRewardedVideoMediationExtras
    {
    }
}
