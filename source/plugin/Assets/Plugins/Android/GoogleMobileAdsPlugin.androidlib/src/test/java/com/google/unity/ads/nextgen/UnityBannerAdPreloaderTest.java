package com.google.unity.ads.nextgen;

import static com.google.common.truth.Truth.assertThat;
import static com.google.common.util.concurrent.MoreExecutors.newDirectExecutorService;
import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.timeout;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import com.google.android.libraries.ads.mobile.sdk.banner.AdSize;
import com.google.android.libraries.ads.mobile.sdk.banner.BannerAd;
import com.google.android.libraries.ads.mobile.sdk.banner.BannerAdRequest;
import com.google.android.libraries.ads.mobile.sdk.common.AdRequest;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.common.PreloadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.PreloadConfiguration;
import com.google.android.libraries.ads.mobile.sdk.common.ResponseInfo;
import com.google.unity.ads.nextgen.UnityBannerAdPreloader.BannerAdPreloaderWrapper;
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

/** Unit tests for {@link UnityBannerAdPreloader}. */
@RunWith(RobolectricTestRunner.class)
public final class UnityBannerAdPreloaderTest {

  @Rule public final MockitoRule mocks = MockitoJUnit.rule();

  private Activity activity;
  private UnityBannerAdPreloader unityBannerAdPreloader;
  private PreloadConfiguration preloadConfiguration;

  private static final String PRELOAD_ID = "preloadId";

  @Mock private UnityPreloadCallback mockPreloadCallback;
  @Mock private AdRequest mockAdRequest;
  @Mock private BannerAd mockBannerAd;
  @Mock private UnityBannerAdCallback mockBannerAdCallback;
  @Mock private BannerAdPreloaderWrapper mockWrapper;
  @Captor private ArgumentCaptor<PreloadCallback> preloadCallbackCaptor;

  @Before
  public void setUp() {
    activity = Robolectric.buildActivity(Activity.class).create().get();
    unityBannerAdPreloader =
        new UnityBannerAdPreloader(
            activity, mockPreloadCallback, mockWrapper, newDirectExecutorService());
    preloadConfiguration = new PreloadConfiguration(mockAdRequest, 1);
  }

  @Test
  public void testStart() {
    boolean unused = unityBannerAdPreloader.start(PRELOAD_ID, preloadConfiguration);

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
    UnityBannerAdPreloader preloader =
        new UnityBannerAdPreloader(activity, null, mockWrapper, newDirectExecutorService());

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
    boolean unused = unityBannerAdPreloader.isAdAvailable(PRELOAD_ID);
    verify(mockWrapper).isAdAvailable(PRELOAD_ID);
  }

  @Test
  public void testGetNumAdsAvailable() {
    int unused = unityBannerAdPreloader.getNumAdsAvailable(PRELOAD_ID);
    verify(mockWrapper).getNumAdsAvailable(PRELOAD_ID);
  }

  @Test
  public void testPollAd_adAvailable() {
    when(mockWrapper.pollAd(PRELOAD_ID)).thenReturn(mockBannerAd);

    UnityBannerAd unityBannerAd = unityBannerAdPreloader.pollAd(PRELOAD_ID, mockBannerAdCallback);

    assertThat(unityBannerAd).isNotNull();
    when(mockBannerAd.getView(activity)).thenReturn(new View(activity));

    BannerAdRequest adRequest = new BannerAdRequest.Builder("adUnit", AdSize.BANNER).build();
    unityBannerAd.load(adRequest);
    verify(mockBannerAdCallback, timeout(1000)).onAdLoaded();
  }

  @Test
  public void testPollAd_adNotAvailable() {
    when(mockWrapper.pollAd(PRELOAD_ID)).thenReturn(null);

    UnityBannerAd unityBannerAd = unityBannerAdPreloader.pollAd(PRELOAD_ID, mockBannerAdCallback);

    assertThat(unityBannerAd).isNull();
  }

  @Test
  public void testGetConfiguration() {
    PreloadConfiguration unused = unityBannerAdPreloader.getConfiguration(PRELOAD_ID);
    verify(mockWrapper).getConfiguration(PRELOAD_ID);
  }

  @Test
  public void testGetConfigurations() {
    Map<String, PreloadConfiguration> unused = unityBannerAdPreloader.getConfigurations();
    verify(mockWrapper).getConfigurations();
  }

  @Test
  public void testDestroy() {
    unityBannerAdPreloader.destroy(PRELOAD_ID);
    verify(mockWrapper).destroy(PRELOAD_ID);
  }

  @Test
  public void testPublicConstructor() {
    UnityBannerAdPreloader preloader = new UnityBannerAdPreloader(activity, mockPreloadCallback);
    assertThat(preloader).isNotNull();
  }
}
