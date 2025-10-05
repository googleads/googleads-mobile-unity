package com.google.unity.ads.nativead;

import static com.google.common.truth.Truth.assertThat;

import com.google.testing.junit.testparameterinjector.TestParameter;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.robolectric.RobolectricTestParameterInjector;

/** Tests for {@link UnityNativeTemplateFontStyle} */
@RunWith(RobolectricTestParameterInjector.class)
public final class UnityNativeTemplateFontStyleTest {

  private enum FontStyleTestCase {
    INVALID_INDEX(-1, UnityNativeTemplateFontStyle.NORMAL), // should return default value
    NORMAL(0, UnityNativeTemplateFontStyle.NORMAL),
    BOLD(1, UnityNativeTemplateFontStyle.BOLD),
    ITALIC(2, UnityNativeTemplateFontStyle.ITALIC),
    MONOSPACE(3, UnityNativeTemplateFontStyle.MONOSPACE),
    MISSING_INDEX(4, UnityNativeTemplateFontStyle.NORMAL); // should return default value

    final int intValue;
    final UnityNativeTemplateFontStyle expectedFontStyle;

    FontStyleTestCase(int intValue, UnityNativeTemplateFontStyle expectedFontStyle) {
      this.intValue = intValue;
      this.expectedFontStyle = expectedFontStyle;
    }
  }

  @Test
  public void fromIntValue_returnsCorrectFontStyle(@TestParameter FontStyleTestCase testCase)
      throws Exception {
    UnityNativeTemplateFontStyle gotFontStyle =
        UnityNativeTemplateFontStyle.fromIntValue(testCase.intValue);
    assertThat(gotFontStyle).isEqualTo(testCase.expectedFontStyle);
    assertThat(gotFontStyle.getTypeface()).isNotNull();
    assertThat(gotFontStyle.getTypeface()).isEqualTo(testCase.expectedFontStyle.getTypeface());
  }
}
