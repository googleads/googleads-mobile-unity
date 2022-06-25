package com.google.unity.ads;

/**
 * Ad inspector interface form of {@link UnityAdInspectorListener} that can be implemented via
 * {@code AndroidJavaProxy} in Unity to receive ad inspector events synchronously.
 */
public interface UnityAdInspectorListener {

  void onAdInspectorClosed(String error);
}
