using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[CreateAssetMenu(fileName = "DailyInitData", menuName = "GameData/DailyData")]
public class DailyInitData : ScriptableObject
{
    private static DailyInitData instance;
    public static DailyInitData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<DailyInitData>("DailyInitData");
            }
            return instance;
        }
    }
    public List<DailyItem> DailyItems;
    public List<DailyItem> GetDailyItems()
    {
        return DailyItems;
    }
    public DailyItem GetDailyItemByIndex(int iIndex)
    {
        return DailyItems[iIndex];
    }
    
}
[Serializable]
public class DailyItem
{
    public int day;
    public ID_Object[] id_object;
}
[Serializable]
public class ID_Object
{
    public ItemType itemType;
    public int Value;
}
public class CID_Object
{
    public ItemType itemType;
    public int Value;
    public string skinType;
    public string boxType;

}
public enum ItemType
{
    Ruby=0,Coin=1,Skin=2,box=3
}