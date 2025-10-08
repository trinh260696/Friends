using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkinItem : MonoBehaviour
{
    public Image skin;
    public GameObject isLocked;
    public SkinItem cSkinItem;
    public GameObject selectObj;
    public int iIndex;
    public void Unlock(string name)
    {
        if (cSkinItem.SkinObject.name == name)
        {
            isLocked.SetActive(false);
        }
    }
    public void Init(SkinItem skinItem, bool isColected = false, bool isSelected=false)
    {
        this.cSkinItem = skinItem;

        skin.sprite = Resources.Load<Sprite>("Avatar/" + skinItem.SkinObject.name);
        skin.SetNativeSize();
        skin.rectTransform.localScale = Vector2.one * 0.3f;
       
        isLocked.SetActive(!isColected);
        selectObj.SetActive(isSelected);
    }
    public void OnClickBuyListener()
    {
        AudioManager.instance.Play("ButtonClick");

        if (this.cSkinItem.IsOpen)
        {
            selectObj.SetActive(true);
            UserData.Instance.OnSelectSkin(cSkinItem);
            NotificationCenter.DefaultCenter().PostNotification(this, "ChangeSkin");
        }
        var hashtable = new Hashtable();
        hashtable.Add("0", cSkinItem);
        NotificationCenter.DefaultCenter().PostNotification(this, "ShowSkinSelect", hashtable);
        NotificationCenter.DefaultCenter().PostNotification(this, "RanSpinCoroutine");
        VkAdjustTracker.TrackFeatureSkinShopViewSkin(cSkinItem.SkinObject.nameType);
    }
   
}
