using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using LitJson;

using UnityEngine;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif

namespace VKSdk
{
    public static class VKAnalytics
    {
        public static ATrackingLog _trackLog;

#if GA_UNIVERSAL_ANALYTICS
        private static int trackScreenCount = 0;
#endif

        public static void Init()
        {

#if GA_UNIVERSAL_ANALYTICS
             string json = FileHelper.LoadTextFromFile("VK_TRACK_LOG");
            trackScreenCount = 0;
            try
            {
                if(!string.IsNullOrEmpty(json))
                {
                    _trackLog = JsonMapper.ToObject<ATrackingLog>(json);
                }
                else
                {
                    _trackLog =  new ATrackingLog();
                }
            }
            catch
            {
                _trackLog =  new ATrackingLog();
            }
#endif

        }

        public static void SaveTrackLog()
        {
            try
            {
                FileHelper.WriteTextToFile("VK_TRACK_LOG", JsonMapper.ToJson(_trackLog));
            }
            catch
            {
            }
        }

#region Tracking
        // login
        public static void TrackSignUp(string signinType)
        {
            // Set the user's sign up method.
            TrackStringParam("signup", VKGameConfig.Instance.PlatformLogin(), signinType);
        }

        public static void TrackSignIn(string signinType)
        {
            TrackStringParam("signin", VKGameConfig.Instance.PlatformLogin(), signinType);
        }

        // play game
        public static void TrackGameLobby(string gameType, string userId)
        {
            Dictionary<string, object> param = new Dictionary<string, object>
            {
                { "gametype", gameType},
                { "userid", userId }
            };

            TrackMultiParam("gamelobby", param);
        }

        public static void TrackVersionRunning()
        {
            Dictionary<string, object> param = new Dictionary<string, object>
            {
                { "platform", VKGameConfig.Instance.PlatformLogin() },
                { "version", VKGameConfig.Instance.VersionGame() }
            };

            TrackMultiParam("version", param);
        }

        public static void TrackPlatformRunning()
        {
            TrackStringParam("platformrunning", VKGameConfig.Instance.PlatformLogin(), VKGameConfig.Instance.VersionGame());
        }

        // first
        public static void TrackFirst(string firstKey)
        {
            if(_trackLog.AllowTrackFirst(firstKey))
            {
                TrackNoParam(firstKey, "first");
            }
        }

        // count
        public static void TrackCount(string trackKey, int max)
        {
            int count = _trackLog.AllowTrackCount(trackKey, max);
            if(count > 0)
            {
                TrackNoParam(trackKey + "_" + count, "count");
            }
        }

        public static void TrackCount(string trackKey, int max, int divisible)
        {
            int count = _trackLog.AllowTrackCount(trackKey, max);
            if(count > 0 && (count/divisible) == 0)
            {
                TrackNoParam(trackKey + "_" + count, "count");
            }
        }
#endregion

#region Track function
        public static void TrackSession(bool isStart)
        {
#if GA_UNIVERSAL_ANALYTICS
            if (!Strobotnik.GUA.Analytics.gua.analyticsDisabled)
            {
                Strobotnik.GUA.Analytics.gua.beginHit(Strobotnik.GUA.GoogleUniversalAnalytics.HitType.Screenview);
                Strobotnik.GUA.Analytics.gua.addScreenName("game-" + (isStart ? "start" : "end"));
                Strobotnik.GUA.Analytics.gua.addSessionControl(isStart); // end current session
                Strobotnik.GUA.Analytics.gua.sendHit();
            }
#endif
        }

        public static void TrackNoParam(string eventName, string eventCategory = "common")
        {
#if UNITY_ANALYTICS
            Analytics.CustomEvent(eventName);
#endif
#if FIREBASE
            VKFirebaseManager.Instance.analytics.TrackNoParam(eventName);
#endif
#if GA_UNIVERSAL_ANALYTICS
            Strobotnik.GUA.Analytics.gua.sendEventHit(eventCategory, eventName);
#endif
        }

