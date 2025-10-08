using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VKSdk.UI;
using System.Linq;
using UnityEngine.UI;

public class UIDailyReward : VKLayer
{
    #region properties
    [SerializeField] private UIDailyItem[] uiDailyItems;
    [SerializeField] private GameObject[] TweenObj;
    [SerializeField] private Transform CurrenTransform;
    [SerializeField] private Transform RubyTransform;
    [SerializeField] private Transform CoinTransform;
    [SerializeField] private Button btnNothanks;
    [SerializeField] private Button btnClaim;
    [SerializeField] private Button btnX2Reward;
    private int currentDay;

    DailyItem dailyItem;

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
        OnInit();
    }

    public override void StartLayer()
    {
        base.StartLayer();
    }

    #endregion
    #region method Unity

    #endregion
    #region method
    public void OnInit()
    {
        Refresh();
    }
    void Refresh()
    {
        var dailyData = new List<DailyItem>(DailyInitData.Instance.DailyItems);
        currentDay = DailyRewardData.Instance.dailyObject.receiveDay;
        List<bool> listStatus = DailyRewardData.Instance.dailyObject.StatusList;
        for (int i = 0; i < uiDailyItems.Length; i++)
        {

            var uiDaiItem = uiDailyItems[i];
            bool isReach = i <= currentDay;
            bool isReceived = (!listStatus[i]) && isReach;
            uiDaiItem.Init(dailyData[i], i, isReceived, isReach);
        }
    }
    #endregion
    #region Listener
    private void Awake()
    {
        for(int i = 0; i < TweenObj.Length; i++)
        {
            if (TweenObj[i] != null)
            {
                TweenObj[i].SetActive(false);
                TweenObj[i].GetComponent<Image>().rectTransform.sizeDelta = Vector2.one * 100;
            }
        }
    }
    private void Start()
    {
        dailyItem = DailyInitData.Instance.GetDailyItemByIndex(currentDay);
        checkBtnClaimSkin();
    }
    void BtnInter()
    {
        btnClaim.interactable = false;
        btnNothanks.interactable = false;
        btnX2Reward.interactable = false;
    }
    void checkBtnClaimSkin()
    {
        for (int i = 0; i < dailyItem.id_object.Length; i++)
        {
            if (dailyItem.id_object[i].itemType == ItemType.Skin)
            {
                btnClaim.gameObject.SetActive(true);
            }
            else btnClaim.gameObject.SetActive(false);
        }
    }

    public void OnClickClose()
    {
        AudioManager.instance.Play("SuccessReward");
       
        BtnInter();
        string skin = "";
        int iIndexSkin = -1;
        SkinItem skinItem = null;
        for (int i = 0; i < dailyItem.id_object.Length; i++)
        {
            if (dailyItem.id_object[i].itemType == ItemType.Skin)
            {
                iIndexSkin = i;
            }
            else if (dailyItem.id_object[i].itemType == ItemType.Coin)
            {
                UserData.Instance.GameData.coin += dailyItem.id_object[i].Value;
                NotificationCenter.DefaultCenter().PostNotification(this, "UpdateCoin");
                VkAdjustTracker.TrackResourceGain("coin", dailyItem.id_object[i].Value, UserData.Instance.GameData.coin, "daily_nothank");
                CoinTweenDailyReward();
            }
            else
            {
                UserData.Instance.GameData.ruby += dailyItem.id_object[i].Value;
                NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");
                VkAdjustTracker.TrackResourceGain("ruby", dailyItem.id_object[i].Value, UserData.Instance.GameData.ruby, "daily_nothank");
                RubyTweenDailyReward();
            }
        }
        if (iIndexSkin != -1)
        {
            var filterSKins = SkinItemData.Instance.GetFilterSkin();
            iIndexSkin = -1;
            if (filterSKins.Length > 0)
            {
                int rnd = Random.Range(0, filterSKins.Length);
                skinItem = filterSKins[rnd];
            }
        }
        skin = skinItem == null ? "" : skinItem.SkinObject.name;
        var cItems = System.Array.ConvertAll<ID_Object, CID_Object>(dailyItem.id_object,
            (id) => new CID_Object { itemType = id.itemType, Value = id.Value, skinType = skin });
       
         cItems = cItems.Where(c => c.itemType == ItemType.Skin).ToArray();
        
        for (int i = 0; i < cItems.Length; i++)
        {
            if (cItems[i].itemType == ItemType.Skin)
            {
                var uiReward = VKLayerController.Instance.ShowLayer("UIPopupNewSkin") as UIPopupNewSkin;
                uiReward.Init(cItems, true, true);
                break;
            }
        }
        if (skinItem != null)
        {
            SkinItemData.Instance.SetSkin(skinItem);           
            SkinItemData.Instance.SaveSkinData();
        }
        VkAdjustTracker.TrackFeatureDailyReward(currentDay, "no_thanks");
        UserData.Instance.SaveLocalData();

        DailyRewardData.Instance.ReceiveiDaily(currentDay);
        Refresh();

        for (int i = 0; i < dailyItem.id_object.Length; i++)
        {
            if (dailyItem.id_object[i].itemType == ItemType.Skin)
            {
                Invoke("Close", 0.5f);
            }
            else Invoke("Close", 2);
        }
    }

    public void OnClickVideoReward()
    {
        AudioManager.instance.Play("ButtonClick");
        
        AdsManager.Instance.ShowRewardedAd(() =>
        {
            BtnInter();
            UserData.Instance.GameData.ruby += 1;
            AudioManager.instance.Play("SuccessReward");
           
            OnVideoDaily();
            NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");
        });
       
    }
    void OnVideoDaily()
    {
        AudioManager.instance.Play("SuccessReward");
        string skin = "";
        int iIndexSkin = -1;
        SkinItem skinItem = null;
        for (int i = 0; i < dailyItem.id_object.Length; i++)
        {
            if (dailyItem.id_object[i].itemType == ItemType.Skin)
            {
                iIndexSkin = i;
            }
            else if (dailyItem.id_object[i].itemType == ItemType.Coin)
            {
                UserData.Instance.GameData.coin += dailyItem.id_object[i].Value * 2;
                NotificationCenter.DefaultCenter().PostNotification(this, "UpdateCoin");
                VkAdjustTracker.TrackResourceGain("coin", dailyItem.id_object[i].Value * 2, UserData.Instance.GameData.coin, "daily_x2");
                CoinTweenDailyReward();
            }
            else
            {
                UserData.Instance.GameData.ruby += dailyItem.id_object[i].Value * 2;
                VkAdjustTracker.TrackResourceGain("ruby", dailyItem.id_object[i].Value * 2, UserData.Instance.GameData.ruby, "daily_x2");
                RubyTweenDailyReward();
            }
        }
        if (iIndexSkin != -1)
        {
            var filterSKins = SkinItemData.Instance.GetFilterSkin();
            iIndexSkin = -1;
            if (filterSKins.Length > 0)
            {
                int rnd = Random.Range(0, filterSKins.Length);
                skinItem = filterSKins[rnd];
            }
        }
        skin = skinItem == null ? "" : skinItem.SkinObject.name;
        var cItems = System.Array.ConvertAll<ID_Object, CID_Object>(dailyItem.id_object,
            (id) => new CID_Object { itemType = id.itemType, Value = id.Value, skinType = skin });
      
         cItems = cItems.Where(c => c.itemType == ItemType.Skin).ToArray();
        
        
        for (int i = 0; i < cItems.Length; i++)
        {
            if (cItems[i].itemType == ItemType.Skin)
            {
                var uiReward = VKLayerController.Instance.ShowLayer("UIPopupNewSkin") as UIPopupNewSkin;
                uiReward.Init(cItems, true, true);
                break;
            }
        }
        if (skinItem != null)
        {
            SkinItemData.Instance.SetSkin(skinItem);
            SkinItemData.Instance.SaveSkinData();
        }
        VkAdjustTracker.TrackFeatureDailyReward(currentDay, "x2_reward");
        UserData.Instance.SaveLocalData();
        DailyRewardData.Instance.ReceiveiDaily(currentDay);
        Refresh();

        for (int i = 0; i < dailyItem.id_object.Length; i++)
        {
            if (dailyItem.id_object[i].itemType == ItemType.Skin)
            {
                Invoke("Close", 0.5f);
            }
            else Invoke("Close", 2);
        }
    }
    void OnVideoDailyX2Skin()
    {
        int iIndexSkin = -1;
        SkinItem skinItem1 = null;
        SkinItem skinItem2 = null;
        string skin = "";
        for (int i = 0; i < dailyItem.id_object.Length; i++)
        {
            if (dailyItem.id_object[i].itemType == ItemType.Skin)
            {
                iIndexSkin = i;

            }
            else if (dailyItem.id_object[i].itemType == ItemType.Coin)
            {
                UserData.Instance.GameData.coin += dailyItem.id_object[i].Value * 2;
                NotificationCenter.DefaultCenter().PostNotification(this, "UpdateCoin");
                CoinTweenDailyReward();
            }
            else
            {
                UserData.Instance.GameData.ruby += dailyItem.id_object[i].Value * 2;
                RubyTweenDailyReward();
            }

        }
        UserData.Instance.GameData.ruby += 1;
        NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");
        if (iIndexSkin != -1)
        {
            var filterSKins = SkinItemData.Instance.GetFilterSkin();

            iIndexSkin = -1;
            if (filterSKins.Length == 1)
            {
                int rnd = Random.Range(0, filterSKins.Length);
                skinItem1 = filterSKins[rnd];
            }
            else if (filterSKins.Length > 1)
            {
                int rnd = Random.Range(0, filterSKins.Length);
                skinItem1 = filterSKins[rnd];
                skinItem2 = filterSKins[(rnd + 1) % filterSKins.Length];
            }

        }

        var cItems = System.Array.ConvertAll<ID_Object, CID_Object>(dailyItem.id_object,
            (id) => new CID_Object { itemType = id.itemType, Value = id.Value * 2, skinType = skin });
        var result = new List<CID_Object>();
        for (int i = 0; i < cItems.Length; i++)
        {
            if (cItems[i].itemType != ItemType.Skin)
                result.Add(cItems[i]);
            if (cItems[i].itemType == ItemType.Ruby)
            {
                result[i].Value += 1;
            }
        }
        if (skinItem1 != null)
        {
            result.Add(new CID_Object { itemType = ItemType.Skin, Value = 1, skinType = skinItem1.SkinObject.name });
        }
        if (skinItem2 != null)
        {
            result.Add(new CID_Object { itemType = ItemType.Skin, Value = 1, skinType = skinItem2.SkinObject.name });
        }

        // Show Popup NewSkin
        for (int i = 0; i < dailyItem.id_object.Length; i++)
        {
            var uiReward = VKLayerController.Instance.ShowLayer("UIPopupNewSkin") as UIPopupNewSkin;
            uiReward.Init(result.ToArray(), true, true);
        }

        SkinItemData.Instance.SaveSkinData();
        UserData.Instance.SaveLocalData();
        DailyRewardData.Instance.ReceiveDailyVideo(currentDay);
        Refresh();
        Invoke("Close", 3f);
    }
    void CoinTweenDailyReward()
    {
        for (int i = 0; i < TweenObj.Length; i++)
        {
            if (TweenObj[i] == null)
            {
                Debug.Log("TweenObj null");
                return;
            }
            else
            {
                TweenObj[i].SetActive(true);
                float distanceX = UnityEngine.Random.Range(-2, 2);
                float distanceY = UnityEngine.Random.Range(-1, 1);
                TweenObj[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Item/Coin");
                LeanTween.scale(TweenObj[i], new Vector3(2, 2, 1), 0);
                LeanTween.move(TweenObj[i], CurrenTransform.position, 0);
                LeanTween.move(TweenObj[i], new Vector3(CurrenTransform.position.x + distanceX, CurrenTransform.position.y + distanceY, 1), 1).setEase(LeanTweenType.easeOutExpo);
                LeanTween.move(TweenObj[i], CoinTransform.position, 1).setDelay(0.5f + i * 0.1f).setEase(LeanTweenType.easeInOutCirc).setOnComplete(() =>
                {
                    AudioManager.instance.Play("CollectCoin");
                });
                LeanTween.scale(TweenObj[i], new Vector3(0.7f, 0.7f, 1), 1.5f + 0.1f * i);
            }
        }
    }
    void RubyTweenDailyReward()
    {
        for (int i = 0; i < TweenObj.Length; i++)
        {
            if (TweenObj[i] == null)
            {
                Debug.Log("TweenObj null");
                return;
            }
            else
            {
                TweenObj[i].SetActive(true);
                float distanceX = UnityEngine.Random.Range(-2, 2);
                float distanceY = UnityEngine.Random.Range(-1, 1);
                TweenObj[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Item/Ruby");
                LeanTween.scale(TweenObj[i], new Vector3(2, 2, 1), 0);
                LeanTween.move(TweenObj[i], CurrenTransform.position, 0);
                LeanTween.move(TweenObj[i], new Vector3(CurrenTransform.position.x + distanceX, CurrenTransform.position.y + distanceY, 1), 1).setEase(LeanTweenType.easeOutExpo);
                LeanTween.move(TweenObj[i], RubyTransform.position, 1).setDelay(0.5f + i * 0.1f).setEase(LeanTweenType.easeInOutCirc).setOnComplete(() =>
                {
                    AudioManager.instance.Play("CollectRuby");
                });
                LeanTween.scale(TweenObj[i], new Vector3(0.7f, 0.7f, 1), 1.5f + 0.1f * i);
            }
        }
    }
    #endregion
}
