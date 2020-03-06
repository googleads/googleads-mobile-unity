// Empty externs implementation for Android compatibility with unused iOS Externs defined in GoogleMobileAds.iOS namespace.
#include <jni.h>
#include <string>

extern "C" {
    void GADUInitialize(const char *appId) {}

    void GADUInitializeWithCallback(void *mobileAdsClientRef,
                                    void *callback) {}

    const char *GADUGetInitDescription(void *statusRef,
                                       const char *className) {
        return nullptr;
    }

    int GADUGetInitLatency(void *statusRef, const char *className) {
        return 0;
    }

    int GADUGetInitState(void *statusRef, const char *className) {
        return 0;
    }

    const char **GADUGetInitAdapterClasses(void *statusRef) {
        const char **stringArray;

        return stringArray;
    }

    int GADUGetInitNumberOfAdapterClasses(void *statusRef) {
        return 0;
    }

    void GADUSetApplicationVolume(float volume) {}

    void GADUSetApplicationMuted(bool muted) {}

    void GADUSetiOSAppPauseOnBackground(bool pause) {}

    float GADUDeviceScale() {
        return 0.0;
    }

    int GADUDeviceSafeWidth() {
        return 0;
    }

    void *GADUCreateBannerView(void **bannerClient, const char *adUnitID,
                               int width, int height,
                               int adPosition) {
        return nullptr;
    }

    void *GADUCreateBannerViewWithCustomPosition(void **bannerClient,
                                                 const char *adUnitID, int width,
                                                 int height, int x,
                                                 int y) {
        return nullptr;
    }

    void *GADUCreateSmartBannerView(void **bannerClient,
                                    const char *adUnitID, int adPosition) {
        return nullptr;
    }

    void *GADUCreateSmartBannerViewWithCustomPosition(void **bannerClient,
                                                      const char *adUnitID, int x,
                                                      int y) {
        return nullptr;
    }

    void *GADUCreateAnchoredAdaptiveBannerView(void **bannerClient,
                                               const char *adUnitID, int width,
                                               int orientation,
                                               int adPosition) {
        return nullptr;
    }

    void *GADUCreateAnchoredAdaptiveBannerViewWithCustomPosition(
            void **bannerClient, const char *adUnitID, int width,
            int orientation, int x, int y) {
        return nullptr;
    }

    void *GADUCreateInterstitial(void **interstitialClient,
                                 const char *adUnitID) {
        return nullptr;
    }

    void *GADUCreateRewardBasedVideoAd(
            void **rewardBasedVideoAdClient) {
        return nullptr;
    }

    void *GADUCreateRewardedAd(void **rewardedAdClient,
                               const char *adUnitID) {
        return nullptr;

    }

    void *GADUCreateAdLoader(void **adLoaderClient,
                             const char *adUnitID,
                             const char **templateIDs, int templateIDLength,
                             struct AdTypes *types, bool returnUrlsForImageAssets) {
        return nullptr;
    }

    void GADUSetBannerCallbacks(void *banner,
                                void *adReceivedCallback,
                                void *adFailedCallback,
                                void *willPresentCallback,
                                void *didDismissCallback,
                                void *willLeaveCallback,
                                void *paidEventCallback
    ) {}

    void GADUSetInterstitialCallbacks(
            void *interstitial, void *adReceivedCallback,
            void *adFailedCallback,
            void *willPresentCallback,
            void *didDismissCallback,
            void *willLeaveCallback,
            void *paidEventCallback
    ) {}

    void GADUSetRewardBasedVideoAdCallbacks(
            void *rewardBasedVideoAd,
            void *adReceivedCallback,
            void *adFailedCallback,
            void *didOpenCallback,
            void *didStartCallback,
            void *didCloseCallback,
            void *didRewardCallback,
            void *willLeaveCallback,
            void *didCompleteCallback) {}

    void GADUSetRewardedAdCallbacks(
            void *rewardedAd, void *adReceivedCallback,
            void *adFailedToLoadCallback,
            void *adFailedToShowCallback,
            void *didOpenCallback, void *didCloseCallback,
            void *didEarnRewardCallback,
            void *paidEventCallback
    ) {}

    void GADUSetAdLoaderCallbacks(
            void *adLoader,
            void *customTemplateAdReceivedCallback,
            void *adFailedCallback) {}

    void GADUHideBannerView(void *banner) {}

    void GADUShowBannerView(void *banner) {}

    void GADURemoveBannerView(void *banner) {}

    float GADUGetBannerViewHeightInPixels(void *banner) {
        return 0.0;
    }

    float GADUGetBannerViewWidthInPixels(void *banner) {
        return 0.0;
    }

    bool GADUInterstitialReady(void *interstitial) {
        return false;
    }

    void GADUShowInterstitial(void *interstitial) {}

    bool GADURewardBasedVideoAdReady(void *rewardBasedVideo) {
        return false;
    }

    void GADUSetRewardBasedVideoAdUserId(void *rewardBasedVideo,
                                         const char *userId) {}

    void GADUShowRewardBasedVideoAd(void *rewardBasedVideoAd) {}

    bool GADURewardedAdReady(void *rewardedAd) {
        return false;
    }

    void GADUShowRewardedAd(void *rewardedAd) {}

    const char *GADURewardedAdGetRewardType(void *rewardedAd) {
        return nullptr;
    }

    double GADURewardedAdGetRewardAmount(void *rewardedAd) {
        return 0.0;
    }

    void *GADUCreateRequest() {
        return nullptr;
    }

    void GADUAddTestDevice(void *request, const char *deviceID) {}

    void GADUAddKeyword(void *request, const char *keyword) {}

    void GADUSetRequestAgent(void *request, const char *requestAgent) {}

    void GADUSetBirthday(void *request, int year, int month, int day) {}

    void GADUSetGender(void *request, int genderCode) {}

    void GADUTagForChildDirectedTreatment(void *request, bool childDirectedTreatment) {}

    void *GADUCreateServerSideVerificationOptions() {
        return nullptr;
    }

    void GADUServerSideVerificationOptionsSetUserId(void *options,
                                                    const char *userId) {}

    void GADUServerSideVerificationOptionsSetCustomRewardString(
            void *options, const char *customRewardString) {}

    void *GADUCreateMutableDictionary() {
        return nullptr;
    }

    void GADUMutableDictionarySetValue(void *dictionary, const char *key,
                                       const char *value) {}

    void GADUSetMediationExtras(void *request, void *dictionary,
                                const char *adNetworkExtraClassName) {}

    void GADUSetExtra(void *request, const char *key, const char *value) {}

    void GADURequestBannerAd(void *banner, void *request) {}

    void GADUSetBannerViewAdPosition(void *banner, int position) {}

    void GADUSetBannerViewCustomPosition(void *banner, int x, int y) {}

    void GADURequestInterstitial(void *interstitial, void *request) {}

    void GADURequestRewardBasedVideoAd(void *rewardBasedVideoAd,
                                       void *request, const char *adUnitID) {}

    void GADURequestRewardedAd(void *rewardedAd, void *request) {}

    void GADURequestNativeAd(void *adLoader, void *request) {}

    void GADURewardedAdSetServerSideVerificationOptions(
            void *rewardedAd, void *options) {}

    #pragma mark - Native Custom Template Ad methods
    const char *GADUNativeCustomTemplateAdTemplateID(
            void *nativeCustomTemplateAd) {
        return nullptr;
    }

    const char *GADUNativeCustomTemplateAdImageAsBytesForKey(
            void *nativeCustomTemplateAd, const char *key) {
        return nullptr;
    }

    const char *GADUNativeCustomTemplateAdStringForKey(
            void *nativeCustomTemplateAd, const char *key) {
        return nullptr;
    }

    void GADUNativeCustomTemplateAdRecordImpression(
            void *nativeCustomTemplateAd) {}

    void GADUNativeCustomTemplateAdPerformClickOnAssetWithKey(
            void *nativeCustomTemplateAd, const char *key,
            bool customClickAction) {}

    const char **GADUNativeCustomTemplateAdAvailableAssetKeys(
            void *nativeCustomTemplateAd) {
        const char **ptr;
        return ptr;
    }

    int GADUNativeCustomTemplateAdNumberOfAvailableAssetKeys(
            void *nativeCustomTemplateAd) {
        return 0;
    }

    void GADUSetNativeCustomTemplateAdUnityClient(
            void *nativeCustomTemplateAd,
            void *nativeCustomTemplateClient) {}

    void GADUSetNativeCustomTemplateAdCallbacks(
            void *nativeCustomTemplateAd,
            void *adClickedCallback) {}

    #pragma mark - Other methods
    void GADURelease(void *ref) {}

    const char *GADUMediationAdapterClassNameForBannerView(void *bannerView) {
        return nullptr;
    }

    const char *GADUMediationAdapterClassNameForRewardedVideo(void *rewardedVideo) {
        return nullptr;
    }

    const char *GADUMediationAdapterClassNameForRewardedAd(void *rewardedAd) {
        return nullptr;
    }

    const char *GADUMediationAdapterClassNameForInterstitial(void *interstitial) {
        return nullptr;
    }
}
