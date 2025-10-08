using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config {
	public static string API_DT_1 = "http://skymobilegame.com:7982/paymentgame/naptien/getverupdate?platform={0}&lang={1}&gameid={2}&version={3}";
	public static string API_DT_2 = "http://skynetmobilegame.com:7982/paymentgame/naptien/getverupdate?platform={0}&lang={1}&gameid={2}&version={3}";
	public static string API_DT_3 = "http://sknbk.com:7982/paymentgame/naptien/getverupdate?platform={0}&lang={1}&gameid={2}&version={3}";
	public static string API_RECEIPT = "http://skynetmobilegame.com:7982/MiniGame/service/validatereceipt?deviceid={0}&transid={1}&gameid={2}";
	public static string API_TOKEN = "http://skynetmobilegame.com:7982/MiniGame/service/validatetoken?deviceid={0}&gameid={1}&isreceiver={2}";

	public static string KEY_FILENAME_IMG = "FILENAME_IMG_ADS_101";
	public static string KEY_LINK_IMG = "LINK_ADS_101";

	public static string KEY_MAX_VER_FB = "key_max_ver_fb_";
	public static string KEY_PRE_GET_DATA = "key_pre_get_data_x";
	public static string KEY_OPEN_ATT_FIR = "key_open_att_fir";
	public static string KEY_OPEN_ATT_SER = "key_open_att_ser";
	public static string KEY_CFR_ATT_FIR = "key_confirm_att_fir";
	public static string KEY_CFR_ATT_SER = "key_confirm_att_ser";
	public static string KEY_CHECK_RATE = "key_check_rate_x";
	public static string KEY_SAVE_DAY_RATE = "key_save_day_rate_x";
	public static string KEY_SAVE_NUMBER_RATE = "key_save_number_rate_x";
	public static string KEY_SAVE_NUMBER_DATE_RATE = "key_save_number_date_rate_x";
	public static string KEY_POLICY = "key_policy_x";
	public static string KEY_REMOVE_ADS = "key_remove_ads_x";

#if UNITY_ANDROID
	public static bool IAP = false;
	public static string IAP1 = "PACK_IAP_01"; //Pack RemoveAds
	public static string IAP2 = "PACK_IAP_02"; //Pack Bundle
	public static string IAP3 = "PACK_IAP_03"; //Pack UnlimtedHeart

	public static string LINK_POLICY = "";
	public static string LINK_TERMS = "";
	public static string LINK_APP = "";

	public static long DAY_RW_END = 1582183833;
	public static long DEL_DAY = (4*24*60*60);
    public static string GAMEID = "284";
	public static int VS_CURR = 103;
	public static string PLATFORM = "ANDROID";
#else
	public static bool IAP = false;
	public static string IAP1 = "";
	public static string IAP2 = "";
	public static string IAP3 = "";

	public static string LINK_POLICY = "http://galaxyprivacypolicy.blogspot.com/2017/11/galaxy-privacy-policy.html";
	public static string LINK_TERMS = "http://galaxyprivacypolicy.blogspot.com/2021/02/terms-of-service.html";
	public static string LINK_APP = "https://itunes.apple.com/us/app/keynote/id1384962501?mt=8";

	public static long DAY_RW_END = 1612682645;
	public static long DEL_DAY = (21 * 24 * 60 * 60);
	public static string GAMEID = "318";
	public static int VS_CURR = 100;
	public static string PLATFORM = "IOS";
#endif
}
