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
        public bool CanRequestAds =>
            ConsentInformation.ConsentStatus == ConsentStatus.Obtained ||
            ConsentInformation.ConsentStatus == ConsentStatus.NotRequired;

        [SerializeField, Tooltip("Button to show user consent and privacy settings.")]
        private Button _privacyButton;

        [SerializeField, Tooltip("GameObject with the error popup.")]
        private GameObject _errorPopup;

        [SerializeField, Tooltip("Error message for the error popup,")]
        private Text _errorText;

        private ConsentForm _consentForm;

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
            ConsentInformation.Update(requestParameters, (FormError error) =>
            {
                UpdatePrivacyButton();

                if (error != null)
                {
                    onComplete(error.Message);
                    return;
                }

                Debug.Log("Consent information updated.");

                // Determine the consent-related action to take based on the ConsentStatus.
                if (CanRequestAds)
                {
                    // Consent has already been gathered or not required.
                    // Return control back to the user.
                    if (onComplete != null)
                    {
                        onComplete(null);
                    }

                    // Always load another consent form in the background so that
                    // privacy options form may show on user request.
                    LoadConsentForm(null);
                    return;
                }

                // Consent not obtained and is required.
                // Load the initial consent request form for the user.
                LoadConsentForm((string loadError) =>
                {
                    if (loadError != null)
                    {
                        if (onComplete != null)
                        {
                            onComplete(loadError);
                        }
                        return;
                    }

                    // Show the initial consent request form for the user.
                    ShowConsentForm((string showError) =>
                    {
                        if (onComplete != null)
                        {
                            onComplete(showError);
                        }
                    });
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

            LoadConsentForm((string loadError) =>
            {
                if (loadError != null)
                {
                    onComplete(loadError);
                    return;
                }

                ShowConsentForm(onComplete);
            });
        }

        private void ShowConsentForm(Action<string> onComplete)
        {
            // Error, no consent form is loaded.
            if (_consentForm == null)
            {
                var errorMessage = "No form available. Please try again later.";
                if (onComplete != null)
                {
                    onComplete(errorMessage);
                }
                else
                {
                    Debug.LogError(errorMessage);
                }

                // Always load another consent form in the background so that
                // privacy options form may show on user request.
                LoadConsentForm(null);
                return;
            }

            Debug.Log("Showing consent form.");

            // Show the loaded consent form.
            _consentForm.Show((FormError showError) =>
            {
                _consentForm = null;

                if (onComplete != null)
                {
                    var showErrorMessage = showError == null ? "Error showing consent form." 
                                                             : showError.Message;
                    onComplete(showErrorMessage);
                }

                // Always load another consent form in the background so that
                // privacy options form may show on user request.
                LoadConsentForm(null);
            });
        }

        private void LoadConsentForm(Action<string> onComplete)
        {
            // The consent form is already loaded and reeady.
            if (_consentForm != null)
            {
                if (onComplete != null)
                {
                    onComplete(null);
                }
                return;
            }

            Debug.Log("Loading consent form.");

            // The consent form is not loaded. Loading a consent form.
            ConsentForm.Load((ConsentForm form, FormError loadError) =>
            {
                if (loadError != null)
                {
                    if (onComplete != null)
                    {
                        onComplete(loadError.Message);
                    }
                    return;
                }

                if (form == null)
                {
                    if (onComplete != null)
                    {
                        onComplete("Error loading consent form.");
                    }
                    return;
                }

                _consentForm = form;
                UpdatePrivacyButton();
                Debug.Log("Consent form loaded.");
            });

        }

        void UpdatePrivacyButton()
        {
            // Enable our privacy settings button.
            if (_privacyButton != null)
            {
                _privacyButton.interactable = ConsentInformation.IsConsentFormAvailable();
            }
        }

        void UpdateErrorPopup(string message)
        {
            if (string.IsNullOrEmpty(message) || _errorPopup == null)
            {
                return;
            }

            if (_errorText != null)
            {
                _errorText.text = message;
            }

            _errorPopup.SetActive(true);
        }
    }
}
