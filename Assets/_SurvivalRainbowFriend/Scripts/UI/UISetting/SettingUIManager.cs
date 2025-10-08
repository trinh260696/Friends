using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingUIManager : MonoBehaviour
{
    public void LoadMainStartUI()
    {
        //LoadScene.Instance.LoadSceneAndLoading("MainStartUI");
    }
    public void SoundBtnClick()
    {
        AudioManager.instance.Play("ButtonClick");
    }
}
