## AdColony Unity Mediation Plugin Changelog

#### Version 2.2.0
- Supports [AdColony Android adapter version 4.2.4.0](https://github.com/googleads/googleads-mobile-android-mediation/blob/master/ThirdPartyAdapters/adcolony/CHANGELOG.md#version-4240).
- Supports [AdColony iOS adapter version 4.4.1.0](https://github.com/googleads/googleads-mobile-ios-mediation/blob/master/adapters/AdColony/CHANGELOG.md#version-4410).

#### Version 2.1.0
- Supports [AdColony Android adapter version 4.2.0.0](https://github.com/googleads/googleads-mobile-android-mediation/blob/master/ThirdPartyAdapters/adcolony/CHANGELOG.md#version-4200).
- Supports [AdColony iOS adapter version 4.3.0.0](https://github.com/googleads/googleads-mobile-ios-mediation/blob/master/adapters/AdColony/CHANGELOG.md#version-4300).

#### Version 2.0.3
- Repackaged `adcolony-extras-library.jar` to `adcolony-unity-android-library.aar`, using a `.aar` build to support Unity 2020.
  * When upgrading to this version, please remove `adcolony-extras-library.jar` from your project.
- Supports [AdColony Android adapter version 4.1.4.1](https://github.com/googleads/googleads-mobile-android-mediation/blob/master/ThirdPartyAdapters/adcolony/CHANGELOG.md#version-4141).
- Supports [AdColony iOS adapter version 4.1.5.0](https://github.com/googleads/googleads-mobile-ios-mediation/blob/master/adapters/AdColony/CHANGELOG.md#version-4150).

#### Version 2.0.2
- Supports [AdColony Android adapter version 4.1.4.1](https://github.com/googleads/googleads-mobile-android-mediation/blob/master/ThirdPartyAdapters/adcolony/CHANGELOG.md#version-4141).
- Supports [AdColony iOS adapter version 4.1.4.1](https://github.com/googleads/googleads-mobile-ios-mediation/blob/master/adapters/AdColony/CHANGELOG.md#version-4141).

#### Version 2.0.1
- Supports [AdColony Android adapter version 4.1.4.0](https://github.com/googleads/googleads-mobile-android-mediation/blob/master/ThirdPartyAdapters/adcolony/CHANGELOG.md#version-4140).
- Supports [AdColony iOS adapter version 4.1.4.0](https://github.com/googleads/googleads-mobile-ios-mediation/blob/master/adapters/AdColony/CHANGELOG.md#version-4140).

#### Version 2.0.0
- Supports [AdColony Android adapter version 4.1.0.0](https://github.com/googleads/googleads-mobile-android-mediation/blob/master/ThirdPartyAdapters/adcolony/CHANGELOG.md#version-4100).
- Supports [AdColony iOS adapter version 4.1.2.0](https://github.com/googleads/googleads-mobile-ios-mediation/blob/master/adapters/AdColony/CHANGELOG.md#version-4120).

#### Version 1.0.6
- Supports [AdColony Android adapter version 3.3.11.0](https://github.com/googleads/googleads-mobile-android-mediation/blob/master/ThirdPartyAdapters/adcolony/CHANGELOG.md#version-33110).
- Supports [AdColony iOS adapter version 3.3.8.1.0](https://github.com/googleads/googleads-mobile-ios-mediation/blob/master/adapters/AdColony/CHANGELOG.md#version-33810).

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
