using UnityEngine;
using System.Linq;
#if VK_XLUA
using XLua;
#endif

namespace VKSdk.Lua
{
    public class VKLuaAssetGameObject : VKLuaAsset
    {
        public GameObject[] assets;

#if VK_XLUA
        public override void LoadAsset(LuaTable scriptEnv)
        {
            for(int i = 0; i < assets.Length; i++)
            {
                scriptEnv.Set(assets[i].name, assets[i]);
            }
        }

        public override void LoadListAsset(VKLuaBehaviour luaBehaviour)
        {
            luaBehaviour.InvokeLua(methodAddList, new object[] {assets});
        }
#endif

        public override object FindAsset(string name)
        {
            return assets.FirstOrDefault(a => a.name.Equals(name));
        }
    }
}
