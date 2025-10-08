using UnityEngine;

namespace VKSdk.Support
{
    public class VKFollowOrder : MonoBehaviour
    {
        public Canvas followCanvas;
        public int orderAdd;

        private void OnEnable()
        {
            Canvas canvas = GetComponent<Canvas>();
            if (canvas)
            {
                canvas.sortingOrder = followCanvas.sortingOrder + orderAdd;
            }
        }
    }
}