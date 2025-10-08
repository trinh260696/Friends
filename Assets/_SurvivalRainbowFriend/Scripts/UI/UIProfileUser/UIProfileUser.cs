using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VKSdk.UI;
using TMPro;

public class UIProfileUser : VKLayer
{
    [SerializeField] private bool firstLogin;
    [SerializeField] private int firstDay;
    [SerializeField] private TMP_InputField NameUser;
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
        ShowUIProfile();
    }

    public override void StartLayer()
    {
        base.StartLayer();
    }
#endregion

    public void ShowUIProfile()
    {
        NameUser.text = UserData.Instance.GameData.name;
    }
    public void ConfirmClickBtn()
    {
        AudioManager.instance.Play("ButtonClick");
        if (NameUser.text == "")
        {
            UIPopup.OpenPopup("Error", "Name is empty!", false);
            return;
        }
        UserData.Instance.GameData.name = NameUser.text;
        UIPopup.OpenPopup("Congratulation!", string.Format("Hello {0}, Welcome to Sus imposter Rainbow Monsters! ", UserData.Instance.GameData.name),(isOK)=> {
            VKLayerController.Instance.ShowLayer("UIDailyReward");
        },false);
        Close();
    }
}
