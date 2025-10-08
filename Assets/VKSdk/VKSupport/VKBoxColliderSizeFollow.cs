using System.Collections;
using UnityEngine;

namespace VKSdk.Support
{
    public class VKBoxColliderSizeFollow : MonoBehaviour
    {
        public RectTransform rectTarget;

        public BoxCollider boxCollider;
        public BoxCollider2D boxCollider2d;

        [Space(10)]
        public bool autoDetect;
        public float secondDelay;

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
            if(boxCollider != null) boxCollider.size = new Vector3(rectTarget.sizeDelta.x, rectTarget.sizeDelta.y, 1);
            if(boxCollider2d != null) boxCollider2d.size = new Vector3(rectTarget.sizeDelta.x, rectTarget.sizeDelta.y, 1);
        }
    }
}