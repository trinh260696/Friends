using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VKSdk.UI;
using Spine;
using Spine.Unity;
using TMPro;
public class UISkin : VKLayer
{
    public LeanTweenManager tweenSkin;

    [Header("SkinShop")]
    [SerializeField] private GameObject[] pageSkin;
    [SerializeField] private UISkinItem[] uiSkinItems;
    [SerializeField] private UIBoxItem[] uiBoxItems;

    [SerializeField] private GameObject btnAds;
    [SerializeField] private GameObject btnCoinRand;
    [SerializeField] private GameObject btnRubyAds;
    [SerializeField] private GameObject btnBoxCoin;
    [SerializeField] private GameObject btnBoxAds;
    [SerializeField] private GameObject PanelAds;

    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI adsText;
    [SerializeField] private RectTransform skeletonGraphicParents;
    public SkinItem selectedSkin;
    public BoxItem selectedBox;
    public GameObject btnBuy;

    public static string spineNameBox = "";
    private int currentPage = 0;

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
        tweenSkin.OnShowLayer();
    }

    public override void StartLayer()
    {
        base.StartLayer();
    }

    #endregion

    private void Start()
    {
        LoadPage();
    }
    public void CloseSkinUI()
    {
        SoundBtnClick();
        tweenSkin.OnCloseLayer();
        NotificationCenter.DefaultCenter().PostNotification(this, "RanSpinCoroutine");
        Invoke("Close", 0.5f);
    }
    public void SoundBtnClick()
    {
        AudioManager.instance.Play("ButtonClick");
    }

    public void NextPageBtn()
    {
        int lastPageIndex = pageSkin.Length - 1;
        SoundBtnClick();

        if (currentPage == lastPageIndex)
        {
            currentPage = 0;
            LoadPage();
        }
        else
        {
            currentPage += 1;
            LoadPage();
        }

    }
    public void PrePageBtn()
    {
        int lastPageIndex = pageSkin.Length - 1;
        SoundBtnClick();

        if (currentPage == 0)
        {
            currentPage = lastPageIndex;
            LoadPage();
        }
        else
        {
            currentPage -= 1;
            LoadPage();
        }
    }
    void LoadPage()
    {
        for (int i = 0; i <= pageSkin.Length - 1; i++)
        {
            pageSkin[i].SetActive(false);
        }
        pageSkin[currentPage].SetActive(true);
        NotificationCenter.DefaultCenter().PostNotification(this, "LoadItems");
    }

    public void OnInit()
    {
        NotificationCenter.DefaultCenter().AddObserver(this, "ShowSkinSelect");
        NotificationCenter.DefaultCenter().AddObserver(this, "SetSkinBoxSelect");
        selectedSkin = UserData.Instance.GameData.currentSkin;
        selectedBox = BoxItemData.Instance.userSkinBoxData.currentBox;
       
        LoadData();
        ShowSkin(selectedSkin);
        BoxSelected(selectedBox);
    }
    void LoadData()
    {
        var skinData = new List<SkinItem>(SkinItemData.Instance.userSkinData.skinItems);
        var boxData = new List<BoxItem>(BoxItemData.Instance.userSkinBoxData.boxItems);

        for (int i = 0; i < uiSkinItems.Length; i++)
        {
            var uiSkinItem = uiSkinItems[i];
            var select = skinData[i].SkinObject.name == UserData.Instance.GameData.currentSkin.SkinObject.name;
            uiSkinItem.Init(skinData[i], SkinItemData.Instance.CheckIsExist(skinData[i]),select);
        }
        for (int i = 0; i < uiBoxItems.Length; i++)
        {
            var uiBoxItem = uiBoxItems[i];
            var select = boxData[i].BoxObject.name == BoxItemData.Instance.userSkinBoxData.currentBox.BoxObject.name;
            uiBoxItem.Init(boxData[i], BoxItemData.Instance.CheckIsExist(boxData[i]), select);
        }
    }
    #region Methor
    public void ShowSkinSelect(Notification receivedData)
    {
        var hashtable = receivedData.data as Hashtable;
        var skinItem = hashtable["0"] as SkinItem;
        LoadData();
        ShowSkin(skinItem);
        //todo
    }
    void ShowSkin(SkinItem skinItem)
    {
        foreach (RectTransform rect in skeletonGraphicParents)
        {
            rect.gameObject.SetActive(false);
        }
        GetItemSkeleton(skinItem.SkinObject.name);
        this.selectedSkin = skinItem;     
        if (! this.selectedSkin.IsOpen)
        {
            btnBuy.SetActive(true);
            if (selectedSkin.SkinObject.skinType == SkinType.Coin)
            {
                BtnTypeCoinRand();
                coinText.text = SkinItemData.Instance.userSkinData.CoinPrize.ToString();
            }
            else if (selectedSkin.SkinObject.skinType == SkinType.VideoADS)
            {
                BtnTypeADS();
                adsText.text = string.Format("{0}/{1}", selectedSkin.current, selectedSkin.SkinObject.value);
            }
            else if (selectedSkin.SkinObject.skinType == SkinType.RubyADS)
            {
                BtnTypeRubyADS();
                adsText.text = string.Format("{0}/{1}", selectedSkin.current, selectedSkin.SkinObject.value);
            }
        }
        else
        {
            btnBuy.SetActive(false);
            PanelAds.SetActive(false);
        }
    }

    /// <summary>
    /// SelectBoxItem
    /// </summary>
    /// <param name="boxItem"></param>
    public void SetSkinBoxSelect(Notification receivedData)
    {
        var hashtable = receivedData.data as Hashtable;
        var boxItem = hashtable["0"] as BoxItem;
        LoadData();
        BoxSelected(boxItem);
        Debug.Log("--------------" + boxItem.BoxObject.nameType);
    }

    void BoxSelected(BoxItem boxItem)
    {
        this.selectedBox = boxItem;
        if (!this.selectedBox.IsOpen)
        {
            btnBuy.SetActive(true);
            if (selectedBox.BoxObject.boxType == BoxType.Coin)
            {
                BtnTypeCoin();
                coinText.text = BoxItemData.Instance.userSkinBoxData.CoinPrize.ToString();
            }
            else if (selectedBox.BoxObject.boxType == BoxType.VideoADS)
            {
                BtnTypeBoxADS();
                adsText.text = string.Format("{0}/{1}", selectedBox.current, selectedBox.BoxObject.value);
            }
        }
        else
        {
            btnBuy.SetActive(false);
            PanelAds.SetActive(false);
        }

    }
    void BtnTypeCoinRand()
    {
        btnAds.SetActive(false);
        PanelAds.SetActive(false);
        btnCoinRand.SetActive(true);
        btnBoxCoin.SetActive(false);
        btnBoxAds.SetActive(false);
        btnRubyAds.SetActive(false);
    }
    void BtnTypeADS()
    {
        PanelAds.SetActive(true);
        btnAds.SetActive(true);
        adsText.gameObject.SetActive(true);
        btnCoinRand.SetActive(false);
        btnBoxCoin.SetActive(false);
        btnBoxAds.SetActive(false);
        btnRubyAds.SetActive(false);
    }
    void BtnTypeBoxADS()
    {
        PanelAds.SetActive(true);
        btnAds.SetActive(false);
        adsText.gameObject.SetActive(true);
        btnCoinRand.SetActive(false);
        btnBoxCoin.SetActive(false);
        btnBoxAds.SetActive(true);
        btnRubyAds.SetActive(false);
    }
    void BtnTypeCoin()
    {
        btnAds.SetActive(false);
        PanelAds.SetActive(false);
        btnCoinRand.SetActive(false);
        btnBoxCoin.SetActive(true);
        btnBoxAds.SetActive(false);
        btnRubyAds.SetActive(false);
    }
    void BtnTypeRubyADS()
    {
        PanelAds.SetActive(true);
        btnAds.SetActive(false);
        btnRubyAds.SetActive(true);
        adsText.gameObject.SetActive(true);
        btnCoinRand.SetActive(false);
        btnBoxCoin.SetActive(false);
        btnBoxAds.SetActive(false);
    }
