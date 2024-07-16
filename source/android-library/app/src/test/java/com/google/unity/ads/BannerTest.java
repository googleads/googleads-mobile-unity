package com.google.unity.ads;

import static org.junit.Assert.assertFalse;
import static org.junit.Assert.assertTrue;

import android.app.Activity;
import com.google.android.gms.ads.AdSize;
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
