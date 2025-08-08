package com.google.unity.ads;

import static com.google.common.truth.Truth.assertThat;
import static org.mockito.Mockito.verify;

import android.app.Activity;
import android.view.View;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.AdSize;
import com.google.android.gms.ads.LoadAdError;
import com.google.unity.ads.fakes.FakeAdView;
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

/** Tests for {@link Banner} */
@RunWith(RobolectricTestRunner.class)
public final class BannerTest {

  @Rule public final MockitoRule mockito = MockitoJUnit.rule();
  @Mock private UnityAdListener mockListener;

  private Activity activity;
  private Banner banner;
  private FakeAdView fakeAdView;

  @Before
  public void setUp() {
    activity = Robolectric.buildActivity(Activity.class).create().get();
    banner = new Banner(activity, mockListener);
    fakeAdView = new FakeAdView(activity, /* adViewType= */ 0);
    banner.create("test-ad-unit-id", AdSize.BANNER, PluginUtils.POSITION_TOP);
  }

  @After
  public void tearDown() {
    banner.destroy();
  }

  @Test
  public void createBanner_adViewIsNotNull() {
    assertThat(banner.adView).isNotNull();
  }

  // TODO(srichakradhar): This test - and probably the other ones below - are not testing the right
  // things. They trivially set some fields on the listener and then verify them. For example, the
  // banner ad load test below is passing even if the banner ad is not loaded. As such, I would
  // recommend to rewrite those tests.
  @Test
  public void loadAd_callsOnAdLoaded_whenAdLoadSucceeds() {
    banner.loadAd(new AdRequest.Builder().build());

    // Simulate a successful ad load.
    fakeAdView.setAdListener(mockListener);
    fakeAdView.simulateAdLoaded();

    // Verify that the UnityAdListener's onAdLoaded method is called.
    verify(mockListener).onAdLoaded();
  }

  @Test
  public void loadAd_callsOnAdFailedToLoad_whenAdLoadFails() {
    banner.loadAd(new AdRequest.Builder().build());

    // Simulate an ad failed to load event.
    LoadAdError loadAdError =
        new LoadAdError(
            /* code= */ 0,
            /* message= */ "Test Error",
            /* domain= */ "domain",
            /* cause= */ null,
            /* responseInfo= */ null);
    fakeAdView.setAdListener(mockListener);
    fakeAdView.simulateAdFailedToLoad(loadAdError);

    // Verify that the UnityAdListener's onAdFailedToLoad method is called.
    verify(mockListener).onAdFailedToLoad(loadAdError);
  }

  @Test
  public void isCollapsible_returnsTrue_whenAdViewIsCollapsible() {
    fakeAdView.setCollapsible(true);
    banner.adView = fakeAdView;
    assertThat(banner.isCollapsible()).isTrue();
  }

  @Test
  public void isCollapsible_returnsFalse_whenAdViewIsNotCollapsible() {
    fakeAdView.setCollapsible(false);
    banner.adView = fakeAdView;
    assertThat(banner.isCollapsible()).isFalse();
  }

  @Test
  public void isCollapsible_returnsFalse_whenAdViewIsNull() {
    fakeAdView.setCollapsible(true);
    banner.adView = null;
    assertThat(banner.isCollapsible()).isFalse();
  }

  @Test
  public void isVisible_returnsTrue_whenAdViewIsVisible() {
    fakeAdView.setVisibility(View.VISIBLE);
    banner.adView = fakeAdView;
    assertThat(banner.isVisible()).isTrue();
  }

  @Test
  public void isVisible_returnsFalse_whenAdViewIsNull() {
    banner.adView = null;
    assertThat(banner.isVisible()).isFalse();
  }

  @Test
  public void isVisible_returnsFalse_whenAdViewIsGone() {
    fakeAdView.setVisibility(View.GONE);
    banner.adView = fakeAdView;
    assertThat(banner.isVisible()).isFalse();
  }

  @Test
  public void isVisible_returnsFalse_whenAdViewIsInvisible() {
    fakeAdView.setVisibility(View.INVISIBLE);
    banner.adView = fakeAdView;
    assertThat(banner.isVisible()).isFalse();
  }

  @Test
  public void show_setsAdViewVisibilityToVisible() {
    banner.show();
    assertThat(banner.adView.getVisibility()).isEqualTo(View.VISIBLE);
  }

  @Test
  public void hide_setsAdViewVisibilityToGone() {
    banner.hide();
    assertThat(banner.adView.getVisibility()).isEqualTo(View.GONE);
  }
}
