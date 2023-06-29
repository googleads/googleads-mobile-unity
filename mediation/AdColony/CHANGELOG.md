## AdColony Unity Mediation Plugin Changelog

#### Next Version
- Fixed a duplicate definition warning for `MediationExtras`.

#### [Version 2.6.2](https://dl.google.com/googleadmobadssdk/mediation/unity/adcolony/AdColonyUnityAdapter-2.6.2.zip)
- Moved adapter content to `GoogleMobileAds/Mediation/AdColony/`.
- Refactored adapter namespace to use `GoogleMobileAds.Mediation.AdColony`.
- Supports [AdColony Android adapter version 4.8.0.2](https://github.com/googleads/googleads-mobile-android-mediation/blob/main/ThirdPartyAdapters/adcolony/CHANGELOG.md#version-4802).
- Supports [AdColony iOS adapter version 4.9.0.2](https://github.com/googleads/googleads-mobile-ios-mediation/blob/main/adapters/AdColony/CHANGELOG.md#version-4902).
- Built and tested with the Google Mobile Ads Unity Plugin version 8.1.0.

#### [Version 2.6.1](https://dl.google.com/googleadmobadssdk/mediation/unity/adcolony/AdColonyUnityAdapter-2.6.1.zip)
- Updated the privacy APIs from the `AdColonyAppOptions` class to the following methods to support GDPR and CCPA:
  * `SetGDPRRequired(bool)` -> `SetPrivacyFrameworkRequired(AdColonyPrivacyFramework, bool)`
  * `IsGDPRRequired()` -> `GetPrivacyFrameworkRequired(AdColonyPrivacyFramework)`
  * `SetGDPRConsentString(string)`-> `SetPrivacyConsentString(AdColonyPrivacyFramework, string)`
  * `GetGDPRConsentString()` -> `GetPrivacyConsentString(AdColonyPrivacyFramework)`
- Supports [AdColony Android adapter version 4.8.0.1](https://github.com/googleads/googleads-mobile-android-mediation/blob/main/ThirdPartyAdapters/adcolony/CHANGELOG.md#version-4801).
- Supports [AdColony iOS adapter version 4.9.0.2](https://github.com/googleads/googleads-mobile-ios-mediation/blob/main/adapters/AdColony/CHANGELOG.md#version-4902).
- Built and tested with the Google Mobile Ads Unity Plugin version 7.4.1.

#### [Version 2.6.0](https://dl.google.com/googleadmobadssdk/mediation/unity/adcolony/AdColonyUnityAdapter-2.6.0.zip)
- Supports [AdColony Android adapter version 4.8.0.0](https://github.com/googleads/googleads-mobile-android-mediation/blob/main/ThirdPartyAdapters/adcolony/CHANGELOG.md#version-4800).
- Supports [AdColony iOS adapter version 4.9.0.0](https://github.com/googleads/googleads-mobile-ios-mediation/blob/main/adapters/AdColony/CHANGELOG.md#version-4900).
- Built and tested with the Google Mobile Ads Unity Plugin version 7.1.0.

#### [Version 2.5.0](https://dl.google.com/googleadmobadssdk/mediation/unity/adcolony/AdColonyUnityAdapter-2.5.0.zip)
- Supports [AdColony Android adapter version 4.7.0.0](https://github.com/googleads/googleads-mobile-android-mediation/blob/main/ThirdPartyAdapters/adcolony/CHANGELOG.md#version-4700).
- Supports [AdColony iOS adapter version 4.8.0.0](https://github.com/googleads/googleads-mobile-ios-mediation/blob/main/adapters/AdColony/CHANGELOG.md#version-4800).
- Built and tested with the Google Mobile Ads Unity Plugin version 7.0.0.

#### [Version 2.4.1](https://dl.google.com/googleadmobadssdk/mediation/unity/adcolony/AdColonyUnityAdapter-2.4.1.zip)
- Supports [AdColony Android adapter version 4.6.5.0](https://github.com/googleads/googleads-mobile-android-mediation/blob/main/ThirdPartyAdapters/adcolony/CHANGELOG.md#version-4650).
- Supports [AdColony iOS adapter version 4.7.2.2](https://github.com/googleads/googleads-mobile-ios-mediation/blob/main/adapters/AdColony/CHANGELOG.md#version-4722).
- Built and tested with the Google Mobile Ads Unity Plugin version 7.0.0.

#### [Version 2.4.0](https://dl.google.com/googleadmobadssdk/mediation/unity/adcolony/AdColonyUnityAdapter-2.4.0.zip)
- Supports [AdColony Android adapter version 4.6.5.0](https://github.com/googleads/googleads-mobile-android-mediation/blob/main/ThirdPartyAdapters/adcolony/CHANGELOG.md#version-4650).
- Supports [AdColony iOS adapter version 4.7.2.0](https://github.com/googleads/googleads-mobile-ios-mediation/blob/main/adapters/AdColony/CHANGELOG.md#version-4720).

#### [Version 2.3.0](https://dl.google.com/googleadmobadssdk/mediation/unity/adcolony/AdColonyUnityAdapter-2.3.0.zip)
- Supports [AdColony Android adapter version 4.5.0.0](https://github.com/googleads/googleads-mobile-android-mediation/blob/main/ThirdPartyAdapters/adcolony/CHANGELOG.md#version-4500).
- Supports [AdColony iOS adapter version 4.6.1.0](https://github.com/googleads/googleads-mobile-ios-mediation/blob/main/adapters/AdColony/CHANGELOG.md#version-4610).

#### [Version 2.2.0](https://dl.google.com/googleadmobadssdk/mediation/unity/adcolony/AdColonyUnityAdapter-2.2.0.zip)
- Supports [AdColony Android adapter version 4.2.4.0](https://github.com/googleads/googleads-mobile-android-mediation/blob/main/ThirdPartyAdapters/adcolony/CHANGELOG.md#version-4240).
- Supports [AdColony iOS adapter version 4.4.1.0](https://github.com/googleads/googleads-mobile-ios-mediation/blob/main/adapters/AdColony/CHANGELOG.md#version-4410).

#### [Version 2.1.0](https://dl.google.com/googleadmobadssdk/mediation/unity/adcolony/AdColonyUnityAdapter-2.1.0.zip)
- Supports [AdColony Android adapter version 4.2.0.0](https://github.com/googleads/googleads-mobile-android-mediation/blob/main/ThirdPartyAdapters/adcolony/CHANGELOG.md#version-4200).
- Supports [AdColony iOS adapter version 4.3.0.0](https://github.com/googleads/googleads-mobile-ios-mediation/blob/main/adapters/AdColony/CHANGELOG.md#version-4300).

#### [Version 2.0.3](https://dl.google.com/googleadmobadssdk/mediation/unity/adcolony/AdColonyUnityAdapter-2.0.3.zip)
- Repackaged `adcolony-extras-library.jar` to `adcolony-unity-android-library.aar`, using a `.aar` build to support Unity 2020.
  * When upgrading to this version, please remove `adcolony-extras-library.jar` from your project.
- Supports [AdColony Android adapter version 4.1.4.1](https://github.com/googleads/googleads-mobile-android-mediation/blob/main/ThirdPartyAdapters/adcolony/CHANGELOG.md#version-4141).
- Supports [AdColony iOS adapter version 4.1.5.0](https://github.com/googleads/googleads-mobile-ios-mediation/blob/main/adapters/AdColony/CHANGELOG.md#version-4150).

#### [Version 2.0.2](https://dl.google.com/googleadmobadssdk/mediation/unity/adcolony/AdColonyUnityAdapter-2.0.2.zip)
- Supports [AdColony Android adapter version 4.1.4.1](https://github.com/googleads/googleads-mobile-android-mediation/blob/main/ThirdPartyAdapters/adcolony/CHANGELOG.md#version-4141).
- Supports [AdColony iOS adapter version 4.1.4.1](https://github.com/googleads/googleads-mobile-ios-mediation/blob/main/adapters/AdColony/CHANGELOG.md#version-4141).

#### Version 2.0.1
- Supports [AdColony Android adapter version 4.1.4.0](https://github.com/googleads/googleads-mobile-android-mediation/blob/main/ThirdPartyAdapters/adcolony/CHANGELOG.md#version-4140).
- Supports [AdColony iOS adapter version 4.1.4.0](https://github.com/googleads/googleads-mobile-ios-mediation/blob/main/adapters/AdColony/CHANGELOG.md#version-4140).

#### Version 2.0.0
- Supports [AdColony Android adapter version 4.1.0.0](https://github.com/googleads/googleads-mobile-android-mediation/blob/main/ThirdPartyAdapters/adcolony/CHANGELOG.md#version-4100).
- Supports [AdColony iOS adapter version 4.1.2.0](https://github.com/googleads/googleads-mobile-ios-mediation/blob/main/adapters/AdColony/CHANGELOG.md#version-4120).

#### Version 1.0.6
- Supports [AdColony Android adapter version 3.3.11.0](https://github.com/googleads/googleads-mobile-android-mediation/blob/main/ThirdPartyAdapters/adcolony/CHANGELOG.md#version-33110).
- Supports [AdColony iOS adapter version 3.3.8.1.0](https://github.com/googleads/googleads-mobile-ios-mediation/blob/main/adapters/AdColony/CHANGELOG.md#version-33810).

#### Version 1.0.5
- Moved the following methods from the `AdColonyMediationExtras` Builder class to the `AdColonyAppOptions` class:
  * `SetGDPRRequired()`
  * `SetGDPRConsentString()`
  * `SetUserId()`
  * `SetTestMode()`
- Added the following methods to the `AdColonyAppOptions` class:
  * `IsGDPRRequired()`
  * `GetGDPRConsentString()`
  * `GetUserId()`
  * `IsTestMode()`
- Removed the `SetZoneId()` method from the `AdColonyMediationExtras` Builder class.
- Supports AdColony Android adapter version 3.3.10.1.
- Supports AdColony iOS adapter version 3.3.7.2.

#### Version 1.0.4
- Updated the plugin to support the new open-beta Rewarded API.
- Supports AdColony Android adapter version 3.3.8.1.
- Supports AdColony iOS adapter version 3.3.6.1.

#### Version 1.0.3
- Supports AdColony Android adapter version 3.3.5.1.
- Supports AdColony iOS adapter version 3.3.5.0.

#### Version 1.0.2
- Supports AdColony Android SDK version 3.3.4.
- Supports AdColony iOS SDK version 3.3.4.
- `SetTestMode()` from the `AdColonyMediationExtras` Builder class is now ignored for Android. Publishers can now request test ads from AdColony for Android by specifying a test device via `AddTestDevice()` from the `AdRequest` Builder class.
- Added the following methods to the `AdColonyMediationExtras` Builder class:
  * `SetGDPRRequired()`
  * `SetGDPRConsentString()`

#### Version 1.0.1
- Supports AdColony Android SDK version 3.3.0-unity.
- Supports AdColony iOS SDK version 3.3.0.

#### Version 1.0.0
- First release!
- Supports AdColony Android SDK version 3.3.0.
- Supports AdColony iOS SDK version 3.3.0.
