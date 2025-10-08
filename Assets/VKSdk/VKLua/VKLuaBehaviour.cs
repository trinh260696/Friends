#if VK_XLUA
using XLua;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using VKSdk;

namespace VKSdk.Lua
{
#if VK_XLUA
    public class VKLuaBehaviour : MonoBehaviour
    {
        #region Properties
        [Header("Lua Script")]
        [Space(10)]
        public string luaScriptName; // if have n't luaScript this name get from assetbundle
        public TextAsset luaScript;

        [Space(10)]
        public bool hideWhenAwake;

        [Header("Lua Sprite")]
        [Space(10)]
        public InjectionSprite[] sprites; 

        [Header("Lua Asset")]
        [Space(10)]
        public AudioClip[] audios;
        [Space(5)]
        public float[] numbers;
        [Space(5)]
        public string[] strings;
        [Space(5)]
        public Color[] colors;
        [Space(5)]
        public Vector3[] vectors;

        [HideInInspector]
        public VKLuaAsset[] assets;

        [HideInInspector]
        public Dictionary<string, object> datas;
        #endregion

        #region Private
        private Action luaStart;
        private Action luaUpdate;
        private Action luaOnDestroy;
        private Action luaOnEnable;
        private Action luaOnDisable;
        private Action<bool> luaOnApplicationPause;

        private LuaTable scriptEnv;
        private Dictionary<string, LuaFunction> dictLuaFunc;
        #endregion

        #region Listener
        public void ButtonXLuaClickListener(int index)
        {
            InvokeLua("OnButtonClick", index);
        }

        public void ButtonXLuaClickListener(string method)
        {
            InvokeLua(method);
        }

        public void ButtonXLuaClickWithValueListener(string methodAndValue)
        {
            string[] strs = methodAndValue.Split('|');
            if(strs.Length > 2)
            {
                string[] strDatas = new string[strs.Length - 1];
                for(int i = 1; i < strs.Length; i++)
                {
                    strDatas[i-1] = strs[i];
                }
                InvokeLua(strs[0], strDatas);
            }
            else
            {
                InvokeLua(strs[0], strs[1]);
            }
        }
        #endregion

        #region Unity Method
        void Awake()
        {
            datas = null;
            dictLuaFunc = new Dictionary<string, LuaFunction>();

            assets = GetComponents<VKSdk.Lua.VKLuaAsset>();

            // XODebug.LogWarning("LUA AWAKE");
            LuaEnv luaEnv = VKLuaEnvironment.Instance.LuaEnv;
            scriptEnv = luaEnv.NewTable();

            LuaTable meta = luaEnv.NewTable();
            meta.Set("__index", luaEnv.Global);
            scriptEnv.SetMetaTable(meta);
            meta.Dispose();

            // declare object to luascrip
            scriptEnv.Set("self", this);

            List<VKLuaAsset> assetListTemps = new List<VKLuaAsset>();
            if(assets != null)
            {
                foreach(var ass in assets)
                {
                    if(string.IsNullOrEmpty(ass.methodAddList))
                    {
                        ass.LoadAsset(scriptEnv);
                    }
                    else
                    {
                        assetListTemps.Add(ass);
                    }
                }
            }

            // lay script tu file - neu ko co file thi se lay trong assetbundle (lam sau)
            string luaText = "";
            string luaName = "";
            if(luaScript != null)
            {
                luaText = luaScript.text;
                luaScriptName = luaScript.name;
                luaName = luaScriptName.Replace(".lua", "");
            }
            else
            {
                string[] temps = luaScriptName.Split('.');
                if(temps.Length > 2)
                {
                    var luaScriptTemp = VKLuaEnvironment.Instance.GetLuaFileByName(temps[0], temps[1] + ".lua");
                    luaText = luaScriptTemp.text;
                    luaName = temps[1];
                }
                else
                {
                    var luaScriptTemp = VKLuaEnvironment.Instance.GetLuaShareFileByName(luaScriptName);
                    luaText = luaScriptTemp.text;
                    luaName = luaScriptName.Replace(".lua", "");
                }
            }
            luaEnv.DoString(luaText, luaName, scriptEnv);

            Action luaAwake = scriptEnv.Get<Action>("Awake");
            scriptEnv.Get("Start", out luaStart);
            scriptEnv.Get("Update", out luaUpdate);
            scriptEnv.Get("OnDestroy", out luaOnDestroy);
            scriptEnv.Get("OnEnable", out luaOnEnable);
            scriptEnv.Get("OnDisable", out luaOnDisable);
            scriptEnv.Get("OnApplicationPause", out luaOnApplicationPause);

            // load list asset
            foreach(var ass in assetListTemps)
            {
                ass.LoadListAsset(this);
            }
            assetListTemps = null;

            if (luaAwake != null)
            {
                luaAwake();
            }

            // Debug.LogWarning("LUA START END");
        }

