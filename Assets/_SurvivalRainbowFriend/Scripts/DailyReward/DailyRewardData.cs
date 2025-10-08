using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using VKSdk.UI;

public class DailyRewardData : MonoBehaviour
{
    public static DailyRewardData Instance;
    public DailyObject dailyObject;
   
    void Start()
    {
        Instance = this;
        LoadDailyData();
    }
    public void LoadDailyData()
    {
        string jsonString = PlayerPrefs.GetString("DAILYDATA", "");
        if (jsonString != "")
        {
            dailyObject=JsonUtility.FromJson<DailyObject>(jsonString);
            
            if (CheckDailyReward())
            {

                long delTaTime = DateTimeOffset.Now.ToUnixTimeSeconds() - dailyObject.previousTime;
                float f = (float)(delTaTime) / (60 * 60 * 24);
                if(f>1)
                {
                    if (dailyObject.receiveDay == 6)
                    {
                        ResetDailyData();
                    }
                    else
                    {
                        dailyObject.receiveDay++;
                        dailyObject.StatusList[dailyObject.receiveDay] = true;
                    }
                    dailyObject.previousTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                }                   
                VKLayerController.Instance.ShowLayer("UIDailyReward");
               
            }
           
        }
        else
        {
            dailyObject = new DailyObject();
            dailyObject.firstTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            dailyObject.StatusList = new List<bool>();
            for (int i = 0; i < 7; i++)
                dailyObject.StatusList.Add(false);
            dailyObject.previousTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            dailyObject.receiveDay = 0;
            dailyObject.StatusList[0] = true;
            if (!PlayerPrefs.HasKey("UIREWARD"))
            {
                VKLayerController.Instance.ShowLayer("UIProfileUser");
                PlayerPrefs.SetString("UIREWARD", "HasKey");
            }
            else
            {
                VKLayerController.Instance.ShowLayer("UIDailyReward");
                StartCoroutine("ShowAds");
            }
           
        }
        SaveDailyData();
    }
    
    public void SaveDailyData()
    {
        string jsonString = JsonUtility.ToJson(dailyObject);
        PlayerPrefs.SetString("DAILYDATA", jsonString);
    }
    public void ReceiveiDaily(int iIndex)
    {
        dailyObject.StatusList[iIndex] = false;            
        SaveDailyData();
    }
    public void ReceiveDailyVideo(int iIndex)
    {
        dailyObject.StatusList[iIndex] = false;
        SaveDailyData();
    }
    void ResetDailyData()
    {
        dailyObject.receiveDay = 0;
        for(int i=0; i<dailyObject.StatusList.Count; i++)
        {
            dailyObject.StatusList[i] = false;
        }
        dailyObject.StatusList[0] = true;
    }
    public bool CheckDailyReward()
    {
        long delTaTime = DateTimeOffset.Now.ToUnixTimeSeconds() - dailyObject.previousTime;
        float f = (float)(delTaTime) / (60 * 60 * 24);
        if (f <= 1)
        {
            for(int i=0; i<dailyObject.StatusList.Count; i++)
            {
                if (dailyObject.StatusList[i]) return true;
                
            }
            return false;
        }
        else return true;
    }
}

[Serializable]
public class DailyObject
{
    public int receiveDay;
    public long firstTime;
    public long previousTime;
    public List<bool> StatusList;
}
