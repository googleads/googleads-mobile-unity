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

using GoogleMobileAds.Ump.Api;
using GoogleMobileAds.Ump.Common;

namespace GoogleMobileAds.Ump.Unity
{
    internal class ConsentFormClient : IConsentFormClient
    {
        internal const int ErrorCode = 7; // Form is unavailable.
        internal const string ErrorMessage = "Form not found!";
        internal static GameObject _prefabForm;
        internal static GameObject _placeholderForm;
        private static readonly PlaceholderFormBehaviour _formBehaviour =
                new GameObject().AddComponent<PlaceholderFormBehaviour>();
        private ButtonBehaviour _buttonBehaviour;

        /// <summary>
        /// Loads a prefab of <see cref="GoogleMobileAds.Ump.Api.ConsentForm">ConsentForm</see>
        /// from resources if it exists. Call <see cref="Show"/> to add the form to the scene.
        /// <paramref name="onFormLoaded">Called when the form is successfully loaded.</paramref>
        /// <paramref name="onError">Called when form couldn't load.
        /// Provides <see cref="FormError"/> as argument for details about the error.</paramref>
        /// </summary>
        public void Load(Action onFormLoaded, Action<FormError> onError)
        {
            if (_prefabForm == null)
            {
                _prefabForm = Resources.Load("Ump/ConsentForm") as GameObject;
            }
            if (_prefabForm == null)
            {
                Debug.Log("No Prefab found for form.");
                onError(new FormError(ErrorCode, ErrorMessage));
            }
            else
            {
                Debug.Log("Consent Form Loaded.");
                onFormLoaded();
            }
        }

        /// <summary>
        /// Adds an instance of <see cref="GoogleMobileAds.Ump.Api.ConsentForm">ConsentForm</see>
        /// to the scene with click behaviour and pauses the game if a prefab exists.
        /// <paramref name="onDismissed">Called when the form is dismissed.</paramref>
        /// </summary>
        public void Show(Action<FormError> onDismissed)
        {
            if (_prefabForm != null)
            {
                _placeholderForm = _formBehaviour.ShowForm(_prefabForm, new Vector3(0, 0, 1));
                CreateButtonBehavior();
                AddClickBehavior(_placeholderForm, onDismissed);
                Debug.Log("Consent Form Shown.");
                _formBehaviour.PauseGame();
            }
            else
            {
                Debug.Log("Consent Form not loaded.");
                onDismissed(new FormError(ErrorCode, ErrorMessage));
            }
        }

        /// <summary>
        /// Load and show the consent form when the user consent is required but not yet obtained.
        /// <param name="onDismissed">The listener that gets called when the consent form is
        /// dismissed or fails to show.</param>
        /// </summary>
        public void LoadAndShowConsentFormIfRequired(Action<FormError> onDismissed)
        {
            if (ConsentInformationClient.Instance.CanRequestAds())
            {
                onDismissed(null);
                return;
            }
            Load(() => {}, (FormError error) => {});
            Show(onDismissed);
        }

        /// <summary>
        /// Show the privacy option form when the privacy options are required.
        /// <param name="onDismissed">The listener that gets called when the privacy options form is
        /// dismissed or fails to show.</param>
        /// </summary>
        public void ShowPrivacyOptionsForm(Action<FormError> onDismissed)
        {
            Show(onDismissed);
        }

        /// <summary>
        /// Adds <see cref="Button.ButtonClickedEvent"/> to the Consent button.
        /// <paramref name="placeholder">A placeholder form instance</paramref>
        /// <paramref name="onClick">Called when consent button is clicked
        /// (and the form is dismissed).</paramref>
        /// </summary>
        private void AddClickBehavior(GameObject placeholder, Action<FormError> onClick)
        {
            Image[] images = placeholder.GetComponentsInChildren<Image>();
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
        /// Adds <see cref="ButtonBehaviour"/> to the placeholder consent form.
        /// </summary>
        private void CreateButtonBehavior()
        {
            _buttonBehaviour = _placeholderForm.AddComponent<ButtonBehaviour>();
        }

        /// <summary>
        /// Removes the existing consent form from memory so that a fresh one can be loaded.
        /// </summary>
        private void DestroyConsentForm()
        {
            _formBehaviour.DestroyForm(_placeholderForm);
            _prefabForm = null;
        }
    }
}
