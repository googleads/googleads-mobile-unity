using System;

namespace GoogleMobileAds.Common
{
  public interface IInsightsEmitter
  {
      // TODO: b/431227569 - Make it not a string but a proper object, probably a proto.
      void Emit(string insight);
  }
}
