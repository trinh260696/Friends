using UnityEngine;

namespace VKSdk.Support
{
    public class VKAlwayChildOrder : MonoBehaviour
    {
        public int order;

        // Update is called once per frame
        void LateUpdate()
        {
            if (transform.GetSiblingIndex() != order) transform.SetSiblingIndex(order);
        }
    }
}
