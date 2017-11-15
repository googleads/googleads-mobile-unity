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

import android.app.Activity;
import android.graphics.Color;
import android.os.Build;
import android.util.Log;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;
import android.view.ViewParent;
import android.view.WindowManager;
import android.widget.PopupWindow;
import com.google.android.gms.ads.AdListener;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.AdSize;
import com.google.android.gms.ads.NativeExpressAdView;

/**
 * Native express ad implementation for the Google Mobile Ads Unity plugin.
 */
public class NativeExpressAd {

    /**
     * The {@link NativeExpressAdView} to display to the user.
     */
    private NativeExpressAdView mAdView;

    /**
     * The {@link Activity} that the native express ad will be displayed in.
     */
    private Activity mUnityPlayerActivity;

    /**
     * The {@code PopupWindow} that the banner ad be displayed in to ensure banner ads will be
     * presented over a {@code SurfaceView}.
     */
    private PopupWindow mPopupWindow;

    /**
     * A code indicating where to place the ad.
     */
    private int mPositionCode;

    /**
     * Offset for the ad in the x-axis when a custom position is used. Value will be 0 for
     * non-custom positions.
     */
    private int mHorizontalOffset;

    /**
     * Offset for the ad in the y-axis when a custom position is used. Value will be 0 for
     * non-custom positions.
     */
    private int mVerticalOffset;

    /**
     * A boolean indicating whether the ad has been hidden.
     */
    private boolean mHidden;

    /**
     * A listener implemented in Unity via {@code AndroidJavaProxy} to receive ad events.
     */
    private UnityAdListener mUnityListener;

    /**
     * Creates an instance of {@code NativeExpressAd}.
     *
     * @param activity The {@link Activity} that will contain an ad.
     * @param listener The {@link UnityAdListener} used to receive ad events in Unity.
     */
    public NativeExpressAd(Activity activity, UnityAdListener listener) {
        this.mUnityPlayerActivity = activity;
        this.mUnityListener = listener;
    }

