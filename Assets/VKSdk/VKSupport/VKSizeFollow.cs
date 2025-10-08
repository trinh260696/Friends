using System.Collections;
using UnityEngine;

namespace VKSdk.Support
{
    public class VKSizeFollow : MonoBehaviour
    {
        public RectTransform rectTarget;

        public bool followWidth;
        public bool followHeight;
        
        [Space(10)]
        public float offsetWidth;
        public float offsetHeight;

        [Space(10)]
        public bool autoDetect;
        public float secondDelay;

        // public void Awake()
        // {
        //     if(autoDetect)
        //     {
        //         Change();
        //         StartCoroutine(IEAutoDetect());
        //     }
        // }

        public void Start()
        {
            if(autoDetect)
            {
                Change();
                StartCoroutine(IEAutoDetect());
            }
        }

        public IEnumerator IEAutoDetect()
        {       
            if(secondDelay > 0)
            {
                yield return new WaitForSeconds(secondDelay);
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
            Change();
        }

        public void Change()
        {
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2((followWidth ? rectTarget.sizeDelta.x : rect.sizeDelta.x) + offsetWidth, (followHeight ? rectTarget.sizeDelta.y : rect.sizeDelta.y) + offsetHeight);
        }
    }
}