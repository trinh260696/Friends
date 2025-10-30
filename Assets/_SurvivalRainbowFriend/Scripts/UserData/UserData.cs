using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserData : MonoBehaviour
{
    public static UserData Instance;
    public SGameData GameData;
    public long lastTime;
    private void Start()
    {
        Instance = this;
        StartCoroutine("LoadLocalData");

    }
    private void Update()
    {
        if(GameData.TIME_SPIN_FLAG==1)
        {
            GameData.TIME_SPIN_CURRENT += Time.deltaTime;
            if (GameData.TIME_SPIN_CURRENT > StaticData.TIME_SPIN_MAX)
            {
                GameData.TIME_SPIN_CURRENT = 0;
                GameData.TIME_SPIN_FLAG = 0;
            }
        }
    }
    public void SaveLocalData()
    {
       
        string jsonString = JsonUtility.ToJson(GameData);
        PlayerPrefs.SetString("GAMEDATA", jsonString);
    }
    IEnumerator LoadLocalData()
    {
        string strJson = PlayerPrefs.GetString("GAMEDATA", "");
        
        if(strJson == "")
        {
            GameData = new SGameData();
            GameData.Init();
            lastTime = System.DateTimeOffset.Now.ToUnixTimeSeconds();
            PlayerPrefs.SetInt("LastTime", (int)lastTime);
        }
        else
        {
            GameData = JsonUtility.FromJson<SGameData>(strJson);
            lastTime = (long)PlayerPrefs.GetInt("LastTime");
        }
        if (GameData.TIME_SPIN_FLAG == 1)
        {
            long timeOffset = System.DateTimeOffset.Now.ToUnixTimeSeconds() - lastTime;
            GameData.TIME_SPIN_CURRENT += (float)timeOffset;
        }
        while (GameData.level == 0) yield return null;
        SaveLocalData();
        OnComplete();
       
    }
    public void OnSelectSkin(SkinItem skinItem)
    {
        GameData.currentSkin = skinItem;
        SaveLocalData();
    }
    
    void OnComplete()
    {
        SceneManager.LoadSceneAsync ("MainStartUI");
       
    }
    public void SetTimeSpinStartup()
    {
        GameData.TIME_SPIN_CURRENT = 0;
        GameData.TIME_SPIN_FLAG = 1;
        SaveLocalData(); 
    }
    public void OpenLevelSurvival(int lv)
    {
        for(int i=0; i<UserData.Instance.GameData.SurviveLevels.Length; i++)
        {
            if(UserData.Instance.GameData.SurviveLevels[i].level==lv)
            {
                UserData.Instance.GameData.SurviveLevels[i].status = true;
                break;
            }
        }
        SaveLocalData();
    }
    public void SaveNameUser(string str)
    {
        GameData.name = str;
        SaveLocalData();
    }
    bool statusPause = false, statusFocus = false;

    void OnApplicationFocus(bool focus)
    {
        statusFocus = focus;
#if UNITY_EDITOR
        if (focus)
        {

        }
        else
        {
            LeaveApplication();
        }
#else

#endif

    }
    void OnApplicationPause(bool pause)
    {
#if UNITY_EDITOR
        if (pause)
        {

        }
        else
        {

        }
#else
       if (pause)
        {
            if (!statusFocus)
            {
                LeaveApplication();
            }
        }
        else
        {
            if (statusFocus)
            {
                ResumeApplication();
            }
        }

#endif

    }
    void ResumeApplication()
    {
        Debug.LogWarning("ResumeGame");
        if (GameData.TIME_SPIN_FLAG == 1)
        {
            long timeOffset = System.DateTimeOffset.Now.ToUnixTimeSeconds() - lastTime;
            GameData.TIME_SPIN_CURRENT += (float)timeOffset;
        }
    }
    void LeaveApplication()
    {
        Debug.LogWarning("LeaveGame");
        lastTime = (int)System.DateTimeOffset.Now.ToUnixTimeSeconds();
        PlayerPrefs.SetInt("LastTime", (int)lastTime);
        SaveLocalData();
    }

    public void UpdateLevelWin(int lEVEL)
    {
        
        for (int i = 0; i < UserData.Instance.GameData.SurviveLevels.Length; i++)
        {
            if (UserData.Instance.GameData.SurviveLevels[i].level == lEVEL)
            {
                if(UserData.Instance.GameData.SurviveLevels[i].survive==0)
                {
                    UserData.Instance.GameData.level++;
                }
                UserData.Instance.GameData.SurviveLevels[i].survive++;
                break;
            }
        }
        SaveLocalData();
    }
    public void UpdateLevelDie(int lEVEL)
    {
        for (int i = 0; i < UserData.Instance.GameData.SurviveLevels.Length; i++)
        {
            if (UserData.Instance.GameData.SurviveLevels[i].level == lEVEL)
            {
                UserData.Instance.GameData.SurviveLevels[i].die++;
                break;
            }
        }
        SaveLocalData();
    }
}


