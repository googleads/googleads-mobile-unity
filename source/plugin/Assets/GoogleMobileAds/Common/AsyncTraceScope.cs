namespace GoogleMobileAds.Common
{
    internal class AsyncTraceScope
    {
        private ITrace _activeTrace;
        private readonly object _lock = new object();

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
        public void Start(ITracer tracer, string name)
        {
            ITrace oldTrace;
            lock (_lock)
            {
                oldTrace = _activeTrace;
                _activeTrace = tracer.BeginAsyncTrace(name);
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
        public bool StartIfIdle(ITracer tracer, string name)
        {
            lock (_lock)
            {
                if (_activeTrace != null)
                {
                    return false;
                }
                _activeTrace = tracer.BeginAsyncTrace(name);
                return true;
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