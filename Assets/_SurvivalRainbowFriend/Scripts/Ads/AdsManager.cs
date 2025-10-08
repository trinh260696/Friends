using com.adjust.sdk;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;
    public string MaxSdkKey = "ENTER_MAX_SDK_KEY_HERE";
    public string InterstitialAdUnitId = "ENTER_INTERSTITIAL_AD_UNIT_ID_HERE";
    public string RewardedAdUnitId = "ENTER_REWARD_AD_UNIT_ID_HERE";
    public string RewardedInterstitialAdUnitId = "ENTER_REWARD_INTER_AD_UNIT_ID_HERE";
    public string BannerAdUnitId = "ENTER_BANNER_AD_UNIT_ID_HERE";
    public string MRecAdUnitId = "ENTER_MREC_AD_UNIT_ID_HERE";
    public string AppOpenAdUnitId = "ENTER_OPEN_AD_UNIT_ID_HERE";



    private bool isBannerShowing;
    private bool isMRecShowing;
    public UnityAction RewardAdsAction;
    private int interstitialRetryAttempt;
    private int rewardedRetryAttempt;
    private int rewardedInterstitialRetryAttempt;
    private void Awake()
    {
        Instance = this;
    }
    private static float DeviceDiagonalSizeInInches()
    {
        float screenWidth = Screen.width / Screen.dpi;
        float screenHeight = Screen.height / Screen.dpi;
        float diagonalInches = Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));

        return diagonalInches;
    }
    void Start()
    {
        

        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
        {
            // AppLovin SDK is initialized, configure and start loading ads.
            Debug.Log("MAX SDK Initialized");
            InitializeOpenAds();
            InitializeInterstitialAds();
            InitializeRewardedAds();
            InitializeRewardedInterstitialAds();
            InitializeBannerAds();
            InitializeMRecAds();
            MaxSdk.LoadAppOpenAd(AppOpenAdUnitId);
            // Initialize Adjust SDK

        };

        MaxSdk.SetSdkKey(MaxSdkKey);
        MaxSdk.InitializeSdk();
        float aspectRatio = Mathf.Max(Screen.height, Screen.width) / Mathf.Min(Screen.height, Screen.width);
        bool isTablet = (DeviceDiagonalSizeInInches() > 6.5f && aspectRatio < 2f);

        if (isTablet)
        {
            StaticData.HEIGHT_BANNER = 180;
        }
        else
        {
            StaticData.HEIGHT_BANNER = 100;
        }               
    }
    #region AppOpen Ads Methods
    void InitializeOpenAds()
    {
        MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += OnAppOpenAdLoadedEvent;
        MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += OnAppOpenAdFailedEvent;
        MaxSdkCallbacks.AppOpen.OnAdDisplayFailedEvent += OnAppOpenAdFailedToDisplayEvent;
        MaxSdkCallbacks.AppOpen.OnAdDisplayedEvent += OnAppOpenAdDisplayedEvent;
        MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += OnAppOpenRevenuePaidEvent;
    }
    public void ShowOpenADS()
    {
        if (UserData.Instance.GameData.vip > 0) return;
        MaxSdk.ShowAppOpenAd(AppOpenAdUnitId);
    }
    private void OnAppOpenAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'

        Debug.Log("OpenAds loaded");
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("ad_type", "open ads");
        event_params.Add("ad_unit_id", adUnitId);
        event_params.Add("result", "success");
        event_params.Add("reason", "OK");
        VkAdjustTracker.AdjustTrack(AdjustCode.ADS_LOAD, event_params);     
    }
    private void OnAppOpenAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        Debug.Log("OpenAds load failed");
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("ad_type", "open ads");
        event_params.Add("ad_unit_id", adUnitId);
        event_params.Add("result", "fail");
        event_params.Add("reason", errorInfo.Code);
        VkAdjustTracker.AdjustTrack(AdjustCode.ADS_LOAD, event_params);
    }
    private void OnAppOpenAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.LogWarning("open ad displayed");
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("ad_type", "open ads");
        event_params.Add("ad_unit_id", adUnitId);
        event_params.Add("result", "success");
        event_params.Add("reason", "OK");
        VkAdjustTracker.AdjustTrack(AdjustCode.ADS_SHOW, event_params);
    }
    private void OnAppOpenAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
       
        Debug.Log("openads failed to display with error code: " + errorInfo.Code);
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("ad_type", "open ads");
        event_params.Add("ad_unit_id", adUnitId);
        event_params.Add("result", "fail");
        event_params.Add("reason", errorInfo.Code);
        VkAdjustTracker.AdjustTrack(AdjustCode.ADS_SHOW, event_params);
    }
    void OnAppOpenRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad revenue paid. Use this callback to track user revenue.
        Debug.Log("open ad revenue paid");

        // Ad revenue
        double revenue = adInfo.Revenue;
        StaticData.revenue += revenue;
        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to
        TrackAdRevenueFirebase(adInfo);
        TrackAdRevenueAdjust(adInfo);
    }
    #endregion
    #region Interstitial Ad Methods

    private void InitializeInterstitialAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
       
        // Load the first interstitial
        LoadInterstitial();
    }

    void LoadInterstitial()
    {       
        MaxSdk.LoadInterstitial(InterstitialAdUnitId);
    }
  
    public void ShowInterstitial()
    {
        if (UserData.Instance.GameData.vip > 0) return;
        if (MaxSdk.IsInterstitialReady(InterstitialAdUnitId))
        {
           
            MaxSdk.ShowInterstitial(InterstitialAdUnitId);
        }
        else
        {
           
        }
    }
    
    
    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'
      
        Debug.Log("Interstitial loaded");
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("ad_type", "inter ads");
        event_params.Add("ad_unit_id", adUnitId);
        event_params.Add("result", "success");
        event_params.Add("reason", "OK");
        VkAdjustTracker.AdjustTrack(AdjustCode.ADS_LOAD, event_params);
        // Reset retry attempt
        interstitialRetryAttempt = 0;
    }

    private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        interstitialRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("ad_type", "inter ads");
        event_params.Add("ad_unit_id", adUnitId);
        event_params.Add("result", "fail");
        event_params.Add("reason", errorInfo.Code);
        VkAdjustTracker.AdjustTrack(AdjustCode.ADS_LOAD, event_params);
        Debug.Log("Interstitial failed to load with error code: " + errorInfo.Code);      
        Invoke("LoadInterstitial", (float)retryDelay);
    }
    private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. We recommend loading the next ad
        Debug.Log("Interstitial failed to display with error code: " + errorInfo.Code);
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("ad_type", "inter ads");
        event_params.Add("ad_unit_id", adUnitId);
        event_params.Add("result", "fail");
        event_params.Add("reason", errorInfo.Code);
        VkAdjustTracker.AdjustTrack(AdjustCode.ADS_SHOW, event_params);
        LoadInterstitial();
    }
    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        Debug.LogWarning("inter ad displayed");
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("ad_type", "inter ads");
        event_params.Add("ad_unit_id", adUnitId);
        event_params.Add("result", "success");
        event_params.Add("reason", "OK");
        VkAdjustTracker.AdjustTrack(AdjustCode.ADS_SHOW, event_params);
    }

    private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad
        Debug.Log("Interstitial dismissed");
        LoadInterstitial();
    }

    private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad revenue paid. Use this callback to track user revenue.
        Debug.Log("Interstitial revenue paid");

        // Ad revenue
        double revenue = adInfo.Revenue;
        StaticData.revenue += revenue;
        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to
        TrackAdRevenueFirebase(adInfo);
        TrackAdRevenueAdjust(adInfo);
    }

    #endregion

    #region Rewarded Ad Methods

    private void InitializeRewardedAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;

        // Load the first RewardedAd
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        
        MaxSdk.LoadRewardedAd(RewardedAdUnitId);
    }

    public void ShowRewardedAd(UnityAction action)
    {
        if (MaxSdk.IsRewardedAdReady(RewardedAdUnitId))
        {
            this.RewardAdsAction = action;
            MaxSdk.ShowRewardedAd(RewardedAdUnitId);
        }
        else
        {
            UIPopup.OpenPopup("Error", "Video is not available!", false);
        }
    }
    public bool CheckAdsReady()
    {
        return MaxSdk.IsRewardedAdReady(RewardedAdUnitId);
    }
    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'
      
        Debug.LogWarning("Rewarded ad loaded");
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("ad_type", "reward ads");
        event_params.Add("ad_unit_id", adUnitId);
        event_params.Add("result", "success");
        event_params.Add("reason", "OK");
        VkAdjustTracker.AdjustTrack(AdjustCode.ADS_LOAD, event_params);
        // Reset retry attempt
        rewardedRetryAttempt = 0;
       
    }

    private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("ad_type", "reward ads");
        event_params.Add("ad_unit_id", adUnitId);
        event_params.Add("result", "fail");
        event_params.Add("reason", errorInfo.Code);
        VkAdjustTracker.AdjustTrack(AdjustCode.ADS_LOAD, event_params);
        rewardedRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, rewardedRetryAttempt));      
        Debug.LogWarning("Rewarded ad failed to load with error code: " + errorInfo.Code);      
        Invoke("LoadRewardedAd", (float)retryDelay);
        
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. We recommend loading the next ad
        Debug.LogWarning("Rewarded ad failed to display with error code: " + errorInfo.Code);
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("ad_type", "reward ads");
        event_params.Add("ad_unit_id", adUnitId);
        event_params.Add("result", "fail");
        event_params.Add("reason", errorInfo.Code);
        VkAdjustTracker.AdjustTrack(AdjustCode.ADS_SHOW, event_params);
        UIPopup.OpenPopup("Error", "Video is not available!", false);
        LoadRewardedAd();
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.LogWarning("Rewarded ad displayed");
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("ad_type", "reward ads");
        event_params.Add("ad_unit_id", adUnitId);
        event_params.Add("result", "success");
        event_params.Add("reason", "OK");
        VkAdjustTracker.AdjustTrack(AdjustCode.ADS_SHOW, event_params);
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.LogWarning("Rewarded ad clicked");
    }

    private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        Debug.LogWarning("Rewarded ad dismissed");
        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        if (RewardAdsAction != null)
        {
            RewardAdsAction();
            RewardAdsAction = null;
        }
        // Rewarded ad was displayed and user should receive the reward
        Debug.LogWarning("Rewarded ad received reward");
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad revenue paid. Use this callback to track user revenue.
        Debug.LogWarning("Rewarded ad revenue paid");

        // Ad revenue
        double revenue = adInfo.Revenue;
        StaticData.revenue += (double)adInfo.Revenue;

        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to
      
        TrackAdRevenueFirebase(adInfo);
        TrackAdRevenueAdjust(adInfo);
    }

    #endregion

    #region Rewarded Interstitial Ad Methods

    private void InitializeRewardedInterstitialAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.RewardedInterstitial.OnAdLoadedEvent += OnRewardedInterstitialAdLoadedEvent;
        MaxSdkCallbacks.RewardedInterstitial.OnAdLoadFailedEvent += OnRewardedInterstitialAdFailedEvent;
        MaxSdkCallbacks.RewardedInterstitial.OnAdDisplayFailedEvent += OnRewardedInterstitialAdFailedToDisplayEvent;
        MaxSdkCallbacks.RewardedInterstitial.OnAdDisplayedEvent += OnRewardedInterstitialAdDisplayedEvent;
        MaxSdkCallbacks.RewardedInterstitial.OnAdClickedEvent += OnRewardedInterstitialAdClickedEvent;
        MaxSdkCallbacks.RewardedInterstitial.OnAdHiddenEvent += OnRewardedInterstitialAdDismissedEvent;
        MaxSdkCallbacks.RewardedInterstitial.OnAdReceivedRewardEvent += OnRewardedInterstitialAdReceivedRewardEvent;
        MaxSdkCallbacks.RewardedInterstitial.OnAdRevenuePaidEvent += OnRewardedInterstitialAdRevenuePaidEvent;

        // Load the first RewardedInterstitialAd
        LoadRewardedInterstitialAd();
    }

    private void LoadRewardedInterstitialAd()
    {
        
        MaxSdk.LoadRewardedInterstitialAd(RewardedInterstitialAdUnitId);
    }

    private void ShowRewardedInterstitialAd()
    {
        if (MaxSdk.IsRewardedInterstitialAdReady(RewardedInterstitialAdUnitId))
        {
           
            MaxSdk.ShowRewardedInterstitialAd(RewardedInterstitialAdUnitId);
        }
        else
        {
           
        }
    }

    private void OnRewardedInterstitialAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded interstitial ad is ready to be shown. MaxSdk.IsRewardedInterstitialAdReady(rewardedInterstitialAdUnitId) will now return 'true'
       
        Debug.Log("Rewarded interstitial ad loaded");
        
        // Reset retry attempt
        rewardedInterstitialRetryAttempt = 0;
    }

    private void OnRewardedInterstitialAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        rewardedInterstitialRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, rewardedInterstitialRetryAttempt));

       
        Debug.Log("Rewarded interstitial ad failed to load with error code: " + errorInfo.Code);     
        Invoke("LoadRewardedInterstitialAd", (float)retryDelay);
    }

    private void OnRewardedInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded interstitial ad failed to display. We recommend loading the next ad
        Debug.Log("Rewarded interstitial ad failed to display with error code: " + errorInfo.Code);
        LoadRewardedInterstitialAd();
    }

    private void OnRewardedInterstitialAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Rewarded interstitial ad displayed");
    }

    private void OnRewardedInterstitialAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Rewarded interstitial ad clicked");
    }

    private void OnRewardedInterstitialAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded interstitial ad is hidden. Pre-load the next ad
        Debug.Log("Rewarded interstitial ad dismissed");
        LoadRewardedInterstitialAd();
    }

    private void OnRewardedInterstitialAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded interstitial ad was displayed and user should receive the reward
        Debug.Log("Rewarded interstitial ad received reward");
    }

    private void OnRewardedInterstitialAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded interstitial ad revenue paid. Use this callback to track user revenue.
        Debug.Log("Rewarded interstitial ad revenue paid");

        // Ad revenue
        StaticData.revenue += (double)adInfo.Revenue;
        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied t      
       
    }

    #endregion

    #region Banner Ad Methods

    private void InitializeBannerAds()
    {
        // Attach Callbacks
        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;

        // Banners are automatically sized to 320x50 on phones and 728x90 on tablets.
        // You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments.
        MaxSdk.CreateBanner(BannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

        // Set background or background color for banners to be fully functional.
        MaxSdk.SetBannerBackgroundColor(BannerAdUnitId, Color.clear);
    }

    public void ToggleBannerVisibility()
    {
        if (!isBannerShowing)
        {
            if (UserData.Instance.GameData.vip == 0)
                MaxSdk.ShowBanner(BannerAdUnitId);

        }
        else
        {
            MaxSdk.HideBanner(BannerAdUnitId);

        }

        isBannerShowing = !isBannerShowing;
    }

    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Banner ad is ready to be shown.
        // If you have already called MaxSdk.ShowBanner(BannerAdUnitId) it will automatically be shown on the next ad refresh.
        Debug.Log("Banner ad loaded");
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("ad_type", "banner ads");
        event_params.Add("ad_unit_id", adUnitId);
        event_params.Add("result", "success");
        event_params.Add("reason", "OK");
        VkAdjustTracker.AdjustTrack(AdjustCode.ADS_LOAD, event_params);
        VkAdjustTracker.AdjustTrack(AdjustCode.ADS_SHOW, event_params);
    }

    private void OnBannerAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Banner ad failed to load. MAX will automatically try loading a new ad internally.
        Debug.Log("Banner ad failed to load with error code: " + errorInfo.Code);
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("ad_type", "banner ads");
        event_params.Add("ad_unit_id", adUnitId);
        event_params.Add("result", "fail");
        event_params.Add("reason", errorInfo.Code);
        VkAdjustTracker.AdjustTrack(AdjustCode.ADS_LOAD, event_params);
        VkAdjustTracker.AdjustTrack(AdjustCode.ADS_SHOW, event_params);
    }

    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Banner ad clicked");
    }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Banner ad revenue paid. Use this callback to track user revenue.
        Debug.Log("Banner ad revenue paid");

        // Ad revenue
        double revenue = adInfo.Revenue;
        StaticData.revenue += (double)adInfo.Revenue;
        
        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to
        TrackAdRevenueFirebase(adInfo);
        TrackAdRevenueAdjust(adInfo);
    }

    #endregion

    #region MREC Ad Methods

    private void InitializeMRecAds()
    {
        // Attach Callbacks
        MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMRecAdLoadedEvent;
        MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMRecAdFailedEvent;
        MaxSdkCallbacks.MRec.OnAdClickedEvent += OnMRecAdClickedEvent;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnMRecAdRevenuePaidEvent;

        // MRECs are automatically sized to 300x250.
        MaxSdk.CreateMRec(MRecAdUnitId, MaxSdkBase.AdViewPosition.BottomCenter);
    }

    private void ToggleMRecVisibility()
    {
        if (!isMRecShowing)
        {
            MaxSdk.ShowMRec(MRecAdUnitId);
           
        }
        else
        {
            MaxSdk.HideMRec(MRecAdUnitId);
           
        }

        isMRecShowing = !isMRecShowing;
    }

    private void OnMRecAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // MRec ad is ready to be shown.
        // If you have already called MaxSdk.ShowMRec(MRecAdUnitId) it will automatically be shown on the next MRec refresh.
        Debug.Log("MRec ad loaded");
    }

    private void OnMRecAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // MRec ad failed to load. MAX will automatically try loading a new ad internally.
        Debug.Log("MRec ad failed to load with error code: " + errorInfo.Code);
    }

    private void OnMRecAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("MRec ad clicked");
    }

    private void OnMRecAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // MRec ad revenue paid. Use this callback to track user revenue.
        Debug.Log("MRec ad revenue paid");

        // Ad revenue
        double revenue = adInfo.Revenue;

        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD"!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        
    }

    #endregion

    #region Track Revenue
    public void TrackAdRevenueFirebase(MaxSdkBase.AdInfo adInfo)
    {
        if (adInfo == null)
        {
            Debug.LogWarning("ad info null");
        }
        else
        {
            if (StaticData.revenue >= VKFirebaseRemoteConfig.instance.remote_threshold)
            {
                Firebase.Analytics.Parameter[] mlAdRevenueParameters = {
                    new Firebase.Analytics.Parameter("ad_platform", "Applovin"),
                    new Firebase.Analytics.Parameter("ad_source", adInfo.NetworkName),
                    new Firebase.Analytics.Parameter("ad_id", adInfo.AdUnitIdentifier),
                    new Firebase.Analytics.Parameter("ad_format", adInfo.AdFormat),
                    new Firebase.Analytics.Parameter("value", StaticData.revenue),
                    new Firebase.Analytics.Parameter("currency", "USD")
                };

                Firebase.Analytics.FirebaseAnalytics.LogEvent("ml_ad_revenue_with_threshold", mlAdRevenueParameters);
                // VKSdk.VKFirebaseManager.Instance.analytics.TrackMultiParam("ml_ad_revenue_with_threshold", param);
                StaticData.revenue = 0;
            }
        }

    }
    public void TrackAdRevenueAdjust(MaxSdkBase.AdInfo info)
    {
        if (info == null)
        {
            Debug.LogWarning("ad info null");
        }
        else
        {
            var adRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAppLovinMAX);
            adRevenue.setRevenue(info.Revenue, "USD");
            adRevenue.setAdRevenueNetwork(info.NetworkName);
            adRevenue.setAdRevenueUnit(info.AdUnitIdentifier);
            adRevenue.setAdRevenuePlacement(info.Placement);

            Adjust.trackAdRevenue(adRevenue);
        }

    }
    #endregion
}