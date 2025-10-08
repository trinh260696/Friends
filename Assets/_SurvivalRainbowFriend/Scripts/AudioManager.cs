using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sound[] sounds;
   
    //public AudioSource musicBackGround;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

            
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.pitch = s.pitch;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        if (!SaveSetting.instance.settingData.isMusicOn) return;
        playGeneral("Music");
    }

    public void Play(string name)
    {
        if (!SaveSetting.instance.settingData.isSoundOn) return;
        playGeneral(name);
    }
    void playGeneral(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }
    public void BtnClick()
    {
        Play("ButtonClick");
    }
    public void PlayMusic(bool isInGame)
    {
        if (SaveSetting.instance.settingData.isMusicOn)
        {
            if (isInGame)
            {
                AudioManager.instance.sounds[0].source.Stop();
                AudioManager.instance.sounds[1].source.Play();
            }
            else
            {
                AudioManager.instance.sounds[1].source.Stop();
                AudioManager.instance.sounds[0].source.Play();
            }
            
        }
        
    }
}
