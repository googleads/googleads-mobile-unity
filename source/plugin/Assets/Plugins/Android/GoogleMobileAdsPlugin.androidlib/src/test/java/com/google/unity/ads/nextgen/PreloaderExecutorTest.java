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
import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.verify;
import static org.robolectric.Robolectric.buildActivity;

import android.app.Activity;
import android.os.Bundle;
import com.google.android.libraries.ads.mobile.sdk.common.AdRequest;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.common.PreloadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.PreloadConfiguration;
import com.google.android.libraries.ads.mobile.sdk.common.ResponseInfo;
import java.lang.reflect.Field;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.atomic.AtomicReference;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.ArgumentCaptor;
import org.robolectric.RobolectricTestRunner;

/** Unit tests for {@link PreloaderExecutor}. */
@RunWith(RobolectricTestRunner.class)
public final class PreloaderExecutorTest {

  private static final String PRELOAD_ID = "preloadId";

  @Test
  public void testGetExecutor_returnsNonNullInstance() {
    ExecutorService executor = PreloaderExecutor.getExecutor();
    assertThat(executor).isNotNull();
  }

  @Test
  public void testGetExecutor_returnsSameInstance() {
    ExecutorService executor1 = PreloaderExecutor.getExecutor();
    ExecutorService executor2 = PreloaderExecutor.getExecutor();
    assertThat(executor1).isSameInstanceAs(executor2);
  }

  @Test
  public void testSharedExecutor_runsTasksForMultipleAdTypes() throws Exception {
    ExecutorService executor = PreloaderExecutor.getExecutor();
    CountDownLatch latch = new CountDownLatch(4);
    List<String> completedTypes = Collections.synchronizedList(new ArrayList<>());

    executor.execute(
        () -> {
          completedTypes.add("Banner");
          latch.countDown();
        });
    executor.execute(
        () -> {
          completedTypes.add("AppOpen");
          latch.countDown();
        });
    executor.execute(
        () -> {
          completedTypes.add("Interstitial");
          latch.countDown();
        });
    executor.execute(
        () -> {
          completedTypes.add("Rewarded");
          latch.countDown();
        });

    assertThat(latch.await(5, TimeUnit.SECONDS)).isTrue();
    assertThat(completedTypes).containsExactly("Banner", "AppOpen", "Interstitial", "Rewarded");
  }

  @Test
  public void testPreloadersUseSharedExecutor() throws Exception {
    Activity activity = buildActivity(Activity.class).create().get();
    UnityPreloadCallback mockCallback = mock(UnityPreloadCallback.class);

    UnityBannerAdPreloader banner = new UnityBannerAdPreloader(activity, mockCallback);
    UnityAppOpenAdPreloader appOpen = new UnityAppOpenAdPreloader(activity, mockCallback);
    UnityInterstitialAdPreloader interstitial =
        new UnityInterstitialAdPreloader(activity, mockCallback);
    UnityRewardedAdPreloader rewarded = new UnityRewardedAdPreloader(activity, mockCallback);

    ExecutorService shared = PreloaderExecutor.getExecutor();
    assertThat(getExecutorField(banner)).isSameInstanceAs(shared);
    assertThat(getExecutorField(appOpen)).isSameInstanceAs(shared);
    assertThat(getExecutorField(interstitial)).isSameInstanceAs(shared);
    assertThat(getExecutorField(rewarded)).isSameInstanceAs(shared);
  }

  @Test
  public void testBannerPreloader_runsCallbackOnSharedExecutor() throws Exception {
    PreloadConfiguration config = new PreloadConfiguration(mock(AdRequest.class), 1);
    UnityBannerAdPreloader.BannerAdPreloaderWrapper mockWrapper =
        mock(UnityBannerAdPreloader.BannerAdPreloaderWrapper.class);
    ArgumentCaptor<PreloadCallback> captor = ArgumentCaptor.forClass(PreloadCallback.class);

    verifyCallbackRunsOnSharedExecutor(
        (activity, testCallback) -> {
          UnityBannerAdPreloader preloader =
              new UnityBannerAdPreloader(
                  activity, testCallback, mockWrapper, PreloaderExecutor.getExecutor());
          boolean unused = preloader.start(PRELOAD_ID, config);
          verify(mockWrapper).start(eq(PRELOAD_ID), eq(config), captor.capture());
          return captor.getValue();
        });
  }

