using UnityEngine;
using UnityEngine.UI;

namespace VKSdk.UI
{
    public class VKProgressBar : MonoBehaviour
    {
        public Image imgProgress;
#if VK_TMP_ACTIVE
        public TMPro.TMP_Text txtProgress;
#else
        public Text txtProgress;
#endif

        public float currentPercent;

        public void SetProgress(float percent)
        {
            currentPercent = percent;
            if (imgProgress != null)
            {
                imgProgress.fillAmount = (percent / 100f);
            }
            if (txtProgress != null)
            {
                txtProgress.text = percent.ToString("F0") + "%";
            }
        }

        public void RunFakeProgress(float second)
        {
            StopFakeProgress();

            if (currentPercent >= 100)
            {
                currentPercent = 99f;
            }

            LeanTween.value(gameObject, (percent) =>
            {
                SetProgress(percent);
            }, currentPercent, 100, second * ((100f - currentPercent) / 100f));
        }

        public void StopFakeProgress()
        {
            LeanTween.cancel(gameObject);
        }
    }
}