#endregion

    #region Listener
    public void OnClickBuySkinListener()
    {
        AudioManager.instance.Play("ButtonClick");
        if (selectedSkin.SkinObject.skinType == SkinType.Coin)
        {
            if (UserData.Instance.GameData.coin >= SkinItemData.Instance.userSkinData.CoinPrize)
            {
                SkinItem skinItem= SkinItemData.Instance.AddSkinRnd();
                UserData.Instance.GameData.coin -= SkinItemData.Instance.userSkinData.CoinPrize;
                NotificationCenter.DefaultCenter().PostNotification(this, "UpdateCoin");
                VkAdjustTracker.TrackResourceLose("coin", SkinItemData.Instance.userSkinData.CoinPrize, UserData.Instance.GameData.coin, "buy_skin");
                VkAdjustTracker.TrackFeatureSkinShopBuySkin(selectedSkin.SkinObject.nameType, "random_coin",-1);
                SkinItemData.Instance.userSkinData.CoinPrize += 2000;
                SkinItemData.Instance.SaveSkinData();
                LoadData();
                ShowSkin(selectedSkin);
                CID_Object[] param = new CID_Object[] { new CID_Object {itemType=ItemType.Skin,Value=1,skinType=skinItem.SkinObject.name } };
                var uiReward = VKLayerController.Instance.ShowLayer("UIPopupNewSkin") as UIPopupNewSkin;
                uiReward.Init(param, true, true);
            }
            else
            {
                VKLayerController.Instance.ShowLayer("UIPopupGetMoreCoin");
            }
        }
        else if (selectedSkin.SkinObject.skinType == SkinType.RubyADS)
        {
            if (UserData.Instance.GameData.ruby >= SkinItemData.Instance.userSkinData.RubyPrize)
            {
                SkinItem skinItem = selectedSkin;
                UserData.Instance.GameData.ruby -= SkinItemData.Instance.userSkinData.RubyPrize;
                NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");
                //VkAdjustTracker.TrackResourceLose("ruby", SkinItemData.Instance.userSkinData.RubyPrize, UserData.Instance.GameData.ruby, "buy_skin");
                //VkAdjustTracker.TrackFeatureSkinShopBuySkin(selectedSkin.SkinObject.nameType, "Buy_ruby", -1);
                SkinItemData.Instance.SetSkin(selectedSkin);
                LoadData();
                ShowSkin(selectedSkin);
                CID_Object[] param = new CID_Object[] { new CID_Object { itemType = ItemType.Skin, Value = 1, skinType = skinItem.SkinObject.name } };
                var uiReward = VKLayerController.Instance.ShowLayer("UIPopupNewSkin") as UIPopupNewSkin;
                uiReward.Init(param, true, true);
            }
            else
            {
                VKLayerController.Instance.ShowLayer("UIPopupGetMoreRuby");
            }
        }
    }
    public void OnClickBuyBoxListener()
    {
        ////////// Boxx ///////////
        if (selectedBox.BoxObject.boxType == BoxType.Coin)
        {
            if (UserData.Instance.GameData.coin >= BoxItemData.Instance.userSkinBoxData.CoinPrize)
            {
                BoxItem boxItem = selectedBox;
                UserData.Instance.GameData.coin -= BoxItemData.Instance.userSkinBoxData.CoinPrize;
                NotificationCenter.DefaultCenter().PostNotification(this, "UpdateCoin");
                BoxItemData.Instance.SetSkinBox(boxItem);
                LoadData();
                CID_Object[] param = new CID_Object[] { new CID_Object { itemType = ItemType.box, Value = 1, boxType = selectedBox.BoxObject.name } };
                var uiReward = VKLayerController.Instance.ShowLayer("UIPopupNewSkin") as UIPopupNewSkin;
                uiReward.Init(param, true, true);
                LimitAds.Instance.ADS_SKIN = 0;
            }
            else
            {
                VKLayerController.Instance.ShowLayer("UIPopupGetMoreCoin");
            }
        }
    }
    /// <summary>
    /// AdsListener
    /// </summary>
    public void OnClickBuySkinAdsListener()
    {
        AudioManager.instance.Play("ButtonClick");
        if (selectedSkin.SkinObject.skinType == SkinType.VideoADS || selectedSkin.SkinObject.skinType == SkinType.RubyADS)
        {          
            AdsManager.Instance.ShowRewardedAd(() => {
                LimitAds.Instance.ADS_SKIN++;
                AudioManager.instance.Play("CollectRuby");
                bool result = SkinItemData.Instance.AddAdsSkin(selectedSkin);
                LoadData();
                ShowSkin(selectedSkin);
                UserData.Instance.GameData.ruby += 1;
                UserData.Instance.SaveLocalData();
                NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");
                VkAdjustTracker.TrackResourceGain("ruby", 1, UserData.Instance.GameData.ruby, "ads_skin");
                VkAdjustTracker.TrackFeatureSkinShopBuySkin(selectedSkin.SkinObject.nameType, "video_coin", selectedSkin.current);
                if (result)
                {
                    CID_Object[] param = new CID_Object[] { new CID_Object { itemType = ItemType.Skin, Value = 1, skinType = selectedSkin.SkinObject.name } };
                    var uiReward = VKLayerController.Instance.ShowLayer("UIPopupNewSkin") as UIPopupNewSkin;
                    uiReward.Init(param, true, true);
                    LimitAds.Instance.ADS_SKIN = 0;
                }
            });         
        }
    }
    public void OnClickBuyBoxADSListener()
    {
        //////// Boxx ////////////
        if (selectedBox.BoxObject.boxType == BoxType.VideoADS)
        {
            AdsManager.Instance.ShowRewardedAd(() => {
                LimitAds.Instance.ADS_SKIN++;
                UserData.Instance.GameData.ruby += 1;
                AudioManager.instance.Play("CollectRuby");
                NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");
                bool result = BoxItemData.Instance.AddAdsSkinBox(selectedBox);
                LoadData();
                BoxSelected(selectedBox);
                UserData.Instance.SaveLocalData();
                if (result)
                {
                    CID_Object[] param = new CID_Object[] { new CID_Object { itemType = ItemType.box, Value = 1, boxType = selectedBox.BoxObject.name } };
                    var uiReward = VKLayerController.Instance.ShowLayer("UIPopupNewSkin") as UIPopupNewSkin;
                    uiReward.Init(param, true, true);
                    LimitAds.Instance.ADS_SKIN = 0;
                }
            });
        }
    }
    #endregion
    public RectTransform GetItemSkeleton(string name)
    {
        foreach(RectTransform tr in skeletonGraphicParents)
        {
            if(tr.gameObject.name==name)
            {
                tr.gameObject.SetActive(true);
                return tr;
            }
        }
        var go = ContainAssistant.Instance.GetItem<RectTransform>(name);
        go.SetParent(skeletonGraphicParents,false);
        go.anchoredPosition = Vector2.down*(140f);
        go.name = name;
        go.localScale = Vector2.one * 0.8f;
        return go;      
    }
    void BuySuccess()
    {
       
    }
}
