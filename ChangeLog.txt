Google Mobile Ads Unity Plugin Change Log

**************
Next version
**************

Plugin:
- Fixed https://github.com/googleads/googleads-mobile-unity/issues/1613 where build error occurs on Unity 2021.
- Fixed https://github.com/googleads/googleads-mobile-unity/issues/1616 where iOS build contains undefined symbol.
- Automatically added SKAdNetworkIdentifiers recommended by https://developers.google.com/admob/ios/ios14#skadnetwork into generated iOS builds. You can manage the list of SKAdNetworkIdentifier values by editing `Assets/GoogleMobileAds/Editor/GoogleMobileAdsSKAdNetworkItems.xml`.

Built and tested with:
- Google Play services 20.0.0
- Google Mobile Ads iOS SDK 8.2.0
- External Dependency Manager for Unity 1.2.165.

**************
Version 6.0.0
**************

Plugin:
- Added support for GMA iOS SDK v8 and GMA Android SDK v20. Requires using GMA iOS SDK v8.0.0 or higher, and GMA Android SDK 20.0.0 or higher.
- Removed MobileAds.Initialize(string appId).
- Removed Birthday, Gender, TestDevices, TagForChildDirectedTreatment properties on AdRequest. TagForChildDirectedTreatment and TestDeviceIds properties are available under RequestConfiguration..
- Removed OnAdLeavingApplication event for all formats.
- Removed MediationAdapterClassName from all formats in favor of ResponseInfo.
- Removed Message from AdErrorEventArgs class in favor of AdError.
- Removed RewardBasedVideoAd in favor of RewardedAd.
- Added support for ad load errors, please see https://developers.google.com/admob/unity/ad-load-errors for details.
- Ad Manager integration now requires providing the app ID in the Unity Editor.
- Changed package format to contain compiled assemblies in DLL format in place of the uncompiled code.
- Prior to GMA iOS SDK v8.7.0 release, you need to disable "Add use_frameworks!" and "Always add the main target to Podfile" in Unity Editor -> Assets -> External Dependency Manager -> iOS Resolver -> Settings, or else the GMA plugin does not work.

Built and tested with:
- Google Play services 20.0.0
- Google Mobile Ads iOS SDK 8.2.0
- External Dependency Manager for Unity 1.2.165.

**************
Version 5.4.0
**************

Plugin:
- Add support for iOS14 with Googles `SKAdNetwork` identifiers automatically included in
  `Info.plist`.
- Added the RewardedInterstitialAd format. This feature is currently in private beta. Reach out to your account manager to request access.
- Added mock ad views to enable developers to test ad placement and callback logic within the Unity editor.
- Added fix for crash that occurs when attempting to show interstitial when app is closing.
- Added fix for crash that occurs when calling `GetResponseInfo()` on iOS before ad is loaded.

Built and tested with:
- Google Play services 19.5.0
- Google Mobile Ads iOS SDK 7.68.0
- External Dependency Manager for Unity 1.2.161.

**************
Version 5.3.0
**************

Plugin:
- Add InitializationStatusClient for Init callback in Unity Editor. Fixes #1394.
- Update to Android SDK version 19.3.0

Built and tested with:
- Google Play services 19.3.0
- Google Mobile Ads iOS SDK 7.63.0
- External Dependency Manager for Unity 1.2.156.

**************
Version 5.2.0
**************

Plugin:
  - Added ResponseInfo class. See
  https://developers.google.com/admob/unity/response-info for usage details.
  - Fixes #1307 - issue with running in Unity Editor when targeting iOS platform.
  - Fixes #1287 - issue where a crash is caused in equality check when AdSize is
  null.
  - Moved GoogleMobileAdsPlugin to GoogleMobileAdsPlugin.androidlib to ensure manifest
  is picked up when building android app in Unity 2020. Fixes issue #1310. Thanks @pipe-alt!
  - Fix error messages for iOS plugin.
  - Added the DisableMediationInitialization() method to MobileAds.
   Warning: Calling this method may negatively impact your Google mediation performance.
   This method should only be called if you include Google mediation adapters in your app, but you
    won't use mediate through Google during a particular app session (for example, you are running
     an A/B mediation test).

Built and tested with:
- Google Play services 19.2.0
- Google Mobile Ads iOS SDK 7.60.0
- External Dependency Manager for Unity 1.2.156.

**************
Version 5.1.0
**************

