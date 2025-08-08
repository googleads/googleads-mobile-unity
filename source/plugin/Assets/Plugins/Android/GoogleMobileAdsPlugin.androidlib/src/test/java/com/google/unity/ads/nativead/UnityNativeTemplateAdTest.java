package com.google.unity.ads.nativead;

import static com.google.common.truth.Truth.assertThat;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.anyInt;
import static org.mockito.ArgumentMatchers.anyLong;
import static org.mockito.ArgumentMatchers.anyString;
import static org.mockito.Mockito.doAnswer;
import static org.mockito.Mockito.never;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import android.app.Activity;
import android.content.Context;
import android.widget.FrameLayout;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.internal.client.AdRequestParcel;
import com.google.android.gms.ads.internal.client.ClientSingletons;
import com.google.android.gms.ads.internal.client.IAdLoader;
import com.google.android.gms.ads.internal.client.IAdLoaderBuilder;
import com.google.android.gms.ads.internal.client.MockClientSingletons;
import com.google.android.gms.ads.internal.formats.client.INativeAdViewDelegate;
import com.google.android.gms.ads.internal.formats.client.IOnUnifiedNativeAdLoadedListener;
import com.google.android.gms.ads.internal.formats.client.IUnifiedNativeAd;
import com.google.android.gms.ads.internal.mediation.client.IAdapterCreator;
import com.google.android.gms.ads.nativead.NativeAdOptions;
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

/** Tests for {@link UnityNativeTemplateAd} */
@RunWith(RobolectricTestRunner.class)
public final class UnityNativeTemplateAdTest {

  private static final String AD_UNIT_ID = "test-ad-unit-id";
  private static final String REQUEST_AGENT = "test-request-agent";
  private static final Duration SLEEP_DURATION = Duration.ofMillis(200);

  @Rule public final MockitoRule mockito = MockitoJUnit.rule();
  @Mock private UnityNativeTemplateAdCallback mockCallback;
  @Mock private INativeAdViewDelegate mockNativeAdViewDelegate;
  @Mock private IAdLoaderBuilder mockAdLoaderBuilder;
  @Mock private IAdLoader mockAdLoader;
  @Mock private IUnifiedNativeAd mockUnifiedNativeAd;

  private Activity activity;
  private UnityNativeTemplateAd unityNativeTemplateAd;

  @Before
  public void setUp() throws Exception {
    activity = Robolectric.buildActivity(Activity.class).create().get();
    unityNativeTemplateAd = new UnityNativeTemplateAd(activity, mockCallback);
    new MockClientSingletons.Installer()
        .mockClientApiBroker(mockNativeAdViewDelegate)
        .mockFlags()
        .install();
    when(ClientSingletons.clientApiBroker()
            .createNativeAdViewDelegate(
                any(Context.class), any(FrameLayout.class), any(FrameLayout.class)))
        .thenReturn(mockNativeAdViewDelegate);
    when(ClientSingletons.clientApiBroker()
            .createAdLoaderBuilder(
                any(Context.class), any(String.class), any(IAdapterCreator.class)))
        .thenReturn(mockAdLoaderBuilder);
    when(mockAdLoaderBuilder.build()).thenReturn(mockAdLoader);
  }

  @After
  public void tearDown() {
    MockClientSingletons.tearDown();
    unityNativeTemplateAd.destroy();
  }

  @Test
  public void loadAd_callsOnNativeAdLoaded_whenAdLoadSucceeds() throws Exception {
    loadNativeTemplateAd();

    assertThat(unityNativeTemplateAd.getResponseInfo()).isNull();

    verify(mockAdLoader).loadAd(any(AdRequestParcel.class));
    verify(mockCallback).onNativeAdLoaded();
    verify(mockUnifiedNativeAd).setOnPaidEventListener(any());
    verify(mockCallback, never()).onPaidEvent(anyInt(), anyLong(), anyString());
    verify(mockCallback, never()).onAdShowedFullScreenContent();
  }

  // TODO(jochac): As a prerequisite to showing the ad, we should call
  // `renderDefaultSizeAtPosition()` as this is needed to init `templateView`.
  @Test
  public void showAd_succeeds() throws Exception {
    loadNativeTemplateAd();
    unityNativeTemplateAd.show(); // just a sanity check for now to make sure it does not crash
  }

  private void loadNativeTemplateAd() throws Exception {
    doAnswer(
            invocation -> {
              AdRequestParcel adRequestParcel = invocation.getArgument(0);
              assertThat(adRequestParcel.requestAgent).isEqualTo(REQUEST_AGENT);
              return null;
            })
        .when(mockAdLoader)
        .loadAd(any());

    doAnswer(
            invocation -> {
              IOnUnifiedNativeAdLoadedListener listener = invocation.getArgument(0);
              listener.onUnifiedNativeAdLoaded(mockUnifiedNativeAd);
              return null;
            })
        .when(mockAdLoaderBuilder)
        .forUnifiedNativeAd(any());

    unityNativeTemplateAd.loadAd(
        AD_UNIT_ID,
        new NativeAdOptions.Builder().build(),
        new AdRequest.Builder().setRequestAgent(REQUEST_AGENT).build());
    // Sleep to wait for background thread execution.
    Sleeper.defaultSleeper().sleep(SLEEP_DURATION);
  }
}
