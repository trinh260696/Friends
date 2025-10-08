using System.Linq;
using UnityEngine;
#if VK_XLUA
using XLua;
#endif

namespace VKSdk.Lua
{
    public class VKLuaAssetTextAsset : VKLuaAsset
    {
#if VK_XLUA
        public TextAsset[] assets;

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

        public override object FindAsset(string name)
        {
            return assets.FirstOrDefault(a => a.name.Equals(name));
        }
#endif
    }
}
