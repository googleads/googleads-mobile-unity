package com.google.unity.ads;

import static org.mockito.ArgumentMatchers.anyBoolean;
import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.never;
import static org.mockito.Mockito.verify;

import android.app.Activity;
import androidx.lifecycle.LifecycleOwner;
import androidx.lifecycle.ProcessLifecycleOwner;
import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.Mock;
import org.mockito.junit.MockitoJUnit;
import org.mockito.junit.MockitoRule;
import org.robolectric.Robolectric;
import org.robolectric.RobolectricTestRunner;

/** Tests for {@link UnityAppStateEventNotifier} */
@RunWith(RobolectricTestRunner.class)
public final class UnityAppStateEventNotifierTest {

  @Rule public final MockitoRule mockito = MockitoJUnit.rule();
  @Mock private UnityAppStateEventCallback mockCallback;

  private Activity activity;
  private UnityAppStateEventNotifier unityAppStateEventNotifier;
  private final LifecycleOwner lifecycleOwner = ProcessLifecycleOwner.get();

  @Before
  public void setUp() {
    activity = Robolectric.buildActivity(Activity.class).create().get();
    unityAppStateEventNotifier = new UnityAppStateEventNotifier(activity, mockCallback);
  }

  @Test
  public void startListening_succeeds() {
    unityAppStateEventNotifier.startListening();
    verify(mockCallback, never()).onAppStateChanged(anyBoolean());
  }

  @Test
  public void stopListening_succeeds() {
    unityAppStateEventNotifier.stopListening();
    verify(mockCallback, never()).onAppStateChanged(anyBoolean());
  }

  @Test
  public void onStart_succeeds() {
    unityAppStateEventNotifier.onStart(lifecycleOwner);
    verify(mockCallback).onAppStateChanged(eq(false));
  }

  @Test
  public void onStop_succeeds() {
    unityAppStateEventNotifier.onStop(lifecycleOwner);
    verify(mockCallback).onAppStateChanged(eq(true));
  }

  @Test
  public void onCreate_succeeds() {
    unityAppStateEventNotifier.onCreate(lifecycleOwner);
    verify(mockCallback, never()).onAppStateChanged(anyBoolean());
  }

  @Test
  public void onDestroy_succeeds() {
    unityAppStateEventNotifier.onDestroy(lifecycleOwner);
    verify(mockCallback, never()).onAppStateChanged(anyBoolean());
  }

  @Test
  public void onResume_succeeds() {
    unityAppStateEventNotifier.onResume(lifecycleOwner);
    verify(mockCallback, never()).onAppStateChanged(anyBoolean());
  }

  @Test
  public void onPause_succeeds() {
    unityAppStateEventNotifier.onPause(lifecycleOwner);
    verify(mockCallback, never()).onAppStateChanged(anyBoolean());
  }
}
