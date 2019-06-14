# AdColony Adapter plugin for Google Mobile Ads SDK for Unity 3D Changelog

## 1.0.5
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

## 1.0.4
- Updated the plugin to support the new open-beta Rewarded API.
- Supports AdColony Android adapter version 3.3.8.1.
- Supports AdColony iOS adapter version 3.3.6.1.

## 1.0.3
- Supports AdColony Android adapter version 3.3.5.1.
- Supports AdColony iOS adapter version 3.3.5.0.

## 1.0.2
- Supports AdColony Android SDK version 3.3.4.
- Supports AdColony iOS SDK version 3.3.4.
- `SetTestMode()` from the `AdColonyMediationExtras` Builder class is now ignored for Android. Publishers can now request test ads from AdColony for Android by specifying a test device via `AddTestDevice()` from the `AdRequest` Builder class.
- Added the following methods to the `AdColonyMediationExtras` Builder class:
  * `SetGDPRRequired()`
  * `SetGDPRConsentString()`

## 1.0.1
- Supports AdColony Android SDK version 3.3.0-unity.
- Supports AdColony iOS SDK version 3.3.0.

## 1.0.0
- First release!
- Supports AdColony Android SDK version 3.3.0.
- Supports AdColony iOS SDK version 3.3.0.