    /**
     * Creates a {@link NativeExpressAdView} to hold native express ads.
     *
     * @param publisherId  Your ad unit ID.
     * @param adSize       The size of the native express ad.
     * @param positionCode A code indicating where to place the ad.
     */
    public void create(final String publisherId, final AdSize adSize, final int positionCode) {
        mUnityPlayerActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                createNativeExpressAdView(publisherId, adSize);
                createPopupWindow();
                mPositionCode = positionCode;
                mHorizontalOffset = 0;
                mVerticalOffset = 0;
                mHidden = false;
            }
        });
    }

    /**
     * Creates a {@link NativeExpressAdView} to hold native express ads with a custom position.
     *
     * @param publisherId Your ad unit ID.
     * @param adSize      The size of the native express ad.
     * @param positionX   Position of native express ad on the x axis.
     * @param positionY   Position of native express ad on the y axis.
     */
    public void create(final String publisherId, final AdSize adSize, final int positionX, final
    int positionY) {
        mUnityPlayerActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                createNativeExpressAdView(publisherId, adSize);
                createPopupWindow();
                mPositionCode = PluginUtils.POSITION_CUSTOM;
                mHorizontalOffset = positionX;
                mVerticalOffset = positionY;
                mHidden = false;
            }
        });
    }

    public void createNativeExpressAdView(final String publisherId, final AdSize adSize) {
        mAdView = new NativeExpressAdView(mUnityPlayerActivity);
        // Setting the background color works around an issue where the first ad isn't
        // visible.
        mAdView.setBackgroundColor(Color.TRANSPARENT);
        mAdView.setAdUnitId(publisherId);
        mAdView.setAdSize(adSize);
        mAdView.setAdListener(new AdListener() {
            @Override
            public void onAdLoaded() {
                if (mUnityListener != null) {
                    if (!mPopupWindow.isShowing() && !mHidden) {
                        showPopUpWindow();
                    }
                    mUnityListener.onAdLoaded();
                }
            }

            @Override
            public void onAdFailedToLoad(int errorCode) {
                if (mUnityListener != null) {
                    mUnityListener.onAdFailedToLoad(PluginUtils.getErrorReason(errorCode));
                }
            }

            @Override
            public void onAdOpened() {
                if (mUnityListener != null) {
                    mUnityListener.onAdOpened();
                }
            }

            @Override
            public void onAdClosed() {
                if (mUnityListener != null) {
                    mUnityListener.onAdClosed();
                }
            }

            @Override
            public void onAdLeftApplication() {
                if (mUnityListener != null) {
                    mUnityListener.onAdLeftApplication();
                }
            }
        });

    }

    public void createPopupWindow() {
        int popUpWindowWidth = mAdView.getAdSize().getWidthInPixels(mUnityPlayerActivity);
        int popUpWindowHeight = mAdView.getAdSize().getHeightInPixels(mUnityPlayerActivity);
        mPopupWindow = new PopupWindow(mAdView, popUpWindowWidth, popUpWindowHeight);

        // Copy system UI visibility flags set on Unity player window to newly created PopUpWindow.
        int visibilityFlags = mUnityPlayerActivity.getWindow().getAttributes().flags;
        mPopupWindow.getContentView().setSystemUiVisibility(visibilityFlags);

        // Workaround to prevent ad views from losing visibility on activity changes for certain
        // devices (eg. Huawei devices).
        PluginUtils.setPopUpWindowLayoutType(mPopupWindow,
                WindowManager.LayoutParams.TYPE_APPLICATION_SUB_PANEL);

    }

    private void showPopUpWindow() {
        View anchorView = mUnityPlayerActivity.getWindow().getDecorView().getRootView();

        if (this.mPositionCode == PluginUtils.POSITION_CUSTOM) {
            // Android Nougat has a PopUpWindow bug gravity doesn't position views as expected.
            // Using offset values as a workaround. On certain devices (ie. Samsung S8) calls to
            // update() cause the PopUpWindow to be rendered at the top of the screen. Using
            // showAsDropDown() instead of showAtLocation() (when possible) avoids this issue.
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.KITKAT) {
                mPopupWindow.showAsDropDown(anchorView,
                        (int) PluginUtils.convertDpToPixel(mHorizontalOffset),
                        -anchorView.getHeight()
                                + (int) PluginUtils.convertDpToPixel(mVerticalOffset));
            } else {
                mPopupWindow.showAtLocation(
                        anchorView, Gravity.NO_GRAVITY,
                        (int) PluginUtils.convertDpToPixel(mHorizontalOffset),
                        (int) PluginUtils.convertDpToPixel(mVerticalOffset));
            }
        } else {
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.KITKAT) {
                int adViewWidth = mAdView.getAdSize().getWidthInPixels(mUnityPlayerActivity);
                int adViewHeight = mAdView.getAdSize().getHeightInPixels(mUnityPlayerActivity);

                mPopupWindow.showAsDropDown(anchorView,
                        PluginUtils.getHorizontalOffsetForPositionCode(mPositionCode, adViewWidth,
                                anchorView.getWidth()),
                        PluginUtils.getVerticalOffsetForPositionCode(mPositionCode, adViewHeight,
                                anchorView.getHeight()));
            } else {
                mPopupWindow.showAtLocation(anchorView,
                        PluginUtils.getLayoutGravityForPositionCode(mPositionCode), 0, 0);
            }
        }
    }

    /**
     * Loads an ad on a background thread.
     *
     * @param request The {@link AdRequest} object with targeting parameters.
     */
    public void loadAd(final AdRequest request) {
        mUnityPlayerActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Log.d(PluginUtils.LOGTAG, "Calling loadAd() on NativeExpressAdView");
                mAdView.loadAd(request);
            }
        });
    }

    /**
     * Returns the mediation adapter class name. In the case of a mediated ad response, this is the
     * name of the class that was responsible for performing the ad request and rendering the ad.
     * For non-mediated responses, this value will be {@code null}.
     */
    public String getMediationAdapterClassName() {
        return mAdView != null ? mAdView.getMediationAdapterClassName() : null;
    }

    /**
     * Sets the ad size for the {@link NativeExpressAdView}.
     */
    public void setAdSize(final AdSize adSize) {
        mUnityPlayerActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                mAdView.setAdSize(adSize);
            }
        });
    }

    /**
     * Sets the {@link NativeExpressAdView} to be visible.
     */
    public void show() {
        mUnityPlayerActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Log.d(PluginUtils.LOGTAG, "Calling show() on NativeExpressAdView");
                mHidden = false;
                mAdView.setVisibility(View.VISIBLE);
                mPopupWindow.setTouchable(true);
                mPopupWindow.update();
                if (!mPopupWindow.isShowing()) {
                    showPopUpWindow();
                }
                mAdView.resume();
            }
        });
    }

    /**
     * Sets the {@link NativeExpressAdView} to be gone.
     */
    public void hide() {
        mUnityPlayerActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Log.d(PluginUtils.LOGTAG, "Calling hide() on NativeExpressAdView");
                mHidden = true;
                mAdView.setVisibility(View.GONE);
                mPopupWindow.setTouchable(false);
                mPopupWindow.update();
                mAdView.pause();
            }
        });
    }

    /**
     * Destroys the {@link NativeExpressAdView}.
     */
    public void destroy() {
        mUnityPlayerActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Log.d(PluginUtils.LOGTAG, "Calling destroy() on NativeExpressAdView");
                mAdView.destroy();
                mPopupWindow.dismiss();
                ViewParent parentView = mAdView.getParent();
                if (parentView != null && parentView instanceof ViewGroup) {
                    ((ViewGroup) parentView).removeView(mAdView);
                }
            }
        });
    }
}

