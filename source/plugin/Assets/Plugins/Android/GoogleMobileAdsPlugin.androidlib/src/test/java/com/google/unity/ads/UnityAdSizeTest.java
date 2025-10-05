// Copyright 2024 Google Inc. All Rights Reserved.

package com.google.unity.ads;

import static com.google.common.truth.Truth.assertThat;
import static junit.framework.Assert.fail;
import static org.junit.Assert.assertThrows;

import android.app.Activity;
import com.google.android.gms.ads.AdSize;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.robolectric.Robolectric;
import org.robolectric.RobolectricTestRunner;
import org.robolectric.annotation.Config;

/** Tests for {@link UnityAdSize} */
@RunWith(RobolectricTestRunner.class)
public final class UnityAdSizeTest {

  private static final int WIDTH = 320;
  private static final AdSize AD_SIZE = new AdSize(WIDTH, 50);

  private final Activity activity = Robolectric.buildActivity(Activity.class).setup().get();

  @Test
  public void
      testGetCurrentOrientationAnchoredAdaptiveBannerAdSize_withNullActivity_returnsInvalidAdSize() {
    // Null Activity.
    assertThat(UnityAdSize.getCurrentOrientationAnchoredAdaptiveBannerAdSize(null, WIDTH))
        .isEqualTo(AdSize.INVALID);
  }

  @Test
  public void
      testGetCurrentOrientationAnchoredAdaptiveBannerAdSize_withInvalidWidth_raisesIllegalArgumentException() {

    assertThrows(
        IllegalArgumentException.class,
        () -> UnityAdSize.getCurrentOrientationAnchoredAdaptiveBannerAdSize(activity, -WIDTH));
  }

  @Test
  public void
      testGetCurrentOrientationAnchoredAdaptiveBannerAdSize_withValidActivity_returnsValidAdSize() {
    assertThat(UnityAdSize.getCurrentOrientationAnchoredAdaptiveBannerAdSize(activity, WIDTH))
        .isEqualTo(AD_SIZE);
  }

  @Test
  public void testGetCurrentOrientationAnchoredAdaptiveBannerAdSize_withFullWidth() {
    assertThat(
            UnityAdSize.getCurrentOrientationAnchoredAdaptiveBannerAdSize(
                activity, AdSize.FULL_WIDTH))
        .isEqualTo(AD_SIZE);
  }

  @Test
  public void testGetLandscapeAnchoredAdaptiveBannerAdSize_withNullActivity_returnsInvalidAdSize() {
    // Null Activity.
    assertThat(UnityAdSize.getLandscapeAnchoredAdaptiveBannerAdSize(null, WIDTH))
        .isEqualTo(AdSize.INVALID);
  }

  @Test
  public void
      testGetLandscapeAnchoredAdaptiveBannerAdSize_withInvalidWidth_raisesIllegalArgumentException() {
    assertThrows(
        IllegalArgumentException.class,
        () -> UnityAdSize.getLandscapeAnchoredAdaptiveBannerAdSize(activity, -WIDTH));
  }

  @Test
  public void testGetLandscapeAnchoredAdaptiveBannerAdSize_withValidActivity_returnsValidAdSize() {
    assertThat(UnityAdSize.getLandscapeAnchoredAdaptiveBannerAdSize(activity, WIDTH))
        .isEqualTo(AD_SIZE);
  }

  @Test
  public void testGetLandscapeAnchoredAdaptiveBannerAdSize_withFullWidth() {
    assertThat(UnityAdSize.getLandscapeAnchoredAdaptiveBannerAdSize(activity, AdSize.FULL_WIDTH))
        .isEqualTo(AD_SIZE);
  }

  @Test
  public void testGetPortraitAnchoredAdaptiveBannerAdSize_withNullActivity_returnsInvalidAdSize() {
    // Null Activity.
    assertThat(UnityAdSize.getPortraitAnchoredAdaptiveBannerAdSize(null, WIDTH))
        .isEqualTo(AdSize.INVALID);
  }

  @Test
  public void
      testGetPortraitAnchoredAdaptiveBannerAdSize_withInvalidWidth_raisesIllegalArgumentException() {
    try {
      UnityAdSize.getPortraitAnchoredAdaptiveBannerAdSize(activity, -WIDTH);
      fail("Exception expected to be raised already.");
    } catch (IllegalArgumentException e) {
      // Expected.
    }
  }

  @Test
  public void testGetPortraitAnchoredAdaptiveBannerAdSize_withValidActivity_returnsValidAdSize() {
    assertThat(UnityAdSize.getPortraitAnchoredAdaptiveBannerAdSize(activity, WIDTH))
        .isEqualTo(AD_SIZE);
  }

  @Test
  public void testGetPortraitAnchoredAdaptiveBannerAdSize_withFullWidth() {
    assertThat(UnityAdSize.getPortraitAnchoredAdaptiveBannerAdSize(activity, AdSize.FULL_WIDTH))
        .isEqualTo(AD_SIZE);
  }

  @Test
  @Config(qualifiers = "w300dp-h580dp-port")
  public void testFullWidth_300x500Portrait() {
    // (300 / 320) * 50 = 46.875 but minimum height is 50.
    AdSize expectedAdSize = new AdSize(300, 50);
    assertThat(UnityAdSize.getPortraitAnchoredAdaptiveBannerAdSize(activity, AdSize.FULL_WIDTH))
        .isEqualTo(expectedAdSize);
    assertThat(
            UnityAdSize.getCurrentOrientationAnchoredAdaptiveBannerAdSize(
                activity, AdSize.FULL_WIDTH))
        .isEqualTo(expectedAdSize);
    // Max(300 * 0.15 = 45, minimum height 50).
    expectedAdSize = new AdSize(300, 50);
    assertThat(UnityAdSize.getLandscapeAnchoredAdaptiveBannerAdSize(activity, AdSize.FULL_WIDTH))
        .isEqualTo(expectedAdSize);
  }

