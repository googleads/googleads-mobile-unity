// Copyright 2014 Google Inc. All Rights Reserved.

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
