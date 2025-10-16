using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIFriend : MonoBehaviour
{
    public CFriendObj cFriendObj;
    [SerializeField] Image imgIcon;
    [SerializeField] TextMeshProUGUI textNumber;
    [SerializeField] GameObject xObject;
    public void InitStatusFriend(CFriendObj friendObj)
    {
        this.cFriendObj = friendObj;
        this.imgIcon.sprite = Resources.Load<Sprite>("Avatar/" + cFriendObj.id.ToString());
        this.textNumber.text = friendObj.number.ToString();
        xObject.SetActive(false);
    }
    public void ClearFriend()
    {
        xObject.SetActive(true);
       
    }
    public void AddItem()
    {
        cFriendObj.number++;
        this.textNumber.text = cFriendObj.number.ToString();
    }
}
