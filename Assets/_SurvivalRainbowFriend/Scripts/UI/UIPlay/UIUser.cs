using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIUser : MonoBehaviour
{
    
    [SerializeField] Image imgIcon;
    [SerializeField] TextMeshProUGUI textNumber;
    [SerializeField] GameObject xObject;
    int number = 0;
    public void InitStatusUser()
    {
       
        this.imgIcon.sprite = Resources.Load<Sprite>("Avatar/" + UserData.Instance.GameData.currentSkin.SkinObject.name);
        this.textNumber.text = "0";
        xObject.SetActive(false);
    }
    public void ClearUser()
    {
        xObject.SetActive(true);

    }
    public void AddItem()
    {
        number++;
        this.textNumber.text = number.ToString();
    }
    public void Revive()
    {
        xObject.SetActive(false);
    }
}
