using System.Collections;
using UnityEngine;

namespace VKSdk.Support
{
    public class VKFollowOffset : MonoBehaviour
    {
        public RectTransform rectFollow;
        public RectTransform rectTarget;

        public int secondDetect;
        public bool autoDetect;

        private Vector2 offsetMax;
        private Vector2 offsetMin;

        private float defaultSizeX;
        private float defaultSizeY;

        public void Awake()
        {
            defaultSizeX = rectFollow.sizeDelta.x - rectTarget.offsetMax.x - rectTarget.offsetMin.x;
            defaultSizeY = rectFollow.sizeDelta.y - rectTarget.offsetMax.y - rectTarget.offsetMin.y;

            Change();

            if(autoDetect)
            {
                StartCoroutine(IEAutoDetect());
            }
        }

        public IEnumerator IEAutoDetect()
        {
            while (true)
            {
                if(offsetMax != rectTarget.offsetMax || offsetMin != rectTarget.offsetMin)
                {
                    Change();
                }
                yield return new WaitForSeconds(secondDetect);
            }
        }

        public void Change()
        {
            Debug.Log("Change");
            Debug.Log("defaultSizeX: " + defaultSizeX);
            Debug.Log("defaultSizeY: " + defaultSizeY);

            offsetMax = rectTarget.offsetMax;
            offsetMin = rectTarget.offsetMin;

            float newSizeX = defaultSizeX + offsetMax.x + offsetMin.x;
            float newSizeY = defaultSizeY + offsetMax.y + offsetMin.y;
            Debug.Log("newSizeX: " + newSizeX);
            Debug.Log("newSizeY: " + newSizeY);

            rectFollow.sizeDelta = new Vector2(newSizeX, newSizeY);
        }
    }
}