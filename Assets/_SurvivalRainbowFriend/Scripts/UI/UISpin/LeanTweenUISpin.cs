using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LeanTweenUISpin : MonoBehaviour
{
    [SerializeField] private GameObject SpinTitle, SpinBtn, SpinAdsBtn, TimeCountDown;
    [SerializeField] private GameObject[] Item;
    [SerializeField] private GameObject[] ObjScale;
    public void OnShowSpinLayer()
    {
        for(int i = 0; i < ObjScale.Length; i++)
        {
            LeanTween.scale(ObjScale[i], new Vector3(0f, 0f, 0f), 0);
            LeanTween.scale(ObjScale[i], new Vector3(1f, 1f, 1f), 0.5f).setEase(LeanTweenType.easeOutBack);
        }
        for (int i = 0; i < Item.Length; i++)
        {
            LeanTween.scale(Item[i], new Vector3(0f, 0f, 0f), 0);
            LeanTween.scale(Item[i], new Vector3(1f, 1f, 1f), 1f).setDelay(0.2f + 0.05f * i).setEase(LeanTweenType.easeOutElastic);
        }

        LeanTween.moveLocal(SpinTitle, new Vector3(0f, 337f, 0f), 1f).setDelay(0f).setEase(LeanTweenType.easeOutExpo);
        LeanTween.moveLocal(SpinBtn, new Vector3(624f, -250f, 0f), 0.5f).setDelay(0f).setEase(LeanTweenType.easeOutExpo);
        LeanTween.moveLocal(SpinAdsBtn, new Vector3(624f, -250f, 0f), 0.5f).setDelay(0f).setEase(LeanTweenType.easeOutExpo);
        LeanTween.moveLocal(TimeCountDown, new Vector3(-603f, 255f, 0f), 0.5f).setDelay(0f).setEase(LeanTweenType.easeOutExpo);
        //LeanTween.scale(TimeCountDown, new Vector3(1f, 1f, 1f), 1f).setDelay(0f).setEase(LeanTweenType.easeOutElastic);
    }
    public void OnCloseSpinLayer()
    {
        for (int i = 0; i < ObjScale.Length; i++)
        {
            LeanTween.scale(ObjScale[i], new Vector3(0, 0, 0), 0.5f).setDelay(0f).setEase(LeanTweenType.easeInBack);
        }
        LeanTween.moveLocal(SpinTitle, new Vector3(0, 900f, 0), 0.5f).setDelay(0f).setEase(LeanTweenType.easeInBack);
        LeanTween.moveLocal(SpinBtn, new Vector3(200f, -100f, 0), 0.5f).setDelay(0.5f).setEase(LeanTweenType.easeInOutExpo);
        LeanTween.moveLocal(SpinAdsBtn, new Vector3(200f, -100f, 0), 0.5f).setDelay(0.5f).setEase(LeanTweenType.easeInOutExpo);
        LeanTween.moveLocal(TimeCountDown, new Vector3(-262, 100f, 0), 0.5f).setDelay(0.5f).setEase(LeanTweenType.easeInOutExpo);
        //LeanTween.scale(TimeCountDown, new Vector3(0, 0, 0), 0.5f).setDelay(0f).setEase(LeanTweenType.easeInBack);
    }

    //public void OnTweenObj()
    //{
    //    for(int i = 0; i < TweenObj.Length; i++)
    //    {
    //        float distance = UnityEngine.Random.Range(1f, 2f);
    //        TweenObj[i].SetActive(true);
    //        LeanTween.moveLocal(TweenObj[i], new Vector3(objCurrent.position.x + distance - 1.5f, objCurrent.position.y + distance, 0), 1f).setDelay(1f + i * 0.1f).setEase(LeanTweenType.easeOutElastic);
    //        LeanTween.moveLocal(TweenObj[i], targetTransform.position, 1f).setDelay(1.5f + i * 0.1f).setEase(LeanTweenType.easeInOutCirc);
    //        LeanTween.alpha(TweenObj[i], 0, 1.5f + i * 0.1f);
    //    }
    //}

}
