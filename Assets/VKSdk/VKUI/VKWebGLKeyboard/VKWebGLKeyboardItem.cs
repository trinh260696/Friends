using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VKSdk.UI
{
    public class VKWebGLKeyboardItem : MonoBehaviour, ISelectHandler
    {
#if UNITY_WEBGL
    public void OnSelect(BaseEventData data)
    {
        if (VKWebGLKeyboardController.Instance != null)
        {
            VKWebGLKeyboardController.Instance.OnInputFieldSelect(gameObject.GetComponent<InputField>());
        }
    }

    public void OnDeselect(BaseEventData data)
    {
        if (VKWebGLKeyboardController.Instance != null)
        {
            VKWebGLKeyboardController.Instance.OnInputFieldUnSelect(gameObject.GetComponent<InputField>());
        }
    }

    public void OnDisable()
    {
        if (VKWebGLKeyboardController.Instance != null)
        {
            VKWebGLKeyboardController.Instance.OnInputFieldUnSelect(gameObject.GetComponent<InputField>());
        }
    }
#else
        public void OnSelect(BaseEventData data)
        {
        }
#endif
    }
}