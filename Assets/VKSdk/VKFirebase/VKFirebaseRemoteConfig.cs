using UnityEngine;
using Firebase.RemoteConfig;
using System;
using System.Threading.Tasks;
using Firebase.Extensions;
using System.Collections.Generic;
using UnityEngine.Networking;

public class VKFirebaseRemoteConfig : MonoBehaviour
{
    public static VKFirebaseRemoteConfig instance;
    public FirebaseRemoteConfig remoteConfig;
    public double remote_threshold=0.01f;
    Dictionary<string, object> defaults = new Dictionary<string, object>
          {
              {"conf_ad_rev_threshold", 0.01f },
              
          };
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableInvalid;
    private void Awake()
    {
        instance = this;
        
    }

    private void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all firebase dependencies: " + dependencyStatus);
            }
        });
    }

    public void FetchFireBase()
    {
        FetchDataAsync();
    }
    public Task FetchDataAsync()
    {
        Debug.LogWarning("Fetching data...");
        System.Threading.Tasks.Task fetchTask =
        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
            TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }
    async void FetchComplete(Task fetchTask)
    {
        if (fetchTask.IsCanceled)
        {
            Debug.LogWarning("Fetch canceled.");
        }else if (fetchTask.IsFaulted)
        {
            Debug.LogWarning("Fetch encountered an error.");
        }else if (fetchTask.IsCompleted)
        {
            Debug.LogWarning("Fetch completed successfully!");
            await Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
            ShowData();
            GetAllValue();
        }
    }
    void InitializeFirebase()
    {
        Debug.LogWarning("Remote config ready!");
        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);
        FetchFireBase();
    }
    public void ShowData()
    {
        remote_threshold = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("conf_ad_rev_threshold").DoubleValue;
       
    }
   public void GetAllValue()
    {
        // retrieve values in native collection
        // Unity/C#
        IDictionary<string, ConfigValue> values = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.AllValues;
        foreach(var pair in values)
        {
            Debug.LogWarning(pair.Key);
        }
    }
    public void GetConfig()
    {
        // handle config
        // Unity/C#
        remoteConfig.FetchAndActivateAsync().ContinueWithOnMainThread(task =>
        {

            // handle completion
            if (task.IsCanceled || task.IsFaulted)
            {
               
                Debug.LogWarning("Faild.");
            }
            else
            {
                Debug.LogWarning("Completed.");
            }
            
            
            
        });
    }
    
    public void DebugRemote()
    {
        // Debug
        // Unity/C#
        var configSettings = new ConfigSettings();
        if (Debug.isDebugBuild)
        {
            // refresh immediately when debugging
            configSettings.MinimumFetchInternalInMilliseconds = 0;
        }

        remoteConfig.SetConfigSettingsAsync(configSettings).ContinueWithOnMainThread(task =>
        {
            // handle completion
        });
    }
    public void fetch(Action<bool> completionHandler)
    {
        // TODO: RELEASE時にここを外す
        var settings = remoteConfig.ConfigSettings;
        

        System.Threading.Tasks.Task fetchTask = remoteConfig.FetchAsync(new System.TimeSpan(0));

        fetchTask.ContinueWith(task => {
            if (task.IsCanceled || task.IsFaulted)
            {
                completionHandler(false);
                Debug.LogWarning("Faild.");
            }
            else
            {
                Debug.LogWarning("Completed.");
            }
            remoteConfig.ActivateAsync();
            
            completionHandler(true);
        });
    }
    
}