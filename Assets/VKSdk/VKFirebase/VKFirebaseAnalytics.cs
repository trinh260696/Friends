using System.Collections.Generic;
#if FIREBASE
using Firebase.Analytics;
#endif
/* Author: Anhnh1721
 * Version: 1.0
 */
namespace VKSdk
{
    public class VKFirebaseAnalytics
    {
#if FIREBASE
    public bool isInitialized;

    // Handle initialization of the necessary firebase modules:
    public void InitAnalytics()
    {
        // VKFirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        isInitialized = true;
    }

    public void DisableAnalytics()
    {
        isInitialized = false;
    }

    #region Method
    // common
    // track common
    public void TrackNoParam(string eventName)
    {
        if(!isInitialized) return;
        Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName);
    }

    public void TrackFloatParam(string eventName, string pramName, float paramValue)
    {
        if(!isInitialized) return;
        Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName, pramName, paramValue);
        Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName, pramName, paramValue);
    }

    public void TrackIntParam(string eventName, string pramName, int paramValue)
    {
        if(!isInitialized) return;
        Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName, pramName, paramValue);
    }

    public void TrackStringParam(string eventName, string pramName, string paramValue)
    {
        if(!isInitialized) return;
        Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName, pramName, paramValue);
    }

    public void TrackMultiParam(string eventName, Dictionary<string, object> param)
    {
        if(!isInitialized) return;

        List<Firebase.Analytics.Parameter> parameters = new List<Firebase.Analytics.Parameter>();
        foreach (var item in param)
        {
            Firebase.Analytics.Parameter paramTemp = null;
            if(item.Value is string) paramTemp = new Parameter(item.Key, (string)item.Value);
            if(item.Value is System.String) paramTemp = new Parameter(item.Key, (System.String)item.Value);
            if(item.Value is double) paramTemp = new Parameter(item.Key, (double)item.Value);
            if(item.Value is System.Double) paramTemp = new Parameter(item.Key, (System.Double)item.Value);
            if(item.Value is float) paramTemp = new Parameter(item.Key, (float)item.Value);
            if(item.Value is int) paramTemp = new Parameter(item.Key, (int)item.Value);

            if(paramTemp != null)
            {
                parameters.Add(paramTemp);
            }
            else
            {
                return;
            }
        }

        Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName, parameters.ToArray());
    }
    #endregion
#endif
    }
}