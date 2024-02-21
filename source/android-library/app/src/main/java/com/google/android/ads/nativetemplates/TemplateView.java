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
import android.view.LayoutInflater;
import android.widget.Button;
import android.widget.FrameLayout;
import android.widget.ImageView;
import android.widget.RatingBar;
import android.widget.TextView;
import androidx.annotation.Nullable;
import androidx.constraintlayout.widget.ConstraintLayout;
import com.google.android.gms.ads.nativead.MediaView;
import com.google.android.gms.ads.nativead.NativeAd;
import com.google.android.gms.ads.nativead.NativeAdView;
import com.google.unity.ads.R;

/** Base class for a template view. */
public final class TemplateView extends FrameLayout {

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

  private static final String MEDIUM_TEMPLATE = "medium_template";
  private static final String SMALL_TEMPLATE = "small_template";

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

  public TemplateView(Context context, AttributeSet attrs, int defStyleAttr, int defStyleRes) {
    super(context, attrs, defStyleAttr, defStyleRes);
    initView(context, attrs);
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
      background.setBackground(mainBackground);
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

  private boolean adHasOnlyStore(NativeAd nativeAd) {
    String store = nativeAd.getStore();
    String advertiser = nativeAd.getAdvertiser();
    return !TextUtils.isEmpty(store) && TextUtils.isEmpty(advertiser);
  }

  public void setNativeAd(NativeAd nativeAd) {
    this.nativeAd = nativeAd;

    String store = nativeAd.getStore();
    String advertiser = nativeAd.getAdvertiser();
    String headline = nativeAd.getHeadline();
    String body = nativeAd.getBody();
    String cta = nativeAd.getCallToAction();
    Double starRating = nativeAd.getStarRating();
    NativeAd.Image icon = nativeAd.getIcon();

    String secondaryText;
    callToActionView.setText(cta);

    nativeAdView.setCallToActionView(callToActionView);
    nativeAdView.setHeadlineView(primaryView);
    nativeAdView.setMediaView(mediaView);
    secondaryView.setVisibility(VISIBLE);
    if (adHasOnlyStore(nativeAd)) {
      nativeAdView.setStoreView(secondaryView);
      secondaryText = store;
    } else if (!TextUtils.isEmpty(advertiser)) {
      nativeAdView.setAdvertiserView(secondaryView);
      secondaryText = advertiser;
    } else {
      secondaryText = "";
    }

    primaryView.setText(headline);

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
    nativeAd.destroy();
  }

  public String getTemplateTypeName() {
    if (templateType == R.layout.gnt_medium_template_view) {
      return MEDIUM_TEMPLATE;
    } else if (templateType == R.layout.gnt_small_template_view) {
      return SMALL_TEMPLATE;
    }
    return "";
  }

  private void initView(Context context, AttributeSet attributeSet) {
    System.out.println("<VINAY> Inflating layout!");
    TypedArray attributes =
        context.getTheme().obtainStyledAttributes(attributeSet, R.styleable.TemplateView, 0, 0);

    try {
      templateType =
          attributes.getResourceId(
              R.styleable.TemplateView_gnt_template_type, R.layout.gnt_medium_template_view);
    } finally {
      attributes.recycle();
    }
    System.out.println("<VINAY> LayoutInflater!");
    LayoutInflater inflater =
        (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
    System.out.println("<VINAY> inflate!");
    inflater.inflate(templateType, this);
  }

  @Override
  public void onFinishInflate() {
    super.onFinishInflate();
    System.out.println("<VINAY> onFinishInflate!");
    nativeAdView = (NativeAdView) findViewById(R.id.native_ad_view);
    primaryView = (TextView) findViewById(R.id.primary);
    secondaryView = (TextView) findViewById(R.id.secondary);
    tertiaryView = (TextView) findViewById(R.id.body);

    ratingBar = (RatingBar) findViewById(R.id.rating_bar);
    ratingBar.setEnabled(false);

    callToActionView = (Button) findViewById(R.id.cta);
    iconView = (ImageView) findViewById(R.id.icon);
    mediaView = (MediaView) findViewById(R.id.media_view);
    background = (ConstraintLayout) findViewById(R.id.background);
  }
}
