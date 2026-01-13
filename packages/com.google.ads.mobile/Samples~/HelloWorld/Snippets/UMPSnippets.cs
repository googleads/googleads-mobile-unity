using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using System.Collections.Generic;
using UnityEngine.UI;

namespace GoogleMobileAds.Snippets
{
    /// <summary>
    /// Code snippets used for the UMP developer guides.
    /// </summary>
    internal class UMPSnippets
    {
        // [START request_consent]
        void Start()
        {
            // Create a ConsentRequestParameters object.
            ConsentRequestParameters requestParameters = new ConsentRequestParameters();

            // Request an update of the user's consent information.
            ConsentInformation.Update(requestParameters, OnConsentInfoUpdated);
        }
        // [END request_consent]

        void OnConsentInfoUpdated(FormError consentError)
        {
            if (consentError != null)
            {
                // Handle the error. Can still check CanRequestAds() based on previous session.
                Debug.LogError("Error updating consent information: " + consentError.Message);
                AttemptAdInitialization();
                return;
            }

            // Consent information state was updated successfully.
            // [START load_and_present]
            ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
            {
                if (formError != null)
                {
                    // Consent gathering failed.
                    Debug.LogError("Error loading/showing consent form: " + formError.Message);
                }

                // [START_EXCLUDE]
                // Consent has been gathered or was not required.
                UpdatePrivacyButton();
                AttemptAdInitialization();
                // [END_EXCLUDE]
            });
            // [END load_and_present]
        }

        void AttemptAdInitialization()
        {
            // Use CanRequestAds() to check if consent has been obtained.
            if (ConsentInformation.CanRequestAds())
            {
                // Initialize the Mobile Ads SDK now that consent has been obtained.
                MobileAds.Initialize((InitializationStatus initstatus) =>
                {
                    Debug.Log("Mobile Ads SDK Initialized. Ready to request ads.");
                });
            }
            else
            {
                Debug.Log("Cannot request ads yet.");
            }
        }


        [SerializeField, Tooltip("Button to show the privacy options form.")]
        private Button _privacyButton;

        // Note: For this snippet to work, ensure the Button is assigned in the Inspector
        // and the Start() method links the button's onClick event:
        // void Start() {
        //     // ... other code
        //     if (_privacyButton != null) {
        //         _privacyButton.onClick.AddListener(ShowPrivacyOptionsForm);
        //         _privacyButton.interactable = false; // Disable until status is checked
        //     }
        //     // ...
        // }

        /// <summary>
        /// Updates the privacy buttons visual state based on the consent information.
        /// </summary>
        // [START is_privacy_options_required]
        void UpdatePrivacyButton()
        {
            if (_privacyButton != null)
            {
                // Enable the button only if a privacy options entry point is required.
                _privacyButton.interactable =
                    ConsentInformation.PrivacyOptionsRequirementStatus == PrivacyOptionsRequirementStatus.Required;
            }
        }
        // [END is_privacy_options_required]

        /// <summary>
        /// Shows the privacy options form to the user.
        /// This method is typically called when the user taps the privacy button.
        /// </summary>
        // [START show_privacy_options]
        public void ShowPrivacyOptionsForm()
        {
            ConsentForm.ShowPrivacyOptionsForm((FormError showError) =>
            {
                if (showError != null)
                {
                    Debug.LogError("Error showing privacy options form: " + showError.Message);
                }
            });
        }
        // [END show_privacy_options]
    }
}