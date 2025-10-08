using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[CreateAssetMenu(fileName = "InitData", menuName = "GameData/InitData")]
public class InitData : ScriptableObject
{
    private static InitData instance;
    public static InitData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<InitData>("InitData");
            }
            return instance;
        }
    }
    public List<WORLD> WORLDS;
    public List<WORLD> GetWorldData()
    {
        return WORLDS;
    }
    
    public WORLD GetWorldDataByWorld(int world)
    {
        return WORLDS.Where(w => w.world == world).First();
    }
    public LevelData GetLevelData(int lv, int world)
    {
        var WorldData = GetWorldDataByWorld(world);
        if (WorldData != null)
        {
            return WorldData.ListOfLevels.Where(leveldata => leveldata.level == lv).First();
        }
        else
        {
            return null;
        }
    }

}