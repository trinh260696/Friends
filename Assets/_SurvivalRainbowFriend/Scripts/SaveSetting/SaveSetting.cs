using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SaveSetting : MonoBehaviour
{
    public static SaveSetting instance;
    public SettingData settingData;

    private void Awake()
    {
        instance = this;
        LoadSaveSetting();
    }

    public void LoadSaveSetting()
    {
        string strJson = PlayerPrefs.GetString("SETTINGDATA", "");

        if (strJson == "")
        {
            settingData = new SettingData();
            settingData.isMusicOn = true;
            settingData.isSoundOn = true;
            settingData.isVibrationOn = true;
        }
        else
        {
            settingData = JsonUtility.FromJson<SettingData>(strJson);
        }
        SaveLocalSetting();
    }

    public void SaveLocalSetting()
    {
        string jsonString = JsonUtility.ToJson(settingData);
        PlayerPrefs.SetString("SETTINGDATA", jsonString);
    }
}
[Serializable]
public class SettingData
{
    public bool isSoundOn;
    public bool isMusicOn;
    public bool isVibrationOn;
}