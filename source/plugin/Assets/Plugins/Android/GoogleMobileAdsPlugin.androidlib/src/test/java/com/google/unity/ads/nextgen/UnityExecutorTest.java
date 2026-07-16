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
import static java.util.concurrent.TimeUnit.SECONDS;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.doAnswer;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;
import static org.robolectric.Robolectric.buildActivity;

import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import com.google.android.libraries.ads.mobile.sdk.appopen.AppOpenAd;
import com.google.android.libraries.ads.mobile.sdk.banner.AdSize;
import com.google.android.libraries.ads.mobile.sdk.banner.BannerAd;
import com.google.android.libraries.ads.mobile.sdk.banner.BannerAdRequest;
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.AdRequest;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.common.PreloadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.PreloadConfiguration;
import com.google.android.libraries.ads.mobile.sdk.common.ResponseInfo;
import com.google.android.libraries.ads.mobile.sdk.interstitial.InterstitialAd;
import com.google.android.libraries.ads.mobile.sdk.nativead.NativeAd;
import com.google.android.libraries.ads.mobile.sdk.nativead.NativeAdLoaderCallback;
import com.google.android.libraries.ads.mobile.sdk.nativead.NativeAdRequest;
import com.google.android.libraries.ads.mobile.sdk.rewarded.RewardedAd;
import com.google.android.libraries.ads.mobile.sdk.rewardedinterstitial.RewardedInterstitialAd;
import com.google.common.collect.ImmutableList;
import java.lang.reflect.Field;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.Executor;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.atomic.AtomicReference;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.ArgumentCaptor;
import org.robolectric.RobolectricTestRunner;

/** Unit tests for {@link UnityExecutor}. */
@RunWith(RobolectricTestRunner.class)
@SuppressWarnings("unchecked")
public final class UnityExecutorTest {

  private static final String PRELOAD_ID = "preloadId";

  @Test
  public void testGetExecutor_returnsNonNullInstance() {
    ExecutorService executor = UnityExecutor.getExecutor();
    assertThat(executor).isNotNull();
  }

  @Test
  public void testGetExecutor_returnsSameInstance() {
    ExecutorService executor1 = UnityExecutor.getExecutor();
    ExecutorService executor2 = UnityExecutor.getExecutor();
    assertThat(executor1).isSameInstanceAs(executor2);
  }

  @Test
  public void testGetPreloaderExecutor_returnsNonNullInstance() {
    ExecutorService executor = UnityExecutor.getPreloaderExecutor();
    assertThat(executor).isNotNull();
  }

  @Test
  public void testGetPreloaderExecutor_returnsSameInstance() {
    ExecutorService executor1 = UnityExecutor.getPreloaderExecutor();
    ExecutorService executor2 = UnityExecutor.getPreloaderExecutor();
    assertThat(executor1).isSameInstanceAs(executor2);
  }

  @Test
  public void testSharedExecutor_runsTasksForMultipleAdTypes() throws Exception {
    ExecutorService executor = UnityExecutor.getExecutor();
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

    assertThat(latch.await(5, SECONDS)).isTrue();
    assertThat(completedTypes).containsExactly("Banner", "AppOpen", "Interstitial", "Rewarded");
  }

