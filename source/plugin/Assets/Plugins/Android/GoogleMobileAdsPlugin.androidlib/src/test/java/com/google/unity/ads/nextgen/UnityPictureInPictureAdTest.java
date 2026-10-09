/*
 * Copyright (C) 2026 Google, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package com.google.unity.ads.nextgen;

import static com.google.common.truth.Truth.assertThat;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.never;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import android.app.Activity;
import android.os.Bundle;
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.AdValue;
import com.google.android.libraries.ads.mobile.sdk.common.FullScreenContentError;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.common.PrecisionType;
import com.google.android.libraries.ads.mobile.sdk.common.ResponseInfo;
import com.google.android.libraries.ads.mobile.sdk.pip.PictureInPictureAd;
import com.google.android.libraries.ads.mobile.sdk.pip.PictureInPictureAdEventCallback;
import com.google.android.libraries.ads.mobile.sdk.pip.PictureInPictureAdOptions;
import com.google.android.libraries.ads.mobile.sdk.pip.PictureInPictureAdPosition;
import com.google.android.libraries.ads.mobile.sdk.pip.PictureInPictureAdRequest;
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

/** Unit tests for {@link UnityPictureInPictureAd} and {@link UnityPictureInPictureAdCallback}. */
@RunWith(RobolectricTestRunner.class)
public final class UnityPictureInPictureAdTest {

  private static final String AD_UNIT_ID = "ca-app-pub-3940256099942544/6300978111";

  @Rule public final MockitoRule mocks = MockitoJUnit.rule();

  private Activity activity;
  @Mock private UnityPictureInPictureAdCallback mockCallback;
  @Mock private PictureInPictureAd mockPipAd;
  @Mock private AdWrapper<PictureInPictureAd> mockAdWrapper;

  @Captor private ArgumentCaptor<AdLoadCallback<PictureInPictureAd>> adLoadCallbackCaptor;
  @Captor private ArgumentCaptor<PictureInPictureAdEventCallback> adEventCallbackCaptor;
  @Captor private ArgumentCaptor<PictureInPictureAdOptions> adOptionsCaptor;

  private UnityPictureInPictureAd unityPictureInPictureAd;
  private PictureInPictureAdRequest adRequest;

  @Before
  public void setUp() {
    activity = Robolectric.buildActivity(Activity.class).create().get();
    unityPictureInPictureAd = new UnityPictureInPictureAd(activity, mockCallback, mockAdWrapper);
    adRequest = new PictureInPictureAdRequest.Builder(AD_UNIT_ID).build();
  }

  @Test
  public void testPublicConstructor_createsInstance() {
    UnityPictureInPictureAd ad = new UnityPictureInPictureAd(activity, mockCallback);
    assertThat(ad).isNotNull();
  }

  @Test
  public void testLoad_onAdLoaded_invokesCallback() {
    unityPictureInPictureAd.load(adRequest);

    verify(mockAdWrapper).load(eq(adRequest), adLoadCallbackCaptor.capture());

    adLoadCallbackCaptor.getValue().onAdLoaded(mockPipAd);
    verify(mockCallback).onAdLoaded();
    verify(mockPipAd).setAdEventCallback(any());
  }

  @Test
  public void testLoad_onAdFailedToLoad_invokesCallback() {
    unityPictureInPictureAd.load(adRequest);

    verify(mockAdWrapper).load(eq(adRequest), adLoadCallbackCaptor.capture());
    LoadAdError loadAdError =
        new LoadAdError(LoadAdError.ErrorCode.INTERNAL_ERROR, "error message", null);
    adLoadCallbackCaptor.getValue().onAdFailedToLoad(loadAdError);

    verify(mockCallback).onAdFailedToLoad(loadAdError);
    verify(mockCallback, never()).onAdLoaded();
  }

