using System;
using System.Collections.Generic;
using System.Threading;

namespace GoogleMobileAds.Common
{
    /// <summary>
    /// Serializable class to hold trace information.
    /// </summary>
    [Serializable]
    internal class TraceInfo
    {
        public string Id;
        /// <summary>
        /// The name of the trace operation. This is stored on the stack to be retrieved during
        /// `Dispose()` so the final end-of-trace insight can be emitted with the correct operation
        /// name.
        /// </summary>
        public string OperationName;
        public long StartTimeEpochMillis;
    }

    /// <summary>
    /// Implements tracing by emitting insights (CUIs). It is safe to reuse the same `Tracer`
    /// instance across multiple threads because the instance itself is stateless, holding only a
    /// reference to the insights emitter. The actual trace context (the stack of active traces) is
    /// managed via the `ThreadStatic _traceStack' field.
    ///
    /// This design ensures that each thread maintains its own independent trace tree structure
    /// (nested or single-level), preventing conflicts between concurrent operations on different
    /// threads.
    /// </summary>
    public class Tracer : ITracer
    {
        // `AsyncLocal` was intentionally skipped because we need to support .NET Framework 3.5+.
        [ThreadStatic]
        private static Stack<TraceInfo> _traceStack;
        private readonly IInsightsEmitter _insightsEmitter;

        public Tracer(IInsightsEmitter insightsEmitter)
        {
            _insightsEmitter = insightsEmitter;
        }

        internal IInsightsEmitter Emitter
        {
            get { return _insightsEmitter; }
        }

        /// <summary>
        /// Retrieves the trace stack for the current thread.
        /// </summary>
        internal Stack<TraceInfo> GetTraceStack()
        {
            // We lazy-initialize the stack as a singleton to ensure each thread has its
            // own dedicated context for managing traces.
            if (_traceStack == null)
            {
                _traceStack = new Stack<TraceInfo>();
            }
            return _traceStack;
        }

        /// <returns>A Trace instance.</returns>
        public ITrace StartTrace(string name)
        {
            return StartTraceInternal(name, isSynchronous: true);
        }

        public ITrace StartAsyncTrace(string name)
        {
            return StartTraceInternal(name, isSynchronous: false);
        }

        private ITrace StartTraceInternal(string name, bool isSynchronous)
        {
            Stack<TraceInfo> traceStack = GetTraceStack();

            // If there is already an active trace on this thread's stack,
            // it becomes the parent of the new trace we are starting.
            string parentId = traceStack.Count > 0 ? traceStack.Peek().Id : null;

            long startTime = (long)DateTime.UtcNow
                .Subtract(Insight.UnixEpoch)
                .TotalMilliseconds;

            TraceInfo traceInfo = new TraceInfo
            {
                Id = Guid.NewGuid().ToString(),
                OperationName = name,
                StartTimeEpochMillis = startTime,
            };

            if (isSynchronous)
            {
                traceStack.Push(traceInfo);
            }

            _insightsEmitter.Emit(new Insight
            {
                Tracing = new Insight.TracingActivity
                {
                    Id = traceInfo.Id,
                    OperationName = name,
                    ParentId = parentId,
                }
            });

            return new Trace(traceInfo, parentId, this, isSynchronous);
        }

        /// <summary>
        /// Represents an active trace.
        /// </summary>
        private class Trace : ITrace
        {
            private readonly TraceInfo _traceInfo;
            private readonly string _parentId;
            private readonly Tracer _tracer;
            private readonly bool _isSynchronous;
            private bool _isDisposed = false;
            private readonly object _disposeLock = new object();

            public Trace(TraceInfo traceInfo, string parentId, Tracer tracer, bool isSynchronous)
            {
                _traceInfo = traceInfo;
                // We capture the parentId on init to ensure the end-of-trace insight is consistent
                // with the start-of-trace insight, even if the stack state is modified in between.
                _parentId = parentId;
                _tracer = tracer;
                _isSynchronous = isSynchronous;
            }

            /// <summary>
            /// Ends the trace, emits an end-of-trace insight, and cleans up the trace stack.
            /// This method is thread-safe and idempotent, ensuring that it can be called
            /// multiple times without causing errors or duplicate insights.
            /// </summary>
            public void Dispose()
            {
                // Ensures thread-safe idempotency for the disposal logic.
                lock (_disposeLock)
                {
                    if (_isDisposed) return;
                    _isDisposed = true;
                }

                long endTime = (long)DateTime.UtcNow
                    .Subtract(Insight.UnixEpoch)
                    .TotalMilliseconds;

                // Ensure duration is non-negative.
                long duration = Math.Max(0, endTime - _traceInfo.StartTimeEpochMillis);

                _tracer.Emitter.Emit(new Insight
                {
                    Tracing = new Insight.TracingActivity
                    {
                        Id = _traceInfo.Id,
                        OperationName = _traceInfo.OperationName,
                        ParentId = _parentId,
                        DurationMillis = duration,
                        HasEnded = true,
                    }
                });
                if (!_isSynchronous)
                {
                    // Because `ThreadStatic` stacks are local to a specific thread, they cannot
                    // track state for asynchronous operations that migrate between threads.
                    // `AsyncTraceScope` holds a direct reference to an `ITrace` handle,
                    // which keeps track of the start time.
                    return;
                }
                // For synchronous traces, manage the trace stack.
                // If the current trace is at the top of the stack, pop it. Otherwise, the stack is
                // in a corrupted state, so we clear it to prevent further issues.
                Stack<TraceInfo> traceStack = _tracer.GetTraceStack();
                if (traceStack.Count > 0)
                {
                    if (traceStack.Peek().Id == _traceInfo.Id)
                    {
                        traceStack.Pop();
                    }
                    else
                    {
                        traceStack.Clear();
                    }
                }
            }
        }
    }
}