  @Test
  public void testAdClassesUseUnityExecutorByDefault() throws Exception {
    Activity activity = buildActivity(Activity.class).create().get();

    UnityBannerAd banner = new UnityBannerAd(activity, mock(UnityBannerAdCallback.class));
    UnityAppOpenAd appOpen = new UnityAppOpenAd(activity, mock(UnityAppOpenAdCallback.class));
    UnityInterstitialAd interstitial =
        new UnityInterstitialAd(activity, mock(UnityInterstitialAdCallback.class));
    UnityRewardedAd rewarded = new UnityRewardedAd(activity, mock(UnityRewardedAdCallback.class));
    UnityRewardedInterstitialAd rewardedInterstitial =
        new UnityRewardedInterstitialAd(activity, mock(UnityRewardedInterstitialAdCallback.class));
    UnityNativeTemplateAd nativeTemplate =
        new UnityNativeTemplateAd(activity, mock(UnityNativeTemplateAdCallback.class));

    ExecutorService shared = UnityExecutor.getExecutor();
    assertThat(getAdExecutorField(banner)).isSameInstanceAs(shared);
    assertThat(getAdExecutorField(appOpen)).isSameInstanceAs(shared);
    assertThat(getAdExecutorField(interstitial)).isSameInstanceAs(shared);
    assertThat(getAdExecutorField(rewarded)).isSameInstanceAs(shared);
    assertThat(getAdExecutorField(rewardedInterstitial)).isSameInstanceAs(shared);
    assertThat(getAdExecutorField(nativeTemplate)).isSameInstanceAs(shared);
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

    ExecutorService shared = UnityExecutor.getPreloaderExecutor();
    assertThat(getPreloaderExecutorField(banner)).isSameInstanceAs(shared);
    assertThat(getPreloaderExecutorField(appOpen)).isSameInstanceAs(shared);
    assertThat(getPreloaderExecutorField(interstitial)).isSameInstanceAs(shared);
    assertThat(getPreloaderExecutorField(rewarded)).isSameInstanceAs(shared);
  }

  @Test
  public void testBannerAd_runsCallbackOnSharedExecutor() throws Exception {
    AdWrapper<BannerAd> mockWrapper = mock(AdWrapper.class);
    ArgumentCaptor<AdLoadCallback<BannerAd>> captor = ArgumentCaptor.forClass(AdLoadCallback.class);

    verifyCallbackRunsOnSharedExecutor(
        (activity, callbackThread, latch) -> {
          UnityBannerAdCallback testCallback = mock(UnityBannerAdCallback.class);
          doAnswer(
                  invocation -> {
                    callbackThread.set(Thread.currentThread());
                    latch.countDown();
                    return null;
                  })
              .when(testCallback)
              .onAdLoaded();

          BannerAd mockBannerAd = mock(BannerAd.class);
          View fakeView = new View(activity);
          when(mockBannerAd.getView(any(Activity.class))).thenReturn(fakeView);

          UnityBannerAd ad =
              new UnityBannerAd(activity, testCallback, mockWrapper, UnityExecutor.getExecutor());
          ad.load(new BannerAdRequest.Builder("test-ad-unit", AdSize.BANNER).build());
          verify(mockWrapper).load(any(AdRequest.class), captor.capture());
          return () -> captor.getValue().onAdLoaded(mockBannerAd);
        });
  }

  @Test
  public void testAppOpenAd_runsCallbackOnSharedExecutor() throws Exception {
    AdWrapper<AppOpenAd> mockWrapper = mock(AdWrapper.class);
    ArgumentCaptor<AdLoadCallback<AppOpenAd>> captor =
        ArgumentCaptor.forClass(AdLoadCallback.class);

    verifyCallbackRunsOnSharedExecutor(
        (activity, callbackThread, latch) -> {
          UnityAppOpenAdCallback testCallback = mock(UnityAppOpenAdCallback.class);
          doAnswer(
                  invocation -> {
                    callbackThread.set(Thread.currentThread());
                    latch.countDown();
                    return null;
                  })
              .when(testCallback)
              .onAppOpenAdLoaded();

          UnityAppOpenAd ad =
              new UnityAppOpenAd(activity, testCallback, mockWrapper, UnityExecutor.getExecutor());
          ad.load(mock(AdRequest.class));
          verify(mockWrapper).load(any(AdRequest.class), captor.capture());
          return () -> captor.getValue().onAdLoaded(mock(AppOpenAd.class));
        });
  }

