package com.google.unity.ump;

import android.app.Activity;
import com.google.android.ump.ConsentForm;
import com.google.android.ump.ConsentForm.OnConsentFormDismissedListener;
import com.google.android.ump.FormError;
import com.google.android.ump.UserMessagingPlatform;

/** Native consent form implementation for the Unity UMP plugin. */
public class UnityConsentForm {
  // TODO (b/284206705): Set up unit and integration tests for platform code.

  /** The {@code Activity} on which the consent form will display. */
  private final Activity activity;

  /**
   * A listener implemented in Unity via {@code AndroidJavaProxy} to receive consent form events.
   */
  private final UnityConsentFormCallback callback;

  /** Callback to be invoked when a consent form is dismissed. */
  private final OnConsentFormDismissedListener onConsentFormDismissedListener =
      new OnConsentFormDismissedListener() {
        @Override
        public void onConsentFormDismissed(final FormError error) {
          new Thread(
                  () -> {
                    if (callback != null) {
                      callback.onConsentFormDismissed(error);
                    }
                  })
              .start();
        }
      };

  public UnityConsentForm(Activity activity, UnityConsentFormCallback callback) {
    this.activity = activity;
    this.callback = callback;
  }

  /**
   * Loads and shows a consent form.
   *
   * <p>This method must be called on the main thread.
   */
  public void loadAndShowConsentFormIfRequired() {
    UserMessagingPlatform.loadAndShowConsentFormIfRequired(
        activity, onConsentFormDismissedListener);
  }

  /**
   * Shows the consent form.
   *
   * <p>This method must be called on the main thread.
   */
  public void show(ConsentForm consentForm) {
    consentForm.show(activity, onConsentFormDismissedListener);
  }

  /**
   * Presents a privacy options form from the provided {@code Activity} if required.
   *
   * <p>This method must be called on the main thread.
   */
  public void showPrivacyOptionsForm() {
    UserMessagingPlatform.showPrivacyOptionsForm(activity, onConsentFormDismissedListener);
  }
}
