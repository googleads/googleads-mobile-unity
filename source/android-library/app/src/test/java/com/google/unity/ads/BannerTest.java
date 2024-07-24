package com.google.unity.ads;

import static org.junit.Assert.assertFalse;
import static org.junit.Assert.assertNotNull;
import static org.junit.Assert.assertTrue;
import static org.mockito.Mockito.verify;

import android.app.Activity;
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
public class BannerTest {

  @Rule public final MockitoRule mockito = MockitoJUnit.rule();

  private Banner banner;
  private Activity activity;
  @Mock private UnityAdListener mockListener;
  private FakeAdView fakeAdView;

  @Before
  public void setUp() {
    activity = Robolectric.buildActivity(Activity.class).create().get();
    banner = new Banner(activity, mockListener);
    fakeAdView = new FakeAdView(activity, /* adViewType= */ 0);
    banner.create("test-ad-unit-id", AdSize.BANNER, PluginUtils.POSITION_TOP);
  }

  @Test
  public void testCreateBanner_adViewIsNotNull() {
    assertNotNull(banner.adView);
  }

  @Test
  public void testLoadAd() {
    banner.loadAd(new AdRequest.Builder().build());

    // Simulate a successful ad load.
    fakeAdView.setAdListener(mockListener);
    fakeAdView.simulateAdLoaded();

    // Verify that the UnityAdListener's onAdLoaded method is called.
    verify(mockListener).onAdLoaded();
  }

  @Test
  public void testAdFailedToLoad() {
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
    assertTrue(banner.isCollapsible());
  }

  @Test
  public void isCollapsible_returnsFalse_whenAdViewIsNotCollapsible() {
    fakeAdView.setCollapsible(false);
    banner.adView = fakeAdView;
    assertFalse(banner.isCollapsible());
  }

  @Test
  public void isCollapsible_returnsFalse_whenAdViewIsNull() {
    fakeAdView.setCollapsible(true);
    banner.adView = null;
    assertFalse(banner.isCollapsible());
  }

  @After
  public void tearDown() {
    banner.destroy();
  }
}
