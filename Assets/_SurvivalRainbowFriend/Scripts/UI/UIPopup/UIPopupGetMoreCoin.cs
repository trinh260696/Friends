using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using VKSdk;
using VKSdk.UI;

public class UIPopupGetMoreCoin : VKLayer
{
    [SerializeField] private GameObject coinIAP;
    void Start()
    {
        coinIAP.GetComponent<Button>().onClick.AddListener(VKLayerController.Instance.ShowLoading);
        coinIAP.GetComponent<IAPButton>().onPurchaseComplete.AddListener(SupportThisGame.Instance.OnCompleteIAP);
        coinIAP.GetComponent<IAPButton>().onPurchaseFailed.AddListener(SupportThisGame.Instance.OnPurchaseFailed);
    }
    public void ClosePopupBtn()
    {
        AudioManager.instance.Play("ButtonClick");
        LeanTweenGetMore.instance.OnCloseLayer();
        Invoke("OnClose", 0.5f);
    }
    void OnClose()
    {
        Close();
    }
    public void BtnGetMoreFree()
    {
        AudioManager.instance.Play("ButtonClick");
        if (LimitAds.Instance.ADS_MORE_COIN >= 5)
        {
            UIPopup.OpenPopup("Notify", "ADS is limited", false);
            return;
        }
       

        AdsManager.Instance.ShowRewardedAd(() => {
            LimitAds.Instance.ADS_MORE_COIN++;
            LimitAds.Instance.SaveAdsLimit();
            LeanTweenGetMore.instance.OnTweenObj();
            UserData.Instance.GameData.coin += 300;
            UserData.Instance.GameData.ruby += 1;
            VkAdjustTracker.TrackResourceGain("coin", 300, UserData.Instance.GameData.coin, "ads_morecoin");
            VkAdjustTracker.TrackResourceGain("ruby", 1, UserData.Instance.GameData.ruby, "ads_morecoin");
            VkAdjustTracker.TrackFeatureGetMoreCoin("ads_reward");
            UserData.Instance.SaveLocalData();
            NotificationCenter.DefaultCenter().PostNotification(this, "UpdateCoin");
            NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");

        });
        
    }
    public void BtnGetMoreCoinVND()
    {
        AudioManager.instance.Play("ButtonClick");
        //UserData.Instance.GameData.coin += 7000;
        //UserData.Instance.SaveLocalData();
        //NotificationCenter.DefaultCenter().PostNotification(this, "UpdateCoin");
    }
    #region override
    public override void BeforeHideLayer()
    {
        base.BeforeHideLayer();
    }

    public override void Close()
    {
        base.Close();
    }

    public override void DestroyLayer()
    {
        base.DestroyLayer();
    }

    public override void DisableLayer()
    {
        base.DisableLayer();
    }

    public override void EnableLayer()
    {
        base.EnableLayer();
    }

    public override void FirstLoadLayer()
    {
        base.FirstLoadLayer();
    }

    public override void HideLayer()
    {
        base.HideLayer();
    }

    public override void OnLayerCloseDone()
    {
        base.OnLayerCloseDone();
    }

    public override void OnLayerOpenDone()
    {
        base.OnLayerOpenDone();
    }

    public override void OnLayerOpenPopupDone()
    {
        base.OnLayerOpenPopupDone();
    }

    public override void OnLayerPopupCloseDone()
    {
        base.OnLayerPopupCloseDone();
    }

    public override void OnLayerReOpenDone()
    {
        base.OnLayerReOpenDone();
    }

    public override void OnLayerSlideHideDone()
    {
        base.OnLayerSlideHideDone();
    }

    public override void ReloadCanvasScale(float screenRatio, float screenScale)
    {
        base.ReloadCanvasScale(screenRatio, screenScale);
    }

    public override void ReloadLayer()
    {
        base.ReloadLayer();
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
    }

    public override void StartLayer()
    {
        base.StartLayer();
    }
#endregion
}
