using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using VKSdk;
using VKSdk.UI;

public class UIPopupGetMoreRuby : VKLayer
{
    [SerializeField] private GameObject rubyIAP;
    private void Start()
    {
        rubyIAP.GetComponent<Button>().onClick.AddListener(VKLayerController.Instance.ShowLoading);
        rubyIAP.GetComponent<IAPButton>().onPurchaseComplete.AddListener(SupportThisGame.Instance.OnCompleteIAP);
        rubyIAP.GetComponent<IAPButton>().onPurchaseFailed.AddListener(SupportThisGame.Instance.OnPurchaseFailed);
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
        if (LimitAds.Instance.ADS_MORE_RUBY >= 5)
        {
            UIPopup.OpenPopup("Notify", "ADS is limited", false);
            return;
        }
       
        AdsManager.Instance.ShowRewardedAd(() => {
            LimitAds.Instance.ADS_MORE_RUBY++;
            LimitAds.Instance.SaveAdsLimit();
            LeanTweenGetMore.instance.OnTweenRuby();
            UserData.Instance.GameData.ruby += 3;
            UserData.Instance.SaveLocalData();
            //NotificationCenter.DefaultCenter().PostNotification(this, "UpdateCoin");
            
            VkAdjustTracker.TrackResourceGain("ruby", 3, UserData.Instance.GameData.ruby, "ads_moreruby");
            VkAdjustTracker.TrackFeatureGetMoreDiamond("ads_reward");
            NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");
        });      
    }

    public void BtnGetMoreRubyVND()
    {
        AudioManager.instance.Play("ButtonClick");
       
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

    public override bool Equals(object other)
    {
        return base.Equals(other);
    }

    public override void FirstLoadLayer()
    {
        base.FirstLoadLayer();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
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

    public override string ToString()
    {
        return base.ToString();
    }
    #endregion
}
