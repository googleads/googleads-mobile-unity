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
import static com.google.common.util.concurrent.MoreExecutors.newDirectExecutorService;
import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import android.app.Activity;
import android.os.Bundle;
import com.google.android.libraries.ads.mobile.sdk.common.AdRequest;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.common.PreloadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.PreloadConfiguration;
import com.google.android.libraries.ads.mobile.sdk.common.ResponseInfo;
import com.google.android.libraries.ads.mobile.sdk.rewardedinterstitial.RewardedInterstitialAd;
import com.google.unity.ads.nextgen.UnityRewardedInterstitialAdPreloader.RewardedInterstitialAdPreloaderWrapper;
import java.util.ArrayList;
import java.util.Map;
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

/** Unit tests for {@link UnityRewardedInterstitialAdPreloader}. */
@RunWith(RobolectricTestRunner.class)
public final class UnityRewardedInterstitialAdPreloaderTest {

  @Rule public final MockitoRule mocks = MockitoJUnit.rule();

  private Activity activity;
  private UnityRewardedInterstitialAdPreloader unityRewardedInterstitialAdPreloader;
  private PreloadConfiguration preloadConfiguration;

  private static final String PRELOAD_ID = "preloadId";

  @Mock private UnityPreloadCallback mockPreloadCallback;
  @Mock private AdRequest mockAdRequest;
  @Mock private RewardedInterstitialAd mockRewardedInterstitialAd;
  @Mock private UnityRewardedInterstitialAdCallback mockRewardedInterstitialAdCallback;
  @Mock private RewardedInterstitialAdPreloaderWrapper mockWrapper;
  @Captor private ArgumentCaptor<PreloadCallback> preloadCallbackCaptor;

  @Before
  public void setUp() {
    activity = Robolectric.buildActivity(Activity.class).create().get();
    unityRewardedInterstitialAdPreloader =
        new UnityRewardedInterstitialAdPreloader(
            activity, mockPreloadCallback, mockWrapper, newDirectExecutorService());
    preloadConfiguration = new PreloadConfiguration(mockAdRequest, 1);
  }

  @Test
  public void testStart() {
    boolean unused = unityRewardedInterstitialAdPreloader.start(PRELOAD_ID, preloadConfiguration);

    verify(mockWrapper)
        .start(eq(PRELOAD_ID), eq(preloadConfiguration), preloadCallbackCaptor.capture());

    PreloadCallback callback = preloadCallbackCaptor.getValue();
    ResponseInfo responseInfo =
        new ResponseInfo("AdapterName", "responseId", new Bundle(), null, new ArrayList<>());

    callback.onAdPreloaded(PRELOAD_ID, responseInfo);
    verify(mockPreloadCallback).onAdPreloaded(PRELOAD_ID, responseInfo);

    LoadAdError loadAdError =
        new LoadAdError(LoadAdError.ErrorCode.INTERNAL_ERROR, "error message", null);
    callback.onAdFailedToPreload(PRELOAD_ID, loadAdError);
    verify(mockPreloadCallback).onAdFailedToPreload(PRELOAD_ID, loadAdError);

    callback.onAdsExhausted(PRELOAD_ID);
    verify(mockPreloadCallback).onAdsExhausted(PRELOAD_ID);
  }

  @Test
  public void testStart_nullCallback() {
    UnityRewardedInterstitialAdPreloader preloader =
        new UnityRewardedInterstitialAdPreloader(
            activity, null, mockWrapper, newDirectExecutorService());

    boolean unused = preloader.start(PRELOAD_ID, preloadConfiguration);

    verify(mockWrapper)
        .start(eq(PRELOAD_ID), eq(preloadConfiguration), preloadCallbackCaptor.capture());

    PreloadCallback callback = preloadCallbackCaptor.getValue();
    ResponseInfo responseInfo =
        new ResponseInfo("AdapterName", "responseId", new Bundle(), null, new ArrayList<>());

    callback.onAdPreloaded(PRELOAD_ID, responseInfo);
    LoadAdError loadAdError =
        new LoadAdError(LoadAdError.ErrorCode.INTERNAL_ERROR, "error message", null);
    callback.onAdFailedToPreload(PRELOAD_ID, loadAdError);

    callback.onAdsExhausted(PRELOAD_ID);
  }

  @Test
  public void testIsAdAvailable() {
    boolean unused = unityRewardedInterstitialAdPreloader.isAdAvailable(PRELOAD_ID);
    verify(mockWrapper).isAdAvailable(PRELOAD_ID);
  }

  @Test
  public void testGetNumAdsAvailable() {
    int unused = unityRewardedInterstitialAdPreloader.getNumAdsAvailable(PRELOAD_ID);
    verify(mockWrapper).getNumAdsAvailable(PRELOAD_ID);
  }

  @Test
  public void testPollAd_adAvailable() {
    when(mockWrapper.pollAd(PRELOAD_ID)).thenReturn(mockRewardedInterstitialAd);

    UnityRewardedInterstitialAd unityRewardedInterstitialAd =
        unityRewardedInterstitialAdPreloader.pollAd(PRELOAD_ID, mockRewardedInterstitialAdCallback);

    assertThat(unityRewardedInterstitialAd).isNotNull();
    assertThat(unityRewardedInterstitialAd.getRewardedInterstitialAd())
        .isEqualTo(mockRewardedInterstitialAd);
  }

  @Test
  public void testPollAd_adNotAvailable() {
    when(mockWrapper.pollAd(PRELOAD_ID)).thenReturn(null);

    UnityRewardedInterstitialAd unityRewardedInterstitialAd =
        unityRewardedInterstitialAdPreloader.pollAd(PRELOAD_ID, mockRewardedInterstitialAdCallback);

    assertThat(unityRewardedInterstitialAd).isNull();
  }

  @Test
  public void testGetConfiguration() {
    PreloadConfiguration unused = unityRewardedInterstitialAdPreloader.getConfiguration(PRELOAD_ID);
    verify(mockWrapper).getConfiguration(PRELOAD_ID);
  }

  @Test
  public void testGetConfigurations() {
    Map<String, PreloadConfiguration> unused =
        unityRewardedInterstitialAdPreloader.getConfigurations();
    verify(mockWrapper).getConfigurations();
  }

  @Test
  public void testDestroy() {
    unityRewardedInterstitialAdPreloader.destroy(PRELOAD_ID);
    verify(mockWrapper).destroy(PRELOAD_ID);
  }

  @Test
  public void testDestroyAll() {
    unityRewardedInterstitialAdPreloader.destroyAll();
    verify(mockWrapper).destroyAll();
  }

  @Test
  public void testPublicConstructor() {
    UnityRewardedInterstitialAdPreloader preloader =
        new UnityRewardedInterstitialAdPreloader(activity, mockPreloadCallback);
    assertThat(preloader).isNotNull();
  }
}
