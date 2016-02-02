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

import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.purchase.InAppPurchaseResult;

import android.app.Activity;
import android.util.Log;

/**
 * Utilities for the Google Mobile Ads Unity plugin.
 */
public class PluginUtils {
  /** Tag used for logging statements. */
  public static final String LOGTAG = "AdsUnity";

  /**
   * Gets a string error reason from an error code.
   *
   * @param errorCode The error code.
   * @return The reason for the error.
   */
  public static String getErrorReason(int errorCode) {
    switch(errorCode) {
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
   * Returns whether the InAppPurchaseResult was successful.
   * @param result The InAppPurchaseResult to check.
   * @return true on success, otherwise false.
   */
  public static boolean isResultSuccess(InAppPurchaseResult result) {
      return result.getResultCode() == Activity.RESULT_OK;
  }
}
