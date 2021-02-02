// Copyright (C) 2019 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;

using UnityEditor;
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Placement;

namespace GoogleMobileAds.Editor
{
    [CustomPropertyDrawer(typeof(AdSizeProperty))]
    public class AdSizePropertyDrawer : PropertyDrawer
    {
        private const string BANNER = "banner";

        private const string MEDIUM_RECTANGLE = "medium_rectangle";

        private const string IAB_BANNER = "iab_banner";

        private const string LEADERBOARD = "leaderboard";

        private const string SMART_BANNER = "smart";

        private const string ANCHORED_ADAPTIVE_BANNER = "anchored_adaptive";

        private const string CUSTOM = "custom";

        private readonly string[] adSizeEntries = new string[]
        {
            "Banner (320x50)", "Medium Rectangle (300x250)", "IAB Banner (468x60)",
            "Leaderboard (728x90)", "Smart Banner", "Anchored Adaptive Banner", "Custom"
        };

        private readonly string[] adSizeValues = new string[]
        {
            BANNER, MEDIUM_RECTANGLE, IAB_BANNER,
            LEADERBOARD, SMART_BANNER, ANCHORED_ADAPTIVE_BANNER, CUSTOM
        };

        private float propertyHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            propertyHeight = 0;

            SerializedProperty propType = property.FindPropertyRelative("type");
            SerializedProperty propWidth = property.FindPropertyRelative("width");
            SerializedProperty propHeight = property.FindPropertyRelative("height");
            SerializedProperty propOrientation = property.FindPropertyRelative("orientation");

            string currentAdSize = ResolveAdSize(
                    propWidth.intValue, propHeight.intValue, (AdSize.Type)propType.intValue);

            string newAdSize = adSizeValues[EditorGUI.Popup(
                    GetNextFieldPositionRect(position), "Size",
                    ArrayIndexOf(adSizeValues, currentAdSize), adSizeEntries)];

            if (currentAdSize != newAdSize)
            {
            }

            if (ANCHORED_ADAPTIVE_BANNER.Equals(newAdSize))
            {
                if (!currentAdSize.Equals(newAdSize))
                {
                    UpdateProperty(propWidth, AdSize.FullWidth);
                }
                UpdateProperty(propType, AdSize.Type.AnchoredAdaptive);

                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(GetNextFieldPositionRect(position), propOrientation);

                bool isFullWidth = EditorGUI.Toggle(
                        GetNextFieldPositionRect(position), "Use full screen width",
                        propWidth.intValue == AdSize.FullWidth);

                if (!isFullWidth)
                {
                    EditorGUI.indentLevel++;
                    int currentWidth = propWidth.intValue < 50 ? 50 : propWidth.intValue;
                    int newWidth = EditorGUI.IntSlider(
                        GetNextFieldPositionRect(position), "% width of the screen", currentWidth, 50, 99);

                    UpdateProperty(propWidth, newWidth);
                    EditorGUI.indentLevel--;
                }
                else
                {
                    UpdateProperty(propWidth, AdSize.FullWidth);
                }
                EditorGUI.indentLevel--;
            }
            else if (CUSTOM.Equals(newAdSize))
            {
                EditorGUI.indentLevel++;

                if (!currentAdSize.Equals(newAdSize))
                {
                    UpdateProperty(propWidth, 0);
                    UpdateProperty(propHeight, 0);
                }

                Vector2Int newBannerSize = EditorGUI.Vector2IntField(
                    GetNextFieldPositionRect(position), "Custom size (dp)",
                    new Vector2Int(propWidth.intValue, propHeight.intValue));

                if (newBannerSize.x <= 0 || newBannerSize.y <= 0)
                {
                    EditorGUILayout.HelpBox("Invalid ad size.", MessageType.Error);
                }

                UpdateProperty(propWidth, newBannerSize.x);
                UpdateProperty(propHeight, newBannerSize.y);
                UpdateProperty(propOrientation, (int)Orientation.Current);

                EditorGUI.indentLevel--;
            }
            else
            {
                switch (newAdSize)
                {
                    case BANNER:
                        UpdateProperty(propType, AdSize.Type.Standard);
                        UpdateProperty(propWidth, 320);
                        UpdateProperty(propHeight, 50);
                        UpdateProperty(propOrientation, (int)Orientation.Current);
                        break;

                    case MEDIUM_RECTANGLE:
                        UpdateProperty(propType, AdSize.Type.Standard);
                        UpdateProperty(propWidth, 300);
                        UpdateProperty(propHeight, 250);
                        UpdateProperty(propOrientation, (int)Orientation.Current);
                        break;

                    case IAB_BANNER:
                        UpdateProperty(propType, AdSize.Type.Standard);
                        UpdateProperty(propWidth, 480);
                        UpdateProperty(propHeight, 60);
                        UpdateProperty(propOrientation, (int)Orientation.Current);
                        break;

                    case LEADERBOARD:
                        UpdateProperty(propType, AdSize.Type.Standard);
                        UpdateProperty(propWidth, 728);
                        UpdateProperty(propHeight, 90);
                        UpdateProperty(propOrientation, (int)Orientation.Current);
                        break;

                    case SMART_BANNER:
                        UpdateProperty(propType, AdSize.Type.SmartBanner);
                        UpdateProperty(propWidth, 0);
                        UpdateProperty(propHeight, 0);
                        UpdateProperty(propOrientation, (int)Orientation.Current);
                        break;

                    default:
                        throw new ArgumentException("Unexpected size: " + newAdSize);
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return propertyHeight == 0 ? base.GetPropertyHeight(property, label) : propertyHeight;
        }

        private string ResolveAdSize(int width, int height, AdSize.Type type)
        {
            switch (type)
            {
                case AdSize.Type.Standard:
                    AdSize size = new AdSize(width, height);

                    if (AdSize.Banner.Equals(size))
                    {
                        return BANNER;
                    }
                    else if (AdSize.MediumRectangle.Equals(size))
                    {
                        return MEDIUM_RECTANGLE;
                    }
                    else if (AdSize.IABBanner.Equals(size))
                    {
                        return IAB_BANNER;
                    }
                    else if (AdSize.Leaderboard.Equals(size))
                    {
                        return LEADERBOARD;
                    }
                    break;

                case AdSize.Type.SmartBanner:
                    return SMART_BANNER;

                case AdSize.Type.AnchoredAdaptive:
                    return ANCHORED_ADAPTIVE_BANNER;
            }

            return CUSTOM;
        }

        private int ArrayIndexOf<T>(T[] array, T value, int defaultIndexIfNotFound = 0)
        {
            int index = Array.IndexOf(array, value);
            return index != -1 ? index : defaultIndexIfNotFound;
        }

        private Rect GetNextFieldPositionRect(Rect baseRect)
        {
            Rect rect = new Rect(
                    baseRect.x, baseRect.y + propertyHeight,
                    baseRect.width, EditorGUIUtility.singleLineHeight);
            propertyHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            return rect;
        }

        private void UpdateProperty(SerializedProperty property, int newValue)
        {
            if (property.intValue != newValue)
            {
                property.intValue = newValue;
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        private void UpdateProperty(SerializedProperty property, string newValue)
        {
            if (!property.stringValue.Equals(newValue))
            {
                property.stringValue = newValue;
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        private void UpdateProperty(SerializedProperty property, AdSize.Type newValue)
        {
            UpdateProperty(property, (int)newValue);
        }
    }
}
