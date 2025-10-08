using UnityEngine;

namespace VKSdk.Support
{
    public class VKAutoMove : MonoBehaviour
    {
        public float time;
        public LeanTweenType loopType;
        public LeanTweenType easeType;

        public Vector3 vTarget;

        private Vector3 vDefault;

        void Awake()
        {
            vDefault = transform.localPosition;
        }

        void OnEnable()
        {
            transform.localPosition = vDefault;

            LeanTween.moveLocal(gameObject, vTarget, time).setLoopType(loopType).setEase(easeType);
        }

        void OnDisable()
        {
            LeanTween.cancel(gameObject);
        }
    }
}