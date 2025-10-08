using System.Collections;
using UnityEngine;

namespace VKSdk.Support
{
    public class VKAutoHide : MonoBehaviour
    {
        public float time;

        public CanvasGroup canvasGroup;
        public float timeShow;
        public float timeHide;

        public bool isNeedActive;

        void OnEnable()
        {
            if(!isNeedActive)
            {
                StartHide();
            }
        }

        void OnDisable()
        {
            LeanTween.cancel(gameObject);
        }

        public void StartHide()
        {
            if(canvasGroup != null && timeShow > 0)
            {
                LeanTween.value(gameObject, (value) => {
                    canvasGroup.alpha = value;
                }, 0f, 1f, timeShow).setOnComplete(() => {
                    canvasGroup.alpha = 1f;
                });
            }
            StartCoroutine(WaitToHide());
        }

        IEnumerator WaitToHide()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(time);

            if(canvasGroup != null)
            {
                LeanTween.value(gameObject, (value) => {
                    canvasGroup.alpha = value;
                }, 1f, 0f, timeHide).setOnComplete(() => {
                    canvasGroup.alpha = 0f;
                    gameObject.SetActive(false);
                });
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}