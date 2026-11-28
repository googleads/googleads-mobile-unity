/*
 * Copyright (C) 2026 Google LLC
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
import static com.google.common.util.concurrent.MoreExecutors.directExecutor;
import static org.junit.Assert.assertThrows;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.doThrow;
import static org.mockito.Mockito.never;
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
import com.google.android.libraries.ads.mobile.sdk.squeezeback.SqueezebackAd;
import com.google.android.libraries.ads.mobile.sdk.squeezeback.SqueezebackAdEventCallback;
import com.google.android.libraries.ads.mobile.sdk.squeezeback.SqueezebackAdOptions;
import com.google.android.libraries.ads.mobile.sdk.squeezeback.SqueezebackAdRequest;
import java.util.ArrayList;
import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.ArgumentCaptor;
import org.mockito.Captor;
import org.mockito.Mock;
import org.mockito.junit.MockitoJUnit;
import org.mockito.junit.MockitoRule;
import org.robolectric.Robolectric;
import org.robolectric.RobolectricTestRunner;

/** Unit tests for {@link UnitySqueezebackAd} and {@link UnitySqueezebackAdCallback}. */
@RunWith(RobolectricTestRunner.class)
public final class UnitySqueezebackAdTest {

  private static final String AD_UNIT_ID = "ca-app-pub-3940256099942544/2247696110";

  @Rule public final MockitoRule mocks = MockitoJUnit.rule();

  private Activity activity;
  @Mock private UnitySqueezebackAdCallback mockCallback;
  @Mock private SqueezebackAd mockSqueezebackAd;
  @Mock private AdWrapper<SqueezebackAd> mockAdWrapper;
  @Mock private AdLoadCallback<SqueezebackAd> mockAdLoadCallback;

  @Captor private ArgumentCaptor<AdLoadCallback<SqueezebackAd>> adLoadCallbackCaptor;
  @Captor private ArgumentCaptor<SqueezebackAdEventCallback> adEventCallbackCaptor;
  @Captor private ArgumentCaptor<SqueezebackAdOptions> adOptionsCaptor;
  @Captor private ArgumentCaptor<LoadAdError> loadAdErrorCaptor;

  private UnitySqueezebackAd unitySqueezebackAd;
  private SqueezebackAdRequest adRequest;

  @Before
  public void setUp() {
    activity = Robolectric.buildActivity(Activity.class).create().get();
    unitySqueezebackAd =
        new UnitySqueezebackAd(activity, mockCallback, mockAdWrapper, directExecutor());
    adRequest = new SqueezebackAdRequest.Builder(AD_UNIT_ID).build();
  }

  @Test
  public void publicConstructor_createsInstance() {
    UnitySqueezebackAd ad = new UnitySqueezebackAd(activity, mockCallback);

    assertThat(ad).isNotNull();
  }

  @Test
  public void adWrapperForSqueezeback_invalidRequestType_throwsIllegalArgumentException() {
    AdWrapper<SqueezebackAd> wrapper = AdWrapper.forSqueezeback();
    AdRequest genericRequest = new AdRequest.Builder(AD_UNIT_ID).build();

    IllegalArgumentException exception =
        assertThrows(
            IllegalArgumentException.class, () -> wrapper.load(genericRequest, mockAdLoadCallback));

    assertThat(exception).hasMessageThat().contains("SqueezebackAdRequest");
  }

  @Test
  public void load_onAdLoaded_setsEventCallbackAndInvokesCallback() {
    unitySqueezebackAd.load(adRequest);

    verify(mockAdWrapper).load(eq(adRequest), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockSqueezebackAd);

    verify(mockSqueezebackAd).setAdEventCallback(any(SqueezebackAdEventCallback.class));
    verify(mockCallback).onAdLoaded();
  }

  @Test
  public void load_onAdFailedToLoad_invokesCallback() {
    unitySqueezebackAd.load(adRequest);

    verify(mockAdWrapper).load(eq(adRequest), adLoadCallbackCaptor.capture());
    LoadAdError loadAdError =
        new LoadAdError(LoadAdError.ErrorCode.INTERNAL_ERROR, "error message", null);
    adLoadCallbackCaptor.getValue().onAdFailedToLoad(loadAdError);

    verify(mockCallback).onAdFailedToLoad(loadAdError);
    verify(mockCallback, never()).onAdLoaded();
  }

  @Test
  public void load_whenWrapperThrows_invokesOnAdFailedToLoadWithInternalError() {
    doThrow(new RuntimeException("load boom")).when(mockAdWrapper).load(eq(adRequest), any());

    unitySqueezebackAd.load(adRequest);

    verify(mockCallback).onAdFailedToLoad(loadAdErrorCaptor.capture());
    assertThat(loadAdErrorCaptor.getValue().getCode())
        .isEqualTo(LoadAdError.ErrorCode.INTERNAL_ERROR);
    assertThat(loadAdErrorCaptor.getValue().getMessage()).isEqualTo("load boom");
  }

