## Vungle Unity Mediation Plugin Changelog

#### Version 3.4.0
- Supports [Vungle Android adapter version 6.7.0.0](https://github.com/googleads/googleads-mobile-android-mediation/blob/master/ThirdPartyAdapters/vungle/CHANGELOG.md#version-6700).
- Supports [Vungle iOS adapter version 6.7.0.0](https://github.com/googleads/googleads-mobile-ios-mediation/blob/master/adapters/Vungle/CHANGELOG.md#version-6700).

#### Version 3.3.1
- Repackaged `vungle-extras-library.jar` to `vungle-unity-android-library.aar`, using a `.aar` build to support Unity 2020.
  * When upgrading to this version, please remove `vungle-extras-library.jar` from your project.
- Supports [Vungle Android adapter version 6.5.3.0](https://github.com/googleads/googleads-mobile-android-mediation/blob/master/ThirdPartyAdapters/vungle/CHANGELOG.md#version-6530).
- Supports [Vungle iOS adapter version 6.5.3.0](https://github.com/googleads/googleads-mobile-ios-mediation/blob/master/adapters/Vungle/CHANGELOG.md#version-6530).

#### Version 3.3.0
- Supports [Vungle Android adapter version 6.5.3.0](https://github.com/googleads/googleads-mobile-android-mediation/blob/master/ThirdPartyAdapters/vungle/CHANGELOG.md#version-6530).
- Supports [Vungle iOS adapter version 6.5.3.0](https://github.com/googleads/googleads-mobile-ios-mediation/blob/master/adapters/Vungle/CHANGELOG.md#version-6530).

#### Version 3.2.0
- Supports [Vungle Android adapter version 6.4.11.1](https://github.com/googleads/googleads-mobile-android-mediation/blob/master/ThirdPartyAdapters/vungle/CHANGELOG.md#version-64111).
- Supports [Vungle iOS adapter version 6.4.6.0](https://github.com/googleads/googleads-mobile-ios-mediation/blob/master/adapters/Vungle/CHANGELOG.md#version-6460).

#### Version 3.1.4
- Supports Android adapter version 6.3.24.1.
- Supports iOS adapter version 6.3.2.3.

#### Version 3.1.3
- Supports Android adapter version 6.3.24.1.
- Supports iOS adapter version 6.3.2.2.

#### Version 3.1.2
- Updated the plugin to support the new open-beta Rewarded API.
- Supports Android adapter version 6.3.24.1.
- Supports iOS adapter version 6.3.2.1.

#### Version 3.1.1
- Supports Android adapter version 6.3.24.0.
- Supports iOS adapter version 6.3.2.0.
- Updated `Vungle.UpdateConsentStatus()` method to make the `consentMessageVersion` parameter optional. The value of `consentMessageVersion` is now ignored for iOS.
- Deprecated `Vungle.GetCurrentConsentMessageVersion()` for iOS.

#### Version 3.1.0
- Supports Android adapter version 6.3.17.0.
- Supports iOS adapter version 6.3.0.0.
- Updated `Vungle.UpdateConsentStatus()` method to take an additional `String` parameter for the publiser-provided consent message version.
- Added `Vungle.GetCurrentConsentMessageVersion()` method to get the publisher-provided consent message version.

#### Version 3.0.1
- Supports Android adapter version 6.2.5.1.
- Supports iOS adapter version 6.2.0.3.

#### Version 3.0.0
- Supports Android adapter version 6.2.5.0.
- Supports iOS adapter version 6.2.0.2.
- Added the following methods:
  * `Vungle.UpdateConsentStatus()`  method to set the consent status that will be recorded in the Vungle SDK.
  * `Vungle.GetCurrentConsentStatus()` method to get the user's current consent status.

#### Version 2.0.0
- Supports Android adapter version 5.3.2.1.
- Supports iOS adapter version 6.2.0.0.

#### Version 1.1.0
- Supports Android adapter version 5.3.2.1.
- Supports iOS adapter version 5.4.0.0.

#### Version 1.0.0
- First release!
- Supports Android adapter version 5.3.0.0.
- Supports iOS adapter version 5.3.0.0.
