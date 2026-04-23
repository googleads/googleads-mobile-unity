package com.google.unity.ads.nextgen;

import static com.google.common.truth.Truth.assertThat;
import static java.util.concurrent.TimeUnit.SECONDS;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.doAnswer;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.never;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import android.app.Activity;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageInfo;
import android.os.Bundle;
import androidx.annotation.Nullable;
import com.google.android.libraries.ads.mobile.sdk.common.RequestConfiguration;
import com.google.android.libraries.ads.mobile.sdk.initialization.InitializationConfig;
import com.google.android.libraries.ads.mobile.sdk.initialization.InitializationStatus;
import com.google.android.libraries.ads.mobile.sdk.initialization.OnAdapterInitializationCompleteListener;
import java.lang.reflect.Field;
import java.lang.reflect.Method;
import java.util.concurrent.CountDownLatch;
import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.ArgumentCaptor;
import org.mockito.Mock;
import org.mockito.junit.MockitoJUnit;
import org.mockito.junit.MockitoRule;
import org.robolectric.Robolectric;
import org.robolectric.RobolectricTestRunner;
import org.robolectric.Shadows;

/** Unit tests for {@link UnityMobileAds}. */
@RunWith(RobolectricTestRunner.class)
public class UnityMobileAdsTest {

  @Rule public final MockitoRule mocks = MockitoJUnit.rule();
  @Mock private MobileAdsWrapper mockMobileAdsWrapper;

  @Before
  public void setUp() throws Exception {
    UnityMobileAds.setMobileAdsWrapper(mockMobileAdsWrapper);

    // Reset static state to prevent bleeding between tests
    setStaticField("isMobileAdsInitialized", false);
    setStaticField("requestConfiguration", null);
    setStaticField("userVolume", -1f);
    setStaticField("isMuted", false);
    setStaticField("isPublisherFirstPartyIdEnabled", false);
  }

  @Test
  public void testSetRequestConfiguration_initialized() throws Exception {
    setStaticField("isMobileAdsInitialized", true);
    setStaticField("requestConfiguration", null);

    RequestConfiguration config = new RequestConfiguration.Builder().build();

    UnityMobileAds.setRequestConfiguration(config);
    verify(mockMobileAdsWrapper).setRequestConfiguration(config);
    RequestConfiguration storedConfig =
        (RequestConfiguration) getStaticField("requestConfiguration");
    assertThat(storedConfig).isNull();
  }

  @Test
  public void testInitialize_nullAppId() throws Exception {
    Activity activity = setupActivityWithAppId(null);
    UnityMobileAds.initialize(activity, status -> {});
    verify(mockMobileAdsWrapper, never()).initialize(any(), any(), any());
  }

  @Test
  public void testInitialize_success() throws Exception {
    Activity activity = setupActivityWithAppId("ca-app-pub-3940256099942544~3347511713");

    InitializationStatus mockStatus = mock(InitializationStatus.class);

    // Stub the wrapper to call the callback inline
    doAnswer(
            invocation -> {
              OnAdapterInitializationCompleteListener listener = invocation.getArgument(2);
              listener.onAdapterInitializationComplete(mockStatus);
              return null;
            })
        .when(mockMobileAdsWrapper)
        .initialize(any(), any(), any());

    CountDownLatch latch = new CountDownLatch(1);
    UnityMobileAds.initialize(
        activity,
        status -> {
          assertThat(status).isEqualTo(mockStatus);
          latch.countDown();
        });

    assertThat(latch.await(5, SECONDS)).isTrue();

    verify(mockMobileAdsWrapper).initialize(any(), any(), any());
  }

