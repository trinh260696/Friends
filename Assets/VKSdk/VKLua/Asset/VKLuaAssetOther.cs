using System.Linq;
#if VK_XLUA
using XLua;
#endif

namespace VKSdk.Lua
{
    public class VKLuaAssetOther : VKLuaAsset
    {
#if VK_XLUA
        public InjectionOther[] assets;

        public override void LoadAsset(LuaTable scriptEnv)
        {
             for(int i = 0; i < assets.Length; i++)
             {
                scriptEnv.Set(assets[i].name, assets[i].value);
             }
        }

        public override void LoadListAsset(VKLuaBehaviour luaBehaviour)
        {
            luaBehaviour.InvokeLua(methodAddList, new object[] {assets});
        }

        public override object FindAsset(string name)
        {
            var item =  assets.FirstOrDefault(a => a.name.Equals(name));

            if(item != null)
            {
                return item.value;
            }

            return null;
        }
#endif
    }
}
