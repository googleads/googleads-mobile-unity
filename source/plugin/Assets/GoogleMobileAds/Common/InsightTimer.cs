using System;
using System.Diagnostics;
using UnityEngine;

namespace GoogleMobileAds.Common
{
    /// <summary>
    /// A helper class to time an operation and emit an insight with the duration.
    /// </summary>
    public class InsightTimer
    {
        private Insight _insight;
        private Stopwatch _stopwatch;

        /// <summary>
        /// Creates an instance of InsightTimer and starts timing.
        /// </summary>
        /// <param name="insight">The insight to emit when End() is called.</param>
        public InsightTimer(Insight insight)
        {
            _insight = insight;
            if (_insight.Tracing == null)
            {
                _insight.Tracing = new Insight.TracingActivity();
            }
            _insight.Tracing.HasEnded = false;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        /// <summary>
        /// Stops the timer, calculates the duration, and emits the insight.
        /// </summary>
        public void End()
        {
            if (_insight == null || _insight.Tracing == null || _insight.Tracing.HasEnded)
            {
                return;
            }
            _stopwatch.Stop();
            _insight.Tracing.DurationMillis = _stopwatch.ElapsedMilliseconds;
            _insight.Tracing.HasEnded = true;
            InsightsEmitter.Instance.Emit(_insight);
        }
    }
}
