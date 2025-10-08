using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIDailyItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textDay;
    [SerializeField] private Button btnItem;
    public GameObject objectCheck;
    public SubItem[] subItems;
    int Index;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Init(DailyItem dailyItem,int iIndex, bool isReceived=false, bool reach=false)
    {
        this.Index = iIndex;
        textDay.text = string.Format("DAY {0}", iIndex + 1);
        for(int i=0; i<dailyItem.id_object.Length; i++)
        {
            subItems[i].iconImg.sprite = Resources.Load<Sprite>("Item/" + dailyItem.id_object[i].itemType.ToString());
            if (dailyItem.id_object[i].itemType != ItemType.Skin)
            {
                subItems[i].textValue.text = "+"+dailyItem.id_object[i].Value.ToString();
            }
            btnItem.interactable = (reach) && (!isReceived);
        }
        objectCheck.SetActive(isReceived);
    }
    void Refresh()
    {
        objectCheck.SetActive(true);
        btnItem.interactable = false;
    }
    public void OnClickButtonListener()
    {
        DailyRewardData.Instance.ReceiveiDaily(Index);
        Refresh();
    }
}
