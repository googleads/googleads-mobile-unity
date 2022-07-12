// Copyright (C) 2020 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Unity
{
    public class AdInspectorClient : BaseAdClient
    {
        public void OpenAdInspector(Action<IAdErrorClient> onAdInspectorClosed)
        {
            LoadPrefab("DummyAds/AdInspector/768x1024");

            if (adPrefab == null)
            {
                if(onAdInspectorClosed != null)
                {
                    onAdInspectorClosed(new AdError());
                }
                return;
            }

            LoadAd(new Vector3(0, 0, 1));
            Image[] images = adInstance
                .GetComponentsInChildren<Image>();
            Image adInspectorImage = images[1];
            Button[] innerButtons = adInspectorImage
                .GetComponentsInChildren<Button>();

            innerButtons[1].onClick.AddListener(() =>
            {
                Destroy();
                ResumeGame();
            });

            PauseGame();
        }
    }
}
