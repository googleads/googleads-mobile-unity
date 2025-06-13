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
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace GoogleMobileAds.Common
{
    public class MobileAdsEventExecutor : MonoBehaviour
    {
        public static MobileAdsEventExecutor instance = null;

        private static List<Action> adEventsQueue = new List<Action>();

        private volatile static bool adEventsQueueEmpty = true;

        // The managed thread id of the Unity main thread.
        private static int UnityMainThreadId = -1;

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

        /// <summary>
        /// Returns true if the current thread is the Unity main thread.
        /// </summary>
        public static bool IsOnMainThread()
        {
            return Thread.CurrentThread.ManagedThreadId == UnityMainThreadId;
        }

        public static bool IsActive()
        {
            return instance != null;
        }

        public void Awake()
        {
            if (UnityMainThreadId == -1)
            {
                UnityMainThreadId = Thread.CurrentThread.ManagedThreadId;
            }
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

        public static void InvokeInUpdate(UnityEvent eventParam)
        {
          ExecuteInUpdate(() =>
          {
              eventParam.Invoke();
          });
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
                if (stagedEvent.Target != null)
                {
                    stagedEvent.Invoke();
                }
            }
        }

        public void OnDisable()
        {
            instance = null;
        }
    }
}
