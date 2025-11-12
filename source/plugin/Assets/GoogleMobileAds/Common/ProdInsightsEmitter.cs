using System;

namespace GoogleMobileAds.Common
{
  public class InsightsEmitter : IInsightsEmitter
  {
      public void Emit(Insight insight)
      {
          CuiHandler.Instance.ReportCui(insight);
      }
  }
}
