using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using VKSdk.UI;
public class FortuneWheelManager : MonoBehaviour
{
    public static FortuneWheelManager instance;

    public static bool _isStarted = false;
    private float[] _sectorsAngles;
    public float _finalAngle;
    public float _startAngle = 0;
    private float _currentLerpRotationTime;
    public Button TurnButton;
    public Button AdsButton;
    public GameObject Circle; 			// Rotatable Object with rewards

    public VKCountDownLite TextTimeCountDown;
    public TextMeshProUGUI ItemDeltaText;
    public Image ItemDeltaImage;
    private bool isClickAds = false;

    public bool isCoin, isRuby, isSkin;
    float randomFinalAngle;

    private void Awake()
    {
        if(instance == null)   
            instance = this;
        AdsButton.gameObject.SetActive(false);

    }
    public void OnInit()
    {
        LoadData();
    }
    void LoadData()
    {
        if (UserData.Instance.GameData.TIME_SPIN_FLAG == 1)
        {

            TextTimeCountDown.SetSeconds(StaticData.TIME_SPIN_MAX - UserData.Instance.GameData.TIME_SPIN_CURRENT);
            TextTimeCountDown.StartCountDown(() => OnCountDownComplete());
            //  TextTimeCountDown.AddListener(() => OnCountDownComplete(), null);
            TurnButton.gameObject.SetActive(false);
            AdsButton.gameObject.SetActive(true);
        }
        else
        {

            TextTimeCountDown.StopCountDown();
            TextTimeCountDown.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Ready";
            TurnButton.gameObject.SetActive(true);
            TurnButton.interactable = true;
            AdsButton.gameObject.SetActive(false);
        }
    }
    void OnCountDownComplete()
    {
        TextTimeCountDown.StopCountDown();
        TextTimeCountDown.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Ready";
        TurnButton.interactable = true;
        AdsButton.gameObject.SetActive(false);
    }
    public void TurnWheel()
    {
        AudioManager.instance.Play("ButtonClick");
        AudioManager.instance.Play("FortuneSpin");

        _currentLerpRotationTime = 0f;
        // Fill the necessary angles (for example if you want to have 12 sectors you need to fill the angles with 30 degrees step)
        _sectorsAngles = new float[] { 45, 90, 135, 180, 225, 270, 315, 360 };
                                     //0   1   2    3    4    5    6    7

        int fullCircles = 15;

        ///////////////////////
        //float[] Rand = new float[100];
        //for(int i = 0; i < Rand.Length; i++)
        //{
        //    Rand[i] = i;

        //}
        int rand = UnityEngine.Random.Range(0, 99);
     
        if (rand == 0) randomFinalAngle = _sectorsAngles[7];                       // 360       //Skin      // 1%
        else if (0 < rand && rand <= 8) randomFinalAngle = _sectorsAngles[1];      // 90        //5R        // 8%
        else if (8 < rand && rand <= 19) randomFinalAngle = _sectorsAngles[4];     // 225       //3R        // 11%
        else if (19 < rand && rand <= 33) randomFinalAngle = _sectorsAngles[3];    // 180       //2R        // 14%
        else if (33 < rand && rand <= 45) randomFinalAngle = _sectorsAngles[5];    // 270       //5000C     // 12%
        else if (45 < rand && rand <= 60) randomFinalAngle = _sectorsAngles[0];    // 45        //1000C     // 15%
        else if (60 < rand && rand <= 78) randomFinalAngle = _sectorsAngles[2];    // 135       //500C      // 18%
        else if (78 < rand && rand <= 99) randomFinalAngle = _sectorsAngles[6];    // 315       //300C      // 21%

        //float randomFinalAngle = _sectorsAngles[UnityEngine.Random.Range(0, _sectorsAngles.Length)];

        // Here we set up how many circles our wheel should rotate before stop
        _finalAngle = -(fullCircles * 360 + randomFinalAngle);
        _isStarted = true;
    }

    public void AdsTurnWheel()
    {
        if (!_isStarted)
        {
            AudioManager.instance.Play("ButtonClick");
            if (LimitAds.Instance.ADS_SPIN >= 5)
            {
                UIPopup.OpenPopup("Notify", "ADS is limited", false);
                return;
            }
            
            AdsManager.Instance.ShowRewardedAd(() =>
            {
                LimitAds.Instance.ADS_SPIN++;
                LimitAds.Instance.SaveAdsLimit();
                AudioManager.instance.Play("CollectRuby");
                isClickAds = true;
                TurnWheel();
                UserData.Instance.GameData.ruby += 1;
                VkAdjustTracker.TrackResourceGain("ruby", 1, UserData.Instance.GameData.ruby, "ads_spin");
                NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");
            });
        }
    }
    public void OnClickTurnWheel()
    {
        if (!_isStarted)
        {
            isClickAds = false;
            TurnWheel();
        }
    }

