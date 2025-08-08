package com.google.unity.ads;

import static com.google.common.truth.Truth.assertThat;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.anyInt;
import static org.mockito.ArgumentMatchers.anyLong;
import static org.mockito.ArgumentMatchers.anyString;
import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.doAnswer;
import static org.mockito.Mockito.never;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import android.app.Activity;
import android.content.Context;
import com.google.android.gms.ads.AdPreloaderSingleton;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.internal.client.AdErrorParcel;
import com.google.android.gms.ads.internal.client.AdSizeParcel;
import com.google.android.gms.ads.internal.client.ClientSingletons;
import com.google.android.gms.ads.internal.client.IAdLoadCallback;
import com.google.android.gms.ads.internal.client.IAdManager;
import com.google.android.gms.ads.internal.client.IAdPreloader;
import com.google.android.gms.ads.internal.client.MockClientSingletons;
import com.google.android.gms.ads.internal.mediation.client.IAdapterCreator;
import com.google.common.time.Sleeper;
import java.time.Duration;
import org.junit.After;
import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.Mock;
import org.mockito.junit.MockitoJUnit;
import org.mockito.junit.MockitoRule;
import org.robolectric.Robolectric;
import org.robolectric.RobolectricTestRunner;

/** Tests for {@link Interstitial} */
@RunWith(RobolectricTestRunner.class)
public final class InterstitialTest {

  private static final String AD_UNIT_ID = "test-ad-unit-id";
  private static final Duration SLEEP_DURATION = Duration.ofMillis(200);

  @Rule public final MockitoRule mockito = MockitoJUnit.rule();
  @Mock private UnityInterstitialAdCallback mockCallback;
  @Mock private IAdPreloader mockAdPreloader;
  @Mock private IAdManager mockAdManager;

  private Activity activity;
  private Interstitial interstitial;

  @Before
  public void setUp() {
    activity = Robolectric.buildActivity(Activity.class).create().get();
    interstitial = new Interstitial(activity, mockCallback);
    AdPreloaderSingleton.setAdPreloader(mockAdPreloader);
    new MockClientSingletons.Installer()
        .mockFlags()
        .mockClientApiBroker(mockAdPreloader)
        .mockClientApiBroker(mockAdManager)
        .install();
    when(ClientSingletons.clientApiBroker()
            .getAdPreloader(any(Context.class), any(IAdapterCreator.class)))
        .thenReturn(mockAdPreloader);
    when(ClientSingletons.clientApiBroker()
            .createInterstitialAdManager(
                any(Context.class),
                any(AdSizeParcel.class),
                eq(AD_UNIT_ID),
                any(IAdapterCreator.class)))
        .thenReturn(mockAdManager);
  }

  @After
  public void tearDown() {
    MockClientSingletons.tearDown();
    interstitial.destroy();
  }

  @Test
  public void loadAd_callsOnInterstitialAdLoaded_whenAdLoadSucceeds() throws Exception {
    loadInterstitialAd();

    assertThat(interstitial.isAdAvailable(AD_UNIT_ID)).isFalse();
    assertThat(interstitial.getAdUnitId()).isEqualTo(AD_UNIT_ID);
    assertThat(interstitial.getResponseInfo()).isNotNull();

    verify(mockCallback).onInterstitialAdLoaded();
    verify(mockCallback, never()).onAdShowedFullScreenContent();
    verify(mockCallback, never()).onPaidEvent(anyInt(), anyLong(), anyString());
    verify(mockAdManager).getResponseInfo();
    verify(mockAdManager).setFullScreenContentCallback(any());
  }

  @Test
  public void loadAd_callsOnInterstitialAdFailedToLoad_whenAdLoadFails() throws Exception {
    doAnswer(
            invocation -> {
              IAdLoadCallback callback = invocation.getArgument(1);
              callback.onAdFailedToLoad(
                  new AdErrorParcel(
                      /* errorCode= */ 0,
                      /* errorMessage= */ "Test Error",
                      /* errorDomain= */ "domain",
                      /* cause= */ null,
                      /* responseInfo= */ null));
              return null;
            })
        .when(mockAdManager)
        .load(any(), any());

    interstitial.loadAd(AD_UNIT_ID, new AdRequest.Builder().build());
    // Sleep to wait for background thread execution.
    Sleeper.defaultSleeper().sleep(SLEEP_DURATION);

    verify(mockCallback).onInterstitialAdFailedToLoad(any());
  }

  @Test
  public void pollAd_succeeds_whenNotNullAdManager() throws Exception {
    when(mockAdPreloader.pollInterstitialAdManager(AD_UNIT_ID)).thenReturn(mockAdManager);

    interstitial.pollAd(AD_UNIT_ID);

    verify(mockCallback).onInterstitialAdLoaded();
    verify(mockAdManager).setOnPaidEventListener(any());
    verify(mockAdManager).setFullScreenContentCallback(any());
  }

  @Test
  public void pollAd_fails_whenNullAdManager() throws Exception {
    when(mockAdPreloader.pollInterstitialAdManager(AD_UNIT_ID)).thenReturn(null);

    interstitial.pollAd(AD_UNIT_ID);
    // Sleep to wait for background thread execution.
    Sleeper.defaultSleeper().sleep(SLEEP_DURATION);

    verify(mockCallback).onInterstitialAdFailedToLoad(any());
  }

  @Test
  public void isAdAvailable_returnsTrue_whenAdIsAvailableFromPreloader() throws Exception {
    when(mockAdPreloader.isInterstitialAdAvailable(AD_UNIT_ID)).thenReturn(true);
    assertThat(interstitial.isAdAvailable(AD_UNIT_ID)).isTrue();
  }

  @Test
  public void showAd_shouldEnableImmersiveMode_whenLoaded() throws Exception {
    loadInterstitialAd();
    interstitial.show();

    verify(mockCallback, never()).onAdShowedFullScreenContent();
    verify(mockAdManager).setImmersiveMode(eq(true));
  }

  private void loadInterstitialAd() throws Exception {
    doAnswer(
            invocation -> {
              IAdLoadCallback callback = invocation.getArgument(1);
              callback.onAdLoaded();
              return null;
            })
        .when(mockAdManager)
        .load(any(), any());

    interstitial.loadAd(AD_UNIT_ID, new AdRequest.Builder().build());
    // Sleep to wait for background thread execution.
    Sleeper.defaultSleeper().sleep(SLEEP_DURATION);
  }
}
