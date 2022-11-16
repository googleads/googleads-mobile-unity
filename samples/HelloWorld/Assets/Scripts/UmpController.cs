using System;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using GoogleMobileAds.Ump;
using GoogleMobileAds.Ump.Api;
using GoogleMobileAds.Ump.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UmpController : MonoBehaviour
{
    private float deltaTime;
    public bool showFpsMeter = true;
    public Text fpsMeter;
    public Text statusText;
    private int initStartTime;
    private ConsentDebugSettings debugSettings;
    private ConsentRequestParameters request;
    private bool initComplete = false;
    private bool isChildUser = true;
    private DebugGeography debugGeography =
            DebugGeography.DEBUG_GEOGRAPHY_DISABLED;
    private ConsentForm consentForm;
    private Button btnResetConsentInfo;
    private Button btnUpdateConsentInfo;
    private Button btnBuildConsentForm;
    private Button btnShowConsentForm;
    private InterstitialAd interstitialAd;
    private Button btnLoadAd;
    private Button btnShowAd;
    public UnityEvent OnAdLoadedEvent;
    public UnityEvent OnAdFailedToLoadEvent;
    public UnityEvent OnAdOpeningEvent;
    public UnityEvent OnAdFailedToShowEvent;
    public UnityEvent OnUserEarnedRewardEvent;
    public UnityEvent OnAdClosedEvent;
    [SerializeField] private TMPro.TMP_Dropdown selChildUser;
    [SerializeField] private TMPro.TextMeshProUGUI txtConsentStatus;


    #region UNITY MONOBEHAVIOR METHODS

    void Start()
    {
        btnResetConsentInfo = GameObject.Find("BtnResetConsentInfo").GetComponent<Button>();
        btnUpdateConsentInfo = GameObject.Find("BtnUpdateConsentInfo").GetComponent<Button>();
        btnBuildConsentForm = GameObject.Find("BtnBuildConsentForm").GetComponent<Button>();
        btnShowConsentForm = GameObject.Find("BtnShowConsentForm").GetComponent<Button>();
        btnLoadAd = GameObject.Find("BtnLoadAd").GetComponent<Button>();
        btnShowAd = GameObject.Find("BtnShowAd").GetComponent<Button>();
        selChildUser = GameObject.Find("SelChildUser").GetComponent<TMPro.TMP_Dropdown>();
        txtConsentStatus =
                GameObject.Find("TxtConsentStatus").GetComponent<TMPro.TextMeshProUGUI>();
        ResetButtons();

        List<String> deviceIds = new List<String>() { AdRequest.TestDeviceSimulator };

        // Add some test device IDs (replace with your own device IDs).
        #if UNITY_IPHONE
            deviceIds.Add("96e23e80653bb28980d3f40beb58915c");
        #elif UNITY_ANDROID
            deviceIds.Add("75EF8D155528C04DACBBA6F36F433035");
        #endif

        // Configure TagForChildDirectedTreatment and test device IDs.
        RequestConfiguration requestConfiguration =
            new RequestConfiguration.Builder()
            .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
            .SetTestDeviceIds(deviceIds).build();
        MobileAds.SetRequestConfiguration(requestConfiguration);

        // Initialize the Google Mobile Ads SDK.
        PrintStatus("Initializing Google Mobile Ads SDK.");
        initStartTime = DateTime.Now.Millisecond;
        MobileAds.Initialize(HandleInitCompleteAction);

        MobileAds.SetiOSAppPauseOnBackground(true);

        // Listen to application foreground / background events.
        AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
    }

    private void Update()
    {
        if (showFpsMeter)
        {
            fpsMeter.gameObject.SetActive(true);
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            fpsMeter.text = string.Format("{0:0.} fps", fps);
        }
        else
        {
            fpsMeter.gameObject.SetActive(false);
        }
    }

    #endregion

    #region UMP WORKFLOW

    public void ResetUMP()
    {
        ConsentInformation.ResetInfo();
        ResetButtons();
        PrintStatus("Consent Information Reset.");
    }

    public void UpdateConsentStatus()
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            btnUpdateConsentInfo.interactable = false;
            btnLoadAd.interactable = false;
            btnShowAd.interactable = false;
        });
        debugSettings = new ConsentDebugSettings(debugGeography, new List<string>
                {
                    "8FD4F63C7945A026C23086DF0FD24AA9"
                });

        // Set tag for underage of consent. Here false means users are not underage.
        request = new ConsentRequestParameters(isChildUser, debugSettings);
        ConsentInformation.RequestConsentInfoUpdate(request, OnConsentInfoSuccess, OnConsentInfoError);
    }

    public void RequestAndLoadInterstitialAd()
    {
        PrintStatus("Requesting Interstitial ad.");

        #if UNITY_EDITOR
            string adUnitId = "unused";
        #elif UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/1033173712";
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/4411468910";
        #else
            string adUnitId = "unexpected_platform";
        #endif

        // Clean up interstitial before using it
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }

        interstitialAd = new InterstitialAd(adUnitId);

        // Add Event Handlers
        interstitialAd.OnAdLoaded += (sender, args) =>
        {
            PrintStatus("Interstitial ad loaded.");
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                OnAdLoadedEvent.Invoke();
                btnShowAd.interactable = true;
            });
        };
        interstitialAd.OnAdFailedToLoad += (sender, args) =>
        {
            PrintStatus("Interstitial ad failed to load with error: " + args.LoadAdError.GetMessage());
            OnAdFailedToLoadEvent.Invoke();
        };
        interstitialAd.OnAdOpening += (sender, args) =>
        {
            PrintStatus("Interstitial ad opening.");
            OnAdOpeningEvent.Invoke();
        };
        interstitialAd.OnAdClosed += (sender, args) =>
        {
            PrintStatus("Interstitial ad closed.");
            OnAdClosedEvent.Invoke();
        };
        interstitialAd.OnAdDidRecordImpression += (sender, args) =>
        {
            PrintStatus("Interstitial ad recorded an impression.");
        };
        interstitialAd.OnAdFailedToShow += (sender, args) =>
        {
            PrintStatus("Interstitial ad failed to show with error: " + args.AdError.GetMessage());
        };
        interstitialAd.OnPaidEvent += (sender, args) =>
        {
            string msg = string.Format("{0} (currency: {1}, value: {2}",
                                        "Interstitial ad received a paid event.",
                                        args.AdValue.CurrencyCode,
                                        args.AdValue.Value);
            PrintStatus(msg);
        };

        // Load an interstitial ad
        interstitialAd.LoadAd(CreateAdRequest());
    }

    public void ShowInterstitialAd()
    {
        if (interstitialAd != null && interstitialAd.IsLoaded())
        {
            interstitialAd.Show();
        }
        else
        {
            PrintStatus("Interstitial ad is not ready yet.");
        }
    }

    #endregion

    #region HELPER METHODS

    void OnConsentInfoSuccess()
    {
        // The consent information state was updated.
        // Check if a form is available.
        if (ConsentInformation.IsConsentFormAvailable())
        {
            PrintStatus("Consent form available.");
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                btnBuildConsentForm.interactable = initComplete;
            });
        }
        else
        {
            PrintStatus("Consent form not available.");
        }
        UpdateStatusAndButtons();
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            btnUpdateConsentInfo.interactable = true;
        });
    }

    void OnConsentInfoError(FormError error)
    {
        if (error != null)
        {
            PrintStatus("Failed to request consent with error: " + error.Message);
        }
        else
        {
            PrintStatus("Consent request failed with unknown error.");
        }
        UpdateStatusAndButtons();
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            btnUpdateConsentInfo.interactable = true;
        });
    }

    public void LoadForm()
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            //Loads a consent form
            ConsentForm.LoadConsentForm(OnLoadFormSuccess, OnLoadFormError);
        });
    }

    void OnLoadFormSuccess(ConsentForm form)
    {
        // The consent form was loaded.
        // Consent form can be shown.
        consentForm = form;
        if (ConsentInformation.GetConsentStatus() == ConsentStatus.Required)
        {
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                btnShowConsentForm.interactable = true;
            });
            //ShowForm();
        }
        else
        {
            PrintStatus("Consent form loaded. Consent already given or not required.");
        }
        UpdateStatusAndButtons();
    }

    void OnLoadFormError(FormError loadError)
    {
        if (loadError != null)
        {
            PrintStatus("Failed to load Consent form: " + loadError.Message);
        }
        UpdateStatusAndButtons();
    }

    public void ShowForm()
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            consentForm.Show(OnShowFormDismiss);
        });
    }

    void OnShowFormDismiss(FormError showError)
    {
        if (showError != null)
        {
            PrintStatus("Error when Consent form dismissed: " + showError.Message);
            // Handle dismissal error by reloading the form.
            LoadForm();
        }
        else
        {
            PrintStatus("Consent form dismissed.");
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                btnLoadAd.interactable = true;
            });
        }
        UpdateStatusAndButtons();
    }

    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder()
            .AddKeyword("unity-admob-sample")
            .Build();
    }

    private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        PrintStatus("Initialization complete in " +
                    (float)(DateTime.Now.Millisecond - initStartTime) / 100 + "s.");
        initComplete = true;
        // Check the current consent information status.
        UpdateConsentStatus();
    }

    public void OnAppStateChanged(AppState state)
    {
        // An Ad can be loaded only when the app is in foreground.
        // The app state is not accurate the first time app is opened.
        Debug.Log("App State is " + state);
    }

    #endregion

    #region Utility

    ///<summary>
    /// Log the message and update the status text on the main thread.
    ///<summary>
    private void PrintStatus(string message)
    {
        Debug.Log(message);
        // Callbacks from GoogleMobileAds are not guaranteed to be called on
        // the main thread.
        // So, we use MobileAdsEventExecutor to schedule these calls on
        // the next Update() loop.
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            statusText.text = message;
        });
    }

    public void SelChildUserChangedHandler(int selection)
    {
        isChildUser = selection == 0;
        UpdateConsentStatus();
    }

    public void SelDebugGeoChangedHandler(int selection)
    {
        switch (selection)
        {
            case 0:
                debugGeography = DebugGeography.DEBUG_GEOGRAPHY_DISABLED;
                break;
            case 1:
                debugGeography = DebugGeography.DEBUG_GEOGRAPHY_EEA;
                break;
            case 2:
                debugGeography = DebugGeography.DEBUG_GEOGRAPHY_NOT_EEA;
                break;
            default:
                debugGeography = DebugGeography.DEBUG_GEOGRAPHY_DISABLED;
                break;
        }
        UpdateConsentStatus();
    }

    public void UpdateStatusAndButtons()
    {
        ConsentStatus consentStatus = ConsentInformation.GetConsentStatus();
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            txtConsentStatus.text = "" + consentStatus;
            btnResetConsentInfo.interactable = initComplete;
            btnUpdateConsentInfo.interactable = initComplete;
            btnLoadAd.interactable = consentStatus != ConsentStatus.Required;
            btnShowAd.interactable = interstitialAd != null && interstitialAd.IsLoaded();
        });
    }

    private void ResetButtons()
    {
        btnResetConsentInfo.interactable = initComplete;
        btnUpdateConsentInfo.interactable = initComplete;
        btnBuildConsentForm.interactable = false;
        btnShowConsentForm.interactable = false;
        btnLoadAd.interactable = false;
        btnShowAd.interactable = false;
        txtConsentStatus.text = "Status";
    }

    public void SwitchScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }

    #endregion
}
