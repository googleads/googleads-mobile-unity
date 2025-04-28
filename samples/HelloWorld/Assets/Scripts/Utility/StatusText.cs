using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GoogleMobileAds.Samples.Utility
{
    /// <summary>
    /// Text element which renders messages from Application.logMessageReceivedThreaded.
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/Utility/StatusText")]
    public class StatusText : Text
    {
        private SynchronizationContext _synchronizationContext;
        private const int MAX_LINES = 25; // Adjust this value as needed
        private List<string> _lines = new List<string>();
        private Regex _colorTagRegex = new Regex(@"<color=[^>]+>|</color>");

        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                supportRichText = true;
                text = string.Empty;
                _synchronizationContext = SynchronizationContext.Current;
                Application.logMessageReceivedThreaded += OnLogMessageReceivedThreaded;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Application.logMessageReceivedThreaded -= OnLogMessageReceivedThreaded;
        }

        private void OnLogMessageReceivedThreaded(string logString, string stackTrace, LogType type)
        {
            _synchronizationContext.Post((sender) =>
            {
                // Safeguard against race conditions from Unity disposed objects.
                if (this == null || !Application.isPlaying)
                {
                    return;
                }

                string color;
                switch (type)
                {
                    case LogType.Warning:
                        color = "yellow";
                        break;
                    case LogType.Error:
                    case LogType.Exception:
                        color = "red";
                        break;
                    default:
                        color = "white";
                        break;
                }

                string message = $"<color={color}>{logString}</color>\n\r";
                _lines.Add(message);
                if (_lines.Count > MAX_LINES)
                {
                    RemoveOldestLines();
                }
                text = string.Join("", _lines);
            }, this);
        }

        /// <summary>
        /// Removes the oldest lines from the list of lines to ensure that the list does not exceed
        /// the maximum number of lines. It also ensures that the formatting of the lines is
        /// preserved.
        /// </summary>
        private void RemoveOldestLines()
        {
            while (_lines.Count > MAX_LINES)
            {
                if (_colorTagRegex.IsMatch(_lines[0]) &&
                    !_colorTagRegex.IsMatch(_lines[_lines.Count - 1]))
                {
                    // If the first line has a color tag, but the last line does not, we need to
                    // remove more lines.
                    int index = 0;
                    bool foundClosingTag = false;
                    for (int i = 0; i < _lines.Count; i++)
                    {
                        if (_lines[i].Contains("</color>"))
                        {
                            index = i;
                            foundClosingTag = true;
                            break;
                        }
                    }
                    if (foundClosingTag)
                    {
                        _lines.RemoveRange(0, index + 1);
                    }
                    else
                    {
                        _lines.Clear();
                    }
                }
                else
                {
                    _lines.RemoveAt(0);
                }
            }
        }
    }
}