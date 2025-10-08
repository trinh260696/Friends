using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace VKSdk.Support
{
    public class VKAutoHideObject : MonoBehaviour
    {
        public Text txtTarget;
        public string strCompare;

        void OnEnable()
        {
            StartCoroutine(WaitToHide());
        }

        IEnumerator WaitToHide()
        {
            yield return new WaitForEndOfFrame();
            if(txtTarget.text.Equals(strCompare))
            {
                gameObject.SetActive(false);
            }
        }
    }
}