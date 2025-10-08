using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VKSdk.UI;

public class UIPopupColectItem : VKLayer
{
    [SerializeField] private Image colectedItem;
    [SerializeField] private TextMeshProUGUI textValue;
    [SerializeField] private GameObject BtnAdsClaim;
    [SerializeField] private GameObject btnNoThank;

    SkinItem skinItem;

    private int Valuex2;

    private bool isCoin, isRuby, isSkin;
    private void Start()
    {
        isCoin = FortuneWheelManager.instance.isCoin;
        isRuby = FortuneWheelManager.instance.isRuby;
        isSkin = FortuneWheelManager.instance.isSkin;
        LoadSprite();
    }
    void LoadSprite()
    {
        if (this.isCoin)
        {
            colectedItem.rectTransform.sizeDelta = new Vector2(245, 150);
            colectedItem.sprite = Resources.Load<Sprite>($"Item/+Coin");
        }
        if (this.isRuby)
        {
            colectedItem.sprite = Resources.Load<Sprite>($"Item/Ruby");
        }
        else
        {
            colectedItem.SetNativeSize();
            colectedItem.rectTransform.localScale = Vector3.one * 0.6f;
        }
    }

    void OnClaimCoin()
    {
        NotificationCenter.DefaultCenter().PostNotification(this, "UpdateCoin");
        VkAdjustTracker.TrackResourceGain("coin", Valuex2, UserData.Instance.GameData.coin, "nothanks_spin");
        UserData.Instance.SaveLocalData();
    }
    void OnClaimRuby()
    {
        NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");
        VkAdjustTracker.TrackResourceGain("ruby", Valuex2, UserData.Instance.GameData.ruby, "nothanks_spin");
        UserData.Instance.SaveLocalData();
    }
    void OnclaimSkin()
    {
        if(this.skinItem == null)
        {
            NotificationCenter.DefaultCenter().PostNotification(this, "UpdateCoin");
        }
        UserData.Instance.SaveLocalData();
    }
    void OnClose()
    {
        Close();
    }
    public void OnClickNoThankClaim()
    {
        AudioManager.instance.Play("ButtonClick");
        LeanTweenSpinManager.instance.OnTweenObj();

        if (this.isCoin)
        {
            OnClaimCoin();
            LeanTweenSpinManager.instance.OnCloseLayer();
            Invoke("OnClose", 2f);
        }
        else if (this.isRuby)
        {
            OnClaimRuby();
            LeanTweenSpinManager.instance.OnCloseLayer();
            Invoke("OnClose", 2f);
        }
        if (this.isSkin)
        {
            OnclaimSkin();
            LeanTweenSpinManager.instance.OnCloseLayer();
            Invoke("OnClose", 0.5f);
        }
    }
    public void OnClickAdsRewardClaim()
    {
        AudioManager.instance.Play("ButtonClick");
        AdsManager.Instance.ShowRewardedAd(() => {
            AudioManager.instance.Play("CollectRuby");
            UserData.Instance.GameData.ruby += 1;
            NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");
            LeanTweenSpinManager.instance.OnTweenObj();

            if (this.isCoin)
            {
                UserData.Instance.GameData.coin += Valuex2;
                OnClaimCoin();
                VkAdjustTracker.TrackResourceGain("coin", Valuex2*2, UserData.Instance.GameData.coin, "ads_x2coin_spin");
                LeanTweenSpinManager.instance.OnCloseLayer();
                Invoke("OnClose", 2f);
            }
            if (this.isRuby)
            {
                UserData.Instance.GameData.ruby += Valuex2;
                OnClaimRuby();
                VkAdjustTracker.TrackResourceGain("ruby", Valuex2 * 2, UserData.Instance.GameData.ruby, "ads_x2ruby_spin");
                LeanTweenSpinManager.instance.OnCloseLayer();
                Invoke("OnClose", 2f);
            }
            if (this.isSkin)
            {
                OnclaimSkin();
                LeanTweenSpinManager.instance.OnCloseLayer();
                Invoke("OnClose", 0.5f);
            }
        });
    }

    #region override
    public void Init(SkinItem skinItem)
    {
        this.skinItem = skinItem;
        if (skinItem != null)
        {
            colectedItem.rectTransform.sizeDelta = new Vector2(180, 220);
            colectedItem.sprite = Resources.Load<Sprite>("Avatar/" + skinItem.SkinObject.name);
            //textValue.text = skinItem.SkinObject.nameType;
            textValue.gameObject.SetActive(false);
            BtnAdsClaim.SetActive(false);
            btnNoThank.SetActive(false);
        }
        else
        {
            colectedItem.rectTransform.sizeDelta = new Vector2(245, 150);
            colectedItem.sprite = Resources.Load<Sprite>($"Item/+Coin");
            textValue.text = "+10000";

            BtnAdsClaim.SetActive(false);
            btnNoThank.SetActive(false);
        }
    }

    public void InitValue(int value)
    {
        colectedItem.rectTransform.sizeDelta = new Vector2(150, 150);
        textValue.text = "+" + value.ToString();
        Valuex2 = value;
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
