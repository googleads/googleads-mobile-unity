package com.google.unity.ads;

import static com.google.common.truth.Truth.assertThat;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.anyBoolean;
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
import com.google.android.gms.ads.internal.appopen.client.AppOpenAdLoadCallbackProxy;
import com.google.android.gms.ads.internal.appopen.client.IAppOpenAd;
import com.google.android.gms.ads.internal.client.AdErrorParcel;
import com.google.android.gms.ads.internal.client.AdSizeParcel;
import com.google.android.gms.ads.internal.client.ClientSingletons;
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

/** Tests for {@link UnityAppOpenAd} */
@RunWith(RobolectricTestRunner.class)
public final class UnityAppOpenAdTest {

  private static final String AD_UNIT_ID = "test-ad-unit-id";
  private static final Duration SLEEP_DURATION = Duration.ofMillis(200);

  @Rule public final MockitoRule mockito = MockitoJUnit.rule();
  @Mock private UnityAppOpenAdCallback mockCallback;
  @Mock private IAdPreloader mockAdPreloader;
  @Mock private IAdManager mockAdManager;
  @Mock private IAppOpenAd mockAppOpenAd;

  private Activity activity;
  private UnityAppOpenAd unityAppOpenAd;

  @Before
  public void setUp() {
    activity = Robolectric.buildActivity(Activity.class).create().get();
    unityAppOpenAd = new UnityAppOpenAd(activity, mockCallback);
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
            .createAppOpenAdManager(
                any(Context.class),
                any(AdSizeParcel.class),
                eq(AD_UNIT_ID),
                any(IAdapterCreator.class)))
        .thenReturn(mockAdManager);
  }

  @After
  public void tearDown() {
    MockClientSingletons.tearDown();
    unityAppOpenAd.destroy();
  }

  @Test
  public void loadAd_callsOnAppOpenAdLoaded_whenAdLoadSucceeds() throws Exception {
    loadAppOpenAd();

    assertThat(unityAppOpenAd.isAdAvailable(AD_UNIT_ID)).isFalse();
    assertThat(unityAppOpenAd.getAdUnitId()).isEqualTo(AD_UNIT_ID);

    verify(mockCallback).onAppOpenAdLoaded();
    verify(mockCallback, never()).onAdShowedFullScreenContent();
    verify(mockCallback, never()).onPaidEvent(anyInt(), anyLong(), anyString());
  }

  @Test
  public void loadAd_callsOnAppOpenAdFailedToLoad_whenAdLoadFails() throws Exception {
    doAnswer(
            invocation -> {
              AppOpenAdLoadCallbackProxy callbackProxy = invocation.getArgument(0);
              callbackProxy.onAppOpenAdFailedToLoadWithAdError(
                  new AdErrorParcel(
                      /* errorCode= */ 0,
                      /* errorMessage= */ "Test Error",
                      /* errorDomain= */ "domain",
                      /* cause= */ null,
                      /* responseInfo= */ null));
              return null;
            })
        .when(mockAdManager)
        .setAppOpenAdLoadCallback(any());

    unityAppOpenAd.loadAd(AD_UNIT_ID, new AdRequest.Builder().build());
    // Sleep to wait for background thread execution.
    Sleeper.defaultSleeper().sleep(SLEEP_DURATION);

    verify(mockCallback).onAppOpenAdFailedToLoad(any());
  }

  @Test
  public void pollAd_succeeds_whenNotNullAppOpenAd() throws Exception {
    when(mockAdPreloader.pollAppOpenAd(eq(AD_UNIT_ID))).thenReturn(mockAppOpenAd);

    unityAppOpenAd.pollAd(AD_UNIT_ID);

    verify(mockCallback, never()).onAppOpenAdLoaded();
    verify(mockAdManager, never()).setOnPaidEventListener(any());
    verify(mockAdManager, never()).setFullScreenContentCallback(any());
  }

  @Test
  public void pollAd_fails_whenNullAppOpenAd() throws Exception {
    when(mockAdPreloader.pollAppOpenAd(eq(AD_UNIT_ID))).thenReturn(null);

    unityAppOpenAd.pollAd(AD_UNIT_ID);
    // Sleep to wait for background thread execution.
    Sleeper.defaultSleeper().sleep(SLEEP_DURATION);

    verify(mockCallback).onAppOpenAdFailedToLoad(any());
  }

  @Test
  public void isAdAvailable_returnsTrue_whenAdIsAvailableFromPreloader() throws Exception {
    when(mockAdPreloader.isAppOpenAdAvailable(AD_UNIT_ID)).thenReturn(true);
    assertThat(unityAppOpenAd.isAdAvailable(AD_UNIT_ID)).isTrue();
  }

  @Test
  public void showAd_shouldNotSetImmersiveMode_whenLoaded() throws Exception {
    loadAppOpenAd();

    unityAppOpenAd.show();

    verify(mockCallback, never()).onAdShowedFullScreenContent();
    verify(mockAdManager, never()).setImmersiveMode(anyBoolean());
  }

  private void loadAppOpenAd() throws Exception {
    doAnswer(
            invocation -> {
              AppOpenAdLoadCallbackProxy callbackProxy = invocation.getArgument(0);
              callbackProxy.onAppOpenAdLoaded(mockAppOpenAd);
              return null;
            })
        .when(mockAdManager)
        .setAppOpenAdLoadCallback(any());

    unityAppOpenAd.loadAd(AD_UNIT_ID, new AdRequest.Builder().build());
    // Sleep to wait for background thread execution.
    Sleeper.defaultSleeper().sleep(SLEEP_DURATION);
  }
}
