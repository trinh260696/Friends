using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VKSdk;
using VKSdk.UI;

public class UIPopupNewSkin : VKLayer
{
    #region Properties
    [SerializeField] UIItemReward[] uiItemRewards;
    [SerializeField] GameObject parentObject;
    [SerializeField] GameObject panel;
    //[SerializeField] GameObject[] ItemTween;
    public GameObject btnClaim;
    private CID_Object[] giftObj;
    
    int monney;

    #endregion

    #region Implement
    public override void StartLayer()
    {
        base.StartLayer();

    }

    public override void ShowLayer()
    {
        base.ShowLayer();
        StartCoroutine("OnShowLayer");
    }

    public override void ReloadLayer()
    {
        base.ReloadLayer();
    }

    public override void EnableLayer()
    {
        base.EnableLayer();
    }

    public override void DisableLayer()
    {
        base.DisableLayer();
    }

    public override void HideLayer()
    {
        base.HideLayer();


    }
    #endregion

    #region Listener
    public void OnClickClaim()
    {
        AudioManager.instance.Play("ButtonClick");
        AudioManager.instance.Play("SuccessReward");
        UserData.Instance.SaveLocalData();
        SkinItemData.Instance.SaveSkinData();
        StartCoroutine("OnClose");
    }
    IEnumerator OnShowLayer()
    {
        AudioManager.instance.Play("UnlockAchievement");
        LeanTween.scale(panel, new Vector3(0, 0, 0), 0);
        LeanTween.scale(panel, new Vector3(1, 1, 1), 1).setEase(LeanTweenType.easeOutExpo);
        yield return new WaitForSeconds(1f);
        LeanTween.scale(btnClaim, new Vector3(0, 0, 0), 0);
        LeanTween.scale(btnClaim, new Vector3(1, 1, 1), 1).setEase(LeanTweenType.easeOutElastic);
    }
    IEnumerator OnClose()
    {
        LeanTween.scale(panel, new Vector3(0, 0, 0), 0.5f).setEase(LeanTweenType.easeInOutExpo);
        yield return new WaitForSeconds(0.5f);        
        Close();
    }   
    #endregion

    #region Method
    public void Init(CID_Object[] giftObj, bool isRandom = false, bool isClaim = true)
    {
        btnClaim.SetActive(isClaim);
        foreach (Transform tr in parentObject.transform)
        {
            tr.gameObject.SetActive(false);
        }
        this.giftObj = new CID_Object[giftObj.Length];
        System.Array.Copy(giftObj, this.giftObj, giftObj.Length);

        StartCoroutine("ShowReward");
        if (!isClaim)
            Invoke("Close", 2f);
    }
    IEnumerator ShowReward()
    {
        yield return null;
        //AudioManager.instance.Play("success");
        for (int i = 0; i < uiItemRewards.Length; i++)
        {
            if (i < giftObj.Length)
            {
                uiItemRewards[i].gameObject.SetActive(true);
                if (giftObj[i].itemType == ItemType.Coin || giftObj[i].itemType == ItemType.Ruby)
                {
                    uiItemRewards[i].nameText.text = string.Format("+ {0}", giftObj[i].Value);
                    uiItemRewards[i].imgIcon.sprite = Resources.Load<Sprite>("Item/" + giftObj[i].itemType.ToString());
                    uiItemRewards[i].imgIcon.rectTransform.sizeDelta = new Vector2(420, 420);
                }
                else if (giftObj[i].itemType == ItemType.box)
                {
                    uiItemRewards[i].nameText.gameObject.SetActive(false);
                   // uiItemRewards[i].nameText.text = string.Format("{0}", SkinBoxInitData.Instance.GetBoxName(giftObj[i].boxType)).Replace("_"," ");
                    uiItemRewards[i].imgIcon.sprite = Resources.Load<Sprite>("SkinBox/" + giftObj[i].boxType);
                    uiItemRewards[i].imgIcon.SetNativeSize();
                }
                else
                {
                    uiItemRewards[i].nameText.gameObject.SetActive(false);
                    // uiItemRewards[i].nameText.text = string.Format("{0}", SkinInitData.Instance.GetSkinName(giftObj[i].skinType));
                    uiItemRewards[i].imgIcon.sprite = Resources.Load<Sprite>("Avatar/" + giftObj[i].skinType);
                    uiItemRewards[i].imgIcon.SetNativeSize();
                }
            }
            else
            {
                uiItemRewards[i].gameObject.SetActive(false);
            }
        }
    }
    
    #endregion
}
