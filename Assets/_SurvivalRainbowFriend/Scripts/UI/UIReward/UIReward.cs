using System.Collections;
using UnityEngine;
using VKSdk.UI;
using Spine;
using Spine.Unity;
using System.Collections.Generic;
using VKSdk;

public class UIReward : VKLayer
{
    #region Properties
    [SerializeField] UIItemReward[] uiItemRewards;
    [SerializeField] GameObject parentObject;
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
    void OnClose()
    {           
        Close();
    }
    #region Listener
    public void OnClickClaim()
    {
        AudioManager.instance.Play("ButtonClick");
        UserData.Instance.SaveLocalData();
        SkinItemData.Instance.SaveSkinData();
        Close();
        
    }
    #endregion

    #region Method
    public void Init(CID_Object[] giftObj, bool isRandom=false,bool isClaim=true )
    {
        btnClaim.SetActive(isClaim);
        foreach(Transform tr in parentObject.transform)
        {
            tr.gameObject.SetActive(false);
        }
        this.giftObj = new CID_Object[giftObj.Length];
        System.Array.Copy(giftObj, this.giftObj, giftObj.Length);
        
        StartCoroutine("ShowReward");
        if(!isClaim)
        Invoke("OnClose", 2f);
    }
    IEnumerator ShowReward()
    {
        yield return null;
        AudioManager.instance.Play("success");     
        for(int i=0; i< uiItemRewards.Length; i++)
        {
            if (i < giftObj.Length)
            {
                uiItemRewards[i].gameObject.SetActive(true);
                if (giftObj[i].itemType == ItemType.Coin || giftObj[i].itemType == ItemType.Ruby)
                {

                    uiItemRewards[i].nameText.text = string.Format("+ {0}", giftObj[i].Value);
                    uiItemRewards[i].imgIcon.sprite = Resources.Load<Sprite>("Item/" + giftObj[i].itemType.ToString());
                }
                else
                {
                    uiItemRewards[i].nameText.text = string.Format("+ {0}", SkinInitData.Instance.GetSkinName(giftObj[i].skinType));
                    uiItemRewards[i].imgIcon.sprite = Resources.Load<Sprite>("Avatar/" + giftObj[i].skinType);
                }
            }
        }  
    }
    #endregion
}
