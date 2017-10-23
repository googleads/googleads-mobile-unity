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
import android.util.Log;

import com.google.android.gms.ads.AdListener;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.InterstitialAd;

/**
 * Native interstitial implementation for the Google Mobile Ads Unity plugin.
 */
public class Interstitial {
    /**
     * The {@link InterstitialAd}.
     */
    private InterstitialAd interstitial;

    /**
     * The {@code Activity} on which the interstitial will display.
     */
    private Activity activity;

    /**
     * A listener implemented in Unity via {@code AndroidJavaProxy} to receive ad events.
     */
    private UnityAdListener adListener;

    /**
     * Whether or not the {@link InterstitialAd} is ready to be shown.
     */
    private boolean isLoaded;

    public Interstitial(Activity activity, UnityAdListener adListener) {
        this.activity = activity;
        this.adListener = adListener;
        this.isLoaded = false;
    }

    /**
     * Creates an {@link InterstitialAd}.
     *
     * @param adUnitId Your interstitial ad unit ID.
     */
    public void create(final String adUnitId) {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                interstitial = new InterstitialAd(activity);
                interstitial.setAdUnitId(adUnitId);
                interstitial.setAdListener(new AdListener() {
                    @Override
                    public void onAdLoaded() {
                        isLoaded = true;
                        adListener.onAdLoaded();
                    }

                    @Override
                    public void onAdFailedToLoad(int errorCode) {
                        adListener.onAdFailedToLoad(PluginUtils.getErrorReason(errorCode));
                    }

                    @Override
                    public void onAdOpened() {
                        adListener.onAdOpened();
                    }

                    @Override
                    public void onAdClosed() {
                        adListener.onAdClosed();
                    }

                    @Override
                    public void onAdLeftApplication() {
                        adListener.onAdLeftApplication();
                    }
                });
            }
        });
    }

    /**
     * Loads an interstitial ad.
     *
     * @param request The {@link AdRequest} object with targeting parameters.
     */
    public void loadAd(final AdRequest request) {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                interstitial.loadAd(request);
            }
        });
    }

    /**
     * Returns the mediation adapter class name. In the case of a mediated ad response, this is the
     * name of the class that was responsible for performing the ad request and rendering the ad.
     * For non-mediated responses, this value will be {@code null}.
     */
    public String getMediationAdapterClassName() {
        return interstitial != null ? interstitial.getMediationAdapterClassName() : null;
    }

    /**
     * Returns {@code True} if the interstitial has loaded.
     */
    public boolean isLoaded() {
        return isLoaded;
    }

    /**
     * Shows the interstitial if it has loaded.
     */
    public void show() {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (interstitial.isLoaded()) {
                    isLoaded = false;
                    interstitial.show();
                } else {
                    Log.d(PluginUtils.LOGTAG, "Interstitial was not ready to be shown.");
                }
            }
        });
    }

    /**
     * Destroys the {@link InterstitialAd}.
     */
    public void destroy() {
        // Currently there is no interstitial.destroy() method. This method is a placeholder in case
        // there is any cleanup to do here in the future.
    }
}
