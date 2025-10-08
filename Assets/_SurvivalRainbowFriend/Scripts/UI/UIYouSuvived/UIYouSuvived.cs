using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using VKSdk.UI;


public class UIYouSuvived : VKLayer
{
    [SerializeField] private GameObject StickObject;
    private bool rotateClock = false;
    [SerializeField] private TextMeshProUGUI coinReward;
    [SerializeField] private TextMeshProUGUI xRewardText;
    [SerializeField] private TextMeshProUGUI rateRewardText;
    [SerializeField] private TextMeshProUGUI namePlayer;
    [SerializeField] private Button btnVideo;
    [SerializeField] private Button btnNoThank;
    [SerializeField] private Image processImg;
    [SerializeField] private TextMeshProUGUI proccessText;
    [SerializeField] private LeanTweenSurvived leanTweenSurvived;
    private int reward = 500;
    public static int rate;
    public static int Proccess = 0;
    public static bool isSkin = false;

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
        if (Proccess >= 99)
        {
            Proccess = 0;
        }
        StopCoroutine("RotateStick");
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
        canvas.sortingLayerName = "UI";
        btnNoThank.interactable = true;
        btnVideo.interactable = true;
        StartCoroutine("RotateStick");
    }

    public override void StartLayer()
    {
        base.StartLayer();
    }
    #endregion
    public void Init(string str)
    {
        UIYouSuvived.rate = 1;
        processImg.fillAmount = (float)(Proccess) / 100f;
        if (UIYouSuvived.Proccess+25 < 100)
            PlayerPrefs.SetInt("Proccess", UIYouSuvived.Proccess + 25);
        else
            PlayerPrefs.SetInt("Proccess", 0);
        proccessText.text = Proccess.ToString() + "%";
        int from = UIYouSuvived.Proccess;
        LeanTween.value(proccessText.gameObject, (value) =>
        {
            proccessText.text = string.Format("{0}%", (int)(value));
            processImg.fillAmount = (float)(value) / 100f;
        }, from, from + 25, 1f).setDelay(1.5f).setOnComplete(()=> 
        {
            Proccess += 25;
            proccessText.text = string.Format("{0}%", Proccess);
            processImg.fillAmount= (float)(Proccess) / 100f;
            if (Proccess >= 99)
            {
                var skin = SkinItemData.Instance.GetSkinRndNotSet();
                var newSkinLayer = VKLayerController.Instance.ShowLayer("UINewSkin") as UINewSkin;
                newSkinLayer.OnInit(skin);
                //Proccess = 0;
                //processImg.fillAmount = 1;
            }
        });
        var levelObj = InitData.Instance.GetLevelData(StaticData.LEVEL, 1);
        VkAdjustTracker.TrackProgressComplete("survival", levelObj.name, levelObj.level, (int)GameManager.Instance.totalTime);
    }

    private void Start()
    {
        StartCoroutine("RotateStick");
        namePlayer.text = UserData.Instance.GameData.name.ToString();
    }
    private void Update()
    {
        if (isSkin)
        {
            LeanTween.value(proccessText.gameObject, (value) =>
            {
                proccessText.text = string.Format("{0}%", (int)(value));
                processImg.fillAmount = (float)(value) / 100f;
            }, 100, 0, 1f).setDelay(.5f);
            isSkin= false;
        }
    }

    void BtnInter()
    {
        btnVideo.interactable = false;
        btnNoThank.interactable = false;
    }

    public void OnClickNoThank()
    {
        AudioManager.instance.Play("ButtonClick");
        AudioManager.instance.Play("SuccessReward");
        StopAllCoroutines();
        BtnInter();
        UserData.Instance.GameData.coin += reward;
        UserData.Instance.GameData.ruby += 1;
        UserData.Instance.SaveLocalData();
        leanTweenSurvived.CoidRewardTween(() =>
        {
            OnClaimComplete();
            VkAdjustTracker.TrackResourceGain("coin", 500, UserData.Instance.GameData.coin, "reward_nothank_passlevel");
            VkAdjustTracker.TrackResourceGain("ruby", 1, UserData.Instance.GameData.ruby, "reward_nothank_passlevel");
            Invoke("OnAdsInter", 1.5f);
        });
        
    }
    public void OnClickVideo()
    {
        AudioManager.instance.Play("ButtonClick");
        if (!AdsManager.Instance.CheckAdsReady())
        {
            UIPopup.OpenPopup("Error", "Video is not available!", false);
            return;
        }
        StopAllCoroutines();
        BtnInter();      
        LeanTween.cancel(StickObject.gameObject);
        float RotZ = StickObject.transform.localEulerAngles.z;

        if (RotZ <= 90 && RotZ >= 40 || RotZ < 310 && RotZ >= 270) rate = 2;
        else if (RotZ < 40 && RotZ >= 10 || RotZ < 350 && RotZ >= 310) rate = 3;
        else rate = 5;

        AdsManager.Instance.ShowRewardedAd(() =>
        {
            AudioManager.instance.Play("SuccessReward");
            AudioManager.instance.Play("CollectRuby");
            int from = reward;
            LeanTween.value(xRewardText.gameObject, (value) => {
                int iValue = (int)value;
                xRewardText.text = iValue.ToString();
            }, from, reward * rate, 1f);
            xRewardText.text = string.Format("{0}", reward * rate);
            UserData.Instance.GameData.ruby += 2;
            UserData.Instance.GameData.coin += reward*rate;
            UserData.Instance.SaveLocalData();
            leanTweenSurvived.CoidRewardTween(() =>
            {
                OnClaimComplete();
                VkAdjustTracker.TrackResourceGain("coin",reward*rate, UserData.Instance.GameData.coin, "reward_ads_passlevel");
                VkAdjustTracker.TrackResourceGain("ruby", 1, UserData.Instance.GameData.ruby, "reward_ads_passlevel");
            });
        });

        
    }
    IEnumerator RotateStick()
    {
        float from = 90f;
        while (!rotateClock)
        {
            float r = 0;
            LeanTween.value(StickObject.gameObject, (value) =>
            {
                StickObject.transform.localEulerAngles = Vector3.forward * value;
                float RotZ = StickObject.transform.localEulerAngles.z;

                if (RotZ <= 90 && RotZ >= 40 || RotZ < 310 && RotZ >= 270) r = 2;
                else if (RotZ < 40 && RotZ >= 10 || RotZ < 350 && RotZ >= 310) r = 3;
                else r = 5;

                coinReward.text = (reward * r).ToString();
                rateRewardText.text = string.Format("x{0}", r);
            }, from, -90f, 1f).setEase(LeanTweenType.linear);
            yield return new WaitForSeconds(1.01f);
            from = -90f;
            LeanTween.value(StickObject.gameObject, (value) =>
            {
                StickObject.transform.localEulerAngles = Vector3.forward * value;
                float RotZ = StickObject.transform.localEulerAngles.z;

                if (RotZ <= 90 && RotZ >= 40 || RotZ < 310 && RotZ >= 270) r = 2;
                else if (RotZ < 40 && RotZ >= 10 || RotZ < 350 && RotZ >= 310) r = 3;
                else r = 5;

                coinReward.text = (reward * r).ToString();
                rateRewardText.text = string.Format("x{0}", r);
            }, from, 90f, 1f).setEase(LeanTweenType.linear);
            from = 90f;
            yield return new WaitForSeconds(1.01f);
        }
       
    }
    public void OnAdsReward()
    {
        NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");
        UserData.Instance.SaveLocalData();
        Invoke("OnClose", 1.5f);
    }
    public void OnClaimComplete()
    {
        
        NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");
        NotificationCenter.DefaultCenter().PostNotification(this, "UpdateCoin");      
        Invoke("OnClose", 1.5f);
    }
    void OnClose()
    {
        GameManager.Instance.CloseGame();
        LoadScene.Instance.LoadSceneAndLoading("MainStartUI");
        
        Close();
    }
    void OnAdsInter()
    {
        AdsManager.Instance.ShowInterstitial();
    }
}
