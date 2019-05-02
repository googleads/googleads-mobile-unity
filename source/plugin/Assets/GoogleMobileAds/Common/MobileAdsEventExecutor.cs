// Copyright (C) 2018 Google, Inc.
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
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace GoogleMobileAds.Common
{
    public class MobileAdsEventExecutor : MonoBehaviour
    {
        public static MobileAdsEventExecutor instance = null;

        private static List<Action> adEventsQueue = new List<Action>();

        private volatile static bool adEventsQueueEmpty = true;

        public static void Initialize()
        {
            if (IsActive())
            {
                return;
            }

            // Add an invisible game object to the scene
            GameObject obj = new GameObject("MobileAdsMainThreadExecuter");
            obj.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(obj);
            instance = obj.AddComponent<MobileAdsEventExecutor>();
        }

        public static bool IsActive()
        {
            return instance != null;
        }

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public static void ExecuteInUpdate(Action action)
        {
            lock (adEventsQueue)
            {
                adEventsQueue.Add(action);
                adEventsQueueEmpty = false;
            }
        }
        public void Update()
        {
            if (adEventsQueueEmpty)
            {
                return;
            }

            List<Action> stagedAdEventsQueue = new List<Action>();

            lock (adEventsQueue)
            {
                stagedAdEventsQueue.AddRange(adEventsQueue);
                adEventsQueue.Clear();
                adEventsQueueEmpty = true;
            }

            foreach (Action stagedEvent in stagedAdEventsQueue)
            {
                stagedEvent.Invoke();
            }
        }

        public void OnDisable()
        {
            instance = null;
        }
    }
}
