package com.google.unity.ads.nativead;

import static com.google.common.truth.Truth.assertThat;

import android.graphics.Color;
import android.graphics.drawable.ColorDrawable;
import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.robolectric.RobolectricTestRunner;

/** Tests for {@link UnityNativeTemplateTextStyle} */
@RunWith(RobolectricTestRunner.class)
public final class UnityNativeTemplateTextStyleTest {

  private UnityNativeTemplateTextStyle unityNativeTemplateTextStyle;

  @Before
  public void setUp() {
    unityNativeTemplateTextStyle =
        new UnityNativeTemplateTextStyle(
            /* textColor= */ new ColorDrawable(Color.RED),
            /* backgroundColor= */ new ColorDrawable(Color.BLUE),
            /* fontStyle= */ UnityNativeTemplateFontStyle.ITALIC,
            /* size= */ 10.0);
  }

  @Test
  public void unityNativeTemplateTextStyle_canBeInstantiated() throws Exception {
    assertThat(unityNativeTemplateTextStyle.getTextColor().getColor()).isEqualTo(Color.RED);
    assertThat(unityNativeTemplateTextStyle.getBackgroundColor().getColor()).isEqualTo(Color.BLUE);
    assertThat(unityNativeTemplateTextStyle.getFontStyle())
        .isEqualTo(UnityNativeTemplateFontStyle.ITALIC);
    assertThat(unityNativeTemplateTextStyle.getSize()).isEqualTo(10.0f);
    assertThat(unityNativeTemplateTextStyle.hashCode()).isNotEqualTo(0);
  }

  @Test
  public void unityNativeTemplateTextStyle_canBeCompared() throws Exception {
    UnityNativeTemplateTextStyle anotherUnityNativeTemplateTextStyle =
        new UnityNativeTemplateTextStyle(
            new ColorDrawable(Color.GREEN),
            new ColorDrawable(Color.YELLOW),
            UnityNativeTemplateFontStyle.BOLD,
            30.0);
    assertThat(unityNativeTemplateTextStyle).isNotEqualTo(anotherUnityNativeTemplateTextStyle);
    assertThat(unityNativeTemplateTextStyle.hashCode())
        .isNotEqualTo(anotherUnityNativeTemplateTextStyle.hashCode());
  }
}
