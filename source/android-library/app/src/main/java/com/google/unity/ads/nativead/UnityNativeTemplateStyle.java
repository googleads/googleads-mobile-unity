/*
 * Copyright (C) 2024 Google, Inc.
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

package com.google.unity.ads.nativead;

import android.content.Context;
import android.graphics.drawable.ColorDrawable;
import android.view.LayoutInflater;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.annotation.VisibleForTesting;
import com.google.android.ads.nativetemplates.NativeTemplateStyle;
import com.google.android.ads.nativetemplates.TemplateView;
import java.util.Objects;

/** UnityNativeTemplateStyle class is used to load and configure style to selected Template */
public final class UnityNativeTemplateStyle {

  @NonNull final UnityNativeTemplateType templateType;
  @Nullable final ColorDrawable mainBackgroundColor;
  @Nullable final UnityNativeTemplateTextStyle callToActionStyle;
  @Nullable final UnityNativeTemplateTextStyle primaryTextStyle;
  @Nullable final UnityNativeTemplateTextStyle secondaryTextStyle;
  @Nullable final UnityNativeTemplateTextStyle tertiaryTextStyle;

  private LayoutInflater layoutInflater;

  public UnityNativeTemplateStyle(
      @NonNull UnityNativeTemplateType templateType,
      @Nullable ColorDrawable mainBackgroundColor,
      @Nullable UnityNativeTemplateTextStyle callToActionStyle,
      @Nullable UnityNativeTemplateTextStyle primaryTextStyle,
      @Nullable UnityNativeTemplateTextStyle secondaryTextStyle,
      @Nullable UnityNativeTemplateTextStyle tertiaryTextStyle) {
    this.templateType = templateType;
    this.mainBackgroundColor = mainBackgroundColor;
    this.callToActionStyle = callToActionStyle;
    this.primaryTextStyle = primaryTextStyle;
    this.secondaryTextStyle = secondaryTextStyle;
    this.tertiaryTextStyle = tertiaryTextStyle;
  }

  private void setLayoutInflater(Context context) {
    this.layoutInflater =
        (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
  }

  @VisibleForTesting(otherwise = VisibleForTesting.NONE)
  public void setLayoutInflater(LayoutInflater layoutInflater) {
    this.layoutInflater = layoutInflater;
  }

  @Nullable
  public TemplateView asTemplateView(Context context) {
    if (layoutInflater == null) {
      setLayoutInflater(context);
    }
    TemplateView templateView =
        (TemplateView) layoutInflater.inflate(templateType.resourceId(), null);
    if (templateView == null) {
      return null;
    }

    templateView.setStyles(asNativeTemplateStyle());
    return templateView;
  }

  @NonNull
  public UnityNativeTemplateType getTemplateType() {
    return templateType;
  }

  @Nullable
  public ColorDrawable getMainBackgroundColor() {
    return mainBackgroundColor;
  }

  @Nullable
  public UnityNativeTemplateTextStyle getCallToActionStyle() {
    return callToActionStyle;
  }

  @Nullable
  public UnityNativeTemplateTextStyle getPrimaryTextStyle() {
    return primaryTextStyle;
  }

  @Nullable
  public UnityNativeTemplateTextStyle getSecondaryTextStyle() {
    return secondaryTextStyle;
  }

  @Nullable
  public UnityNativeTemplateTextStyle getTertiaryTextStyle() {
    return tertiaryTextStyle;
  }

  @Override
  public boolean equals(Object o) {
    if (this == o) {
      return true;
    }

    if (!(o instanceof UnityNativeTemplateStyle)) {
      return false;
    }

    UnityNativeTemplateStyle other = (UnityNativeTemplateStyle) o;
    return templateType == other.templateType
        && ((mainBackgroundColor == null && other.mainBackgroundColor == null)
            || mainBackgroundColor.getColor() == other.mainBackgroundColor.getColor())
        && Objects.equals(callToActionStyle, other.callToActionStyle)
        && Objects.equals(primaryTextStyle, other.primaryTextStyle)
        && Objects.equals(secondaryTextStyle, other.secondaryTextStyle)
        && Objects.equals(tertiaryTextStyle, other.tertiaryTextStyle);
  }

  @Override
  public int hashCode() {
    return Objects.hash(
        mainBackgroundColor == null ? null : mainBackgroundColor.getColor(),
        callToActionStyle,
        primaryTextStyle,
        secondaryTextStyle,
        tertiaryTextStyle);
  }

  private NativeTemplateStyle asNativeTemplateStyle() {
    NativeTemplateStyle.Builder builder = new NativeTemplateStyle.Builder();
    if (mainBackgroundColor != null) {
      builder.withMainBackgroundColor(mainBackgroundColor);
    }
    if (callToActionStyle != null) {
      if (callToActionStyle.getBackgroundColor() != null) {
        builder.withCallToActionBackgroundColor(callToActionStyle.getBackgroundColor());
      }
      if (callToActionStyle.getTextColor() != null) {
        builder.withCallToActionTypefaceColor(callToActionStyle.getTextColor().getColor());
      }
      if (callToActionStyle.getFontStyle() != null) {
        builder.withCallToActionTextTypeface(callToActionStyle.getFontStyle().getTypeface());
      }
      if (callToActionStyle.getSize() != null) {
        builder.withCallToActionTextSize(callToActionStyle.getSize());
      }
    }
    if (primaryTextStyle != null) {
      if (primaryTextStyle.getBackgroundColor() != null) {
        builder.withPrimaryTextBackgroundColor(primaryTextStyle.getBackgroundColor());
      }
      if (primaryTextStyle.getTextColor() != null) {
        builder.withPrimaryTextTypefaceColor(primaryTextStyle.getTextColor().getColor());
      }
      if (primaryTextStyle.getFontStyle() != null) {
        builder.withPrimaryTextTypeface(primaryTextStyle.getFontStyle().getTypeface());
      }
      if (primaryTextStyle.getSize() != null) {
        builder.withPrimaryTextSize(primaryTextStyle.getSize());
      }
    }
    if (secondaryTextStyle != null) {
      if (secondaryTextStyle.getBackgroundColor() != null) {
        builder.withSecondaryTextBackgroundColor(secondaryTextStyle.getBackgroundColor());
      }
      if (secondaryTextStyle.getTextColor() != null) {
        builder.withSecondaryTextTypefaceColor(secondaryTextStyle.getTextColor().getColor());
      }
      if (secondaryTextStyle.getFontStyle() != null) {
        builder.withSecondaryTextTypeface(secondaryTextStyle.getFontStyle().getTypeface());
      }
      if (secondaryTextStyle.getSize() != null) {
        builder.withSecondaryTextSize(secondaryTextStyle.getSize());
      }
    }
    if (tertiaryTextStyle != null) {
      if (tertiaryTextStyle.getBackgroundColor() != null) {
        builder.withTertiaryTextBackgroundColor(tertiaryTextStyle.getBackgroundColor());
      }
      if (tertiaryTextStyle.getTextColor() != null) {
        builder.withTertiaryTextTypefaceColor(tertiaryTextStyle.getTextColor().getColor());
      }
      if (tertiaryTextStyle.getFontStyle() != null) {
        builder.withTertiaryTextTypeface(tertiaryTextStyle.getFontStyle().getTypeface());
      }
      if (tertiaryTextStyle.getSize() != null) {
        builder.withTertiaryTextSize(tertiaryTextStyle.getSize());
      }
    }
    return builder.build();
  }
}
