using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SkinItemData : MonoBehaviour
{
    public static SkinItemData Instance;
    public UserSkinData userSkinData;
    private List<SkinItem> skinItems;

    private void Start()
    {
        Instance = this;
        LoadSkinData();
    }
    public void LoadSkinData()
    {
        string jsonString = PlayerPrefs.GetString("SKINDATA", "");
        if(jsonString == "")
        {
            userSkinData = new UserSkinData();
            userSkinData.skinItems = new List<SkinItem>(SkinInitData.Instance.GetSkinItems());
            userSkinData.CoinPrize = 1000;
            userSkinData.RubyPrize = 50;
        }
        else
        {
            userSkinData = JsonUtility.FromJson<UserSkinData>(jsonString);
            // Khoi tao data cu
            skinItems = userSkinData.skinItems.ToList();                        // copy sang list moi       // 12 element
            int listCount = userSkinData.skinItems.Count;                       // listCount = 12
            int coinPrize = userSkinData.CoinPrize;

            // Data Update
            userSkinData = new UserSkinData();                                  // tao lai userSkinData     

            userSkinData.skinItems = new List<SkinItem>(SkinInitData.Instance.GetSkinItems());              // 16 element
            userSkinData.skinItems.RemoveRange(0, listCount);                   // 16 - 12

            skinItems.AddRange(userSkinData.skinItems);                         // 12 + 4
            userSkinData.skinItems = skinItems.ToList();

            userSkinData.CoinPrize = coinPrize;
            userSkinData.RubyPrize = 50;
        }
        SaveSkinData();
    }
    public SkinItem AddSkinRnd()
    {
        var unlockList = userSkinData.skinItems.Where(skin => skin.SkinObject.skinType == SkinType.Coin && !skin.IsOpen).ToList();
        if (unlockList.Count == 0) return null;
        var rnd = UnityEngine.Random.Range(0, unlockList.Count);
        var skinItem = unlockList[rnd];

        for (int i=0; i<userSkinData.skinItems.Count; i++)
        {
            if (userSkinData.skinItems[i].SkinObject.name == skinItem.SkinObject.name)
            {
                userSkinData.skinItems[i].IsOpen = true;
                skinItem = userSkinData.skinItems[i];
                break;
            }
        }
        SaveSkinData();
        return skinItem;
    }
    public SkinItem GetSkinRnd()
    {
        var unlockList = userSkinData.skinItems.Where(skin => skin.SkinObject.skinType == SkinType.Coin && !skin.IsOpen).ToList();
        if (unlockList.Count == 0) return null;
        var rnd = UnityEngine.Random.Range(0, unlockList.Count);
        var skinItem = unlockList[rnd];
       
        for (int i = 0; i < userSkinData.skinItems.Count; i++)
        {
            if (userSkinData.skinItems[i].SkinObject.name == skinItem.SkinObject.name)
            {
                userSkinData.skinItems[i].IsOpen = true;
                skinItem = userSkinData.skinItems[i];
                break;
            }
        }
        return skinItem;
    }
    public SkinItem GetSkinRndNotSet()
    {
        var unlockList = userSkinData.skinItems.Where(skin => skin.SkinObject.skinType == SkinType.Coin && !skin.IsOpen).ToList();
        if (unlockList.Count == 0) return null;
        var rnd = UnityEngine.Random.Range(0, unlockList.Count);
        var skinItem = unlockList[rnd];

        for (int i = 0; i < userSkinData.skinItems.Count; i++)
        {
            if (userSkinData.skinItems[i].SkinObject.name == skinItem.SkinObject.name)
            {
                
                skinItem = userSkinData.skinItems[i];
                break;
            }
        }
        return skinItem;
    }
    public void SetSkin(SkinItem skinItem)
    {
        for (int i = 0; i < userSkinData.skinItems.Count; i++)
        {
            if (userSkinData.skinItems[i].SkinObject.name == skinItem.SkinObject.name)
            {
                userSkinData.skinItems[i].IsOpen = true;
                break;
            }
        }
        SaveSkinData();
    }
    public bool AddAdsSkin(SkinItem skinItem)
    {
        for (int i = 0; i < userSkinData.skinItems.Count; i++)
        {
            if (userSkinData.skinItems[i].SkinObject.name == skinItem.SkinObject.name)
            {
                userSkinData.skinItems[i].current++;
                if (userSkinData.skinItems[i].current >= userSkinData.skinItems[i].SkinObject.value)
                {
                    userSkinData.skinItems[i].IsOpen = true;
                    SaveSkinData();
                    return true;
                }
                break;
            }
        }
        SaveSkinData();
        return false;
    }
    public int GetIndexOfSkin(SkinItem skinItem)
    {
        return userSkinData.skinItems.IndexOf(skinItem);
    }
    public SkinItem[] GetFilterSkin( )
    {
         return userSkinData.skinItems.Where(skin => !skin.IsOpen).ToArray();
        
    }
    public bool CheckIsExist(SkinItem skinItem)
    {
        return userSkinData.skinItems.Where(skin => skin.SkinObject.name == skinItem.SkinObject.name && skin.IsOpen).Count() > 0;
    }
    public void SaveSkinData()
    {
        string jsonString = JsonUtility.ToJson(userSkinData);
        PlayerPrefs.SetString("SKINDATA", jsonString);
    }
}
[Serializable]
public class UserSkinData
{
    public List<SkinItem> skinItems;
    public int CoinPrize;
    public int RubyPrize;
}
