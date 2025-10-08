using UnityEngine;

namespace VKSdk
{
    public class VKAudioItem : MonoBehaviour
    {

        public AudioSource mAudio;
        public VKObjectPoolManager poolManager;

        private bool isPlay = false;

        void Update()
        {
            if (isPlay)
            {
                if (!mAudio.isPlaying)
                {
                    RemoveAudio();
                }
            }
        }

        public void PlayAudio(AudioClip audio, VKObjectPoolManager poolManager, bool isLoop = false)
        {
            this.poolManager = poolManager;
            
            gameObject.SetActive(true);
            mAudio.clip = audio;
            mAudio.loop = isLoop;
            mAudio.Play();

            isPlay = true;
        }

        public void RemoveAudio()
        {
            isPlay = false;
            mAudio.clip = null;
            poolManager.GiveBackObject(gameObject);
        }
    }
}
