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
       
    }
    public void ShowOpenADS()
    {
        if (UserData.Instance.GameData.vip > 0) return;
      
    }
   
    #endregion
    #region Interstitial Ad Methods

    
  
    public void ShowInterstitial()
    {
        if (UserData.Instance.GameData.vip > 0) return;
       
    }
    
    
   

    #endregion

    #region Rewarded Ad Methods

  

   

    public void ShowRewardedAd(UnityAction action)
    {
        action.Invoke();
    }

    internal void ToggleBannerVisibility()
    {
        
    }

    #endregion



    #region Banner Ad Methods





    #endregion



    #region Track Revenue

    #endregion
}