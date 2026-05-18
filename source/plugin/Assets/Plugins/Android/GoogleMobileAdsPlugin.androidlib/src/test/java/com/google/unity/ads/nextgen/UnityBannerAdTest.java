package com.google.unity.ads.nextgen;

import static com.google.common.truth.Truth.assertThat;
import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.atLeastOnce;
import static org.mockito.Mockito.never;
import static org.mockito.Mockito.timeout;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;
import static org.robolectric.Shadows.shadowOf;

import android.app.Activity;
import android.os.Bundle;
import android.view.Gravity;
import android.view.View;
import android.widget.FrameLayout;
import com.google.android.libraries.ads.mobile.sdk.banner.AdSize;
import com.google.android.libraries.ads.mobile.sdk.banner.BannerAd;
import com.google.android.libraries.ads.mobile.sdk.banner.BannerAdEventCallback;
import com.google.android.libraries.ads.mobile.sdk.banner.BannerAdRequest;
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.AdValue;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.common.PrecisionType;
import com.google.android.libraries.ads.mobile.sdk.common.ResponseInfo;
import com.google.unity.ads.PluginUtils;
import java.util.ArrayList;
import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.ArgumentCaptor;
import org.mockito.Captor;
import org.mockito.Mock;
import org.mockito.junit.MockitoJUnit;
import org.mockito.junit.MockitoRule;
import org.robolectric.Robolectric;
import org.robolectric.RobolectricTestRunner;
import org.robolectric.annotation.Config;
import org.robolectric.shadows.ShadowLooper;

/** Unit tests for {@link UnityBannerAd}. */
@RunWith(RobolectricTestRunner.class)
public final class UnityBannerAdTest {

  @Rule public final MockitoRule mocks = MockitoJUnit.rule();

  private Activity activity;
  @Mock private UnityBannerAdCallback mockCallback;
  @Mock private BannerAd mockBannerAd;
  @Mock private AdWrapper<BannerAd> mockAdWrapper;

  @Captor private ArgumentCaptor<AdLoadCallback<BannerAd>> adLoadCallbackCaptor;
  @Captor private ArgumentCaptor<BannerAdEventCallback> adEventCallbackCaptor;

  private UnityBannerAd unityBannerAd;
  private BannerAdRequest bannerAdRequest;

  @Before
  public void setUp() {
    activity = Robolectric.buildActivity(Activity.class).create().start().resume().visible().get();
    unityBannerAd = new UnityBannerAd(activity, mockCallback, mockAdWrapper);
    bannerAdRequest = new BannerAdRequest.Builder("test-ad-unit", AdSize.BANNER).build();
  }

  private View simulateAdLoadSuccess() {
    return simulateAdLoadSuccess(AdSize.BANNER);
  }

  private View simulateAdLoadSuccess(AdSize adSize) {
    View fakeView = new View(activity);
    when(mockBannerAd.getView(activity)).thenReturn(fakeView);
    when(mockBannerAd.getAdSize()).thenReturn(adSize);

    unityBannerAd.load(bannerAdRequest);
    verify(mockAdWrapper).load(eq(bannerAdRequest), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockBannerAd);
    return fakeView;
  }

  @Test
  public void testLoad_onAdLoaded() {
    View fakeView = simulateAdLoadSuccess();

    verify(mockCallback, timeout(1000)).onAdLoaded();

    ShadowLooper.idleMainLooper();
    verify(mockBannerAd).getView(activity);
    assertThat(fakeView.getVisibility()).isEqualTo(View.VISIBLE);
  }

  @Test
  public void testLoad_onAdFailedToLoad() {
    unityBannerAd.load(bannerAdRequest);

    verify(mockAdWrapper).load(eq(bannerAdRequest), adLoadCallbackCaptor.capture());
    LoadAdError loadAdError =
        new LoadAdError(LoadAdError.ErrorCode.INTERNAL_ERROR, "domain", /* responseInfo= */ null);
    adLoadCallbackCaptor.getValue().onAdFailedToLoad(loadAdError);

    verify(mockCallback, timeout(1000)).onAdFailedToLoad(loadAdError);
  }

  @Test
  public void testShow_whenAdLoaded_setsVisibilityAndPosition() {
    View fakeView = simulateAdLoadSuccess();

    unityBannerAd.show();
    ShadowLooper.idleMainLooper();

    verify(mockBannerAd, atLeastOnce()).getView(activity);

    assertThat(fakeView.getVisibility()).isEqualTo(View.VISIBLE);
  }

