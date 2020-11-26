package com.google.unity.ads;

import android.app.Activity;
import android.util.Log;
import androidx.annotation.NonNull;
import com.google.android.gms.ads.AdError;
import com.google.android.gms.ads.AdRequest;
import com.google.android.gms.ads.AdValue;
import com.google.android.gms.ads.LoadAdError;
import com.google.android.gms.ads.OnPaidEventListener;
import com.google.android.gms.ads.ResponseInfo;
import com.google.android.gms.ads.rewarded.RewardItem;
import com.google.android.gms.ads.rewarded.RewardedAd;
import com.google.android.gms.ads.rewarded.RewardedAdCallback;
import com.google.android.gms.ads.rewarded.RewardedAdLoadCallback;
import com.google.android.gms.ads.rewarded.ServerSideVerificationOptions;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.FutureTask;

/**
 * Native rewarded ad implementation for the Google Mobile Ads Unity plugin.
 */
public class UnityRewardedAd {

    /**
     * The {@link RewardedAd}.
     */
    private RewardedAd rewardedAd;

    /**
     * The {@code Activity} on which the rewarded ad will display.
     */
    private Activity activity;

    /**
     * A callback implemented in Unity via {@code AndroidJavaProxy} to receive ad events.
     */
    private UnityRewardedAdCallback callback;


    public UnityRewardedAd(Activity activity, UnityRewardedAdCallback callback) {
        this.activity = activity;
        this.callback = callback;
    }

    /**
     * Creates a {@link RewardedAd}.
     */
    public void create(final String adUnitID) {
        activity.runOnUiThread(
                new Runnable() {
                    @Override
                    public void run() {
                        rewardedAd = new RewardedAd(activity, adUnitID);
                        rewardedAd.setOnPaidEventListener(
                                new OnPaidEventListener() {
                                    @Override
                                    public void onPaidEvent(final AdValue adValue) {
                                        if (callback != null) {
                                            new Thread(
                                                    new Runnable() {
                                                        @Override
                                                        public void run() {
                                                            if (callback != null) {
                                                                callback.onPaidEvent(
                                                                        adValue.getPrecisionType(),
                                                                        adValue.getValueMicros(),
                                                                        adValue.getCurrencyCode());
                                                            }
                                                        }
                                                    })
                                                    .start();
                                        }
                                    }
                                });
                    }
                });
    }

