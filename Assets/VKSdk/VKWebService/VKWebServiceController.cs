using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
#if VK_XLUA
using XLua;
using VKSdk.Lua;
#endif
using VKSdk;
using UnityEngine.Events;
using LitJson;
using VKSdk.UI;

namespace VKSdk.WebService
{
    public enum VKWebServiceStatus : int
    {
        OK = 1,
        ERROR = 2,

        AUTHORIZATION_EXCEPTION = 100,
        SERVER_EXCEPTION = 101,
        INTERNET_ERROR = 102,
    }

    [System.Serializable]
    public class VKWebServiceApi
    {
        public string domain;
        public List<VKWebServiceApiItem> apis;
    }

    [System.Serializable]
    public class VKWebServiceApiItem
    {
        public string key;
        public string value;
    }

    [System.Serializable]
    public class VKBaseResponseData
    {
        public int error;
        public string msg;
        public string data;
    }

    // Dùng cái này phải có 1 file SendRequet tự quản lý
    public class VKWebServiceController : MonoBehaviour
    {
        #region Properties
        // static
        public static string domain = "https://test.com/api";
        public static int TIME_OUT = 16;

#if VK_XLUA
        public VKLuaBehaviour xlua;
#endif
        public TextAsset apiData;

        [HideInInspector]
        public string token = "";
        [HideInInspector]
        public string platform = "unknow";
        [HideInInspector]
        public Dictionary<string, string> apiLists;

        // private
        private DateTime lastTimeInternetError = DateTime.MinValue;

        // action to lua
#if VK_XLUA
        [CSharpCallLua]
#endif
        [Serializable]
        public class WebServiceResponseDelegate : UnityEvent<string, int, string, string> { } // code, status, data, baseData
#if VK_XLUA
        [CSharpCallLua]
#endif
        [SerializeField]
        public WebServiceResponseDelegate OnWebServiceResponse = new WebServiceResponseDelegate();

        public Action onLostInternetSoFar;
        public Action onLogout;

        public List<string> ignoreCodes;
        #endregion

        #region Sinleton
        private static VKWebServiceController instance;

