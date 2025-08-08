package com.google.unity.ads;

import static com.google.common.truth.Truth.assertThat;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.anyFloat;
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
import com.google.android.gms.ads.internal.client.ClientSingletons;
import com.google.android.gms.ads.internal.client.IAdPreloader;
import com.google.android.gms.ads.internal.client.MockClientSingletons;
import com.google.android.gms.ads.internal.mediation.client.IAdapterCreator;
import com.google.android.gms.ads.internal.rewarded.client.IRewardedAd;
import com.google.android.gms.ads.internal.rewarded.client.IRewardedAdLoadCallback;
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

/** Tests for {@link UnityRewardedAd} */
@RunWith(RobolectricTestRunner.class)
public final class UnityRewardedAdTest {

  private static final String AD_UNIT_ID = "test-ad-unit-id";
  private static final Duration SLEEP_DURATION = Duration.ofMillis(200);

  @Rule public final MockitoRule mockito = MockitoJUnit.rule();
  @Mock private UnityRewardedAdCallback mockCallback;
  @Mock private IAdPreloader mockAdPreloader;
  @Mock private IRewardedAd mockRewardedAd;

  private Activity activity;
  private UnityRewardedAd unityRewardedAd;

  @Before
  public void setUp() {
    activity = Robolectric.buildActivity(Activity.class).create().get();
    unityRewardedAd = new UnityRewardedAd(activity, mockCallback);
    AdPreloaderSingleton.setAdPreloader(mockAdPreloader);
    new MockClientSingletons.Installer()
        .mockFlags()
        .mockClientApiBroker(mockAdPreloader)
        .mockClientApiBroker(mockRewardedAd)
        .install();
    when(ClientSingletons.clientApiBroker()
            .getAdPreloader(any(Context.class), any(IAdapterCreator.class)))
        .thenReturn(mockAdPreloader);
    when(ClientSingletons.clientApiBroker()
            .createRewardedAd(any(Context.class), eq(AD_UNIT_ID), any(IAdapterCreator.class)))
        .thenReturn(mockRewardedAd);
  }

  @After
  public void tearDown() {
    MockClientSingletons.tearDown();
    unityRewardedAd.destroy();
  }

  @Test
  public void getRewardItem_returnsNull_whenAdIsNull() throws Exception {
    // Sanity check to ensure that the method doesn't throw an exception.
    assertThat(unityRewardedAd.getRewardItem()).isNull();
  }

  @Test
  public void loadAd_callsOnRewardedAdLoaded_whenAdLoadSucceeds() throws Exception {
    loadRewardedAd();

    assertThat(unityRewardedAd.isAdAvailable(AD_UNIT_ID)).isFalse();
    assertThat(unityRewardedAd.getAdUnitId()).isEqualTo(AD_UNIT_ID);
    assertThat(unityRewardedAd.getResponseInfo()).isNotNull();
    assertThat(unityRewardedAd.getRewardItem()).isNotNull();

    verify(mockCallback).onRewardedAdLoaded();
    verify(mockCallback, never()).onUserEarnedReward(anyString(), anyFloat());
    verify(mockCallback, never()).onAdShowedFullScreenContent();
    verify(mockCallback, never()).onPaidEvent(anyInt(), anyLong(), anyString());
    verify(mockRewardedAd).getResponseInfo();
  }

  @Test
  public void loadAd_callsOnRewardedAdFailedToLoad_whenAdLoadFails() throws Exception {
    doAnswer(
            invocation -> {
              IRewardedAdLoadCallback callback = invocation.getArgument(1);
              // TODO(vkini): Thinking `callback.onRewardedAdFailedToLoad` should also trigger the
              // Unity callback, but it currently doesn't.
              callback.onRewardedAdFailedToLoadWithAdError(
                  new AdErrorParcel(
                      /* errorCode= */ 0,
                      /* errorMessage= */ "Test Error",
                      /* errorDomain= */ "domain",
                      /* cause= */ null,
                      /* responseInfo= */ null));
              return null;
            })
        .when(mockRewardedAd)
        .loadAd(any(), any());

    unityRewardedAd.loadAd(AD_UNIT_ID, new AdRequest.Builder().build());
    // Sleep to wait for background thread execution.
    Sleeper.defaultSleeper().sleep(SLEEP_DURATION);

    verify(mockCallback).onRewardedAdFailedToLoad(any());
  }

  @Test
  public void pollAd_succeeds_whenNotNullRewardedAd() throws Exception {
    when(mockAdPreloader.pollRewardedAd(AD_UNIT_ID)).thenReturn(mockRewardedAd);

    unityRewardedAd.pollAd(AD_UNIT_ID);
    // Sleep to wait for background thread execution.
    Sleeper.defaultSleeper().sleep(SLEEP_DURATION);

    verify(mockCallback, never()).onRewardedAdLoaded();
    verify(mockRewardedAd).setOnPaidEventListener(any());
  }

  @Test
  public void pollAd_fails_whenNullRewardedAd() throws Exception {
    when(mockAdPreloader.pollRewardedAd(AD_UNIT_ID)).thenReturn(null);

    unityRewardedAd.pollAd(AD_UNIT_ID);
    // Sleep to wait for background thread execution.
    Sleeper.defaultSleeper().sleep(SLEEP_DURATION);

    verify(mockCallback).onRewardedAdFailedToLoad(any());
  }

  @Test
  public void isAdAvailable_returnsTrue_whenAdIsAvailableFromPreloader() throws Exception {
    when(mockAdPreloader.isRewardedAdAvailable(AD_UNIT_ID)).thenReturn(true);
    assertThat(unityRewardedAd.isAdAvailable(AD_UNIT_ID)).isTrue();
  }

  @Test
  public void showAd_shouldEnableImmersiveMode_whenLoaded() throws Exception {
    loadRewardedAd();
    unityRewardedAd.show();

    verify(mockCallback, never()).onAdShowedFullScreenContent();
    verify(mockRewardedAd).setImmersiveMode(eq(true));
  }

  private void loadRewardedAd() throws Exception {
    doAnswer(
            invocation -> {
              IRewardedAdLoadCallback callback = invocation.getArgument(1);
              callback.onRewardedAdLoaded();
              return null;
            })
        .when(mockRewardedAd)
        .loadAd(any(), any());

    unityRewardedAd.loadAd(AD_UNIT_ID, new AdRequest.Builder().build());
    // Sleep to wait for background thread execution.
    Sleeper.defaultSleeper().sleep(SLEEP_DURATION);
  }
}
