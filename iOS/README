========================================================================
Copyright 2013 Google Inc. All Rights Reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
========================================================================


This is the AdMob Unity Plugin for iOS.  It provides a way to request
AdMob ads from a Unity project.  This plugin was written and tested with
the Google AdMob SDK version 6.4.1 for iOS, and Unity 4.1.2.

The AdMob Unity plugin contains a .unitypackage file for those that want to
easily import the plugin, as well as the source code.

Requirements:

- Unity Pro 4.1.2
- Google AdMob Ads SDK for iOS
- iOS version 4.3 or later as well as XCode 4.5 or later
- AdMob publisher ID

How to import AdMobUnityPlugin.unitypackage:

1. Open your project in the Unity editor.
2. On the top toolbar, select "Assets" -> "Import Package" -> "Custom Package".
3. Select the AdMobUnityPlugin.unitypackage file.
4. Import all of the files for the plugins by selecting "Import". Make sure
   to check for any conflicts with files.
5. Drag the AdMobPlugin prefab from the Plugins/AdMobPlugin/ folder into
   your Unity scene.

The plugin also comes with an example script. Simply attach
AdMobPluginDemoScript (found in Plugins/AdMobPlugin/) to your MainCamera as a
Component and build your project to see AdMob working.

iOS - Building project:

1. Open up AdMobMobilePlugin.cs and make sure that the AdMobMobilePlugin
   is set to AdMobPluginiOS.
2. On the top toolbar, select "File" -> "Build Settings".
3. Check off the scenes you wan to build in the "Scenes In Build" section.
4. Switch the platform to iOS, then select "Player Settings".
5. Set "Target iOS Version" to at least iOS 4.3 or above.
6. Select "Build" on the "Build Settings" menu to build a XCode project.

iOS - Running project:

Before running your compiled XCode project, you will need to add some
framework references as well as modify linker flags. You can find a detailed
set of instructions on how to do this at:

https://developers.google.com/mobile-ads-sdk/docs/

Implementation:

The plugin provides the following AdMob methods:

1.  CreateBannerView

    Takes in a publisherId string as well as a constant for the ad size. The
    last boolean parameter denotes whether the ad should be shown at the top
    or bottom of the screen.

    An example call placing the ad at the top of the screen is provided below:

    AdMobPlugin.CreateBannerView ("INSERT_PUBLISHER_ID_HERE",
                                  AdMobPlugin.ADSIZE_SMART_BANNER,
                                  true);
2.  RequestBannerAd

    Takes in a testing flag as well as a string representing a list of extras.
    Make sure to use correctly formed JSON for the extras. Pass in an empty
    string ("") if you have no extras.

    An example call requesting a test ad with some extras is shown below:

    AdMobPlugin.RequestBannerAd (true,
                                 "{\"color_bg\":\"AAAAFF\"}");

3. HideBannerView

   Called after a BannerView has been created. This method can hide the ad from
   showing on screen. An example call of this is shown below:

   AdMobPlugin.HideBannerView();

4. ShowBannerView

   Called after a BannerView has been created, this method can show any ad that
   has been hidden. An example call of this is shown below:

   AdMobPlugin.ShowBannerView();

This plugin also allows you the option to listen for ad events. The following
events are supported:

    public static event Action ReceivedAd;
    public static event Action<string> FailedToReceiveAd;
    public static event Action ShowingOverlay;
    public static event Action DismissingOverlay;
    public static event Action DismissedOverlay;
    public static event Action LeavingApplication;

Registering for an event can be done using the += operater as is shown below:

    // Assume HandleReceivedAd is your function.
    AdMobPlugin.ReceivedAd += HandleDidReceiveAd;

Remember to un-register for events when you're cleaning up your GameObjects.
You can unregister using the -= operator as is shown below:

    // Assume HandleReceivedAd is your function.
    AdMobPlugin.ReceivedAd -= HandleDidReceiveAd;

Additional Resources:
https://developers.google.com/mobile-ads-sdk/docs
https://groups.google.com/group/google-admob-ads-sdk
https://plus.google.com/+GoogleAdsDevelopers
