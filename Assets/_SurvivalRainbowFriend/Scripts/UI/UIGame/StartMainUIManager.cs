using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMainUIManager : MonoBehaviour
{
    public GameObject panelChoseMode;
    public GameObject SurvivorMode;
    public GameObject HostileMode;
    public GameObject backBtn;
    public LevelSelector[] SurviveModeSelect;
    public LevelSelector[] HostleModeSelect;


    public void SoundBtnClick()
    {
        AudioManager.instance.Play("ButtonClick");
    }
    public void PanelChoseMode()
    {
        SoundBtnClick();
        panelChoseMode.SetActive(true);
       
        SurvivorMode.SetActive(false);
        backBtn.SetActive(false);
    }
    public void SelectSurvivorMode()
    {
        SoundBtnClick();
        backBtn.SetActive(true);
        panelChoseMode.SetActive(false);
        SurvivorMode.SetActive(true);
        for (int i = 0; i < SurviveModeSelect.Length; i++)
        {
            SurviveModeSelect[i].InitDatas(UserData.Instance.GameData.SurviveLevels[i]);
        }
    }
    public void SelectHostileMode()
    {
        SoundBtnClick();
        StaticData.LEVEL = 1;
        StaticData.GM_Mode = Mode.HostleMode;
        LoadScene.Instance.LoadSceneAndLoading("Hostile_Level_1");
        return;
        backBtn.SetActive(true);
        panelChoseMode.SetActive(false);
        HostileMode.SetActive(true);
    }
    public void SelectBattleMode()
    {
        SoundBtnClick();
        UIPopup.OpenPopup("Coming soon", " We are updating!", false);
        return;
    }
    public void BackBtnClick()
    {
        SoundBtnClick();
        backBtn.SetActive(false);
        panelChoseMode.SetActive(true );
        SurvivorMode.SetActive(false );
        HostileMode.SetActive(false );
    }

    public void ScrollLeftBtn()
    {

    }
    public void ScrollRightBtn()
    {

    }
}
