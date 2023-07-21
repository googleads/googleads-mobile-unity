using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GoogleMobileAds.Sample
{
    /// <summary>
    /// Demonstrates how to use Google Mobile Ads User Messaging Platform
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
            "emulator",
            "D3008DE61F37216C4657717D479A6463",
            // Add your test device IDs (replace with your own device IDs).
            #if UNITY_IPHONE
                "96e23e80653bb28980d3f40beb58915c",
            #elif UNITY_ANDROID
                "75EF8D155528C04DACBBA6F36F433035"
            #endif
        };

        [Header("User Interface")]
        public Text TextConsent;
        public Dropdown SelectChildUser;
        public Dropdown SelectDebugGeography;
        public Button BtnResetConsentInformation;
        public Button BtnUpdateConsentInformation;
        public Button BtnLoadConsentForm;
        public Button BtnShowConsentForm;

        private ConsentForm _consentForm;

        #region Unity API

        private void Awake()
        {
            // For dispatching events back onto the Unity main thread.
            MobileAds.RaiseAdEventsOnUnityMainThread = true;

            // On Android, Unity is paused when displaying interstitial or rewarded video.
            // This behavior should be made consistent with iOS.
            MobileAds.SetiOSAppPauseOnBackground(true);

            // This method will redraw the UI.
            // Call it whenever there is a change.
            UpdateUI();
        }

        #endregion


        #region Google Mobile Ads UMP API

        /// <summary>
        /// Clears all consent information from persistent storage.
        /// </summary>
        public void ResetConsentInformation()
        {
            ConsentInformation.Reset();
            Debug.Log("Consent information has been reset.");
            UpdateUI();
        }

        /// <summary>
        /// Updates the consent information.
        /// </summary>
        public void UpdateConsentInformation()
        {
            Debug.Log("Updating consent information.");

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
                    if (error == null)
                    {
                        // The consent information updated successfully.
                        Debug.Log(string.Format(
                                "Consent information updated to {0}. You may load the consent " +
                                "form.", ConsentInformation.ConsentStatus));
                    }
                    else
                    {
                        // The consent information failed to update.
                        Debug.LogError("Failed to update consent information with error: " +
                                error.Message);
                    }
                    UpdateUI();
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
            Debug.Log("Loading consent form.");

            ConsentForm.Load(
                // OnConsentFormLoad
                (ConsentForm form, FormError error) =>
                {
                    if (form != null)
                    {
                        // The consent form was loaded.
                        // We cache the consent form for showing later.
                        _consentForm = form;
                        Debug.Log("Consent form is loaded and is ready to show.");
                    }
                    else
                    {
                        // The consent form failed to load.
                        Debug.LogError("Failed to load consent form with error: " +
                            error == null ? "unknown error" : error.Message);
                    }
                    UpdateUI();
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

                         // Load another consent form for use later.
                         LoadConsentForm();
                     }
                     else
                     {
                         // The consent form failed to show.
                         Debug.LogError("Failed to show consent form with error: " +
                                           error.Message);
                     }
                     UpdateUI();
                 });
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// This method will update the UI elements with the newly loaded application state.
        /// </summary>
        private void UpdateUI()
        {
            BtnLoadConsentForm.interactable = _consentForm == null;
            BtnShowConsentForm.interactable = _consentForm != null;
            TextConsent.text = string.Format("Consent status is {0}",
                                                ConsentInformation.ConsentStatus);
        }

        #endregion
    }
}
