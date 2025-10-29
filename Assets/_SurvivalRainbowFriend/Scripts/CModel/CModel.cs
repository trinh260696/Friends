using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SGameData
{
    public string name;
    public int coin;
    public int ruby;
    public int level;
    public int world;
    public int vip;
    public float TIME_SPIN_CURRENT;
    public int TIME_SPIN_FLAG;      
    public SkinItem currentSkin;
    
    public LevelObject[] SurviveLevels;
    public LevelObject[] HostileLevels;
    public string BoxKey;
    public SGameData()
    {
            
    }
    public void Init()
    {
        name = "Player111";
        coin = 0;
        ruby = 0;
        level = 1;
        world = 1;
        vip = 0;
        TIME_SPIN_CURRENT = 0;
        TIME_SPIN_FLAG = 0;
        currentSkin = SkinInitData.Instance.GetSkinDefault();              
        var tmp = InitData.Instance.GetWorldDataByWorld(1).ListOfLevels;
        SurviveLevels = System.Array.ConvertAll(tmp.ToArray(), lv => lv.ConvertToLevelObject());
        SurviveLevels[0].status = true;
        BoxKey = "carton";
    }
}  
[System.Serializable]
public class LevelData
{
    public string name;   
    public int level;
    public int friendNumber;
    public int WIDTH, HEIGHT;
    public float timeout;
    public float radiusLight;
    public LevelObject ConvertToLevelObject()
    {
        LevelObject result = new LevelObject();
        result.name = this.name;
        result.level = this.level;
        result.status = false;
        result.survive = 0;
        result.die = 0;
        return result;
    }
}
[System.Serializable]
public class ItemObject
{
    public TypeMission typeMission;
    public int missionNumber;
    public int missionMax;
}
[System.Serializable]
public class WORLD
{
    public List<LevelData> ListOfLevels;
    public int world;
}
[System.Serializable]
public enum TypeMission
{
    hop_cuu_thuong,the_tu,nangluong_blue, nangluong_cam, hom_vukhi,hom_dan,pin_green,pin_blue,pin_red
}
    
[Serializable]
public class NPCObj
{
    public FriendType friendType;
    public string name;
    public int IQ;
}
[Serializable]
public class SkinObj
{
    public string skin_name;
    public int IQ;
}
[Serializable]
public class CFriendObj
{
    public int id;
    public int number;
    
}
public enum FriendType
{
    Friend_0=0,Friend_1=1, Friend_2 = 2
}
[Serializable]
public class LevelObject
{
    public string name;
    public int level;   
    public bool status;
    public int survive;
    public int die;
}
[Serializable]
public enum Mode
{
    SurvivalMode=0,
    HostleMode=1,
    BattleMode=2
}



