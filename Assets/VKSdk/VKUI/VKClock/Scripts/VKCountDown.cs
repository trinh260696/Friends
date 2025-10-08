using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
#if VK_XLUA
using XLua;
#endif

namespace VKSdk.UI
{
    public enum VKCountDownType
    {
        SECONDS,
        MINUS,
        HOURS,
        DAYS
    }

    public class VKCountDown : MonoBehaviour
    {
        public VKClockItem[] itemSecs;
        public VKClockItem[] itemMins;
        public VKClockItem[] itemHours;
        public VKClockItem[] itemDays;

        public VKCountDownType type;

        [Space(20)]
        [Header("SPECIAL")]
        public Color colorNormal;
        public Color colorSpecial;
        public int timeShowSpecial;

        [Space(10)]
        public CanvasGroup canvasGroup;
        public int timeShowCanvas;

        
        private bool isShowSpecial;
        private bool isShowed;

#if VK_XLUA
        [CSharpCallLua]
#endif
        [Serializable]
        public class ShowSpecialDelegate : UnityEvent { } // code, status, data, baseData
#if VK_XLUA
        [CSharpCallLua]
#endif
        [SerializeField]
        public ShowSpecialDelegate OnShowSpecial = new ShowSpecialDelegate();

#if VK_XLUA
        [CSharpCallLua]
#endif
        [Serializable]
        public class CountDownCompleteDelegate : UnityEvent { } // code, status, data, baseData
#if VK_XLUA
        [CSharpCallLua]
#endif
        [SerializeField]
        public CountDownCompleteDelegate OnCountDownComplete = new CountDownCompleteDelegate();

        private int days;
        private int hours;
        private int minus;
        private int seconds;

        private float delay;

        public float countdownTest;// seconds

        public bool isCountDone = false;
        public bool isCounDown = false;

        public DateTime timePause;

        private float secondReal;

        [ContextMenu("Test")]
        public void Test()
        {
            SetSeconds(countdownTest);
        }

        [ContextMenu("Test2")]
        public void StartTest()
        {
            StartCountDown();
        }

        public void SetSeconds(float secondTemp)
        {
            if (gameObject.activeSelf)
                StopCountDown();

            isCountDone = false;
            isShowSpecial = timeShowSpecial > 0 && secondTemp <= timeShowSpecial;
            isShowed = timeShowCanvas > 0 && secondTemp <= timeShowCanvas;

            if(canvasGroup != null)
            {
                canvasGroup.alpha = isShowed ? 1f : 0f;
            }

            secondReal = secondTemp;

            days = 0;
            hours = 0;
            minus = 0;
            seconds = (int)secondTemp;
            delay = secondTemp - seconds;

            if (type != VKCountDownType.SECONDS)
            {
                TimeSpan time = TimeSpan.FromSeconds(secondTemp);
                days = time.Days;
                hours = time.Hours;
                minus = time.Minutes;
                seconds = time.Seconds;
            }

            if (delay > 0)
            {
                seconds += 1;
            }

            if (itemDays.Length > 0)
                SetText(days, itemDays, false);
            if (itemHours.Length > 0)
                SetText(hours, itemHours, false);
            if (itemMins.Length > 0)
                SetText(minus, itemMins, false);

            SetText(seconds, itemSecs, false);
        }

        public void AddListener(UnityAction actionDone, UnityAction actionSpecial)
        {
            OnCountDownComplete.AddListener(actionDone);
            OnShowSpecial.AddListener(actionSpecial);
        }

        public void RemoveListener(UnityAction actionDone, UnityAction actionSpecial)
        {
            OnCountDownComplete.RemoveListener(actionDone);
            OnShowSpecial.RemoveListener(actionSpecial);
        }

