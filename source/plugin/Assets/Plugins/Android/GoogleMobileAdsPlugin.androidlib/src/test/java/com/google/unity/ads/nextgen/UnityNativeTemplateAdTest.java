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
import static java.util.concurrent.TimeUnit.SECONDS;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.anyInt;
import static org.mockito.ArgumentMatchers.nullable;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.timeout;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;
import static org.robolectric.Shadows.shadowOf;

import android.app.Activity;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageManager;
import android.graphics.Rect;
import android.os.Bundle;
import android.view.DisplayCutout;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.view.WindowInsets;
import android.widget.FrameLayout;
import com.google.android.libraries.ads.mobile.sdk.banner.AdSize;
import com.google.android.libraries.ads.mobile.sdk.common.AdValue;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.common.PrecisionType;
import com.google.android.libraries.ads.mobile.sdk.common.ResponseInfo;
import com.google.android.libraries.ads.mobile.sdk.initialization.InitializationConfig;
import com.google.android.libraries.ads.mobile.sdk.internal.GmaComponent;
import com.google.android.libraries.ads.mobile.sdk.nativead.NativeAd;
import com.google.android.libraries.ads.mobile.sdk.nativead.NativeAdEventCallback;
import com.google.android.libraries.ads.mobile.sdk.nativead.NativeAdLoaderCallback;
import com.google.android.libraries.ads.mobile.sdk.nativead.NativeAdRequest;
import com.google.common.collect.ImmutableList;
import com.google.unity.ads.PluginUtils;
import com.google.unity.ads.nativead.UnityNativeTemplateStyle;
import com.google.unity.ads.nativead.UnityNativeTemplateType;
import java.lang.reflect.Field;
import java.lang.reflect.Method;
import java.util.Collections;
import java.util.concurrent.CountDownLatch;
import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.ArgumentCaptor;
import org.mockito.Mock;
import org.mockito.Mockito;
import org.mockito.junit.MockitoJUnit;
import org.mockito.junit.MockitoRule;
import org.robolectric.Robolectric;
import org.robolectric.RobolectricTestRunner;
import org.robolectric.annotation.Config;
import org.robolectric.shadows.ShadowLooper;

/** Unit tests for {@link UnityNativeTemplateAd}. */
@RunWith(RobolectricTestRunner.class)
public final class UnityNativeTemplateAdTest {

  private static final String TEST_AD_UNIT = "test-ad-unit";
  private static final int HORIZONTAL_OFFSET = 15;
  private static final int VERTICAL_OFFSET = 25;
  private static final int CUSTOM_WIDTH = 320;
  private static final int CUSTOM_HEIGHT = 50;

  @Rule public final MockitoRule mocks = MockitoJUnit.rule();

  @Mock private UnityNativeTemplateAdCallback mockCallback;
  @Mock private NativeAd mockNativeAd;
  @Mock private UnityNativeTemplateAd.NativeAdLoaderWrapper mockNativeAdLoader;

  private TestActivity activity;
  private UnityNativeTemplateAd unityNativeTemplateAd;

  @Before
  public void setUp() throws Exception {
    activity = Robolectric.buildActivity(TestActivity.class).create().get();

    ApplicationInfo appInfo =
        activity
            .getPackageManager()
            .getApplicationInfo(activity.getPackageName(), PackageManager.GET_META_DATA);
    if (appInfo.metaData == null) {
      appInfo.metaData = new Bundle();
    }
    appInfo.metaData.putString(
        "com.google.android.gms.ads.APPLICATION_ID", "ca-app-pub-3940256099942544~3347511713");

    InitializationConfig config =
        new InitializationConfig.Builder("ca-app-pub-3940256099942544~3347511713").build();

    @SuppressWarnings("unused")
    GmaComponent unusedComponent =
        GmaComponent.Companion.getInstance(activity, config, activity.getClass());

    unityNativeTemplateAd = new UnityNativeTemplateAd(activity, mockCallback);
    Field nativeAdField = UnityNativeTemplateAd.class.getDeclaredField("nativeAd");
    nativeAdField.setAccessible(true);
    nativeAdField.set(unityNativeTemplateAd, mockNativeAd);
    Method method =
        UnityNativeTemplateAd.class.getDeclaredMethod("setAdEventsListener", NativeAd.class);
    method.setAccessible(true);
    method.invoke(unityNativeTemplateAd, mockNativeAd);
  }

  @Test
  public void testAdEventsListener_onAdClicked() {
    NativeAdEventCallback eventCallback = captureEventCallback();
    eventCallback.onAdClicked();
    verify(mockCallback).onAdClicked();
  }

  @Test
  public void testAdEventsListener_onAdImpression() {
    NativeAdEventCallback eventCallback = captureEventCallback();
    eventCallback.onAdImpression();
    verify(mockCallback).onAdImpression();
  }

