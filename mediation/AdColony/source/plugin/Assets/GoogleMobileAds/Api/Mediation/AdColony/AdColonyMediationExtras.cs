using System;

namespace GoogleMobileAds.Api.Mediation.AdColony
{
    public class AdColonyMediationExtras : MediationExtras
    {
        public const string ShowPrePopupKey = "show_pre_popup";
        public const string ShowPostPopupKey = "show_post_popup";

        public AdColonyMediationExtras() : base() { }

        public override string AndroidMediationExtraBuilderClassName
        {
            get { return "com.google.unity.mediation.adcolony.AdColonyUnityExtrasBuilder"; }
        }

        public override String IOSMediationExtraBuilderClassName
        {
            get { return "AdColonyExtrasBuilder"; }
        }

        public void SetShowPrePopup(bool showPrePopup)
        {
            this.Extras.Add(ShowPrePopupKey, showPrePopup.ToString());
        }

        public void SetShowPostPopup(bool showPostPopup)
        {
            this.Extras.Add(ShowPostPopupKey, showPostPopup.ToString());
        }
    }
}
