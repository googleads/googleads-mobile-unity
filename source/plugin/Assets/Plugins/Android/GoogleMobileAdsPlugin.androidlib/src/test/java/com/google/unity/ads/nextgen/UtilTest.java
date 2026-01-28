package com.google.unity.ads.nextgen;

import static org.junit.Assert.assertEquals;

import com.google.android.libraries.ads.mobile.sdk.common.PrecisionType;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.robolectric.RobolectricTestRunner;

/** Unit tests for {@link Util}. */
@RunWith(RobolectricTestRunner.class)
public final class UtilTest {

  @Test
  public void getAdValuePrecisionType_returnsUnknown() {
    assertEquals(0, Util.getAdValuePrecisionType(PrecisionType.UNKNOWN));
  }

  @Test
  public void getAdValuePrecisionType_returnsEstimated() {
    assertEquals(1, Util.getAdValuePrecisionType(PrecisionType.ESTIMATED));
  }

  @Test
  public void getAdValuePrecisionType_returnsPublisherProvided() {
    assertEquals(2, Util.getAdValuePrecisionType(PrecisionType.PUBLISHER_PROVIDED));
  }

  @Test
  public void getAdValuePrecisionType_returnsPrecise() {
    assertEquals(3, Util.getAdValuePrecisionType(PrecisionType.PRECISE));
  }
}
