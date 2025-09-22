package com.google.unity.ads.decagon;

import static com.google.common.util.concurrent.MoreExecutors.directExecutor;
import static org.mockito.Mockito.verify;

import android.app.Activity;
import com.google.android.libraries.ads.mobile.sdk.appopen.AppOpenAd;
import com.google.android.libraries.ads.mobile.sdk.appopen.AppOpenAdEventCallback;
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.AdRequest;
import com.google.android.libraries.ads.mobile.sdk.common.FullScreenContentError;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
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

/** Unit tests for {@link UnityAppOpenAd}. */
@RunWith(RobolectricTestRunner.class)
public class UnityAppOpenAdTest {

  @Rule public final MockitoRule mocks = MockitoJUnit.rule();

  private Activity activity;
  @Mock private UnityAppOpenAdCallback mockCallback;
  @Mock private AdRequest mockAdRequest;
  @Mock private AppOpenAd mockAppOpenAd;
  @Mock private AdWrapper<AppOpenAd> mockAdWrapper;

  @Captor private ArgumentCaptor<AdLoadCallback<AppOpenAd>> adLoadCallbackCaptor;
  @Captor private ArgumentCaptor<AppOpenAdEventCallback> adEventCallbackCaptor;

  private UnityAppOpenAd unityAppOpenAd;

  @Before
  public void setUp() {
    activity = Robolectric.buildActivity(Activity.class).create().get();
    unityAppOpenAd = new UnityAppOpenAd(activity, mockCallback, mockAdWrapper, directExecutor());
  }

  @Test
  public void testLoad_onAdLoaded() {
    unityAppOpenAd.load(mockAdRequest);

    // Capture the callback and simulate successful ad load.
    verify(mockAdWrapper).load(Mockito.eq(mockAdRequest), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockAppOpenAd);

    // Verify the callback was invoked. Using timeout because it runs on a separate thread.
    verify(mockCallback).onAppOpenAdLoaded();
  }

  @Test
  public void testLoad_onAdFailedToLoad() {
    unityAppOpenAd.load(mockAdRequest);

    // Capture the callback and simulate failed ad load.
    verify(mockAdWrapper).load(Mockito.eq(mockAdRequest), adLoadCallbackCaptor.capture());
    LoadAdError loadAdError = new LoadAdError(LoadAdError.ErrorCode.INTERNAL_ERROR, "domain", null);
    adLoadCallbackCaptor.getValue().onAdFailedToLoad(loadAdError);

    verify(mockCallback).onAppOpenAdFailedToLoad(loadAdError);
  }

  @Test
  public void testShow_showsAdAndTriggersCallbacks() {
    // First, simulate a successful ad load.
    unityAppOpenAd.load(mockAdRequest);
    verify(mockAdWrapper).load(Mockito.eq(mockAdRequest), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockAppOpenAd);

    // Now, call show().
    unityAppOpenAd.show();

    // Verify the ad is shown and the event callback is set.
    verify(mockAppOpenAd).setAdEventCallback(adEventCallbackCaptor.capture());
    verify(mockAppOpenAd).show(activity);
    verify(mockAppOpenAd).setImmersiveMode(true);

    // Trigger and verify all event callbacks.
    AppOpenAdEventCallback eventCallback = adEventCallbackCaptor.getValue();

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
