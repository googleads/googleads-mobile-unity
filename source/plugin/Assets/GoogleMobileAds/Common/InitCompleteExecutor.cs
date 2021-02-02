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

namespace GoogleMobileAds.Common
{
    public class InitCompleteExecutor
    {
        private static InitCompleteExecutor instance;

        private List<Action> stagedEvents;

        private readonly object sdkInitStateLock;

        private volatile bool isSdkInitialized = false;

        public static InitCompleteExecutor Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InitCompleteExecutor();
                }

                return instance;
            }
        }

        private InitCompleteExecutor()
        {
            sdkInitStateLock = new object();
            isSdkInitialized = false;
            stagedEvents = new List<Action>();
        }

        public void NotifySdkInitialized()
        {
            lock (sdkInitStateLock)
            {
                if (isSdkInitialized)
                {
                    return;
                }

                isSdkInitialized = true;

                foreach (Action stagedEvent in stagedEvents)
                {
                    MobileAdsEventExecutor.ExecuteInUpdate(stagedEvent);
                }
            }
        }

        public void InvokeOnInitComplete(Action action)
        {
            lock (sdkInitStateLock)
            {
                if (!isSdkInitialized)
                {
                    stagedEvents.Add(action);
                }
                else
                {
                    action.Invoke();
                }
            }
        }

        ~InitCompleteExecutor()
        {
            stagedEvents.Clear();
        }
    }
}
