using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif
using UnityEngine;

namespace VKSdk
{
    public class VKSystemNotifyManager : MonoBehaviour
    {
        public DateTime dTimeStart;

        public string androidChannelId = "vksdk_channel_0";
        public string iosKey = "vksdk";

        // public List<string> strNotifyKeys;

        private static VKSystemNotifyManager instance;
        public static VKSystemNotifyManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<VKSystemNotifyManager>();
                }
                return instance;
            }
        }

        public void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        public void Start()
        {
            dTimeStart = DateTime.Now;

            // get key from PlayerPrefs
            // string json = PlayerPrefs.GetString("notify_keys", "");
            // if (!string.IsNullOrEmpty(json))
            // {
            //     strNotifyKeys = JsonMapper.ToObject<List<string>>(json);
            // }
            // else
            // {
            //     strNotifyKeys = new List<string>();
            // }

#if UNITY_ANDROID
            var c = new AndroidNotificationChannel()
            {
                Id = androidChannelId,
                Name = "Default Channel",
                Importance = Importance.High,
                Description = "Generic notifications",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(c);
            RemoveAllNotification();
#elif UNITY_IOS
            StartCoroutine(RequestAuthorization());
#endif
        }

        // public void SaveCache()
        // {
        //     PlayerPrefs.SetString("notify_keys", JsonMapper.ToJson(strNotifyKeys));
        // }

        public string GetLastRespondedData()
        {
#if UNITY_ANDROID
            var notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();
            if (notificationIntentData != null)
            {
                try
                {
                    var notification = notificationIntentData.Notification;
                    return notification.IntentData;
                }
                catch {}
            }
#elif UNITY_IOS
            var notification = iOSNotificationCenter.GetLastRespondedNotification();
            if (notification != null)
            {
                return notification.Data;
            }
#endif
            return null;
        }

        public void CreateNotification(int index, string title, string subTitle, string info, string data, double secondPush)
        {
           
            
#if DEVELOPER
            VKDebug.LogWarning("title " + title);
            VKDebug.LogWarning("subTitle " + subTitle);
            VKDebug.LogWarning("info " + info);
            VKDebug.LogWarning("secondPush " + secondPush);
#endif

#if UNITY_ANDROID
            VKDebug.LogWarning("android notify " + title);
            var notification1 = new AndroidNotification();

            notification1.Title = title;
            notification1.Text = info;
            notification1.FireTime = DateTime.Now.AddSeconds(secondPush);
            notification1.ShouldAutoCancel = true;
            notification1.LargeIcon = "icon_large_1";
            notification1.SmallIcon = "icon_small_1";
            if(!string.IsNullOrEmpty(data))
            {
                notification1.IntentData = data;
            }

            var identifier1 = AndroidNotificationCenter.SendNotification(notification1, androidChannelId);
            // strNotifyKeys.Add(identifier1.ToString());
#elif UNITY_IOS
            var timeTrigger1 = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = TimeSpan.FromSeconds(secondPush),
                Repeats = false
            };

            string identifier = iosKey + "_notifi_" + index;
            var notification1 = new iOSNotification()
            {
                Identifier = identifier,
                Title = title,
                Body = info,
                Subtitle = subTitle,
                ShowInForeground = true,
                ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
                CategoryIdentifier = "category_" + iosKey + "_" + index,
                ThreadIdentifier = "thread_" + index,
                Trigger = timeTrigger1,
            };

            if(!string.IsNullOrEmpty(data))
            {
                notification1.Data = data;
            }
            
            iOSNotificationCenter.ScheduleNotification(notification1);
            // strNotifyKeys.Add(identifier);
#endif
        }

        public void RemoveAllNotification()
        {
            // xóa thông báo được lưu
//             foreach (var identifier in strNotifyKeys)
//             {
//                 try
//                 {
// #if UNITY_ANDROID
//                     AndroidNotificationCenter.CancelNotification(int.Parse(identifier));
// #elif UNITY_IOS
//                     iOSNotificationCenter.RemoveScheduledNotification(identifier);
//                     iOSNotificationCenter.RemoveDeliveredNotification(identifier);
// #endif
//                 }
//                 catch { }
//             }
//             strNotifyKeys.Clear();

            try
            {
#if UNITY_ANDROID
                AndroidNotificationCenter.CancelAllNotifications();
                AndroidNotificationCenter.CancelAllDisplayedNotifications();
                AndroidNotificationCenter.CancelAllScheduledNotifications();
#elif UNITY_IOS
                iOSNotificationCenter.RemoveAllDeliveredNotifications();
                iOSNotificationCenter.RemoveAllScheduledNotifications();
#endif
            }
            catch { }
        }

#if UNITY_IOS
    IEnumerator RequestAuthorization()
    {
        using (var req = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true))
        {
            while (!req.IsFinished)
            {
                yield return null;
            };

            string res = "\n RequestAuthorization: \n";
            res += "\n finished: " + req.IsFinished;
            res += "\n granted :  " + req.Granted;
            res += "\n error:  " + req.Error;
            res += "\n deviceToken:  " + req.DeviceToken;
            // Debug.Log(res);

            RemoveAllNotification();
        }
    }
#endif

        // public double GetSecondPushNotification()
        // {
        //     double timeRange = LMRemoteSettingManager.Instance.config.time_notification;
        //     TimeSpan range = (DateTime.Now - dTimeStart);
        //     double time = timeRange - range.TotalSeconds;
        //     if (time < 0)
        //     {
        //         time = 120; //2p
        //     }
        //     return time;
        // }

        public double GetSecondFromOpenApp()
        {
            TimeSpan range = (DateTime.Now - dTimeStart);
            return range.TotalSeconds;
        }
    }

}