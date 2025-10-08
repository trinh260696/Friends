using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VKSdk.UI;

public class UISpin : VKLayer
{
    public FortuneWheelManager fortuneWheel;
    public LeanTweenUISpin leanTweenUISpin;
    void Start()
    {

    }
    public void CloseSpinUI()
    {
        AudioManager.instance.Play("ButtonClick");
        if (!FortuneWheelManager._isStarted)
        {
            leanTweenUISpin.OnCloseSpinLayer();
            StartCoroutine(CloseSpin());
        }
    }

    IEnumerator CloseSpin()
    {
        yield return new WaitForSeconds(0.5f);
        Close();
    }
    #region override
    public override void ReloadCanvasScale(float screenRatio, float screenScale)
    {
        base.ReloadCanvasScale(screenRatio, screenScale);
    }

    public override void StartLayer()
    {
        base.StartLayer();
    }

    public override void FirstLoadLayer()
    {
        base.FirstLoadLayer();
    }

    public override void ShowLayer()
    {
        base.ShowLayer();
        fortuneWheel.OnInit();
        leanTweenUISpin.OnShowSpinLayer();
    }

    public override void EnableLayer()
    {
        base.EnableLayer();

    }

    public override void ReloadLayer()
    {
        base.ReloadLayer();
    }

    public override void BeforeHideLayer()
    {
        base.BeforeHideLayer();
    }

    public override void DisableLayer()
    {
        base.DisableLayer();
    }

    public override void HideLayer()
    {
        base.HideLayer();
    }

    public override void DestroyLayer()
    {
        base.DestroyLayer();
    }

    public override void Close()
    {
        base.Close();
    }

    public override void OnLayerOpenDone()
    {
        base.OnLayerOpenDone();
    }

    public override void OnLayerCloseDone()
    {
        base.OnLayerCloseDone();
    }

    public override void OnLayerOpenPopupDone()
    {
        base.OnLayerOpenPopupDone();
    }

    public override void OnLayerPopupCloseDone()
    {
        base.OnLayerPopupCloseDone();
    }

    public override void OnLayerSlideHideDone()
    {
        base.OnLayerSlideHideDone();
    }

    public override void OnLayerReOpenDone()
    {
        base.OnLayerReOpenDone();
    }
    #endregion
}
