using System;
using UnityEngine;
using UnityEngine.UI;

namespace GoogleMobileAds.Samples
{
    /// <summary>
    /// Utility for displaying frames per second text.
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/FpsTextController")]
    [RequireComponent(typeof(Text))]
    public class FpsTextController : MonoBehaviour
    {
        private Text _text;

        private float _deltaTime;

        private void Awake()
        {
            _text = GetComponent<Text>();
        }

        private void Update()
        {
            _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
            float fps = 1.0f / _deltaTime;
            _text.text = string.Format("{0:0.} fps", fps);
        }
    }
}
