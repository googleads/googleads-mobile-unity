// Copyright 2019 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

package com.google.android.ads.nativetemplates;

import android.graphics.Typeface;
import android.graphics.drawable.ColorDrawable;
import androidx.annotation.Nullable;
import com.google.errorprone.annotations.CanIgnoreReturnValue;

/** A class containing the optional styling options for the Native Template. */
public final class NativeTemplateStyle {

  // Call to action typeface.
  private Typeface callToActionTextTypeface;

  // Size of call to action text.
  private float callToActionTextSize;

  // Call to action typeface color in the form 0xAARRGGBB.
  @Nullable private Integer callToActionTypefaceColor;

  // Call to action background color.
  private ColorDrawable callToActionBackgroundColor;

  // All templates have a primary text area which is populated by the native ad's headline.

  // Primary text typeface.
  private Typeface primaryTextTypeface;

  // Size of primary text.
  private float primaryTextSize;

  // Primary text typeface color in the form 0xAARRGGBB.
  @Nullable private Integer primaryTextTypefaceColor;

  // Primary text background color.
  private ColorDrawable primaryTextBackgroundColor;

  // The typeface, typeface color, and background color for the second row of text in the template.
  // All templates have a secondary text area which is populated either by the body of the ad or
  // by the rating of the app.

  // Secondary text typeface.
  private Typeface secondaryTextTypeface;

  // Size of secondary text.
  private float secondaryTextSize;

  // Secondary text typeface color in the form 0xAARRGGBB.
  @Nullable private Integer secondaryTextTypefaceColor;

  // Secondary text background color.
  private ColorDrawable secondaryTextBackgroundColor;

  // The typeface, typeface color, and background color for the third row of text in the template.
  // The third row is used to display store name or the default tertiary text.

  // Tertiary text typeface.
  private Typeface tertiaryTextTypeface;

  // Size of tertiary text.
  private float tertiaryTextSize;

  // Tertiary text typeface color in the form 0xAARRGGBB.
  @Nullable private Integer tertiaryTextTypefaceColor;

  // Tertiary text background color.
  private ColorDrawable tertiaryTextBackgroundColor;

  // The background color for the bulk of the ad.
  private ColorDrawable mainBackgroundColor;

  public Typeface getCallToActionTextTypeface() {
    return callToActionTextTypeface;
  }

  public float getCallToActionTextSize() {
    return callToActionTextSize;
  }

  @Nullable
  public Integer getCallToActionTypefaceColor() {
    return callToActionTypefaceColor;
  }

  public ColorDrawable getCallToActionBackgroundColor() {
    return callToActionBackgroundColor;
  }

  public Typeface getPrimaryTextTypeface() {
    return primaryTextTypeface;
  }

  public float getPrimaryTextSize() {
    return primaryTextSize;
  }

  @Nullable
  public Integer getPrimaryTextTypefaceColor() {
    return primaryTextTypefaceColor;
  }

  public ColorDrawable getPrimaryTextBackgroundColor() {
    return primaryTextBackgroundColor;
  }

  public Typeface getSecondaryTextTypeface() {
    return secondaryTextTypeface;
  }

  public float getSecondaryTextSize() {
    return secondaryTextSize;
  }

  @Nullable
  public Integer getSecondaryTextTypefaceColor() {
    return secondaryTextTypefaceColor;
  }

  public ColorDrawable getSecondaryTextBackgroundColor() {
    return secondaryTextBackgroundColor;
  }

  public Typeface getTertiaryTextTypeface() {
    return tertiaryTextTypeface;
  }

  public float getTertiaryTextSize() {
    return tertiaryTextSize;
  }

  @Nullable
  public Integer getTertiaryTextTypefaceColor() {
    return tertiaryTextTypefaceColor;
  }

  public ColorDrawable getTertiaryTextBackgroundColor() {
    return tertiaryTextBackgroundColor;
  }

  public ColorDrawable getMainBackgroundColor() {
    return mainBackgroundColor;
  }

  /** A class that provides helper methods to build a style object. */
  public static final class Builder {

