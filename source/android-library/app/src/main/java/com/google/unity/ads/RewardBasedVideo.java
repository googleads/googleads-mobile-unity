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
                        if (adListener != null) {
                            new Thread(new Runnable() {
                                @Override
                                public void run() {
                                    if (adListener != null) {
                                        adListener.onAdLoaded();
                                    }
                                }
                            }).start();
                        }
                    }

                    @Override
                    public void onRewardedVideoAdFailedToLoad(final int errorCode) {
                        if (adListener != null) {
                            new Thread(new Runnable() {
                                @Override
                                public void run() {
                                    if (adListener != null) {
                                        adListener.onAdFailedToLoad(
                                                PluginUtils.getErrorReason(errorCode));
                                    }
                                }
                            }).start();
                        }
                    }

                    @Override
                    public void onRewardedVideoAdOpened() {
                        if (adListener != null) {
                            new Thread(new Runnable() {
                                @Override
                                public void run() {
                                    if (adListener != null) {
                                        adListener.onAdOpened();
                                    }
                                }
                            }).start();
                        }
                    }

                    @Override
                    public void onRewardedVideoStarted() {
                        if (adListener != null) {
                            new Thread(new Runnable() {
                                @Override
                                public void run() {
                                    if (adListener != null) {
                                        adListener.onAdStarted();
                                    }
                                }
                            }).start();
                        }
                    }

                    @Override
                    public void onRewardedVideoAdClosed() {
                        if (adListener != null) {
                            new Thread(new Runnable() {
                                @Override
                                public void run() {
                                    if (adListener != null) {
                                        adListener.onAdClosed();
                                    }
                                }
                            }).start();
                        }
                    }

                    @Override
                    public void onRewarded(final RewardItem reward) {
                        if (adListener != null) {
                            new Thread(new Runnable() {
                                @Override
                                public void run() {
                                    if (adListener != null) {
                                        adListener.onAdRewarded(reward.getType(),
                                                reward.getAmount());
                                    }
                                }
                            }).start();
                        }
                    }

                    @Override
                    public void onRewardedVideoAdLeftApplication() {
                        if (adListener != null) {
                            new Thread(new Runnable() {
                                @Override
                                public void run() {
                                    if (adListener != null) {
                                        adListener.onAdLeftApplication();
                                    }
                                }
                            }).start();
                        }
                    }

                    @Override
                    public void onRewardedVideoCompleted() {
                        if (adListener != null) {
                            new Thread(new Runnable() {
                                @Override
                                public void run() {
                                    if (adListener != null) {
                                        adListener.onAdCompleted();
                                    }
                                }
                            }).start();
                        }
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
     * Sets the user ID to be used in server-to-server reward callbacks.
     */
    public void setUserId(final String userId) {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                rewardBasedVideo.setUserId(userId);
            }
        });
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
     * Returns the mediation adapter class name. In the case of a mediated ad response, this is the
     * name of the class that was responsible for performing the ad request and rendering the ad.
     * For non-mediated responses, this value will be {@code null}.
     */
    public String getMediationAdapterClassName() {
        return rewardBasedVideo != null ? rewardBasedVideo.getMediationAdapterClassName() : null;
    }

    /**
     * Destroys the {@link RewardedVideoAd}.
     */
    public void destroy() {
        // Currently there is no destroy() method for the RewardedVideoAd class. This method is a
        // placeholder in case there is any cleanup to do here in the future.
    }
}

