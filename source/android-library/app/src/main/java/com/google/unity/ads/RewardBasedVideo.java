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
import android.os.Bundle;
import android.util.Log;
import android.widget.Toast;

import com.google.ads.mediation.admob.AdMobAdapter;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.MobileAds;
import com.google.android.gms.ads.reward.RewardItem;
import com.google.android.gms.ads.reward.RewardedVideoAd;
import com.google.android.gms.ads.reward.RewardedVideoAdListener;

/**
 * Native reward based video ad implementation for the Google Mobile Ads Unity plugin.
 */
public class RewardBasedVideo {

    /**
     * The {@link RewardedVideoAd}.
     */
    private RewardedVideoAd rewardBasedVideo;

    /**
     * The {@code Activity} on which the reward based video ad will display.
     */
    private Activity activity;

    /**
     * A listener implemented in Unity via {@code AndroidJavaProxy} to receive ad events.
     */
    private UnityRewardBasedVideoAdListener adListener;

    /**
     * Whether or not the {@link RewardedVideoAd} is ready to be shown.
     */
    private boolean isLoaded;

    public RewardBasedVideo(Activity activity, UnityRewardBasedVideoAdListener adListener) {
        this.activity = activity;
        this.adListener = adListener;
        this.isLoaded = false;
    }

    /**
     * Creates a {@link RewardedVideoAd}.
     */
    public void create() {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                rewardBasedVideo = MobileAds.getRewardedVideoAdInstance(activity);
                rewardBasedVideo.setRewardedVideoAdListener(new RewardedVideoAdListener() {
                    @Override
                    public void onRewardedVideoAdLoaded() {
                        isLoaded = true;
                        adListener.onAdLoaded();
                    }

                    @Override
                    public void onRewardedVideoAdFailedToLoad(int errorCode) {
                        adListener.onAdFailedToLoad(PluginUtils.getErrorReason(errorCode));
                    }

                    @Override
                    public void onRewardedVideoAdOpened() {
                        adListener.onAdOpened();
                    }

                    @Override
                    public void onRewardedVideoStarted() {
                        adListener.onAdStarted();
                    }

                    @Override
                    public void onRewardedVideoAdClosed() {
                        adListener.onAdClosed();
                    }

                    @Override
                    public void onRewarded(RewardItem reward) {
                        adListener.onAdRewarded(reward.getType(), reward.getAmount());
                        Toast.makeText(activity,
                                String.format(" onRewarded! currency: %s amount: %d",
                                        reward.getType(),
                                        reward.getAmount()),
                                Toast.LENGTH_SHORT).show();
                    }

                    @Override
                    public void onRewardedVideoAdLeftApplication() {
                        adListener.onAdLeftApplication();
                    }
                });
            }
        });
    }

    /**
     * Loads a reward based video ad.
     *
     * @param request  The {@link AdRequest} object with targeting parameters.
     * @param adUnitId Your reward based video ad unit ID.
     */
    public void loadAd(final AdRequest request, final String adUnitId) {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Bundle extras = request.getNetworkExtrasBundle(AdMobAdapter.class);
                // Bundle should be present as it is set by Unity plugin.
                if (extras != null) {
                    extras.putBoolean("_noRefresh", true);
                }
                rewardBasedVideo.loadAd(adUnitId, request);
            }
        });
    }

    /**
     * Returns {@code True} if the reward based video ad has loaded.
     */
    public boolean isLoaded() {
        return isLoaded;
    }

    /**
     * Shows the reward based video ad if it has loaded.
     */
    public void show() {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (rewardBasedVideo.isLoaded()) {
                    isLoaded = false;
                    rewardBasedVideo.show();
                } else {
                    Log.w(PluginUtils.LOGTAG, "Reward based video ad is not ready to be shown.");
                }
            }
        });
    }

    /**
     * Destroys the {@link RewardedVideoAd}.
     */
    public void destroy() {
        rewardBasedVideo.destroy();
    }
}

