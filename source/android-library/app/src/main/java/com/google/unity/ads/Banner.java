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
import com.google.android.gms.ads.AdView;

/**
 * This class represents the native implementation for the Google Mobile Ads Unity plugin. This
 * class is used to request Google Mobile ads natively via the Google Mobile Ads library in Google
 * Play services. The Google Play services library is a dependency for this plugin.
 */
public class Banner {

    /**
     * The {@link AdView} to display to the user.
     */
    private AdView mAdView;

    /**
     * The {@code Activity} that the banner will be displayed in.
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
     * Creates an instance of {@code Banner}.
     *
     * @param activity The {@link Activity} that will contain an ad.
     * @param listener The {@link UnityAdListener} used to receive synchronous ad events in
     *                 Unity.
     */
    public Banner(Activity activity, UnityAdListener listener) {
        this.mUnityPlayerActivity = activity;
        this.mUnityListener = listener;
    }

    /**
     * Creates an {@link AdView} to hold banner ads.
     *
     * @param publisherId  Your ad unit ID.
     * @param adSize       The size of the banner.
     * @param positionCode A code indicating where to place the ad.
     */
    public void create(final String publisherId, final AdSize adSize, final int positionCode) {
        mUnityPlayerActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                createAdView(publisherId, adSize);
                createPopupWindow();
                mHorizontalOffset = 0;
                mVerticalOffset = 0;
                mPositionCode = positionCode;
                mHidden = false;
            }
        });
    }

    /**
     * Creates an {@link AdView} to hold banner ads with a custom position.
     *
     * @param publisherId Your ad unit ID.
     * @param adSize      The size of the banner.
     * @param positionX   Position of banner ad on the x axis.
     * @param positionY   Position of banner ad on the y axis.
     */
    public void create(final String publisherId, final AdSize adSize, final int positionX, final
    int positionY) {
        mUnityPlayerActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                createAdView(publisherId, adSize);
                createPopupWindow();
                mPositionCode = PluginUtils.POSITION_CUSTOM;
                mHorizontalOffset = positionX;
                mVerticalOffset = positionY;
                mHidden = false;
            }
        });
    }

    public void createAdView(final String publisherId, final AdSize adSize) {
        mAdView = new AdView(mUnityPlayerActivity);
        // Setting the background color works around an issue where the first ad isn't visible.
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

    private void createPopupWindow() {
        // Workaround for issue where popUpWindow will not resize to the full width
        // of the screen to accommodate a smart banner.
        int popUpWindowWidth = mAdView.getAdSize().equals(AdSize.SMART_BANNER)
                ? ViewGroup.LayoutParams.MATCH_PARENT
                : mAdView.getAdSize().getWidthInPixels(mUnityPlayerActivity);
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
        if (this.mPositionCode == PluginUtils.POSITION_CUSTOM) {
            mPopupWindow.showAtLocation(
                    mUnityPlayerActivity.getWindow().getDecorView().getRootView(),
                    Gravity.NO_GRAVITY, (int) PluginUtils.convertDpToPixel(mHorizontalOffset),
                    (int) PluginUtils.convertDpToPixel(mVerticalOffset));
        } else {
            mPopupWindow.showAtLocation(mUnityPlayerActivity.getWindow().getDecorView()
                            .getRootView(),
                    PluginUtils.getLayoutGravityForPositionCode(mPositionCode), 0, 0);
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
                Log.d(PluginUtils.LOGTAG, "Calling loadAd() on Android");
                mAdView.loadAd(request);
            }
        });
    }

    /**
     * Sets the {@link AdView} to be visible.
     */
    public void show() {
        mUnityPlayerActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Log.d(PluginUtils.LOGTAG, "Calling show() on Android");
                mHidden = false;
                mAdView.setVisibility(View.VISIBLE);
                if (!mPopupWindow.isShowing()) {
                    showPopUpWindow();
                }
                mPopupWindow.setTouchable(true);
                mPopupWindow.update();
                mAdView.resume();
            }
        });
    }

    /**
     * Sets the {@link AdView} to be gone.
     */
    public void hide() {
        mUnityPlayerActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Log.d(PluginUtils.LOGTAG, "Calling hide() on Android");
                mHidden = true;
                mAdView.setVisibility(View.GONE);
                mPopupWindow.setTouchable(false);
                mPopupWindow.update();
                mAdView.pause();
            }
        });
    }

    /**
     * Destroys the {@link AdView}.
     */
    public void destroy() {
        mUnityPlayerActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Log.d(PluginUtils.LOGTAG, "Calling destroy() on Android");
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
