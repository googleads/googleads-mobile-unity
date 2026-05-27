/*
 * Copyright (C) 2026 Google, Inc.
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

package com.google.unity.ads.nextgen;

import static com.google.common.truth.Truth.assertThat;
import static org.junit.Assert.assertThrows;
import static org.mockito.ArgumentMatchers.anyInt;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;
import static org.robolectric.Shadows.shadowOf;

import android.content.Context;
import android.content.ContextWrapper;
import android.content.res.XmlResourceParser;
import android.graphics.Color;
import android.graphics.Typeface;
import android.graphics.drawable.ColorDrawable;
import android.graphics.drawable.Drawable;
import android.net.Uri;
import android.util.AttributeSet;
import android.util.Xml;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.RatingBar;
import android.widget.TextView;
import androidx.constraintlayout.widget.ConstraintLayout;
import androidx.test.core.app.ApplicationProvider;
import com.google.android.ads.nativetemplates.NativeTemplateStyle;
import com.google.android.libraries.ads.mobile.sdk.common.Image;
import com.google.android.libraries.ads.mobile.sdk.nativead.MediaView;
import com.google.android.libraries.ads.mobile.sdk.nativead.NativeAd;
import com.google.android.libraries.ads.mobile.sdk.nativead.NativeAdView;
import com.google.unity.ads.R;
import java.lang.reflect.Field;
import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.robolectric.RobolectricTestRunner;
import org.robolectric.shadows.ShadowView;

/** Unit tests for nextgen {@link TemplateView}. */
@RunWith(RobolectricTestRunner.class)
public final class TemplateViewTest {

  private Context context;
  private TemplateView templateView;

  private TextView primaryView;
  private Button callToActionView;
  private TextView secondaryView;
  private ImageView iconView;
  private TextView tertiaryView;
  private RatingBar ratingBar;
  private NativeAdView realAdView;

  @Before
  public void setUp() {
    context = ApplicationProvider.getApplicationContext();
    templateView = new TemplateView(context);
  }

  private void setUpStandardViews() throws Exception {
    primaryView = new TextView(context);
    callToActionView = new Button(context);
    secondaryView = new TextView(context);
    iconView = new ImageView(context);
    tertiaryView = new TextView(context);
    ratingBar = new RatingBar(context);
    realAdView = new NativeAdView(context);

    setField("primaryView", primaryView);
    setField("callToActionView", callToActionView);
    setField("secondaryView", secondaryView);
    setField("iconView", iconView);
    setField("tertiaryView", tertiaryView);
    setField("ratingBar", ratingBar);
    setField("nativeAdView", realAdView);
  }

  private NativeAd setUpMockNativeAd() {
    NativeAd mockAd = mock(NativeAd.class);
    when(mockAd.getHeadline()).thenReturn("Headline");
    when(mockAd.getCallToAction()).thenReturn("CTA");
    return mockAd;
  }

  private AttributeSet getLayoutAttributeSet(int layoutResId) {
    XmlResourceParser parser = context.getResources().getXml(layoutResId);
    return Xml.asAttributeSet(parser);
  }

  @Test
  public void setStyles_appliesMainBackgroundColor() throws Exception {
    ConstraintLayout background = new ConstraintLayout(context);
    Field backgroundField = TemplateView.class.getDeclaredField("background");
    backgroundField.setAccessible(true);
    backgroundField.set(templateView, background);

    ColorDrawable expectedColor = new ColorDrawable(Color.RED);
    NativeTemplateStyle styles =
        new NativeTemplateStyle.Builder().withMainBackgroundColor(expectedColor).build();

    templateView.setStyles(styles);

    assertThat(background.getBackground()).isEqualTo(expectedColor);
  }

  @Test
  public void setStyles_appliesPrimaryTextColor() throws Exception {
    TextView primary = new TextView(context);
    Field primaryViewField = TemplateView.class.getDeclaredField("primaryView");
    primaryViewField.setAccessible(true);
    primaryViewField.set(templateView, primary);

    int expectedColor = Color.BLUE;
    NativeTemplateStyle styles =
        new NativeTemplateStyle.Builder().withPrimaryTextTypefaceColor(expectedColor).build();

    templateView.setStyles(styles);

    assertThat(primary.getCurrentTextColor()).isEqualTo(expectedColor);
  }

