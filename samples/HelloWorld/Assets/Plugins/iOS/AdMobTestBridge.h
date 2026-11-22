// Copyright 2026 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#import <UIKit/UIKit.h>

#if defined(__cplusplus)
extern "C" {
#endif

// Initializes the AdMob test bridge overlay container and status label.
void AdMobTest_Init(void);

// Updates the accessibility label and value of the status label to the given
// event.
void AdMobTest_SetLastEvent(const char* eventName);

// Registers or updates a native overlay button at the specified normalized
// coordinates.
void AdMobTest_RegisterButton(const char* buttonId, float normX, float normY,
                              float normWidth, float normHeight);

// Clears all currently registered overlay buttons.
void AdMobTest_ClearButtons(void);

#if defined(__cplusplus)
}
#endif
