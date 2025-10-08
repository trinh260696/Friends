using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIItemDie : MonoBehaviour
{
    [SerializeField] private Image iconImg;
    [SerializeField] private TextMeshProUGUI textValue;
    [SerializeField] private Button TickBtn;
    [SerializeField] private GameObject tickObj;

    public void InitData(ItemObject StatusMission, ItemObject TotalMission)
    {
        iconImg.sprite = Resources.Load<Sprite>("Avatar/" + StatusMission.typeMission.ToString());
        iconImg.SetNativeSize();
        textValue.text = string.Format("{0}/{1}", StatusMission.missionNumber, TotalMission.missionNumber);
        bool isComplete = StatusMission.missionNumber >= TotalMission.missionNumber;
        tickObj.SetActive(isComplete);
        TickBtn.interactable = isComplete;
        
    }
}
