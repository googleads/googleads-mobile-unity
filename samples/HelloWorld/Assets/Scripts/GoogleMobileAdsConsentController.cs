using GoogleMobileAds.Ump.Api;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GoogleMobileAds.Samples
{
    /// <summary>
    /// Helper class that implements consent using the Google User Messaging Platform (UMP) SDK.
    /// </summary>
    public class GoogleMobileAdsConsentController : MonoBehaviour
    {
        /// <summary>
        /// If true, it is safe to call MobileAds.Initialize() and load Ads.
        /// </summary>
        public bool CanRequestAds => ConsentInformation.CanRequestAds();

        [SerializeField, Tooltip("Button to show user consent and privacy settings.")]
        private Button _privacyButton;

        [SerializeField, Tooltip("GameObject with the error popup.")]
        private GameObject _errorPopup;

        [SerializeField, Tooltip("Error message for the error popup,")]
        private Text _errorText;

        private void Start()
        {
            // Disable the privacy settings button.
            if (_privacyButton != null)
            {
                _privacyButton.interactable = false;
            }
            // Disable the error popup,
            if (_errorPopup != null)
            {
                _errorPopup.SetActive(false);
            }
        }

        /// <summary>
        /// Startup method for the Google User Messaging Platform (UMP) SDK
        /// which will run all startup logic including loading any required
        /// updates and displaying any required forms.
        /// </summary>
        public void GatherConsent(Action<string> onComplete)
        {
            Debug.Log("Gathering consent.");

            var requestParameters = new ConsentRequestParameters
            {
                // False means users are not under age.
                TagForUnderAgeOfConsent = false,
                ConsentDebugSettings = new ConsentDebugSettings
                {
                    // For debugging consent settings by geography.
                    DebugGeography = DebugGeography.Disabled,
                    // https://developers.google.com/admob/unity/test-ads
                    TestDeviceHashedIds = GoogleMobileAdsController.TestDeviceIds,
                }
            };

            // Combine the callback with an error popup handler.
            onComplete = (onComplete == null)
                ? UpdateErrorPopup
                : onComplete + UpdateErrorPopup;

            // The Google Mobile Ads SDK provides the User Messaging Platform (Google's
            // IAB Certified consent management platform) as one solution to capture
            // consent for users in GDPR impacted countries. This is an example and
            // you can choose another consent management platform to capture consent.
            ConsentInformation.Update(requestParameters, (FormError updateError) =>
            {
                // Enable the change privacy settings button.
                UpdatePrivacyButton();

                if (updateError != null)
                {
                    onComplete(updateError.Message);
                    return;
                }

                // Determine the consent-related action to take based on the ConsentStatus.
                if (CanRequestAds)
                {
                    // Consent has already been gathered or not required.
                    // Return control back to the user.
                    onComplete(null);
                    return;
                }

                // Consent not obtained and is required.
                // Load the initial consent request form for the user.
                ConsentForm.LoadAndShowConsentFormIfRequired((FormError showError) =>
                {
                    UpdatePrivacyButton();
                    if (showError != null)
                    {
                        // Form showing failed.
                        if (onComplete != null)
                        {
                            onComplete(showError.Message);
                        }
                    }
                    // Form showing succeeded.
                    else if (onComplete != null)
                    {
                        onComplete(null);
                    }
                });
            });
        }

        /// <summary>
        /// Shows the privacy options form to the user.
        /// </summary>
        /// <remarks>
        /// Your app needs to allow the user to change their consent status at any time.
        /// Load another form and store it to allow the user to change their consent status
        /// </remarks>
        public void ShowPrivacyOptionsForm(Action<string> onComplete)
        {
            Debug.Log("Showing privacy options form.");

            // combine the callback with an error popup handler.
            onComplete = (onComplete == null)
                ? UpdateErrorPopup
                : onComplete + UpdateErrorPopup;

            ConsentForm.ShowPrivacyOptionsForm((FormError showError) =>
            {
                UpdatePrivacyButton();
                if (showError != null)
                {
                    // Form showing failed.
                    if (onComplete != null)
                    {
                        onComplete(showError.Message);
                    }
                }
                // Form showing succeeded.
                else if (onComplete != null)
                {
                    onComplete(null);
                }
            });
        }

        /// <summary>
        /// Reset ConsentInformation for the user.
        /// </summary>
        public void ResetConsentInformation()
        {
            ConsentInformation.Reset();
            UpdatePrivacyButton();
        }

        void UpdatePrivacyButton()
        {
            if (_privacyButton != null)
            {
                _privacyButton.interactable =
                    ConsentInformation.PrivacyOptionsRequirementStatus ==
                        PrivacyOptionsRequirementStatus.Required;
            }
        }

        void UpdateErrorPopup(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if (_errorText != null)
            {
                _errorText.text = message;
            }

            if (_errorPopup != null)
            {
                _errorPopup.SetActive(true);
            }
        }
    }
}