  @Test
  public void testAdEventsListener_onAdShowedFullScreenContent() {
    NativeAdEventCallback eventCallback = captureEventCallback();
    eventCallback.onAdShowedFullScreenContent();
    verify(mockCallback).onAdShowedFullScreenContent();
  }

  @Test
  public void testAdEventsListener_onAdDismissedFullScreenContent() {
    NativeAdEventCallback eventCallback = captureEventCallback();
    eventCallback.onAdDismissedFullScreenContent();
    verify(mockCallback).onAdDismissedFullScreenContent();
  }

  @Test
  public void testAdEventsListener_onAdPaid() throws Exception {
    NativeAdEventCallback eventCallback = captureEventCallback();

    AdValue realAdValue = new AdValue(PrecisionType.ESTIMATED, 123456L, "USD");

    eventCallback.onAdPaid(realAdValue);

    verify(mockCallback, timeout(1000))
        .onPaidEvent(Util.getAdValuePrecisionType(PrecisionType.ESTIMATED), 123456L, "USD");
  }

  @Test
  public void testGetPlacementId_returnsPreviouslySetPlacementId() {
    long placementId = 12345L;
    when(mockNativeAd.getPlacementId()).thenReturn(placementId);

    unityNativeTemplateAd.setPlacementId(placementId);
    assertThat(unityNativeTemplateAd.getPlacementId()).isEqualTo(placementId);
    verify(mockNativeAd).setPlacementId(placementId);
  }

  @Test
  public void testShow_updatesVisibilityTrue() throws Exception {
    TemplateView realTemplateView = new TemplateView(activity);
    realTemplateView.setVisibility(View.GONE);
    setPrivateTemplateView(realTemplateView);

    unityNativeTemplateAd.show();
    ShadowLooper.idleMainLooper();

    assertThat(realTemplateView.getVisibility()).isEqualTo(View.VISIBLE);
  }

  @Test
  public void testHide_updatesVisibilityFalse() throws Exception {
    TemplateView realTemplateView = new TemplateView(activity);
    setPrivateTemplateView(realTemplateView);

    unityNativeTemplateAd.hide();
    ShadowLooper.idleMainLooper();

    assertThat(realTemplateView.getVisibility()).isEqualTo(View.GONE);
  }

  @Test
  public void testGetSizingInPixels_returnsDimensions() throws Exception {
    TemplateView realTemplateView = new TemplateView(activity);
    realTemplateView.layout(0, 0, 200, 100);
    setPrivateTemplateView(realTemplateView);

    assertThat(unityNativeTemplateAd.getHeightInPixels()).isEqualTo(100f);
    assertThat(unityNativeTemplateAd.getWidthInPixels()).isEqualTo(200f);
  }

  @Test
  public void testDestroy_cleansUpComponents() throws Exception {
    TemplateView realTemplateView = new TemplateView(activity);
    FrameLayout parent = new FrameLayout(activity);
    parent.addView(realTemplateView);
    setPrivateTemplateView(realTemplateView);

    unityNativeTemplateAd.destroy();
    ShadowLooper.idleMainLooper();

    assertThat(realTemplateView.getParent()).isNull();
  }

  @Test
  public void testGetLayoutParams_customPositionAndOffsets() throws Exception {

    setPrivatePositionFields(PluginUtils.POSITION_CUSTOM, HORIZONTAL_OFFSET, VERTICAL_OFFSET);

    FrameLayout.LayoutParams params = unityNativeTemplateAd.getLayoutParams();

    int expectedLeft = (int) PluginUtils.convertDpToPixel(HORIZONTAL_OFFSET);
    int expectedTop = (int) PluginUtils.convertDpToPixel(VERTICAL_OFFSET);
    assertThat(params.leftMargin).isEqualTo(expectedLeft);
    assertThat(params.topMargin).isEqualTo(expectedTop);
  }

  @Test
  public void testGetLayoutParams_standardPositionCodes() throws Exception {

    setPrivatePositionFields(0, 0, 0);
    assertThat(unityNativeTemplateAd.getLayoutParams().gravity)
        .isEqualTo(Gravity.TOP | Gravity.CENTER_HORIZONTAL);

    setPrivatePositionFields(1, 0, 0);
    assertThat(unityNativeTemplateAd.getLayoutParams().gravity)
        .isEqualTo(Gravity.BOTTOM | Gravity.CENTER_HORIZONTAL);

    setPrivatePositionFields(2, 0, 0);
    assertThat(unityNativeTemplateAd.getLayoutParams().gravity)
        .isEqualTo(Gravity.TOP | Gravity.LEFT);

    setPrivatePositionFields(3, 0, 0);
    assertThat(unityNativeTemplateAd.getLayoutParams().gravity)
        .isEqualTo(Gravity.TOP | Gravity.RIGHT);
  }

  @Test
  public void testGetResponseInfo_returnsResponseInfo() {
    ResponseInfo realResponseInfo =
        new ResponseInfo("adapter", "responseId", new Bundle(), null, Collections.emptyList());
    when(mockNativeAd.getResponseInfo()).thenReturn(realResponseInfo);

    assertThat(unityNativeTemplateAd.getResponseInfo()).isEqualTo(realResponseInfo);
  }

