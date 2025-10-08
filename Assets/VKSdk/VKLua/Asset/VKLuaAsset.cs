using UnityEngine;
#if VK_XLUA
using XLua;
#endif

namespace VKSdk.Lua
{
    public class VKLuaAsset : MonoBehaviour
    {
#if VK_XLUA
        public string methodAddList;

        public virtual void LoadAsset(LuaTable scriptEnv)
        {
        }

        public virtual void LoadListAsset(VKLuaBehaviour luaBehaviour)
        {
        }
#endif

        public virtual object FindAsset(string name)
        {
            return null;
        }
    }
}
