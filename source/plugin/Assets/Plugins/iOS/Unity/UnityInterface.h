#ifndef JAVA_COM_GOOGLE_ANDROID_LIBRARIES_ADMOB_DEMO_UNITY_GOOGLEMOBILEADS_SOURCE_PLUGIN_ASSETS_PLUGINS_IOS_UNITY_UNITYINTERFACE_H_
#define JAVA_COM_GOOGLE_ANDROID_LIBRARIES_ADMOB_DEMO_UNITY_GOOGLEMOBILEADS_SOURCE_PLUGIN_ASSETS_PLUGINS_IOS_UNITY_UNITYINTERFACE_H_

#ifdef __cplusplus
extern "C" {
#endif

// Pauses or resumes the Unity player runtime.
void UnityPause(int pause);

// Returns 1 if the Unity player runtime is paused, or 0 if running.
int UnityIsPaused(void);

// Sends a message to a Unity GameObject by name.
void UnitySendMessage(const char* obj, const char* method, const char* msg);

#ifdef __cplusplus
}
#endif

#endif  // JAVA_COM_GOOGLE_ANDROID_LIBRARIES_ADMOB_DEMO_UNITY_GOOGLEMOBILEADS_SOURCE_PLUGIN_ASSETS_PLUGINS_IOS_UNITY_UNITYINTERFACE_H_
