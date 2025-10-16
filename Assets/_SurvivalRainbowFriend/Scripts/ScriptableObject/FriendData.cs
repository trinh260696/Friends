using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[CreateAssetMenu(fileName = "FriendData", menuName = "GameData/Friend Data")]
public class FriendData : ScriptableObject
{
    private static FriendData instance;
    public static FriendData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<FriendData>("FriendData");
            }
            return instance;
        }
    }
    public List<NPCObj> Friends;
    public List<NPCObj> GetFriendData()
    {
        return Friends;
    }
   
    public List<NPCObj> GetGroupFriend(int number)
    {
        List<NPCObj> result = new List<NPCObj>();
        int rnd = Random.Range(0, Friends.Count);
        for (int i=0; i<number; i++)
        {
            
            result.Add(Friends[(rnd+i)%Friends.Count]);
        }
        return result;
    }
}