Plugin:
  - Added RequestConfiguration class. See
  https://developers.google.com/admob/unity/targeting for usage details.
  - Fixed issue with building for IL2CPP in versions of Unity 2017 and earlier.
  - Adding missing imports for Unity 5.6 build (Thanks @EldersJavas !).
  - Added GoogleMobileAds assembly definition.
  - Added thread safety to GADUObjectCache in iOS plugin.
  - Revised project structure. If upgrading from a previous version, delete
  your GoogleMobileAds/ folder before importing this plugin.

Built and tested with:
- Google Play services 19.1.0
- Google Mobile Ads iOS SDK 7.58.0
- Unity Jar Resolver 1.2.152


**************
Version 5.0.1
**************

Plugin:
  - Fixed issue with externs.cpp in pre-2019 versions of Unity

Built and tested with:
- Google Play services 19.0.0
- Google Mobile Ads iOS SDK 7.56.0
- Unity Jar Resolver 1.2.136

**************
Version 5.0.0
**************

Plugin:
  - Removed preprocessor directives for custom assembly support.
  - Fixed IL2CPP build support on Android.
  - Updated to Google Play services 19.0.0.
  - Updated minimum Android API level to 16.

Built and tested with:
- Google Play services 19.0.0
- Google Mobile Ads iOS SDK 7.56.0
- Unity Jar Resolver 1.2.136

**************
Version 4.2.1
**************

Plugin:
  - Fixed issue with using `AdSize.FullWidth` API for apps that only support landscape.

Built and tested with:
- Google Play services 18.3.0
- Google Mobile Ads iOS SDK 7.53.1
- Unity Jar Resolver 1.2.135

**************
Version 4.2.0
**************

Plugin:
  - Added support for using AdSize.FullWidth with Adaptive banner APIs.
  - Added `GetRewardItem()` API for `RewardedAd`.
  - Fixed issue with Android implementation of `GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth`.

Built and tested with:
- Google Play services 18.3.0
- Google Mobile Ads iOS SDK 7.53.1
- Unity Jar Resolver 1.2.135

**************
Version 4.1.0
**************

Plugin:
  - Released Anchored Adaptive Banner APIs.

Built and tested with:
- Google Play services 18.2.0
- Google Mobile Ads iOS SDK 7.51.0
- Unity Jar Resolver 1.2.130

**************
Version 4.0.0
**************

Plugin:
- Breaking change: The Android library included in this plugin is now distributed as an aar, and
  lives at `Assets/Plugins/Android/googlemobileads-unity.aar'. If you are upgrading from a previous
  version, remove the `Assets/Plugins/Android/GoogleMobileAdsPlugin' folder prior to importing this
  latest version of the plugin.
- Added proguard support on Android.
- Update Android Google Mobile Ads SDK version to 18.2.0.
- Fixed a bug where the AdSize.SMART_BANNER banner size did not work on Unity 2019.2+.
- Added assertion to stop Android builds when Google Mobile Ads settings are invalid.

Built and tested with:
- Google Play services 18.2.0
- Google Mobile Ads iOS SDK 7.50.0
- Unity Jar Resolver 1.2.125

**************
Version 3.18.3
**************

Plugin:
- Update JAR resolver.

Built and tested with:
- Google Play services 18.1.1
- Google Mobile Ads iOS SDK 7.48.0
- Unity Jar Resolver 1.2.124

**************
Version 3.18.2
**************

Plugin:
- Update to Android release 18.1.1.

Built and tested with:
- Google Play services 18.1.1
- Google Mobile Ads iOS SDK 7.47.0
- Unity Jar Resolver 1.2.123

**************
Version 3.18.1
**************

Plugin:
- Add new Initialization API.
- Fixed Android compile error with PListProcessor.
- Removed reflection for improved IL2CPP support.
- Fixed iOS dependency to not use patch version.

Built and tested with:
- Google Play services 18.1.0
- Google Mobile Ads iOS SDK 7.46.0
- Unity Jar Resolver 1.2.122

**************
Version 3.18.0
**************

Plugin:
- Added GoogleMobileAdsSettings editor UI for making Plist / manifest changes.
- Fix OnRewardedAdFailedToShow callbacks.
- Migrated android support library to androidx (JetPack) with Google Mobile Ads
  SDK version 18.0.0.

Built and tested with:
- Google Play services 18.0.0
- Google Mobile Ads iOS SDK 7.45.0
- Unity Jar Resolver 1.2.119

**************
Version 3.17.0
**************

Plugin:
- Revised Banner positioning code to use gravity instead of popup window.
- Tested Banner positioning with notched devices supporting Google P APIs.
- Added Rewarded Ads ServerSideVerificationOptions (thanks @halfdevil !)
- Fixed issue with PListProcessor macro.
- Added whitelist for apache http library (thanks @RolandSzep !)
- Specified package for gender enum (thanks @armnotstrong !)
- Added mediation extras for custom events (thanks SeanPONeil !)