        void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                timePause = DateTime.Now;
            }
            else
            {
                TimeSpan timeRange = DateTime.Now - timePause;
                double secondRange = timeRange.TotalSeconds;

                if (gameObject.activeSelf && isCounDown)
                {
                    double totalSecond = days * 24 * 60 * 60 + hours * 60 * 60 + minus * 60 + seconds;
                    totalSecond -= secondRange;
                    if (totalSecond < 0)
                        totalSecond = 0;

                    isCounDown = false;
                    SetSeconds((float)totalSecond);
                    if (totalSecond > 0)
                        StartCountDown();
                    else
                    {
                        isCountDone = true;
                        if (OnCountDownComplete != null)
                            OnCountDownComplete.Invoke();
                    }
                }
            }
        }

        public void StartCountDown()
        {
            if (gameObject.activeSelf)
                StartCoroutine(ChangeTime());
        }

        public void StopCountDown()
        {
            if (gameObject.activeSelf)
                LeanTween.cancel(gameObject);
            StopAllCoroutines();
        }

        public float GetSeconds()
        {
            return secondReal < 0f ? 0f : secondReal;
        }

        private void SetText(int num, VKClockItem[] texts, bool isAnim)
        {
            int num1 = 0;
            int num2 = 0;
            if (num < 10)
            {
                num2 = num;
            }
            else
            {
                string str = num.ToString();
                char[] chars = str.ToCharArray();

                num1 = (int)Char.GetNumericValue(chars[0]);
                num2 = (int)Char.GetNumericValue(chars[1]);
            }

            if(timeShowSpecial > 0)
            {
                var c = isShowSpecial ? colorSpecial : colorNormal;
                texts[0].SetColor(c);
                texts[1].SetColor(c);
            }

            if (isAnim)
            {
                texts[0].StartAminText(num1);
                texts[1].StartAminText(num2);
            }
            else
            {
                texts[0].SetText(num1);
                texts[1].SetText(num2);
            }
        }

        IEnumerator ChangeTime()
        {
            isCounDown = true;

            float delayTemp = delay > 0 ? delay : 1f;
            while (true)
            {
                yield return null;
                delayTemp -= Time.deltaTime;
                secondReal -= Time.deltaTime;
                if(delayTemp <= 0) break;
            }

            // yield return new WaitForSeconds(delay > 0 ? delay : 1f);
            while (true)
            {
                seconds--;
                if (timeShowSpecial > 0 && !isShowSpecial)
                {
                    if (seconds <= timeShowSpecial)
                    {
                        isShowSpecial = true;

                        if (OnShowSpecial != null)
                        {
                            OnShowSpecial.Invoke();
                        }
                    }
                }

                if (canvasGroup != null && timeShowCanvas > 0 && !isShowed)
                {
                    if (seconds <= timeShowCanvas)
                    {
                        isShowed = true;

                        LeanTween.value(gameObject,(valueTemp) =>  {
                            canvasGroup.alpha = valueTemp;
                        }, 0f, 1f, 0.25f).setOnComplete(() => canvasGroup.alpha = 1f);
                    }
                }

                // normal
                if (seconds < 0)
                {
                    if (type == VKCountDownType.SECONDS)
                    {
                        break;
                    }
                    else
                    {
                        minus--;
                        if (minus < 0)
                        {
                            if (type == VKCountDownType.MINUS)
                            {
                                break;
                            }
                            else
                            {
                                hours--;
                                if (hours < 0)
                                {
                                    if (type == VKCountDownType.HOURS)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        days--;
                                        if (days < 0)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            SetText(days, itemDays, true);
                                        }
                                    }

                                    hours = 23;
                                    SetText(hours, itemHours, true);
                                }
                                else
                                {
                                    SetText(hours, itemHours, true);
                                }
                            }

                            minus = 59;
                            SetText(minus, itemMins, true);
                        }
                        else
                        {
                            SetText(minus, itemMins, true);
                        }
                    }

                    seconds = 59;
                    SetText(seconds, itemSecs, true);
                }
                else
                {
                    SetText(seconds, itemSecs, true);
                }

                delayTemp = 1f;
                while (true)
                {
                    yield return null;
                    delayTemp -= Time.deltaTime;
                    secondReal -= Time.deltaTime;
                    if(delayTemp <= 0) break;
                }
                // yield return new WaitForSeconds(1f);
            }

            isCountDone = true;
            isCounDown = false;
            if (OnCountDownComplete != null)
                OnCountDownComplete.Invoke();
        }
    }
}