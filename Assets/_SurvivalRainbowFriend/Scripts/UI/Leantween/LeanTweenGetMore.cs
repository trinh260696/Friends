using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeanTweenGetMore : MonoBehaviour
{
    public static LeanTweenGetMore instance;
    [SerializeField] private GameObject[] Item;
    [SerializeField] private GameObject[] ObjScale;

    [SerializeField] private float timeDelay = 0;
    [SerializeField] private float timeTweenDelayObj = 0;
    //[SerializeField] private ShowItemType ShowType;

    [SerializeField] public GameObject[] TweenObj;

    [SerializeField] private Transform objCurrent;
    [SerializeField] public Transform coinTransform;
    [SerializeField] public Transform rubyTransform;

    private void Awake()
    {

        if (instance == null)
            instance = this;
    }
    private void Start()
    {
        for (int i = 0; i < TweenObj.Length; i++)
        {
            TweenObj[i].SetActive(false);
        }

        OnShowLayer();
        OnShowItem();
    }
    public void OnTweenObj()
    {
        AudioManager.instance.Play("SuccessReward");
        for (int i = 0; i < TweenObj.Length; i++)
        {
            TweenObj[i].SetActive(true);
            float distanceX = Random.Range(-2f, 2f);
            float distanceY = Random.Range(-1f, 1f);
            if (i == 0)
            {
                TweenObj[i].GetComponent<Image>().sprite = Resources.Load<Sprite>($"Item/Ruby");
                LeanTween.scale(TweenObj[i], new Vector3(2, 2, 2), 0);
                LeanTween.move(TweenObj[i], objCurrent.position, 0);
                LeanTween.move(TweenObj[i], new Vector3(objCurrent.position.x + distanceX, objCurrent.position.y + distanceY, 0), 1f).setDelay(timeTweenDelayObj).setEase(LeanTweenType.easeOutExpo);
                LeanTween.move(TweenObj[i], rubyTransform.position, 1f).setDelay(timeTweenDelayObj + 0.5f + i * 0.1f).setEase(LeanTweenType.easeInOutCirc).setOnComplete(CollectRuby);
                LeanTween.scale(TweenObj[i], new Vector3(0.6f, 0.6f, 1), timeTweenDelayObj + 1.5f + i * 0.1f);
            }
            else
            {
                TweenObj[i].GetComponent<Image>().sprite = Resources.Load<Sprite>($"Item/Coin");
                LeanTween.scale(TweenObj[i], new Vector3(2, 2, 2), 0);
                LeanTween.move(TweenObj[i], objCurrent.position, 0);
                LeanTween.move(TweenObj[i], new Vector3(objCurrent.position.x + distanceY, objCurrent.position.y + distanceY, 0), 1f).setDelay(timeTweenDelayObj).setEase(LeanTweenType.easeOutExpo);
                LeanTween.move(TweenObj[i], coinTransform.position, 1f).setDelay(timeTweenDelayObj + 0.5f + i * 0.1f).setEase(LeanTweenType.easeInOutCirc).setOnComplete(CollectCoin);
                LeanTween.scale(TweenObj[i], new Vector3(0.6f, 0.6f, 1), timeTweenDelayObj + 1.5f + i * 0.1f);
            }
        }
    }
    public void OnTweenCoin()
    {
        AudioManager.instance.Play("SuccessReward");
        for(int i = 0; i < TweenObj.Length; i++)
        {
            TweenObj[i].SetActive(true);
        }
        for (int i = 0; i < TweenObj.Length; i++)
        {
            float distanceX = Random.Range(-2f, 2f);
            float distanceY = Random.Range(-1f, 1f);
            TweenObj[i].GetComponent<Image>().sprite = Resources.Load<Sprite>($"Item/Coin");
            LeanTween.scale(TweenObj[i], new Vector3(2, 2, 1), 0);
            LeanTween.move(TweenObj[i], objCurrent.position, 0);
            LeanTween.move(TweenObj[i], new Vector3(objCurrent.position.x + distanceX, objCurrent.position.y + distanceY, 0), 1f).setDelay(timeTweenDelayObj).setEase(LeanTweenType.easeOutExpo);
            LeanTween.move(TweenObj[i], coinTransform.position, 1f).setDelay(timeTweenDelayObj + 0.5f + i * 0.1f).setEase(LeanTweenType.easeInOutCirc).setOnComplete(CollectCoin);
            LeanTween.scale(TweenObj[i], new Vector3(0.6f, 0.6f, 1), timeTweenDelayObj + 1.5f + i * 0.1f);
        }
    }
    public void OnTweenRuby()
    {
        AudioManager.instance.Play("SuccessReward");
        for (int i = 0; i < TweenObj.Length; i++)
        {
            TweenObj[i].SetActive(true);
        }
        for (int i = 0; i < TweenObj.Length; i++)
        {
            float distanceX = Random.Range(-2f, 2f);
            float distanceY = Random.Range(-1f, 1f);
            TweenObj[i].GetComponent<Image>().sprite = Resources.Load<Sprite>($"Item/Ruby");
            LeanTween.scale(TweenObj[i], new Vector3(2, 2, 1), 0);
            LeanTween.move(TweenObj[i], objCurrent.position, 0);
            LeanTween.move(TweenObj[i], new Vector3(objCurrent.position.x + distanceX, objCurrent.position.y + distanceY, 0), 1f).setDelay(timeTweenDelayObj).setEase(LeanTweenType.easeOutExpo);
            LeanTween.move(TweenObj[i], rubyTransform.position, 1f).setDelay(timeTweenDelayObj + 0.5f + i * 0.1f).setEase(LeanTweenType.easeInOutCirc).setOnComplete(CollectRuby);
            LeanTween.scale(TweenObj[i], new Vector3(0.6f, 0.6f, 1), timeTweenDelayObj + 1.5f + i * 0.1f);
        }
    }
    void CollectCoin()
    {
        AudioManager.instance.Play("CollectCoin");
    }
    void CollectRuby()
    {
        AudioManager.instance.Play("CollectRuby");
    }

    public void OnShowLayer()
    {
        for (int i = 0; i < ObjScale.Length; i++)
        {
            ObjScale[i].transform.localScale = new Vector3(0, 0, 1);
            LeanTween.scale(ObjScale[i], new Vector3(1f, 1f, 1f), 0.5f).setDelay(timeDelay).setEase(LeanTweenType.easeOutExpo);
        }
    }
    public void OnCloseLayer()
    {
        for (int i = 0; i < ObjScale.Length; i++)
        {
            LeanTween.scale(ObjScale[i], new Vector3(0, 0, 0), 0.5f).setDelay(0).setEase(LeanTweenType.easeInOutExpo);
        }
    }
    public void OnShowItem()
    {
        for (int i = 0; i < Item.Length; i++)
        {
            Item[i].transform.localScale = new Vector3(0, 0, 1);
            LeanTween.scale(Item[i], new Vector3(1f, 1f, 1f), 0.5f).setDelay(timeDelay + 0.2f + 0.05f * i).setEase(LeanTweenType.easeOutExpo);
        }
    }
}