  @Test
  public void testInitialize_flushesBufferedMute() throws Exception {
    Activity activity = setupActivityWithAppId("ca-app-pub-3940256099942544~3347511713");

    UnityMobileAds.setApplicationMuted(true);
    InitializationStatus mockStatus = mock(InitializationStatus.class);
    doAnswer(
            invocation -> {
              OnAdapterInitializationCompleteListener listener = invocation.getArgument(2);
              listener.onAdapterInitializationComplete(mockStatus);
              return null;
            })
        .when(mockMobileAdsWrapper)
        .initialize(any(), any(), any());

    CountDownLatch latch = new CountDownLatch(1);
    UnityMobileAds.initialize(activity, status -> latch.countDown());
    assertThat(latch.await(5, SECONDS)).isTrue();

    verify(mockMobileAdsWrapper).setUserMutedApp(true);

    boolean isMuted = (boolean) getStaticField("isMuted");
    assertThat(isMuted).isFalse();
  }

  @Test
  public void testInitialize_appliesBufferedRequestConfiguration() throws Exception {
    Activity activity = setupActivityWithAppId("ca-app-pub-3940256099942544~3347511713");

    RequestConfiguration config = new RequestConfiguration.Builder().build();
    UnityMobileAds.setRequestConfiguration(config);

    InitializationStatus mockStatus = mock(InitializationStatus.class);
    ArgumentCaptor<InitializationConfig> configCaptor =
        ArgumentCaptor.forClass(InitializationConfig.class);
    doAnswer(
            invocation -> {
              OnAdapterInitializationCompleteListener listener = invocation.getArgument(2);
              listener.onAdapterInitializationComplete(mockStatus);
              return null;
            })
        .when(mockMobileAdsWrapper)
        .initialize(any(), configCaptor.capture(), any());

    CountDownLatch latch = new CountDownLatch(1);
    UnityMobileAds.initialize(activity, status -> latch.countDown());
    assertThat(latch.await(5, SECONDS)).isTrue();

    assertThat(configCaptor.getValue().getRequestConfiguration()).isEqualTo(config);

    RequestConfiguration storedConfig =
        (RequestConfiguration) getStaticField("requestConfiguration");
    assertThat(storedConfig).isNull();
  }

  @Test
  public void testInitialize_flushesBufferedUserVolume() throws Exception {
    Activity activity = setupActivityWithAppId("ca-app-pub-3940256099942544~3347511713");

    UnityMobileAds.setUserControlledAppVolume(0.5f);

    InitializationStatus mockStatus = mock(InitializationStatus.class);
    doAnswer(
            invocation -> {
              OnAdapterInitializationCompleteListener listener = invocation.getArgument(2);
              listener.onAdapterInitializationComplete(mockStatus);
              return null;
            })
        .when(mockMobileAdsWrapper)
        .initialize(any(), any(), any());

    CountDownLatch latch = new CountDownLatch(1);
    UnityMobileAds.initialize(activity, status -> latch.countDown());
    assertThat(latch.await(5, SECONDS)).isTrue();

    verify(mockMobileAdsWrapper).setUserControlledAppVolume(0.5f);

    float userVolume = (float) getStaticField("userVolume");
    assertThat(userVolume).isEqualTo(-1f);
  }

  @Test
  public void testInitialize_flushesBufferedPublisherFirstPartyId() throws Exception {
    Activity activity = setupActivityWithAppId("ca-app-pub-3940256099942544~3347511713");

    var unused = UnityMobileAds.putPublisherFirstPartyIdEnabled(true);

    InitializationStatus mockStatus = mock(InitializationStatus.class);
    doAnswer(
            invocation -> {
              OnAdapterInitializationCompleteListener listener = invocation.getArgument(2);
              listener.onAdapterInitializationComplete(mockStatus);
              return null;
            })
        .when(mockMobileAdsWrapper)
        .initialize(any(), any(), any());

    CountDownLatch latch = new CountDownLatch(1);
    UnityMobileAds.initialize(activity, status -> latch.countDown());
    assertThat(latch.await(5, SECONDS)).isTrue();

    verify(mockMobileAdsWrapper).putPublisherFirstPartyIdEnabled(true);

    boolean isEnabled = (boolean) getStaticField("isPublisherFirstPartyIdEnabled");
    assertThat(isEnabled).isFalse();
  }