  @Test
  public void setStyles_invalidatesViewAndRequestsLayout() {
    NativeTemplateStyle styles = new NativeTemplateStyle.Builder().build();

    ShadowView shadowView = shadowOf(templateView);
    shadowView.clearWasInvalidated();
    shadowView.setDidRequestLayout(false);

    templateView.setStyles(styles);

    assertThat(shadowView.wasInvalidated()).isTrue();
    assertThat(shadowView.didRequestLayout()).isTrue();
  }

  @Test
  public void destroyNativeAd_callsDestroyOnNativeAd() {
    NativeAd mockAd = mock(NativeAd.class);

    templateView.setNativeAd(mockAd);
    templateView.destroyNativeAd();

    verify(mockAd).destroy();
  }

  @Test
  public void setNativeAd_bindsHeadlineAndCallToAction() throws Exception {
    setUpStandardViews();

    NativeAd mockAd = mock(NativeAd.class);
    when(mockAd.getHeadline()).thenReturn("Test Headline");
    when(mockAd.getCallToAction()).thenReturn("Click Me");

    templateView.setNativeAd(mockAd);

    assertThat(primaryView.getText().toString()).isEqualTo("Test Headline");
    assertThat(callToActionView.getText().toString()).isEqualTo("Click Me");
    assertThat(realAdView.getHeadlineView()).isEqualTo(primaryView);
    assertThat(realAdView.getCallToActionView()).isEqualTo(callToActionView);
  }

  @Test
  public void setNativeAd_withStoreOnly_bindsStoreView() throws Exception {
    setUpStandardViews();

    NativeAd mockAd = setUpMockNativeAd();
    when(mockAd.getStore()).thenReturn("Play Store");
    when(mockAd.getAdvertiser()).thenReturn(null);

    templateView.setNativeAd(mockAd);

    assertThat(secondaryView.getText().toString()).isEqualTo("Play Store");
    assertThat(realAdView.getStoreView()).isEqualTo(secondaryView);
  }

  @Test
  public void setNativeAd_withAdvertiserOnly_bindsAdvertiserView() throws Exception {
    setUpStandardViews();

    NativeAd mockAd = setUpMockNativeAd();
    when(mockAd.getStore()).thenReturn(null);
    when(mockAd.getAdvertiser()).thenReturn("Google");

    templateView.setNativeAd(mockAd);

    assertThat(secondaryView.getText().toString()).isEqualTo("Google");
    assertThat(realAdView.getAdvertiserView()).isEqualTo(secondaryView);
  }

  @Test
  public void setNativeAd_withBothStoreAndAdvertiser_bindsAdvertiserView() throws Exception {
    setUpStandardViews();

    NativeAd mockAd = setUpMockNativeAd();
    when(mockAd.getStore()).thenReturn("Play Store");
    when(mockAd.getAdvertiser()).thenReturn("Google");

    templateView.setNativeAd(mockAd);

    assertThat(secondaryView.getText().toString()).isEqualTo("Google");
    assertThat(realAdView.getAdvertiserView()).isEqualTo(secondaryView);
  }

  @Test
  public void setNativeAd_withStarRating_bindsRatingBar() throws Exception {
    setUpStandardViews();

    NativeAd mockAd = setUpMockNativeAd();
    when(mockAd.getStarRating()).thenReturn(4.5);

    templateView.setNativeAd(mockAd);

    assertThat(ratingBar.getRating()).isEqualTo(4.5f);
    assertThat(ratingBar.getVisibility()).isEqualTo(View.VISIBLE);
    assertThat(secondaryView.getVisibility()).isEqualTo(View.GONE);
    assertThat(realAdView.getStarRatingView()).isEqualTo(ratingBar);
  }

