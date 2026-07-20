# Google Mobile Ads (GMA) Unity Plugin — Agent Guidelines

## Project Overview

The Google Mobile Ads (GMA) Unity Plugin lets Unity developers serve mobile ads
on Android and iOS without writing Java or Objective-C code directly in their
game scripts. The plugin provides a C# interface (`GoogleMobileAds.Api`) that
wraps the native Android (`com.google.android.gms.ads`) and iOS
(`GoogleMobileAds`) SDKs.

-   **Primary Codebase**:
    `google3/java/com/google/android/libraries/admob/demo/unity/googlemobileads/`
-   **Open-Source Repository**: Synchronized to GitHub
    (`googleads/googleads-mobile-unity`) using Copybara rules defined in
    `opensource/copy.bara.sky`.

## Architecture

The plugin follows a 3-layer bridge architecture:

1.  **C# API (`GoogleMobileAds.Api` / `Common`)**: Game developer-facing classes
    (`BannerView`, `InterstitialAd`, `RewardedAd`, `MobileAds`).
2.  **Platform Bridges (`GoogleMobileAds.Platforms`)**:
    -   **Android (`Platforms/Android/`)**: Uses `AndroidJavaClass`,
        `AndroidJavaObject`, and `AndroidJavaProxy` (`Utils.cs`) to interop with
        the Android JVM.
    -   **iOS (`Platforms/iOS/`)**: Uses P/Invoke `[DllImport("__Internal")]`
        (`Externs.cs`) to call native Objective-C C-linkage functions, and
        `[MonoPInvokeCallback]` delegates for callbacks.
3.  **Native Plugin Libs (`Assets/Plugins/`)**:
    -   **Android**: `Assets/Plugins/Android/GoogleMobileAdsPlugin.androidlib/`
        containing Java bridge classes (`com.google.unity.ads.*`).
    -   **iOS**: Objective-C/Swift native wrapper sources and headers.

## Key Files & Entry Points

-   `BUILD`: Main Blaze build targets (`unitypackage_public`, `basic_unittests`,
    `gmscore_package`).
-   `source/plugin/Assets/GoogleMobileAds/Api/MobileAds.cs`: Entry point for SDK
    initialization and configuration.
-   `source/plugin/Assets/GoogleMobileAds/Common/Utils.cs`: C# / Java / ObjC
    type conversion helpers.
-   `source/plugin/Assets/GoogleMobileAds/Editor/PListProcessor.cs`: Xcode
    post-build hook (`[PostProcessBuild]`) injecting App IDs and SKAdNetwork
    items into `Info.plist`.
-   `source/plugin/Assets/GoogleMobileAds/Editor/GradleProcessor.cs`: Android
    build hook editing `build.gradle` and managing NextGen dependencies.
-   `source/plugin/Assets/GoogleMobileAds/Editor/IntegrationManager/IntegrationManagerWindow.cs`:
    Modern UI Elements-based Editor window (`Google Mobile Ads > Manage
    Mediation`) for managing mediation adapters.
-   `source/plugin/Assets/GoogleMobileAds/Editor/MediationManager/PackageDataHandler.cs`:
    Backend handler querying OpenUPM and Unity Package Manager (`UPM`) for
    mediation adapters.
-   `opensource/copy.bara.sky`: Defines public export workflow, EAP comment
    stripping, and internal path exclusions.

## Build & Test Commands

### Building the Unity Package

To build the public `.unitypackage` with full iOS cross-compilation support:

```bash
blaze --blazerc=/google/src/head/depot/google3/java/com/google/android/gmscore/blaze/blazerc build //java/com/google/android/libraries/admob/demo/unity/googlemobileads:unitypackage_public --config=gmscore_sdk --config=ios_common --ios_multi_cpus=arm64,x86_64 --ios_minimum_os=13.0 --copt=-Wno-unguarded-availability-new --config=apple_cross_compile
```

### Running Unit Tests

To run the basic C# unit tests (using NSubstitute mocks) and Android Java bridge
tests:

```bash
blaze test //java/com/google/android/libraries/admob/demo/unity/googlemobileads:basic_unittests
```

## Gotchas & Anti-Patterns

1.  **EDM4U Dependency Trap**: Do **NOT** add precompiled External Dependency
    Manager for Unity (EDM4U) DLLs (like `Google.VersionHandler.dll`) to `BUILD`
    `srcs` to satisfy compiler warnings. Doing so forces untracked binaries into
    the build input and breaks headless Forge / Test Welder execution.
2.  **Unity 2019.4 IStyle Shorthand Ban**: Unity 2019.4 UIElements `IStyle` does
    not support shorthand properties (`borderRadius`, `borderWidth`,
    `borderColor`). Setting them throws compilation errors (`CS1061`). Always
    expand to individual side properties (`borderLeftWidth`,
    `borderTopLeftRadius`, etc.).
3.  **Headless `EditorWindow` Crashes**: Never put UI layout logic directly
    inside an `EditorWindow` class if that window or tool will be tested in
    headless mode (`Test Welder` / Forge). `EditorWindow` cannot be instantiated
    in batch mode (`NullReferenceException`). Extract layout into a
    `VisualElement` subclass view.
4.  **Guarding Native Editor APIs**: Native graphical Editor APIs
    (`EditorGUIUtility.IconContent` or texture loaders) throw
    `TypeInitializationException` in headless mode due to missing graphical
    contexts. Always wrap these calls in `try-catch` blocks in UI view classes
    with clean text fallbacks (`"ⓘ"` or `"X"`).
5.  **Headless Click Simulation**: Standard `button.SendEvent(new ClickEvent())`
    fails in headless mode because layout bounds are zero (`NaN`), failing the
    `ContainsPoint` check. To simulate clicks synchronously in tests, use C#
    reflection to invoke the private `clicked` delegate on `button.clickable`.
6.  **OpenUPM Dictionary Deserialization**: Unity's `JsonUtility` does not
    deserialize C# `Dictionary<string, object>`. Do not rely on dictionary
    deserialization for OpenUPM payload parsing in `PackageDataHandler.cs`; use
    strongly-typed serializable structs.
7.  **Unity .meta Files & GUIDs**: Unity requires matching `.meta` files and
    unique GUID registration in `internal/guids.json` whenever new C# scripts or
    Editor assets are added. Failing to register new GUIDs breaks presubmits and
    package generation.
8.  **EAP Code Stripping**: Internal-only or Early Access Program (EAP) features
    inside public classes must be wrapped in `// START_EAP_STRIP` and `//
    END_EAP_STRIP` comments so Copybara strips them before public release.

## Task-Based Routing

If your task involves...                              | Then investigate...
:---------------------------------------------------- | :------------------
Adding or moving C# / Editor scripts                  | `development.md`
Building the package or running unit tests            | `building_and_testing.md`
Creating or modifying Editor UI / Integration Manager | `IntegrationManagerWindow.cs`, `PackageDataHandler.cs`
Modifying open-source sync or adding internal files   | `opensource/copy.bara.sky`
NextGen SDK clients (`NextGen*Client.cs`)             | `source/plugin/Assets/GoogleMobileAds/Platforms/Android/NextGen*` and `GradleProcessor.cs`
iOS post-process build injections                     | `source/plugin/Assets/GoogleMobileAds/Editor/PListProcessor.cs`

## Available Skills & Rules

Skills placed inside `_agents/skills/<skill_name>/SKILL.md` and rules inside
`_agents/rules/*.md` are automatically discovered and loaded by Jetski when
working in this repository.
