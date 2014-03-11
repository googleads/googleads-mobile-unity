using System;
using UnityEngine;
using System.Runtime.InteropServices;

// The Google Mobile Ads script used to call into the native Google Mobile Ads Plugin library.
public class GoogleMobileAdsPlugin : MonoBehaviour {

#region Preamble
    // Defines string values for supported ad sizes.
    public class AdSize
    {
        private string adSize;
        private AdSize(string value)
        {
            this.adSize = value;
        }

        public override string ToString()
        {
            return adSize;
        }

        public static AdSize Banner = new AdSize("BANNER");
        public static AdSize MediumRectangle = new AdSize("IAB_MRECT");
        public static AdSize IABBanner = new AdSize("IAB_BANNER");
        public static AdSize Leaderboard = new AdSize("IAB_LEADERBOARD");
        public static AdSize SmartBanner = new AdSize("SMART_BANNER");
    }

    // These are the ad callback events that can be hooked into.
    public static event Action ReceivedAd = delegate {};
    public static event Action<string> FailedToReceiveAd = delegate {};
    public static event Action ShowingOverlay = delegate {};
	public static event Action DismissingOverlay = delegate {};	// iOS only
    public static event Action DismissedOverlay = delegate {};
    public static event Action LeavingApplication = delegate {};

    void Awake()
    {
		// Call plugin only when running on real device.
		if (Application.isEditor)
		{
			return;
		}
		gameObject.name = this.GetType().ToString();
		SetCallbackHandlerName(gameObject.name);
		DontDestroyOnLoad(this);
    }
#endregion

#region Public Interface
	// Create a GADBannerView and adds it into the view hierarchy.
	public static void CreateBannerView(string publisherId, AdSize adSize, bool positionAtTop)
	{
		// Call plugin only when running on real device.
		if (Application.isEditor)
		{
			return;
		}
		_CreateBannerView(publisherId, adSize.ToString(), positionAtTop);
	}
	
	// Request a new ad for the GADBannerView with optional extras as a JSON string.
	public static void RequestBannerAd(bool isTesting, string extras=null)
	{
		// Call plugin only when running on real device.
		if (Application.isEditor)
		{
			return;
		}
		if (string.IsNullOrEmpty(extras))
		{
			_InternalRequestBannerAd(isTesting);
		}
		else
		{
			_InternalRequestBannerAd(isTesting, extras);
		}
	}
	
	// Set the name of the callback handler so the right component gets ad callbacks.
	public static void SetCallbackHandlerName(string callbackHandlerName)
	{
		// Call plugin only when running on real device.
		if (Application.isEditor)
		{
			return;
		}
		_SetCallbackHandlerName(callbackHandlerName);
	}
	
	// Hide the GADBannerView from the screen.
	public static void HideBannerView()
	{
		// Call plugin only when running on real device.
		if (Application.isEditor)
		{
			return;
		}
		_HideBannerView();
	}
	
	// Show the GADBannerView on the screen if it's hidden.
	public static void ShowBannerView()
	{
		// Call plugin only when running on real device.
		if (Application.isEditor)
		{
			return;
		}
		_ShowBannerView();
	}
#endregion

#region Event Handlers
	public void OnReceiveAd(string unusedMessage)
	{
		ReceivedAd();
	}
	
	public void OnFailedToReceiveAd(string message)
	{
		FailedToReceiveAd(message);
	}
	
	public void OnPresentScreen(string unusedMessage)
	{
		ShowingOverlay();
	}

	public void OnDimissingScreen(string unusedMessage)
	{
		DismissingOverlay();
	}
	
	public void OnDismissScreen(string unusedMessage)
	{
		DismissedOverlay();
	}
	
	public void OnLeaveApplication(string unusedMessage)
	{
		LeavingApplication();
	}
#endregion
	
#region Platform-specific Code
#if UNITY_ANDROID
	// The plugin's class name.
	private const string PluginClassName = "com.google.unity.GoogleMobileAdsPlugin";