  @Test
  public void setNativeAd_withIcon_bindsIconView() throws Exception {
    setUpStandardViews();

    Drawable mockDrawable = new ColorDrawable(Color.RED);
    Image mockImage = new Image(mockDrawable, Uri.EMPTY, 1.0);

    NativeAd mockAd = setUpMockNativeAd();
    when(mockAd.getIcon()).thenReturn(mockImage);

    templateView.setNativeAd(mockAd);

    assertThat(iconView.getVisibility()).isEqualTo(View.VISIBLE);
    assertThat(iconView.getDrawable()).isEqualTo(mockDrawable);
    assertThat(realAdView.getIconView()).isEqualTo(iconView);
  }

  @Test
  public void setNativeAd_withBody_bindsTertiaryView() throws Exception {
    setUpStandardViews();

    NativeAd mockAd = setUpMockNativeAd();
    when(mockAd.getBody()).thenReturn("Sample Body");

    templateView.setNativeAd(mockAd);

    assertThat(tertiaryView.getText().toString()).isEqualTo("Sample Body");
    assertThat(realAdView.getBodyView()).isEqualTo(tertiaryView);
  }

  @Test
  public void getTemplateTypeName_returnsMediumTemplate() throws Exception {
    Field typeField = TemplateView.class.getDeclaredField("templateType");
    typeField.setAccessible(true);
    typeField.set(templateView, R.layout.nextgen_medium_template_view);

    assertThat(templateView.getTemplateTypeName()).isEqualTo("medium_template");
  }

  @Test
  public void getTemplateTypeName_returnsSmallTemplate() throws Exception {
    Field typeField = TemplateView.class.getDeclaredField("templateType");
    typeField.setAccessible(true);
    typeField.set(templateView, R.layout.nextgen_small_template_view);

    assertThat(templateView.getTemplateTypeName()).isEqualTo("small_template");
  }

  @Test
  public void destroyNativeAd_whenNativeAdIsNull_returnsEarly() {
    templateView.destroyNativeAd();
  }

  @Test
  public void setStyles_appliesTextColorsAndSizes() throws Exception {
    TextView primary = new TextView(context);
    TextView secondary = new TextView(context);
    TextView tertiary = new TextView(context);
    Button callToAction = new Button(context);

    setField("primaryView", primary);
    setField("secondaryView", secondary);
    setField("tertiaryView", tertiary);
    setField("callToActionView", callToAction);

    ColorDrawable primaryBg = new ColorDrawable(Color.BLACK);
    ColorDrawable secondaryBg = new ColorDrawable(Color.WHITE);
    ColorDrawable tertiaryBg = new ColorDrawable(Color.YELLOW);
    ColorDrawable ctaBg = new ColorDrawable(Color.GRAY);

    Typeface primaryTypeface = Typeface.create(Typeface.SANS_SERIF, Typeface.BOLD);
    Typeface secondaryTypeface = Typeface.create(Typeface.SERIF, Typeface.ITALIC);
    Typeface tertiaryTypeface = Typeface.create(Typeface.MONOSPACE, Typeface.NORMAL);
    Typeface ctaTypeface = Typeface.create(Typeface.DEFAULT, Typeface.BOLD_ITALIC);

    NativeTemplateStyle styles =
        new NativeTemplateStyle.Builder()
            .withPrimaryTextTypefaceColor(Color.RED)
            .withSecondaryTextTypefaceColor(Color.GREEN)
            .withTertiaryTextTypefaceColor(Color.BLUE)
            .withCallToActionTypefaceColor(Color.MAGENTA)
            .withPrimaryTextSize(12f)
            .withSecondaryTextSize(14f)
            .withTertiaryTextSize(16f)
            .withCallToActionTextSize(18f)
            .withPrimaryTextBackgroundColor(primaryBg)
            .withSecondaryTextBackgroundColor(secondaryBg)
            .withTertiaryTextBackgroundColor(tertiaryBg)
            .withCallToActionBackgroundColor(ctaBg)
            .withPrimaryTextTypeface(primaryTypeface)
            .withSecondaryTextTypeface(secondaryTypeface)
            .withTertiaryTextTypeface(tertiaryTypeface)
            .withCallToActionTextTypeface(ctaTypeface)
            .build();

    templateView.setStyles(styles);

    assertThat(primary.getCurrentTextColor()).isEqualTo(Color.RED);
    assertThat(secondary.getCurrentTextColor()).isEqualTo(Color.GREEN);
    assertThat(tertiary.getCurrentTextColor()).isEqualTo(Color.BLUE);
    assertThat(callToAction.getCurrentTextColor()).isEqualTo(Color.MAGENTA);

    assertThat(primary.getTextSize()).isEqualTo(12f);
    assertThat(secondary.getTextSize()).isEqualTo(14f);
    assertThat(tertiary.getTextSize()).isEqualTo(16f);
    assertThat(callToAction.getTextSize()).isEqualTo(18f);

    assertThat(primary.getBackground()).isEqualTo(primaryBg);
    assertThat(secondary.getBackground()).isEqualTo(secondaryBg);
    assertThat(tertiary.getBackground()).isEqualTo(tertiaryBg);
    assertThat(callToAction.getBackground()).isEqualTo(ctaBg);

    assertThat(primary.getTypeface()).isEqualTo(primaryTypeface);
    assertThat(secondary.getTypeface()).isEqualTo(secondaryTypeface);
    assertThat(tertiary.getTypeface()).isEqualTo(tertiaryTypeface);
    assertThat(callToAction.getTypeface()).isEqualTo(ctaTypeface);
  }

