using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;
using System;
using System.Collections;

namespace VKSdk.UI
{
    public class VKWebGLKeyboardController : MonoBehaviour
    {
#if UNITY_WEBGL
        //[DllImport("__Internal")]
        //private static extern void FKeyboardCheckPlatform();

        //[DllImport("__Internal")]
        //private static extern void FOpenInputKeyboard(string str);
        //[DllImport("__Internal")]
        //private static extern void FCloseInputKeyboard();

        ////Just adds these functions references to avoid stripping
        //[DllImport("__Internal")]
        //private static extern void FFixInputOnBlur();
        //[DllImport("__Internal")]
        //private static extern void FFixInputUpdate();

        public InputField inputFieldCurrent;
        public bool webGLMobile;

            #region Sinleton
        private static VKWebGLKeyboardController instance;

        public static VKWebGLKeyboardController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<VKWebGLKeyboardController>();
                }
                return instance;
            }
        }

        public void Start()
        {
    #if UNITY_EDITOR
            OnTargetPlatform("true|iOS|Safari");
    #else
            StartCoroutine(WaitToCheckPlatform());
    #endif
        }
        #endregion

        #region Method
        IEnumerator WaitToCheckPlatform()
        {
            yield return new WaitForEndOfFrame();
            Application.ExternalCall("KeyboardCheckPlatform");

#if GAPO
            Application.ExternalCall("GapoGetTokenAndKey");
#endif

        }

        public void OnInputFieldSelect(InputField inputField)
        {
            if(webGLMobile)
            {
                this.inputFieldCurrent = inputField;
                try
                {
                    Application.ExternalCall("OpenInputKeyboard", inputField.text);
                    UnityEngine.WebGLInput.captureAllKeyboardInput = false;
                }
                catch (Exception error) { }
            }
        }

        public void OnInputFieldUnSelect(InputField inputField)
        {
            if (webGLMobile)
            {
                if (inputFieldCurrent != null && inputFieldCurrent.Equals(inputField))
                {
                    this.inputFieldCurrent = null;
                    try
                    {
                        Application.ExternalCall("CloseInputKeyboard");
                        UnityEngine.WebGLInput.captureAllKeyboardInput = true;
                    }
                    catch { }
                }
            }
        }
        #endregion

        #region Callback
        public void OnTargetPlatform(string data)
        {
            string[] datas = data.Split('|');
            if(datas != null && datas.Length == 3)
            {
                webGLMobile = datas[0].Equals("true");
                VKGameConfig.Instance.webglPlatform = datas[1];
                VKGameConfig.Instance.webglBrowser = datas[2];

                // // add to home screen
                // if (webGLMobile && SceneStart.Instance != null)
                // {
                //     if (datas[1].Equals("iOS") && !datas[2].Equals("Safari"))
                //     {
                //         return;
                //     }
                //     // SceneStart.Instance.ShowAddHomePopup(datas[2].Equals("Safari") ? 1 : 0);
                // }
            }
        }

#if GAPO
        public void OnGapoCallback(string data)
        {
            string[] datas = data.Split("|1221|");
            if(datas != null && datas.Length == 2)
            {
                // signin gapo
            }
        }
#endif

#if FACEBOOK_INSTANT
        public void GetFacebookSignature()
        {
#if UNITY_EDITOR
            LSignin.FB_INSTANT_ID = "4703318163034232";
            LSignin.FB_INSTANT_SIGNATURE = "2sALbhslxz-1prPKw2QNdrqjfpDQaC1u14ucHSRNZJM.eyJhbGdvcml0aG0iOiJITUFDLVNIQTI1NiIsImlzc3VlZF9hdCI6MTY0NjIwMzAxOSwicGxheWVyX2lkIjoiNDcwMzMxODE2MzAzNDIzMiIsInJlcXVlc3RfcGF5bG9hZCI6bnVsbH0";
            LSignin.FB_INSTANT_NAME = "UNITY EDITOR";
#else
            Application.ExternalCall("GetFacebookSignature");
#endif
        }

        public void OnFacebookSignature(string data)
        {
            Debug.Log(data);
            if(!string.IsNullOrEmpty(data))
            {
                string[] strs = data.Split(new string[] { "|1221|" }, StringSplitOptions.None);
                try
                {
                    if(string.IsNullOrEmpty(strs[1]))
                    {
                        LSignin.FB_INSTANT_ID = "NONE";
                        LSignin.FB_INSTANT_SIGNATURE = "NONE";
                        LSignin.FB_INSTANT_NAME = "NONE";
                    }
                    else
                    {
                        LSignin.FB_INSTANT_ID = strs[0];
                        LSignin.FB_INSTANT_SIGNATURE = strs[1];
                        LSignin.FB_INSTANT_NAME = strs[2];
                    }
                }
                catch
                {
                    LSignin.FB_INSTANT_ID = "NONE";
                    LSignin.FB_INSTANT_SIGNATURE = "NONE";
                    LSignin.FB_INSTANT_NAME = "NONE";
                }
            }
            else
            {
                LSignin.FB_INSTANT_ID = "NONE";
                LSignin.FB_INSTANT_SIGNATURE = "NONE";
                LSignin.FB_INSTANT_NAME = "NONE";
            }
        }
#endif

        public void OnLoseFocus()
        {
            if (webGLMobile)
            {
                if(inputFieldCurrent != null)
                {
                    inputFieldCurrent.DeactivateInputField();
                    inputFieldCurrent = null;
                }
                UnityEngine.WebGLInput.captureAllKeyboardInput = true;
            }
        }

        public void OnReceiveInputChange(string value)
        {
            if (webGLMobile && inputFieldCurrent != null)
            {
                inputFieldCurrent.text = value;
                StartCoroutine(WaitToMoveInputFieldEnd());
            }
        }

        IEnumerator WaitToMoveInputFieldEnd()
        {
            yield return new WaitForEndOfFrame();
            if (inputFieldCurrent != null)
            {
                inputFieldCurrent.MoveTextEnd(false);
            }
        }
        #endregion
#endif
    }
}