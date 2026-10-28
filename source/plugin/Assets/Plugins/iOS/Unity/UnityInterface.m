#import "UnityInterface.h"

#include <stdbool.h>

// Global flag exported by the Unity iOS player runtime. Checked during ad dismissal across
// full-screen ad presentations to avoid invoking script callbacks if the runtime was already
// destroyed during application exit.
bool _didResignActive = false;

static int gUnityIsPaused = 0;

void UnityPause(int pause) { gUnityIsPaused = pause; }

int UnityIsPaused(void) { return gUnityIsPaused; }
