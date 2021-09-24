package com.google.unity.ads;

import com.google.android.gms.ads.AdInspectorError;

/**
 * Ad inspector interface form of {@link UnityAdInspectorListener} that can be implemented via
 * {@code AndroidJavaProxy} in Unity to receive ad inspector events synchronously.
 */
public interface UnityAdInspectorListener {

  void onAdInspectorClosed(AdInspectorError error);
}
