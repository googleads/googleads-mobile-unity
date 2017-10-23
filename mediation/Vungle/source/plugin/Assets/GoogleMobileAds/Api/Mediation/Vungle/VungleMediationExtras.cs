using System;
using System.Collections.Generic;

namespace GoogleMobileAds.Api.Mediation.Vungle
{
    public abstract class VungleMediationExtras : MediationExtras
    {
        public const string AllPlacementsKey = "all_placements";

        public const string UserIdKey = "user_id";

        public const string SoundEnabledKey = "sound_enabled";

        public VungleMediationExtras() : base() { }

        public override string IOSMediationExtraBuilderClassName
        {
            get { return "VungleExtrasBuilder"; }
        }

        public void SetAllPlacements(string[] allPlacements)
        {
            this.Extras.Add(AllPlacementsKey, String.Join(",", allPlacements));
        }

        public void SetUserId(string userId)
        {
            this.Extras.Add(UserIdKey, userId);
        }

        public void SetSoundEnabled(bool soundEnabled)
        {
            this.Extras.Add(SoundEnabledKey, soundEnabled.ToString());
        }
    }
}
