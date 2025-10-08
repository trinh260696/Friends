using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VKSdk.UI
{
    public class VKCountDownLite : MonoBehaviour
    {
#if VK_TMP_ACTIVE
        public TMPro.TMP_Text txtCountDown;
#else
        public Text txtCountDown;
#endif

        public float countdownTest; // seconds

        public VKCountDownType typeCountDown;

        public Action OnCountDownComplete;
        public Action OnShowSpecial;
        public Action<int> OnChangeNumber;

        public Color colorNormal;
        public Color colorSpecial;

        public int timeShowSpecial;
        public bool showSecondText;
        public bool autoChangeType;
        public Action actionComplete;
        [HideInInspector]
        public bool isCountDone;
        private bool isShowSpecial;

        private DateTime timePause;

        private int seconds; // seconds
        private float secondReal; // seconds

        private float delay; // seconds

        //public void OnDisable()
        //{
        //    StopCountDown();
        //}

        [ContextMenu("Test")]
        public void Test()
        {
            // StartCountDown(countdownTest);
        }

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
                float totalSecond = secondReal - (float)range.TotalSeconds;

                if(totalSecond < 0) totalSecond = 0;

                SetSeconds(totalSecond);
                if (totalSecond > 0)
                {
                    StartCountDown(actionComplete);
                }
                else
                {
                    if (OnCountDownComplete != null)
                        OnCountDownComplete.Invoke();
                    if (actionComplete != null)
                    {
                        actionComplete();
                    }
                }
            }
        }
        #endregion

        // public void StartCountDown(double seconds)
        // {
        //     StartCountDown((float)seconds);
        // }

        // public void StartCountDown(float seconds)
        // {
        //     countdown = seconds;
        //     countdownReal = seconds;
        //     StartCountDown();
        // }

        public void StartCountDown(Action action)
        {
             actionComplete= action;
            if (seconds < 0)
            {
                seconds = 0;
                secondReal = 0;
            }

            if (isShowSpecial)
            {
                txtCountDown.color = colorNormal;
                isShowSpecial = false;
            }

            ShowTime();

            StopAllCoroutines();
            StartCoroutine(ChangeTime());
        }

        public void SetSeconds(float secondTemp)
        {
            StopAllCoroutines();
            isCountDone = true;

            if (isShowSpecial)
            {
                txtCountDown.color = colorNormal;
                isShowSpecial = false;
            }

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
                    t.Days > 1 ? "DAYS" : "DAY");
            }
            else
            {
                str = string.Format("{0:D2}",
                    (int)t.TotalSeconds);
            }

            if (timeShowSpecial > 0 && !isShowSpecial)
            {
                if (seconds <= timeShowSpecial)
                {
                    isShowSpecial = true;
                    txtCountDown.color = colorSpecial;

                    if (OnShowSpecial != null)
                    {
                        OnShowSpecial.Invoke();
                    }
                }
            }

            if (showSecondText)
            {
                str += "s";
            }

            txtCountDown.text = str;
        }

        public void ShowTextOnly(string str, bool isSpecial = false)
        {
            StopAllCoroutines();
            isCountDone = true;
            isShowSpecial = isSpecial;

            seconds = 0;
            secondReal = 0;
            txtCountDown.color = isShowSpecial ? colorSpecial : colorNormal;
            txtCountDown.text = str;
        }

        public void ShowTextOnly(string str, Color cText)
        {
            StopAllCoroutines();
            isCountDone = true;
            isShowSpecial = true;

            seconds = 0;
            secondReal = 0;
            txtCountDown.color = cText;
            txtCountDown.text = str;
        }

        public void SetColor(Color cText, bool stopAll = false)
        {
            if(stopAll)
            {
                StopAllCoroutines();
                isCountDone = true;
                seconds = 0;
                secondReal = 0;
            }
            isShowSpecial = cText != colorNormal;
            txtCountDown.color = cText;
        }

        public void StopCountDown()
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
                secondReal -= Time.deltaTime;
                if(delayTemp <= 0) break;
            }

            while (true)
            {
                // yield return new WaitForSeconds(1f);
                seconds--;

                if (seconds < 0)
                    seconds = 0;

                if (OnChangeNumber != null)
                {
                    OnChangeNumber.Invoke(seconds);
                }

                ShowTime();
                if (seconds <= 0)
                {
                    isCountDone = true;
                    if (OnCountDownComplete != null)
                    {
                        OnCountDownComplete.Invoke();
                    }
                    if (actionComplete != null)
                    {
                        actionComplete();
                    }
                    break;
                }

                delayTemp = 1f;
                while (true)
                {
                    yield return null;
                    delayTemp -= Time.deltaTime;
                    secondReal -= Time.deltaTime;
                    if(delayTemp <= 0) break;
                }
            }
        }
    }
}