  @Test
  public void testHide_removesViewFromParent() {
    View unusedView = simulateAdLoadSuccess();

    ShadowLooper.idleMainLooper();
    assertThat(unityBannerAd.hidden).isFalse();

    unityBannerAd.hide();
    ShadowLooper.idleMainLooper();

    assertThat(unityBannerAd.hidden).isTrue();
  }

  @Test
  public void testAdEvents_invokeCallback() {
    View unusedView = simulateAdLoadSuccess();

    verify(mockBannerAd).setAdEventCallback(adEventCallbackCaptor.capture());
    BannerAdEventCallback eventCallback = adEventCallbackCaptor.getValue();

    eventCallback.onAdImpression();
    verify(mockCallback).onAdImpression();

    eventCallback.onAdClicked();
    verify(mockCallback).onAdClicked();

    eventCallback.onAdShowedFullScreenContent();
    verify(mockCallback).onAdOpened();

    eventCallback.onAdDismissedFullScreenContent();
    verify(mockCallback).onAdClosed();

    PrecisionType precisionType = PrecisionType.PRECISE;
    long valueMicros = 1000000L;
    String currencyCode = "USD";
    AdValue adValue = new AdValue(precisionType, valueMicros, currencyCode);
    eventCallback.onAdPaid(adValue);
    verify(mockCallback)
        .onPaidEvent(Util.getAdValuePrecisionType(precisionType), valueMicros, currencyCode);
  }

  @Test
  public void testGetResponseInfo_whenAdNotLoaded_returnsNull() {
    assertThat(unityBannerAd.getResponseInfo()).isNull();
  }

  @Test
  public void testGetResponseInfo_whenAdLoaded_returnsResponseInfo() {
    ResponseInfo responseInfo =
        new ResponseInfo(
            "AdapterName",
            "responseId",
            new Bundle(),
            /* loadedAdSourceResponseInfo= */ null,
            new ArrayList<>());
    when(mockBannerAd.getResponseInfo()).thenReturn(responseInfo);

    View unusedView = simulateAdLoadSuccess();

    ResponseInfo actualResponseInfo = unityBannerAd.getResponseInfo();
    verify(mockBannerAd).getResponseInfo();
    assertThat(actualResponseInfo).isEqualTo(responseInfo);
  }

  @Test
  public void testGetHeightInPixels_whenAdNotLoaded_returnsNegativeOne() {
    assertThat(unityBannerAd.getHeightInPixels()).isEqualTo(-1.0f);
  }

  @Test
  public void testGetHeightInPixels_whenAdLoaded_returnsHeight() {
    AdSize customAdSize = new AdSize(300, 250);
    View unusedView = simulateAdLoadSuccess(customAdSize);

    int expectedHeight = customAdSize.getHeightInPixels(activity);
    assertThat(unityBannerAd.getHeightInPixels()).isEqualTo(expectedHeight);
  }

  @Test
  public void testGetWidthInPixels_whenAdNotLoaded_returnsNegativeOne() {
    assertThat(unityBannerAd.getWidthInPixels()).isEqualTo(-1.0f);
  }

  @Test
  public void testGetWidthInPixels_whenAdLoaded_returnsWidth() {
    AdSize customAdSize = new AdSize(300, 250);
    View unusedView = simulateAdLoadSuccess(customAdSize);

    int expectedWidth = customAdSize.getWidthInPixels(activity);
    assertThat(unityBannerAd.getWidthInPixels()).isEqualTo(expectedWidth);
  }

  @Test
  public void testIsCollapsible_whenAdNotLoaded_returnsFalse() {
    assertThat(unityBannerAd.isCollapsible()).isFalse();
  }

  @Test
  public void testIsCollapsible_whenAdLoaded_returnsCollapsible() {
    when(mockBannerAd.isCollapsible()).thenReturn(true);
    View unusedView = simulateAdLoadSuccess();

    assertThat(unityBannerAd.isCollapsible()).isTrue();
  }

  @Test
  public void testDestroy_callsBannerAdDestroy() {
    View unusedView = simulateAdLoadSuccess();

    unityBannerAd.destroy();
    ShadowLooper.idleMainLooper();

    verify(mockBannerAd).destroy();
  }

  @Test
  public void testDestroy_whenBannerAdIsNull_doesNothing() {
    unityBannerAd.load(bannerAdRequest);

    unityBannerAd.destroy();
    ShadowLooper.idleMainLooper();

    verify(mockBannerAd, never()).destroy();
  }

  @Test
  public void testShow_beforeAdLoaded_doesNothing() {
    unityBannerAd.show();
    ShadowLooper.idleMainLooper();

    verify(mockBannerAd, never()).getView(activity);
  }

