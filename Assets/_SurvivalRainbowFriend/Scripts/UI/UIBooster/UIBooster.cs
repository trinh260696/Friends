using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VKSdk.UI;

public class UIBooster : VKLayer
{
    #region override
    public override void BeforeHideLayer()
    {
        base.BeforeHideLayer();
    }

    public override void Close()
    {
        base.Close();
        StopCoroutine("Close_Enum");
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
    public void OnClickClose()
    {
        AudioManager.instance.Play("ButtonClick"); 
        LeanTweenClose();
    }
    void LeanTweenClose()
    {
        LeanTweenSpinManager.instance.OnCloseLayer();
        Invoke("OnClose", 0f);
    }

    public void OnClickMoreTime()
    {
        AudioManager.instance.Play("ButtonClick");
        AdsManager.Instance.ShowRewardedAd(() =>
        {
            AudioManager.instance.Play("CollectRuby");
            GameManager.Instance.MoreTime();
            UserData.Instance.GameData.ruby += 1;
            UserData.Instance.SaveLocalData();
            NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");
           
            VkAdjustTracker.TrackResourceGain("ruby", 1, UserData.Instance.GameData.ruby, "get_booster");
            VkAdjustTracker.TrackFeatureGetMoreBooster("more_time");
            Close();
        });
        
    }
    public void OnClickMoreBox()
    {
        AudioManager.instance.Play("ButtonClick");
        AdsManager.Instance.ShowRewardedAd(() =>
        {
            AudioManager.instance.Play("CollectRuby");
            GameManager.Instance.MoreItem();
            UserData.Instance.GameData.ruby += 1;
            UserData.Instance.SaveLocalData();
            NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");
            VkAdjustTracker.TrackResourceGain("ruby", 1, UserData.Instance.GameData.ruby, "get_booster");
            VkAdjustTracker.TrackFeatureGetMoreBooster("more_box");
            Close();
        });
       
    }
    public void OnClickMoreSpeed()
    {
        AudioManager.instance.Play("ButtonClick");
        AdsManager.Instance.ShowRewardedAd(() => 
        {
            AudioManager.instance.Play("CollectRuby");
            GameManager.Instance.MoreSpeed();
            UserData.Instance.GameData.ruby += 1;
            UserData.Instance.SaveLocalData();
            NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");
            VkAdjustTracker.TrackResourceGain("ruby", 1, UserData.Instance.GameData.ruby, "get_booster");
            VkAdjustTracker.TrackFeatureGetMoreBooster("more_speed");
            Close();
        });
       
    }
    public void OnClickLightup()
    {
        AudioManager.instance.Play("ButtonClick");
        AdsManager.Instance.ShowRewardedAd(() =>
        {
            AudioManager.instance.Play("CollectRuby");
            GameManager.Instance.TurnOnLight();
            UserData.Instance.GameData.ruby += 1;
            UserData.Instance.SaveLocalData();
            NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");
            VkAdjustTracker.TrackResourceGain("ruby", 1, UserData.Instance.GameData.ruby, "get_booster");
            VkAdjustTracker.TrackFeatureGetMoreBooster("light_up");
            Close();
        });
        
    }
    public void OnClose()
    {
        Close();
    }
    IEnumerator Close_Enum()
    {
        yield return new WaitForSeconds(10f);
        Close();
    }
    public void Init()
    {
        StartCoroutine("Close_Enum");
    }
}