  @Test
  public void testGetResponseInfo_whenExecutionExceptionThrown_returnsNullAndLogsError() {
    when(mockNativeAd.getResponseInfo())
        .thenThrow(new RuntimeException("Test execution exception"));

    ResponseInfo info = unityNativeTemplateAd.getResponseInfo();
    assertThat(info).isNull();
  }

  @Test
  public void testTwoParameterConstructor() throws Exception {
    UnityNativeTemplateAd client = new UnityNativeTemplateAd(activity, mockCallback);
    assertThat(client.getPlacementId()).isEqualTo(0L);

    Field horizField = UnityNativeTemplateAd.class.getDeclaredField("horizontalOffset");
    Field vertField = UnityNativeTemplateAd.class.getDeclaredField("verticalOffset");
    horizField.setAccessible(true);
    vertField.setAccessible(true);

    assertThat(horizField.get(client)).isEqualTo(0);
    assertThat(vertField.get(client)).isEqualTo(0);
  }

  @Test
  public void testDestroy_whenTemplateViewIsNull() throws Exception {
    setPrivateTemplateView(null);
    unityNativeTemplateAd.destroy();
    ShadowLooper.idleMainLooper();
    assertThat(getPrivateTemplateView()).isNull();
  }

  @Test
  @Config(sdk = 29)
  public void testGetInsets_withDisplayCutout_returnsSafeInsets() throws Exception {
    DisplayCutout realCutout = new DisplayCutout(new Rect(20, 10, 40, 30), Collections.emptyList());

    WindowInsets realInsets = new WindowInsets.Builder().setDisplayCutout(realCutout).build();

    View realDecorView =
        new View(activity) {
          @Override
          public WindowInsets getRootWindowInsets() {
            return realInsets;
          }
        };

    Window mockWindow = mock(Window.class);
    when(mockWindow.getDecorView()).thenReturn(realDecorView);

    Field windowField = Activity.class.getDeclaredField("mWindow");
    windowField.setAccessible(true);
    windowField.set(activity, mockWindow);

    setPrivatePositionFields(0, 0, 0); // POSITION_TOP
    FrameLayout.LayoutParams params = unityNativeTemplateAd.getLayoutParams();

    assertThat(params.leftMargin).isEqualTo(20);
    assertThat(params.topMargin).isEqualTo(10);
    assertThat(params.rightMargin).isEqualTo(40);
    assertThat(params.bottomMargin).isEqualTo(30);
  }

  @Test
  public void testGetLayoutParams_defaultMarginsWithoutCutout() throws Exception {
    setPrivatePositionFields(0, 0, 0); // POSITION_TOP
    FrameLayout.LayoutParams params = unityNativeTemplateAd.getLayoutParams();
    assertThat(params.leftMargin).isEqualTo(0);
    assertThat(params.rightMargin).isEqualTo(0);
    assertThat(params.topMargin).isEqualTo(0);
    assertThat(params.bottomMargin).isEqualTo(0);
  }

  @Test
  @Config(sdk = 29)
  public void testGetLayoutParams_marginsWithoutCutoutOnSdk29_returnsZeroMargins()
      throws Exception {
    setPrivatePositionFields(0, 0, 0); // POSITION_TOP
    FrameLayout.LayoutParams params = unityNativeTemplateAd.getLayoutParams();
    assertThat(params.leftMargin).isEqualTo(0);
    assertThat(params.rightMargin).isEqualTo(0);
    assertThat(params.topMargin).isEqualTo(0);
    assertThat(params.bottomMargin).isEqualTo(0);
  }

  @Test
  @Config(sdk = 29)
  public void testGetLayoutParams_marginsWithNonNullInsetsAndNullCutout_returnsZeroMargins()
      throws Exception {
    WindowInsets realInsets = new WindowInsets.Builder().setDisplayCutout(null).build();

    View realDecorView =
        new View(activity) {
          @Override
          public WindowInsets getRootWindowInsets() {
            return realInsets;
          }
        };

    Window mockWindow = mock(Window.class);
    when(mockWindow.getDecorView()).thenReturn(realDecorView);

    Field windowField = Activity.class.getDeclaredField("mWindow");
    windowField.setAccessible(true);
    windowField.set(activity, mockWindow);

    setPrivatePositionFields(0, 0, 0); // POSITION_TOP
    FrameLayout.LayoutParams params = unityNativeTemplateAd.getLayoutParams();

    assertThat(params.leftMargin).isEqualTo(0);
    assertThat(params.rightMargin).isEqualTo(0);
    assertThat(params.topMargin).isEqualTo(0);
    assertThat(params.bottomMargin).isEqualTo(0);
  }

