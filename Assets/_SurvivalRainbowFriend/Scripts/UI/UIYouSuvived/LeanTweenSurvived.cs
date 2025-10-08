using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using VKSdk.UI;

public class LeanTweenSurvived : MonoBehaviour
{
    [SerializeField] private GameObject youSurvivedPanel, youSurvivedTitle, xRewardPanel, BtnxReward;
    [SerializeField] private GameObject[] RewardTween;
    [SerializeField] private GameObject CoinPrefab;
    //[SerializeField] private int MaxCoin;

    [SerializeField] private Transform CoinTransform;
    [SerializeField] private Transform RubyTransform;
    [SerializeField] private Transform CoinCurrent;
    [SerializeField] private Transform RubyCurrent;

    private Action actionComplete;
    public void Start()
    {
        for (int i = 0; i < RewardTween.Length; i++)
        {
            RewardTween[i].SetActive(false);
        }
        ShowPopupsurvived();
    }
    public void ShowPopupsurvived()
    {
        LeanTween.scale(youSurvivedPanel, new Vector3(0, 0, 0), 0);
        LeanTween.scale(youSurvivedTitle, new Vector3(0, 0, 0), 0);
        LeanTween.scale(xRewardPanel, new Vector3(0, 0, 0), 0);
        LeanTween.scale(BtnxReward, new Vector3(0, 0, 0), 0);
        //PrepareCoins();
        LeanTween.scale(youSurvivedPanel, new Vector3(1f, 1f, 1f), 1f).setEase(LeanTweenType.easeOutElastic);
        LeanTween.scale(youSurvivedTitle, new Vector3(1f, 1f, 1f), 1f).setEase(LeanTweenType.easeOutElastic);
        LeanTween.scale(xRewardPanel, new Vector3(1f, 1f, 1f), 1f).setDelay(0.2f).setEase(LeanTweenType.easeOutElastic);
        LeanTween.scale(BtnxReward, new Vector3(1f, 1f, 1f), 1f).setDelay(0.2f).setEase(LeanTweenType.easeOutElastic);
    }

    public void CoidRewardTween(Action action)
    {
        //coin.AddComponent<RectTransform>();
        for (int i = 0; i<RewardTween.Length; i++)
        {
            RewardTween[i].SetActive(true);
        }
        for (int i = 0; i < RewardTween.Length; i++)
        {
            float distanceX = UnityEngine.Random.Range(-2f, 2f);
            float distanceY = UnityEngine.Random.Range(-1f, 1f);
            if (i == 0)
            {
                RewardTween[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Item/Ruby");
                LeanTween.move(RewardTween[i], RubyCurrent.position, 0f);
                LeanTween.scale(RewardTween[i], new Vector3(2, 2, 2), 0);
                LeanTween.move(RewardTween[i], new Vector3(RubyCurrent.position.x + distanceX, RubyCurrent.position.y + distanceY, 0), 1f).setEase(LeanTweenType.easeOutExpo);
                LeanTween.move(RewardTween[i], RubyTransform.position, 1f).setDelay(.5f + i * 0.1f).setEase(LeanTweenType.easeInOutCirc).setOnComplete(CollectRuby);
                LeanTween.scale(RewardTween[i], new Vector3(1, 1, 1), 1).setDelay(.5f + i * 0.1f);
            }
            else
            {
                RewardTween[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Item/Coin");
                LeanTween.move(RewardTween[i], CoinCurrent.position, 0f);
                LeanTween.scale(RewardTween[i], new Vector3(2, 2, 2), 0);
                LeanTween.move(RewardTween[i], new Vector3(CoinCurrent.position.x + distanceX, CoinCurrent.position.y + distanceY, 0), 1f).setEase(LeanTweenType.easeOutExpo);
                LeanTween.move(RewardTween[i], CoinTransform.position, 1f).setDelay(.5f + i * 0.1f).setEase(LeanTweenType.easeInOutCirc).setOnComplete(CollectCoin);
                LeanTween.scale(RewardTween[i], new Vector3(1, 1, 1), 1).setDelay(.5f + i * 0.1f);
            }
           
        }
        actionComplete = action;
        Invoke("OnComplete", 1f);
        Invoke("InvokAction", 1.5f);
    }

    void CollectCoin()
    {
        AudioManager.instance.Play("CollectCoin");
    }
    void CollectRuby()
    {
        AudioManager.instance.Play("CollectRuby");
    }
    void OnComplete()
    {
       
        NotificationCenter.DefaultCenter().PostNotification(this, "UpdateCoin");
        
       
        NotificationCenter.DefaultCenter().PostNotification(this, "UpdateRuby");
        UserData.Instance.SaveLocalData();

    }
    void InvokAction()
    {
        if (actionComplete != null)
        {
            actionComplete();
        }
    }
   
}
