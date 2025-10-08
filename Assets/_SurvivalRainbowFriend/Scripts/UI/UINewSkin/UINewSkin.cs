using Spine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using VKSdk.UI;

public class UINewSkin : VKLayer
{
    [SerializeField] private Image imgIcon;
    [SerializeField] private GameObject Panel;
    [SerializeField] private Transform coinTransform;
    [SerializeField] private Transform rubyTransform;
    [SerializeField] private GameObject[] TweenObj;
    [SerializeField] private Button btnClaim;
    [SerializeField] private Button btnNothank;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private TextMeshProUGUI titleText;
    private SkinItem skinItem;

    #region Override
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
        AudioManager.instance.Play("UnlockAchievement");
        LeanTween.scale(Panel, Vector2.zero, 0);
        LeanTween.scale(Panel, Vector2.one, 1).setEase(LeanTweenType.easeOutElastic);
    }

    public override void StartLayer()
    {
        base.StartLayer();
    }
    #endregion

    private void Start()
    {
        for (int i = 0; i < TweenObj.Length; i++)
            TweenObj[i].SetActive(false);
    }

    public void OnInit(SkinItem skinItem)
    {
        if (skinItem != null)
        {
            this.skinItem = skinItem;
            imgIcon.sprite = Resources.Load<Sprite>("Avatar/" + skinItem.SkinObject.name);
            imgIcon.SetNativeSize();
            imgIcon.rectTransform.localScale = Vector3.one * 0.5f;
            // valueText.text = skinItem.SkinObject.nameType.ToString();
            valueText.gameObject.SetActive(false);
        }
        else
        {
            imgIcon.rectTransform.sizeDelta = new Vector2(300, 230);
            titleText.text = "Congratulation";
            valueText.text = "+10000";
            imgIcon.sprite = Resources.Load<Sprite>("Item/+Coin");
        }
    }

    void BtnInter()
    {
        btnClaim.gameObject.SetActive(false);
        btnNothank.gameObject.SetActive(false);
    }
    public void OnClickClaim()
    {
        AudioManager.instance.Play("ButtonClick");
        if(this.skinItem != null)
        {
            AdsManager.Instance.ShowRewardedAd(() =>
            {
                UserData.Instance.GameData.ruby += 1;
                NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");
                AudioManager.instance.Play("SuccessReward");
                UIYouSuvived.isSkin = true;
                UIYouSuvived.Proccess = 0;
                BtnInter();
                SkinItemData.Instance.SetSkin(this.skinItem);
                LeanTween.scale(Panel, Vector2.zero, 0.5f).setEase(LeanTweenType.easeInOutCirc).setOnComplete(() => Close());

            });
        }
        else
        {
            AdsManager.Instance.ShowRewardedAd(() =>
            {
                UserData.Instance.GameData.coin += 10000;
                UserData.Instance.GameData.ruby += 1;
                NotificationCenter.DefaultCenter().PostNotification(this, "UpdateCoin");
                NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");
                AudioManager.instance.Play("SuccessReward");
                UIYouSuvived.isSkin = true;
                UIYouSuvived.Proccess = 0;
                BtnInter();
                for(int i=0; i<TweenObj.Length; i++)
                    TweenObj[i].SetActive(true);
                for (int i = 0; i < TweenObj.Length; i++)
                {
                    if(i == 0)
                    {
                        float distanceX = Random.Range(-2f, 2f);
                        float distanceY = Random.Range(-1f, 1f);
                        TweenObj[i].GetComponent<Image>().sprite = Resources.Load<Sprite>($"Item/Ruby");
                        LeanTween.scale(TweenObj[i], new Vector3(2, 2, 2), 0);
                        LeanTween.move(TweenObj[i], Panel.transform.position, 0);
                        LeanTween.move(TweenObj[i], new Vector3(Panel.transform.position.x + distanceY, Panel.transform.position.y + distanceY, 0), 1f).setEase(LeanTweenType.easeOutExpo);
                        LeanTween.move(TweenObj[i], rubyTransform.position, 1f).setDelay(0.5f + i * 0.1f).setEase(LeanTweenType.easeInOutCirc).setOnComplete(() => { AudioManager.instance.Play("CollectRuby"); });
                        LeanTween.scale(TweenObj[i], new Vector3(0.6f, 0.6f, 1), 1.5f + i * 0.1f);
                    }
                    else
                    {
                        float distanceX = Random.Range(-2f, 2f);
                        float distanceY = Random.Range(-1f, 1f);
                        TweenObj[i].GetComponent<Image>().sprite = Resources.Load<Sprite>($"Item/Coin");
                        LeanTween.scale(TweenObj[i], new Vector3(2, 2, 2), 0);
                        LeanTween.move(TweenObj[i], Panel.transform.position, 0);
                        LeanTween.move(TweenObj[i], new Vector3(Panel.transform.position.x + distanceY, Panel.transform.position.y + distanceY, 0), 1f).setEase(LeanTweenType.easeOutExpo);
                        LeanTween.move(TweenObj[i], coinTransform.position, 1f).setDelay(0.5f + i * 0.1f).setEase(LeanTweenType.easeInOutCirc).setOnComplete(() => { AudioManager.instance.Play("CollectCoin"); });
                        LeanTween.scale(TweenObj[i], new Vector3(0.6f, 0.6f, 1), 1.5f + i * 0.1f);
                    }
                }
                Invoke("OnClose", 2f);
            });
        }
    }

    void OnClose()
    {
        LeanTween.scale(Panel, Vector2.zero, 0.5f).setEase(LeanTweenType.easeInOutCirc).setOnComplete(() => Close());
    }
    public void OnClickClose()
    {
        AudioManager.instance.Play("ButtonClick");
        UIYouSuvived.isSkin = true;
        UIYouSuvived.Proccess = 0;
        BtnInter();
        LeanTween.scale(Panel, Vector2.zero, 0.5f).setEase(LeanTweenType.easeInOutCirc).setOnComplete(() => Close());
    }
}
