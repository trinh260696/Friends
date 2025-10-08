using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VKSdk.UI
{
    public class VKInputfield : MonoBehaviour
    {
#if !(UNITY_IOS || UNITY_ANDROID || UNITY_WEBGL)
        private InputField _inputField;
        private int position;

        private Color cDefault;
        private Color cAlpha;
        
        private void Start()
        {
            _inputField = GetComponent<InputField>();

            cDefault = _inputField.selectionColor;
            cAlpha = new Color(cDefault.r, cDefault.g, cDefault.b, 0);
        }

        void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == _inputField.gameObject)
            {
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    if(Input.GetKeyUp(KeyCode.Return))
                    {
                        StartCoroutine(IEBreakLineInputField());
                    }
                }
                else
                {
                    position = _inputField.caretPosition;
                }
            }
        }

        IEnumerator IEBreakLineInputField()
        {
            _inputField.selectionColor = cAlpha;
            _inputField.ActivateInputField();
    
            yield return null;

            if(position > _inputField.text.Length) 
            {
                _inputField.text += "\n";
            }
            else
            {
                _inputField.text = _inputField.text.Insert(position, "\n");
            }

            position = position + 1;
            if(position >= _inputField.text.Length)
            {
                _inputField.MoveTextEnd(false);
            }
            else
            {
                _inputField.caretPosition = position;
            }
            _inputField.selectionColor = cDefault;
        }
#endif
    }
}