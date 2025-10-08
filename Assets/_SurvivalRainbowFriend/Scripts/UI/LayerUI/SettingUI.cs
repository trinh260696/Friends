using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingUI : MonoBehaviour
{
    UiManager uiManager;
    AudioManager audioManager;

    public GameObject SoundOn;
    public GameObject SoundOff;

    private bool isSound;

    private void Start()
    {
        uiManager = UiManager.instance;
        audioManager = AudioManager.instance;

        PlayerPrefs.GetFloat("Sound");
    }

    private void Update()
    {
        
    }


    public void CloseBtnClick()
    {
        //uiManager.SettingUI.SetActive(false);
        audioManager.Play("ButtonClick");
        Destroy(uiManager.SettingUI, 0.2f);
    }

    public void ButtonSound()
    {
        audioManager.Play("ButtonClick");
        if (isSound)
        {
            ButtonSoundStatus(isSound);
            isSound = false;
        }
        else if (isSound == false)
        {
            ButtonSoundStatus(isSound);
            isSound = true;
        }
    }

    void ButtonSoundStatus(bool on)
    {
        on = isSound;
        if (on)
        {
            SoundOff.SetActive(false);
            SoundOn.SetActive(true);
            AudioListener.volume = 1;
        }
        else
        {
            SoundOn.SetActive(false);
            SoundOff.SetActive(true);
            AudioListener.volume = 0;
        }
    }
}
