
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Purchasing;
using VKSdk;
using VKSdk.UI;
using Product = UnityEngine.Purchasing.Product;

public class SupportThisGame : MonoBehaviour
{
    public static SupportThisGame Instance;
    private bool IsBegin = false;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        float aspectRatio = Mathf.Max(Screen.width, Screen.height) / Mathf.Min(Screen.width, Screen.height);
        StaticData.IsIPAD = (aspectRatio <= 1.6f);
        StaticData.ADS_SCALE_RATIO = StaticData.IsIPAD ? 0.767f : 0.85f;
    } 
    private void Start()
    {
        UIYouSuvived.Proccess = PlayerPrefs.GetInt("Proccess", 0);
        //TurnOnCheck();
        StartCoroutine("ShowAds");
        // handle connection status here

    }
    private void Update()
    {
        StaticData.TIME_INTER += Time.deltaTime;
    }
    public void TurnOnCheck()
    {
        StartCoroutine(checkInternetConnection());
    }
    IEnumerator checkInternetConnection()
    {
        while (true)
        {
            WWW www = new WWW("http://google.com");
            yield return www;
            if (www.error != null)
            {
                VKLayerController.Instance.ShowLayer("UILoseConnection");
                yield break;
            }
            else
            {
                Debug.Log("Connect success");
            }
            yield return new WaitForSeconds(15f);
        }

       
    }
    IEnumerator ShowAds()
    {
        yield return null;
        if (UserData.Instance.GameData.vip == 1)
        {
            yield break;
        }
        AdsManager.Instance.ToggleBannerVisibility();
        if (PlayerPrefs.HasKey(StaticData.STR_BEGIN))
        {
            yield return new WaitForSeconds(5f);
            IsBegin = false;
            AdsManager.Instance.ShowOpenADS();
        }
        else
        {
            IsBegin = true;
            PlayerPrefs.SetString(StaticData.STR_BEGIN, "BEGIN");
            yield return null;
        }
    }


    #region IAP
    public void OnCompleteIAP(Product product)
    {
        VKLayerController.Instance.HideLoading();
        UserData.Instance.GameData.vip = 1;
       
        var quantity = 1;
        switch (product.definition.id)
        {
            case "com.grus.survivor.rainbow.monsters.removeads":
                    UIPopup.OpenPopup("NOTIFY", "Buy remove ads successfull! Remove Ads unlimited!", (isOK) => {                       
                    }, false);

                break;
            case "com.grus.survivor.rainbow.monsters.coin":
                UserData.Instance.GameData.coin += 7000*quantity;
                NotificationCenter.DefaultCenter().PostNotification(this, "UpdateCoin");
                VkAdjustTracker.TrackResourceGain("coin", 7000, UserData.Instance.GameData.coin, "iap");
                VkAdjustTracker.TrackFeatureGetMoreCoin("iap");
                LeanTweenGetMore.instance.OnTweenCoin();
                break;
            case "com.grus.survivor.rainbow.monsters.ruby":
                UserData.Instance.GameData.ruby += 20*quantity;
                NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");
                VkAdjustTracker.TrackResourceGain("ruby", 20, UserData.Instance.GameData.ruby, "iap");
                VkAdjustTracker.TrackFeatureGetMoreDiamond("iap");
                LeanTweenGetMore.instance.OnTweenRuby();
                break;
        }
        // Adjust

        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("product_name", product.metadata.localizedTitle);
        event_params.Add("product_id", product.definition.id);
        event_params.Add("price", (float)product.metadata.localizedPrice);
        event_params.Add("currency", product.metadata.isoCurrencyCode);
        VkAdjustTracker.AdjustTrack(AdjustCode.IAP_CODE, event_params);
        AdsManager.Instance.ToggleBannerVisibility();
        VKLayerController.Instance.ReloadCanvasScaler();
        NotificationCenter.DefaultCenter().PostNotification(this, "HideBanner");
        

        UserData.Instance.SaveLocalData();
    }
    public void OnPurchaseFailed(UnityEngine.Purchasing.Product product, PurchaseFailureReason failureReason)
    {
        VKDebug.Log("Fail: " + product.definition.storeSpecificId);

        VKLayerController.Instance.HideLoading();
        if (failureReason != PurchaseFailureReason.UserCancelled && failureReason != PurchaseFailureReason.Unknown)
        {
            UIPopup.OpenPopup("FAILED!", "You cancelled or internet connection error!", (isOK) => {
            }, false);
        }
    }
    #endregion
}
