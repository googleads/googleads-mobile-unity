package com.google.unity.ads;

import static com.google.common.truth.Truth.assertThat;
import static org.junit.Assert.assertThrows;

import android.content.res.Resources;
import android.view.Gravity;
import com.google.android.gms.ads.AdRequest;
import com.google.testing.junit.testparameterinjector.TestParameter;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.robolectric.RobolectricTestParameterInjector;

/** Tests for {@link PluginUtils} */
@RunWith(RobolectricTestParameterInjector.class)
public final class PluginUtilsTest {

  private enum ErrorReasonTestCase {
    INTERNAL_ERROR(AdRequest.ERROR_CODE_INTERNAL_ERROR, "Internal error"),
    INVALID_REQUEST(AdRequest.ERROR_CODE_INVALID_REQUEST, "Invalid request"),
    NETWORK_ERROR(AdRequest.ERROR_CODE_NETWORK_ERROR, "Network error"),
    NO_FILL(AdRequest.ERROR_CODE_NO_FILL, "No fill"),
    UNKNOWN_ERROR(1234, "");

    final int errorCode;
    final String expectedErrorReason;

    ErrorReasonTestCase(int errorCode, String expectedErrorReason) {
      this.errorCode = errorCode;
      this.expectedErrorReason = expectedErrorReason;
    }
  }

  @Test
  public void getErrorReason_succeeds(@TestParameter ErrorReasonTestCase testCase)
      throws Exception {
    assertThat(PluginUtils.getErrorReason(testCase.errorCode))
        .isEqualTo(testCase.expectedErrorReason);
  }

  private enum LayoutGravityTestCase {
    TOP(PluginUtils.POSITION_TOP, Gravity.TOP | Gravity.CENTER_HORIZONTAL),
    BOTTOM(PluginUtils.POSITION_BOTTOM, Gravity.BOTTOM | Gravity.CENTER_HORIZONTAL),
    TOP_RIGHT(PluginUtils.POSITION_TOP_RIGHT, Gravity.TOP | Gravity.RIGHT),
    BOTTOM_LEFT(PluginUtils.POSITION_BOTTOM_LEFT, Gravity.BOTTOM | Gravity.LEFT),
    BOTTOM_RIGHT(PluginUtils.POSITION_BOTTOM_RIGHT, Gravity.BOTTOM | Gravity.RIGHT),
    CENTER(PluginUtils.POSITION_CENTER, Gravity.CENTER_HORIZONTAL | Gravity.CENTER_VERTICAL),
    TOP_LEFT(PluginUtils.POSITION_TOP_LEFT, Gravity.TOP | Gravity.LEFT),
    CUSTOM(PluginUtils.POSITION_CUSTOM, Gravity.TOP | Gravity.LEFT);

    final int positionCode;
    final int expectedGravity;

    LayoutGravityTestCase(int positionCode, int expectedGravity) {
      this.positionCode = positionCode;
      this.expectedGravity = expectedGravity;
    }
  }

  @Test
  public void getLayoutGravityForPositionCode_returnsCorrectGravity(
      @TestParameter LayoutGravityTestCase testCase) throws Exception {
    assertThat(PluginUtils.getLayoutGravityForPositionCode(testCase.positionCode))
        .isEqualTo(testCase.expectedGravity);
  }

  @Test
  public void getLayoutGravityForPositionCode_throws_whenPositionCodeIsInvalid() throws Exception {
    assertThrows(
        IllegalArgumentException.class, () -> PluginUtils.getLayoutGravityForPositionCode(1234));
  }

  @Test
  public void convertPixelsToDp_returnsCorrectDp() throws Exception {
    assertThat(PluginUtils.convertPixelsToDp(100.5f)).isEqualTo(100.5f);
    Resources.getSystem().getDisplayMetrics().density = 2.0f;
    assertThat(PluginUtils.convertPixelsToDp(100.5f)).isEqualTo(50.25f);
  }

  @Test
  public void convertPixelsToDp_throws_whenDensityIsZero() throws Exception {
    Resources.getSystem().getDisplayMetrics().density = 0.0f;
    assertThrows(IllegalStateException.class, () -> PluginUtils.convertPixelsToDp(1234));
  }

  @Test
  public void convertDpToPixel_returnsCorrectPixel() throws Exception {
    assertThat(PluginUtils.convertDpToPixel(100.5f)).isEqualTo(100.5f);
    Resources.getSystem().getDisplayMetrics().density = 2.0f;
    assertThat(PluginUtils.convertDpToPixel(100.5f)).isEqualTo(201.0f);
  }
}
