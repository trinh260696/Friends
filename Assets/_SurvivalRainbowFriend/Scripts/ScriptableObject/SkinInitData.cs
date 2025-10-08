using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinInitData", menuName = "GameData/SkinData")]
public class SkinInitData : ScriptableObject
{
    private static SkinInitData instance;
    public static SkinInitData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<SkinInitData>("SkinInitData");
            }
            return instance;
        }
    }
    public List<SkinItem> SkinItems;
    public List<SkinItem> GetSkinItems()
    {
        return SkinItems;
    }
    public SkinItem GetSkinDefault()
    {
        return SkinItems.Where(sk => sk.isDefauseSkin).First();
    }
    public string GetSkinName(string skinType)
    {
        return SkinItems.Where(sk => sk.SkinObject.name == skinType).First().SkinObject.nameType;
    }
}

[Serializable]
public class SkinItem
{
    public ID_SkinObject SkinObject;
    public bool isDefauseSkin;
    public int current;
    public bool IsOpen;
}
[Serializable]
public class ID_SkinObject
{
    public SkinType skinType;
    public int value;
    public string name;
    public string nameType;
}
public enum SkinType
{
    VideoADS=0, Coin=1, Default=2, RubyADS=3
}