Built and tested with:
- Google Play services 17.2.0
- Google Mobile Ads iOS SDK 7.44.0
- Unity Jar Resolver 1.2.111

**************
Version 3.16.0
**************

Plugin:
- Added new RewardedAd APIs support.
- Added PListProcessor to assist in adding the GADApplicationIdentifier
to iOS build Info.plist.

Built and tested with:
- Google Play services 17.2.0
- Google Mobile Ads iOS SDK 7.42.0
- Unity Jar Resolver 1.2.102.0

**************
Version 3.15.1
**************

Plugin:
- Fixed crash when adding mediation extras to ad request.

Built and tested with:
- Google Play services 15.0.1
- Google Mobile Ads iOS SDK 7.32.0
- Unity Jar Resolver 1.2.88.0

**************
Version 3.15.0
**************

Plugin:
- Forward Android ad events on background thread through JNI interface
to mitigate ANRs.

Mediation packages:
- Updated AppLovin Unity package to v3.0.3.
- Updated Chartboost Unity package to v1.1.1.
- Updated Facebook Unity package to v1.1.3.
- Updated IronSource Unity package to v1.0.2.
- Updated Nend Unity package to v2.0.0.
- Updated Tapjoy Unity package to v2.0.0.

Built and tested with:
- Google Play services 15.0.1
- Google Mobile Ads iOS SDK 7.31.0
- Unity Jar Resolver 1.2.79.0

**************
Version 3.14.0
**************

Plugin:
- Fixed Google Play dependencies version conflict with Firebase plugins.

Mediation packages:
- Updated AdColony Unity package to v1.2.1.
- Updated AppLovin Unity package to v3.0.2.
- Updated Chartboost Unity package to v1.1.0.
- Updated Facebook Unity package to v1.1.2.
- Updated InMobi Unity package to v2.1.0.
- Updated IronSource Unity package to v1.0.1.
- Updated Maio Unity package to v1.1.0.
- Updated MoPub Unity package to v2.1.0.
- Updated MyTarget Unity package to v2.1.0.
- Updated Nend Unity package to v1.0.2.
- Updated Tapjoy Unity package to v1.1.1.
- Updated UnityAds Unity package to v1.1.3.

Built and tested with:
- Google Play services 15.0.1
- Google Mobile Ads iOS SDK 7.31.0
- Unity Jar Resolver 1.2.75.0

**************
Version 3.13.1
**************

Plugin:
- Fixed issue where banner ads reposition to top of screen after a full
screen ad is displayed.

Built and tested with:
- Google Play services 12.0.1
- Google Mobile Ads iOS SDK 7.30.0
- Unity Jar Resolver 1.2.64.0

**************
Version 3.13.0
**************

Plugin:
- Added `OnAdCompleted` ad event to rewarded video ads.
- Removed support for Native Ads Express.

Mediation packages:
- Added Chartboost mediation support package.
- Added MoPub mediation support package.
- Updated AppLovin Unity package to v1.2.1.
- Updated AdColony Unity package to v1.0.1.
- Updated myTarget Unity package to v2.0.0.

Built and tested with:
- Google Play services 12.0.1
- Google Mobile Ads iOS SDK 7.30.0
- Unity Jar Resolver 1.2.64.0

**************
Version 3.12.0
**************

Plugin:
- Added `setUserId` API to rewarded video ads to identify users in
server-to-server reward callbacks.
- Removed functionality that forced ad events to be invoked on the
main thread.

Mediation packages:
- Updated maio Unity package to v1.0.1.

Built and tested with:
- Google Play services 11.8.0
- Google Mobile Ads iOS SDK 7.29.0
- Unity Jar Resolver 1.2.61.0

**************
Version 3.11.1
**************

Plugin:
- Fixed issue where calling GetWidthInPixels() or GetHeightInPixels() resulted
in a null pointer exception.

Mediation packages:
- Added Facebook mediation support package.

Built and tested with:
- Google Play services 11.8.0
- Google Mobile Ads iOS SDK 7.28.0
- Unity Jar Resolver 1.2.61.0

**************
Version 3.11.0
**************

Plugin:
- Updated Android ad events to be invoked on the main thread.
- Added `MobileAds.SetiOSAppPauseOnBackground()` method to pause iOS apps when
displaying full screen ads.
- Fixed issue were banners repositioned incorrectly following an orienation
change.

Mediation packages:
- Added maio mediation support package.
- Added nend mediation support package.

Built and tested with:
- Google Play services 11.8.0
- Google Mobile Ads iOS SDK 7.27.0
- Unity Jar Resolver 1.2.61.0

