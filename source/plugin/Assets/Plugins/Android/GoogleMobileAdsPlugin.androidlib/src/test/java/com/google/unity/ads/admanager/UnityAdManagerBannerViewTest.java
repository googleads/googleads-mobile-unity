package com.google.unity.ads.admanager;

import static com.google.common.truth.Truth.assertThat;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.doAnswer;
import static org.mockito.Mockito.never;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import android.app.Activity;
import android.content.Context;
import com.google.android.gms.ads.AdSize;
import com.google.android.gms.ads.admanager.AdManagerAdRequest;
import com.google.android.gms.ads.internal.client.AdRequestParcel;
import com.google.android.gms.ads.internal.client.AdSizeParcel;
import com.google.android.gms.ads.internal.client.ClientSingletons;
import com.google.android.gms.ads.internal.client.IAdLoadCallback;
import com.google.android.gms.ads.internal.client.IAdManager;
import com.google.android.gms.ads.internal.client.MockClientSingletons;
import com.google.android.gms.ads.internal.mediation.client.IAdapterCreator;
import com.google.common.time.Sleeper;
import com.google.unity.ads.PluginUtils;
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

/** Tests for {@link UnityAdManagerBannerView} */
@RunWith(RobolectricTestRunner.class)
public final class UnityAdManagerBannerViewTest {

  private static final String AD_UNIT_ID = "test-ad-unit-id";
  private static final String REQUEST_AGENT = "test-request-agent";
  private static final Duration SLEEP_DURATION = Duration.ofMillis(200);

  @Rule public final MockitoRule mockito = MockitoJUnit.rule();
  @Mock private UnityAdManagerAdListener mockListener;
  @Mock private IAdManager mockAdManager;

  private Activity activity;
  private UnityAdManagerBannerView unityAdManagerBannerView;

  @Before
  public void setUp() {
    activity = Robolectric.buildActivity(Activity.class).create().get();
    unityAdManagerBannerView = new UnityAdManagerBannerView(activity, mockListener);
    // The ad view creation is delegated to the Unity plugin. This is also where the listener is
    // set on the view.
    unityAdManagerBannerView.create(AD_UNIT_ID, AdSize.BANNER, PluginUtils.POSITION_TOP);
    new MockClientSingletons.Installer().mockFlags().mockClientApiBroker(mockAdManager).install();
    when(ClientSingletons.clientApiBroker()
            .createBannerAdManager(
                any(Context.class),
                any(AdSizeParcel.class),
                eq(AD_UNIT_ID),
                any(IAdapterCreator.class)))
        .thenReturn(mockAdManager);
  }

  @After
  public void tearDown() {
    MockClientSingletons.tearDown();
    unityAdManagerBannerView.destroy();
  }

  @Test
  public void loadAd_callsOnAdLoaded_whenAdLoadSucceeds() throws Exception {
    loadAdManagerBannerAd();

    // Assert
    verify(mockAdManager).setAppEventListener(any());
    verify(mockAdManager).setOnPaidEventListener(any());
    verify(mockAdManager).setManualImpressionsEnabled(false);
    verify(mockAdManager).loadAd(any());

    // TODO(vkini): Why are these not being called? If we cannot make it work, we may need to access
    // the ad manager ad view directly (it would require some modification to the class under test).
    verify(mockAdManager, never()).load(any(), any());
    verify(mockAdManager, never()).resume();
    verify(mockListener, never()).onAdLoaded();
  }

  private void loadAdManagerBannerAd() throws Exception {
    // Arrange
    doAnswer(
            invocation -> {
              // TODO(vkini): This is not being called. This is supposedly what should then trigger
              // the Unity callback on the view.
              IAdLoadCallback callback = invocation.getArgument(1);
              callback.onAdLoaded();
              return null;
            })
        .when(mockAdManager)
        .load(any(), any());

    when(mockAdManager.loadAd(any()))
        .thenAnswer(
            invocation -> {
              AdRequestParcel adRequestParcel = invocation.getArgument(0);
              assertThat(adRequestParcel).isNotNull();
              assertThat(adRequestParcel.requestAgent).isEqualTo(REQUEST_AGENT);
              assertThat(adRequestParcel.requestPackage).isNull();
              return null;
            });

    // Act
    unityAdManagerBannerView.loadAd(
        new AdManagerAdRequest.Builder().setRequestAgent(REQUEST_AGENT).build());
    // Sleep to wait for background thread execution.
    Sleeper.defaultSleeper().sleep(SLEEP_DURATION);
  }
}
