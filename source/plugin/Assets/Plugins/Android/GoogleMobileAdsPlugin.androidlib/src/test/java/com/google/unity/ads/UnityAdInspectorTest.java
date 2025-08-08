package com.google.unity.ads;

import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.verify;

import android.app.Activity;
import com.google.android.gms.ads.AdInspectorError;
import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.Mock;
import org.mockito.junit.MockitoJUnit;
import org.mockito.junit.MockitoRule;
import org.robolectric.Robolectric;
import org.robolectric.RobolectricTestRunner;

/** Tests for {@link UnityAdInspector} */
@RunWith(RobolectricTestRunner.class)
public final class UnityAdInspectorTest {

  @Rule public final MockitoRule mockito = MockitoJUnit.rule();
  @Mock private UnityAdInspectorListener mockListener;

  private Activity activity;

  @Before
  public void setUp() {
    activity = Robolectric.buildActivity(Activity.class).create().get();
  }

  @Test
  public void openAdInspector_succeeds() throws Exception {
    UnityAdInspector.openAdInspector(activity, mockListener);
    verify(mockListener).onAdInspectorClosed(any(AdInspectorError.class));
  }
}
