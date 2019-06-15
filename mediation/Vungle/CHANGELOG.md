# Vungle Adapter plugin for Google Mobile Ads SDK for Unity 3D Changelog

## 3.1.3
- Supports Android adapter version 6.3.24.1.
- Supports iOS adapter version 6.3.2.2.

## 3.1.2
- Updated the plugin to support the new open-beta Rewarded API.
- Supports Android adapter version 6.3.24.1.
- Supports iOS adapter version 6.3.2.1.

## 3.1.1
- Supports Android adapter version 6.3.24.0.
- Supports iOS adapter version 6.3.2.0.
- Updated `Vungle.UpdateConsentStatus()` method to make the `consentMessageVersion` parameter optional. The value of `consentMessageVersion` is now ignored for iOS.
- Deprecated `Vungle.GetCurrentConsentMessageVersion()` for iOS.

## 3.1.0
- Supports Android adapter version 6.3.17.0.
- Supports iOS adapter version 6.3.0.0.
- Updated `Vungle.UpdateConsentStatus()` method to take an additional `String` parameter for the publiser-provided consent message version.
- Added `Vungle.GetCurrentConsentMessageVersion()` method to get the publisher-provided consent message version.

## 3.0.1
- Supports Android adapter version 6.2.5.1.
- Supports iOS adapter version 6.2.0.3.

## 3.0.0
- Supports Android adapter version 6.2.5.0.
- Supports iOS adapter version 6.2.0.2.
- Added the following methods:
  * `Vungle.UpdateConsentStatus()`  method to set the consent status that will be recorded in the Vungle SDK.
  * `Vungle.GetCurrentConsentStatus()` method to get the user's current consent status.

## 2.0.0
- Supports Android adapter version 5.3.2.1.
- Supports iOS adapter version 6.2.0.0.

## 1.1.0
- Supports Android adapter version 5.3.2.1.
- Supports iOS adapter version 5.4.0.0.

## 1.0.0
- First release!
- Supports Android adapter version 5.3.0.0.
- Supports iOS adapter version 5.3.0.0.
