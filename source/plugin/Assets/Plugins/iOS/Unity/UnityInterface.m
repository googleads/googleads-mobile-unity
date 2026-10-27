#import "UnityInterface.h"

#include <stdbool.h>

// Global flag tracking application active state.
bool _didResignActive = false;

static int gUnityIsPaused = 0;

void UnityPause(int pause) { gUnityIsPaused = pause; }

int UnityIsPaused(void) { return gUnityIsPaused; }

void UnitySendMessage(const char *obj, const char *method, const char *msg) {
  // No-op for unit testing and headless environments.
}
