package com.google.unity.ads.decagon;

import static com.google.common.util.concurrent.MoreExecutors.directExecutor;
import static org.mockito.Mockito.verify;

import android.app.Activity;
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.AdRequest;
import com.google.android.libraries.ads.mobile.sdk.common.FullScreenContentError;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.rewardedinterstitial.RewardedInterstitialAd;
import com.google.android.libraries.ads.mobile.sdk.rewardedinterstitial.RewardedInterstitialAdEventCallback;
import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.ArgumentCaptor;
import org.mockito.Captor;
import org.mockito.Mock;
import org.mockito.Mockito;
import org.mockito.junit.MockitoJUnit;
import org.mockito.junit.MockitoRule;
import org.robolectric.Robolectric;
import org.robolectric.RobolectricTestRunner;

/** Unit tests for {@link UnityRewardedInterstitialAd}. */
@RunWith(RobolectricTestRunner.class)
public final class UnityRewardedInterstitialAdTest {

  @Rule public final MockitoRule mocks = MockitoJUnit.rule();

  private Activity activity;
  @Mock private UnityRewardedInterstitialAdCallback mockCallback;
  @Mock private RewardedInterstitialAd mockRewardedAd;
  @Mock private AdWrapper<RewardedInterstitialAd> mockAdWrapper;
  @Mock private AdRequest mockAdRequest;

  @Captor private ArgumentCaptor<AdRequest> adRequestCaptor;
  @Captor private ArgumentCaptor<AdLoadCallback<RewardedInterstitialAd>> adLoadCallbackCaptor;
  @Captor private ArgumentCaptor<RewardedInterstitialAdEventCallback> adEventCallbackCaptor;

  private UnityRewardedInterstitialAd unityRewardedInterstitialAd;

  @Before
  public void setUp() {
    activity = Robolectric.buildActivity(Activity.class).create().get();
    unityRewardedInterstitialAd =
        new UnityRewardedInterstitialAd(activity, mockCallback, mockAdWrapper, directExecutor());
  }

  @Test
  public void testLoadAd_onAdLoaded_invokesCallback() {
    unityRewardedInterstitialAd.load(mockAdRequest);

    verify(mockAdWrapper).load(adRequestCaptor.capture(), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockRewardedAd);

    verify(mockCallback).onRewardedInterstitialAdLoaded();
  }

  @Test
  public void testLoadAd_onAdFailedToLoad_invokesCallback() {
    unityRewardedInterstitialAd.load(mockAdRequest);

    verify(mockAdWrapper).load(adRequestCaptor.capture(), adLoadCallbackCaptor.capture());
    LoadAdError loadAdError = new LoadAdError(LoadAdError.ErrorCode.INTERNAL_ERROR, "domain", null);
    adLoadCallbackCaptor.getValue().onAdFailedToLoad(loadAdError);

    verify(mockCallback).onRewardedInterstitialAdFailedToLoad(loadAdError);
  }

  @Test
  public void testShow_whenAdNotLoaded_doesNotThrow() {
    unityRewardedInterstitialAd.show();
  }

  @Test
  public void testShow_whenAdLoaded_showsAdAndTriggersCallbacks() {
    // Simulate a successful ad load.
    unityRewardedInterstitialAd.load(mockAdRequest);
    verify(mockAdWrapper).load(adRequestCaptor.capture(), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockRewardedAd);

    // Call show().
    unityRewardedInterstitialAd.show();

    // Verify the ad is shown and the event callback is set.
    verify(mockRewardedAd).setAdEventCallback(adEventCallbackCaptor.capture());
    verify(mockRewardedAd).show(Mockito.eq(activity), Mockito.any());
    // Verify immersive mode was set on the ad.
    verify(mockRewardedAd).setImmersiveMode(true);

    // Trigger and verify all event callbacks.
    RewardedInterstitialAdEventCallback eventCallback = adEventCallbackCaptor.getValue();

    eventCallback.onAdShowedFullScreenContent();
    verify(mockCallback).onAdShowedFullScreenContent();

    FullScreenContentError fullScreenContentError =
        new FullScreenContentError(
            FullScreenContentError.ErrorCode.INTERNAL_ERROR, "error message", null);
    eventCallback.onAdFailedToShowFullScreenContent(fullScreenContentError);
    verify(mockCallback).onAdFailedToShowFullScreenContent(fullScreenContentError);

    eventCallback.onAdDismissedFullScreenContent();
    verify(mockCallback).onAdDismissedFullScreenContent();
  }
}
