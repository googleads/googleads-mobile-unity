package com.google.unity.ump;

import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.doAnswer;
import static org.mockito.Mockito.verify;

import android.app.Activity;
import com.google.android.ump.ConsentForm;
import com.google.android.ump.ConsentForm.OnConsentFormDismissedListener;
import com.google.android.ump.FormError;
import com.google.common.time.Sleeper;
import java.time.Duration;
import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.Mock;
import org.mockito.junit.MockitoJUnit;
import org.mockito.junit.MockitoRule;
import org.robolectric.Robolectric;
import org.robolectric.RobolectricTestRunner;

/** Tests for {@link UnityConsentForm} */
@RunWith(RobolectricTestRunner.class)
public final class UnityConsentFormTest {

  private static final Duration SLEEP_DURATION = Duration.ofMillis(200);

  @Rule public final MockitoRule mockito = MockitoJUnit.rule();
  @Mock private UnityConsentFormCallback mockCallback;
  @Mock private ConsentForm mockConsentForm;

  private Activity activity;
  private UnityConsentForm unityConsentForm;

  @Before
  public void setUp() throws Exception {
    activity = Robolectric.buildActivity(Activity.class).create().get();
    unityConsentForm = new UnityConsentForm(activity, mockCallback);
    unityConsentForm.loadAndShowConsentFormIfRequired();
    // Sleep to wait for background thread execution.
    Sleeper.defaultSleeper().sleep(SLEEP_DURATION);
  }

  @Test
  public void loadAndShowConsentForm_succeeds() throws Exception {
    setupConsentFormMock();
    unityConsentForm.show(mockConsentForm);
    verify(mockCallback).onConsentFormDismissed(any(FormError.class));
  }

  @Test
  public void loadAndShowPrivacyOptionsForm_succeeds() throws Exception {
    unityConsentForm.showPrivacyOptionsForm();
    verify(mockCallback).onConsentFormDismissed(any(FormError.class));
  }

  private void setupConsentFormMock() throws Exception {
    doAnswer(
            invocation -> {
              OnConsentFormDismissedListener listener = invocation.getArgument(1);
              listener.onConsentFormDismissed(null);
              return null;
            })
        .when(mockConsentForm)
        .show(any(), any());
  }
}
