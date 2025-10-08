using System;
using System.Collections;
using UnityEngine;
using VKSdk;

namespace VKSdk.UI
{
    public class VKClock : MonoBehaviour
    {
        public VKClockItem[] itemSecs;
        public VKClockItem[] itemMins;

        private float countdown;// seconds
        public float countdownTest;// seconds

        [ContextMenu("Test")]
        public void Test()
        {
            Setup(countdownTest);
        }

        public void Setup(float seconds)
        {
            countdown = seconds;
            ShowTime(false);

            StopAllCoroutines();
            StartCoroutine(ChangeTime());
        }

        private void ShowTime(bool isAnim)
        {
            float time = countdown;

            int hours = (int)(time / 3600);
            time -= hours * 3600;
            int mins = (int)(time / 60);
            time -= mins * 60;
            int secs = (int)time;

            SetText(mins, itemMins, isAnim);
            SetText(secs, itemSecs, isAnim);
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

            VKDebug.LogWarning("num1 + " + num1);
            VKDebug.LogWarning("num2 + " + num2);

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
            while (true)
            {
                yield return new WaitForSeconds(1f);
                countdown -= 1f;

                ShowTime(true);
                if (countdown < 0)
                    break;
            }

        }
    }
}
