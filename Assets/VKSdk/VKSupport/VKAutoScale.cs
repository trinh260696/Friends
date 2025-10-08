using UnityEngine;

namespace VKSdk.Support
{
    public class VKAutoScale : MonoBehaviour
    {
        public float time;
        public LeanTweenType loopType;
        public LeanTweenType easeType;

        public Vector3 vTarget;

        private Vector3 vDefault;

        void Awake()
        {
            vDefault = transform.localScale;
        }

        void OnEnable()
        {
            transform.localScale = vDefault;

            LeanTween.scale(gameObject, vTarget, time).setLoopType(loopType).setEase(easeType);
        }

        void OnDisable()
        {
            LeanTween.cancel(gameObject);
        }
    }
}