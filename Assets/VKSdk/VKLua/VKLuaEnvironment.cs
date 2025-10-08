using UnityEngine;
#if VK_XLUA
using XLua;
#endif
using System.Collections.Generic;
using System.Linq;

namespace VKSdk.Lua
{
#if VK_XLUA
    public class VKLuaEnvironment : MonoBehaviour
    {
        private static VKLuaEnvironment _instance = null;
        public static VKLuaEnvironment Instance { private set { _instance = value; } get { return _instance; } }

        internal static LuaEnv luaEnv = new LuaEnv();
        public LuaEnv LuaEnv { get { return luaEnv; } }

        internal static float lastGCTime = 0;
        internal const float GCInterval = 1;//1 second 

        public List<VKLuaFile> luaFiles;
        public Dictionary<string, VKLuaBehaviour> dictVKLuas;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;

            dictVKLuas = new Dictionary<string, VKLuaBehaviour>();
        }

        // Use this for initialization
        void Start()
        {
            luaEnv.AddLoader((ref string filename) =>
            {
                return GetLuaCode(filename);
            });
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.time - lastGCTime > GCInterval)
            {
                luaEnv.Tick();
                lastGCTime = Time.time;
            }
        }

        public byte[] GetLuaCode(string filename)
        {
            string[] strs = filename.Split('.');
            if(strs != null && strs.Length == 2)
            {
                string garoupName = strs[0];
                string luaScriptName = strs[1] + ".lua";

                TextAsset ass = GetLuaFileByName(garoupName, luaScriptName);
                if(ass != null)
                {
                    return ass.bytes;
                }
            }
            
            return null;
        }

        public TextAsset GetLuaFileByName(string groupName, string fileName) 
        {
            VKLuaFile group = luaFiles.FirstOrDefault(a => a.name.Equals(groupName));
            if(group != null)
            {
                return group.values.FirstOrDefault(a => a.name.Equals(fileName));
            }
            return null;
        }

        public TextAsset GetLuaShareFileByName(string fileName)
        {
            return GetLuaFileByName("share", fileName);
        }
    
        public void AddVKLuaBehaviour(string name, VKLuaBehaviour behavior)
        {
            if(dictVKLuas.ContainsKey(name))
            {
                dictVKLuas[name] = behavior;
            }
            else
            {
                dictVKLuas.Add(name, behavior);
            }
        }

        public void RemoveVKLuaBehaviour(string name, VKLuaBehaviour behavior)
        {
            if(dictVKLuas.ContainsKey(name))
            {
                dictVKLuas.Remove(name);
            }
        }

        public VKLuaBehaviour VKLua(string name)
        {
            if(dictVKLuas.ContainsKey(name))
            {
                return dictVKLuas[name];
            }
            return null;
        }
    }

    [System.Serializable]
    public class VKLuaFile
    {
        public string name;
        public List<TextAsset> values;
    }
#endif
}