  @Test
  @Config(sdk = 29)
  public void testGetLayoutParams_bottomPositionWithCutout_doesNotApplyTopInset() throws Exception {
    DisplayCutout realCutout = new DisplayCutout(new Rect(20, 10, 40, 30), Collections.emptyList());
    WindowInsets realInsets = new WindowInsets.Builder().setDisplayCutout(realCutout).build();

    View realDecorView =
        new View(activity) {
          @Override
          public WindowInsets getRootWindowInsets() {
            return realInsets;
          }
        };

    Window mockWindow = mock(Window.class);
    when(mockWindow.getDecorView()).thenReturn(realDecorView);

    Field windowField = Activity.class.getDeclaredField("mWindow");
    windowField.setAccessible(true);
    windowField.set(activity, mockWindow);

    setPrivatePositionFields(PluginUtils.POSITION_BOTTOM, 0, 0);
    FrameLayout.LayoutParams params = unityNativeTemplateAd.getLayoutParams();

    assertThat(params.topMargin).isEqualTo(0);
    assertThat(params.leftMargin).isEqualTo(20);
  }

  @Test
  @Config(sdk = 29)
  public void testGetInsets_whenWindowIsNull_returnsZeroInsets() throws Exception {
    Field windowField = Activity.class.getDeclaredField("mWindow");
    windowField.setAccessible(true);
    windowField.set(activity, null);

    Method method = UnityNativeTemplateAd.class.getDeclaredMethod("getInsets");
    method.setAccessible(true);
    Object insets = method.invoke(unityNativeTemplateAd);

    Field top = insets.getClass().getDeclaredField("top");
    Field left = insets.getClass().getDeclaredField("left");
    Field bottom = insets.getClass().getDeclaredField("bottom");
    Field right = insets.getClass().getDeclaredField("right");
    top.setAccessible(true);
    left.setAccessible(true);
    bottom.setAccessible(true);
    right.setAccessible(true);

    assertThat(top.get(insets)).isEqualTo(0);
    assertThat(left.get(insets)).isEqualTo(0);
    assertThat(bottom.get(insets)).isEqualTo(0);
    assertThat(right.get(insets)).isEqualTo(0);
  }

  @Test
  public void testSetPositionCode_updatesPositionCode() throws Exception {
    unityNativeTemplateAd.setPositionCode(1); // POSITION_BOTTOM
    ShadowLooper.idleMainLooper();

    Field field = UnityNativeTemplateAd.class.getDeclaredField("positionCode");
    field.setAccessible(true);
    assertThat(field.get(unityNativeTemplateAd)).isEqualTo(1);
  }

  @Test
  public void testSetPosition_updatesOffsets() throws Exception {
    unityNativeTemplateAd.setPosition(50, 60);
    ShadowLooper.idleMainLooper();

    Field posCode = UnityNativeTemplateAd.class.getDeclaredField("positionCode");
    posCode.setAccessible(true);
    assertThat(posCode.get(unityNativeTemplateAd)).isEqualTo(PluginUtils.POSITION_CUSTOM);

    Field horiz = UnityNativeTemplateAd.class.getDeclaredField("horizontalOffset");
    horiz.setAccessible(true);
    assertThat(horiz.get(unityNativeTemplateAd)).isEqualTo(50);

    Field vert = UnityNativeTemplateAd.class.getDeclaredField("verticalOffset");
    vert.setAccessible(true);
    assertThat(vert.get(unityNativeTemplateAd)).isEqualTo(60);
  }

  @Test
  public void testLayoutChangeListener_triggersUpdatePosition() throws Exception {
    TemplateView realTemplateView = new TemplateView(activity);
    setPrivateTemplateView(realTemplateView);

    unityNativeTemplateAd.setLayoutChangeListener();

    Field field = UnityNativeTemplateAd.class.getDeclaredField("layoutChangeListener");
    field.setAccessible(true);
    View.OnLayoutChangeListener listener =
        (View.OnLayoutChangeListener) field.get(unityNativeTemplateAd);

    View rootView = activity.getWindow().getDecorView().getRootView();
    assertThat(shadowOf(rootView).getOnLayoutChangeListeners()).contains(listener);

    listener.onLayoutChange(new View(activity), 0, 0, 100, 100, 0, 0, 200, 200);
    ShadowLooper.idleMainLooper();

    ViewGroup.LayoutParams params = realTemplateView.getLayoutParams();
    assertThat(params).isNotNull();
  }

  @Test
  public void testInitializeTemplateView_inflatesSuccessfully() throws Exception {
    TemplateView realTemplateView = setUpMockLayoutInflater();
    UnityNativeTemplateStyle templateStyle = createDefaultTemplateStyle();

    Method method =
        UnityNativeTemplateAd.class.getDeclaredMethod(
            "initializeTemplateView", int.class, UnityNativeTemplateStyle.class);
    method.setAccessible(true);
    method.invoke(unityNativeTemplateAd, 12345, templateStyle);

    assertThat(getPrivateTemplateView()).isEqualTo(realTemplateView);
    assertThat(realTemplateView.getStyles()).isNotNull();

    Field nativeAdField = TemplateView.class.getDeclaredField("nativeAd");
    nativeAdField.setAccessible(true);
    assertThat(nativeAdField.get(realTemplateView)).isEqualTo(mockNativeAd);
  }

