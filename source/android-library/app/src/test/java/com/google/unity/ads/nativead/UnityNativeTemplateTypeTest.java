package com.google.unity.ads.nativead;

import static com.google.common.truth.Truth.assertThat;

import com.google.testing.junit.testparameterinjector.TestParameter;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.robolectric.RobolectricTestParameterInjector;

/** Tests for {@link UnityNativeTemplateType} */
@RunWith(RobolectricTestParameterInjector.class)
public final class UnityNativeTemplateTypeTest {

  private enum TemplateTypeTestCase {
    INVALID_INDEX(-1, UnityNativeTemplateType.MEDIUM), // should return default value
    ZERO(0, UnityNativeTemplateType.SMALL),
    ONE(1, UnityNativeTemplateType.MEDIUM),
    MISSING_INDEX(2, UnityNativeTemplateType.MEDIUM); // should return default value

    final int intValue;
    final UnityNativeTemplateType expectedTemplateType;

    TemplateTypeTestCase(int intValue, UnityNativeTemplateType expectedTemplateType) {
      this.intValue = intValue;
      this.expectedTemplateType = expectedTemplateType;
    }
  }

  @Test
  public void fromIntValue_returnsCorrectTemplateType(@TestParameter TemplateTypeTestCase testCase)
      throws Exception {
    assertThat(UnityNativeTemplateType.fromIntValue(testCase.intValue))
        .isEqualTo(testCase.expectedTemplateType);
    assertThat(UnityNativeTemplateType.fromIntValue(testCase.intValue).resourceId())
        .isEqualTo(testCase.expectedTemplateType.resourceId());
  }
}