  @Test
  public void testLoad_whenHidden_doesNotShowAutomatically() {
    unityBannerAd.hidden = true;

    View unusedView = simulateAdLoadSuccess();

    ShadowLooper.idleMainLooper();

    verify(mockBannerAd, never()).getView(activity);
  }

  @Test
  public void testCreate_withPositionCode_setsLayoutParamsGravity() {
    unityBannerAd.create(PluginUtils.POSITION_BOTTOM);

    FrameLayout.LayoutParams params = unityBannerAd.getLayoutParams();

    assertThat(params.gravity).isEqualTo(Gravity.BOTTOM | Gravity.CENTER_HORIZONTAL);
  }

  @Test
  public void testCreate_withPositionTop_setsLayoutParamsGravity() {
    unityBannerAd.create(PluginUtils.POSITION_TOP);

    FrameLayout.LayoutParams params = unityBannerAd.getLayoutParams();

    assertThat(params.gravity).isEqualTo(Gravity.TOP | Gravity.CENTER_HORIZONTAL);
  }

  @Test
  public void testCreate_withCustomOffset_setsCustomPositionAndOffsets() {
    int customX = 100;
    int customY = 200;
    unityBannerAd.create(customX, customY);

    FrameLayout.LayoutParams params = unityBannerAd.getLayoutParams();

    assertThat(params.gravity).isEqualTo(Gravity.TOP | Gravity.LEFT);
    float density = activity.getResources().getDisplayMetrics().density;
    assertThat(params.leftMargin).isEqualTo((int) (customX * density));
    assertThat(params.topMargin).isEqualTo((int) (customY * density));
  }

  @Test
  public void testSetPosition_withPositionCode_updatesLayoutParams() {
    unityBannerAd.create(PluginUtils.POSITION_TOP);

    unityBannerAd.setPosition(PluginUtils.POSITION_BOTTOM);
    ShadowLooper.idleMainLooper();

    FrameLayout.LayoutParams params = unityBannerAd.getLayoutParams();
    assertThat(params.gravity).isEqualTo(Gravity.BOTTOM | Gravity.CENTER_HORIZONTAL);
  }

  @Test
  public void testSetPosition_withCustomOffset_updatesLayoutParams() {
    unityBannerAd.create(PluginUtils.POSITION_TOP);

    int customX = 50;
    int customY = 60;
    unityBannerAd.setPosition(customX, customY);
    ShadowLooper.idleMainLooper();

    FrameLayout.LayoutParams params = unityBannerAd.getLayoutParams();
    assertThat(params.gravity).isEqualTo(Gravity.TOP | Gravity.LEFT);

    float density = activity.getResources().getDisplayMetrics().density;
    assertThat(params.leftMargin).isEqualTo((int) (customX * density));
    assertThat(params.topMargin).isEqualTo((int) (customY * density));
  }

  @Test
  public void testLayoutChangeListener_onLayoutChange_updatesPositionIfBoundsChanged() {
    View fakeView = simulateAdLoadSuccess();
    unityBannerAd.show();
    ShadowLooper.idleMainLooper();

    FrameLayout.LayoutParams initialParams = (FrameLayout.LayoutParams) fakeView.getLayoutParams();
    assertThat(initialParams).isNotNull();
    int initialGravity = initialParams.gravity;

    unityBannerAd.setPosition(PluginUtils.POSITION_BOTTOM);

    View decorView = activity.getWindow().getDecorView();
    decorView.layout(0, 0, 500, 500);
    ShadowLooper.idleMainLooper();

    FrameLayout.LayoutParams updatedParams = (FrameLayout.LayoutParams) fakeView.getLayoutParams();
    assertThat(updatedParams).isNotNull();
    assertThat(updatedParams.gravity).isNotEqualTo(initialGravity);
  }

  @Test
  @Config(sdk = 27)
  public void testGetSafeInsets_sdkLessThanP_returnsEmptyInsets() {
    View fakeView = new View(activity);
    when(mockBannerAd.getView(activity)).thenReturn(fakeView);
    unityBannerAd.load(bannerAdRequest);
    verify(mockAdWrapper).load(eq(bannerAdRequest), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockBannerAd);

    unityBannerAd.create(PluginUtils.POSITION_TOP);
    ShadowLooper.idleMainLooper();

    FrameLayout.LayoutParams params = unityBannerAd.getLayoutParams();
    assertThat(params.leftMargin).isEqualTo(0);
    assertThat(params.topMargin).isEqualTo(0);
    assertThat(params.rightMargin).isEqualTo(0);
    assertThat(params.bottomMargin).isEqualTo(0);

    // Verify that setLayoutParams was called on the fakeView.
    assertThat(fakeView.getLayoutParams()).isInstanceOf(FrameLayout.LayoutParams.class);
    FrameLayout.LayoutParams layoutParams = (FrameLayout.LayoutParams) fakeView.getLayoutParams();
    assertThat(layoutParams.leftMargin).isEqualTo(0);
    assertThat(layoutParams.topMargin).isEqualTo(0);
    assertThat(layoutParams.rightMargin).isEqualTo(0);
    assertThat(layoutParams.bottomMargin).isEqualTo(0);
  }

