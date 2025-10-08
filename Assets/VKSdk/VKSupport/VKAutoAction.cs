using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VKSdk.UI;
#if VK_XLUA
using XLua;
#endif

namespace VKSdk.Support
{
    public class VKAutoAction : MonoBehaviour
    {
        public float countdown;
        public bool autoHide;

        public VKCountDownLite vkCountDown;
        public Button btAction;

#if VK_XLUA
        [CSharpCallLua]
#endif
        [Serializable]
        public class ActionCallbackDelegate : UnityEvent { } // code, status, data, baseData
#if VK_XLUA
        [CSharpCallLua]
#endif
        [SerializeField]
        public ActionCallbackDelegate actionCallback = new ActionCallbackDelegate();

        public void OnEnable()
        {
            if(countdown > 0)
            {
                if(vkCountDown != null)
                {
                    vkCountDown.SetSeconds(countdown);
                    vkCountDown.StartCountDown(null);
                }

                LeanTween.delayedCall(gameObject, countdown, () => {
                    if(btAction != null) btAction.onClick.Invoke();
                    if(actionCallback != null) actionCallback.Invoke();

                    if(autoHide) gameObject.SetActive(false);
                });
            }
        }

        public void OnDisable()
        {
            LeanTween.cancel(gameObject);
        }
    }
}
