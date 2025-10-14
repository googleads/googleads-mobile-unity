package com.google.unity.ads.decagon;

import static com.google.common.truth.Truth.assertThat;
import static org.mockito.Mockito.when;

import com.google.android.libraries.ads.mobile.sdk.common.RequestConfiguration;
import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.Mock;
import org.mockito.junit.MockitoJUnit;
import org.mockito.junit.MockitoRule;
import org.robolectric.RobolectricTestRunner;

/** Unit tests for {@link UnityMobileAds}. */
@RunWith(RobolectricTestRunner.class)
public class UnityMobileAdsTest {

  @Rule public final MockitoRule mocks = MockitoJUnit.rule();
  @Mock private MobileAdsWrapper mockMobileAdsWrapper;

  @Before
  public void setUp() {
    UnityMobileAds.setMobileAdsWrapper(mockMobileAdsWrapper);
  }

  @Test
  public void testGetRequestConfiguration() {
    RequestConfiguration requestConfiguration = new RequestConfiguration.Builder().build();
    when(mockMobileAdsWrapper.getRequestConfiguration()).thenReturn(requestConfiguration);

    RequestConfiguration result = UnityMobileAds.getRequestConfiguration();

    assertThat(result).isEqualTo(requestConfiguration);
  }

  @Test
  public void testGetSdkVersionString() {
    String sdkVersion = "1.2.3";
    when(mockMobileAdsWrapper.getVersionString()).thenReturn(sdkVersion);

    String result = UnityMobileAds.getSdkVersionString();

    assertThat(result).isEqualTo(sdkVersion);
  }
}