  @Test
  public void testGetHeightInPixels_whenTaskThrowsException_returnsNegativeOne() {
    View fakeView = new View(activity);
    when(mockBannerAd.getView(activity)).thenReturn(fakeView);

    when(mockBannerAd.getAdSize()).thenThrow(new RuntimeException("Test Exception"));

    unityBannerAd.load(bannerAdRequest);
    verify(mockAdWrapper).load(eq(bannerAdRequest), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockBannerAd);

    assertThat(unityBannerAd.getHeightInPixels()).isEqualTo(-1.0f);
  }

  @Test
  public void testGetWidthInPixels_whenTaskThrowsException_returnsNegativeOne() {
    View fakeView = new View(activity);
    when(mockBannerAd.getView(activity)).thenReturn(fakeView);

    when(mockBannerAd.getAdSize()).thenThrow(new RuntimeException("Test Exception"));

    unityBannerAd.load(bannerAdRequest);
    verify(mockAdWrapper).load(eq(bannerAdRequest), adLoadCallbackCaptor.capture());
    adLoadCallbackCaptor.getValue().onAdLoaded(mockBannerAd);

    assertThat(unityBannerAd.getWidthInPixels()).isEqualTo(-1.0f);
  }

  @Test
  public void testDestroy_whenHiddenAndNotShown_doesNotThrowAndCallsDestroy() {
    unityBannerAd.hidden = true;

    View unusedView = simulateAdLoadSuccess();

    ShadowLooper.idleMainLooper();
    unityBannerAd.destroy();
    ShadowLooper.idleMainLooper();

    verify(mockBannerAd).destroy();
  }

  @Test
  @Config(sdk = 28)
  public void testGetSafeInsets_whenWindowIsNull_returnsZeroInsets() {
    shadowOf(activity).setWindow(null);
    UnityBannerAd ad = new UnityBannerAd(activity, mockCallback, mockAdWrapper);
    ad.create(PluginUtils.POSITION_TOP);

    FrameLayout.LayoutParams params = ad.getLayoutParams();

    assertThat(params.leftMargin).isEqualTo(0);
    assertThat(params.topMargin).isEqualTo(0);
    assertThat(params.rightMargin).isEqualTo(0);
    assertThat(params.bottomMargin).isEqualTo(0);
  }

  @Test
  public void testPublicConstructor_createsInstanceWithoutCrash() {
    UnityBannerAd ad = new UnityBannerAd(activity, mockCallback);
    assertThat(ad).isNotNull();
  }

  @Test
  public void testLoad_multipleTimes_onlyOneLayoutChangeListener() throws Exception {
    java.lang.reflect.Field field = UnityBannerAd.class.getDeclaredField("layoutChangeListener");
    field.setAccessible(true);

    View.OnLayoutChangeListener listenerBefore =
        (View.OnLayoutChangeListener) field.get(unityBannerAd);
    assertThat(listenerBefore).isNull();

    unityBannerAd.load(bannerAdRequest);

    View.OnLayoutChangeListener firstRegistration =
        (View.OnLayoutChangeListener) field.get(unityBannerAd);
    assertThat(firstRegistration).isNotNull();

    unityBannerAd.load(bannerAdRequest);

    View.OnLayoutChangeListener secondRegistration =
        (View.OnLayoutChangeListener) field.get(unityBannerAd);

    assertThat(firstRegistration).isSameInstanceAs(secondRegistration);
  }

  @Test
  public void testDestroy_nullifiesLayoutChangeListener() throws Exception {

    unityBannerAd.load(bannerAdRequest);

    java.lang.reflect.Field field = UnityBannerAd.class.getDeclaredField("layoutChangeListener");
    field.setAccessible(true);
    View.OnLayoutChangeListener listener = (View.OnLayoutChangeListener) field.get(unityBannerAd);
    assertThat(listener).isNotNull();

    unityBannerAd.destroy();

    View.OnLayoutChangeListener listenerAfter =
        (View.OnLayoutChangeListener) field.get(unityBannerAd);
    assertThat(listenerAfter).isNull();
  }
}