  @Test
  public void testRemoveTemplateView_removesFromParent() throws Exception {
    TemplateView realTemplateView = setUpMockLayoutInflater();
    UnityNativeTemplateStyle templateStyle = createDefaultTemplateStyle();

    Method initMethod =
        UnityNativeTemplateAd.class.getDeclaredMethod(
            "initializeTemplateView", int.class, UnityNativeTemplateStyle.class);
    initMethod.setAccessible(true);
    initMethod.invoke(unityNativeTemplateAd, 12345, templateStyle);

    FrameLayout parent = new FrameLayout(activity);
    parent.addView(realTemplateView);

    Method removeMethod = UnityNativeTemplateAd.class.getDeclaredMethod("removeTemplateView");
    removeMethod.setAccessible(true);
    removeMethod.invoke(unityNativeTemplateAd);
    ShadowLooper.idleMainLooper();

    assertThat(realTemplateView.getParent()).isNull();
  }

  @Test
  public void testHide_whenTemplateViewIsNull_returnsEarly() throws Exception {
    setPrivateTemplateView(null);
    unityNativeTemplateAd.hide();
    ShadowLooper.idleMainLooper();
  }

  @Test
  public void testGetHeightInPixels_whenTemplateViewIsNull_returnsZero() throws Exception {
    setPrivateTemplateView(null);
    assertThat(unityNativeTemplateAd.getHeightInPixels()).isEqualTo(0f);
    assertThat(unityNativeTemplateAd.getWidthInPixels()).isEqualTo(0f);
  }

  @Test
  public void testSetLayoutChangeListener_whenAlreadySet_returnsEarly() throws Exception {
    unityNativeTemplateAd.setLayoutChangeListener();

    Field field = UnityNativeTemplateAd.class.getDeclaredField("layoutChangeListener");
    field.setAccessible(true);
    View.OnLayoutChangeListener original =
        (View.OnLayoutChangeListener) field.get(unityNativeTemplateAd);
    assertThat(original).isNotNull();

    unityNativeTemplateAd.setLayoutChangeListener();
    View.OnLayoutChangeListener current =
        (View.OnLayoutChangeListener) field.get(unityNativeTemplateAd);
    assertThat(current).isSameInstanceAs(original);
  }

  @Test
  public void testLayoutChangeListener_whenBoundsSame_returnsEarly() throws Exception {
    TemplateView realTemplateView = new TemplateView(activity);
    setPrivateTemplateView(realTemplateView);

    unityNativeTemplateAd.setLayoutChangeListener();

    Field field = UnityNativeTemplateAd.class.getDeclaredField("layoutChangeListener");
    field.setAccessible(true);
    View.OnLayoutChangeListener listener =
        (View.OnLayoutChangeListener) field.get(unityNativeTemplateAd);

    listener.onLayoutChange(new View(activity), 0, 0, 100, 100, 0, 0, 100, 100);
    ShadowLooper.idleMainLooper();
    assertThat(realTemplateView.getLayoutParams()).isNull();
  }

  @Test
  public void testLayoutChangeListener_withCustomAdSize_updatesHeightAndWidth() throws Exception {
    TemplateView realTemplateView = new TemplateView(activity);
    setPrivateTemplateView(realTemplateView);

    Field sizeField = UnityNativeTemplateAd.class.getDeclaredField("adSize");
    sizeField.setAccessible(true);
    AdSize customSize = new AdSize(300, 250);
    sizeField.set(unityNativeTemplateAd, customSize);

    unityNativeTemplateAd.setLayoutChangeListener();

    Field field = UnityNativeTemplateAd.class.getDeclaredField("layoutChangeListener");
    field.setAccessible(true);
    View.OnLayoutChangeListener listener =
        (View.OnLayoutChangeListener) field.get(unityNativeTemplateAd);

    listener.onLayoutChange(new View(activity), 0, 0, 100, 100, 0, 0, 200, 200);
    ShadowLooper.idleMainLooper();

    ViewGroup.LayoutParams params = realTemplateView.getLayoutParams();
    assertThat(params).isNotNull();
    assertThat(params.height).isEqualTo(250);
    assertThat(params.width).isEqualTo(300);
  }

  @Test
  public void testRenderDefaultSizeAtPositionCode_succeeds() throws Exception {
    Field size = UnityNativeTemplateAd.class.getDeclaredField("adSize");
    size.setAccessible(true);
    size.set(unityNativeTemplateAd, new AdSize(100, 50));

    TemplateView realTemplateView = setUpMockLayoutInflater();
    UnityNativeTemplateStyle templateStyle = createDefaultTemplateStyle();

    unityNativeTemplateAd.renderDefaultSizeAtPositionCode(templateStyle, 1);
    ShadowLooper.idleMainLooper();

    assertThat(getPrivateTemplateView()).isEqualTo(realTemplateView);

    Field posCode = UnityNativeTemplateAd.class.getDeclaredField("positionCode");
    posCode.setAccessible(true);

    assertThat(posCode.get(unityNativeTemplateAd)).isEqualTo(1);
    assertThat(size.get(unityNativeTemplateAd)).isNull();
  }

