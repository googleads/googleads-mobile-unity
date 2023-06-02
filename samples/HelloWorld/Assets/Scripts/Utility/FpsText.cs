using System;
using UnityEngine;
using UnityEngine.UI;

namespace GoogleMobileAds.Samples.Utility
{
    /// <summary>
    /// Frames per second text element.
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/Utility/FpsText")]
    public class FpsText : Text
    {
        private void Update()
        {
            if (Application.isPlaying)
            {
                int fps = (int)(1f / Time.unscaledDeltaTime);
                text = string.Format("{0:0.} fps", fps);
            }
        }
    }
}