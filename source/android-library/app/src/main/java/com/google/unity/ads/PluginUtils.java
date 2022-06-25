/*
 * Copyright (C) 2015 Google, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package com.google.unity.ads;

import android.content.res.Resources;
import android.os.Bundle;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.Gravity;
import com.google.android.gms.ads.AdError;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.AdapterResponseInfo;
import com.google.android.gms.ads.LoadAdError;
import com.google.android.gms.ads.ResponseInfo;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

/** Utilities for the Google Mobile Ads Unity plugin. */
public class PluginUtils {

  /** Tag used for logging statements. */
  public static final String LOGTAG = "AdsUnity";

  /** Position constant for a position with a custom offset. */
  public static final int POSITION_CUSTOM = -1;

  /** Position constant for top of the screen. */
  public static final int POSITION_TOP = 0;

  /** Position constant for bottom of the screen. */
  public static final int POSITION_BOTTOM = 1;

  /** Position constant for top-left of the screen. */
  public static final int POSITION_TOP_LEFT = 2;

  /** Position constant for top-right of the screen. */
  public static final int POSITION_TOP_RIGHT = 3;

  /** Position constant for bottom-left of the screen. */
  public static final int POSITION_BOTTOM_LEFT = 4;

  /** Position constant bottom-right of the screen. */
  public static final int POSITION_BOTTOM_RIGHT = 5;

  /** Position constant center of the screen. */
  public static final int POSITION_CENTER = 6;

  /**
   * Gets a string error reason from an error code.
   *
   * @param errorCode The error code.
   * @return The reason for the error.
   */
  public static String getErrorReason(int errorCode) {
    switch (errorCode) {
      case AdRequest.ERROR_CODE_INTERNAL_ERROR:
        return "Internal error";
      case AdRequest.ERROR_CODE_INVALID_REQUEST:
        return "Invalid request";
      case AdRequest.ERROR_CODE_NETWORK_ERROR:
        return "Network Error";
      case AdRequest.ERROR_CODE_NO_FILL:
        return "No fill";
      default:
        Log.w(LOGTAG, String.format("Unexpected error code: %s", errorCode));
        return "";
    }
  }

  /**
   * Returns a {@link Gravity} constant corresponding to a positionCode.
   *
   * @param positionCode A code indicating where to place the ad.
   * @return {@link Gravity} constant corresponding to positionCode argument.
   */
  public static int getLayoutGravityForPositionCode(int positionCode) {
    int gravity;
    switch (positionCode) {
      case POSITION_TOP:
        gravity = Gravity.TOP | Gravity.CENTER_HORIZONTAL;
        break;
      case POSITION_BOTTOM:
        gravity = Gravity.BOTTOM | Gravity.CENTER_HORIZONTAL;
        break;
      case POSITION_TOP_LEFT:
        gravity = Gravity.TOP | Gravity.LEFT;
        break;
      case POSITION_TOP_RIGHT:
        gravity = Gravity.TOP | Gravity.RIGHT;
        break;
      case POSITION_BOTTOM_LEFT:
        gravity = Gravity.BOTTOM | Gravity.LEFT;
        break;
      case POSITION_BOTTOM_RIGHT:
        gravity = Gravity.BOTTOM | Gravity.RIGHT;
        break;
      case POSITION_CENTER:
        gravity = Gravity.CENTER_HORIZONTAL | Gravity.CENTER_VERTICAL;
        break;
      case POSITION_CUSTOM:
        gravity = Gravity.TOP | Gravity.LEFT;
        break;
      default:
        throw new IllegalArgumentException(
            "Attempted to position ad with invalid ad " + "position.");
    }
    return gravity;
  }

  public static float convertPixelsToDp(float px) {
    DisplayMetrics metrics = Resources.getSystem().getDisplayMetrics();
    return px / metrics.density;
  }

  public static float convertDpToPixel(float dp) {
    DisplayMetrics metrics = Resources.getSystem().getDisplayMetrics();
    return dp * metrics.density;
  }

