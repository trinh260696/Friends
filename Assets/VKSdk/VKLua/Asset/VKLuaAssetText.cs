using System.Linq;
using UnityEngine.UI;
#if VK_XLUA
using XLua;
#endif

namespace VKSdk.Lua
{
    public class VKLuaAssetText : VKLuaAsset
    {
#if VK_XLUA
        public Text[] assets;

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