  @Test
  @Config(qualifiers = "w360dp-h560dp-port")
  public void testFullWidth_360x560Portrait() {
    // (360 / 320) * 50 = 56.25 (max height 560 * 0.15 = 84).
    AdSize expectedAdSize = new AdSize(360, 56);
    assertThat(UnityAdSize.getPortraitAnchoredAdaptiveBannerAdSize(activity, AdSize.FULL_WIDTH))
        .isEqualTo(expectedAdSize);
    assertThat(
            UnityAdSize.getCurrentOrientationAnchoredAdaptiveBannerAdSize(
                activity, AdSize.FULL_WIDTH))
        .isEqualTo(expectedAdSize);
    // Max height for landscape 360 * 0.15 = 54.
    expectedAdSize = new AdSize(360, 54);
    assertThat(UnityAdSize.getLandscapeAnchoredAdaptiveBannerAdSize(activity, AdSize.FULL_WIDTH))
        .isEqualTo(expectedAdSize);
  }

  @Test
  @Config(qualifiers = "w360dp-h560dp-land")
  public void testFullWidth_360x560Landscape() {
    // (560 / 468) * 60 = 71.795 (max height 360 * 0.15 = 54).
    AdSize expectedAdSize = new AdSize(560, 54);
    assertThat(UnityAdSize.getLandscapeAnchoredAdaptiveBannerAdSize(activity, AdSize.FULL_WIDTH))
        .isEqualTo(expectedAdSize);
    assertThat(
            UnityAdSize.getCurrentOrientationAnchoredAdaptiveBannerAdSize(
                activity, AdSize.FULL_WIDTH))
        .isEqualTo(expectedAdSize);
    // (560 / 468) * 60 = 71.795 (max height for portrait 560 * 0.15 = 84).
    expectedAdSize = new AdSize(560, 72);
    assertThat(UnityAdSize.getPortraitAnchoredAdaptiveBannerAdSize(activity, AdSize.FULL_WIDTH))
        .isEqualTo(expectedAdSize);
  }

  @Test
  @Config(qualifiers = "w560dp-h1500dp-port")
  public void testFullWidth_560x1500Portrait() {
    // (560 / 468) * 60 = 71.795 (max height 90).
    AdSize expectedAdSize = new AdSize(560, 72);
    assertThat(UnityAdSize.getPortraitAnchoredAdaptiveBannerAdSize(activity, AdSize.FULL_WIDTH))
        .isEqualTo(expectedAdSize);
    assertThat(
            UnityAdSize.getCurrentOrientationAnchoredAdaptiveBannerAdSize(
                activity, AdSize.FULL_WIDTH))
        .isEqualTo(expectedAdSize);
    // (560 / 468) * 60 = 71.795 (max height for landscape 560 * .15 = 84).
    assertThat(UnityAdSize.getLandscapeAnchoredAdaptiveBannerAdSize(activity, AdSize.FULL_WIDTH))
        .isEqualTo(expectedAdSize);
  }

  @Test
  @Config(qualifiers = "w560dp-h1500dp-land")
  public void testFullWidth_560x1500Landscape() {
    // (1500 / 728) * 90 = 185.4 (max height 560 * .15 = 84).
    AdSize expectedAdSize = new AdSize(1500, 84);
    assertThat(UnityAdSize.getLandscapeAnchoredAdaptiveBannerAdSize(activity, AdSize.FULL_WIDTH))
        .isEqualTo(expectedAdSize);
    assertThat(
            UnityAdSize.getCurrentOrientationAnchoredAdaptiveBannerAdSize(
                activity, AdSize.FULL_WIDTH))
        .isEqualTo(expectedAdSize);
    // 1500 * 0.15 = 225 but, max height is set to 90 for any width.
    expectedAdSize = new AdSize(1500, 90);
    assertThat(UnityAdSize.getPortraitAnchoredAdaptiveBannerAdSize(activity, AdSize.FULL_WIDTH))
        .isEqualTo(expectedAdSize);
  }

  @Test
  @Config(qualifiers = "w800dp-h560dp-land")
  public void testFullWidth_800dpLandscape() {
    // (800 / 728) * 90 = 98.9 (max height 560 * .15 = 84).
    AdSize expectedAdSize = new AdSize(800, 84);
    assertThat(UnityAdSize.getLandscapeAnchoredAdaptiveBannerAdSize(activity, AdSize.FULL_WIDTH))
        .isEqualTo(expectedAdSize);
    assertThat(
            UnityAdSize.getCurrentOrientationAnchoredAdaptiveBannerAdSize(
                activity, AdSize.FULL_WIDTH))
        .isEqualTo(expectedAdSize);
    // (800 / 728) * 90 = 98.9 but, max height is set to 90 for any width.
    expectedAdSize = new AdSize(800, 90);
    assertThat(UnityAdSize.getPortraitAnchoredAdaptiveBannerAdSize(activity, AdSize.FULL_WIDTH))
        .isEqualTo(expectedAdSize);
  }
}
