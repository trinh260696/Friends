using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeanTweenManager : MonoBehaviour
{
    public static LeanTweenManager instance;

    [SerializeField] private GameObject[] ItemPage1;
    [SerializeField] private GameObject[] ItemPage2;
    [SerializeField] private GameObject[] ItemPage3;
    [SerializeField] private GameObject[] ItemBoxPage1;
    [SerializeField] private GameObject[] ObjMoveLocal;
    [SerializeField] private GameObject[] ObjScale;

    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        NotificationCenter.DefaultCenter().AddObserver(this, "LoadItems");
        LoadItems();
    }
    public void LoadItems()
    {
        for (int i = 0; i < ItemPage1.Length; i++)
        {
            LeanTween.cancel(ItemPage1[i]);
            LeanTween.scale(ItemPage1[i], new Vector3(0, 0, 0), 0);
            LeanTween.scale(ItemPage1[i], new Vector3(1f, 1f, 1f), 0.4f).setDelay(0.05f + 0.02f * i).setEase(LeanTweenType.easeOutExpo);
        }
        for (int i = 0; i < ItemPage2.Length; i++)
        {
            LeanTween.cancel(ItemPage2[i]);
            LeanTween.scale(ItemPage2[i], new Vector3(0, 0, 0), 0);
            LeanTween.scale(ItemPage2[i], new Vector3(1f, 1f, 1f), 0.4f).setDelay(0.05f + 0.02f * i).setEase(LeanTweenType.easeOutExpo);
        }
        for (int i = 0; i < ItemPage3.Length; i++)
        {
            LeanTween.cancel(ItemPage3[i]);
            LeanTween.scale(ItemPage3[i], new Vector3(0, 0, 0), 0);
            LeanTween.scale(ItemPage3[i], new Vector3(1f, 1f, 1f), 0.4f).setDelay(0.05f + 0.02f * i).setEase(LeanTweenType.easeOutExpo);
        }
        for (int i = 0; i < ItemBoxPage1.Length; i++)
        {
            LeanTween.cancel(ItemBoxPage1[i]);
            LeanTween.scale(ItemBoxPage1[i], new Vector3(0, 0, 0), 0);
            LeanTween.scale(ItemBoxPage1[i], new Vector3(1f, 1f, 1f), 0.4f).setDelay(0.05f + 0.02f * i).setEase(LeanTweenType.easeOutExpo);
        }
    }

    public void OnShowLayer()
    {
        for (int i = 0; i < ObjScale.Length; i++)
        {
            LeanTween.scale(ObjScale[i], new Vector3(0, 0, 0), 0);
            LeanTween.scale(ObjScale[i], new Vector3(1f, 1f, 1f), 0.2f).setEase(LeanTweenType.easeOutExpo);
        }
    }
    public void OnCloseLayer()
    {
        for (int i = 0; i < ObjScale.Length; i++)
        {
            LeanTween.scale(ObjScale[i], new Vector3(0, 0, 0), 0.5f).setEase(LeanTweenType.easeInOutExpo);
        }
    }
}
