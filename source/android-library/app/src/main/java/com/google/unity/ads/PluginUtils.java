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

import android.content.res.Resources;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.Gravity;
import android.widget.PopupWindow;

import com.google.android.gms.ads.AdRequest;

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;

/**
 * Utilities for the Google Mobile Ads Unity plugin.
 */
public class PluginUtils {

    /**
     * Tag used for logging statements.
     */
    public static final String LOGTAG = "AdsUnity";

    /**
     * Position constant for a position with a custom offset.
     */
    public static final int POSITION_CUSTOM = -1;

    /**
     * Position constant for top of the screen.
     */
    private static final int POSITION_TOP = 0;

    /**
     * Position constant for bottom of the screen.
     */
    private static final int POSITION_BOTTOM = 1;

    /**
     * Position constant for top-left of the screen.
     */
    private static final int POSITION_TOP_LEFT = 2;

    /**
     * Position constant for top-right of the screen.
     */
    private static final int POSITION_TOP_RIGHT = 3;

    /**
     * Position constant for bottom-left of the screen.
     */
    private static final int POSITION_BOTTOM_LEFT = 4;

    /**
     * Position constant bottom-right of the screen.
     */
    private static final int POSITION_BOTTOM_RIGHT = 5;

    /**
     * Position constant center of the screen.
     */
    private static final int POSITION_CENTER = 6;

    /**
     * Gets a string error reason from an error code.
     *
     * @param errorCode The error code.
     * @return The reason for the error.
     */
    public static String getErrorReason(int errorCode) {
        switch (errorCode) {
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
     * Returns a {@link Gravity} constant corresponding to a positionCode.
     *
     * @param positionCode A code indicating where to place the ad.
     * @return {@link Gravity} constant corresponding to positionCode argument.
     */
    public static int getLayoutGravityForPositionCode(int positionCode) {
        int gravity;
        switch (positionCode) {
            case POSITION_TOP:
                gravity = Gravity.TOP | Gravity.CENTER_HORIZONTAL;
                break;
            case POSITION_BOTTOM:
                gravity = Gravity.BOTTOM | Gravity.CENTER_HORIZONTAL;
                break;
            case POSITION_TOP_LEFT:
                gravity = Gravity.TOP | Gravity.LEFT;
                break;
            case POSITION_TOP_RIGHT:
                gravity = Gravity.TOP | Gravity.RIGHT;
                break;
            case POSITION_BOTTOM_LEFT:
                gravity = Gravity.BOTTOM | Gravity.LEFT;
                break;
            case POSITION_BOTTOM_RIGHT:
                gravity = Gravity.BOTTOM | Gravity.RIGHT;
                break;
            case POSITION_CENTER:
                gravity = Gravity.CENTER_HORIZONTAL | Gravity.CENTER_VERTICAL;
                break;
            default:
                throw new IllegalArgumentException("Attempted to position ad with invalid ad "
                        + "position.");
        }
        return gravity;
    }

    public static int getHorizontalOffsetForPositionCode(int positionCode, int viewWidth,
                                                         int anchorWidth) {
        int offset;
        switch (positionCode) {
            case POSITION_TOP_LEFT:
            case POSITION_BOTTOM_LEFT:
                offset = 0;
                break;
            case POSITION_TOP_RIGHT:
            case POSITION_BOTTOM_RIGHT:
                offset = anchorWidth - viewWidth;
                break;
            case POSITION_TOP:
            case POSITION_BOTTOM:
            case POSITION_CENTER:
                offset = (anchorWidth - viewWidth) / 2;
                break;
            // Make the center position the default horizontal position.
            default:
                Log.w(LOGTAG, "Attempted to position ad with invalid ad "
                        + "position. Using default center horizontal position.");
                offset = (anchorWidth - viewWidth) / 2;
        }

        return offset;
    }

    /**
     * Returns the vertical offset using a bottom left co-ordinate system.
     * i.e top is at -anchorHeight.
     * @param positionCode the position code to use
     * @param viewHeight the height of the view
     * @param anchorHeight the height of the anchoring view to position in
     * @return the vertical offset relative to the bottom of the anchorview.
     */
    public static int getVerticalOffsetForPositionCode(int positionCode, int viewHeight,
                                                       int anchorHeight) {
        int offset;
        switch (positionCode) {
            case POSITION_TOP:
            case POSITION_TOP_LEFT:
            case POSITION_TOP_RIGHT:
                offset = -anchorHeight;
                break;
            case POSITION_CENTER:
                offset = (-anchorHeight - viewHeight) / 2;
                break;
            case POSITION_BOTTOM:
            case POSITION_BOTTOM_LEFT:
            case POSITION_BOTTOM_RIGHT:
                offset = -viewHeight;
                break;
            // Make the bottom position the default vertical position.
            default:
                Log.w(LOGTAG, "Attempted to position ad with invalid ad "
                        + "position. Using default bottom vertical position.");
                offset = -viewHeight;
        }

        return offset;
    }

    public static float convertPixelsToDp(float px) {
        DisplayMetrics metrics = Resources.getSystem().getDisplayMetrics();
        return px / metrics.density;
    }

    public static float convertDpToPixel(float dp) {
        DisplayMetrics metrics = Resources.getSystem().getDisplayMetrics();
        return dp * metrics.density;
    }

    public static void setPopUpWindowLayoutType(PopupWindow popupWindow, int layoutType) {
        try {
            Method method = PopupWindow.class.getDeclaredMethod("setWindowLayoutType", int.class);
            method.setAccessible(true);
            method.invoke(popupWindow, layoutType);
        } catch (NoSuchMethodException exception) {
            Log.w(LOGTAG, String.format("Unable to set popUpWindow window layout type: %s",
                    exception.getLocalizedMessage()));
        } catch (IllegalAccessException exception) {
            Log.w(LOGTAG, String.format("Unable to set popUpWindow window layout type: %s",
                    exception.getLocalizedMessage()));
        } catch (InvocationTargetException exception) {
            Log.d(LOGTAG, String.format("Unable to set popUpWindow window layout type: %s",
                    exception.getLocalizedMessage()));
        }
    }

}