**************
Version 3.10.0
**************

Plugin:
- Updated Smart Banner positioning to render within safe area on iOS 11.
- Added API to return height and width of BannerView in pixels.
- Added SetPosition method to reposition banner ads.
- Updated AppLovin Unity mediation package to support AppLovin initialization
integration.

Mediation packages:
- Added InMobi mediation support package.
- Added Tapjoy mediation support package.
- Added Unity Ads mediation support package.
- Added myTarget mediation support package.

Built and tested with:
- Google Play services 11.6.2
- Google Mobile Ads iOS SDK 7.27.0
- Unity Jar Resolver 1.2.59.0

*************
Version 3.9.0
*************

Plugin:
- Implemented workaround for issue where ad views are rendered in incorrect
position.
- Resolved compatibility issues with Gradle 4.
- Resovled comnpatilbity issues with older versions of Xcode.

Mediation packages:
- Added API for video ad volume control.
- Added AdColony mediation support package.
- Added AppLovin mediation support package.

Built and tested with:
- Google Play services 11.6.0
- Google Mobile Ads iOS SDK 7.25.0
- Unity Jar Resolver 1.2.59.0

*************
Version 3.8.0
*************

- Added support for Vungle mediation extras.
- Updated ad views to render within safe area on iOS 11 when using predefined
AdPosition constants.
- Added MediationAdapterClassName() method to all ad formats.
- Fixed issue where ad views are always rendered on the top of the screen for
certain devices.

Built and tested with:
- Google Play services 11.4.0
- Google Mobile Ads iOS SDK 7.24.1
- Unity Jar Resolver 1.2.59.0

*************
Version 3.7.1
*************

- Fix issue where banner and Native Express ads fail to show after being hidden.

Built and tested with:
- Google Play services 11.4.0
- Google Mobile Ads iOS SDK 7.24.0
- Unity Jar Resolver 1.2.52.0

*************
Version 3.7.0
*************

- Updated dependency specification for JarResolver to use new XML format.
- Resolved JarResolver incompatibility issues when using Firebase Unity plugins.

Built and tested with:
- Google Play services 11.2.0
- Google Mobile Ads iOS SDK 7.23.0
- Unity Jar Resolver 1.2.48.0

*************
Version 3.6.3
*************

- Fixed serving of live ads to iOS simulator when simulator set as test
device.
- Reverted addition of mediation sub-directories to Plugin folder.

Built and tested with:
- Google Play services 11.0.4
- Google Mobile Ads iOS SDK 7.21.0
- Unity Jar Resolver 1.2.35.0

*************
Version 3.6.2
*************

- Add mediation sub-directories to Plugin folder.

Built and tested with:
- Google Play services 11.0.4
- Google Mobile Ads iOS SDK 7.21.0
- Unity Jar Resolver 1.2.35.0

*************
Version 3.6.1
*************

- Updated Unity Jar Resolver.

Built and tested with:
- Google Play services 11.0.0
- Google Mobile Ads iOS SDK 7.21.0
- Unity Jar Resolver 1.2.32.0

*************
Version 3.6.0
*************

- Added method to initialize the GMA SDK.
- Added FullWidth AdSize constant.
- Fixed incompatibility with Gradle build system.
- Updated iOS code to remove modular imports.

Built and tested with:
- Google Play services 11.0.0
- Google Mobile Ads iOS SDK 7.21.0
- Unity Jar Resolver 1.2.31.0

*************
Version 3.5.0
*************
- Fix ad views losing visibility after an activity change for certain devices
(eg. Huaweai devices).

Built and tested with:
- Google Play services 10.2.4
- Google Mobile Ads iOS SDK 7.20.0
- Unity Jar Resolver 1.2.20.0

*************
Version 3.4.0
*************
- Fix native express and banner ad behavior where initializing and
hidden ads create unclickable region.

Built and tested with:
- Google Play services 10.2.1
- Google Mobile Ads iOS SDK 7.19.0
- Unity Jar Resolver 1.2.14.0

*************
Version 3.3.0
*************
- Removed support for in-app purchases.
- Fix positioning of ads in sticky-immersive mode.
- Fix issue were ads larger than 320dp could not be rendered.
- Fix incorrect positioning of ads in iOS for ad position BOTTOM.
- Add rewarded video test ad units to HelloWorld sample app.
- Suppress warnings for unsed dummy ad events.

Built and tested with:
- Google Play services 10.2.0
- Google Mobile Ads iOS SDK 7.18.0
- Unity Jar Resolver 1.2.12.0

*************
Version 3.2.0
*************
- Banner ads and native express ads display correctly on Unity 5.6.
- Add ability to specify x, y location of ad views.