  @Test
  public void testRenderDefaultSizeAtPositionCode_removesPreviousTemplateView() throws Exception {
    TemplateView firstTemplateView = setupAndAttachPrivateTemplateView();

    TemplateView secondTemplateView = setUpMockLayoutInflater();
    UnityNativeTemplateStyle templateStyle = createDefaultTemplateStyle();

    unityNativeTemplateAd.renderDefaultSizeAtPositionCode(templateStyle, 1);
    ShadowLooper.idleMainLooper();

    assertThat(firstTemplateView.getParent()).isNull();
    assertThat(getPrivateTemplateView()).isEqualTo(secondTemplateView);
  }

  @Test
  public void testRenderDefaultSizeAtPosition_succeeds() throws Exception {
    Field size = UnityNativeTemplateAd.class.getDeclaredField("adSize");
    size.setAccessible(true);
    size.set(unityNativeTemplateAd, new AdSize(100, 50));

    TemplateView realTemplateView = setUpMockLayoutInflater();
    UnityNativeTemplateStyle templateStyle = createDefaultTemplateStyle();

    unityNativeTemplateAd.renderDefaultSizeAtPosition(
        templateStyle, HORIZONTAL_OFFSET, VERTICAL_OFFSET);
    ShadowLooper.idleMainLooper();

    assertThat(getPrivateTemplateView()).isEqualTo(realTemplateView);

    Field posCode = UnityNativeTemplateAd.class.getDeclaredField("positionCode");
    Field horiz = UnityNativeTemplateAd.class.getDeclaredField("horizontalOffset");
    Field vert = UnityNativeTemplateAd.class.getDeclaredField("verticalOffset");
    posCode.setAccessible(true);
    horiz.setAccessible(true);
    vert.setAccessible(true);

    assertThat(posCode.get(unityNativeTemplateAd)).isEqualTo(PluginUtils.POSITION_CUSTOM);
    assertThat(horiz.get(unityNativeTemplateAd)).isEqualTo(HORIZONTAL_OFFSET);
    assertThat(vert.get(unityNativeTemplateAd)).isEqualTo(VERTICAL_OFFSET);
    assertThat(size.get(unityNativeTemplateAd)).isNull();
  }

  @Test
  public void testRenderDefaultSizeAtPosition_removesPreviousTemplateView() throws Exception {
    TemplateView firstTemplateView = setupAndAttachPrivateTemplateView();

    TemplateView secondTemplateView = setUpMockLayoutInflater();
    UnityNativeTemplateStyle templateStyle = createDefaultTemplateStyle();

    unityNativeTemplateAd.renderDefaultSizeAtPosition(
        templateStyle, HORIZONTAL_OFFSET, VERTICAL_OFFSET);
    ShadowLooper.idleMainLooper();

    assertThat(firstTemplateView.getParent()).isNull();
    assertThat(getPrivateTemplateView()).isEqualTo(secondTemplateView);
  }

  @Test
  public void testRenderCustomSizeAtPositionCode_succeeds() throws Exception {
    TemplateView realTemplateView = setUpMockLayoutInflater();
    UnityNativeTemplateStyle templateStyle = createDefaultTemplateStyle();

    AdSize customSize = new AdSize(CUSTOM_WIDTH, CUSTOM_HEIGHT);

    unityNativeTemplateAd.renderCustomSizeAtPositionCode(templateStyle, customSize, 1);
    ShadowLooper.idleMainLooper();

    assertThat(getPrivateTemplateView()).isEqualTo(realTemplateView);

    Field posCode = UnityNativeTemplateAd.class.getDeclaredField("positionCode");
    Field size = UnityNativeTemplateAd.class.getDeclaredField("adSize");
    posCode.setAccessible(true);
    size.setAccessible(true);

    assertThat(posCode.get(unityNativeTemplateAd)).isEqualTo(1);
    assertThat(size.get(unityNativeTemplateAd)).isEqualTo(customSize);
  }

  @Test
  public void testRenderCustomSizeAtPositionCode_removesPreviousTemplateView() throws Exception {
    TemplateView firstTemplateView = setupAndAttachPrivateTemplateView();

    TemplateView secondTemplateView = setUpMockLayoutInflater();
    UnityNativeTemplateStyle templateStyle = createDefaultTemplateStyle();
    AdSize customSize = new AdSize(CUSTOM_WIDTH, CUSTOM_HEIGHT);

    unityNativeTemplateAd.renderCustomSizeAtPositionCode(templateStyle, customSize, 1);
    ShadowLooper.idleMainLooper();

    assertThat(firstTemplateView.getParent()).isNull();
    assertThat(getPrivateTemplateView()).isEqualTo(secondTemplateView);
  }

