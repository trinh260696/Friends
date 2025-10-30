using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFriendManager : MonoBehaviour
{
    public List<CFriendObj> FriendsList;
    public List<UIFriend> UIFriends;
    private void Awake()
    {
        FriendsList = new List<CFriendObj>();
        UIFriends = new List<UIFriend>();
    }
    public void InitData(List<CFriendObj> friends)
    {
        UIFriends.Clear();
        for(int i=0; i<friends.Count; i++)
        {
            var go = GetItem();
            var uiFriend=  go.GetComponent<UIFriend>();
            uiFriend.InitStatusFriend(friends[i]);
            UIFriends.Add(uiFriend);
        }
    }
    public void ClearFriend(int name)
    {
        for(int i=0; i<UIFriends.Count; i++)
        {
            if (UIFriends[i].cFriendObj.id == name)
            {
                UIFriends[i].ClearFriend();
                break;
            }
        }
    }
    public void UpdateFriend(int name)
    {
        for (int i = 0; i < UIFriends.Count; i++)
        {
            if (UIFriends[i].cFriendObj.id == name)
            {
                UIFriends[i].ClearItem();
                break;
            }
        }
    }
    public GameObject GetItem()
    {
        foreach(Transform tr in transform)
        {
            if (!tr.gameObject.activeSelf)
            {
                tr.gameObject.SetActive(true);
                return tr.gameObject;
            }
        }
        GameObject pref = Resources.Load<GameObject>("UI/UIPrefabs/UIFriend") as GameObject;
        GameObject go = Instantiate(pref, transform);
        return go;
    }
    public void Clear()
    {
        foreach(Transform tr in transform)
        {
            tr.gameObject.SetActive(false);
        }
    }
}
