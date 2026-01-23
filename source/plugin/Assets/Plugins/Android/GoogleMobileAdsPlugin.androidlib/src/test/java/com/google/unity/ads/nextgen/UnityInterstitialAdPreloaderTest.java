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
import com.google.android.libraries.ads.mobile.sdk.interstitial.InterstitialAd;
import com.google.unity.ads.nextgen.UnityInterstitialAdPreloader.InterstitialAdPreloaderWrapper;
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

/** Unit tests for {@link UnityInterstitialAdPreloader}. */
@RunWith(RobolectricTestRunner.class)
public final class UnityInterstitialAdPreloaderTest {

  @Rule public final MockitoRule mocks = MockitoJUnit.rule();

  private Activity activity;
  private UnityInterstitialAdPreloader unityInterstitialAdPreloader;
  private PreloadConfiguration preloadConfiguration;

  private static final String PRELOAD_ID = "preloadId";

  @Mock private UnityPreloadCallback mockPreloadCallback;
  @Mock private AdRequest mockAdRequest;
  @Mock private InterstitialAd mockInterstitialAd;
  @Mock private UnityInterstitialAdCallback mockInterstitialAdCallback;
  @Mock private InterstitialAdPreloaderWrapper mockWrapper;
  @Captor private ArgumentCaptor<PreloadCallback> preloadCallbackCaptor;

  @Before
  public void setUp() {
    activity = Robolectric.buildActivity(Activity.class).create().get();
    unityInterstitialAdPreloader =
        new UnityInterstitialAdPreloader(
            activity, mockPreloadCallback, mockWrapper, newDirectExecutorService());
    preloadConfiguration = new PreloadConfiguration(mockAdRequest, 1);
  }

  @Test
  public void testStart() {
    boolean unused = unityInterstitialAdPreloader.start(PRELOAD_ID, preloadConfiguration);

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
    UnityInterstitialAdPreloader preloader =
        new UnityInterstitialAdPreloader(activity, null, mockWrapper, newDirectExecutorService());

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
    boolean unused = unityInterstitialAdPreloader.isAdAvailable(PRELOAD_ID);
    verify(mockWrapper).isAdAvailable(PRELOAD_ID);
  }

  @Test
  public void testGetNumAdsAvailable() {
    int unused = unityInterstitialAdPreloader.getNumAdsAvailable(PRELOAD_ID);
    verify(mockWrapper).getNumAdsAvailable(PRELOAD_ID);
  }

  @Test
  public void testPollAd_adAvailable() {
    when(mockWrapper.pollAd(PRELOAD_ID)).thenReturn(mockInterstitialAd);

    UnityInterstitialAd unityInterstitialAd =
        unityInterstitialAdPreloader.pollAd(PRELOAD_ID, mockInterstitialAdCallback);

    assertThat(unityInterstitialAd).isNotNull();
    assertThat(unityInterstitialAd.getInterstitialAd()).isEqualTo(mockInterstitialAd);
  }

  @Test
  public void testPollAd_adNotAvailable() {
    when(mockWrapper.pollAd(PRELOAD_ID)).thenReturn(null);

    UnityInterstitialAd unityInterstitialAd =
        unityInterstitialAdPreloader.pollAd(PRELOAD_ID, mockInterstitialAdCallback);

    assertThat(unityInterstitialAd).isNull();
  }

  @Test
  public void testGetConfiguration() {
    PreloadConfiguration unused = unityInterstitialAdPreloader.getConfiguration(PRELOAD_ID);
    verify(mockWrapper).getConfiguration(PRELOAD_ID);
  }

  @Test
  public void testGetConfigurations() {
    Map<String, PreloadConfiguration> unused = unityInterstitialAdPreloader.getConfigurations();
    verify(mockWrapper).getConfigurations();
  }

  @Test
  public void testDestroy() {
    unityInterstitialAdPreloader.destroy(PRELOAD_ID);
    verify(mockWrapper).destroy(PRELOAD_ID);
  }
}
