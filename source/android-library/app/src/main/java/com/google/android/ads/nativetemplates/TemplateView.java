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

import android.content.Context;
import android.content.res.TypedArray;
import android.graphics.Typeface;
import android.graphics.drawable.Drawable;
import android.text.TextUtils;
import android.util.AttributeSet;
import android.util.Log;
import android.view.LayoutInflater;
import android.widget.Button;
import android.widget.FrameLayout;
import android.widget.ImageView;
import android.widget.RatingBar;
import android.widget.TextView;
import androidx.annotation.Nullable;
import androidx.annotation.VisibleForTesting;
import androidx.constraintlayout.widget.ConstraintLayout;
import com.google.android.gms.ads.nativead.MediaView;
import com.google.android.gms.ads.nativead.NativeAd;
import com.google.android.gms.ads.nativead.NativeAdView;
import com.google.unity.ads.R;

/**
 * Base class for a template view. Inspired from:
 * http://google3/java/com/google/android/libraries/admob/demo/native_templates/NativeTemplatesAndroid/nativetemplates/src/main/java/com/google/android/ads/nativetemplates/TemplateView.java
 * (resource ids may differ).
 */
public final class TemplateView extends FrameLayout {

  private static final String TAG = TemplateView.class.getSimpleName();

  @VisibleForTesting(otherwise = VisibleForTesting.PRIVATE)
  static final String MEDIUM_TEMPLATE = "medium_template";

  private static final String SMALL_TEMPLATE = "small_template";

  private int templateType;
  private NativeTemplateStyle styles;
  private NativeAd nativeAd;
  private NativeAdView nativeAdView;

  private TextView primaryView;
  private TextView secondaryView;
  private RatingBar ratingBar;
  private TextView tertiaryView;
  private ImageView iconView;
  private MediaView mediaView;
  private Button callToActionView;
  private ConstraintLayout background;
  private LayoutInflater layoutInflater;

  public TemplateView(Context context) {
    super(context);
  }

  public TemplateView(Context context, @Nullable AttributeSet attrs) {
    super(context, attrs);
    initView(context, attrs);
  }

  public TemplateView(Context context, @Nullable AttributeSet attrs, int defStyleAttr) {
    super(context, attrs, defStyleAttr);
    initView(context, attrs);
  }

  public TemplateView(
      Context context, @Nullable AttributeSet attrs, int defStyleAttr, int defStyleRes) {
    super(context, attrs, defStyleAttr, defStyleRes);
    initView(context, attrs);
  }

  @VisibleForTesting(otherwise = VisibleForTesting.NONE)
  public TemplateView(
      Context context,
      @Nullable AttributeSet attrs,
      int defStyleAttr,
      int defStyleRes,
      LayoutInflater layoutInflater) {
    super(context, attrs, defStyleAttr, defStyleRes);
    this.layoutInflater = layoutInflater;
    initView(context, attrs);
  }

  private void initView(Context context, @Nullable AttributeSet attributeSet) {
    if (attributeSet == null) {
      return;
    }
    // See http://shortn/_b5UYcrWluD for more details.
    TypedArray typedArray =
        context
            .getTheme()
            .obtainStyledAttributes(
                /* set= */ attributeSet,
                /* attrs= */ R.styleable.TemplateView,
                /* defStyleAttr= */ 0,
                /* defStyleRes= */ 0);
    if (typedArray == null) {
      return;
    }
    int templateTypeResource = R.styleable.TemplateView_gnt_template_type;
    int templateViewResource = R.layout.gnt_medium_template_view;
    try {
      templateType = typedArray.getResourceId(templateTypeResource, templateViewResource);
    } catch (RuntimeException e) {
      Log.e(
          TAG,
          String.format(
              "Failed to get template type from attribute resources (templateTypeResource: %d, "
                  + "templateViewResource: %d).",
              templateTypeResource, templateViewResource),
          e);
      // Rethrow to let the exception propagate up the stack.
      throw e;
    } finally {
      typedArray.recycle();
    }

    if (layoutInflater == null) {
      setLayoutInflater(context);
    }
    layoutInflater.inflate(templateType, this);
  }

