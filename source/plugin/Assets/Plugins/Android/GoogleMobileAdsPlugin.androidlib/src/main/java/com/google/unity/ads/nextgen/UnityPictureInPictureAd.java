/*
 * Copyright (C) 2026 Google, Inc.
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
package com.google.unity.ads.nextgen;

import android.app.Activity;
import android.util.Log;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.annotation.VisibleForTesting;
import com.google.android.libraries.ads.mobile.sdk.common.AdLoadCallback;
import com.google.android.libraries.ads.mobile.sdk.common.AdRequest;
import com.google.android.libraries.ads.mobile.sdk.common.AdValue;
import com.google.android.libraries.ads.mobile.sdk.common.LoadAdError;
import com.google.android.libraries.ads.mobile.sdk.common.ResponseInfo;
import com.google.android.libraries.ads.mobile.sdk.pip.PictureInPictureAd;
import com.google.android.libraries.ads.mobile.sdk.pip.PictureInPictureAdEventCallback;
import com.google.android.libraries.ads.mobile.sdk.pip.PictureInPictureAdOptions;
import com.google.android.libraries.ads.mobile.sdk.pip.PictureInPictureAdPosition;
import com.google.android.libraries.ads.mobile.sdk.pip.PictureInPictureAdRequest;

/** Native Java implementation for Picture-in-Picture Ads in the Google Mobile Ads Unity plugin. */
public class UnityPictureInPictureAd {

  private static final String LOG_TAG = "GoogleMobileAds";

  private final Activity activity;
  private final UnityPictureInPictureAdCallback callback;
  private final AdWrapper<PictureInPictureAd> adWrapper;
  @Nullable private PictureInPictureAd pipAd;

  public UnityPictureInPictureAd(Activity activity, UnityPictureInPictureAdCallback callback) {
    this(activity, callback, AdWrapper.forPictureInPicture());
  }

  @VisibleForTesting
  UnityPictureInPictureAd(
      Activity activity,
      UnityPictureInPictureAdCallback callback,
      AdWrapper<PictureInPictureAd> adWrapper) {
    this.activity = activity;
    this.callback = callback;
    this.adWrapper = adWrapper;
  }

  /**
   * Loads a Picture-in-Picture ad on the UI thread.
   *
   * @param adUnitId The ad unit ID to load.
   * @param adRequest The native AdRequest object.
   */
  public void load(String adUnitId, AdRequest adRequest) {
    activity.runOnUiThread(
        () -> {
          try {
            PictureInPictureAdRequest pipRequest =
                new PictureInPictureAdRequest.Builder(adUnitId).build();

            adWrapper.load(
                pipRequest,
                new AdLoadCallback<PictureInPictureAd>() {
                  @Override
                  public void onAdLoaded(@NonNull PictureInPictureAd ad) {
                    pipAd = ad;
                    pipAd.setAdEventCallback(
                        new PictureInPictureAdEventCallback() {
                          @Override
                          public void onAdShown() {
                            if (callback != null) {
                              callback.onAdShown();
                            }
                          }

                          @Override
                          public void onAdHidden() {
                            if (callback != null) {
                              callback.onAdHidden();
                            }
                          }

                          @Override
                          public void onAdClicked() {
                            if (callback != null) {
                              callback.onAdClicked();
                            }
                          }

                          @Override
                          public void onAdImpression() {
                            if (callback != null) {
                              callback.onAdImpression();
                            }
                          }

                          @Override
                          public void onAdShowedFullScreenContent() {
                            if (callback != null) {
                              callback.onAdShowedFullScreenContent();
                            }
                          }

                          @Override
                          public void onAdDismissedFullScreenContent() {
                            if (callback != null) {
                              callback.onAdDismissedFullScreenContent();
                            }
                          }

                          @Override
                          public void onAdPaid(@NonNull AdValue adValue) {
                            if (callback != null) {
                              callback.onPaidEvent(
                                  Util.getAdValuePrecisionType(adValue.getPrecisionType()),
                                  adValue.getValueMicros(),
                                  adValue.getCurrencyCode());
                            }
                          }
                        });

                    if (callback != null) {
                      callback.onAdLoaded();
                    }
                  }

                  @Override
                  public void onAdFailedToLoad(@NonNull LoadAdError loadAdError) {
                    if (callback != null) {
                      callback.onAdFailedToLoad(loadAdError);
                    }
                  }
                });
          } catch (Throwable t) {
            Log.e(LOG_TAG, "Failed to load PictureInPictureAd: " + t, t);
            if (callback != null) {
              callback.onAdFailedToLoad(
                  new LoadAdError(
                      LoadAdError.ErrorCode.INTERNAL_ERROR,
                      t.getMessage() != null ? t.getMessage() : t.toString(),
                      null));
            }
          }
        });
  }

  /**
   * Shows the Picture-in-Picture ad at the given corner position.
   *
   * @param positionOrdinal 0=BOTTOM_RIGHT, 1=BOTTOM_LEFT, 2=TOP_LEFT, 3=TOP_RIGHT.
   */
  public void show(int positionOrdinal) {
    activity.runOnUiThread(
        () -> {
          if (pipAd == null) {
            Log.e(LOG_TAG, "Failed to show PictureInPictureAd: ad is null.");
            return;
          }
          PictureInPictureAdPosition position;
          switch (positionOrdinal) {
            case 1:
              position = PictureInPictureAdPosition.BOTTOM_RIGHT;
              break;
            case 2:
              position = PictureInPictureAdPosition.BOTTOM_LEFT;
              break;
            case 3:
              position = PictureInPictureAdPosition.TOP_LEFT;
              break;
            case 4:
              position = PictureInPictureAdPosition.TOP_RIGHT;
              break;
            case 0:
            default:
              position = PictureInPictureAdPosition.DEFAULT;
              break;
          }
          PictureInPictureAdOptions options =
              new PictureInPictureAdOptions.Builder().setPosition(position).build();
          pipAd.show(activity, options);
        });
  }

  /** Hides the Picture-in-Picture ad. */
  public void hide() {
    activity.runOnUiThread(
        () -> {
          if (pipAd != null) {
            pipAd.hide();
          }
        });
  }

  /** Destroys the Picture-in-Picture ad and cleans up references. */
  public void destroy() {
    activity.runOnUiThread(
        () -> {
          if (pipAd != null) {
            pipAd.destroy();
            pipAd = null;
          }
        });
  }

  /** Returns the current or last known position ordinal (0=DEFAULT, 1=BOTTOM_RIGHT, etc.). */
  public int getAdPosition() {
    if (pipAd == null) {
      return 0;
    }
    PictureInPictureAdPosition position = pipAd.getPosition();
    if (position == null) {
      return 0;
    }
    switch (position) {
      case DEFAULT:
        return 0;
      case BOTTOM_RIGHT:
        return 1;
      case BOTTOM_LEFT:
        return 2;
      case TOP_LEFT:
        return 3;
      case TOP_RIGHT:
        return 4;
    }
    return 0;
  }

  /** Returns ResponseInfo for the loaded ad, if available. */
  @Nullable
  public ResponseInfo getResponseInfo() {
    return pipAd != null ? pipAd.getResponseInfo() : null;
  }
}
