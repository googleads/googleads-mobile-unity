package com.google.unity.ump;

import com.google.android.ump.FormError;

/**
 * An interface that can be implemented via {@code AndroidJavaProxy} in Unity to receive consent
 * form events synchronously.
 */
public interface UnityConsentFormCallback {

  void onConsentFormDismissed(FormError error);
}
