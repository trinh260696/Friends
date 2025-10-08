using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace VKSdk.Notify
{
    public class VKNotifyMessage : MonoBehaviour
    {
        #region Properties
        public Text txtInfo;
        public RectTransform rectParent;
        public RectTransform rectText;
        public RectTransform rectScreen;
        public float speed;
        #endregion

        public void OnDisable()
        {
            //coroutine = null;
        }

        public void Init(string str)
        {
            this.gameObject.SetActive(true);
            txtInfo.text = str;
            rectText.anchoredPosition = new Vector2(0, 0);

            StopAllCoroutines();
            StartCoroutine(WaitToStartRun());
        }

        IEnumerator WaitToStartRun()
        {
            LeanTween.cancel(rectText.gameObject);
            yield return new WaitForEndOfFrame();

            float xPos = -(rectText.sizeDelta.x + rectScreen.sizeDelta.x - 190);
            rectText.transform.localPosition = Vector3.zero;

            float timeRun = (-xPos / speed);

            LeanTween.cancel(rectText.gameObject);
            LeanTween.moveLocalX(rectText.gameObject, xPos, timeRun).setOnComplete(() =>
            {
                StartCoroutine(WaitToStartRun());
            });
        }
    }
}