using System;
using System.Collections.Generic;

using UnityEngine;

namespace GoogleMobileAds.Api.Mediation
{
    public abstract class MediationExtras
    {
        public Dictionary<string, string> Extras { get; protected set; }

        public MediationExtras()
        {
            this.Extras = new Dictionary<string, string>();
        }

        public abstract string AndroidMediationExtraBuilderClassName { get; }

        public abstract string IOSMediationExtraBuilderClassName { get; }
    }
}
