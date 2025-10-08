using UnityEngine;
using UnityEngine.UI;
using VKSdk;

namespace VKSdk.UI
{
    public class VKTextValueChange : MonoBehaviour
    {
#if VK_TMP_ACTIVE
        public TMPro.TMP_Text txtNumber;
#else
        public Text txtNumber;
#endif
        public bool isMoney;
        public bool isSubMoney;

        public float timeRun = 1f;
        public float minSubMoney = 0f;

        private LTDescr ltDescr;

        private float number;
        [HideInInspector]
        public float targetNumber;

        public void SetTimeRun(float timeRun)
        {
            if (ltDescr != null)
                return;

            this.timeRun = timeRun;
        }

        public void SetNumber(double num)
        {
            SetNumber((float)num);
        }

        public void SetNumber(float num)
        {
            if (ltDescr != null)
            {
                LeanTween.cancel(ltDescr.id);
                ltDescr = null;
            }

            this.number = num;
            this.targetNumber = num;

            ShowText(num);
        }

        private void ShowText(float num)
        {
            if (isMoney)
            {
                if (isSubMoney && num >= minSubMoney)
                {
                    txtNumber.text = VKCommon.ConvertSubMoneyString(num);
                }
                else
                {
                    txtNumber.text = VKCommon.ConvertStringMoney(num);
                }
            }
            else
            {
                txtNumber.text = num.ToString();
            }
        }

        private void ShowOnlyText(string str)
        {
            txtNumber.text = str;
        }

        public void UpdateNumber(double newNumber)
        {
            UpdateNumber((float)newNumber);
        }

        public void UpdateNumber(float newNumber)
        {
            this.targetNumber = newNumber;

            if (ltDescr != null)
            {
                LeanTween.cancel(ltDescr.id);
                ltDescr = null;
            }

            ltDescr = LeanTween.value(gameObject, UpdateNewValue, number, newNumber, timeRun).setOnComplete(() =>
            {
                ltDescr = null;
                number = newNumber;
                ShowText(number);
            });
        }

        public void StopValueChange()
        {
            if (ltDescr != null)
            {
                LeanTween.cancel(ltDescr.id);
                ltDescr = null;
            }

            ShowText(targetNumber);
        }

        private void UpdateNewValue(float newNumber)
        {
            number = newNumber;
            ShowText(number);
        }
    }
}