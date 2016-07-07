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
import android.graphics.Bitmap;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.drawable.Drawable;

import com.google.android.gms.ads.formats.NativeAd;
import com.google.android.gms.ads.formats.NativeCustomTemplateAd;

import java.io.ByteArrayOutputStream;
import java.util.List;

/**
 * Native AdLoader implementation for the Google Mobile Ads Unity plugin.
 */
public class CustomNativeAd {

    /**
     * The {@code Activity} on which the custom native template ad will display.
     */
    private Activity activity;

    /**
     * The {@link NativeCustomTemplateAd}.
     */
    private NativeCustomTemplateAd nativeAd;

    public CustomNativeAd(Activity activity, NativeCustomTemplateAd ad) {
        this.activity = activity;
        this.nativeAd = ad;
    }

    /**
     * Returns a list of all available assets.
     */
    public String[] getAvailableAssetNames() {
        List<String> assetNames = nativeAd.getAvailableAssetNames();
        return assetNames.toArray(new String[assetNames.size()]);
    }

    /**
     * Returns the ID of the custom template used to request this ad.
     */
    public String getTemplateId() {
        return nativeAd.getCustomTemplateId();
    }


    /**
     * Called when the user has clicked on the ad.
     *
     * @param assetName The name of the asset that was clicked.
     */
    public void performClick(final String assetName) {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                nativeAd.performClick(assetName);
            }
        });
    }

    /**
     * Record an impression for the custom template ad.
     */
    public void recordImpression() {
        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                nativeAd.recordImpression();
            }
        });
    }

    /**
     * Returns an image asset.
     *
     * @param key The name of the asset to be retrieved.
     */
    public byte[] getImage(String key) {
        NativeAd.Image imageAsset = nativeAd.getImage(key);
        if (imageAsset == null) {
            return new byte[0];
        }

        Drawable imageDrawable = imageAsset.getDrawable();
        Bitmap bitmap = ((BitmapDrawable) imageDrawable).getBitmap();
        ByteArrayOutputStream stream = new ByteArrayOutputStream();
        bitmap.compress(Bitmap.CompressFormat.PNG, 100, stream);
        return stream.toByteArray();
    }

    /**
     * Returns a string asset of numbers, URLs, or other types other than an image asset.
     *
     * @param key The name of the asset to be retrieved.
     */
    public String getText(String key) {
        CharSequence assetText = nativeAd.getText(key);
        if (assetText == null) {
            return "";
        }

        return nativeAd.getText(key).toString();
    }
}
