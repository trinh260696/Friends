using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VKSdk.UI;
public class UILoseConnection : VKLayer
{
    public void ClickSoundBtn()
    {
        AudioManager.instance.Play("ButtonClick");
        StartCoroutine(checkInternetConnection());
    }
    IEnumerator checkInternetConnection()
    {

        WWW www = new WWW("http://google.com");
        yield return www;
        if (www.error != null)
        {
            yield break;
        }
        else
        {
            SupportThisGame.Instance.TurnOnCheck();
            Close();
        }
    }
}
