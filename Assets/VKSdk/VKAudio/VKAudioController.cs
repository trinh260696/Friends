using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VKSdk
{
    public class VKAudioController : MonoBehaviour
    {
        public const string PATH = "Sounds/";

        #region Properties
        public VKObjectPoolManager poolVKAudioItem;

        [Space(20)]
        [Header("Music")]
        public float GAME_MUSIC_VOLUME = 1f;
        public float MENU_MUSIC_VOLUME = 1f;
        public AudioSource musicSource;
        public AudioClip musicClipDefault;

        [Space(20)]
        public List<AudioClip> audios;

        // private
        private List<AudioClip> audioCaches;

        public bool isSoundOn;
        public bool isMusicOn;

        // private
        private AudioClip lastMusicClip;
        #endregion

        #region Singleton
        private static VKAudioController instance;

        public static VKAudioController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<VKAudioController>();
                }
                return instance;
            }
        }

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                instance = this;
            }

            audioCaches = new List<AudioClip>();
            DontDestroyOnLoad(this.gameObject);
        }

        #endregion

        #region Music
        public void PlayMusic(string musicName, bool isGame=false)
        {
            PlayMusic(GetAudioByName(musicName),isGame);
        }

        public void PlayMusic(AudioClip musicClip, bool isGame=false)
        {
            if (musicClip == null)
            {
                musicClip = musicClipDefault;
            }

            if (isMusicOn)
            {
                if (musicSource.clip != null)
                {
                    if (musicSource.clip.Equals(musicClip))
                    {
                        return;
                    }
                    LeanTween.cancel(gameObject);

                    // cho bé dần nhạc đang chạy
                    StopMusic(() => {
                        if (lastMusicClip == null)
                        {
                            lastMusicClip = musicSource.clip;
                        }
                        else if (lastMusicClip == musicClip)
                        {
                            lastMusicClip = null;
                        }
                        // chuyển sang nhạc mới
                        musicSource.clip = musicClip;
                        StartMusic(isGame);
                    });
                }
                else
                {
                    LeanTween.cancel(gameObject);

                    musicSource.clip = musicClip;
                    StartMusic(isGame);
                }
            }
            else
            {
                LeanTween.cancel(gameObject);
                musicSource.clip = musicClip;
            }
        }

        public void PlayMusicLast()
        {
            if (lastMusicClip == null)
            {
                return;
            }
            AudioClip musicClip = lastMusicClip;
            PlayMusic(musicClip);
        }

        public void StopMusic(Action onStopDone)
        {
            musicSource.clip = null;
            if (isMusicOn)
            {
                float numStart = musicSource.volume;

                LeanTween.value(gameObject, (value) => {
                    musicSource.volume = value;
                }, numStart, 0f, 1f).setOnComplete(() => {
                    musicSource.volume = 0f;
                    musicSource.Stop();

                    if (onStopDone != null)
                    {
                        onStopDone.Invoke();
                    }
                });
            }
            else
            {
                musicSource.volume = 0f;

                if (onStopDone != null)
                {
                    onStopDone.Invoke();
                }
            }
        }

        public void StartMusic(bool isGame=false)
        {
            musicSource.loop = true;
            musicSource.volume = 0f;
            float MAX_MUSIC_VOLUME = isGame ? GAME_MUSIC_VOLUME : MENU_MUSIC_VOLUME;
            if (isMusicOn)
            {
                musicSource.Play();

                LeanTween.value(gameObject, (value) => {
                    musicSource.volume = value;
                }, 0f, MAX_MUSIC_VOLUME, 1f).setOnComplete(() => {
                    musicSource.volume = MAX_MUSIC_VOLUME;
                });
            }
        }
        #endregion

        #region Sound
        public VKAudioItem PlaySound(string soundName, bool isLoop = false)
        {
            if (isSoundOn)
            {
                return PlaySound(GetAudioByName(soundName));
            }
            return null;
        }

        public VKAudioItem PlaySound(AudioClip audio, bool isLoop = false)
        {
            if (isSoundOn)
            {
                if (audio != null)
                {
                    VKAudioItem item = poolVKAudioItem.BorrowObject<VKAudioItem>();
                    item.PlayAudio(audio, poolVKAudioItem, isLoop);

                    return item;
                }
            }

            return null;
        }

        private AudioClip GetAudioByName(string name)
        {
            // lấy ở clips default
            AudioClip aClip = audios.FirstOrDefault(a => a.name.Equals(name));
            if (aClip != null)
            {
                return aClip;
            }

            // lấy ở clips cache
            aClip = audioCaches.FirstOrDefault(a => a.name.Equals(name));
            if (aClip != null)
            {
                return aClip;
            }

            // lấy ở dưới resource
            aClip = Resources.Load(PATH + name) as AudioClip;
            if (aClip != null)
            {
                aClip.name = name;
                audioCaches.Add(aClip);

                return aClip;
            }

            // không có ở đâu cả
            return null;
        }

        public void ClearAudioCache()
        {
            audioCaches.Clear();
        }
        #endregion
    }
}
