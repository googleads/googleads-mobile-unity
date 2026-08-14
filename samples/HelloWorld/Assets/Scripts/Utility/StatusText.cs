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

        private ScrollRect _scrollRect;

        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                verticalOverflow = VerticalWrapMode.Overflow;
                maskable = true;
                supportRichText = true;
                text = string.Empty;
                _synchronizationContext = SynchronizationContext.Current;
                _scrollRect = GetComponentInParent<ScrollRect>();
                EnsureMask();
                Application.logMessageReceivedThreaded += OnLogMessageReceivedThreaded;
            }
        }

        private void EnsureMask()
        {
            GameObject target = null;
            if (_scrollRect != null && _scrollRect.viewport != null)
            {
                target = _scrollRect.viewport.gameObject;
            }
            else if (transform.parent != null)
            {
                target = transform.parent.gameObject;
            }

            if (target != null &&
                target.GetComponent<Mask>() == null &&
                target.GetComponent<RectMask2D>() == null)
            {
                target.AddComponent<RectMask2D>();
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

                if (_scrollRect == null)
                {
                    _scrollRect = GetComponentInParent<ScrollRect>();
                    EnsureMask();
                }
                if (_scrollRect != null)
                {
                    Canvas.ForceUpdateCanvases();
                    _scrollRect.verticalNormalizedPosition = 0f;
                }
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