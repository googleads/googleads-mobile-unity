# Security Threat Model

## Asset Definition & Scope

-   **Asset Name:** GMA SDK (Unity) - SDK Wrapper / Demo
-   **Asset Path:**
    `google3/java/com/google/android/libraries/admob/demo/unity/googlemobileads/`
-   **Platform:** Google3 (Piper)
-   **Asset Type:** Code Target (C# Unity Plugin Wrapper)

This component is the Unity wrapper SDK for Google Mobile Ads, enabling Unity
game publishers to integrate AdMob/GMA capabilities (interstitials, banner ads,
rewarded ads) on Android and iOS platforms.

## Prioritization Signals

-   **1P OSS:** Yes (Google-owned open-source wrapper code)
-   **1P Proprietary Shipped Software:** No (distributed as SDK package to 3P/1P
    developers)
-   **High-Risk Code Surface:** Yes (Uses Android JNI interop, iOS DllImport
    native bridge, parses XML/Gradle files at Editor build time, and includes
    network telemetry telemetry code)
-   **Perimeter Exposure:** No
-   **Data Sensitivity:** Medium (collects and reports device metadata,
    exception stack traces, session IDs, app IDs, network types, orientation,
    and country to `googlesyndication.com`)
-   **Untrusted Input Handling:** Yes (handles untrusted ad configurations and
    callback responses from JNI/native ad server layers)
-   **Business Value:** High (crucial integration vector for mobile game
    developers monetizing with AdMob)

## Scanning Harness Prompts

-   Focus on verifying the certificate validation logic in
    `google3/java/com/google/android/libraries/admob/demo/unity/googlemobileads/source/plugin/Assets/GoogleMobileAds/Common/RcsClient.cs`
    under the `BypassCertificateHandler` class to see if certificate validation
    can be strictly enforced in production rather than blindly returning `true`.
-   Search for potential JNI-based type-confusion, buffer overflows, or
    null-pointer dereferences where the wrapper receives data across JNI or
    native C boundaries (e.g. in `MobileAdsClient.cs`).
-   Analyze XML parsing practices in `ManifestProcessor.cs` to ensure
    `XDocument.Load()` does not trigger XXE (XML External Entity) or XML
    injection flaws when processing Android manifests during build time.

## Entry Points and Untrusted Inputs

| Entry Point            | Type         | Trusted? | Validation                |
| ---------------------- | ------------ | -------- | ------------------------- |
| `MobileAds.Initialize` | C# API       | Yes      | Called by developer;      |
: / ad configuration     :              :          : standard type bounds and  :
: APIs                   :              :          : validation                :
| JNI Callbacks from     | JNI Boundary | No       | Handled by Android/iOS    |
: Native Ads SDK         :              :          : native SDK; marshaled to  :
:                        :              :          : C# structures             :
| `AndroidManifest.xml`  | XML File     | Yes      | Parsed via Unity Editor   |
: processing             :              :          : script; could be modified :
:                        :              :          : by malicious              :
:                        :              :          : assets/plugins            :

## Trust Boundaries and Auth Assumptions

-   **Authentication**: None
-   **Authorization**: None
-   **Implicit trust**: C# wrapper trusts native Android/iOS SDK execution and
    structures.
-   **Boundary crossings**: Unity C# runtime to Android JVM or iOS native
    runtime via JNI (`AndroidJavaObject` / `AndroidJavaProxy` /
    `AndroidJNI.AttachCurrentThread`) and iOS `DllImport("__Internal")`.

## Sensitive Data Paths

| Data Type    | Source                      | Destination   | Protection    |
| ------------ | --------------------------- | ------------- | ------------- |
| Device       | `RcsPayload.cs`             | RCS Telemetry | Bypasses      |
: metadata &   :                             : Server        : SSL/TLS       :
: session info :                             :               : certificate   :
:              :                             :               : validation in :
:              :                             :               : `RcsClient`   :
| Stack traces | `GlobalExceptionHandler.cs` | RCS Telemetry | Bypasses      |
: & Exception  :                             : Server        : SSL/TLS       :
: messages     :                             :               : certificate   :
:              :                             :               : validation in :
:              :                             :               : `RcsClient`   :

## Privileged Actions

Action                               | Location                                             | Guard
------------------------------------ | ---------------------------------------------------- | -----
Post Telemetry via `UnityWebRequest` | `GoogleMobileAds.Common.RcsClient.PostRequest`       | None. Bypasses SSL/TLS certificate validation
Call Native Java SDK                 | `GoogleMobileAds.Android.MobileAdsClient.Initialize` | None
Process and edit manifest files      | `ManifestProcessor.OnPreprocessBuild`                | None

## Priority Review Areas

1.  **Telemetry TLS Validation Bypass (`BypassCertificateHandler`)**: Bypassing
    TLS validation for telemetry and exception logs exposes user session IDs,
    device models, and exception traces (which may contain PII or sensitive
    paths) to person-in-the-middle (PITM) interception.
2.  **JNI Marshalling and Exception Handling**: Uncaught native exceptions or
    unexpected values returned from the JNI boundary in `MobileAdsClient` could
    crash the C# runtime or lead to exploitation.
3.  **XML Parsing Security in Editor Scripts**: Ensure `XDocument` usage in
    `ManifestProcessor` is secure against XXE attacks if arbitrary or
    developer-imported third-party assets can craft custom XML inputs during
    compilation.