Built and tested with:
- Google Play services 10.0.1
- Google Mobile Ads iOS SDK 7.16.0
- Unity Jar Resolver 1.2.9.0

*************
Version 3.1.3
*************
- Fix incorrect invocation of events on ads failing to load.

Built and tested with:
- Google Play services 10.0.0
- Google Mobile Ads iOS SDK 7.15.0
- Unity Jar Resolver 1.2.6.0

*************
Version 3.1.2
*************
- Fix NPE when ad events are not hooked up.

Built and tested with:
- Google Play services 9.8.0
- Google Mobile Ads iOS SDK 7.13.0
- Unity Jar Resolver 1.2.2.0

*************
Version 3.1.1
*************
- Remove dependency on Android Support Library and update GMA iOS SDK
version in `AdMobDependencies.cs`.

Built and tested with:
- Google Play services 9.6.1
- Google Mobile Ads iOS SDK 7.13.0
- Unity Jar Resolver 1.2.2.0

*************
Version 3.1.0
*************
- Integrate plugin with play-services-resolver-1.2.1.0.
- Removal of CocoaPods integration.

Built and tested with:
- Google Play services 9.6.0
- Google Mobile Ads iOS SDK 7.12.0
- Unity Jar Resolver 1.2.1.0

*************
Version 3.0.7
*************
- Fix crash within OnAdLoaded ad event for rewarded video ads on iOS.

Built and tested with:
- Google Play services 9.4.0
- Google Mobile Ads iOS SDK 7.11.0
- Unity Jar Resolver 1.2

*************
Version 3.0.6
*************
- Add support for Native Ads express.
- Fix compatibility issues with Android IL2CPP compilation.
- Fix memory leak of C# client objects

Built and tested with:
- Google Play services 9.4.0
- Google Mobile Ads iOS SDK 7.10.1
- Unity Jar Resolver 1.2

*************
Version 3.0.5
*************
- Remove use of JSONUtility.

Built and tested with:
- Google Play services 9.2.0
- Google Mobile Ads iOS SDK 7.8.1
- Unity Jar Resolver 1.2

*************
Version 3.0.4
*************
- Fix Podfile compatibility with CocoaPods 1.0.0.
- Add support for DFP custom native ad formats.

Built and tested with:
- Google Play services 9.0.0
- Google Mobile Ads iOS SDK 7.8.1
- Unity Jar Resolver 1.2

*************
Version 3.0.3
*************
- Restrict simultaneous rewarded video requests on Android.

Built and tested with:
- Google Play services 8.4.0
- Google Mobile Ads iOS SDK 7.7.0

*************
Version 3.0.2
*************
- Fix compatibility issues with Google Mobile Ads iOS SDK 7.7.0

Built and tested with:
- Google Play services 8.4.0
- Google Mobile Ads iOS SDK 7.7.0

*************
Version 3.0.1
*************
- Update preprocessor directives for iOS post build setup
- Add request agent to all ad requests from plugin

Built and tested with:
- Google Play services 8.4.0
- Google Mobile Ads iOS SDK 7.6.0

*************
Version 3.0.0
*************
- Add support for Custom In-App purchase flow on Android
- Add CocoaPods integration and automated build settings for iOS projects
- Use JarResolver plugin to resolve Google Play services client dependencies
- Ad events for banners and interstitials refactored with new names

Built and tested with:
- Google Play services 8.4.0
- Google Mobile Ads iOS SDK 7.6.0

*************
Version 2.3.1
*************
- Move IInAppBillingService into its own JAR

*************
Version 2.3.0
*************
- Add support for In-App Purchase house ads on Android

*************
Version 2.2.1
*************
- Fix for Android manifest merge issues on Unity 4.x
- Fix for TouchCount issue on Unity 5.0

***********
Version 2.2
***********
- Support for Unity 5.0 & ARC
- Additional Banner positions
- iOS Ads SDK 7.0.0 compatibility

***********
Version 2.1
***********
- Support for Interstitial Ads
- Ad events use EventHandlers

***********
Version 2.0
***********
- A single package with cross platform (Android/iOS) support
- Mock ad calls when running inside Unity editor
- Support for Banner Ads
- Custom banner sizes
- Banner ad events listeners
- AdRequest targeting methods
- A sample project to demonstrate plugin integration

***********
Version 1.2
***********
- Initial Android version with Google Play services support
- Support for Banner Ads only

***********
Version 1.1
***********
- Initial iOS only version
- Support for Banner Ads only

***********
Version 1.0
***********
- Initial version for Android (using now deprecated legacy Android SDK)
