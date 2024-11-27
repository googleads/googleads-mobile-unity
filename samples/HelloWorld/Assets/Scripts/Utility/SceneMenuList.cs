using System;
using UnityEngine;

namespace GoogleMobileAds.Samples.Utility
{
    /// <summary>
    /// Instantiates a list of Scene menu buttons.
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/Utility/SceneMenuList")]
    public class SceneMenuList : MonoBehaviour
    {
        // Data class for configuring the scene list menu.
        private struct SceneInfo
        {
            public string Text;
            public string SceneToLoad;
            public SceneInfo(string text, string sceneToLoad)
            {
                Text = text;
                SceneToLoad = sceneToLoad;
            }
        }

        // SceneMenuList Configurations.
        private readonly SceneInfo[] Scenes = new SceneInfo[]
        {
            new SceneInfo("Banner View", "GoogleBannerViewScene"),
            new SceneInfo("Interstitial Ad", "GoogleInterstitialAdScene"),
            new SceneInfo("Rewarded Ad", "GoogleRewardedAdScene"),
            new SceneInfo("Rewarded Interstital Ad", "GoogleRewardedInterstitialAdScene"),
            new SceneInfo("App Open Ad", "GoogleAppOpenAdScene"),
            new SceneInfo("Native Overlay Ad", "GoogleNativeOverlayAdScene"),
        };

        [Tooltip("Prefab for the SceneMenuButton.")]
        public GameObject ButtonPrefab;

        [Tooltip("Parent transform for the instantiated menu.")]
        public Transform MenuContainer;

        protected void Awake()
        {
            for (int i = Scenes.Length - 1; i >= 0 ; i--)
            {
                var info = Scenes[i];
                var instance = Instantiate(ButtonPrefab, MenuContainer);
                var button = instance.GetComponent<SceneMenuButton>();
                button.SceneToLoadName = info.SceneToLoad;
                button.Label.text = info.Text;
                button.transform.SetAsFirstSibling();
            }
        }
    }
}