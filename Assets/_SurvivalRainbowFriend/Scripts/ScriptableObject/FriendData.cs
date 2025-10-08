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
    public List<FriendObj> Friends;
    public List<FriendObj> GetFriendData()
    {
        return Friends;
    }
    public FriendObj GetFriendByskinName(string skinName, string skinCover="")
    {
        return Friends.Where(f => f.skinId == skinName).First();
    }
    public List<FriendObj> GetGroupFriend(int number)
    {
        List<FriendObj> result = new List<FriendObj>();
        int rnd = Random.Range(0, Friends.Count);
        for (int i=0; i<number; i++)
        {
            
            result.Add(Friends[(rnd+i)%Friends.Count]);
        }
        return result;
    }
}