  @Test
  public void testAdEventCallbacks_allEventsTriggered() {
    // Load the ad first
    unityPictureInPictureAd.load(adRequest);
    verify(mockAdWrapper).load(any(), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockPipAd);

    verify(mockPipAd).setAdEventCallback(adEventCallbackCaptor.capture());
    PictureInPictureAdEventCallback eventCallback = adEventCallbackCaptor.getValue();

    // Verify onAdShown
    eventCallback.onAdShown();
    verify(mockCallback).onAdShown();

    // Verify onAdHidden
    eventCallback.onAdHidden();
    verify(mockCallback).onAdHidden();

    // Verify onAdClicked
    eventCallback.onAdClicked();
    verify(mockCallback).onAdClicked();

    // Verify onAdImpression
    eventCallback.onAdImpression();
    verify(mockCallback).onAdImpression();

    // Verify onAdShowedFullScreenContent
    eventCallback.onAdShowedFullScreenContent();
    verify(mockCallback).onAdShowedFullScreenContent();

    // Verify onAdDismissedFullScreenContent
    eventCallback.onAdDismissedFullScreenContent();
    verify(mockCallback).onAdDismissedFullScreenContent();

    // Verify onAdFailedToShowFullScreenContent
    FullScreenContentError fullScreenContentError =
        new FullScreenContentError(
            FullScreenContentError.ErrorCode.INTERNAL_ERROR, "error message", null);
    eventCallback.onAdFailedToShowFullScreenContent(fullScreenContentError);
    verify(mockCallback).onAdFailedToShowFullScreenContent(fullScreenContentError);

    // Verify onAdPaid
    PrecisionType precisionType = PrecisionType.PRECISE;
    long valueMicros = 1500000L;
    String currencyCode = "USD";
    eventCallback.onAdPaid(new AdValue(precisionType, valueMicros, currencyCode));
    verify(mockCallback)
        .onPaidEvent(Util.getAdValuePrecisionType(precisionType), valueMicros, currencyCode);
  }

  @Test
  public void testShow_whenAdNotLoaded_doesNotThrow() {
    unityPictureInPictureAd.show(0);
    verify(mockPipAd, never()).show(any(), any());
  }

  @Test
  public void testShow_defaultPosition_passesDefaultOptions() {
    // Load the ad
    unityPictureInPictureAd.load(adRequest);
    verify(mockAdWrapper).load(any(), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockPipAd);

    // Call show(0) for Default
    unityPictureInPictureAd.show(0);

    verify(mockPipAd).show(eq(activity), adOptionsCaptor.capture());
    assertThat(adOptionsCaptor.getValue().getPosition())
        .isEqualTo(PictureInPictureAdPosition.DEFAULT);
  }

  @Test
  public void testShow_explicitPositions_passesCorrectOptions() {
    // Load the ad
    unityPictureInPictureAd.load(adRequest);
    verify(mockAdWrapper).load(any(), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockPipAd);

    // Test position 1 -> BOTTOM_RIGHT
    unityPictureInPictureAd.show(1);
    verify(mockPipAd).show(eq(activity), adOptionsCaptor.capture());
    assertThat(adOptionsCaptor.getValue().getPosition())
        .isEqualTo(PictureInPictureAdPosition.BOTTOM_RIGHT);

    // Test position 2 -> BOTTOM_LEFT
    unityPictureInPictureAd.show(2);
    verify(mockPipAd, Mockito.times(2)).show(eq(activity), adOptionsCaptor.capture());
    assertThat(adOptionsCaptor.getValue().getPosition())
        .isEqualTo(PictureInPictureAdPosition.BOTTOM_LEFT);

    // Test position 3 -> TOP_LEFT
    unityPictureInPictureAd.show(3);
    verify(mockPipAd, Mockito.times(3)).show(eq(activity), adOptionsCaptor.capture());
    assertThat(adOptionsCaptor.getValue().getPosition())
        .isEqualTo(PictureInPictureAdPosition.TOP_LEFT);

    // Test position 4 -> TOP_RIGHT
    unityPictureInPictureAd.show(4);
    verify(mockPipAd, Mockito.times(4)).show(eq(activity), adOptionsCaptor.capture());
    assertThat(adOptionsCaptor.getValue().getPosition())
        .isEqualTo(PictureInPictureAdPosition.TOP_RIGHT);
  }

