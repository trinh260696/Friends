using System;
using UnityEngine;
using UnityEngine.Events;

namespace VKSdk
{
    public class VKObjectPoolItem : MonoBehaviour
    {
        public bool isUsing = false;

        // action to lua
        [Serializable]
        public class OnGiveBackCallBack : UnityEvent { } 
        [SerializeField]
        public OnGiveBackCallBack onGiveBack = new OnGiveBackCallBack();

        public void GiveBack()
        {
            isUsing = false;
            if(onGiveBack != null) onGiveBack.Invoke();
        }
    }
}