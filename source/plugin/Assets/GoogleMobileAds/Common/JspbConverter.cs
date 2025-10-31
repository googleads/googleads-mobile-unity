using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GoogleMobileAds.Common
{
    /// <summary>
    /// A helper class to serialize objects to JSPB format.
    /// </summary>
    internal static class JspbConverter
    {
        internal static string ToJspb<TPayload>(LoggableRemoteCaptureRequest<TPayload> request)
        {
            var payloads = new List<string>();
            if (request.payloads != null)
            {
                foreach (var payload in request.payloads)
                {
                    if (payload is ExceptionLoggablePayload)
                    {
                        payloads.Add(ToJspb((ExceptionLoggablePayload)(object)payload));
                    }
                    else
                    {
                        Debug.LogError("JspbConverter encountered an unknown payload type: " +
                                         payload.GetType());
                    }
                }
            }
            if (payloads.Count == 0)
            {
                Debug.LogError("No payloads found in the request.");
                return null;
            }
            return string.Format("[[{0}],{1}]",
                                 string.Join(",", payloads.ToArray()),
                                 ToJspb(request.client_ping_metadata));
        }

        // VisibleForTesting
        internal static string ToJspb(ClientPingMetadata metadata)
        {
            return string.Format("[{0}]", metadata.binary_name);
        }

        // VisibleForTesting
        internal static string QuoteString(string s)
        {
            if (s == null) return "null";

            StringBuilder sb = new StringBuilder();
            sb.Append("\"");
            foreach (char c in s)
            {
                switch (c)
                {
                    // Escape quotes and backslashes.
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    // Escape control characters.
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        // Characters within the printable ASCII range (32-126) are appended
                        // directly. Other characters (control characters or outside ASCII) are
                        // escaped as Unicode.
                        int i = (int)c;
                        if (i < 32 || i > 126)
                        {
                            sb.AppendFormat("\\u{0:X4}", i);
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            sb.Append("\"");
            return sb.ToString();
        }

        #region Exceptions handling
        // VisibleForTesting
        internal static string ToJspb(ExceptionLoggablePayload payload)
        {
            if (payload.unity_gma_sdk_exception_message == null) return "[]";

            // unity_gma_sdk_exception_message has field index 35.
            return string.Format("[{{\"35\":{0}}}]",
                                 ToJspb(payload.unity_gma_sdk_exception_message));
        }

        // VisibleForTesting
        internal static string ToJspb(ExceptionReport report)
        {
            return string.Format(
                "[{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18}]",
                QuoteString(report.time_msec),
                report.trapped.ToString().ToLower(),
                QuoteString(report.name),
                QuoteString(report.exception_class),
                QuoteString(report.top_exception),
                QuoteString(report.stacktrace),
                QuoteString(report.stacktrace_hash),
                QuoteString(report.session_id),
                QuoteString(report.app_id),
                QuoteString(report.app_version_name),
                QuoteString(report.platform),
                QuoteString(report.unity_version),
                QuoteString(report.os_version),
                QuoteString(report.device_model),
                QuoteString(report.country),
                report.total_cpu,
                QuoteString(report.total_memory_bytes),
                QuoteString(report.network_type),
                QuoteString(report.orientation));
        }
        #endregion
    }
}
