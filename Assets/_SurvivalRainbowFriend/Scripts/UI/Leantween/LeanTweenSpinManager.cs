using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class LeanTweenSpinManager : MonoBehaviour
{
    public static LeanTweenSpinManager instance;
    [SerializeField] private GameObject[] Item;
    [SerializeField] private GameObject[] ObjScale;

    [SerializeField] private float timeDelay = 0;
    [SerializeField] private float timeTweenDelayObj = 0;
    //[SerializeField] private ShowItemType ShowType;

    [SerializeField] public GameObject[] TweenObj;
    
    [SerializeField] private Transform objCurrent;
    [SerializeField] public Transform coinTransform;
    [SerializeField] public Transform rubyTransform;

    private void Start()
    {
        if (instance == null) { instance = this; }
        else Destroy(gameObject);

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
        CheckLeanTweenObj();
    }
    void CheckLeanTweenObj()
    {
        if (FortuneWheelManager.instance.isCoin)
        {
            for (int i = 0; i < TweenObj.Length; i++)
            {
                TweenObj[i].SetActive(true);
            }
            for (int i = 0; i < TweenObj.Length; i++)
            {
                TweenObj[i].GetComponent<Image>().sprite = Resources.Load<Sprite>($"Item/Coin");

                float distanceX = Random.Range(-2f, 2f);
                float distanceY = Random.Range(-1f, 1f);
                LeanTween.scale(TweenObj[i], new Vector3(2, 2, 1), 0);
                LeanTween.move(TweenObj[i], new Vector3(objCurrent.position.x + distanceX, objCurrent.position.y + distanceY, 0), 1f).setDelay(timeTweenDelayObj).setEase(LeanTweenType.easeOutExpo);
                LeanTween.move(TweenObj[i], coinTransform.position, 1f).setDelay(timeTweenDelayObj + 0.5f + i * 0.1f).setEase(LeanTweenType.easeInOutCirc).setOnComplete(CollectCoin);
                LeanTween.scale(TweenObj[i], new Vector3(0.65f, 0.65f, 0.65f), 2f + i * 0.1f).setOnComplete(() =>
                {
                    for (int i = 0; i < TweenObj.Length; i++)
                    {
                        TweenObj[i].SetActive(false);
                    }
                });
            }
            return;
        }
        if (FortuneWheelManager.instance.isRuby)
        {
            for (int i = 0; i < TweenObj.Length; i++)
            {
                TweenObj[i].SetActive(true);
            }
            for (int i = 0; i < TweenObj.Length; i++)
            {
                TweenObj[i].GetComponent<Image>().sprite = Resources.Load<Sprite>($"Item/Ruby");

                float distanceX = Random.Range(-2f, 2f);
                float distanceY = Random.Range(-1f, 1f);
                LeanTween.scale(TweenObj[i], new Vector3(2, 2, 1), 0);
                LeanTween.move(TweenObj[i], new Vector3(objCurrent.position.x + distanceX, objCurrent.position.y + distanceY, 0), 1f).setDelay(timeTweenDelayObj).setEase(LeanTweenType.easeOutExpo);
                LeanTween.move(TweenObj[i], rubyTransform.position, 1f).setDelay(timeTweenDelayObj + 0.5f + i * 0.1f).setEase(LeanTweenType.easeInOutCirc).setOnComplete(CollectRuby);
                LeanTween.scale(TweenObj[i], new Vector3(0.65f, 0.65f, 0.65f), 2f + i * 0.1f).setOnComplete(() =>
                {
                    for (int i = 0; i < TweenObj.Length; i++)
                    {
                        TweenObj[i].SetActive(false);
                    }
                });
            }
            return;
        }
        if (FortuneWheelManager.instance.isSkin)
        {
            return;
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
