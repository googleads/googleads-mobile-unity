package com.google.unity.ads.decagon;

import android.app.Activity;
import java.util.concurrent.Executor;

/**
 * Base class for Unity Ad implementations.
 *
 * @param <AdT> The type of the ad object (e.g., InterstitialAd).
 * @param <CallbackT> The type of the Unity ad callback.
 */
public abstract class UnityAdBase<AdT, CallbackT> {

  /** The Ad object. */
  protected AdT ad;

  /** The {@code Activity} on which the ad will display. */
  protected final Activity activity;

  /** A callback implemented in Unity via {@code AndroidJavaProxy} to receive ad events. */
  protected final CallbackT callback;

  /** The executor for running callbacks. */
  protected final Executor executor;

  protected UnityAdBase(Activity activity, CallbackT callback, Executor executor) {
    this.activity = activity;
    this.callback = callback;
    this.executor = executor;
  }
}
