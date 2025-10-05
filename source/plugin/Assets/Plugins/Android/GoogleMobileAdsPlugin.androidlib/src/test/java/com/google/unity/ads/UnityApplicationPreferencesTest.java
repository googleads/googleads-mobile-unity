package com.google.unity.ads;

import static com.google.common.truth.Truth.assertThat;

import android.app.Activity;
import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.robolectric.Robolectric;
import org.robolectric.RobolectricTestRunner;

/** Tests for {@link UnityApplicationPreferences} */
@RunWith(RobolectricTestRunner.class)
public final class UnityApplicationPreferencesTest {

  private Activity activity;
  private UnityApplicationPreferences unityApplicationPreferences;

  @Before
  public void setUp() {
    activity = Robolectric.buildActivity(Activity.class).create().get();
    unityApplicationPreferences = new UnityApplicationPreferences(activity);
  }

  @Test
  public void setInt_shouldSetKeyValueInSharedPreferences() throws Exception {
    unityApplicationPreferences.setInt("test_key", 10);
    assertThat(unityApplicationPreferences.getInt("test_key")).isEqualTo(10);
    unityApplicationPreferences.setInt("test_key", 20);
    assertThat(unityApplicationPreferences.getInt("test_key")).isEqualTo(20);
    unityApplicationPreferences.setInt("another_test_key", -100);
    assertThat(unityApplicationPreferences.getInt("another_test_key")).isEqualTo(-100);
  }

  @Test
  public void setString_shouldSetKeyValueInSharedPreferences() throws Exception {
    unityApplicationPreferences.setString("test_key", "test_value");
    assertThat(unityApplicationPreferences.getString("test_key")).isEqualTo("test_value");
    unityApplicationPreferences.setString("test_key", "foo");
    assertThat(unityApplicationPreferences.getString("test_key")).isEqualTo("foo");
    unityApplicationPreferences.setString("another_test_key", "bar");
    assertThat(unityApplicationPreferences.getString("another_test_key")).isEqualTo("bar");
  }

  @Test
  public void getInt_shouldReturnZero_whenKeyDoesNotExist() throws Exception {
    assertThat(unityApplicationPreferences.getInt("test_key")).isEqualTo(0);
  }

  @Test
  public void getString_shouldReturnNull_whenKeyDoesNotExist() throws Exception {
    assertThat(unityApplicationPreferences.getString("test_key")).isNull();
  }
}