  @Test
  public void testInterstitialAd_runsCallbackOnSharedExecutor() throws Exception {
    AdWrapper<InterstitialAd> mockWrapper = mock(AdWrapper.class);
    ArgumentCaptor<AdLoadCallback<InterstitialAd>> captor =
        ArgumentCaptor.forClass(AdLoadCallback.class);

    verifyCallbackRunsOnSharedExecutor(
        (activity, callbackThread, latch) -> {
          UnityInterstitialAdCallback testCallback = mock(UnityInterstitialAdCallback.class);
          doAnswer(
                  invocation -> {
                    callbackThread.set(Thread.currentThread());
                    latch.countDown();
                    return null;
                  })
              .when(testCallback)
              .onInterstitialAdLoaded();

          UnityInterstitialAd ad =
              new UnityInterstitialAd(
                  activity, testCallback, mockWrapper, UnityExecutor.getExecutor());
          ad.load(mock(AdRequest.class));
          verify(mockWrapper).load(any(AdRequest.class), captor.capture());
          return () -> captor.getValue().onAdLoaded(mock(InterstitialAd.class));
        });
  }

  @Test
  public void testRewardedAd_runsCallbackOnSharedExecutor() throws Exception {
    AdWrapper<RewardedAd> mockWrapper = mock(AdWrapper.class);
    ArgumentCaptor<AdLoadCallback<RewardedAd>> captor =
        ArgumentCaptor.forClass(AdLoadCallback.class);

    verifyCallbackRunsOnSharedExecutor(
        (activity, callbackThread, latch) -> {
          UnityRewardedAdCallback testCallback = mock(UnityRewardedAdCallback.class);
          doAnswer(
                  invocation -> {
                    callbackThread.set(Thread.currentThread());
                    latch.countDown();
                    return null;
                  })
              .when(testCallback)
              .onRewardedAdLoaded();

          UnityRewardedAd ad =
              new UnityRewardedAd(activity, testCallback, mockWrapper, UnityExecutor.getExecutor());
          ad.load(mock(AdRequest.class));
          verify(mockWrapper).load(any(AdRequest.class), captor.capture());
          return () -> captor.getValue().onAdLoaded(mock(RewardedAd.class));
        });
  }

  @Test
  public void testRewardedInterstitialAd_runsCallbackOnSharedExecutor() throws Exception {
    AdWrapper<RewardedInterstitialAd> mockWrapper = mock(AdWrapper.class);
    ArgumentCaptor<AdLoadCallback<RewardedInterstitialAd>> captor =
        ArgumentCaptor.forClass(AdLoadCallback.class);

    verifyCallbackRunsOnSharedExecutor(
        (activity, callbackThread, latch) -> {
          UnityRewardedInterstitialAdCallback testCallback =
              mock(UnityRewardedInterstitialAdCallback.class);
          doAnswer(
                  invocation -> {
                    callbackThread.set(Thread.currentThread());
                    latch.countDown();
                    return null;
                  })
              .when(testCallback)
              .onRewardedInterstitialAdLoaded();

          UnityRewardedInterstitialAd ad =
              new UnityRewardedInterstitialAd(
                  activity, testCallback, mockWrapper, UnityExecutor.getExecutor());
          ad.load(mock(AdRequest.class));
          verify(mockWrapper).load(any(AdRequest.class), captor.capture());
          return () -> captor.getValue().onAdLoaded(mock(RewardedInterstitialAd.class));
        });
  }

  @Test
  public void testNativeTemplateAd_runsCallbackOnSharedExecutor() throws Exception {
    UnityNativeTemplateAd.NativeAdLoaderWrapper mockWrapper =
        mock(UnityNativeTemplateAd.NativeAdLoaderWrapper.class);
    ArgumentCaptor<NativeAdLoaderCallback> captor =
        ArgumentCaptor.forClass(NativeAdLoaderCallback.class);

    verifyCallbackRunsOnSharedExecutor(
        (activity, callbackThread, latch) -> {
          UnityNativeTemplateAdCallback testCallback = mock(UnityNativeTemplateAdCallback.class);
          doAnswer(
                  invocation -> {
                    callbackThread.set(Thread.currentThread());
                    latch.countDown();
                    return null;
                  })
              .when(testCallback)
              .onNativeAdLoaded();

          UnityNativeTemplateAd ad =
              new UnityNativeTemplateAd(
                  activity, testCallback, mockWrapper, UnityExecutor.getExecutor());
          ad.loadAd(
              new NativeAdRequest.Builder(
                      "test-ad-unit", ImmutableList.of(NativeAd.NativeAdType.NATIVE))
                  .build());
          verify(mockWrapper).load(any(NativeAdRequest.class), captor.capture());
          return () -> captor.getValue().onNativeAdLoaded(mock(NativeAd.class));
        });
  }

