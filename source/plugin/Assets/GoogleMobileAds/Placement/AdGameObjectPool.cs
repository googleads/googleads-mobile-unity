// Copyright (C) 2020 Google LLC
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
using System.Collections.Generic;

using UnityEngine;

namespace GoogleMobileAds.Placement
{

    public class AdGameObjectPool
    {
        private static AdGameObjectPool instance;

        private Dictionary<string, PoolEntry> objects;

        private AdGameObjectPool()
        {
            objects = new Dictionary<string, PoolEntry>();
        }

        public static AdGameObjectPool Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AdGameObjectPool();
                }
                return instance;
            }
        }

        public void Add(Type adPlacementType, GameObject go, bool persistent)
        {
            if (go == null)
            {
                throw new ArgumentException("GameObject cannot be null");
            }

            string objectKey = GenerateObjectKey(adPlacementType, go.name);

            // Check for duplicates.
            PoolEntry entry = null;
            objects.TryGetValue(objectKey, out entry);

            if (entry != null)
            {
                if (persistent && entry.SceneNameOrigin.Equals(go.scene.name))
                {
                    // Unity has created another instance of persistent ad placement.
                    // (intended behavior)
                    throw new OperationCanceledException();
                }

                throw new ArgumentException(string.Format(
                    "Can't add Ad placement (name={0} scene={1}) to the scene. (previously added from {2} scene)",
                    go.name, go.scene.name, entry.SceneNameOrigin));
            }

            if (!adPlacementType.IsSubclassOf(typeof(AdGameObject)))
            {
                throw new ArgumentException("Only AdGameObject type is allowed.");
            }

            Component placement = go.GetComponent(adPlacementType);
            if (placement == null)
            {
                throw new ArgumentException("GameObject does not have AdPlacement.");
            }

            // Add a GameObject to the object pool.
            objects.Add(objectKey, new PoolEntry(go, persistent));
        }

        public void Remove(Type adPlacementType, GameObject go, bool persistent)
        {
            objects.Remove(GenerateObjectKey(adPlacementType, go.name));
        }

        public TAdGameObject GetAd<TAdGameObject>(string placementName)
        {
            GameObject go = GetAdGameObject<TAdGameObject>(
                GenerateObjectKey(typeof(TAdGameObject), placementName));

            if (go == null)
            {
                throw new ArgumentException("Ad Placement named " + placementName + " does not exist.");
            }

            return GetAdFromGameObject<TAdGameObject>(go);
        }

        private TAdGameObject GetAdFromGameObject<TAdGameObject>(GameObject adGameObject)
        {
            TAdGameObject placement = adGameObject.GetComponent<TAdGameObject>();
            if (placement == null)
            {
                throw new InvalidOperationException("GameObject does not have AdPlacement data");
            }

            return placement;
        }

        private GameObject GetAdGameObject<TAdGameObject>(string objectKey)
        {
            PoolEntry obj = null;
            objects.TryGetValue(objectKey, out obj);

            return obj != null ? obj.GameObject : null;
        }

        private string GenerateObjectKey(Type adPlacementType, string gameObjectName)
        {
            return string.Format("{0}:{1}", adPlacementType.Name, gameObjectName);
        }

        ~AdGameObjectPool()
        {
            objects.Clear();
        }
    }

    internal class PoolEntry
    {
        private GameObject gameObject;

        private string sceneNameOrigin;

        private bool persistent;

        public GameObject GameObject
        {
            get
            {
                return this.gameObject;
            }
        }

        public string SceneNameOrigin
        {
            get
            {
                return this.sceneNameOrigin;
            }
        }

        public bool IsPersistent
        {
            get
            {
                return this.persistent;
            }
        }

        public PoolEntry(GameObject gameObject, bool persistent)
        {
            this.gameObject = gameObject;
            this.sceneNameOrigin = gameObject.scene.name;
            this.persistent = persistent;
        }
    }
}
