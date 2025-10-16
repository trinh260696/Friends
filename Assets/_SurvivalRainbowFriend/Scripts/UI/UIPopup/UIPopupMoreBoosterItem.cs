using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VKSdk;
using VKSdk.UI;
using TMPro;
public class UIPopupMoreBoosterItem : VKLayer
{
    [SerializeField] private GameObject panelScale;
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

    public void GetItemBtnClick()
    {
        AudioManager.instance.Play("ButtonClick");
        //Code
        Close();
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
        OnShowLayer();
    }

    public override void StartLayer()
    {
        base.StartLayer();
    }
    #region method
    public void Init()
    {
        int from = 7;
        int to = 0;
        //LeanTween.value(textCountDown.gameObject, (value) =>
        //{
        //    textCountDown.text = ((int)(value)).ToString();

        //}, from, 0f, 7).setOnComplete(OnClose);
    }
    public void OnClickClose()
    {
        
        var layer = VKLayerController.Instance.ShowLayer("UIYouDiedDialog") as UIYouDiedDialog;
       // layer.Init(GameManager.Instance.StatusMissions, GameManager.Instance.levelData.Items);
        GameManager.Instance.TurnLose();
        GameManager.Instance.CloseGame();
        Close();
    }
    public void OnClickVideo()
    {
        AudioManager.instance.Play("ButtonClick");
        AdsManager.Instance.ShowRewardedAd(() =>
        {
            //   LeanTween.cancel(textCountDown.gameObject);
            AudioManager.instance.Play("UnlockAchievement3");
            AudioManager.instance.Play("CollectRuby");
            int rand = UnityEngine.Random.Range(0, 1);
            switch (rand)
            {
                case 0: AudioManager.instance.Play("gru1"); break;
                case 1: AudioManager.instance.Play("gru2"); break;
            }
            UserData.Instance.GameData.ruby += 1;
            UserData.Instance.SaveLocalData();
            NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");
            VkAdjustTracker.TrackResourceGain("ruby", 1, UserData.Instance.GameData.ruby, "ads_moretime");
            VkAdjustTracker.TrackFeatureEndMoreBooster("more_time");
            GameManager.Instance.Recover();
            LeanTween.scale(panelScale, new Vector3(0, 0, 0), 0.5f).setEase(LeanTweenType.easeOutExpo);
            Invoke("Close", .5f);
        });
    }
    void OnShowLayer()
    {
        AudioManager.instance.Play("Lose");
        LeanTween.scale(panelScale, new Vector3(0, 0, 0), 0);
        LeanTween.scale(panelScale, new Vector3(1, 1, 1), 1).setEase(LeanTweenType.easeOutExpo);
    }
    #endregion
}
