using UnityEditor;
using UnityEngine;

using GoogleMobileAds.Common;

namespace GoogleMobileAds.Editor
{
    public class GoogleMobileAdsMenuEntries
    {

        [MenuItem("GameObject/Google Mobile Ads/Banner Ad Placement", false, 11)]
        public static void NewBannerAdPlacement()
        {
            GameObject banner = new GameObject("New Banner Ad Placement");
            banner.AddComponent<BannerAdPlacement>();
        }

        [MenuItem("GameObject/Google Mobile Ads/Interstitial Ad Placement", false, 11)]
        public static void NewInterstitialAdPlacement()
        {
            GameObject banner = new GameObject("New Interstitial Ad Placement");
            banner.AddComponent<InterstitialAdPlacement>();
        }

        [MenuItem("GameObject/Google Mobile Ads/Rewarded Ad Placement", false, 11)]
        public static void NewRewardedAdPlacement()
        {
            GameObject banner = new GameObject("New Rewarded Ad Placement");
            banner.AddComponent<RewardedAdPlacement>();
        }
    }
}
