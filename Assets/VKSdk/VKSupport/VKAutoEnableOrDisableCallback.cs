using System;
using UnityEngine;
using UnityEngine.Events;

namespace VKSdk.Support
{
    public class VKAutoEnableOrDisableCallback : MonoBehaviour
    {
        [Serializable]
        public class EnableDelegate : UnityEvent { } // code, status, data, baseData
        public EnableDelegate OnEnableCallback = new EnableDelegate();

        [Serializable]
        public class DisableDelegate : UnityEvent { } // code, status, data, baseData
        public DisableDelegate OnDisableCallback = new DisableDelegate();

        void OnEnable()
        {
            if(OnEnableCallback != null)
            {
                OnEnableCallback.Invoke();
            }
        }

        void OnDisable()
        {
            if(OnDisableCallback != null)
            {
                OnDisableCallback.Invoke();
            }
        }
    }
}