  @Test
  public void testRenderCustomSizeAtPosition_succeeds() throws Exception {
    TemplateView realTemplateView = setUpMockLayoutInflater();
    UnityNativeTemplateStyle templateStyle = createDefaultTemplateStyle();

    AdSize customSize = new AdSize(CUSTOM_WIDTH, CUSTOM_HEIGHT);

    unityNativeTemplateAd.renderCustomSizeAtPosition(
        templateStyle, customSize, HORIZONTAL_OFFSET, VERTICAL_OFFSET);
    ShadowLooper.idleMainLooper();

    assertThat(getPrivateTemplateView()).isEqualTo(realTemplateView);

    Field posCode = UnityNativeTemplateAd.class.getDeclaredField("positionCode");
    Field horiz = UnityNativeTemplateAd.class.getDeclaredField("horizontalOffset");
    Field vert = UnityNativeTemplateAd.class.getDeclaredField("verticalOffset");
    Field size = UnityNativeTemplateAd.class.getDeclaredField("adSize");
    posCode.setAccessible(true);
    horiz.setAccessible(true);
    vert.setAccessible(true);
    size.setAccessible(true);

    assertThat(posCode.get(unityNativeTemplateAd)).isEqualTo(PluginUtils.POSITION_CUSTOM);
    assertThat(horiz.get(unityNativeTemplateAd)).isEqualTo(HORIZONTAL_OFFSET);
    assertThat(vert.get(unityNativeTemplateAd)).isEqualTo(VERTICAL_OFFSET);
    assertThat(size.get(unityNativeTemplateAd)).isEqualTo(customSize);
  }

  @Test
  public void testRenderCustomSizeAtPosition_removesPreviousTemplateView() throws Exception {
    TemplateView firstTemplateView = setupAndAttachPrivateTemplateView();

    TemplateView secondTemplateView = setUpMockLayoutInflater();
    UnityNativeTemplateStyle templateStyle = createDefaultTemplateStyle();
    AdSize customSize = new AdSize(CUSTOM_WIDTH, CUSTOM_HEIGHT);

    unityNativeTemplateAd.renderCustomSizeAtPosition(
        templateStyle, customSize, HORIZONTAL_OFFSET, VERTICAL_OFFSET);
    ShadowLooper.idleMainLooper();

    assertThat(firstTemplateView.getParent()).isNull();
    assertThat(getPrivateTemplateView()).isEqualTo(secondTemplateView);
  }

  @Test
  public void testLoadAd_onNativeAdLoaded_invokesCallbackAndSetsListener() {
    UnityNativeTemplateAd client =
        new UnityNativeTemplateAd(activity, mockCallback, mockNativeAdLoader);

    NativeAdRequest request =
        new NativeAdRequest.Builder(TEST_AD_UNIT, ImmutableList.of(NativeAd.NativeAdType.NATIVE))
            .build();
    client.loadAd(request);
    ShadowLooper.idleMainLooper();

    ArgumentCaptor<NativeAdRequest> requestCaptor = ArgumentCaptor.forClass(NativeAdRequest.class);
    ArgumentCaptor<NativeAdLoaderCallback> loaderCallbackCaptor =
        ArgumentCaptor.forClass(NativeAdLoaderCallback.class);
    verify(mockNativeAdLoader).load(requestCaptor.capture(), loaderCallbackCaptor.capture());

    assertThat(requestCaptor.getValue()).isSameInstanceAs(request);

    NativeAd localMockNativeAd = mock(NativeAd.class);
    loaderCallbackCaptor.getValue().onNativeAdLoaded(localMockNativeAd);

    verify(mockCallback).onNativeAdLoaded();
    verify(localMockNativeAd).setAdEventCallback(any(NativeAdEventCallback.class));
  }

  @Test
  public void testLoadAd_onAdFailedToLoad_invokesFailedCallback() {
    UnityNativeTemplateAd client =
        new UnityNativeTemplateAd(activity, mockCallback, mockNativeAdLoader);

    NativeAdRequest request =
        new NativeAdRequest.Builder(TEST_AD_UNIT, ImmutableList.of(NativeAd.NativeAdType.NATIVE))
            .build();
    client.loadAd(request);
    ShadowLooper.idleMainLooper();

    ArgumentCaptor<NativeAdRequest> requestCaptor = ArgumentCaptor.forClass(NativeAdRequest.class);
    ArgumentCaptor<NativeAdLoaderCallback> loaderCallbackCaptor =
        ArgumentCaptor.forClass(NativeAdLoaderCallback.class);
    verify(mockNativeAdLoader).load(requestCaptor.capture(), loaderCallbackCaptor.capture());

    assertThat(requestCaptor.getValue()).isSameInstanceAs(request);

    LoadAdError loadAdError =
        new LoadAdError(LoadAdError.ErrorCode.INTERNAL_ERROR, "error message", null);
    loaderCallbackCaptor.getValue().onAdFailedToLoad(loadAdError);

    verify(mockCallback).onNativeAdFailedToLoad(loadAdError);
  }

