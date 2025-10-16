package com.google.unity.ads.decagon;

import static com.google.common.truth.Truth.assertThat;
import static com.google.common.util.concurrent.MoreExecutors.directExecutor;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import android.app.Activity;
import android.os.Bundle;
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.AdRequest;
import com.google.android.libraries.ads.mobile.sdk.common.AdValue;
import com.google.android.libraries.ads.mobile.sdk.common.FullScreenContentError;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.common.PrecisionType;
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
  @SuppressWarnings("EnumOrdinal")
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

    PrecisionType precisionType = PrecisionType.PRECISE;
    long valueMicros = 1000000L;
    String currencyCode = "USD";
    eventCallback.onAdPaid(new AdValue(precisionType, valueMicros, currencyCode));
    verify(mockCallback).onPaidEvent(precisionType.ordinal(), valueMicros, currencyCode);
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

  @Test
  public void testGetPlacementId_whenAdNotLoaded_returnsZero() {
    assertThat(unityRewardedAd.getPlacementId()).isEqualTo(0);
    verify(mockRewardedAd, Mockito.never()).getPlacementId();
  }

  @Test
  public void testGetPlacementId_returnsPlacementId() {
    unityRewardedAd.load(mockAdRequest);
    // Capture the callback and simulate successful ad load.
    verify(mockAdWrapper).load(Mockito.eq(mockAdRequest), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockRewardedAd);

    // Mock a placement ID to be returned by the underlying ad.
    long placementId = 12345L;
    when(mockRewardedAd.getPlacementId()).thenReturn(placementId);

    // Verify that the placement ID returned is same as the one returned by the underlying ad.
    long result = unityRewardedAd.getPlacementId();
    assertThat(result).isEqualTo(placementId);
  }

  @Test
  public void testSetPlacementId_whenAdNotLoaded_doesNothing() {
    unityRewardedAd.setPlacementId(12345L);
    verify(mockRewardedAd, Mockito.never()).setPlacementId(Mockito.anyLong());
  }

  @Test
  public void testSetPlacementId_setsPlacementId() {
    unityRewardedAd.load(mockAdRequest);
    // Capture the callback and simulate successful ad load.
    verify(mockAdWrapper).load(Mockito.eq(mockAdRequest), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockRewardedAd);

    // Mock a placement ID to be set by the rewarded ad.
    long placementId = 54321L;
    unityRewardedAd.setPlacementId(placementId);

    // Verify that setPlacementId was called on the underlying ad.
    verify(mockRewardedAd).setPlacementId(placementId);
  }
}