        public static void TrackFloatParam(string eventName, string pramName, float paramValue)
        {
#if UNITY_ANALYTICS
            Analytics.CustomEvent(eventName, new Dictionary<string, object>
            {
                { pramName, paramValue }
            });
#endif
#if FIREBASE
            VKFirebaseManager.Instance.analytics.TrackFloatParam(eventName, pramName, paramValue);
#endif
#if GA_UNIVERSAL_ANALYTICS
            try
            {
                Strobotnik.GUA.Analytics.gua.sendEventHit("numberparam", eventName, pramName, (int)paramValue);
            }
            catch
            {
            }
#endif
        }

        public static void TrackIntParam(string eventName, string pramName, int paramValue)
        {
#if UNITY_ANALYTICS
            Analytics.CustomEvent(eventName, new Dictionary<string, object>
            {
                { pramName, paramValue }
            });
#endif
#if FIREBASE
            VKFirebaseManager.Instance.analytics.TrackIntParam(eventName, pramName, paramValue);
#endif
#if GA_UNIVERSAL_ANALYTICS
            Strobotnik.GUA.Analytics.gua.sendEventHit("numberparam", eventName, pramName, paramValue);
#endif
        }

        public static void TrackStringParam(string eventName, string pramName, string paramValue)
        {
#if UNITY_ANALYTICS
            Analytics.CustomEvent(eventName, new Dictionary<string, object>
            {
                { pramName, paramValue }
            });
#endif
#if FIREBASE
            VKFirebaseManager.Instance.analytics.TrackStringParam(eventName, pramName, paramValue);
#endif
#if GA_UNIVERSAL_ANALYTICS
            Strobotnik.GUA.Analytics.gua.sendEventHit(eventName, pramName, paramValue);
#endif
        }

        public static void TrackMultiParam(string eventName, Dictionary<string, object> param)
        {
#if UNITY_ANALYTICS
            Analytics.CustomEvent(eventName, param);
#endif
#if FIREBASE
            VKFirebaseManager.Instance.analytics.TrackMultiParam(eventName, param);
#endif
        }

        public static void TrackScreenView(string sceenName)
        {
#if GA_UNIVERSAL_ANALYTICS
            if(trackScreenCount >= 200) return;
            ++trackScreenCount;
    
            if(trackScreenCount == 200)
            {
                Strobotnik.GUA.Analytics.gua.sendAppScreenHit("MainGame200");
            }
            else
            {
                Strobotnik.GUA.Analytics.gua.sendAppScreenHit(sceenName);
            }
#endif
        }

        // payment
        public static void TrackPayment(ATrackingTransaction transaction)
        {
#if GA_UNIVERSAL_ANALYTICS
            if(transaction == null) return;
            try
            {
                Strobotnik.GUA.Analytics.gua.sendTransactionHit(
                    transaction.transactionID, 
                    transaction.affiliation, 
                    transaction.revenue,
                    transaction.shipping, 
                    transaction.tax,
                    transaction.currencyCode);

                Strobotnik.GUA.Analytics.gua.sendItemHit(
                    transaction.transactionID, 
                    transaction.itemName, 
                    transaction.price,
                    transaction.quantity, 
                    transaction.itemCode,
                    transaction.itemCategory,
                    transaction.currencyCode);
            }
            catch {}
#endif
        }
#endregion
    }

    [Serializable]
    public class ATrackingLog
    {
        public List<string> firstTracks;
        public List<ATrackingLogCount> countTracks;

        public ATrackingLog()
        {
            firstTracks = new List<string>();
            countTracks = new List<ATrackingLogCount>();
        }

        public bool AllowTrackFirst(string key)
        {
            if(firstTracks.Contains(key))
            {
                return false;
            }

            firstTracks.Add(key);
            return true;
        }

        public int AllowTrackCount(string key, int max)
        {
            var log = countTracks.FirstOrDefault(a => a.key.Equals(key));
            if(log == null)
            {
                log = new ATrackingLogCount {
                    key = key,
                    count = 0
                };
                countTracks.Add(log);
            }

            if(log.count < max)
            {
                log.count++;
                return log.count;
            }

            return -1;
        }
    }

    [Serializable]
    public class ATrackingLogCount
    {
        public string key;
        public int count;
    }

    [Serializable]
    public class ATrackingTransaction
    {
        public string transactionID;
        public string affiliation;
        public double revenue;
        public double shipping;
        public double tax;
        public string currencyCode;

        public string itemName;
        public double price;
        public int quantity;
        public string itemCode;
        public string itemCategory;
    }
}
