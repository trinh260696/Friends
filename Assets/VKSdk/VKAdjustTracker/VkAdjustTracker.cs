using com.adjust.sdk;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VkAdjustTracker
{
    public static void AdjustTrack(string eventcode, Dictionary<string, object> param=null)
    {

        AdjustEvent adjustEvent = new AdjustEvent(eventcode); // progress_level_start
        if (param != null)
        {
            adjustEvent.addCallbackParameter("event_params", JsonConvert.SerializeObject(param));
        }
       
        Adjust.trackEvent(adjustEvent);
    }

    public static void TrackResourceGain(string resource_type, int amount, int total, string source)
    {
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("resource_type", resource_type);
        event_params.Add("amount", amount);
        event_params.Add("total_amount", total);
        event_params.Add("source", source);
        AdjustTrack(AdjustCode.RESOURCE_GAIN, event_params);
    }
    public static void TrackResourceLose(string resource_type, int amount, int total, string source)
    {
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("resource_type", resource_type);
        event_params.Add("amount", amount);
        event_params.Add("total_amount", total);
        event_params.Add("source", source);
        AdjustTrack(AdjustCode.RESOURCE_LOSE, event_params);
    }
    public static void TrackProgressStart(string mode, string level, int level_id)
    {
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("mode", mode);
        event_params.Add("level_name", level);
        event_params.Add("level_id", level_id);
       
        AdjustTrack(AdjustCode.PROGRESS_LEVEL_START, event_params);
    }
    public static void TrackProgressFail(string mode, string level, int level_id, int playing_time)
    {
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("mode", mode);
        event_params.Add("level_name", level);
        event_params.Add("level_id", level_id);
        event_params.Add("playing_time", playing_time);
        AdjustTrack(AdjustCode.PROGRESS_LEVEL_FAIL, event_params);
    }
    public static void TrackProgressComplete(string mode, string level, int level_id, int playing_time)
    {
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("mode", mode);
        event_params.Add("level_name", level);
        event_params.Add("level_id", level_id);
        event_params.Add("playing_time", playing_time);
        AdjustTrack(AdjustCode.PROGRESS_LEVEL_COMPLETE, event_params);
    }
    public static void TrackFeatureLuckySpin(string type, string reward)
    {
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("type", type);
        event_params.Add("reward", reward);
       
        AdjustTrack(AdjustCode.FEATURE_LUCKY_SPIN, event_params);
    }
    public static void TrackFeatureSkinShopViewSkin(string skin_name)
    {
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("skin_name", skin_name);
        AdjustTrack(AdjustCode.FEATURE_SKIN_SHOP_VIEW, event_params);
    }
    public static void TrackFeatureSkinShopBuySkin(string skin_name, string type, int video_viewed_count)
    {
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("skin_name", skin_name);
        event_params.Add("type", type);
        if (video_viewed_count>0)
        {
            event_params.Add("video_viewed_count", video_viewed_count);
        }
        AdjustTrack(AdjustCode.FEATURE_SKIN_SHOP_BUY, event_params);
    }
    public static void TrackFeatureDailyReward(int day, string post_action)
    {
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("day", day);
        event_params.Add("post_action", post_action);
        AdjustTrack(AdjustCode.FEATURE_DAILY_REWARD, event_params);
    }
    public static void TrackFeatureGetMoreCoin(string type)
    {
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("type", type);      
        AdjustTrack(AdjustCode.FEATURE_GET_MORE_COINS, event_params);
    }
    public static void TrackFeatureGetMoreDiamond(string type)
    {
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("type", type);
        AdjustTrack(AdjustCode.FEATURE_GET_MORE_DIAMONDS, event_params);
    }
    public static void TrackFeatureGetMoreBooster(string type)
    {
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("type", type);
        AdjustTrack(AdjustCode.FEATURE_START_BOOSTER, event_params);
    }
    public static void TrackFeatureEndMoreBooster(string type)
    {
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("type", type);
        AdjustTrack(AdjustCode.FEATURE_END_BOOSTER, event_params);
    }
    public static void TrackFeatureUsingMask(int masking_time)
    {
        Dictionary<string, object> event_params = new Dictionary<string, object>();
        event_params.Add("masking_time", masking_time);
        AdjustTrack(AdjustCode.FEATURE_USING_MASK, event_params);
    }
    public static void TrackDesignClickSoundOff()
    {
        AdjustTrack(AdjustCode.DESIGN_CLICK_SOUND_OFF);
    }
    public static void TrackDesignClickSoundOn()
    {
        AdjustTrack(AdjustCode.DESIGN_CLICK_SOUND_ON);
    }
    public static void TrackDesignClickMusicOn()
    {
        AdjustTrack(AdjustCode.DESIGN_CLICK_MUSIC_ON);
    }
    public static void TrackDesignClickMusicOff()
    {
        AdjustTrack(AdjustCode.DESGIN_CLICK_MUSIC_OFF);
    }
    public static void TrackDesignClickEditName()
    {
        AdjustTrack(AdjustCode.DESIGN_CLICK_SETTING_USERNAME);
    }
}
public class AdjustCode
{
    public static string IAP_CODE = "6w2rk7";
    public static string ADS_LOAD = "j3huaa";
    public static string ADS_SHOW = "fmkp8r";
    public static string RESOURCE_GAIN = "psyxee";
    public static string RESOURCE_LOSE = "5z32r7";
    public static string PROGRESS_LEVEL_START = "7nyj6b";
    public static string PROGRESS_LEVEL_COMPLETE = "f8y5gg";
    public static string PROGRESS_LEVEL_FAIL = "4s0u0h";
    public static string FEATURE_LUCKY_SPIN = "45meo3";
    public static string FEATURE_SKIN_SHOP_VIEW = "g68sy5";
    public static string FEATURE_SKIN_SHOP_BUY = "jnt94n";
    public static string FEATURE_DAILY_REWARD = "2505bt";
    public static string FEATURE_GET_MORE_COINS = "u4ybfz";
    public static string FEATURE_GET_MORE_DIAMONDS = "sfk3au";
    public static string FEATURE_START_BOOSTER = "hxeqvp";
    public static string FEATURE_END_BOOSTER = "kfjkon";
    public static string FEATURE_USING_MASK = "f6ts06";
    public static string DESIGN_CLICK_SOUND_ON = "9ogjbh";
    public static string DESIGN_CLICK_SOUND_OFF = "3qci6t";
    public static string DESIGN_CLICK_MUSIC_ON = "xc36kd";
    public static string DESGIN_CLICK_MUSIC_OFF = "qjnj1g";
    public static string DESIGN_CLICK_SETTING_USERNAME = "is3weg";
}
