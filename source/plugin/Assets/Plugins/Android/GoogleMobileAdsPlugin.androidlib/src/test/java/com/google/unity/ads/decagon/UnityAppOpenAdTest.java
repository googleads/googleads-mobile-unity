package com.google.unity.ads.decagon;

import static com.google.common.truth.Truth.assertThat;
import static com.google.common.util.concurrent.MoreExecutors.directExecutor;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import android.app.Activity;
import android.os.Bundle;
import com.google.android.libraries.ads.mobile.sdk.appopen.AppOpenAd;
import com.google.android.libraries.ads.mobile.sdk.appopen.AppOpenAdEventCallback;
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.AdRequest;
import com.google.android.libraries.ads.mobile.sdk.common.AdValue;
import com.google.android.libraries.ads.mobile.sdk.common.FullScreenContentError;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.common.PrecisionType;
import com.google.android.libraries.ads.mobile.sdk.common.ResponseInfo;
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

/** Unit tests for {@link UnityAppOpenAd}. */
@RunWith(RobolectricTestRunner.class)
public class UnityAppOpenAdTest {

  @Rule public final MockitoRule mocks = MockitoJUnit.rule();

  private Activity activity;
  @Mock private UnityAppOpenAdCallback mockCallback;
  @Mock private AdRequest mockAdRequest;
  @Mock private AppOpenAd mockAppOpenAd;
  @Mock private AdWrapper<AppOpenAd> mockAdWrapper;

  @Captor private ArgumentCaptor<AdLoadCallback<AppOpenAd>> adLoadCallbackCaptor;
  @Captor private ArgumentCaptor<AppOpenAdEventCallback> adEventCallbackCaptor;

  private UnityAppOpenAd unityAppOpenAd;

  @Before
  public void setUp() {
    activity = Robolectric.buildActivity(Activity.class).create().get();
    unityAppOpenAd = new UnityAppOpenAd(activity, mockCallback, mockAdWrapper, directExecutor());
  }

  @Test
  public void testLoad_onAdLoaded() {
    unityAppOpenAd.load(mockAdRequest);

    // Capture the callback and simulate successful ad load.
    verify(mockAdWrapper).load(Mockito.eq(mockAdRequest), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockAppOpenAd);

    // Verify the callback was invoked. Using timeout because it runs on a separate thread.
    verify(mockCallback).onAppOpenAdLoaded();
  }

  @Test
  public void testLoad_onAdFailedToLoad() {
    unityAppOpenAd.load(mockAdRequest);

    // Capture the callback and simulate failed ad load.
    verify(mockAdWrapper).load(Mockito.eq(mockAdRequest), adLoadCallbackCaptor.capture());
    LoadAdError loadAdError = new LoadAdError(LoadAdError.ErrorCode.INTERNAL_ERROR, "domain", null);
    adLoadCallbackCaptor.getValue().onAdFailedToLoad(loadAdError);

    verify(mockCallback).onAppOpenAdFailedToLoad(loadAdError);
  }

  @Test
  @SuppressWarnings("EnumOrdinal")
  public void testShow_showsAdAndTriggersCallbacks() {
    // First, simulate a successful ad load.
    unityAppOpenAd.load(mockAdRequest);
    verify(mockAdWrapper).load(Mockito.eq(mockAdRequest), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockAppOpenAd);

    // Now, call show().
    unityAppOpenAd.show();

    // Verify the ad is shown and the event callback is set.
    verify(mockAppOpenAd).setAdEventCallback(adEventCallbackCaptor.capture());
    verify(mockAppOpenAd).show(activity);
    verify(mockAppOpenAd).setImmersiveMode(true);

    // Trigger and verify all event callbacks.
    AppOpenAdEventCallback eventCallback = adEventCallbackCaptor.getValue();

    eventCallback.onAdShowedFullScreenContent();
    verify(mockCallback).onAdShowedFullScreenContent();

    FullScreenContentError fullScreenContentError =
        new FullScreenContentError(
            FullScreenContentError.ErrorCode.INTERNAL_ERROR, "error message", null);
    eventCallback.onAdFailedToShowFullScreenContent(fullScreenContentError);
    verify(mockCallback).onAdFailedToShowFullScreenContent(fullScreenContentError);

    eventCallback.onAdImpression();
    verify(mockCallback).onAdImpression();

    eventCallback.onAdClicked();
    verify(mockCallback).onAdClicked();

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
    assertThat(unityAppOpenAd.getResponseInfo()).isNull();
  }

  @Test
  public void testGetResponseInfo_whenAdLoaded_returnsResponseInfo() {
    ResponseInfo responseInfo =
        new ResponseInfo("AdapterName", "responseId", new Bundle(), null, new ArrayList<>());
    when(mockAppOpenAd.getResponseInfo()).thenReturn(responseInfo);

    // Simulate a successful ad load.
    unityAppOpenAd.load(mockAdRequest);
    verify(mockAdWrapper).load(Mockito.eq(mockAdRequest), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockAppOpenAd);

    // Verify that getResponseInfo() was called on the underlying ad and its result is returned.
    ResponseInfo actualResponseInfo = unityAppOpenAd.getResponseInfo();
    verify(mockAppOpenAd).getResponseInfo();
    assertThat(actualResponseInfo).isEqualTo(responseInfo);
  }

  @Test
  public void testGetPlacementId_whenAdNotLoaded_returnsZero() {
    assertThat(unityAppOpenAd.getPlacementId()).isEqualTo(0);
    verify(mockAppOpenAd, Mockito.never()).getPlacementId();
  }

  @Test
  public void testGetPlacementId_returnsPlacementId() {
    unityAppOpenAd.load(mockAdRequest);

    // Capture the callback and simulate successful ad load.
    verify(mockAdWrapper).load(Mockito.eq(mockAdRequest), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockAppOpenAd);

    // Mock a placement ID to be returned by the underlying ad.
    long placementId = 12345L;
    when(mockAppOpenAd.getPlacementId()).thenReturn(placementId);

    // Verify that the placement ID returned is same as the one returned by the underlying ad.
    long result = unityAppOpenAd.getPlacementId();
    assertThat(result).isEqualTo(placementId);
  }

  @Test
  public void testSetPlacementId_whenAdNotLoaded_doesNothing() {
    unityAppOpenAd.setPlacementId(12345L);
    verify(mockAppOpenAd, Mockito.never()).setPlacementId(Mockito.anyLong());
  }

  @Test
  public void testSetPlacementId_setsPlacementId() {
    unityAppOpenAd.load(mockAdRequest);
    // Capture the callback and simulate successful ad load.
    verify(mockAdWrapper).load(Mockito.eq(mockAdRequest), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockAppOpenAd);

    // Mock a placement ID to be set by the app open ad.
    long placementId = 54321L;
    unityAppOpenAd.setPlacementId(placementId);

    // Verify that setPlacementId was called on the underlying ad.
    verify(mockAppOpenAd).setPlacementId(placementId);
  }
}
