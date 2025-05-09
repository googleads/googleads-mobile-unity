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
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.internal.client.AdErrorParcel;
import com.google.android.gms.ads.internal.client.ClientSingletons;
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

/** Tests for {@link UnityRewardedInterstitialAd} */
@RunWith(RobolectricTestRunner.class)
public final class UnityRewardedInterstitialAdTest {

  private static final String AD_UNIT_ID = "test-ad-unit-id";
  private static final Duration SLEEP_DURATION = Duration.ofMillis(200);

  @Rule public final MockitoRule mockito = MockitoJUnit.rule();
  @Mock private UnityRewardedInterstitialAdCallback mockCallback;
  @Mock private IRewardedAd mockRewardedAd;

  private Activity activity;
  private UnityRewardedInterstitialAd unityRewardedInterstitialAd;

  @Before
  public void setUp() {
    activity = Robolectric.buildActivity(Activity.class).create().get();
    unityRewardedInterstitialAd = new UnityRewardedInterstitialAd(activity, mockCallback);
    new MockClientSingletons.Installer().mockFlags().mockClientApiBroker(mockRewardedAd).install();
    // TODO(jochac): Overall, no need to overspecify `when()` calls:
    // go/mockito-when-vs-verify#overspecifying-a-when-to-implicitly-expect-an-interaction.
    when(ClientSingletons.clientApiBroker()
            .createRewardedAd(any(Context.class), eq(AD_UNIT_ID), any(IAdapterCreator.class)))
        .thenReturn(mockRewardedAd);
  }

  @After
  public void tearDown() {
    MockClientSingletons.tearDown();
    unityRewardedInterstitialAd.destroy();
  }

  // TODO(jochac): Typically, the expected outcome is at the end of the test name
  // (go/unit-testing-practices?polyglot=java#naming).
  @Test
  public void getRewardItem_returnsNull_whenAdIsNull() throws Exception {
    // Sanity check to ensure that the method doesn't throw an exception.
    assertThat(unityRewardedInterstitialAd.getRewardItem()).isNull();
  }

  @Test
  public void loadAd_callsOnRewardedInterstitialAdLoaded_whenAdLoadSucceeds() throws Exception {
    loadRewardedInterstitialAd();

    assertThat(unityRewardedInterstitialAd.getAdUnitId()).isEqualTo(AD_UNIT_ID);
    assertThat(unityRewardedInterstitialAd.getResponseInfo()).isNotNull();
    assertThat(unityRewardedInterstitialAd.getRewardItem()).isNotNull();

    verify(mockCallback).onRewardedInterstitialAdLoaded();
    verify(mockCallback, never()).onUserEarnedReward(anyString(), anyFloat());
    verify(mockCallback, never()).onAdShowedFullScreenContent();
    verify(mockCallback, never()).onPaidEvent(anyInt(), anyLong(), anyString());
    verify(mockRewardedAd).getResponseInfo();
  }

  @Test
  public void loadAd_callsOnRewardedInterstitialAdFailedToLoad_whenAdLoadFails() throws Exception {
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
        .loadRewardedInterstitialAd(any(), any());

    unityRewardedInterstitialAd.loadAd(AD_UNIT_ID, new AdRequest.Builder().build());
    // Sleep to wait for background thread execution.
    Sleeper.defaultSleeper().sleep(SLEEP_DURATION);

    verify(mockCallback).onRewardedInterstitialAdFailedToLoad(any());
  }

  @Test
  public void showAd_shouldEnableImmersiveMode_whenLoaded() throws Exception {
    loadRewardedInterstitialAd();
    unityRewardedInterstitialAd.show();

    verify(mockCallback, never()).onAdShowedFullScreenContent();
    verify(mockRewardedAd).setImmersiveMode(eq(true));
  }

  private void loadRewardedInterstitialAd() throws Exception {
    doAnswer(
            invocation -> {
              IRewardedAdLoadCallback callback = invocation.getArgument(1);
              callback.onRewardedAdLoaded();
              return null;
            })
        .when(mockRewardedAd)
        .loadRewardedInterstitialAd(any(), any());

    unityRewardedInterstitialAd.loadAd(AD_UNIT_ID, new AdRequest.Builder().build());
    // Sleep to wait for background thread execution.
    Sleeper.defaultSleeper().sleep(SLEEP_DURATION);
  }
}