  @Test
  public void testHide_whenAdLoaded_callsPipAdHide() {
    unityPictureInPictureAd.load(adRequest);
    verify(mockAdWrapper).load(any(), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockPipAd);

    unityPictureInPictureAd.hide();
    verify(mockPipAd).hide();
  }

  @Test
  public void testHide_whenAdNotLoaded_doesNotThrow() {
    unityPictureInPictureAd.hide();
    verify(mockPipAd, never()).hide();
  }

  @Test
  public void testDestroy_whenAdLoaded_destroysAndCleansUp() {
    unityPictureInPictureAd.load(adRequest);
    verify(mockAdWrapper).load(any(), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockPipAd);

    unityPictureInPictureAd.destroy();
    verify(mockPipAd).destroy();

    // Verify subsequent show or hide does not invoke pipAd
    unityPictureInPictureAd.show(0);
    verify(mockPipAd, never()).show(any(), any());
  }

  @Test
  public void testGetAdPosition_whenAdNotLoaded_returnsDefaultZero() {
    assertThat(unityPictureInPictureAd.getAdPosition()).isEqualTo(0);
  }

  @Test
  public void testGetAdPosition_whenAdLoaded_mapsPositionsCorrectly() {
    unityPictureInPictureAd.load(adRequest);
    verify(mockAdWrapper).load(any(), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockPipAd);

    // null -> 0
    when(mockPipAd.getPosition()).thenReturn(null);
    assertThat(unityPictureInPictureAd.getAdPosition()).isEqualTo(0);

    // DEFAULT -> 0
    when(mockPipAd.getPosition()).thenReturn(PictureInPictureAdPosition.DEFAULT);
    assertThat(unityPictureInPictureAd.getAdPosition()).isEqualTo(0);

    // BOTTOM_RIGHT -> 1
    when(mockPipAd.getPosition()).thenReturn(PictureInPictureAdPosition.BOTTOM_RIGHT);
    assertThat(unityPictureInPictureAd.getAdPosition()).isEqualTo(1);

    // BOTTOM_LEFT -> 2
    when(mockPipAd.getPosition()).thenReturn(PictureInPictureAdPosition.BOTTOM_LEFT);
    assertThat(unityPictureInPictureAd.getAdPosition()).isEqualTo(2);

    // TOP_LEFT -> 3
    when(mockPipAd.getPosition()).thenReturn(PictureInPictureAdPosition.TOP_LEFT);
    assertThat(unityPictureInPictureAd.getAdPosition()).isEqualTo(3);

    // TOP_RIGHT -> 4
    when(mockPipAd.getPosition()).thenReturn(PictureInPictureAdPosition.TOP_RIGHT);
    assertThat(unityPictureInPictureAd.getAdPosition()).isEqualTo(4);
  }

  @Test
  public void testGetResponseInfo_whenAdNotLoaded_returnsNull() {
    assertThat(unityPictureInPictureAd.getResponseInfo()).isNull();
  }

  @Test
  public void testGetResponseInfo_whenAdLoaded_returnsResponseInfo() {
    ResponseInfo responseInfo =
        new ResponseInfo("AdapterName", "responseId", new Bundle(), null, new ArrayList<>());
    when(mockPipAd.getResponseInfo()).thenReturn(responseInfo);

    unityPictureInPictureAd.load(adRequest);
    verify(mockAdWrapper).load(any(), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockPipAd);

    ResponseInfo actualResponseInfo = unityPictureInPictureAd.getResponseInfo();
    verify(mockPipAd).getResponseInfo();
    assertThat(actualResponseInfo).isEqualTo(responseInfo);
  }
}