  private void setLayoutInflater(Context context) {
    layoutInflater = (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
  }

  public NativeTemplateStyle getStyles() {
    return styles;
  }

  public void setStyles(NativeTemplateStyle styles) {
    this.styles = styles;
    this.applyStyles();
  }

  public NativeAdView getNativeAdView() {
    return nativeAdView;
  }

  private void applyStyles() {
    Drawable mainBackground = styles.getMainBackgroundColor();
    if (mainBackground != null) {
      if (background != null) {
        background.setBackground(mainBackground);
      }
      if (primaryView != null) {
        primaryView.setBackground(mainBackground);
      }
      if (secondaryView != null) {
        secondaryView.setBackground(mainBackground);
      }
      if (tertiaryView != null) {
        tertiaryView.setBackground(mainBackground);
      }
    }

    Typeface primary = styles.getPrimaryTextTypeface();
    if (primary != null && primaryView != null) {
      primaryView.setTypeface(primary);
    }

    Typeface secondary = styles.getSecondaryTextTypeface();
    if (secondary != null && secondaryView != null) {
      secondaryView.setTypeface(secondary);
    }

    Typeface tertiary = styles.getTertiaryTextTypeface();
    if (tertiary != null && tertiaryView != null) {
      tertiaryView.setTypeface(tertiary);
    }

    Typeface ctaTypeface = styles.getCallToActionTextTypeface();
    if (ctaTypeface != null && callToActionView != null) {
      callToActionView.setTypeface(ctaTypeface);
    }

    if (styles.getPrimaryTextTypefaceColor() != null && primaryView != null) {
      primaryView.setTextColor(styles.getPrimaryTextTypefaceColor());
    }

    if (styles.getSecondaryTextTypefaceColor() != null && secondaryView != null) {
      secondaryView.setTextColor(styles.getSecondaryTextTypefaceColor());
    }

    if (styles.getTertiaryTextTypefaceColor() != null && tertiaryView != null) {
      tertiaryView.setTextColor(styles.getTertiaryTextTypefaceColor());
    }

    if (styles.getCallToActionTypefaceColor() != null && callToActionView != null) {
      callToActionView.setTextColor(styles.getCallToActionTypefaceColor());
    }

    float ctaTextSize = styles.getCallToActionTextSize();
    if (ctaTextSize > 0 && callToActionView != null) {
      callToActionView.setTextSize(ctaTextSize);
    }

    float primaryTextSize = styles.getPrimaryTextSize();
    if (primaryTextSize > 0 && primaryView != null) {
      primaryView.setTextSize(primaryTextSize);
    }

    float secondaryTextSize = styles.getSecondaryTextSize();
    if (secondaryTextSize > 0 && secondaryView != null) {
      secondaryView.setTextSize(secondaryTextSize);
    }

    float tertiaryTextSize = styles.getTertiaryTextSize();
    if (tertiaryTextSize > 0 && tertiaryView != null) {
      tertiaryView.setTextSize(tertiaryTextSize);
    }

    Drawable ctaBackground = styles.getCallToActionBackgroundColor();
    if (ctaBackground != null && callToActionView != null) {
      callToActionView.setBackground(ctaBackground);
    }

    Drawable primaryBackground = styles.getPrimaryTextBackgroundColor();
    if (primaryBackground != null && primaryView != null) {
      primaryView.setBackground(primaryBackground);
    }

    Drawable secondaryBackground = styles.getSecondaryTextBackgroundColor();
    if (secondaryBackground != null && secondaryView != null) {
      secondaryView.setBackground(secondaryBackground);
    }

    Drawable tertiaryBackground = styles.getTertiaryTextBackgroundColor();
    if (tertiaryBackground != null && tertiaryView != null) {
      tertiaryView.setBackground(tertiaryBackground);
    }

    invalidate();
    requestLayout();
  }

  private boolean areAllViewsInitialized() {
    return nativeAdView != null
        && callToActionView != null
        && primaryView != null
        && secondaryView != null
        && tertiaryView != null
        && mediaView != null
        && iconView != null
        && ratingBar != null;
  }

  private boolean adHasOnlyStore(NativeAd nativeAd) {
    String store = nativeAd.getStore();
    String advertiser = nativeAd.getAdvertiser();
    return !TextUtils.isEmpty(store) && TextUtils.isEmpty(advertiser);
  }

  public void setNativeAd(NativeAd nativeAd) {
    this.nativeAd = nativeAd;

    if (!areAllViewsInitialized()) {
      // Defensive check against potential NPEs if a view has not been initialized (which is
      // expected if the template view was solely constructed from context).
      return;
    }

    String store = nativeAd.getStore();
    String advertiser = nativeAd.getAdvertiser();
    String headline = nativeAd.getHeadline();
    String body = nativeAd.getBody();
    String cta = nativeAd.getCallToAction();
    Double starRating = nativeAd.getStarRating();
    NativeAd.Image icon = nativeAd.getIcon();

    nativeAdView.setCallToActionView(callToActionView);
    nativeAdView.setHeadlineView(primaryView);
    nativeAdView.setMediaView(mediaView);
    secondaryView.setVisibility(VISIBLE);

    String secondaryText = "";
    if (adHasOnlyStore(nativeAd)) {
      nativeAdView.setStoreView(secondaryView);
      secondaryText = store;
    } else if (!TextUtils.isEmpty(advertiser)) {
      nativeAdView.setAdvertiserView(secondaryView);
      secondaryText = advertiser;
    }

    primaryView.setText(headline);
    callToActionView.setText(cta);

    //  Set the secondary view to be the star rating if available.
    if (starRating != null && starRating > 0) {
      secondaryView.setVisibility(GONE);
      ratingBar.setVisibility(VISIBLE);
      ratingBar.setRating(starRating.floatValue());

      nativeAdView.setStarRatingView(ratingBar);
    } else {
      secondaryView.setText(secondaryText);
      secondaryView.setVisibility(VISIBLE);
      ratingBar.setVisibility(GONE);
    }

    if (icon != null) {
      iconView.setVisibility(VISIBLE);
      iconView.setImageDrawable(icon.getDrawable());
      nativeAdView.setIconView(iconView);
    } else {
      iconView.setVisibility(GONE);
    }

    if (tertiaryView != null) {
      tertiaryView.setText(body);
      nativeAdView.setBodyView(tertiaryView);
    }

    nativeAdView.setNativeAd(nativeAd);
  }

  /**
   * To prevent memory leaks, make sure to destroy your ad when you don't need it anymore. This
   * method does not destroy the template view.
   * https://developers.google.com/admob/android/native-unified#destroy_ad
   */
  public void destroyNativeAd() {
    if (nativeAd != null) {
      nativeAd.destroy();
    }
  }

  public String getTemplateTypeName() {
    if (templateType == R.layout.gnt_medium_template_view) {
      return MEDIUM_TEMPLATE;
    } else if (templateType == R.layout.gnt_small_template_view) {
      return SMALL_TEMPLATE;
    }
    return "";
  }

  @Override
  public void onFinishInflate() {
    super.onFinishInflate();
    nativeAdView = (NativeAdView) findViewById(R.id.native_ad_view);
    primaryView = (TextView) findViewById(R.id.primary);
    secondaryView = (TextView) findViewById(R.id.secondary);
    tertiaryView = (TextView) findViewById(R.id.body);

    ratingBar = (RatingBar) findViewById(R.id.rating_bar);
    ratingBar.setEnabled(false);

    callToActionView = (Button) findViewById(R.id.cta);
    iconView = (ImageView) findViewById(R.id.ad_icon);
    mediaView = (MediaView) findViewById(R.id.media_view);
    background = (ConstraintLayout) findViewById(R.id.ad_background);
  }
}
