// Copyright (C) 2022 Google LLC.
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

namespace GoogleMobileAds.Ump.Unity
{
    /// <summary>
    /// A MonoBehaviour that provides a mock behaviour for Consent Form,
    /// when attached to the Consent Form GameObject.
    /// </summary>
    public class DummyFormBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Pause the game (when the form is shown).
        /// </summary>
        public void PauseGame()
        {
            Time.timeScale = 0;
            Debug.Log("Pause Game");
        }

        /// <summary>
        /// Continue playing the game (when the form is dismissed).
        /// </summary>
        public void ResumeGame()
        {
            Time.timeScale = 1;
            Debug.Log("Resume Game");
        }

        /// <summary>
        /// Uses a prefab GameObject to add a Consent Form instance to the scene.
        /// <paramref name="formPrefab">The GameObject loaded from prefab in resources.</paramref>
        /// <paramref name="position">Coordinates to put Consent Form in the scene.</paramref>
        /// </summary>
        public GameObject ShowForm(GameObject formPrefab, Vector3 position)
        {
            return Instantiate(formPrefab, position, Quaternion.identity) as GameObject;
        }

        /// <summary>
        /// Removes the existing Consent Form from memory so that another one can be loaded.
        /// </summary>
        public void DestroyForm(GameObject dummyForm)
        {
            // Destroy is called immediately after the current Update loop (next frame).
            Destroy(dummyForm);
        }
    }
}