  public static String toJsonString(AdError error) {
    String json = "";
    try {
      json = toJson(error).toString();
    } catch (JSONException exception) {
      Log.e(
          PluginUtils.LOGTAG,
          String.format("Unable to parse ad error: %s", exception.getLocalizedMessage()));
    }
    return json;
  }

  public static String toJsonString(LoadAdError error) {
    String json = "";
    try {
      json = toJson(error).toString();
    } catch (JSONException exception) {
      Log.e(
          PluginUtils.LOGTAG,
          String.format("Unable to parse load ad error: %s", exception.getLocalizedMessage()));
    }
    return json;
  }

  public static String toJsonString(ResponseInfo info) {
    String json = "";
    try {
      json = toJson(info).toString();
    } catch (JSONException exception) {
      Log.e(
          PluginUtils.LOGTAG,
          String.format("Unable to parse ad response info: %s", exception.getLocalizedMessage()));
    }
    return json;
  }

  public static JSONObject toJson(AdError error) throws JSONException {
    JSONObject json = new JSONObject();
    if (error != null) {
      AdError cause = error.getCause();
      if (cause != null) {
        // Serialize as string due to Unity nesting limits. b/243737332
        json.put("cause", toJson(cause).toString());
      }
      json.put("code", error.getCode());
      json.put("description", error.toString());
      json.put("domain", error.getDomain());
      json.put("message", error.getMessage());
    }
    return json;
  }

  public static JSONObject toJson(LoadAdError error) throws JSONException {
    JSONObject json = toJson(error);
    if (error != null) {
      json.put("responseInfo", toJson(error.getResponseInfo()));
    }
    return json;
  }

  public static JSONObject toJson(ResponseInfo info) throws JSONException {
    JSONObject json = new JSONObject();
    json.put("responseId", info.getResponseId());
    json.put("adNetworkName", info.getMediationAdapterClassName());
    json.put("description", info.toString());

    JSONArray adapterResponsesJson = new JSONArray();
    for (AdapterResponseInfo adapterResponseInfo : info.getAdapterResponses()) {
      adapterResponsesJson.put(toJson(adapterResponseInfo));
    }
    json.put("adapterResponseInfos", adapterResponsesJson);

    AdapterResponseInfo loadedResponse = info.getLoadedAdapterResponseInfo();
    if (loadedResponse != null) {
      json.put("adapterResponseInfo", toJson(loadedResponse));
    }

    Bundle responseExtras = info.getResponseExtras();
    if (responseExtras != null) {
      json.put("responseExtras", toJson(responseExtras));
    }
    return json;
  }

  static JSONObject toJson(AdapterResponseInfo info) throws JSONException {
    JSONObject json = new JSONObject();
    json.put("adapterClassName", info.getAdapterClassName());
    json.put("latencyMillis", info.getLatencyMillis());
    json.put("description", info.toString());
    json.put("adSourceName", info.getAdSourceName());
    json.put("adSourceId", info.getAdSourceId());
    json.put("adSourceInstanceName", info.getAdSourceInstanceName());
    json.put("adSourceInstanceId", info.getAdSourceInstanceId());

    Bundle credentials = info.getCredentials();
    if (credentials != null) {
      json.put("adUnitMapping", toJson(credentials));
    }

    AdError adError = info.getAdError();
    if (adError != null) {
      // Serialize as string due to Unity nesting limits. b/243737332
      json.put("adError", toJson(adError).toString());
    }

    return json;
  }

  private static JSONArray toJson(Bundle bundle) throws JSONException {
    JSONArray array = new JSONArray();
    if (bundle != null) {
      for (String key : bundle.keySet()) {
        JSONObject json = new JSONObject();
        // Unity does not support dictionary serialization.
        // Our workaround is to return an aray of objects with a key and value field.
        json.put("key", key);
        json.put("value", bundle.get(key).toString());
        array.put(json);
      }
    }
    return array;
  }
}
