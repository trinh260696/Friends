using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMainUI : MonoBehaviour
{
    UiManager uiManager;
    AudioManager audioManager;

    private void Start()
    {
        uiManager = UiManager.instance;
        audioManager = AudioManager.instance;
    }
    public void SettingBtnClick()
    {
        GameObject SettingUI = Resources.Load<GameObject>($"UI/Layer/SettingUI");
        uiManager.SettingUI = Instantiate(SettingUI, transform.position, Quaternion.identity);
        uiManager.SettingUI.SetActive(true);
        audioManager.Play("ButtonClick");
    }
    public void SpinBtnClick()
    {
        GameObject SpinUI = Resources.Load<GameObject>($"UI/Layer/SpinUI");
        uiManager.SpinUI = Instantiate(SpinUI, transform.position, Quaternion.identity);
        uiManager.SpinUI.SetActive(true);
        audioManager.Play("ButtonClick");
    }
    public void SkinBtnClick()
    {
        GameObject SkinUI = Resources.Load<GameObject>($"UI/Layer/SkinUI");
        uiManager.SkinUI = Instantiate(SkinUI, transform.position, Quaternion.identity);
        uiManager.SkinUI.SetActive(true);
        audioManager.Play("ButtonClick");
    }
}
