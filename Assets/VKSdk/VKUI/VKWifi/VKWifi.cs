using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VKSdk.UI
{
    public class VKWifi : MonoBehaviour
    {
        public Image imgWifi;
#if VK_TMP_ACTIVE
        public TMPro.TMP_Text txtWifi;
#else
        public Text txtWifi;
#endif

        public Sprite[] sprWifis;

        public Color cGood;
        public Color cBad;

        public GameObject goWifiBadNotify;

        public float valueGood;
        public float valueBad;
        public float valueChangeImage;

        private void Start()
        {
            ClearUI();
        }

        public void SetValue(float value)
        {
            float num = 0f;
            if (value <= valueGood) num = 0f;
            else if (value >= valueBad) num = 1f;
            else num = (value - valueGood) / (valueBad - valueGood);

            txtWifi.text = value.ToString("F0") + "ms";

            if (sprWifis.Length >= 2) imgWifi.sprite = sprWifis[value >= valueChangeImage ? 1 : 0];
            if (goWifiBadNotify != null) goWifiBadNotify.SetActive(value >= valueChangeImage ? true : false);

            // change color
            Color cNow = Color.Lerp(cGood, cBad, num);
            imgWifi.color = cNow;
            txtWifi.color = cNow;
        }

        public void ClearUI()
        {
            imgWifi.color = cGood;
            txtWifi.color = cGood;
            txtWifi.text = valueGood + "ms";

            if (sprWifis.Length >= 2) imgWifi.sprite = sprWifis[0];
            if (goWifiBadNotify != null) goWifiBadNotify.SetActive(false);
        }
    }
}