        // Use this for initialization
        void Start()
        {
            if (luaStart != null)
            {
                luaStart();
            }

            if(hideWhenAwake)
            {
                StartCoroutine(WaitForAutoHide());
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (luaUpdate != null)
            {
                luaUpdate();
            }
        }

        void OnEnable()
        {
            if (luaOnEnable != null)
            {
                luaOnEnable();
            }
        }

        void OnDisable()
        {
            if (luaOnDisable != null)
            {
                luaOnDisable();
            }
        }

        void OnApplicationPause(bool paused)
        {
            if(luaOnApplicationPause != null)
            {
                luaOnApplicationPause(paused);
            }
        }

        void OnDestroy()
        {
            if (luaOnDestroy != null)
            {
                luaOnDestroy();
            }
            luaOnDestroy = null;
            luaUpdate = null;
            luaStart = null;
            luaOnEnable = null;
            luaOnDisable = null;
            dictLuaFunc = null;

            scriptEnv.Dispose();
            assets = null;
            sprites = null;
            audios = null;
            numbers = null;
            strings = null;
            colors = null;
            vectors = null;
        }

        // other when awake
        IEnumerator WaitForAutoHide()
        {
            yield return new WaitForEndOfFrame();
            gameObject.SetActive(false);
        }
        #endregion

        #region Method Other 
        public void Load(int index) // dung trong danh sach keo xong load data item
        {
            InvokeLua("Load", index);
        }

        public void Load(object obj) // dung trong danh sach keo xong load data item
        {
            InvokeLua("LoadObject", obj);
        }
        #endregion

        #region Support Lua
        // data
        public void InitData()
        {
            datas = new Dictionary<string, object>();
        }

        public void AddData(string key, object ob)
        {
            if(datas.ContainsKey(key))
                datas[key] = ob;
            else
                datas.Add(key, ob);
        }

        public object GetData(string key)
        {
            if(datas.ContainsKey(key))
                return datas[key];
            else
                return null;
        }

        // other
        public void SetObjectLua<TKey, TValue>(TKey key, TValue value)
        {
            scriptEnv.Set(key, value);
        }

        public void GetActionLua(string actionName, Action actionLua)
        {
            scriptEnv.Get(actionName, out actionLua);
        }

        public T GetVariableLuaT<T>(string variableName)
        {
            return scriptEnv.Get<T>(variableName);
        }


        // can call from lua
        public void SetVariableLua(string key, object value)//, Type type)
        {
            scriptEnv.Set(key, value);
        }

        public object GetVariableLuaWithType(string variableName, Type type)
        {
            object ret;
            scriptEnv.Get(variableName, out ret);
            return Convert.ChangeType(ret, type);
        }

        public object GetVariableLua(string variableName, Type type)
        {
            object ret;
            scriptEnv.Get(variableName, out ret);
            return ret;
        }

        public void CallInvokeLua(string method)
        {
            InvokeLua(method);
        }

        // invoke lua function
        public object[] InvokeLua(string funcName, params object[] args)
        {
            if(dictLuaFunc != null && dictLuaFunc.ContainsKey(funcName))
            {
                return dictLuaFunc[funcName].Call(args);
            }
            else
            {
                LuaFunction luaFunc = null;
                scriptEnv.Get(funcName, out luaFunc);
                if (luaFunc != null)
                {
                    dictLuaFunc.Add(funcName, luaFunc);
                    return luaFunc.Call(args);
                }
#if UNITY_EDITOR
                else
                {
                    VKDebug.LogWarning(String.Format("function {0} from {1} not found", funcName, luaScriptName));
                }
#endif
            }

            return null;
        }
        #endregion

        #region Support Get Asset
        public T FindAsset<T>(string name)
        {
            foreach(var ass in assets)
            {
                var objTemp = ass.FindAsset(name);
                if(objTemp != null)
                {
                    if(objTemp.GetType() == typeof(GameObject))
                    {
                        return ((GameObject)objTemp).GetComponent<T>();
                    }
                    else
                    {
                        return (T)objTemp;
                    }
                }
            }
            return default(T);
        }
        #endregion

        #region Support Sprite
        public Sprite GetSprite(string key, int index)
        {
            var sps = sprites.FirstOrDefault(a => a.name.Equals(key));
            if(sps != null && sps.sprites.Length > index)
            {
                return sps.sprites[index];
            }
            return null;
        }

        public Sprite[] GetListSprite(string key)
        {
            var sps = sprites.FirstOrDefault(a => a.name.Equals(key));
            if(sps != null)
            {
                return sps.sprites;
            }
            return null;
        }
        #endregion

        #region Support Audio
        #endregion
    }

    [System.Serializable]
    public class InjectionOther
    {
        public string name;
        public GameObject value;
    }

    [System.Serializable]
    public class InjectionSprite
    {
        public string name;
        public Sprite[] sprites;
    }
#endif
}