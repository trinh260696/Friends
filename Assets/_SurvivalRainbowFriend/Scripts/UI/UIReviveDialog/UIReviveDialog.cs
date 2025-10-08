using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VKSdk.UI;
using TMPro;
public class UIReviveDialog : VKLayer
{
    #region properties
    [SerializeField] private TextMeshProUGUI textCountDown;
    #endregion
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
        Init();
    }

    private void Init()
    {
        int from = 7;
        int to = 0;
        LeanTween.value(textCountDown.gameObject, (value) =>
        {
            textCountDown.text = ((int)(value)).ToString();

        }, from, 0f, 7).setOnComplete(OnClose);
    }

    public override void StartLayer()
    {
        base.StartLayer();
    }
    #endregion
    #region Listener
    public void OnClickClose()
    {
        AudioManager.instance.Play("ButtonClick");
        OnClose();        
    }
    void OnClose()
    {
        LeanTween.cancel(textCountDown.gameObject);
        OpenDieDialog();
        
        
        Close();
    }
    void OpenDieDialog()
    {
        if (StaticData.GM_Mode == Mode.SurvivalMode)
        {
            GameManager.Instance.TurnLose();
            GameManager.Instance.CloseGame();
            var layer = VKLayerController.Instance.ShowLayer("UIYouDiedDialog") as UIYouDiedDialog;
            layer.Init(GameManager.Instance.StatusMissions, GameManager.Instance.levelData.Items);
        }             
    }
    public void OnClickVideo()
    {
        AudioManager.instance.Play("ButtonClick");
        AdsManager.Instance.ShowRewardedAd(() =>
        {
            AudioManager.instance.Play("UnlockAchievement");
            int rand = UnityEngine.Random.Range(0, 1);
            switch (rand)
            {
                case 0: AudioManager.instance.Play("gru1"); break;
                case 1: AudioManager.instance.Play("gru2"); break;
            }
            LeanTween.cancel(textCountDown.gameObject);
            UserData.Instance.GameData.ruby += 1;
            UserData.Instance.SaveLocalData();
            NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");
            VkAdjustTracker.TrackResourceGain("ruby", 1, UserData.Instance.GameData.ruby, "ads_revive");
            VkAdjustTracker.TrackFeatureEndMoreBooster("revive");
            GameManager.Instance.Revive();
            Close();
        });       
    }
    #endregion 

}
