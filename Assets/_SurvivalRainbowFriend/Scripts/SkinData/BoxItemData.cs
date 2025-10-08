using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoxItemData : MonoBehaviour
{
    public static BoxItemData Instance;
    public UserSkinBoxData userSkinBoxData;
    private List<BoxItem> boxItems;

    private void Start()
    {
        Instance = this;
        LoadSkinBoxData();
    }
    public void LoadSkinBoxData()
    {
        string jsonString = PlayerPrefs.GetString("SKINBOXDATA", "");
        if (jsonString == "")
        {
            userSkinBoxData = new UserSkinBoxData();
            userSkinBoxData.boxItems = new List<BoxItem>(SkinBoxInitData.Instance.GetBoxItems());
            userSkinBoxData.CoinPrize = 2500;
            userSkinBoxData.currentBox = SkinBoxInitData.Instance.GetBoxDefault();
            //userSkinBoxData.boxSpineName = UserData.Instance.GameData.currentBox.BoxObject.nameType;
        }
        else
        {
            userSkinBoxData = JsonUtility.FromJson<UserSkinBoxData>(jsonString);
            //boxItems = userSkinBoxData.boxItems.ToList();                        // copy sang list moi       // 12 element
            //int listCount = userSkinBoxData.boxItems.Count;                       // listCount = 12
            //int coinPrize = userSkinBoxData.CoinPrize;

            //// Data Update
            //userSkinBoxData = new UserSkinBoxData();                                  // tao lai userSkinData     

            //userSkinBoxData.boxItems = new List<BoxItem>(SkinBoxInitData.Instance.GetBoxItems());              // 16 element
            //userSkinBoxData.boxItems.RemoveRange(0, listCount);                   // 16 - 12

            //boxItems.AddRange(userSkinBoxData.boxItems);                         // 12 + 4
            //userSkinBoxData.boxItems = boxItems.ToList();

            //userSkinBoxData.CoinPrize = coinPrize;
        }
        SaveSkinBoxData();
    }

    public void OnSelectBox(BoxItem cBoxItem)
    {
        userSkinBoxData.currentBox = cBoxItem;
        SaveSkinBoxData();
    }
    public BoxItem AddSkinBoxRnd()
    {
        var unlockList = userSkinBoxData.boxItems.Where(box => box.BoxObject.boxType == BoxType.Coin && !box.IsOpen).ToList();
        if (unlockList.Count == 0) return null;
        var rnd = UnityEngine.Random.Range(0, unlockList.Count);
        var boxItem = unlockList[rnd];

        for (int i = 0; i < userSkinBoxData.boxItems.Count; i++)
        {
            if (userSkinBoxData.boxItems[i].BoxObject.name == boxItem.BoxObject.name)
            {
                userSkinBoxData.boxItems[i].IsOpen = true;
                boxItem = userSkinBoxData.boxItems[i];
                break;
            }
        }
        SaveSkinBoxData();
        return boxItem;
    }
    public BoxItem GetSkinBoxRnd()
    {
        var unlockList = userSkinBoxData.boxItems.Where(box => box.BoxObject.boxType == BoxType.Coin && !box.IsOpen).ToList();
        if (unlockList.Count == 0) return null;
        var rnd = UnityEngine.Random.Range(0, unlockList.Count);
        var boxItem = unlockList[rnd];

        for (int i = 0; i < userSkinBoxData.boxItems.Count; i++)
        {
            if (userSkinBoxData.boxItems[i].BoxObject.name == boxItem.BoxObject.name)
            {
                userSkinBoxData.boxItems[i].IsOpen = true;
                boxItem = userSkinBoxData.boxItems[i];
                break;
            }
        }
        return boxItem;
    }
    public BoxItem GetSkinBoxRndNotSet()
    {
        var unlockList = userSkinBoxData.boxItems.Where(box => box.BoxObject.boxType == BoxType.Coin && !box.IsOpen).ToList();
        if (unlockList.Count == 0) return null;
        var rnd = UnityEngine.Random.Range(0, unlockList.Count);
        var boxItem = unlockList[rnd];

        for (int i = 0; i < userSkinBoxData.boxItems.Count; i++)
        {
            if (userSkinBoxData.boxItems[i].BoxObject.name == boxItem.BoxObject.name)
            {

                boxItem = userSkinBoxData.boxItems[i];
                break;
            }
        }
        return boxItem;
    }
    public void SetSkinBox(BoxItem boxItem)
    {
        for (int i = 0; i < userSkinBoxData.boxItems.Count; i++)
        {
            if (userSkinBoxData.boxItems[i].BoxObject.name == boxItem.BoxObject.name)
            {
                userSkinBoxData.boxItems[i].IsOpen = true;
                break;
            }
        }
        SaveSkinBoxData();
    }
    public bool AddAdsSkinBox(BoxItem boxItem)
    {
        for (int i = 0; i < userSkinBoxData.boxItems.Count; i++)
        {
            if (userSkinBoxData.boxItems[i].BoxObject.name == boxItem.BoxObject.name)
            {
                userSkinBoxData.boxItems[i].current++;
                if (userSkinBoxData.boxItems[i].current >= userSkinBoxData.boxItems[i].BoxObject.value)
                {
                    userSkinBoxData.boxItems[i].IsOpen = true;
                    SaveSkinBoxData();
                    return true;
                }
                break;
            }
        }
        SaveSkinBoxData();
        return false;

    }
    public int GetIndexOfSkinBox(BoxItem boxItem)
    {
        return userSkinBoxData.boxItems.IndexOf(boxItem);
    }
    public BoxItem[] GetFilterBox()
    {
        return userSkinBoxData.boxItems.Where(skin => !skin.IsOpen).ToArray();

    }
    public bool CheckIsExist(BoxItem boxItem)
    {
        return userSkinBoxData.boxItems.Where(box => box.BoxObject.name == boxItem.BoxObject.name && box.IsOpen).Count() > 0;
    }
    public void SaveSkinBoxData()
    {
        string jsonString = JsonUtility.ToJson(userSkinBoxData);
        PlayerPrefs.SetString("SKINBOXDATA", jsonString);
    }
}
[Serializable]
public class UserSkinBoxData
{
    public List<BoxItem> boxItems;
    public BoxItem currentBox;
    public int CoinPrize;
    
    //public string boxSpineName;
}