  @Test
  public void constructors_verifyMultiParameterInstantiationWithNull() {
    TemplateView view1 = new TemplateView(context, null);
    TemplateView view2 = new TemplateView(context, null, 0);
    TemplateView view3 = new TemplateView(context, null, 0, 0);
    assertThat(view1).isNotNull();
    assertThat(view2).isNotNull();
    assertThat(view3).isNotNull();
  }

  @Test
  public void initView_withCustomContextAndRealAttributes_setsLayoutInflaterAndInflates()
      throws Exception {
    CustomLayoutInflater customInflater = new CustomLayoutInflater(context);
    CustomContextWrapper customContext = new CustomContextWrapper(context, customInflater);
    AttributeSet attrs = getLayoutAttributeSet(R.layout.nextgen_medium_template_view);

    TemplateView view = new TemplateView(customContext, attrs);

    assertThat(customInflater.inflatedTemplateType)
        .isEqualTo(R.layout.nextgen_medium_template_view);
    assertThat(customInflater.inflatedInto).isEqualTo(view);
  }

  @Test
  public void initView_whenGetResourceIdThrows_logsAndRethrowsNatively() throws Exception {
    AttributeSet mockAttrs = mock(AttributeSet.class);
    when(mockAttrs.getAttributeCount()).thenReturn(1);
    when(mockAttrs.getAttributeName(0)).thenReturn("gnt_template_type");
    when(mockAttrs.getAttributeValue(anyInt())).thenReturn("garbage");

    assertThrows(
        RuntimeException.class,
        () -> {
          TemplateView unused = new TemplateView(context, mockAttrs);
        });
  }

  private static class CustomLayoutInflater extends LayoutInflater {
    int inflatedTemplateType = -1;
    ViewGroup inflatedInto = null;

    CustomLayoutInflater(Context context) {
      super(context);
    }

    @Override
    public LayoutInflater cloneInContext(Context newContext) {
      return this;
    }

    @Override
    public View inflate(int resource, ViewGroup root, boolean attachToRoot) {
      this.inflatedTemplateType = resource;
      this.inflatedInto = root;
      return root;
    }

    @Override
    public View inflate(int resource, ViewGroup root) {
      this.inflatedTemplateType = resource;
      this.inflatedInto = root;
      return root;
    }
  }

  private static class CustomContextWrapper extends ContextWrapper {
    private final CustomLayoutInflater customInflater;

    CustomContextWrapper(Context base, CustomLayoutInflater customInflater) {
      super(base);
      this.customInflater = customInflater;
    }

