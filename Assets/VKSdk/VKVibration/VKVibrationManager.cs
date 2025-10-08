using System.Collections;
using UnityEngine;

namespace VKSdk
{
    public class VKVibrationManager : MonoBehaviour
    {
        private IEnumerator ieVibration;

        private float duration;
        private float delay;

        // private
        private float countdown;

        #region Singleton
        private static VKVibrationManager instance;

        public static VKVibrationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<VKVibrationManager>();
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

            DontDestroyOnLoad(this.gameObject);
        }
        #endregion

        public void VibrateOne()
        {
#if UNITY_IOS || UNITY_ANDROID
            Handheld.Vibrate();
#endif
        }

        public void StartVibrate(float duration, float delayFirst, float delayTurn)
        {
            this.duration = duration;
            this.delay = delayTurn;

            this.countdown = duration;

            StopVibrate();

            if(ieVibration == null)
            {
                ieVibration = WaitToVibrate(delayFirst);
                StartCoroutine(ieVibration);
            }
        }

        public void StopVibrate()
        {
            if(ieVibration != null) StopCoroutine(ieVibration);
            ieVibration = null;
        }

        IEnumerator WaitToVibrate(float delayFirst)
        {
            yield return new WaitForSeconds(delayFirst);
#if UNITY_IOS || UNITY_ANDROID
            while(countdown > 0)
            {
                Handheld.Vibrate();
                yield return new WaitForSeconds(delay);
                countdown -= delay;
            }
#endif
        }
    }
}