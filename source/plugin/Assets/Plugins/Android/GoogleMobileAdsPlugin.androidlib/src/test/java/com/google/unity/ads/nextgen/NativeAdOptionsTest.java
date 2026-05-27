/*
 * Copyright (C) 2026 Google, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package com.google.unity.ads.nextgen;

import static com.google.common.truth.Truth.assertThat;

import com.google.android.libraries.ads.mobile.sdk.common.AdChoicesPlacement;
import com.google.android.libraries.ads.mobile.sdk.common.VideoOptions;
import com.google.android.libraries.ads.mobile.sdk.nativead.NativeAd;
import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.robolectric.RobolectricTestRunner;

/** Unit tests for {@link NativeAdOptions}. */
@RunWith(RobolectricTestRunner.class)
public final class NativeAdOptionsTest {

  private NativeAdOptions nativeAdOptions;

  @Before
  public void setUp() {
    nativeAdOptions = new NativeAdOptions(0, 0);
  }

  @Test
  public void testConstructor_parameterized() {
    NativeAdOptions options = new NativeAdOptions(2, 0); // LANDSCAPE, C# TopRightCorner
    assertThat(options.getMediaAspectRatio()).isEqualTo(NativeAd.NativeMediaAspectRatio.LANDSCAPE);
    assertThat(options.getAdChoicesPlacement()).isEqualTo(AdChoicesPlacement.TOP_RIGHT);
  }

  @Test
  public void testConstructor_parameterizedNonDefault() {
    NativeAdOptions options = new NativeAdOptions(3, 1); // PORTRAIT, TOP_LEFT
    assertThat(options.getMediaAspectRatio()).isEqualTo(NativeAd.NativeMediaAspectRatio.PORTRAIT);
    assertThat(options.getAdChoicesPlacement()).isEqualTo(AdChoicesPlacement.TOP_LEFT);
  }

  @Test
  public void testSetMediaAspectRatio_validMapping() {
    nativeAdOptions.setMediaAspectRatio(0);
    assertThat(nativeAdOptions.getMediaAspectRatio())
        .isEqualTo(NativeAd.NativeMediaAspectRatio.UNKNOWN);

    nativeAdOptions.setMediaAspectRatio(1);
    assertThat(nativeAdOptions.getMediaAspectRatio())
        .isEqualTo(NativeAd.NativeMediaAspectRatio.ANY);

    nativeAdOptions.setMediaAspectRatio(2);
    assertThat(nativeAdOptions.getMediaAspectRatio())
        .isEqualTo(NativeAd.NativeMediaAspectRatio.LANDSCAPE);

    nativeAdOptions.setMediaAspectRatio(3);
    assertThat(nativeAdOptions.getMediaAspectRatio())
        .isEqualTo(NativeAd.NativeMediaAspectRatio.PORTRAIT);

    nativeAdOptions.setMediaAspectRatio(4);
    assertThat(nativeAdOptions.getMediaAspectRatio())
        .isEqualTo(NativeAd.NativeMediaAspectRatio.SQUARE);
  }

  @Test
  public void testSetMediaAspectRatio_invalidMapping() {
    nativeAdOptions.setMediaAspectRatio(99);
    assertThat(nativeAdOptions.getMediaAspectRatio())
        .isEqualTo(NativeAd.NativeMediaAspectRatio.UNKNOWN);
  }

  @Test
  public void testSetAdChoicesPlacement_validMapping() {
    nativeAdOptions.setAdChoicesPlacement(0); // C# TopRightCorner (0) -> Maps to Kotlin TOP_RIGHT
    assertThat(nativeAdOptions.getAdChoicesPlacement()).isEqualTo(AdChoicesPlacement.TOP_RIGHT);

    nativeAdOptions.setAdChoicesPlacement(1); // C# TopLeftCorner (1) -> Maps to Kotlin TOP_LEFT
    assertThat(nativeAdOptions.getAdChoicesPlacement()).isEqualTo(AdChoicesPlacement.TOP_LEFT);

    nativeAdOptions.setAdChoicesPlacement(2);
    assertThat(nativeAdOptions.getAdChoicesPlacement()).isEqualTo(AdChoicesPlacement.BOTTOM_RIGHT);

    nativeAdOptions.setAdChoicesPlacement(3);
    assertThat(nativeAdOptions.getAdChoicesPlacement()).isEqualTo(AdChoicesPlacement.BOTTOM_LEFT);
  }

  @Test
  public void testSetAdChoicesPlacement_invalidMapping() {
    nativeAdOptions.setAdChoicesPlacement(99);
    assertThat(nativeAdOptions.getAdChoicesPlacement()).isEqualTo(AdChoicesPlacement.TOP_RIGHT);
  }

  @Test
  public void testSetVideoOptions_validObject() {
    VideoOptions videoOptions = new VideoOptions.Builder().setStartMuted(true).build();
    nativeAdOptions.setVideoOptions(videoOptions);
    assertThat(nativeAdOptions.getVideoOptions()).isEqualTo(videoOptions);
  }
}
