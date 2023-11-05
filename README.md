# AppHarbr Wrapper Google Mobile Ads Unity Plugin

AppHarbr is Ad Quality realtime service.  
Our mission is to ensure high quality in-app ads and secure the mobile user experience.  
AppHarbr ensures that disruptive and offensive ads don't reach app users with proactive ad quality controls.  
Real-time ad quality protection equips app publishers with control against disruptive, unwanted, malicious, and offensive ads.

Google Mobile Ads SDK is the latest generation in Google mobile advertising
featuring refined ad formats and streamlined APIs for access to mobile ad
networks and advertising solutions. The SDK enables mobile app developers to
maximize their monetization in native mobile apps.

This repository is a fork from the [Google Mobile Ads Unity Plugin](//github.com/googleads/googleads-mobile-unity) repository.

In this source code AppHarbr SDK is integrated into the Google Mobile Ads Unity Plugin as Publishers
are integrating AppHarbr SDK on Native Applications on Android/iOS.

AppHarbr's modifications are specific for Ad Quality and obviously transparent to the publisher.
The following classes were modified:
- `com.google.unity.ads.admanager.UnityAdManagerBannerView`
- `com.google.unity.ads.admanager.UnityAdManagerInterstitialAd`
- `com.google.unity.ads.Banner`
- `com.google.unity.ads.Interstitial`
- `com.google.unity.ads.UnityRewardedAd`
- `com.google.unity.ads.RewardedInterstitialAd`

Every block code that AppHarbr added or modified will be separated with
`//************************************************************************//`

AppHarbr Ad Quality enter into action only when:
- AppHarbr SDK initialized through the C# Script.
- Ad Unit Id required by the publisher to be watched by AppHarbr SDK through the C# Script.

Otherwise, The Google Mobile Ads SDK will functional regularly.

This repository contains the source code for the Google Mobile Ads Unity
plugin and AppHarbr Integration.
This plugin enables Unity developers to easily serve Google Mobile Ads
on Android and iOS apps with AppHarbr Ad Quality service without having to write Java or Objective-C code.
The plugin provides a C# interface for requesting ads that is used by C#
scripts in your Unity project.

## Release Process

On each new release of Google Mobile Ads Unity Plugin version, AppHarbr creating new branch on this repository.
Then AppHarbr is integrating the Ad quality, then having internal testing, committing the changes and releasing a newer version with the same version number of Google Mobile Ads Unity Plugin.

## Usage

The Publisher need to download this AppHarbr Wrapper Google Mobile Ads Unity Plugin
and import it into his project instead of using the official Google Mobile Ads Unity Plugin.
In addition the publisher need to download the AppHarbr SDK Unity Plugin and import it into his project.

- Download from the releases the AppHarbr Wrapper Google Mobile Ads Unity Plugin unitypackage.
- Go to Unity IDE -> Assets -> Import Package -> Custom Package...
- Choose the downloaded AppHarbr Wrapper Google Mobile Ads Unity Plugin unitypackage and import.
- Download from Publihser UI the AppHarbr SDK Unity Plugin.
- Go to Unity IDE -> Assets -> Import Package -> Custom Package...
- Choose the downloaded AppHarbr SDK Unity Plugin unitypackage and import.


## Download

Please check out our [releases](//github.com/appharbr/appharbr-wrapper-googleads-mobile-unity/releases)


## License

[Apache 2.0 License](http://www.apache.org/licenses/LICENSE-2.0.html)

