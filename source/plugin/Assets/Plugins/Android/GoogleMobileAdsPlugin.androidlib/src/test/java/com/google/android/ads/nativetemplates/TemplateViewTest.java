package com.google.android.ads.nativetemplates;

import static com.google.common.truth.Truth.assertThat;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.anyInt;
import static org.mockito.Mockito.never;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import android.content.Context;
import android.content.res.XmlResourceParser;
import android.util.AttributeSet;
import android.util.Xml;
import android.view.LayoutInflater;
import android.view.ViewGroup;
import androidx.test.core.app.ApplicationProvider;
import com.google.android.gms.ads.nativead.NativeAd;
import com.google.unity.ads.R;
import org.junit.After;
import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.Mock;
import org.mockito.junit.MockitoJUnit;
import org.mockito.junit.MockitoRule;
import org.robolectric.RobolectricTestRunner;

/** Tests for {@link TemplateView} */
@RunWith(RobolectricTestRunner.class)
public final class TemplateViewTest {

  @Rule public final MockitoRule mockito = MockitoJUnit.rule();
  @Mock private NativeAd mockNativeAd;
  @Mock private LayoutInflater mockLayoutInflater;

  private Context context;
  private TemplateView templateView;

  @Before
  public void setUp() {
    context = ApplicationProvider.getApplicationContext();
    templateView = new TemplateView(context);
  }

  @After
  public void tearDown() {
    templateView.destroyNativeAd();
  }

  @Test
  public void templateView_canBeInstantiated() throws Exception {
    assertThat(templateView).isNotNull();
    assertThat(templateView.getTemplateTypeName()).isEmpty();
    assertThat(templateView.getNativeAdView()).isNull();
    assertThat(templateView.getBackground()).isNull();
    assertThat(templateView.getStyles()).isNull();
  }

  @Test
  public void setNativeAd_withoutViewInit_succeeds() throws Exception {
    // Act.
    templateView.setNativeAd(mockNativeAd);

    // Assert.
    assertThat(templateView.getNativeAdView()).isNull();
    // Mockito cannot mock final methods. And it cannot detect a final method being called either.
  }

  @Test
  public void setNativeAd_withViewInit_succeeds() throws Exception {
    // Act.
    templateView =
        new TemplateView(
            /* context= */ context,
            /* attrs= */ null,
            /* defStyleAttr= */ 0,
            /* defStyleRes= */ 0,
            /* layoutInflater= */ mockLayoutInflater);
    templateView.setNativeAd(mockNativeAd);

    // Assert.
    assertThat(templateView.getTemplateTypeName()).isEmpty();
    assertThat(templateView.getNativeAdView()).isNull();
    assertThat(templateView.getBackground()).isNull();
    assertThat(templateView.getStyles()).isNull();
    verify(mockLayoutInflater, never()).inflate(anyInt(), any());
  }

  @Test
  public void setNativeAd_withViewInitAndAttributes_succeeds() throws Exception {
    // Arrange.
    when(mockLayoutInflater.inflate(anyInt(), any())).thenReturn(null);

    // Act.
    templateView =
        new TemplateView(
            /* context= */ context,
            /* attrs= */ getFakeAttributeSet(),
            /* defStyleAttr= */ 0,
            /* defStyleRes= */ 0,
            /* layoutInflater= */ mockLayoutInflater);
    templateView.setNativeAd(mockNativeAd);

    // Assert.
    assertThat(templateView.getTemplateTypeName()).isEqualTo(TemplateView.MEDIUM_TEMPLATE);
    assertThat(templateView.getNativeAdView()).isNull();
    assertThat(templateView.getBackground()).isNull();
    assertThat(templateView.getStyles()).isNull();
    verify(mockLayoutInflater).inflate(anyInt(), any(ViewGroup.class));
  }

  private AttributeSet getFakeAttributeSet() {
    XmlResourceParser parser = context.getResources().getXml(R.layout.gnt_medium_template_view);
    return Xml.asAttributeSet(parser);
  }
}