        public static VKWebServiceController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<VKWebServiceController>();
                }
                return instance;
            }
        }

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                instance = this;
            }
            DontDestroyOnLoad(this.gameObject);

            apiLists = new Dictionary<string, string>();

            if (apiData != null && !string.IsNullOrEmpty(apiData.text))
            {
                LoadApiList(apiData.text);
            }
        }
        #endregion

        #region Method lua
        public void InitLua()
        {
            gameObject.SetActive(false);
#if VK_XLUA
            xlua = gameObject.AddComponent<VKLuaBehaviour>();
            xlua.luaScriptName = "core.WebServiceController.lua";
#endif
            gameObject.SetActive(true);
        }
        #endregion

        #region Method
        public void LoadApiList(string jsonData)
        {
            var apiConfig = JsonMapper.ToObject<VKWebServiceApi>(jsonData);
            
            domain = apiConfig.domain;
            apiLists = new Dictionary<string, string>();
            foreach (VKWebServiceApiItem item in apiConfig.apis)
            {
                apiLists.Add(item.key, item.value);
            }
        }

        public void AddIgnoreCodes(List<string> igCodes)
        {
            ignoreCodes = igCodes;
        }

        public void AddListener(UnityAction<string, int, string, string> action)
        {
            OnWebServiceResponse.AddListener(action);
        }

        public void RemoveListener(UnityAction<string, int, string, string> action)
        {
            OnWebServiceResponse.RemoveListener(action);
        }

        public void SendGetRequest(string code, string reqData = "", bool isData = false)
        {
            StartCoroutine(GetText(code, reqData, isData));
        }

        public void SendPostRequest(string code, WWWForm data, bool isData = false)
        {
            StartCoroutine(PostText(code, data, isData));
        }

        public void SetLastTimeInternetError()
        {
            lastTimeInternetError = DateTime.Now;
        }

        public void ResetLastTimeInternetError()
        {
            lastTimeInternetError = DateTime.MinValue;
        }

        public void SetToken(string tk)
        {
            token = tk;
        }

        public void SetPlatform(string pf)
        {
            platform = pf;
        }
        #endregion

        #region Handle Method
        public void RaiseWebServiceResponse(string code, int status, string data, string baseData)
        {
            OnWebServiceResponse.Invoke(code, status, data, baseData);
        }

        //check mất kết nối lău quá cũng đẩy ra login - max 10 min
        public bool CheckLostInternetSoFar()
        {
            if (lastTimeInternetError == DateTime.MinValue)
            {
                lastTimeInternetError = DateTime.Now;
            }
            else
            {
                TimeSpan timeRange = DateTime.Now - lastTimeInternetError;

#if VK_XLUA
                if (xlua != null)
                {
                    return (bool)xlua.InvokeLua("CheckLostInternetSoFar", timeRange.TotalSeconds)[0];
                }
                else
#endif
                {
                    return CheckLostInternetSoFar(timeRange.TotalSeconds);
                }
            }
            return false;
        }
        #endregion

        #region IE Method
        IEnumerator GetText(string code, string reqData, bool isData)
        {
            string url = domain;

            if (apiLists.ContainsKey(code))
            {
                url = apiLists[code] + reqData;
                if(!url.StartsWith("http"))
                {
                    url = domain + url;
                }
            }

#if UNITY_EDITOR || DEVELOPER
            VKDebug.LogWarning("Code: " + code + " | url: " + url);
#endif
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.timeout = TIME_OUT;
                request.useHttpContinue = false;

                if (!string.IsNullOrEmpty(token))
                {
                    request.SetRequestHeader("Authorization", token);
                }
                // request.SetRequestHeader("Platform", platform);

                // Config request request by lua
#if VK_XLUA
                if (xlua != null)
                {
                    xlua.InvokeLua("SetRequestHeader", request);
                }
#endif

                yield return request.SendWebRequest();

                Debug.Log(request.downloadHandler.text);
                if(request.result == UnityWebRequest.Result.Success) // Success
                {
#if VK_XLUA
                    if (xlua != null)
                    {
                        xlua.InvokeLua("HandleResponse", code, request.downloadHandler.text, isData);
                    }
                    else
#endif
                    {
                        HandleResponse(code, request.downloadHandler.text, isData);
                    }
                }
                else // Error
                {
#if VK_XLUA
                    if (xlua != null)
                    {
                        xlua.InvokeLua("HandleNetworkError", code);
                    }
                    else
#endif
                    {
                        HandleNetworkError(code);
                    }
                }
            }
        }

        IEnumerator PostText(string code, WWWForm data, bool isData)
        {
            string url = domain;
            if (apiLists.ContainsKey(code))
            {
                url = apiLists[code];
                if(!url.StartsWith("http"))
                {
                    url = domain + url;
                }
            }

#if UNITY_EDITOR || DEVELOPER
            VKDebug.LogWarning("Code: " + code + " | url: " + url);
#endif
            using (UnityWebRequest request = UnityWebRequest.Post(url, data))
            {
                request.timeout = TIME_OUT;
                request.useHttpContinue = false;

                if (!string.IsNullOrEmpty(token))
                {
                    request.SetRequestHeader("Authorization", token);
                }
                // request.SetRequestHeader("Platform", platform);

                // Config request request by lua
#if VK_XLUA
                if (xlua != null)
                {
                    xlua.InvokeLua("SetRequestHeader", request);
                }
#endif

                yield return request.SendWebRequest();

                Debug.Log(request.downloadHandler.text);
                // VKDebug.Log(VKCommon.ConvertDictionaryToString(request.GetResponseHeaders()), VKCommon.HEX_ORANGE);
                if(request.result == UnityWebRequest.Result.Success) // Success
                {
#if VK_XLUA
                    if (xlua != null)
                    {
                        xlua.InvokeLua("HandleResponse", code, request.downloadHandler.text, isData);
                    }
                    else
#endif
                    {
                        HandleResponse(code, request.downloadHandler.text, isData);
                    }
                }
                else // Error
                {
#if VK_XLUA
                    if (xlua != null)
                    {
                        xlua.InvokeLua("HandleNetworkError", code);
                    }
                    else
#endif
                    {
                        HandleNetworkError(code);
                    }
                }
            }
        }
        #endregion

        #region Method default
        private bool CheckLostInternetSoFar(double totalSeconds)
        {
            if (totalSeconds > 300)
            {
                SetLastTimeInternetError();

                if (onLostInternetSoFar != null)
                {
                    onLostInternetSoFar.Invoke();
                }
                return true;
            }
            return false;
        }

        private void HandleNetworkError(string code)
        {
            VKLayerController.Instance.HideLoading();
            if (!CheckLostInternetSoFar())
            {
                if(ignoreCodes == null || !ignoreCodes.Contains(code))
                {
                    LPopup.OpenPopup("NOTIFICATION", "50_PopupContents/SYS_UNSTABLE_CONNECTION");
                }
            }
            RaiseWebServiceResponse(code, (int)VKWebServiceStatus.INTERNET_ERROR, "", "");
        }

        private void HandleResponse(string code, string response, bool isData)
        {
            VKWebServiceStatus status = CheckError(response);
            string jsonData = response + "";

#if UNITY_EDITOR || DEVELOPER
            VKDebug.LogWarning("code: " + code + " - Response: " + response);
#endif

            switch (status)
            {
                case VKWebServiceStatus.AUTHORIZATION_EXCEPTION:
                    VKLayerController.Instance.HideLoading();
                    if (onLogout != null)
                    {
                        onLogout.Invoke();
                    }
                    break;
                case VKWebServiceStatus.SERVER_EXCEPTION:
                    VKLayerController.Instance.HideLoading();
                    break;
                case VKWebServiceStatus.OK:
                    if (isData)
                    {
                        try
                        {
                            VKBaseResponseData baseData = JsonMapper.ToObject<VKBaseResponseData>(response);
                            if (baseData != null)
                            {
                                if (baseData.error != 0)
                                {
                                    VKLayerController.Instance.HideLoading();

                                    // ignore
                                    if(ignoreCodes == null || !ignoreCodes.Contains(code))
                                    {
                                        LPopup.OpenPopup("ERROR", baseData.msg);
                                    }
                                    status = VKWebServiceStatus.ERROR;
                                }
                                else
                                {
                                    jsonData = baseData.data;
                                }
                            }
                            else
                            {
                                status = VKWebServiceStatus.ERROR;
                            }
                        }
                        catch
                        {
                            status = VKWebServiceStatus.ERROR;
                        }

                    }
                    break;
            }

            RaiseWebServiceResponse(code, (int)status, jsonData, response);
        }

        private VKWebServiceStatus CheckError(string response)
        {
            if (string.IsNullOrEmpty(response))
            {
                return VKWebServiceStatus.SERVER_EXCEPTION;
            }

            if (VKCommon.CheckResponseError(response))
            {
                return VKWebServiceStatus.SERVER_EXCEPTION;
            }

            if (response.Contains("uthorization has been denied for this request")) // bỏ A
            {
                return VKWebServiceStatus.AUTHORIZATION_EXCEPTION;
            }

            if(response.Contains("{\"Message\":\"Success\""))
            {
                return VKWebServiceStatus.OK;
            }
            
            if (response.Contains("{\"Message\":\""))
            {
                return VKWebServiceStatus.SERVER_EXCEPTION;
            }
            return VKWebServiceStatus.OK;

        }
        #endregion
    }
}