using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using GoogleMobileAds.Ump.Api;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GoogleMobileAds.Sample
{
    /// <summary>
    /// Demonstrates how to use the Google Mobile Ads User Messaging Platform
    /// to manage user consent and privacy settings.
    /// </summary>
    public class GoogleUmpController : MonoBehaviour
    {
        /// <summary>
        /// The test device id list.
        /// </summary>
        /// <remarks>
        /// If you click on too many ads without being in test mode,
        /// you risk your account being flagged for invalid activity.
        /// https://developers.google.com/admob/unity/test-ads
        /// </remarks>
        private readonly List<string> TEST_DEVICE_IDS = new List<string>
        {
            AdRequest.TestDeviceSimulator,
            // Add your test device IDs (replace with your own device IDs).
            #if UNITY_IPHONE
                "96e23e80653bb28980d3f40beb58915c",
            #elif UNITY_ANDROID
                "75EF8D155528C04DACBBA6F36F433035"
            #endif
        };

        [Header("User Interface")]
        public Text TextConsent;
        public Text TextStatus;
        public Dropdown SelectChildUser;
        public Dropdown SelectDebugGeography;
        public Button BtnResetConsentInformation;
        public Button BtnUpdateConsentInformation;
        public Button BtnLoadConsentForm;
        public Button BtnShowConsentForm;
        public Button BtnLoadMainScene;

        private ConsentForm _consentForm;

        #region Unity API

        private void Awake()
        {
            // On Android, Unity is paused when displaying interstitial or rewarded video.
            // This behavior should be made consistent with iOS.
            MobileAds.SetiOSAppPauseOnBackground(true);

            // This method will configure the UI events.
            InitUI();

            // This method will redraw the UI.
            // Call it whenever there is a change.
            UpdateUI();

            UpdateStatus("Google Mobile Ads is initializing.");

            MobileAds.Initialize((InitializationStatus status) =>
            {
                UpdateStatus("Google Mobile Ads is ready.");
            });
        }

        #endregion


        #region Google Mobile Ads UMP API

        /// <summary>
        /// Clears all consent information from persistent storage.
        /// </summary>
        public void ResetConsentInformation()
        {
            ConsentInformation.Reset();
            UpdateUI();
            UpdateStatus("Consent information has been reset.");
        }

        /// <summary>
        /// Updates the consent information.
        /// </summary>
        public void UpdateConsentInformation()
        {
            UpdateStatus("Updating consent information.");

            var debugGeography = (DebugGeography)SelectDebugGeography.value;
            var tagForUnderAgeOfConsent = SelectChildUser.value == 1;

            // Confugre the ConsentDebugSettings.
            // The ConsentDebugSettings is serializable so you may expose this to your monobehavior.
            var consentDebugSettings = new ConsentDebugSettings();
            consentDebugSettings.DebugGeography = debugGeography;
            consentDebugSettings.TestDeviceHashedIds = TEST_DEVICE_IDS;

            // Set tag for under age of consent. Here false means users are not under age.
            var consentRequestParameters = new ConsentRequestParameters();
            consentRequestParameters.ConsentDebugSettings = consentDebugSettings;
            consentRequestParameters.TagForUnderAgeOfConsent = tagForUnderAgeOfConsent;

            ConsentInformation.Update(consentRequestParameters,
                // OnConsentInformationUpdate
                (FormError error) =>
                {
                    UpdateUI();

                    if (error == null)
                    {
                        // The consent information updated successfully.
                        UpdateStatus(string.Format(
                                "Consent information updated to {0}. You may load the consent " +
                                "form.", ConsentInformation.ConsentStatus));
                    }
                    else
                    {
                        // The consent information failed to update.
                        UpdateErrorStatus("Failed to update consent information with error: " +
                                error.Message);
                    }

                });
        }

        /// <summary>
        /// Loads a consent form.
        /// </summary>
        /// <remarks>
        /// This should be done before it is needed
        /// so that you can show the consent form without delay when needed.
        /// </remarks>
        public void LoadConsentForm()
        {
            UpdateStatus("Loading consent form.");

            ConsentForm.Load(
                // OnConsentFormLoad
                (ConsentForm form, FormError error) =>
                {
                    UpdateUI();
                    if (form != null)
                    {
                        // The consent form was loaded.
                        // We cache the consent form for showing later.
                        _consentForm = form;
                        UpdateStatus("Consent form is loaded and is ready to show.");
                    }
                    else
                    {
                        // The consent form failed to load.
                        UpdateErrorStatus("Failed to load consent form with error: " +
                            error == null ? "unknown error" : error.Message);
                    }
                });
        }

        /// <summary>
        /// Shows the consent form. The consent form must be loaded first.
        /// </summary>
        public void ShowConsentForm()
        {
            _consentForm.Show(
                 // OnConsentFormShow
                 (FormError error) =>
                 {
                     if (error == null)
                     {
                         // If the error parameter is null,
                         // we showed the consent form without error.
                         UpdateUI();

                         // Load another consent form for use later.
                         LoadConsentForm();
                     }
                     else
                     {
                         // The consent form failed to show.
                         UpdateUI();
                         UpdateErrorStatus("Failed to show consent form with error: " +
                                           error.Message);
                     }
                 });
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// Returns to the main Google mobile ads sample scene.
        /// </summary>
        public void LoadMainScene()
        {
            SceneManager.LoadScene("MainScene");
        }

        /// <summary>
        /// This method will initialize the UI elements.
        /// </summary>
        private void InitUI()
        {
            BtnResetConsentInformation.onClick.AddListener(ResetConsentInformation);
            BtnUpdateConsentInformation.onClick.AddListener(UpdateConsentInformation);
            BtnLoadConsentForm.onClick.AddListener(LoadConsentForm);
            BtnShowConsentForm.onClick.AddListener(ShowConsentForm);
            BtnLoadMainScene.onClick.AddListener(LoadMainScene);
        }

        /// <summary>
        /// This method will update the UI elements with the newly loaded application state.
        /// </summary>
        private void UpdateUI()
        {
            // Google Mobile Ads is not thread safe.
            // Use the MobileAdsEventExecutor to run this code on the Main Unity Thread.
            // This will prevent errors related to mutating the Unity UI from a background thread.
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                BtnLoadConsentForm.interactable = _consentForm == null;
                BtnShowConsentForm.interactable = _consentForm != null;
                TextConsent.text = string.Format("Consent status is {0}",
                                                 ConsentInformation.ConsentStatus);
            });
        }

        private void UpdateStatus(string message)
        {
            Debug.Log(message);
            // Google Mobile Ads is not thread safe. There is a chance that events being raised
            // are not on the main Unity thread. Please dispatch to the Unity main thread.
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                TextStatus.text = message;
            });
        }

        private void UpdateErrorStatus(string message)
        {
            Debug.LogError(message);
            // Google Mobile Ads is not thread safe. There is a chance that events being raised
            // are not on the main Unity thread. Please dispatch to the Unity main thread.
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                TextStatus.text = message;
            });
        }

        #endregion
    }
}
