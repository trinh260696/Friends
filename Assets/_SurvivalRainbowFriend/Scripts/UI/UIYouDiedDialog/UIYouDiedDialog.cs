using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VKSdk.UI;
using TMPro;
public class UIYouDiedDialog : VKLayer
{
    #region properties
    [SerializeField] private UIItemDie[] uiItemDies;
    [SerializeField] private Transform uiDialog;
    [SerializeField] private Transform uiJumScare;
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
    }

    public override void StartLayer()
    {
        base.StartLayer();
    }

    #endregion
   
    public void Init(List<ItemObject> StatusMissions, List<ItemObject> TotalMissions)
    {
        StartCoroutine(ShowJumScare(StatusMissions, TotalMissions));
    }

    IEnumerator ShowJumScare(List<ItemObject> StatusMissions, List<ItemObject> TotalMissions)
    {
        uiDialog.gameObject.SetActive(false);
        uiJumScare.gameObject.SetActive(true);
        foreach (Transform tr in uiJumScare)
        {
            tr.gameObject.SetActive(false);
        }
        if (StaticData.ENEMY_STRING == "")
        {
            StaticData.ENEMY_STRING = StaticData.ENEMY_ARR[GameManager.Instance.levelData.level - 1];
            uiJumScare.Find(StaticData.ENEMY_STRING).gameObject.SetActive(true);          
        }
        else
        {
            AudioManager.instance.Play(StaticData.ENEMY_STRING);
            uiJumScare.Find(StaticData.ENEMY_STRING).gameObject.SetActive(true);
        }
        StaticData.ENEMY_STRING = "";
        yield return new WaitForSeconds(3f);
        uiJumScare.gameObject.SetActive(false);
        uiDialog.gameObject.SetActive(true);
        for (int i = 0; i < uiItemDies.Length; i++)
        {
            if (i < StatusMissions.Count)
            {
                uiItemDies[i].gameObject.SetActive(true);
                uiItemDies[i].InitData(StatusMissions[i], TotalMissions[i]);
            }
            else
                uiItemDies[i].gameObject.SetActive(false);
        }
        UserData.Instance.UpdateLevelDie(StaticData.LEVEL);
        var levelObj = InitData.Instance.GetLevelData(StaticData.LEVEL, 1);
        VkAdjustTracker.TrackProgressFail("survival", levelObj.name, levelObj.level, (int)GameManager.Instance.totalTime);
    }
    public void OnClickContinue()
    {
        AudioManager.instance.Play("ButtonClick");
        int level = StaticData.LEVEL;
        LoadScene.Instance.LoadSceneAndLoading("Level_" + level);      
        Close();
        AdsManager.Instance.ShowInterstitial();
    }
    public void OnClickClose()
    {
        AudioManager.instance.Play("ButtonClick");
        
        LoadScene.Instance.LoadSceneAndLoading("MainStartUI");
        Close();
        AdsManager.Instance.ShowInterstitial();
    }

}