    @Override
    public Object getSystemService(String name) {
      if (name.equals(Context.LAYOUT_INFLATER_SERVICE)) {
        return customInflater;
      }
      return super.getSystemService(name);
    }
  }

  @Test
  public void onFinishInflate_bindsSubviewsAndDisablesRatingBar() throws Exception {
    NativeAdView nativeAd = new NativeAdView(context);
    nativeAd.setId(R.id.native_ad_view);

    TextView primary = new TextView(context);
    primary.setId(R.id.primary);

    TextView secondary = new TextView(context);
    secondary.setId(R.id.secondary);

    TextView tertiary = new TextView(context);
    tertiary.setId(R.id.body);

    RatingBar rating = new RatingBar(context);
    rating.setId(R.id.rating_bar);

    Button cta = new Button(context);
    cta.setId(R.id.cta);

    ImageView icon = new ImageView(context);
    icon.setId(R.id.ad_icon);

    MediaView media = new MediaView(context);
    media.setId(R.id.media_view);

    ConstraintLayout bg = new ConstraintLayout(context);
    bg.setId(R.id.ad_background);

    templateView.addView(nativeAd);
    templateView.addView(primary);
    templateView.addView(secondary);
    templateView.addView(tertiary);
    templateView.addView(rating);
    templateView.addView(cta);
    templateView.addView(icon);
    templateView.addView(media);
    templateView.addView(bg);

    templateView.onFinishInflate();

    assertThat(templateView.getNativeAdView()).isEqualTo(nativeAd);
    assertThat(getPrivateField("primaryView")).isEqualTo(primary);
    assertThat(getPrivateField("secondaryView")).isEqualTo(secondary);
    assertThat(getPrivateField("tertiaryView")).isEqualTo(tertiary);
    assertThat(getPrivateField("ratingBar")).isEqualTo(rating);
    assertThat(rating.isEnabled()).isFalse();
    assertThat(getPrivateField("callToActionView")).isEqualTo(cta);
    assertThat(getPrivateField("iconView")).isEqualTo(icon);
    assertThat(getPrivateField("mediaView")).isEqualTo(media);
    assertThat(getPrivateField("background")).isEqualTo(bg);
  }

  @Test
  public void fiveParameterConstructor_verifySuccessfulInstantiation() throws Exception {
    LayoutInflater mockInflater = mock(LayoutInflater.class);
    TemplateView view = new TemplateView(context, null, 0, 0, mockInflater);

    Field field = TemplateView.class.getDeclaredField("layoutInflater");
    field.setAccessible(true);
    assertThat(field.get(view)).isEqualTo(mockInflater);
  }

  @Test
  public void getNativeAdView_returnsSetNativeAdView() throws Exception {
    NativeAdView mockView = new NativeAdView(context);
    setField("nativeAdView", mockView);

    assertThat(templateView.getNativeAdView()).isEqualTo(mockView);
  }

  @Test
  public void setNativeAd_withStarRatingButNullRatingBar_doesNotCrash() throws Exception {
    setUpStandardViews();
    setField("ratingBar", null);

    NativeAd mockAd = setUpMockNativeAd();
    when(mockAd.getStarRating()).thenReturn(4.5);

    templateView.setNativeAd(mockAd);

    assertThat(secondaryView.getVisibility()).isEqualTo(View.GONE);
  }

  @Test
  public void setNativeAd_whenViewsAreNotInitialized_returnsEarlyAndDoesNotBind() throws Exception {
    iconView = new ImageView(context);
    setField("iconView", iconView);

    NativeAd mockAd = setUpMockNativeAd();

    templateView.setNativeAd(mockAd);

    assertThat(iconView.getVisibility()).isEqualTo(View.VISIBLE);
  }

  private Object getPrivateField(String name) throws Exception {
    Field field = TemplateView.class.getDeclaredField(name);
    field.setAccessible(true);
    return field.get(templateView);
  }

  private void setField(String name, Object value) throws Exception {
    Field field = TemplateView.class.getDeclaredField(name);
    field.setAccessible(true);
    field.set(templateView, value);
  }
}
