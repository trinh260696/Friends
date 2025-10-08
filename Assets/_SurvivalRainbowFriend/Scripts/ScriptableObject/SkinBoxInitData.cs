using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinBoxInitData", menuName = "GameData/SkinBoxData")]
public class SkinBoxInitData : ScriptableObject
{
    private static SkinBoxInitData instance;
    public static SkinBoxInitData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<SkinBoxInitData>("SkinBoxInitData");
            }
            return instance;
        }
    }
    public List<BoxItem> BoxItems;
    public List<BoxItem> GetBoxItems()
    {
        return BoxItems;
    }
    public BoxItem GetBoxDefault()
    {
        return BoxItems.Where(box => box.isDefauseBox).First();
    }
    public string GetBoxName(string boxType)
    {
        return BoxItems.Where(box => box.BoxObject.name == boxType).First().BoxObject.nameType;
    }
}

[Serializable]
public class BoxItem
{
    public ID_BoxObject BoxObject;
    public bool isDefauseBox;
    public int current;
    public bool IsOpen;
}
[Serializable]
public class ID_BoxObject
{
    public BoxType boxType;
    public int value;
    public string name;
    public string nameType;
    public string nameVariable;
}
public enum BoxType
{
    VideoADS = 0, Coin = 1, Default = 2
}