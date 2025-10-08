using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VKSdk.UI;

public class UISetting : VKLayer
{
    
    AudioManager audioManager;

    [SerializeField] private TMP_InputField nameUser;

    [SerializeField] private GameObject SoundOn;
    [SerializeField] private GameObject SoundOff;
    [SerializeField] private GameObject MusicOn;
    [SerializeField] private GameObject MusicOff;
    [SerializeField] private GameObject VibrationOn;
    [SerializeField] private GameObject VibrationOff;

    bool isSound, isMusic, isVibration;

    #region override
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool Equals(object other)
    {
        return base.Equals(other);
    }

    public override string ToString()
    {
        return base.ToString();
    }

    public override void ReloadCanvasScale(float screenRatio, float screenScale)
    {
        base.ReloadCanvasScale(screenRatio, screenScale);
    }

    public override void StartLayer()
    {
        base.StartLayer();
    }

    public override void FirstLoadLayer()
    {
        base.FirstLoadLayer();
    }


    public override void EnableLayer()
    {
        base.EnableLayer();
    }

    public override void ReloadLayer()
    {
        base.ReloadLayer();
    }

    public override void BeforeHideLayer()
    {
        base.BeforeHideLayer();
    }

    public override void DisableLayer()
    {
        base.DisableLayer();
    }

    public override void HideLayer()
    {
        base.HideLayer();
    }

    public override void DestroyLayer()
    {
        base.DestroyLayer();
    }

    public override void Close()
    {
        base.Close();
    }

    public override void OnLayerOpenDone()
    {
        base.OnLayerOpenDone();
    }

    public override void OnLayerCloseDone()
    {
        base.OnLayerCloseDone();
    }

    public override void OnLayerOpenPopupDone()
    {
        base.OnLayerOpenPopupDone();
    }

    public override void OnLayerPopupCloseDone()
    {
        base.OnLayerPopupCloseDone();
    }

    public override void OnLayerSlideHideDone()
    {
        base.OnLayerSlideHideDone();
    }

    public override void OnLayerReOpenDone()
    {
        base.OnLayerReOpenDone();
    }
    #endregion


    public override void ShowLayer()
    {
        base.ShowLayer();
        Init();
    }
    private void Awake()
    {
        audioManager = AudioManager.instance;
    }
    private void Start()
    {
        //controller = VKLayerController.Instance;
        SaveSetting.instance.LoadSaveSetting();
    }

    public void Init()
    {
        nameUser.text = UserData.Instance.GameData.name;
        isSound = SaveSetting.instance.settingData.isSoundOn;
        isMusic = SaveSetting.instance.settingData.isMusicOn;
        isVibration = SaveSetting.instance.settingData.isVibrationOn;
        LoadLocalSetting();
    }

    public void CloseBtn()
    {
        audioManager.Play("ButtonClick");
        if (nameUser.text != UserData.Instance.GameData.name)
        {
            VkAdjustTracker.TrackDesignClickEditName();
            UIPopup.OpenPopup("Change name", "Now, your name is " + nameUser.text, false);
        }
        UserData.Instance.GameData.name = nameUser.text;
        UserData.Instance.SaveLocalData();
        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name!= "MainStartUI")
        {
            NotificationCenter.DefaultCenter().PostNotification(this, "ChangeName");
           // GameManager.Instance.OnResumeGame();
        }         
        SaveSetting.instance.SaveLocalSetting();
        LeanTweenSpinManager.instance.OnCloseLayer();
        Invoke("OnClose", 0.5f);
    }

    void OnClose()
    {
        Close();
    }
    void LoadLocalSetting()
    {
        ButtonMusicStatus();
        ButtonSoundStatus();
        ButtonVibrationStatus();
        SaveSetting.instance.LoadSaveSetting();
    }

    public void ButtonSound()
    {
        if (SaveSetting.instance.settingData.isSoundOn)
        {
            ButtonSoundStatus();
            ButtonIsSound();
            SaveSetting.instance.settingData.isSoundOn = false;
        }
        else
        {
            ButtonSoundStatus();
            ButtonIsSound();
            SaveSetting.instance.settingData.isSoundOn = true;
        }
        audioManager.Play("ButtonClick");
        SaveSetting.instance.SaveLocalSetting();
    }
    public void ButtonMusic()
    {
        audioManager.Play("ButtonClick");
        if (SaveSetting.instance.settingData.isMusicOn)
        {
            ButtonMusicStatus();
            ButtonIsMusic();
            SaveSetting.instance.settingData.isMusicOn = false;
            VkAdjustTracker.TrackDesignClickMusicOff();
        }
        else
        {
            ButtonMusicStatus();
            ButtonIsMusic();
            SaveSetting.instance.settingData.isMusicOn = true;
            VkAdjustTracker.TrackDesignClickMusicOn();
        }
        SaveSetting.instance.SaveLocalSetting();
    }
    public void ButtonVibration()
    {
        audioManager.Play("ButtonClick");
        if (SaveSetting.instance.settingData.isVibrationOn)
        {
            ButtonVibrationStatus();
            SaveSetting.instance.settingData.isVibrationOn = false;
        }
        else
        {
            ButtonVibrationStatus();
            SaveSetting.instance.settingData.isVibrationOn = true;
        }
        SaveSetting.instance.SaveLocalSetting();
    }

    #region SoundStatus
    public void ButtonIsSound()
    {
        if (!SaveSetting.instance.settingData.isSoundOn)
        {
            for (int i = 2; i < audioManager.sounds.Length; i++)
            {
                audioManager.sounds[i].source.volume = 1;
            }
        }
        else
        {
            for (int i = 2; i < audioManager.sounds.Length; i++)
            {
                audioManager.sounds[i].source.volume = 0;
            }
        }
    }
    void ButtonSoundStatus()
    {
        if (isSound)
        {
            SoundOff.SetActive(false);
            SoundOn.SetActive(true);
            isSound = false;
            VkAdjustTracker.TrackDesignClickSoundOn();
        }
        else
        {
            SoundOn.SetActive(false);
            SoundOff.SetActive(true);
            isSound = true;
            VkAdjustTracker.TrackDesignClickSoundOff();
        }
    }
    #endregion

    #region MusicStatus
    public void ButtonIsMusic()
    {
        if (!SaveSetting.instance.settingData.isMusicOn)
        {
            if (StaticData.IsPlay)
            {
                AudioManager.instance.sounds[0].source.Stop();
                AudioManager.instance.sounds[1].source.Play();
            }
            else
            {
                AudioManager.instance.sounds[0].source.Play();
                AudioManager.instance.sounds[1].source.Stop();
            }
        }
        else
        {
            AudioManager.instance.sounds[0].source.Stop();
            AudioManager.instance.sounds[1].source.Stop();
        }

    }
    void ButtonMusicStatus()
    {
        if (isMusic)
        {
            MusicOff.SetActive(false);
            MusicOn.SetActive(true);
            isMusic = false;
            //AudioManager.instance.sounds[0].source.Play();
        }
        else
        {
            MusicOn.SetActive(false);
            MusicOff.SetActive(true);
            isMusic = true;
            //AudioManager.instance.sounds[0].source.Stop();
        }
    }
    #endregion

    void ButtonVibrationStatus()
    {
        if (isVibration)
        {
            VibrationOn.SetActive(true);
            VibrationOff.SetActive(false);
            isVibration = false;
        }
        else
        {
            VibrationOn.SetActive(false);
            VibrationOff.SetActive(true);
            isVibration = true;
        }
    }

    public void BackToMainUI()
    {
        LoadScene.Instance.LoadSceneAndLoading("MainStartUI");
        Close();
    }
}
