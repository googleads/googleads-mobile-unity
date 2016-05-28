/*
 * Copyright (C) 2016 Google, Inc.
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

import com.google.android.gms.ads.AdListener;
import com.google.android.gms.ads.AdLoader;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.formats.NativeCustomTemplateAd;

/**
 * Native AdLoader implementation for the Google Mobile Ads Unity plugin.
 */
public class NativeAdLoader {

    /**
     * The {@code Activity} on which the native ads will display.
     */
    private Activity mActivity;

    /**
     * The {@code AdLoader} used to load native ads.
     */
    private AdLoader mAdLoader;

    /**
     * The {@code AdLoader.Builder} used to construct a AdLoader.
     */
    private AdLoader.Builder mAdLoaderBuilder;

    /**
     * A listener implemented in Unity via {@code AndroidJavaProxy} to receive ad events.
     */
    private UnityCustomNativeAdListener mListener;


    public NativeAdLoader(final Activity activity, final String adUnitId,
                          UnityCustomNativeAdListener listener) {
        this.mActivity = activity;
        this.mListener = listener;

        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                mAdLoaderBuilder = new AdLoader.Builder(activity, adUnitId);
            }
        });
    }

    /**
     * Creates a {@link AdLoader.Builder}.
     */
    public void create() {
        mActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                mAdLoader = mAdLoaderBuilder.build();
            }
        });
    }

    /**
     * Configure ad loader to request custom native template ad.
     *
     * @param templateID       Custom template Id defined in the DFP front end.
     * @param useClickListener Set whether to use optional listener that handles custom clicks on
     *                         custom template ads.
     */
    public void configureCustomNativeTemplateAd(final String templateID, final boolean
            useClickListener) {
        mActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                NativeCustomTemplateAd.OnCustomClickListener clickListener = null;
                if (useClickListener) {
                    clickListener = new NativeCustomTemplateAd.OnCustomClickListener() {
                        @Override
                        public void onCustomClick(NativeCustomTemplateAd ad, String assetName) {
                            mListener.onCustomClick(new CustomNativeAd(mActivity, ad), assetName);
                        }
                    };
                }

                mAdLoaderBuilder = mAdLoaderBuilder
                        .forCustomTemplateAd(templateID,
                                new NativeCustomTemplateAd.OnCustomTemplateAdLoadedListener() {
                                    @Override
                                    public void onCustomTemplateAdLoaded(
                                            NativeCustomTemplateAd ad) {
                                        mListener.onCustomTemplateAdLoaded(new CustomNativeAd
                                                (mActivity, ad));
                                    }
                                }, clickListener)
                        .withAdListener(new AdListener() {
                            @Override
                            public void onAdFailedToLoad(int errorCode) {
                                mListener.onAdFailedToLoad(PluginUtils.getErrorReason(errorCode));
                            }
                        });
            }
        });
    }

    /**
     * Send a request for an ad.
     */
    public void loadAd(final AdRequest request) {
        mActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                mAdLoader.loadAd(request);
            }
        });
    }
}
