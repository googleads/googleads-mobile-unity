#ifndef JAVA_COM_GOOGLE_ANDROID_LIBRARIES_ADMOB_DEMO_UNITY_GOOGLEMOBILEADS_SOURCE_PLUGIN_ASSETS_PLUGINS_IOS_UNITY_UNITYINTERFACE_H_
#define JAVA_COM_GOOGLE_ANDROID_LIBRARIES_ADMOB_DEMO_UNITY_GOOGLEMOBILEADS_SOURCE_PLUGIN_ASSETS_PLUGINS_IOS_UNITY_UNITYINTERFACE_H_

// Declares runtime symbols exported by the Unity iOS player runtime.
//
// Retains the canonical name `UnityInterface.h` matching the header generated
// by Unity under `Classes/Unity/` in exported iOS Xcode projects. In Unity's
// Xcode project structure, the `Classes/` directory contains native iOS
// integration code:
// https://docs.unity.com/en-us/engine/7000.0/manual/platform-specific/iphone/ios-developing/structure-of-xcode-project
//
// While Unity's documentation outlines the overall `Classes/` structure rather
// than indexing internal runtime headers individually, Unity exports
// `Classes/Unity/UnityInterface.h` to declare player runtime functions (such as
// `UnityPause` and `UnityIsPaused`). Retaining this canonical name allows
// plugin source files that import this header to compile cleanly in standalone
// test environments without modifying production sources.

#ifdef __cplusplus
extern "C" {
#endif

// Pauses or resumes the Unity player runtime. Used across full-screen ad
// presentations to pause gameplay when ads appear and resume upon dismissal.
void UnityPause(int pause);

// Returns 1 if the Unity player runtime is paused, or 0 if running. Used to
// determine whether to resume runtime execution upon ad dismissal.
int UnityIsPaused(void);

#ifdef __cplusplus
}
#endif

#endif  // JAVA_COM_GOOGLE_ANDROID_LIBRARIES_ADMOB_DEMO_UNITY_GOOGLEMOBILEADS_SOURCE_PLUGIN_ASSETS_PLUGINS_IOS_UNITY_UNITYINTERFACE_H_
