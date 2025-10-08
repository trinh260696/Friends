using UnityEngine;

namespace VKSdk.UI
{
    public class VKLayerChildOrder : MonoBehaviour
    {
        public int addOrder;

        public void ResetOrder(int parentOrder)
        {
            Canvas childCanvas = GetComponent<Canvas>();
            if (childCanvas != null)
            {
                childCanvas.sortingOrder = parentOrder + addOrder;
            }
            else
            {
                ParticleSystemRenderer particle = GetComponent<ParticleSystemRenderer>();
                if (particle != null)
                    particle.sortingOrder = parentOrder + addOrder;
            }
        }
    }
}