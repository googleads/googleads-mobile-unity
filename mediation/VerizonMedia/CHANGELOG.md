## Verizon Media Unity Mediation Plugin Changelog

#### Next Version
- Renamed the `SetConsentData()` method from the `VerizonMedia` class to `SetPrivacyData()`.
- Supports [Verizon Media Android adapter version 1.3.0.0](https://github.com/googleads/googleads-mobile-android-mediation/blob/master/ThirdPartyAdapters/verizonmedia/CHANGELOG.md#version-1300).
- Supports [Verizon Media iOS adapter version 1.3.0.1](https://github.com/googleads/googleads-mobile-ios-mediation/blob/master/adapters/VerizonMedia/CHANGELOG.md#version-1301).

#### Version 1.0.1
- Added the following methods to the `VerizonMedia` class to support GDPR requirements:
  * `GetVerizonIABConsentKey()` to get the Verizon Media SDK's IAB Consent Key.
  * `SetConsentData()` to pass GDPR consent data to the Verizon Media SDK.

#### Version 1.0.0
- Supports banner and interstitial ads.
- Supports Verizon Media Android adapter version 1.1.1.0.
- Supports Verizon Media iOS adapter version 1.1.2.0.
