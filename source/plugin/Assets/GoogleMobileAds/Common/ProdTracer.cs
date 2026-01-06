using System;
using System.Collections.Concurrent;
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
        /// The name of the trace operation. This is cached on the stack to be retrieved during
        /// `Dispose()` so the final end-of-trace insight can be emitted with the correct operation
        /// name.
        /// </summary>
        public string OperationName;
        public long StartTimeEpochMillis;
    }

    /// <summary>
    /// Implements tracing by emitting insights (CUIs) for the start and end of operations.
    ///
    /// It is safe to reuse the same Tracer instance across multiple threads because the
    /// instance itself is stateless, holding only a reference to the insights emitter.
    /// The actual trace context (the stack of active traces) is managed via the
    /// `ThreadStatic _traceStack' field.
    ///
    /// This design ensures that each thread maintains its own independent trace tree
    /// structure (nested or single-level), preventing conflicts between concurrent
    /// operations on different threads.
    /// </summary>
    public class Tracer : ITracer
    {
        /// `AsyncLocal` was intentionally skipped because it is not supported by the older .NET
        /// platform version currently being used by the Unity plugin.
        [ThreadStatic]
        private static ConcurrentStack<TraceInfo> _traceStack;
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
        internal ConcurrentStack<TraceInfo> GetTraceStack()
        {
            // `ThreadStatic` fields are initialized as null on each new thread.
            // We lazy-initialize the stack to ensure each thread has its
            // own dedicated context for managing nested traces.
            if (_traceStack == null)
            {
                _traceStack = new ConcurrentStack<TraceInfo>();
            }
            return _traceStack;
        }

        /// <summary>
        /// Starts a new trace.
        /// </summary>
        /// <param name="name">The name of trace operation.</param>
        /// <returns>An ITrace instance that should be disposed to end trace.</returns>
        public ITrace StartTrace(string name)
        {
            ConcurrentStack<TraceInfo> traceStack = GetTraceStack();
            TraceInfo latestTrace;

            // If there is already an active trace on this thread's stack,
            // it becomes the parent of the new trace we are starting.
            string parentId = traceStack.TryPeek(out latestTrace) ? latestTrace.Id : null;

            // Standard Unix Epoch calculation for older .NET versions.
            long startTime = (long)DateTime.UtcNow
                .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalMilliseconds;

            TraceInfo traceInfo = new TraceInfo
            {
                Id = Guid.NewGuid().ToString(),
                OperationName = name,
                StartTimeEpochMillis = startTime,
            };

            traceStack.Push(traceInfo);

            _insightsEmitter.Emit(new Insight
            {
                Tracing = new Insight.TracingActivity
                {
                    OperationName = name,
                    Id = traceInfo.Id,
                    ParentId = parentId,
                }
            });

            return new Trace(traceInfo, parentId, this);
        }

        /// <summary>
        /// Represents an active trace. `Dispose()` to end trace and emit insight.
        /// </summary>
        private class Trace : ITrace
        {
            private readonly TraceInfo _traceInfo;
            private readonly string _parentId;
            private readonly Tracer _tracer;
            private int _disposed = 0;

            public Trace(TraceInfo traceInfo, string parentId, Tracer tracer)
            {
                _traceInfo = traceInfo;
                // We capture the parentId at the start of the trace to ensure
                // the end-of-trace insight is consistent with the start-of-trace
                // insight, even if the stack state is modified in between.
                _parentId = parentId;
                _tracer = tracer;
            }

            /// <summary>
            /// Ends the trace, emits an end-of-trace insight, and cleans up the trace stack.
            /// This method is thread-safe and idempotent, ensuring that it can be called
            /// multiple times without causing errors or duplicate insights.
            /// </summary>
            public void Dispose()
            {
                // Ensures thread-safe idempotency for the disposal logic.
                // Unlike 'DevTracer', which delegates state management to
                // 'System.Diagnostics.Activity', this manual implementation requires a guard to
                // prevent redundant insight emissions and stack corruption if `Dispose()` is called
                // multiple times or concurrently.
                if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 1)
                {
                    return;
                }

                ConcurrentStack<TraceInfo> traceStack = _tracer.GetTraceStack();
                TraceInfo topTrace;
                // If LIFO (Last-In, First-Out) order is maintained, verify current trace is top
                // before popping to ensure the trace tree structure remains valid.
                if (traceStack.TryPeek(out topTrace) && topTrace.Id == _traceInfo.Id)
                {
                    TraceInfo tmp;
                    traceStack.TryPop(out tmp);
                }
                // If LIFO order is broken, this branch will handle cleanup.
                else if (!traceStack.IsEmpty)
                {
                    // If top of stack is not current trace, this indicates that traces are not
                    // being disposed in LIFO order (e.g. parent disposed before child), or
                    // traces are being disposed on different threads without context.
                    // The stack is likely corrupted, so we clear it to prevent further issues.
                    TraceInfo traceInfo;
                    while (traceStack.TryPop(out traceInfo));
                }

                // Standard Unix Epoch calculation for older .NET versions.
                long endTime = (long)DateTime.UtcNow
                    .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                    .TotalMilliseconds;

                // Ensure duration is non-negative.
                long duration = Math.Max(0, endTime - _traceInfo.StartTimeEpochMillis);

                _tracer.Emitter.Emit(new Insight
                {
                    Tracing = new Insight.TracingActivity
                    {
                        OperationName = _traceInfo.OperationName,
                        Id = _traceInfo.Id,
                        ParentId = _parentId,
                        DurationMillis = duration,
                        HasEnded = true,
                    }
                });
            }
        }
    }
}