  @Test
  public void adEventCallbacks_allEventsForwarded() {
    unitySqueezebackAd.load(adRequest);
    verify(mockAdWrapper).load(any(), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockSqueezebackAd);

    verify(mockSqueezebackAd).setAdEventCallback(adEventCallbackCaptor.capture());
    SqueezebackAdEventCallback eventCallback = adEventCallbackCaptor.getValue();

    eventCallback.onAdShown();
    verify(mockCallback).onAdShown();

    eventCallback.onAdHidden();
    verify(mockCallback).onAdHidden();

    FullScreenContentError showError =
        new FullScreenContentError(
            FullScreenContentError.ErrorCode.INTERNAL_ERROR, "show failed", null);
    eventCallback.onAdFailedToShow(showError);
    verify(mockCallback).onAdFailedToShow(showError);

    eventCallback.onAdClicked();
    verify(mockCallback).onAdClicked();

    eventCallback.onAdImpression();
    verify(mockCallback).onAdImpression();

    eventCallback.onAdShowedFullScreenContent();
    verify(mockCallback).onAdShowedFullScreenContent();

    eventCallback.onAdDismissedFullScreenContent();
    verify(mockCallback).onAdDismissedFullScreenContent();

    FullScreenContentError fullScreenError =
        new FullScreenContentError(
            FullScreenContentError.ErrorCode.INTERNAL_ERROR, "fullscreen failed", null);
    eventCallback.onAdFailedToShowFullScreenContent(fullScreenError);
    verify(mockCallback).onAdFailedToShowFullScreenContent(fullScreenError);

    PrecisionType precisionType = PrecisionType.PRECISE;
    long valueMicros = 1500000L;
    String currencyCode = "USD";
    eventCallback.onAdPaid(new AdValue(precisionType, valueMicros, currencyCode));
    verify(mockCallback)
        .onPaidEvent(Util.getAdValuePrecisionType(precisionType), valueMicros, currencyCode);
  }

  @Test
  public void show_whenAdNotLoaded_doesNotInvokeSdk() {
    unitySqueezebackAd.show();
    unitySqueezebackAd.show(0xFF1E1F20);

    verify(mockSqueezebackAd, never()).show(any());
    verify(mockSqueezebackAd, never()).show(any(), any());
  }

  @Test
  public void show_defaultOptions_callsSingleArgShow() {
    unitySqueezebackAd.load(adRequest);
    verify(mockAdWrapper).load(any(), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockSqueezebackAd);

    unitySqueezebackAd.show();

    verify(mockSqueezebackAd).show(activity);
    verify(mockSqueezebackAd, never()).show(any(), any());
  }

  @Test
  public void show_withFallbackBackgroundColor_passesOptionsWithColor() {
    unitySqueezebackAd.load(adRequest);
    verify(mockAdWrapper).load(any(), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockSqueezebackAd);

    int expectedColor = 0xFF1E1F20;
    unitySqueezebackAd.show(expectedColor);

    verify(mockSqueezebackAd).show(eq(activity), adOptionsCaptor.capture());
    assertThat(adOptionsCaptor.getValue().getFallbackBackgroundColor()).isEqualTo(expectedColor);
  }

  @Test
  public void hide_whenAdLoaded_callsHide() {
    unitySqueezebackAd.load(adRequest);
    verify(mockAdWrapper).load(any(), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockSqueezebackAd);

    unitySqueezebackAd.hide();

    verify(mockSqueezebackAd).hide();
  }

  @Test
  public void hide_whenAdNotLoaded_doesNotThrow() {
    unitySqueezebackAd.hide();

    verify(mockSqueezebackAd, never()).hide();
  }

  @Test
  public void destroy_whenAdLoaded_destroysAndClearsReference() {
    unitySqueezebackAd.load(adRequest);
    verify(mockAdWrapper).load(any(), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockSqueezebackAd);

    unitySqueezebackAd.destroy();

    verify(mockSqueezebackAd).destroy();
    verify(mockSqueezebackAd, never()).setAdEventCallback(null);

    unitySqueezebackAd.show();
    unitySqueezebackAd.hide();
    verify(mockSqueezebackAd, never()).show(any());
    verify(mockSqueezebackAd, never()).hide();
  }

  @Test
  public void getResponseInfo_whenAdNotLoaded_returnsNull() {
    assertThat(unitySqueezebackAd.getResponseInfo()).isNull();
  }

  @Test
  public void getResponseInfo_whenAdLoaded_returnsResponseInfo() {
    ResponseInfo responseInfo =
        new ResponseInfo("AdapterName", "responseId", new Bundle(), null, new ArrayList<>());
    when(mockSqueezebackAd.getResponseInfo()).thenReturn(responseInfo);

    unitySqueezebackAd.load(adRequest);
    verify(mockAdWrapper).load(any(), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockSqueezebackAd);

    ResponseInfo actualResponseInfo = unitySqueezebackAd.getResponseInfo();

    verify(mockSqueezebackAd).getResponseInfo();
    assertThat(actualResponseInfo).isEqualTo(responseInfo);
  }

  @Test
  public void getAdUnitId_whenAdNotLoaded_returnsNull() {
    assertThat(unitySqueezebackAd.getAdUnitId()).isNull();
  }

  @Test
  public void getAdUnitId_whenAdLoaded_returnsAdUnitId() {
    when(mockSqueezebackAd.getAdUnitId()).thenReturn(AD_UNIT_ID);

    unitySqueezebackAd.load(adRequest);
    verify(mockAdWrapper).load(any(), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockSqueezebackAd);

    String actualAdUnitId = unitySqueezebackAd.getAdUnitId();

    verify(mockSqueezebackAd).getAdUnitId();
    assertThat(actualAdUnitId).isEqualTo(AD_UNIT_ID);
  }
}
