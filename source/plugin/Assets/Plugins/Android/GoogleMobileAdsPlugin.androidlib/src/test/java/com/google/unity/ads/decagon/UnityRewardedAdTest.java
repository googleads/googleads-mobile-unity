package com.google.unity.ads.decagon;

import static com.google.common.truth.Truth.assertThat;
import static com.google.common.util.concurrent.MoreExecutors.directExecutor;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import android.app.Activity;
import android.os.Bundle;
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.AdRequest;
import com.google.android.libraries.ads.mobile.sdk.common.FullScreenContentError;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.common.ResponseInfo;
import com.google.android.libraries.ads.mobile.sdk.rewarded.RewardedAd;
import com.google.android.libraries.ads.mobile.sdk.rewarded.RewardedAdEventCallback;
import java.util.ArrayList;
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

/** Unit tests for {@link UnityRewardedAd}. */
@RunWith(RobolectricTestRunner.class)
public final class UnityRewardedAdTest {

  @Rule public final MockitoRule mocks = MockitoJUnit.rule();

  // TODO(b/446202827): Refactor to use a common base class for all ad tests.
  private Activity activity;
  @Mock private UnityRewardedAdCallback mockCallback;
  @Mock private RewardedAd mockRewardedAd;
  @Mock private AdWrapper<RewardedAd> mockAdWrapper;
  @Mock private AdRequest mockAdRequest;

  @Captor private ArgumentCaptor<AdRequest> adRequestCaptor;
  @Captor private ArgumentCaptor<AdLoadCallback<RewardedAd>> adLoadCallbackCaptor;
  @Captor private ArgumentCaptor<RewardedAdEventCallback> adEventCallbackCaptor;

  private UnityRewardedAd unityRewardedAd;

  @Before
  public void setUp() {
    activity = Robolectric.buildActivity(Activity.class).create().get();
    unityRewardedAd = new UnityRewardedAd(activity, mockCallback, mockAdWrapper, directExecutor());
  }

  @Test
  public void testLoadAd_onAdLoaded_invokesCallback() {
    unityRewardedAd.load(mockAdRequest);

    verify(mockAdWrapper).load(adRequestCaptor.capture(), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockRewardedAd);

    verify(mockCallback).onRewardedAdLoaded();
  }

  @Test
  public void testLoadAd_onAdFailedToLoad_invokesCallback() {
    unityRewardedAd.load(mockAdRequest);

    verify(mockAdWrapper).load(adRequestCaptor.capture(), adLoadCallbackCaptor.capture());
    LoadAdError loadAdError = new LoadAdError(LoadAdError.ErrorCode.INTERNAL_ERROR, "domain", null);
    adLoadCallbackCaptor.getValue().onAdFailedToLoad(loadAdError);

    verify(mockCallback).onRewardedAdFailedToLoad(loadAdError);
  }

  @Test
  public void testShow_whenAdNotLoaded_doesNotThrow() {
    unityRewardedAd.show();
  }

  @Test
  public void testShow_whenAdLoaded_showsAdAndTriggersCallbacks() {
    // Simulate a successful ad load.
    unityRewardedAd.load(mockAdRequest);
    verify(mockAdWrapper).load(adRequestCaptor.capture(), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockRewardedAd);

    // Call show().
    unityRewardedAd.show();

    // Verify the ad is shown and the event callback is set.
    verify(mockRewardedAd).setAdEventCallback(adEventCallbackCaptor.capture());
    verify(mockRewardedAd).show(Mockito.eq(activity), Mockito.any());
    // Verify immersive mode was set on the ad.
    verify(mockRewardedAd).setImmersiveMode(true);

    // Trigger and verify all event callbacks.
    RewardedAdEventCallback eventCallback = adEventCallbackCaptor.getValue();

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

  @Test
  public void testGetResponseInfo_whenAdNotLoaded_returnsNull() {
    assertThat(unityRewardedAd.getResponseInfo()).isNull();
  }

  @Test
  public void testGetResponseInfo_whenAdLoaded_returnsResponseInfo() {
    ResponseInfo responseInfo =
        new ResponseInfo("AdapterName", "responseId", new Bundle(), null, new ArrayList<>());
    when(mockRewardedAd.getResponseInfo()).thenReturn(responseInfo);

    // Simulate a successful ad load.
    unityRewardedAd.load(mockAdRequest);
    verify(mockAdWrapper).load(Mockito.eq(mockAdRequest), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockRewardedAd);

    // Verify that getResponseInfo() was called on the underlying ad and its result is returned.
    ResponseInfo actualResponseInfo = unityRewardedAd.getResponseInfo();
    verify(mockRewardedAd).getResponseInfo();
    assertThat(actualResponseInfo).isEqualTo(responseInfo);
  }
}
