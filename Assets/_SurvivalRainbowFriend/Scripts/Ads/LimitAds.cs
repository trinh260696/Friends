using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitAds : MonoBehaviour
{
    public static LimitAds Instance;
    public int ADS_SPIN;
    public int ADS_SKIN;
    public int ADS_MORE_COIN;
    public int ADS_MORE_RUBY;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        LoadAdsLimit();
        long deltaDay = System.DateTimeOffset.Now.ToUnixTimeSeconds() - UserData.Instance.lastTime;
        if ((float)(deltaDay) / 86400f > 1)
        {
            ADS_SPIN = 0;
            ADS_SKIN = 0;
            ADS_MORE_COIN = 0;
            ADS_MORE_RUBY = 0;
        }
        SaveAdsLimit();
    }

    public void SaveAdsLimit()
    {
        PlayerPrefs.SetInt("ADS_SPIN", ADS_SPIN);
        PlayerPrefs.SetInt("ADS_SKIN", ADS_SKIN);
        PlayerPrefs.SetInt("ADS_MORE_COIN", ADS_MORE_COIN);
        PlayerPrefs.SetInt("ADS_MORE_RUBY", ADS_MORE_RUBY);
    }
    public void LoadAdsLimit()
    {
        ADS_SPIN = PlayerPrefs.GetInt("ADS_SPIN");
        ADS_SKIN = PlayerPrefs.GetInt("ADS_SKIN");
        ADS_MORE_COIN = PlayerPrefs.GetInt("ADS_MORE_COIN");
        ADS_MORE_RUBY = PlayerPrefs.GetInt("ADS_MORE_RUBY");
    }
}
