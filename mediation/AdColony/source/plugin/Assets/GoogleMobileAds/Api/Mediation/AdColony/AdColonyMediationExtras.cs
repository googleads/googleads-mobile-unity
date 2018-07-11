using System;

namespace GoogleMobileAds.Api.Mediation.AdColony
{
    public class AdColonyMediationExtras : MediationExtras
    {
        public const string ZoneIdKey = "zone_id";
        public const string UserIdKey = "user_id";
        public const string ShowPrePopupKey = "show_pre_popup";
        public const string ShowPostPopupKey = "show_post_popup";
        public const string TestModeKey = "test_mode";
        public const string GDPRRequiredKey = "gdpr_required";
        public const string GDPRConsentString = "gdpr_consent_string";

        public AdColonyMediationExtras() : base() { }

        public override string AndroidMediationExtraBuilderClassName
        {
            get { return "com.google.unity.mediation.adcolony.AdColonyUnityExtrasBuilder"; }
        }

        public override String IOSMediationExtraBuilderClassName
        {
            get { return "AdColonyExtrasBuilder"; }
        }

        public void SetZoneId(string zoneId)
        {
            this.Extras.Add(ZoneIdKey, zoneId);
        }

        public void SetUserId(string userId)
        {
            this.Extras.Add(UserIdKey, userId);
        }

        public void SetShowPrePopup(bool showPrePopup)
        {
            this.Extras.Add(ShowPrePopupKey, showPrePopup.ToString());
        }

        public void SetShowPostPopup(bool showPostPopup)
        {
            this.Extras.Add(ShowPostPopupKey, showPostPopup.ToString());
        }

        public void SetTestMode(bool testMode)
        {
            this.Extras.Add(TestModeKey, testMode.ToString());
        }

        public void SetGDPRRequired(bool gdprRequired)
        {
            this.Extras.Add(GDPRRequiredKey, gdprRequired.ToString());
        }

        public void SetGDPRConsentString(string gdprConsentString)
        {
            this.Extras.Add(GDPRConsentString, gdprConsentString);
        }
    }
}
