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
using UnityEngine.UI;

using GoogleMobileAds.Ump.Common;

namespace GoogleMobileAds.Ump.Unity
{
    internal class ConsentFormClient : IConsentFormClient
    {
        internal GameObject prefabForm, dummyForm = null;
        private ButtonBehaviour _buttonBehaviour;
        private static readonly DummyFormBehaviour _formBehaviour =
                new GameObject().AddComponent<DummyFormBehaviour>();
        internal const int ErrorCode = 404;
        internal const string ErrorMessage = "Form not found!";

        /// <summary>
        /// Loads a prefab of <see cref="GoogleMobileAds.Ump.Api.ConsentForm">ConsentForm</see>
        /// from resources if it exists.
        /// Call <see cref="Show"/> to add the form to the scene.
        /// <paramref name="onLoad">Called when the form is successfully loaded.</paramref>
        /// <paramref name="onError">Called when form couldn't load.
        /// Provides <see cref="FormError"/> as argument for details about the error.</paramref>
        /// </summary>
        public void LoadConsentForm(Action onLoad, Action<FormError> onError)
        {
            if (prefabForm == null)
            {
                prefabForm = Resources.Load("Ump/ConsentForm") as GameObject;
            }
            if (prefabForm == null)
            {
                Debug.Log("No Prefab found for form.");
                onError(new FormError(ErrorCode, ErrorMessage));
            }
            else
            {
                Debug.Log("Consent Form Loaded.");
                onLoad();
            }
        }

        /// <summary>
        /// Shows an instance of <see cref="GoogleMobileAds.Ump.Api.ConsentForm">ConsentForm</see>
        /// to the scene with click behaviour and pauses the game if a prefab exists.
        /// Call <see cref="LoadConsentForm"/> to load the prefab for the form.
        /// <paramref name="onDismiss">Called when the form is dismissed.</paramref>
        /// </summary>
        public void Show(Action<FormError> onDismiss)
        {
            if (prefabForm != null)
            {
                dummyForm = _formBehaviour.ShowForm(prefabForm, new Vector3(0, 0, 1));
                CreateButtonBehavior();
                AddClickBehavior(dummyForm, onDismiss);
                Debug.Log("Consent Form Shown.");
                _formBehaviour.PauseGame();
            }
            else
            {
                Debug.Log("Consent Form not loaded.");
                onDismiss(new FormError(ErrorCode, ErrorMessage));
            }
        }

        /// <summary>
        /// Adds <see cref="Button.ButtonClickedEvent"/> to the Consent button.
        /// <paramref name="dummy">A dummy form instance</paramref>
        /// <paramref name="onClick">Called when consent button is clicked
        /// (and the form is dismissed).</paramref>
        /// </summary>
        private void AddClickBehavior(GameObject dummy, Action<FormError> onClick)
        {
            Image[] images = dummy.GetComponentsInChildren<Image>();
            Image adImage = images[1];
            Button button = adImage.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => {
                _buttonBehaviour.OpenURL();
            });

            Button[] innerButtons = adImage.GetComponentsInChildren<Button>();
            innerButtons[1].onClick.AddListener(() =>
            {
                // ConsentStatus.Obtained
                PlayerPrefs.SetInt(ConsentInformationClient.PlayerPrefsKeyConsentStatus, 3);
                DestroyConsentForm();
                _formBehaviour.ResumeGame();
                onClick(null);
            });
        }

        /// <summary>
        /// Assigns <see cref="ButtonBehaviour"/> to <see cref="dummyForm"/>.
        /// </summary>
        private void CreateButtonBehavior()
        {
            _buttonBehaviour = dummyForm.AddComponent<ButtonBehaviour>();
        }

        /// <summary>
        /// Removes the existing Consent Form from memory so that a fresh one can be loaded.
        /// </summary>
        private void DestroyConsentForm()
        {
            _formBehaviour.DestroyForm(dummyForm);
            prefabForm = null;
        }
    }
}