    private NativeTemplateStyle styles;

    public Builder() {
      this.styles = new NativeTemplateStyle();
    }

    @CanIgnoreReturnValue
    public Builder withCallToActionTextTypeface(Typeface callToActionTextTypeface) {
      this.styles.callToActionTextTypeface = callToActionTextTypeface;
      return this;
    }

    @CanIgnoreReturnValue
    public Builder withCallToActionTextSize(float callToActionTextSize) {
      this.styles.callToActionTextSize = callToActionTextSize;
      return this;
    }

    @CanIgnoreReturnValue
    public Builder withCallToActionTypefaceColor(int callToActionTypefaceColor) {
      this.styles.callToActionTypefaceColor = callToActionTypefaceColor;
      return this;
    }

    @CanIgnoreReturnValue
    public Builder withCallToActionBackgroundColor(ColorDrawable callToActionBackgroundColor) {
      this.styles.callToActionBackgroundColor = callToActionBackgroundColor;
      return this;
    }

    @CanIgnoreReturnValue
    public Builder withPrimaryTextTypeface(Typeface primaryTextTypeface) {
      this.styles.primaryTextTypeface = primaryTextTypeface;
      return this;
    }

    @CanIgnoreReturnValue
    public Builder withPrimaryTextSize(float primaryTextSize) {
      this.styles.primaryTextSize = primaryTextSize;
      return this;
    }

    @CanIgnoreReturnValue
    public Builder withPrimaryTextTypefaceColor(int primaryTextTypefaceColor) {
      this.styles.primaryTextTypefaceColor = primaryTextTypefaceColor;
      return this;
    }

    @CanIgnoreReturnValue
    public Builder withPrimaryTextBackgroundColor(ColorDrawable primaryTextBackgroundColor) {
      this.styles.primaryTextBackgroundColor = primaryTextBackgroundColor;
      return this;
    }

    @CanIgnoreReturnValue
    public Builder withSecondaryTextTypeface(Typeface secondaryTextTypeface) {
      this.styles.secondaryTextTypeface = secondaryTextTypeface;
      return this;
    }

    @CanIgnoreReturnValue
    public Builder withSecondaryTextSize(float secondaryTextSize) {
      this.styles.secondaryTextSize = secondaryTextSize;
      return this;
    }

    @CanIgnoreReturnValue
    public Builder withSecondaryTextTypefaceColor(int secondaryTextTypefaceColor) {
      this.styles.secondaryTextTypefaceColor = secondaryTextTypefaceColor;
      return this;
    }

    @CanIgnoreReturnValue
    public Builder withSecondaryTextBackgroundColor(ColorDrawable secondaryTextBackgroundColor) {
      this.styles.secondaryTextBackgroundColor = secondaryTextBackgroundColor;
      return this;
    }

    @CanIgnoreReturnValue
    public Builder withTertiaryTextTypeface(Typeface tertiaryTextTypeface) {
      this.styles.tertiaryTextTypeface = tertiaryTextTypeface;
      return this;
    }

    @CanIgnoreReturnValue
    public Builder withTertiaryTextSize(float tertiaryTextSize) {
      this.styles.tertiaryTextSize = tertiaryTextSize;
      return this;
    }

    @CanIgnoreReturnValue
    public Builder withTertiaryTextTypefaceColor(int tertiaryTextTypefaceColor) {
      this.styles.tertiaryTextTypefaceColor = tertiaryTextTypefaceColor;
      return this;
    }

    @CanIgnoreReturnValue
    public Builder withTertiaryTextBackgroundColor(ColorDrawable tertiaryTextBackgroundColor) {
      this.styles.tertiaryTextBackgroundColor = tertiaryTextBackgroundColor;
      return this;
    }

    @CanIgnoreReturnValue
    public Builder withMainBackgroundColor(ColorDrawable mainBackgroundColor) {
      this.styles.mainBackgroundColor = mainBackgroundColor;
      return this;
    }

    public NativeTemplateStyle build() {
      return styles;
    }
  }
}