    public void OnClickTurnWheelAds()
    {

    }

    private void GiveAwardByAngle()
    {
        // Here you can set up rewards for every sector of wheel
        AudioManager.instance.Play("UnlockAchievement");
        switch ((int)_startAngle)
        {
            case 0:
                RewardSkin();
                break;
            case -315:
                RewardCoins(300);
                break;
            case -270:
                RewardCoins(5000);
                break;
            case -225:
                RewardRuby(3);
                break;
            case -180:
                RewardRuby(2);
                break;
            case -135:
                RewardCoins(500);
                break;
            case -90:
                RewardRuby(5);
                break;
            case -45:
                RewardCoins(1000);
                break;
            default:
                RewardCoins(300);
                break;
        }
        if (!isClickAds)
        {
            UserData.Instance.SetTimeSpinStartup();
            LoadData();
        }
    }

    void Update()
    {
        if (!_isStarted)
            return;

        float maxLerpRotationTime = 9f;

        // increment timer once per frame
        
        _currentLerpRotationTime += Time.deltaTime;
        if (_currentLerpRotationTime > maxLerpRotationTime || Circle.transform.eulerAngles.z == _finalAngle)
        {
            //_currentLerpRotationTime = maxLerpRotationTime;
            _isStarted = false;
            _startAngle = _finalAngle % 360;
            GiveAwardByAngle();

            StartCoroutine(HideItemDelta());
        }

        // Calculate current position using linear interpolation
        float t = _currentLerpRotationTime / maxLerpRotationTime;

        // This formulae allows to speed up at start and speed down at the end of rotation.
        // Try to change this values to customize the speed
        t = t * t * t * (t * (6f * t - 15f) + 10f);

        float angle = Mathf.Lerp(_startAngle, _finalAngle, t);
        
        Circle.transform.eulerAngles = new Vector3(0, 0, angle);

    }

    public void RewardCoins(int awardCoins)
    {
        isRuby = false;
        isCoin = true;
        isSkin = false;
        var uiPopup = VKLayerController.Instance.ShowLayer("UIPopupColectItem") as UIPopupColectItem;
        UserData.Instance.GameData.coin += awardCoins;
        uiPopup.InitValue(awardCoins);
        VkAdjustTracker.TrackFeatureLuckySpin(isClickAds ? "spin_video" : "spin_normal", string.Format("{0} coins", awardCoins));
        StartCoroutine(UpdateItemAmount());
    }
    public void RewardRuby(int awardRuby)
    {
        isCoin = false;
        isRuby = true;
        isSkin = false;
        
        var uiPopup = VKLayerController.Instance.ShowLayer("UIPopupColectItem") as UIPopupColectItem;
        UserData.Instance.GameData.ruby += awardRuby;
        uiPopup.InitValue(awardRuby);
        VkAdjustTracker.TrackFeatureLuckySpin(isClickAds ? "spin_video" : "spin_normal", string.Format("{0} ruby", awardRuby));
        StartCoroutine(UpdateItemAmount());
    }
    public void RewardSkin()
    {
        isCoin = false;
        isRuby = false;
        isSkin = true;
        SkinItem skinItem = SkinItemData.Instance.GetSkinRnd();
        var uiPopup = VKLayerController.Instance.ShowLayer("UIPopupColectItem") as UIPopupColectItem;

        if (skinItem != null)
        {
            SkinItemData.Instance.SaveSkinData();
            VkAdjustTracker.TrackFeatureLuckySpin(isClickAds ? "spin_video" : "spin_normal", string.Format("{0}", skinItem.SkinObject.name));
        }
        else
        {
            UserData.Instance.GameData.coin += 10000;
            VkAdjustTracker.TrackFeatureLuckySpin(isClickAds ? "spin_video" : "spin_normal", string.Format("{0} coins", 10000));
        }
        uiPopup.Init(skinItem);
    }


    private IEnumerator HideItemDelta()
    {
        yield return new WaitForSeconds(1f);
        ItemDeltaText.gameObject.SetActive(false);
        ItemDeltaImage.gameObject.SetActive(false);
    }

    private IEnumerator UpdateItemAmount()
    {
        // Animation for increasing and decreasing of coins amount
        const float seconds = 0.5f;
        float elapsedTime = 0;

        while (elapsedTime < seconds)
        {
            elapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
    }
}