  @Test
  public void testAppOpenPreloader_runsCallbackOnSharedExecutor() throws Exception {
    PreloadConfiguration config = new PreloadConfiguration(mock(AdRequest.class), 1);
    UnityAppOpenAdPreloader.AppOpenAdPreloaderWrapper mockWrapper =
        mock(UnityAppOpenAdPreloader.AppOpenAdPreloaderWrapper.class);
    ArgumentCaptor<PreloadCallback> captor = ArgumentCaptor.forClass(PreloadCallback.class);

    verifyCallbackRunsOnSharedExecutor(
        (activity, testCallback) -> {
          UnityAppOpenAdPreloader preloader =
              new UnityAppOpenAdPreloader(
                  activity, testCallback, mockWrapper, PreloaderExecutor.getExecutor());
          boolean unused = preloader.start(PRELOAD_ID, config);
          verify(mockWrapper).start(eq(PRELOAD_ID), eq(config), captor.capture());
          return captor.getValue();
        });
  }

  @Test
  public void testInterstitialPreloader_runsCallbackOnSharedExecutor() throws Exception {
    PreloadConfiguration config = new PreloadConfiguration(mock(AdRequest.class), 1);
    UnityInterstitialAdPreloader.InterstitialAdPreloaderWrapper mockWrapper =
        mock(UnityInterstitialAdPreloader.InterstitialAdPreloaderWrapper.class);
    ArgumentCaptor<PreloadCallback> captor = ArgumentCaptor.forClass(PreloadCallback.class);

    verifyCallbackRunsOnSharedExecutor(
        (activity, testCallback) -> {
          UnityInterstitialAdPreloader preloader =
              new UnityInterstitialAdPreloader(
                  activity, testCallback, mockWrapper, PreloaderExecutor.getExecutor());
          boolean unused = preloader.start(PRELOAD_ID, config);
          verify(mockWrapper).start(eq(PRELOAD_ID), eq(config), captor.capture());
          return captor.getValue();
        });
  }

  @Test
  public void testRewardedPreloader_runsCallbackOnSharedExecutor() throws Exception {
    PreloadConfiguration config = new PreloadConfiguration(mock(AdRequest.class), 1);
    UnityRewardedAdPreloader.RewardedAdPreloaderWrapper mockWrapper =
        mock(UnityRewardedAdPreloader.RewardedAdPreloaderWrapper.class);
    ArgumentCaptor<PreloadCallback> captor = ArgumentCaptor.forClass(PreloadCallback.class);

    verifyCallbackRunsOnSharedExecutor(
        (activity, testCallback) -> {
          UnityRewardedAdPreloader preloader =
              new UnityRewardedAdPreloader(
                  activity, testCallback, mockWrapper, PreloaderExecutor.getExecutor());
          boolean unused = preloader.start(PRELOAD_ID, config);
          verify(mockWrapper).start(eq(PRELOAD_ID), eq(config), captor.capture());
          return captor.getValue();
        });
  }

  private void verifyCallbackRunsOnSharedExecutor(PreloaderTester tester) throws Exception {
    Activity activity = buildActivity(Activity.class).create().get();
    ResponseInfo responseInfo =
        new ResponseInfo("Adapter", "123", new Bundle(), null, new ArrayList<>());

    AtomicReference<Thread> callbackThread = new AtomicReference<>();
    CountDownLatch latch = new CountDownLatch(1);
    UnityPreloadCallback testCallback =
        new UnityPreloadCallback() {
          @Override
          public void onAdPreloaded(String preloadId, ResponseInfo response) {
            callbackThread.set(Thread.currentThread());
            latch.countDown();
          }

          @Override
          public void onAdsExhausted(String preloadId) {}

          @Override
          public void onAdFailedToPreload(String preloadId, LoadAdError error) {}
        };

    PreloadCallback callback = tester.runStart(activity, testCallback);
    callback.onAdPreloaded(PRELOAD_ID, responseInfo);

    assertThat(latch.await(5, TimeUnit.SECONDS)).isTrue();
    assertThat(callbackThread.get()).isNotEqualTo(Thread.currentThread());
  }

  private interface PreloaderTester {
    PreloadCallback runStart(Activity activity, UnityPreloadCallback callback) throws Exception;
  }

  private static ExecutorService getExecutorField(Object preloader) throws Exception {
    Field field = preloader.getClass().getDeclaredField("service");
    field.setAccessible(true);
    return (ExecutorService) field.get(preloader);
  }
}