  @Test
  public void testBannerPreloader_runsCallbackOnSharedExecutor() throws Exception {
    PreloadConfiguration config = new PreloadConfiguration(mock(AdRequest.class), 1);
    UnityBannerAdPreloader.BannerAdPreloaderWrapper mockWrapper =
        mock(UnityBannerAdPreloader.BannerAdPreloaderWrapper.class);
    ArgumentCaptor<PreloadCallback> captor = ArgumentCaptor.forClass(PreloadCallback.class);

    verifyPreloaderCallbackRunsOnSharedExecutor(
        (activity, testCallback) -> {
          UnityBannerAdPreloader preloader =
              new UnityBannerAdPreloader(
                  activity, testCallback, mockWrapper, UnityExecutor.getPreloaderExecutor());
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

    verifyPreloaderCallbackRunsOnSharedExecutor(
        (activity, testCallback) -> {
          UnityAppOpenAdPreloader preloader =
              new UnityAppOpenAdPreloader(
                  activity, testCallback, mockWrapper, UnityExecutor.getPreloaderExecutor());
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

    verifyPreloaderCallbackRunsOnSharedExecutor(
        (activity, testCallback) -> {
          UnityInterstitialAdPreloader preloader =
              new UnityInterstitialAdPreloader(
                  activity, testCallback, mockWrapper, UnityExecutor.getPreloaderExecutor());
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

    verifyPreloaderCallbackRunsOnSharedExecutor(
        (activity, testCallback) -> {
          UnityRewardedAdPreloader preloader =
              new UnityRewardedAdPreloader(
                  activity, testCallback, mockWrapper, UnityExecutor.getPreloaderExecutor());
          boolean unused = preloader.start(PRELOAD_ID, config);
          verify(mockWrapper).start(eq(PRELOAD_ID), eq(config), captor.capture());
          return captor.getValue();
        });
  }

  private void verifyCallbackRunsOnSharedExecutor(AdTester tester) throws Exception {
    Activity activity = buildActivity(Activity.class).create().get();

    AtomicReference<Thread> callbackThread = new AtomicReference<>();
    CountDownLatch latch = new CountDownLatch(1);

    Runnable trigger = tester.setupAdAndGetTrigger(activity, callbackThread, latch);
    trigger.run();

    assertThat(latch.await(5, SECONDS)).isTrue();
    assertThat(callbackThread.get().getName()).isEqualTo("GMAUnityAdEventsThread");
  }

  private void verifyPreloaderCallbackRunsOnSharedExecutor(PreloaderTester tester)
      throws Exception {
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

    assertThat(latch.await(5, SECONDS)).isTrue();
    assertThat(callbackThread.get().getName()).isEqualTo("GMAUnityPreloaderThread");
  }

  private interface AdTester {
    Runnable setupAdAndGetTrigger(
        Activity activity, AtomicReference<Thread> callbackThread, CountDownLatch latch)
        throws Exception;
  }

  private interface PreloaderTester {
    PreloadCallback runStart(Activity activity, UnityPreloadCallback callback) throws Exception;
  }

  private static Executor getAdExecutorField(Object ad) throws Exception {
    Field field;
    try {
      field = ad.getClass().getDeclaredField("executor");
    } catch (NoSuchFieldException e) {
      field = ad.getClass().getSuperclass().getDeclaredField("executor");
    }
    field.setAccessible(true);
    return (Executor) field.get(ad);
  }

  private static ExecutorService getPreloaderExecutorField(Object preloader) throws Exception {
    Field field = preloader.getClass().getDeclaredField("service");
    field.setAccessible(true);
    return (ExecutorService) field.get(preloader);
  }
}