  // Request Configuration Tests
  @Test
  public void testGetRequestConfiguration() throws Exception {
    setStaticField("isMobileAdsInitialized", true);
    RequestConfiguration requestConfiguration = new RequestConfiguration.Builder().build();
    when(mockMobileAdsWrapper.getRequestConfiguration()).thenReturn(requestConfiguration);

    RequestConfiguration result = UnityMobileAds.getRequestConfiguration();

    assertThat(result).isEqualTo(requestConfiguration);
  }

  @Test
  public void testGetRequestConfiguration_uninitialized_returnsBuffered() throws Exception {
    setStaticField("isMobileAdsInitialized", false);
    RequestConfiguration config = new RequestConfiguration.Builder().build();
    setStaticField("requestConfiguration", config);

    RequestConfiguration result = UnityMobileAds.getRequestConfiguration();

    assertThat(result).isEqualTo(config);
    verify(mockMobileAdsWrapper, never()).getRequestConfiguration();
  }

  // Set Request Configuration Tests
  @Test
  public void testSetRequestConfiguration_uninitialized() throws Exception {
    RequestConfiguration config = new RequestConfiguration.Builder().build();
    UnityMobileAds.setRequestConfiguration(config);

    RequestConfiguration storedConfig =
        (RequestConfiguration) getStaticField("requestConfiguration");

    assertThat(storedConfig).isEqualTo(config);
  }

  // SDK Version Tests
  @Test
  public void testGetSdkVersionString() {
    String sdkVersion = "1.2.3";
    when(mockMobileAdsWrapper.getVersionString()).thenReturn(sdkVersion);

    String result = UnityMobileAds.getSdkVersionString();

    assertThat(result).isEqualTo(sdkVersion);
  }

  // Muted Tests
  @Test
  public void testSetApplicationMuted_uninitialized() throws Exception {
    UnityMobileAds.setApplicationMuted(true);

    boolean isMuted = (boolean) getStaticField("isMuted");

    assertThat(isMuted).isTrue();
  }

  @Test
  public void testSetApplicationMuted_initialized() throws Exception {
    setStaticField("isMobileAdsInitialized", true);

    UnityMobileAds.setApplicationMuted(true);
    verify(mockMobileAdsWrapper).setUserMutedApp(true);
  }

  // User Volume Tests
  @Test
  public void testSetUserControlledAppVolume() throws Exception {
    UnityMobileAds.setUserControlledAppVolume(0.5f);

    float userVolume = (float) getStaticField("userVolume");

    assertThat(userVolume).isEqualTo(0.5f);
  }

  @Test
  public void testSetUserControlledAppVolume_initialized() throws Exception {
    setStaticField("isMobileAdsInitialized", true);

    UnityMobileAds.setUserControlledAppVolume(0.5f);
    verify(mockMobileAdsWrapper).setUserControlledAppVolume(0.5f);
  }

  // Publisher First Party ID Tests
  @Test
  public void testPutPublisherFirstPartyIdEnabled() throws Exception {
    boolean result = UnityMobileAds.putPublisherFirstPartyIdEnabled(true);

    assertThat(result).isTrue();
    boolean isPublisherFirstPartyIdEnabled =
        (boolean) getStaticField("isPublisherFirstPartyIdEnabled");
    assertThat(isPublisherFirstPartyIdEnabled).isTrue();
  }

  @Test
  public void testPutPublisherFirstPartyIdEnabled_initialized() throws Exception {
    setStaticField("isMobileAdsInitialized", true);

    var unused = UnityMobileAds.putPublisherFirstPartyIdEnabled(true);
    verify(mockMobileAdsWrapper).putPublisherFirstPartyIdEnabled(true);
  }

