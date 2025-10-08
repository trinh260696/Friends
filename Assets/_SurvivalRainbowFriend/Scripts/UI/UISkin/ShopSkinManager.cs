using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSkinManager : MonoBehaviour
{
    [SerializeField] private GameObject panelBoxPage;

    [SerializeField] private Button selectSkinBtn;
    [SerializeField] private Button unSelectSkinBtn;
    [SerializeField] private Button selectBoxBtn;
    [SerializeField] private Button unSelectBoxBtn;

    private bool isSelectedSkin, isSelectedBox;

    public void Start()
    {
        isSelectedSkin = true;
        isSelectedBox = false;
        SelectedSkin();
    }

    public void SelectedSkinBtn()
    {
        AudioManager.instance.Play("ButtonClick");
        NotificationCenter.DefaultCenter().PostNotification(this, "LoadItems");
        isSelectedSkin = true;
        SelectedSkin();
    }

    public void SelectedBoxBtn()
    {
        AudioManager.instance.Play("ButtonClick");
        NotificationCenter.DefaultCenter().PostNotification(this, "LoadItems");
        isSelectedBox = true;
        SelectedBox();
    }

    void SelectedSkin()
    {
        isSelectedBox = !isSelectedSkin;
        panelBoxPage.SetActive(isSelectedBox);

        selectSkinBtn.gameObject.SetActive(true);
        selectBoxBtn.gameObject.SetActive(false);
        unSelectSkinBtn.gameObject.SetActive(false);
        unSelectBoxBtn.gameObject.SetActive(true);
    }
    void SelectedBox()
    {
        isSelectedSkin = !isSelectedBox;
        panelBoxPage.SetActive(isSelectedBox);

        selectSkinBtn.gameObject.SetActive(false);
        selectBoxBtn.gameObject.SetActive(true);
        unSelectSkinBtn.gameObject.SetActive(true);
        unSelectBoxBtn.gameObject.SetActive(false);
    }
}
