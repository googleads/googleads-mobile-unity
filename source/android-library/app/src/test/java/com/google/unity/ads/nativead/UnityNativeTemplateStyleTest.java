package com.google.unity.ads.nativead;

import static com.google.common.truth.Truth.assertThat;

import android.graphics.Color;
import android.graphics.drawable.ColorDrawable;
import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.robolectric.RobolectricTestRunner;

/** Tests for {@link UnityNativeTemplateStyle} */
@RunWith(RobolectricTestRunner.class)
public final class UnityNativeTemplateStyleTest {

  private UnityNativeTemplateStyle unityNativeTemplateStyle;

  @Before
  public void setUp() {
    unityNativeTemplateStyle =
        new UnityNativeTemplateStyle(
            /* templateType= */ UnityNativeTemplateType.SMALL,
            /* mainBackgroundColor= */ new ColorDrawable(Color.RED),
            /* callToActionStyle= */ new UnityNativeTemplateTextStyle(
                /* textColor= */ new ColorDrawable(Color.GREEN),
                /* backgroundColor= */ new ColorDrawable(Color.BLUE),
                /* fontStyle= */ UnityNativeTemplateFontStyle.ITALIC,
                /* size= */ 1.0),
            /* primaryTextStyle= */ new UnityNativeTemplateTextStyle(
                /* textColor= */ new ColorDrawable(Color.RED),
                /* backgroundColor= */ new ColorDrawable(Color.BLUE),
                /* fontStyle= */ UnityNativeTemplateFontStyle.ITALIC,
                /* size= */ 5.0),
            /* secondaryTextStyle= */ new UnityNativeTemplateTextStyle(
                /* textColor= */ new ColorDrawable(Color.RED),
                /* backgroundColor= */ new ColorDrawable(Color.BLUE),
                /* fontStyle= */ UnityNativeTemplateFontStyle.ITALIC,
                /* size= */ 20.0),
            /* tertiaryTextStyle= */ new UnityNativeTemplateTextStyle(
                /* textColor= */ new ColorDrawable(Color.RED),
                /* backgroundColor= */ new ColorDrawable(Color.BLUE),
                /* fontStyle= */ UnityNativeTemplateFontStyle.ITALIC,
                /* size= */ 10.0));
  }

  @Test
  public void unityNativeTemplateStyle_canBeInstantiated() throws Exception {
    assertThat(unityNativeTemplateStyle.getTemplateType()).isEqualTo(UnityNativeTemplateType.SMALL);
    assertThat(unityNativeTemplateStyle.getMainBackgroundColor().getColor()).isEqualTo(Color.RED);
    assertThat(unityNativeTemplateStyle.hashCode()).isNotEqualTo(0);

    UnityNativeTemplateTextStyle callToActionStyle =
        unityNativeTemplateStyle.getCallToActionStyle();
    assertThat(callToActionStyle.getTextColor().getColor()).isEqualTo(Color.GREEN);
    assertThat(callToActionStyle.getBackgroundColor().getColor()).isEqualTo(Color.BLUE);
    assertThat(callToActionStyle.getFontStyle()).isEqualTo(UnityNativeTemplateFontStyle.ITALIC);
    assertThat(callToActionStyle.getSize()).isEqualTo(1.0f);

    UnityNativeTemplateTextStyle primaryTextStyle = unityNativeTemplateStyle.getPrimaryTextStyle();
    assertThat(primaryTextStyle.getTextColor().getColor()).isEqualTo(Color.RED);
    assertThat(primaryTextStyle.getBackgroundColor().getColor()).isEqualTo(Color.BLUE);
    assertThat(primaryTextStyle.getFontStyle()).isEqualTo(UnityNativeTemplateFontStyle.ITALIC);
    assertThat(primaryTextStyle.getSize()).isEqualTo(5.0f);

    UnityNativeTemplateTextStyle secondaryTextStyle =
        unityNativeTemplateStyle.getSecondaryTextStyle();
    assertThat(secondaryTextStyle.getTextColor().getColor()).isEqualTo(Color.RED);
    assertThat(secondaryTextStyle.getBackgroundColor().getColor()).isEqualTo(Color.BLUE);
    assertThat(secondaryTextStyle.getFontStyle()).isEqualTo(UnityNativeTemplateFontStyle.ITALIC);
    assertThat(secondaryTextStyle.getSize()).isEqualTo(20.0f);

    UnityNativeTemplateTextStyle tertiaryTextStyle =
        unityNativeTemplateStyle.getTertiaryTextStyle();
    assertThat(tertiaryTextStyle.getTextColor().getColor()).isEqualTo(Color.RED);
    assertThat(tertiaryTextStyle.getBackgroundColor().getColor()).isEqualTo(Color.BLUE);
    assertThat(tertiaryTextStyle.getFontStyle()).isEqualTo(UnityNativeTemplateFontStyle.ITALIC);
    assertThat(tertiaryTextStyle.getSize()).isEqualTo(10.0f);
  }

  @Test
  public void unityNativeTemplateStyle_canBeCompared() throws Exception {
    UnityNativeTemplateStyle anotherUnityNativeTemplateStyle =
        new UnityNativeTemplateStyle(
            /* templateType= */ UnityNativeTemplateType.MEDIUM,
            /* mainBackgroundColor= */ new ColorDrawable(Color.GREEN),
            /* callToActionStyle= */ new UnityNativeTemplateTextStyle(
                /* textColor= */ new ColorDrawable(Color.GREEN),
                /* backgroundColor= */ new ColorDrawable(Color.YELLOW),
                /* fontStyle= */ UnityNativeTemplateFontStyle.BOLD,
                /* size= */ 30.0),
            /* primaryTextStyle= */ new UnityNativeTemplateTextStyle(
                /* textColor= */ new ColorDrawable(Color.GREEN),
                /* backgroundColor= */ new ColorDrawable(Color.YELLOW),
                /* fontStyle= */ UnityNativeTemplateFontStyle.BOLD,
                /* size= */ 30.0),
            /* secondaryTextStyle= */ new UnityNativeTemplateTextStyle(
                /* textColor= */ new ColorDrawable(Color.GREEN),
                /* backgroundColor= */ new ColorDrawable(Color.YELLOW),
                /* fontStyle= */ UnityNativeTemplateFontStyle.BOLD,
                /* size= */ 30.0),
            /* tertiaryTextStyle= */ new UnityNativeTemplateTextStyle(
                /* textColor= */ new ColorDrawable(Color.GREEN),
                /* backgroundColor= */ new ColorDrawable(Color.YELLOW),
                /* fontStyle= */ UnityNativeTemplateFontStyle.BOLD,
                /* size= */ 30.0));
    assertThat(unityNativeTemplateStyle).isNotEqualTo(anotherUnityNativeTemplateStyle);
    assertThat(unityNativeTemplateStyle.hashCode())
        .isNotEqualTo(anotherUnityNativeTemplateStyle.hashCode());
  }
}