  // Meta Data Tests
  @Test
  public void testGetApplicationMetaData_success() throws Exception {
    Activity activity = Robolectric.buildActivity(Activity.class).create().get();

    PackageInfo packageInfo = new PackageInfo();
    packageInfo.packageName = activity.getPackageName();
    packageInfo.applicationInfo = new ApplicationInfo();
    packageInfo.applicationInfo.metaData = new Bundle();
    packageInfo.applicationInfo.metaData.putString("testKey", "testValue");

    Shadows.shadowOf(activity.getPackageManager()).installPackage(packageInfo);

    String result = invokeGetApplicationMetaData(activity, "testKey");
    assertThat(result).isEqualTo("testValue");
  }

  @Test
  public void testGetApplicationMetaData_nullActivity() throws Exception {
    String result = invokeGetApplicationMetaData(null, "testKey");

    assertThat(result).isNull();
  }

  @Test
  public void testGetApplicationMetaData_nullBundle() throws Exception {
    Activity activity = Robolectric.buildActivity(Activity.class).create().get();

    PackageInfo packageInfo = new PackageInfo();
    packageInfo.packageName = activity.getPackageName();
    packageInfo.applicationInfo = new ApplicationInfo();
    packageInfo.applicationInfo.metaData = null;

    Shadows.shadowOf(activity.getPackageManager()).installPackage(packageInfo);

    String result = invokeGetApplicationMetaData(activity, "testKey");

    assertThat(result).isNull();
  }

  @Test
  public void testGetApplicationMetaData_keyNotFound() throws Exception {
    Activity activity = Robolectric.buildActivity(Activity.class).create().get();

    PackageInfo packageInfo = new PackageInfo();
    packageInfo.packageName = activity.getPackageName();
    packageInfo.applicationInfo = new ApplicationInfo();
    packageInfo.applicationInfo.metaData = new Bundle();
    packageInfo.applicationInfo.metaData.putString("otherKey", "someValue");

    Shadows.shadowOf(activity.getPackageManager()).installPackage(packageInfo);

    String result = invokeGetApplicationMetaData(activity, "testKey");

    assertThat(result).isNull();
  }

  @Test
  public void testGetApplicationMetaData_exceptionThrown() throws Exception {
    Activity activity = Robolectric.buildActivity(Activity.class).create().get();

    Shadows.shadowOf(activity.getPackageManager()).removePackage(activity.getPackageName());

    String result = invokeGetApplicationMetaData(activity, "testKey");

    assertThat(result).isNull();
  }

  // Helpers
  private void setStaticField(String fieldName, Object value) throws Exception {
    Field field = UnityMobileAds.class.getDeclaredField(fieldName);
    field.setAccessible(true);
    field.set(null, value);
  }

  private Object getStaticField(String fieldName) throws Exception {
    Field field = UnityMobileAds.class.getDeclaredField(fieldName);
    field.setAccessible(true);
    return field.get(null);
  }

  private Activity setupActivityWithAppId(@Nullable String appId) throws Exception {
    Activity activity = Robolectric.buildActivity(Activity.class).create().get();

    PackageInfo packageInfo = new PackageInfo();
    packageInfo.packageName = activity.getPackageName();
    packageInfo.applicationInfo = new ApplicationInfo();
    packageInfo.applicationInfo.metaData = new Bundle();
    if (appId != null) {
      packageInfo.applicationInfo.metaData.putString(
          "com.google.android.gms.ads.APPLICATION_ID", appId);
    }

    Shadows.shadowOf(activity.getPackageManager()).installPackage(packageInfo);
    return activity;
  }

  private String invokeGetApplicationMetaData(Activity activity, String key) throws Exception {
    Method method =
        UnityMobileAds.class.getDeclaredMethod(
            "getApplicationMetaData", Activity.class, String.class);
    method.setAccessible(true);
    return (String) method.invoke(null, activity, key);
  }
}
