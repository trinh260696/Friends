using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using VKSdk.UI;

public class GetMoreRubyAndCoin : MonoBehaviour
{
    

    [SerializeField] private TextMeshProUGUI textMeshProCoin;
    [SerializeField] private TextMeshProUGUI subtextMeshProCoin;
    [SerializeField] private TextMeshProUGUI textMeshProRuby;
    [SerializeField] private TextMeshProUGUI subtextMeshProRuby;
    int coin;
    int ruby;

    public int Coin { get => coin; 
        set {
            int from = coin;
            subtextMeshProCoin.gameObject.SetActive(true);
            if (from < value)
                subtextMeshProCoin.text = string.Format("+{0}", (int)(value) - from);
            else
                subtextMeshProCoin.text = string.Format("{0}", (int)(value) - from);

            LeanTween.value(textMeshProCoin.gameObject, (v) =>
            {
                textMeshProCoin.text = ((int)(v)).ToString();
                
            }, from,  value, 1f).setOnComplete(() => { coin = value; subtextMeshProCoin.gameObject.SetActive(false); });
            StartCoroutine(play_sound("coin", 10));
        } }
    public int Ruby { get => ruby; set {
            int from = ruby;
            subtextMeshProRuby.gameObject.SetActive(true);
            if (from < value)
                subtextMeshProRuby.text = string.Format("+{0}", (int)(value) - from);
            else
                subtextMeshProRuby.text = string.Format("{0}", (int)(value) - from);
            LeanTween.value(textMeshProRuby.gameObject, (v) =>
            {
                textMeshProRuby.text = ((int)(v)).ToString();              
            }, from, value, 1f).setOnComplete(()=> { ruby = value; subtextMeshProRuby.gameObject.SetActive(false); AudioManager.instance.Play("ruby"); });
           
        }
    }
    IEnumerator play_sound(string name, int number)
    {
        for(int t=0; t<number; t++)
        {
            AudioManager.instance.Play(name);
            yield return new WaitForSeconds(0.2f);
        }
    }
    private void Update()
    {
        
    }
    private void Awake()
    {
        coin = UserData.Instance.GameData.coin;
        ruby = UserData.Instance.GameData.ruby;
        textMeshProCoin.text = UserData.Instance.GameData.coin.ToString();
        textMeshProRuby.text = UserData.Instance.GameData.ruby.ToString();
        subtextMeshProCoin.gameObject.SetActive(false);
        subtextMeshProRuby.gameObject.SetActive(false);
        NotificationCenter.DefaultCenter().AddObserver(this, "UpdateCoin");
        NotificationCenter.DefaultCenter().AddObserver(this, "UpdateRuby");
    }
   
    public void UpdateCoin()
    {                     
        Coin = UserData.Instance.GameData.coin;
    }
    public void UpdateRuby()
    {
        Ruby = UserData.Instance.GameData.ruby;
    }
    public void GetMoreRuby()
    {
        if (FortuneWheelManager._isStarted)
        {
            return;
        }
        AudioManager.instance.Play("ButtonClick");
        VKLayerController.Instance.ShowLayer("UIPopupGetMoreRuby");
    }
    public void GetMoreCoin()
    {
        if (FortuneWheelManager._isStarted)
        {
            return;
        }
        AudioManager.instance.Play("ButtonClick");
        VKLayerController.Instance.ShowLayer("UIPopupGetMoreCoin");
    }
}
