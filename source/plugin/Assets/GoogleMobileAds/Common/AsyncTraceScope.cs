using System;

namespace GoogleMobileAds.Common
{
    internal class AsyncTraceScope
    {
        private ITrace _activeTrace;
        private readonly ITracer _tracer;
        private readonly object _lock = new object();

        public AsyncTraceScope(ITracer tracer)
        {
            _tracer = tracer;
        }


        /// <summary>
        /// Returns true if a trace is currently being managed.
        /// </summary>
        public bool IsActive
        {
            get
            {
                lock (_lock)
                {
                    return _activeTrace != null;
                }
            }
        }

        /// <summary>
        /// Starts a new trace, replacing any existing one.
        /// </summary>
        public void Start(string name)
        {
            ITrace oldTrace;
            lock (_lock)
            {
                oldTrace = _activeTrace;
                _activeTrace = _tracer.StartAsyncTrace(name);
            }

            if (oldTrace != null)
            {
                oldTrace.Dispose();
            }
        }

        /// <summary>
        /// Starts a new trace only if the scope is currently idle.
        /// This avoids the overhead of creating a trace if one is already active.
        /// Helpful for operations like `MobileAdsClient.Initialize`.
        /// </summary>
        public void StartTraceIfInactive(string name)
        {
            lock (_lock)
            {
                if (_activeTrace != null)
                {
                    return;
                }
                _activeTrace = _tracer.StartAsyncTrace(name);
            }
        }

        /// <summary>
        /// Completes and disposes the current trace.
        /// </summary>
        public void Complete()
        {
            ITrace trace;
            lock (_lock)
            {
                trace = _activeTrace;
                _activeTrace = null;
            }

            if (trace != null)
            {
                trace.Dispose();
            }
        }
    }
}