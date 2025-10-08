using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace VKSdk.UI
{
    public class VKCountUpLite : MonoBehaviour
    {
#if VK_TMP_ACTIVE
        public TMPro.TMP_Text txtCountDown;
#else
        public Text txtCountDown;
#endif

        public VKCountDownType typeCountDown;

        public bool showSecondText;
        public bool autoChangeType;

        [HideInInspector]
        public bool isCountDone;

        private DateTime timePause;

        private int seconds; // seconds
        private float secondReal; // seconds

        private float delay; // seconds

        #region Unity Method
        public void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                timePause = DateTime.Now;
            }
            else if (!isCountDone)
            {
                var range = DateTime.Now - timePause;
                float totalSecond = secondReal + (float)range.TotalSeconds;

                SetSeconds(totalSecond);
                StartCountUp();
            }
        }
        #endregion

        public void StartCountUp()
        {
            if (seconds < 0)
            {
                seconds = 0;
                secondReal = 0;
            }

            ShowTime();

            StopAllCoroutines();
            StartCoroutine(ChangeTime());
        }

        public void SetSeconds(float secondTemp)
        {
            StopAllCoroutines();
            isCountDone = true;

            secondReal = secondTemp;
            seconds = (int)secondTemp;
            delay = secondTemp - seconds;

            if (delay > 0)
            {
                seconds += 1;
            }

            ShowTime();
        }

        public float GetSeconds()
        {
            return secondReal < 0f ? 0f : secondReal;
        }

        private void ShowTime()
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);

            string str = seconds.ToString("F0");
            if (seconds < 0)
            {
                str = "0";
            }

            VKCountDownType typeTemp = typeCountDown;

            if(autoChangeType)
            {
                if(t.TotalDays >= 1) typeTemp = VKCountDownType.DAYS;
                else if(t.TotalHours >= 1) typeTemp = VKCountDownType.HOURS;
                else if(t.TotalMinutes >= 1) typeTemp = VKCountDownType.MINUS;
                else typeTemp = VKCountDownType.SECONDS;

                if(typeTemp < typeCountDown) typeTemp = typeCountDown;
            }

            if (typeTemp == VKCountDownType.HOURS)
            {
                str = string.Format("{0:D2}:{1:D2}:{2:D2}",
                    (int)t.TotalHours,
                    t.Minutes,
                    t.Seconds);
            }
            else if (typeTemp == VKCountDownType.MINUS)
            {
                str = string.Format("{0:D2}:{1:D2}",
                    (int)t.TotalMinutes,
                    t.Seconds);
            }
            else if (typeTemp == VKCountDownType.DAYS)
            {
                str = string.Format("{0:D2} {4}, {1:D2}:{2:D2}:{3:D2}",
                    t.Days,
                    t.Hours,
                    t.Minutes,
                    t.Seconds,
                    "DAYS");
                   // VKLanguage.GetI2LanguageTranslation(t.Days > 1 ? "DAYS" : "DAY").ToLower());
            }
            else
            {
                str = string.Format("{0:D2}",
                    (int)t.TotalSeconds);
            }

            if (showSecondText)
            {
                str += "s";
            }

            txtCountDown.text = str;
        }

        public void StopCountUp()
        {
            StopAllCoroutines();
            isCountDone = true;
        }

        IEnumerator ChangeTime()
        {
            isCountDone = false;

            float delayTemp = delay > 0 ? delay : 1f;
            while (true)
            {
                yield return null;
                delayTemp -= Time.deltaTime;
                secondReal += Time.deltaTime;
                if(delayTemp <= 0) break;
            }

            while (true)
            {
                // yield return new WaitForSeconds(1f);
                seconds++;

                ShowTime();

                delayTemp = 1f;
                while (true)
                {
                    yield return null;
                    delayTemp -= Time.deltaTime;
                    secondReal += Time.deltaTime;
                    if(delayTemp <= 0) break;
                }
            }
        }
    }
}