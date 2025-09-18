package com.google.unity.ads.decagon;

import static com.google.common.util.concurrent.MoreExecutors.directExecutor;
import static org.mockito.Mockito.verify;

import android.app.Activity;
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.AdRequest;
import com.google.android.libraries.ads.mobile.sdk.common.FullScreenContentError;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.interstitial.InterstitialAd;
import com.google.android.libraries.ads.mobile.sdk.interstitial.InterstitialAdEventCallback;
import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.ArgumentCaptor;
import org.mockito.Captor;
import org.mockito.Mock;
import org.mockito.Mockito;
import org.mockito.junit.MockitoJUnit;
import org.mockito.junit.MockitoRule;
import org.robolectric.Robolectric;
import org.robolectric.RobolectricTestRunner;

/** Unit tests for {@link UnityInterstitialAd}. */
@RunWith(RobolectricTestRunner.class)
public class UnityInterstitialAdTest {

  @Rule public final MockitoRule mocks = MockitoJUnit.rule();

  private Activity activity;
  @Mock private UnityInterstitialAdCallback mockCallback;
  @Mock private AdRequest mockAdRequest;
  @Mock private InterstitialAd mockInterstitialAd;
  @Mock private InterstitialAdWrapper mockInterstitialAdWrapper;

  @Captor private ArgumentCaptor<AdLoadCallback<InterstitialAd>> adLoadCallbackCaptor;
  @Captor private ArgumentCaptor<InterstitialAdEventCallback> adEventCallbackCaptor;

  private UnityInterstitialAd unityInterstitialAd;

  @Before
  public void setUp() {
    activity = Robolectric.buildActivity(Activity.class).create().get();
    unityInterstitialAd =
        new UnityInterstitialAd(
            activity, mockCallback, mockInterstitialAdWrapper, directExecutor());
  }

  @Test
  public void testLoad_onAdLoaded() {
    unityInterstitialAd.load(mockAdRequest);

    // Capture the callback and simulate successful ad load.
    verify(mockInterstitialAdWrapper)
        .load(Mockito.eq(mockAdRequest), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockInterstitialAd);

    // Verify the callback was invoked. Using timeout because it runs on a separate thread.
    verify(mockCallback).onInterstitialAdLoaded();
  }

  @Test
  public void testLoad_onAdFailedToLoad() {
    unityInterstitialAd.load(mockAdRequest);

    // Capture the callback and simulate failed ad load.
    verify(mockInterstitialAdWrapper)
        .load(Mockito.eq(mockAdRequest), adLoadCallbackCaptor.capture());
    LoadAdError loadAdError = new LoadAdError(LoadAdError.ErrorCode.INTERNAL_ERROR, "domain", null);
    adLoadCallbackCaptor.getValue().onAdFailedToLoad(loadAdError);

    verify(mockCallback).onInterstitialAdFailedToLoad(loadAdError);
  }

  @Test
  public void testShow_showsAdAndTriggersCallbacks() {
    // First, simulate a successful ad load.
    unityInterstitialAd.load(mockAdRequest);
    verify(mockInterstitialAdWrapper)
        .load(Mockito.eq(mockAdRequest), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockInterstitialAd);

    // Now, call show().
    unityInterstitialAd.show();

    // Verify the ad is shown and the event callback is set.
    verify(mockInterstitialAd).setAdEventCallback(adEventCallbackCaptor.capture());
    verify(mockInterstitialAd).show(activity);

    // Trigger and verify all event callbacks.
    InterstitialAdEventCallback eventCallback = adEventCallbackCaptor.getValue();

    eventCallback.onAdShowedFullScreenContent();
    verify(mockCallback).onAdShowedFullScreenContent();

    FullScreenContentError fullScreenContentError =
        new FullScreenContentError(
            FullScreenContentError.ErrorCode.INTERNAL_ERROR, "error message", null);
    eventCallback.onAdFailedToShowFullScreenContent(fullScreenContentError);
    verify(mockCallback).onAdFailedToShowFullScreenContent(fullScreenContentError);

    eventCallback.onAdImpression();
    verify(mockCallback).onAdImpression();

    eventCallback.onAdClicked();
    verify(mockCallback).onAdClicked();

    eventCallback.onAdDismissedFullScreenContent();
    verify(mockCallback).onAdDismissedFullScreenContent();
  }
}