    /**
     * Loads a rewarded ad.
     *
     * @param request The {@link AdRequest} object with targeting parameters.
     */
    public void loadAd(final AdRequest request) {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                rewardedAd.loadAd(request, new RewardedAdLoadCallback() {
                    @Override
                    public void onRewardedAdLoaded() {
                        if (callback != null) {
                            new Thread(new Runnable() {
                                @Override
                                public void run() {
                                    if (callback != null) {
                                        callback.onRewardedAdLoaded();
                                    }
                                }
                            }).start();
                        }
                    }

                    @Override
                    public void onRewardedAdFailedToLoad(final LoadAdError error) {
                        if (callback != null) {
                            new Thread(new Runnable() {
                                @Override
                                public void run() {
                                    if (callback != null) {
                                        callback.onRewardedAdFailedToLoad(error);
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
     * Returns {@code true} if the rewarded ad has loaded.
     */
    public boolean isLoaded() {
        FutureTask<Boolean> task = new FutureTask<>(new Callable<Boolean>() {
            @Override
            public Boolean call() {
                return rewardedAd.isLoaded();
            }
        });
        activity.runOnUiThread(task);

        boolean result = false;
        try {
            result = task.get();
        } catch (InterruptedException e) {
            Log.e(PluginUtils.LOGTAG,
                    String.format("Unable to check if rewarded as has loaded: %s",
                            e.getLocalizedMessage()));
        } catch (ExecutionException e) {
            Log.e(PluginUtils.LOGTAG,
                    String.format("Unable to check if rewarded as has loaded: %s",
                            e.getLocalizedMessage()));
        }
        return result;
    }

    /**
     * Shows the rewarded ad if it has loaded.
     */
    public void show() {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (rewardedAd.isLoaded()) {
                    rewardedAd.show(activity, new RewardedAdCallback() {
                        @Override
                        public void onRewardedAdOpened() {
                            if (callback != null) {
                                new Thread(new Runnable() {
                                    @Override
                                    public void run() {
                                        if (callback != null) {
                                            callback.onRewardedAdOpened();
                                        }
                                    }
                                }).start();
                            }
                        }

                        @Override
                        public void onRewardedAdClosed() {
                            if (callback != null) {
                                new Thread(new Runnable() {
                                    @Override
                                    public void run() {
                                        if (callback != null) {
                                            callback.onRewardedAdClosed();
                                        }
                                    }
                                }).start();
                            }
                        }

                        @Override
                        public void onUserEarnedReward(@NonNull final RewardItem reward) {
                            if (callback != null) {
                                new Thread(new Runnable() {
                                    @Override
                                    public void run() {
                                        if (callback != null) {
                                            callback.onUserEarnedReward(reward.getType(),
                                                    reward.getAmount());
                                        }
                                    }
                                }).start();
                            }
                        }

                        @Override
                        public void onRewardedAdFailedToShow(final AdError error) {
                            if (callback != null) {
                                new Thread(new Runnable() {
                                    @Override
                                    public void run() {
                                        if (callback != null) {
                                            callback.onRewardedAdFailedToShow(error);
                                        }
                                    }
                                }).start();
                            }
                        }
                    });
                } else {
                    Log.w(PluginUtils.LOGTAG, "Rewarded ad is not ready to be shown.");
                }
            }
        });
    }

    /** Sets server side verification options. */
    public void setServerSideVerificationOptions(
        final ServerSideVerificationOptions serverSideVerificationOptions) {
        activity.runOnUiThread(
            new Runnable() {
                @Override
                public void run() {
                    rewardedAd.setServerSideVerificationOptions(serverSideVerificationOptions);
                }
            });
    }

    public String getMediationAdapterClassName() {
        FutureTask<String> task = new FutureTask<>(new Callable<String>() {
            @Override
            public String call() {
                return rewardedAd.getMediationAdapterClassName();
            }
        });
        activity.runOnUiThread(task);

        String result = null;
        try {
            result = task.get();
        } catch (InterruptedException e) {
            Log.e(PluginUtils.LOGTAG,
                    String.format("Unable to check rewarded ad adapter class name: %s",
                            e.getLocalizedMessage()));
        } catch (ExecutionException e) {
            Log.e(PluginUtils.LOGTAG,
                    String.format("Unable to check rewarded ad adapter class name: %s",
                            e.getLocalizedMessage()));
        }
        return result;
    }

    /**
     * Returns the request response info.
     */
    public ResponseInfo getResponseInfo() {
        FutureTask<ResponseInfo> task = new FutureTask<>(new Callable<ResponseInfo>() {
            @Override
            public ResponseInfo call() {
                return rewardedAd.getResponseInfo();
            }
        });
        activity.runOnUiThread(task);

        ResponseInfo result = null;
        try {
            result = task.get();
        } catch (InterruptedException exception) {
            Log.e(PluginUtils.LOGTAG,
                    String.format("Unable to check unity rewarded ad response info: %s",
                            exception.getLocalizedMessage()));
        } catch (ExecutionException exception) {
            Log.e(PluginUtils.LOGTAG,
                    String.format("Unable to check unity rewarded ad response info: %s",
                            exception.getLocalizedMessage()));
        }
        return result;
    }

    public RewardItem getRewardItem() {
        FutureTask<RewardItem> task = new FutureTask<>(new Callable<RewardItem>() {
            @Override
            public RewardItem call() {
                return rewardedAd.getRewardItem();
            }
        });
        activity.runOnUiThread(task);

        RewardItem result = null;
        try {
            result = task.get();
        } catch (InterruptedException e) {
            Log.e(PluginUtils.LOGTAG,
                    String.format("Unable to get rewarded ad reward item: %s",
                            e.getLocalizedMessage()));
        } catch (ExecutionException e) {
            Log.e(PluginUtils.LOGTAG,
                    String.format("Unable to get rewarded ad reward item: %s",
                            e.getLocalizedMessage()));
        }
        return result;
    }
}
