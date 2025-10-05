using System;

namespace GoogleMobileAds.Common
{
    public class Tracer : ITracer
    {
        private class NoOpTrace : ITrace
        {
            public void Dispose() {}
        }

        private static readonly NoOpTrace _noOpTrace = new NoOpTrace();

        public Tracer(IInsightsEmitter insightsEmitter) {}

        public ITrace StartTrace(string name)
        {
            return _noOpTrace;
        }
    }
}
