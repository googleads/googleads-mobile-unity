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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Unity
{
    public class BaseAdClient
    {
        protected static AdBehaviour AdBehaviour = new GameObject().AddComponent<AdBehaviour>();
        protected string _adUnitId;
        protected GameObject prefabAd, dummyAd = null;

        public void LoadAndSetPrefabAd(string prefabName)
        {
            prefabAd = Resources.Load(prefabName) as GameObject;
            if (prefabAd == null)
            {
                Debug.Log("No Prefab found");
                return;
            }
            // Setting the maximum sortingOrder ensures highest priority for rendering the ad.
            Canvas canvas = prefabAd.GetComponent<Canvas>();
            if (canvas != null)
            {
                // sortingOrder is a 16 bit int so the maximum value is 32767.
                canvas.sortingOrder = 32767;
            }
        }

        public RectTransform getRectTransform(GameObject prefabAd) {
            Image myImage = prefabAd.GetComponentInChildren<Image>();
            return myImage.GetComponent<RectTransform>();
        }

        // Returns the mediation adapter class name.
        public string MediationAdapterClassName()
        {
            return new ResponseInfoClient().GetMediationAdapterClassName();
        }

        // Returns the ad unit ID.
        public string GetAdUnitID()
        {
            return _adUnitId;
        }

        // Returns ad request Response info client.
        public IResponseInfoClient GetResponseInfoClient()
        {
            return new ResponseInfoClient();
        }
    }
}
