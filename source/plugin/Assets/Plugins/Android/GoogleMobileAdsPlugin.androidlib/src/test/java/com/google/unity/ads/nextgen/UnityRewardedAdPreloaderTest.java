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
import com.google.android.libraries.ads.mobile.sdk.rewarded.RewardedAd;
import com.google.unity.ads.nextgen.UnityRewardedAdPreloader.RewardedAdPreloaderWrapper;
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

/** Unit tests for {@link UnityRewardedAdPreloader}. */
@RunWith(RobolectricTestRunner.class)
public final class UnityRewardedAdPreloaderTest {

  @Rule public final MockitoRule mocks = MockitoJUnit.rule();

  private Activity activity;
  private UnityRewardedAdPreloader unityRewardedAdPreloader;
  private PreloadConfiguration preloadConfiguration;

  private static final String PRELOAD_ID = "preloadId";

  @Mock private UnityPreloadCallback mockPreloadCallback;
  @Mock private AdRequest mockAdRequest;
  @Mock private RewardedAd mockRewardedAd;
  @Mock private UnityRewardedAdCallback mockRewardedAdCallback;
  @Mock private RewardedAdPreloaderWrapper mockWrapper;
  @Captor private ArgumentCaptor<PreloadCallback> preloadCallbackCaptor;

  @Before
  public void setUp() {
    activity = Robolectric.buildActivity(Activity.class).create().get();
    unityRewardedAdPreloader =
        new UnityRewardedAdPreloader(
            activity, mockPreloadCallback, mockWrapper, newDirectExecutorService());
    preloadConfiguration = new PreloadConfiguration(mockAdRequest, 1);
  }

  @Test
  public void testStart() {
    boolean unused = unityRewardedAdPreloader.start(PRELOAD_ID, preloadConfiguration);

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
    // Create instance with null callback to verify safety
    UnityRewardedAdPreloader preloader =
        new UnityRewardedAdPreloader(activity, null, mockWrapper, newDirectExecutorService());

    boolean unused = preloader.start(PRELOAD_ID, preloadConfiguration);

    verify(mockWrapper)
        .start(eq(PRELOAD_ID), eq(preloadConfiguration), preloadCallbackCaptor.capture());

    PreloadCallback callback = preloadCallbackCaptor.getValue();
    ResponseInfo responseInfo =
        new ResponseInfo("AdapterName", "responseId", new Bundle(), null, new ArrayList<>());

    // Should not interact with any callback, and definitely not crash
    callback.onAdPreloaded(PRELOAD_ID, responseInfo);
  }

  @Test
  public void testIsAdAvailable() {
    boolean unused = unityRewardedAdPreloader.isAdAvailable(PRELOAD_ID);
    verify(mockWrapper).isAdAvailable(PRELOAD_ID);
  }

  @Test
  public void testGetNumAdsAvailable() {
    int unused = unityRewardedAdPreloader.getNumAdsAvailable(PRELOAD_ID);
    verify(mockWrapper).getNumAdsAvailable(PRELOAD_ID);
  }

  @Test
  public void testPollAd_adAvailable() {
    when(mockWrapper.pollAd(PRELOAD_ID)).thenReturn(mockRewardedAd);

    UnityRewardedAd unityRewardedAd =
        unityRewardedAdPreloader.pollAd(PRELOAD_ID, mockRewardedAdCallback);

    assertThat(unityRewardedAd).isNotNull();
    assertThat(unityRewardedAd.getRewardedAd()).isEqualTo(mockRewardedAd);
  }

  @Test
  public void testPollAd_adNotAvailable() {
    when(mockWrapper.pollAd(PRELOAD_ID)).thenReturn(null);

    UnityRewardedAd unityRewardedAd =
        unityRewardedAdPreloader.pollAd(PRELOAD_ID, mockRewardedAdCallback);

    assertThat(unityRewardedAd).isNull();
  }

  @Test
  public void testGetConfiguration() {
    PreloadConfiguration unused = unityRewardedAdPreloader.getConfiguration(PRELOAD_ID);
    verify(mockWrapper).getConfiguration(PRELOAD_ID);
  }

  @Test
  public void testGetConfigurations() {
    Map<String, PreloadConfiguration> unused = unityRewardedAdPreloader.getConfigurations();
    verify(mockWrapper).getConfigurations();
  }

  @Test
  public void testDestroy() {
    unityRewardedAdPreloader.destroy(PRELOAD_ID);
    verify(mockWrapper).destroy(PRELOAD_ID);
  }
}
