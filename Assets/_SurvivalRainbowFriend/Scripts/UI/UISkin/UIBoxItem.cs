using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBoxItem : MonoBehaviour
{
    public Image box;
    public GameObject isLocked;
    public BoxItem cBoxItem;
    public GameObject selectObj;
    public int iIndex;

    private void Start()
    {
        NotificationCenter.DefaultCenter().AddObserver(this, "GetNameBox");
    }

    public void Unlock(string name)
    {
        if (cBoxItem.BoxObject.name == name)
        {
            isLocked.SetActive(false);
        }
    }
    public void Init(BoxItem boxItem, bool isColected = false, bool isSelected = false)
    {
        this.cBoxItem = boxItem;
        box.sprite = Resources.Load<Sprite>("SkinBox/" + boxItem.BoxObject.name);
        box.SetNativeSize();
        box.rectTransform.localScale = Vector2.one * 0.3f;

        isLocked.SetActive(!isColected);
        selectObj.SetActive(isSelected);
    }
    public void OnClickBuyListener()
    {
        AudioManager.instance.Play("ButtonClick");
        UISkin.spineNameBox = cBoxItem.BoxObject.nameType;

        if (this.cBoxItem.IsOpen)
        {
            selectObj.SetActive(true);
            BoxItemData.Instance.OnSelectBox(cBoxItem);
            NotificationCenter.DefaultCenter().PostNotification(this, "ChangeBox");
        }
        var hashtable = new Hashtable();
        hashtable.Add("0", cBoxItem);
        NotificationCenter.DefaultCenter().PostNotification(this, "BoxSkinSelect");
        NotificationCenter.DefaultCenter().PostNotification(this, "SetSkinBoxSelect", hashtable);
        //VkAdjustTracker.TrackFeatureSkinShopViewSkin(cBoxItem.BoxObject.nameType);
    }

}
