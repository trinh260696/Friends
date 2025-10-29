using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using VKSdk.UI;

public class UIGame : VKLayer
{
    [SerializeField] private RectTransform transformParent;
    [SerializeField] private GameObject removeAdsObject;
    private GameObject skinStartupObj;

    private void Start()
    {
        NotificationCenter.DefaultCenter().AddObserver(this, "ChangeSkin");
        NotificationCenter.DefaultCenter().AddObserver(this, "ChangeSkin");
        NotificationCenter.DefaultCenter().AddObserver(this, "HideBanner");
        var scaleFactor = VKLayerController.GetScale(Screen.width, Screen.height, new Vector2(1920, 1080));

        if (UserData.Instance.GameData.vip == 0)
        {
            scaleFactor = scaleFactor * StaticData.ADS_SCALE_RATIO;
        }
        else
        {
            HideBtnAds();
            gameObject.GetComponent<CanvasScaler>().scaleFactor = scaleFactor;
        }
        gameObject.GetComponent<CanvasScaler>().scaleFactor = scaleFactor;
        GetItemSkeleton(UserData.Instance.GameData.currentSkin.SkinObject.name);
        removeAdsObject.GetComponent<Button>().onClick.AddListener(VKLayerController.Instance.ShowLoading);
        removeAdsObject.GetComponent<IAPButton>().onPurchaseComplete.AddListener(SupportThisGame.Instance.OnCompleteIAP);
        removeAdsObject.GetComponent<IAPButton>().onPurchaseFailed.AddListener(SupportThisGame.Instance.OnPurchaseFailed);
       
    }
    public void HideBtnAds()
    {
        removeAdsObject.SetActive(false);
        
    }
 
    public override  void HideBanner()
    {
        base.HideBanner();
        HideBtnAds();
    }
    public void ChangeSkin()
    {
        foreach(RectTransform rect in transformParent)
        {
            rect.gameObject.SetActive(false);
        }
        GetItemSkeleton(UserData.Instance.GameData.currentSkin.SkinObject.name);
    }
    public RectTransform GetItemSkeleton(string name)
    {
        var go = transformParent.GetChild(0).GetComponent<RectTransform>();
        var skeletonMecanim = go.GetComponent<SkeletonGraphic>();
        skeletonMecanim.Skeleton.SetSkin(name);
        skeletonMecanim.Skeleton.SetToSetupPose(); // Đặt về tư thế ban đầu nếu cần
        skeletonMecanim.LateUpdate();
        return go;
    }

    public void ShowSettingLayer()
    {
        ClickSoundBtn();
        VKLayerController.Instance.ShowLayer("UISetting");
        ShowInter();
    }
    public void ShowSpinLayer()
    {
        ClickSoundBtn();
        VKLayerController.Instance.ShowLayer("UISpin");
        ShowInter();
    }

    public void ShowSkinLayer()
    {
        ClickSoundBtn();
        VKLayerController.Instance.ShowLayer("UISkin");
        ShowInter();
    }
    void ShowInter()
    {
        if (StaticData.TIME_INTER < 25) return;
        StaticData.TIME_INTER = 0;
        AdsManager.Instance.ShowInterstitial();
    }
    public void ClickSoundBtn()
    {
        AudioManager.instance.Play("ButtonClick");
    }
    
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
      
    }

    public override void EnableLayer()
    {
        base.EnableLayer();
        
    }

    public override void ReloadLayer()
    {
        base.ReloadLayer();
        foreach (RectTransform rect in transformParent)
        {
            rect.gameObject.SetActive(false);
        }
        GetItemSkeleton(UserData.Instance.GameData.currentSkin.SkinObject.name);
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
    

}