    // Create a banner view and add it into the view hierarchy.
	private static void _CreateBannerView(string publisherId, string adSize, bool positionAtTop)
    {
        AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaClass pluginClass = new AndroidJavaClass(PluginClassName);
        pluginClass.CallStatic("createBannerView",
            new object[4] { activity, publisherId, adSize, positionAtTop });
    }

    // Request a new ad for the banner view without any extras.
	private static void _InternalRequestBannerAd(bool isTesting)
    {
		AndroidJavaClass pluginClass = new AndroidJavaClass(PluginClassName);
        pluginClass.CallStatic("requestBannerAd", new object[1] {isTesting});
    }

    // Request a new ad for the banner view with extras.
	private static void _InternalRequestBannerAd(bool isTesting, string extras)
    {
        AndroidJavaClass pluginClass = new AndroidJavaClass(PluginClassName);
        pluginClass.CallStatic("requestBannerAd", new object[2] {isTesting, extras});
    }

    // Set the name of the callback handler so the right component gets ad callbacks.
	private static void _SetCallbackHandlerName(string callbackHandlerName)
    {
        AndroidJavaClass pluginClass = new AndroidJavaClass(PluginClassName);
        pluginClass.CallStatic("setCallbackHandlerName", new object[1] {callbackHandlerName});
    }

    // Hide the banner view from the screen.
	private static void _HideBannerView()
    {
        AndroidJavaClass pluginClass = new AndroidJavaClass(PluginClassName);
        pluginClass.CallStatic("hideBannerView");
    }

    // Show the banner view on the screen.
	private static void _ShowBannerView() {
        AndroidJavaClass pluginClass = new AndroidJavaClass(PluginClassName);
        pluginClass.CallStatic("showBannerView");
    }
#elif UNITY_IOS
	// These are the interface to native implementation calls for iOS.
	[DllImport("__Internal")]
	private static extern void _CreateBannerView(string publisherId,
	                                             string adSize,
	                                             bool positionAtTop);

	[DllImport("__Internal")]
	private static extern void _RequestBannerAd(bool isTesting, string extras);
	
	[DllImport("__Internal")]
	private static extern void _SetCallbackHandlerName(string handlerName);
	
	[DllImport("__Internal")]
	private static extern void _HideBannerView();
	
	[DllImport("__Internal")]
	private static extern void _ShowBannerView();

	private static void _InternalRequestBannerAd(bool isTesting)
	{
		_RequestBannerAd(isTesting, null);
	}
	private static void _InternalRequestBannerAd(bool isTesting, string extras)
	{
		_RequestBannerAd(isTesting, extras);
	}
	#else
	private static void _CreateBannerView(string publisherID, string adSize, bool positionAtTop)
	{
		if (Application.isEditor == false)
		{
			Debug.LogError("GoogleMobileAdsPlugin is not currently supported on this platform.");
		}
	}
	private static void _RequestBannerAd(bool isTesting)
	{
		if (Application.isEditor == false)
		{
			Debug.LogError("GoogleMobileAdsPlugin is not currently supported on this platform.");
		}
	}
	private static void _RequestBannerAd(bool isTesting, string extras)
	{
		if (Application.isEditor == false)
		{
			Debug.LogError("GoogleMobileAdsPlugin is not currently supported on this platform.");
		}
	}
	private static void _SetCallbackHandlerName(string handlerName)
	{
		if (Application.isEditor == false)
		{
			Debug.LogError("GoogleMobileAdsPlugin is not currently supported on this platform.");
		}
	}
	private static void _HideBannerView()
	{
		if (Application.isEditor == false)
		{
			Debug.LogError("GoogleMobileAdsPlugin is not currently supported on this platform.");
		}
	}
	private static void _ShowBannerView()
	{
		if (Application.isEditor == false)
		{
			Debug.LogError("GoogleMobileAdsPlugin is not currently supported on this platform.");
		}
	}
	private static void _InternalRequestBannerAd(bool isTesting)
	{
		_RequestBannerAd(isTesting, null);
	}
	private static void _InternalRequestBannerAd(bool isTesting, string extras)
	{
		_RequestBannerAd(isTesting, extras);
	}
#endif
#endregion
}

