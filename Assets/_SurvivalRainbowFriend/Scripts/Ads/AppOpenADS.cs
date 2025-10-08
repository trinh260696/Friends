using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppOpenADS : MonoBehaviour
{
    public string AppOpenAdUnitId;
    void Start()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
        {
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenDismissedEvent;

            AppOpenManager.ShowAdIfReady(AppOpenAdUnitId);
        };      
    }

    public void OnAppOpenDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        MaxSdk.LoadAppOpenAd(AppOpenAdUnitId);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            AppOpenManager.ShowAdIfReady(AppOpenAdUnitId);
        }
    }
}

public class AppOpenManager
{
    

    public static void ShowAdIfReady(string AppOpenAdUnitId)
    {
        if (MaxSdk.IsAppOpenAdReady(AppOpenAdUnitId))
        {
            MaxSdk.ShowAppOpenAd(AppOpenAdUnitId);
        }
        else
        {
            MaxSdk.LoadAppOpenAd(AppOpenAdUnitId);
        }
    }
}