  @Test
  public void testLoadAd_loadsRequest() {
    UnityNativeTemplateAd client =
        new UnityNativeTemplateAd(activity, mockCallback, mockNativeAdLoader);

    NativeAdRequest request =
        new NativeAdRequest.Builder(TEST_AD_UNIT, ImmutableList.of(NativeAd.NativeAdType.NATIVE))
            .build();
    client.loadAd(request);
    ShadowLooper.idleMainLooper();

    ArgumentCaptor<NativeAdRequest> requestCaptor = ArgumentCaptor.forClass(NativeAdRequest.class);
    verify(mockNativeAdLoader).load(requestCaptor.capture(), any(NativeAdLoaderCallback.class));

    assertThat(requestCaptor.getValue()).isSameInstanceAs(request);
  }

  @Test
  public void testLoadAd_runsOnCallingThread_notUIThread() throws Exception {
    UnityNativeTemplateAd client =
        new UnityNativeTemplateAd(activity, mockCallback, mockNativeAdLoader);
    NativeAdRequest request =
        new NativeAdRequest.Builder(TEST_AD_UNIT, ImmutableList.of(NativeAd.NativeAdType.NATIVE))
            .build();
    final Thread[] invocationThread = new Thread[1];
    CountDownLatch latch = new CountDownLatch(1);

    Mockito.doAnswer(
            invocation -> {
              invocationThread[0] = Thread.currentThread();
              latch.countDown();
              return null;
            })
        .when(mockNativeAdLoader)
        .load(any(NativeAdRequest.class), any(NativeAdLoaderCallback.class));

    Thread backgroundThread = new Thread(() -> client.loadAd(request));
    backgroundThread.start();

    assertThat(latch.await(5, SECONDS)).isTrue();
    assertThat(invocationThread[0]).isEqualTo(backgroundThread);
    assertThat(invocationThread[0]).isNotEqualTo(activity.getMainLooper().getThread());
  }

  private TemplateView setUpMockLayoutInflater() {
    LayoutInflater mockInflater = mock(LayoutInflater.class);
    TemplateView realTemplateView = new TemplateView(activity);
    when(mockInflater.inflate(anyInt(), nullable(ViewGroup.class))).thenReturn(realTemplateView);
    activity.setInflater(mockInflater);
    return realTemplateView;
  }

  private UnityNativeTemplateStyle createDefaultTemplateStyle() {
    return new UnityNativeTemplateStyle(
        UnityNativeTemplateType.NEXTGEN_SMALL, null, null, null, null, null);
  }

  private NativeAdEventCallback captureEventCallback() {
    ArgumentCaptor<NativeAdEventCallback> eventCallbackCaptor =
        ArgumentCaptor.forClass(NativeAdEventCallback.class);
    verify(mockNativeAd).setAdEventCallback(eventCallbackCaptor.capture());
    return eventCallbackCaptor.getValue();
  }

  private void setPrivatePositionFields(int positionCode, int horizontalOffset, int verticalOffset)
      throws Exception {
    Field posCode = UnityNativeTemplateAd.class.getDeclaredField("positionCode");
    posCode.setAccessible(true);
    posCode.set(unityNativeTemplateAd, positionCode);

    Field horiz = UnityNativeTemplateAd.class.getDeclaredField("horizontalOffset");
    horiz.setAccessible(true);
    horiz.set(unityNativeTemplateAd, horizontalOffset);

    Field vert = UnityNativeTemplateAd.class.getDeclaredField("verticalOffset");
    vert.setAccessible(true);
    vert.set(unityNativeTemplateAd, verticalOffset);
  }

  private void setPrivateTemplateView(TemplateView view) throws Exception {
    Field field = UnityNativeTemplateAd.class.getDeclaredField("templateView");
    field.setAccessible(true);
    field.set(unityNativeTemplateAd, view);
  }

  private TemplateView setupAndAttachPrivateTemplateView() throws Exception {
    TemplateView firstTemplateView = new TemplateView(activity);
    FrameLayout firstParent = new FrameLayout(activity);
    firstParent.addView(firstTemplateView);
    setPrivateTemplateView(firstTemplateView);
    return firstTemplateView;
  }

  private TemplateView getPrivateTemplateView() throws Exception {
    Field field = UnityNativeTemplateAd.class.getDeclaredField("templateView");
    field.setAccessible(true);
    return (TemplateView) field.get(unityNativeTemplateAd);
  }

  private static class TestActivity extends Activity {
    private LayoutInflater inflater;

    void setInflater(LayoutInflater inflater) {
      this.inflater = inflater;
    }

    @Override
    public Object getSystemService(String name) {
      if (name.equals(LAYOUT_INFLATER_SERVICE) && inflater != null) {
        return inflater;
      }
      return super.getSystemService(name);
    }
  }
}
