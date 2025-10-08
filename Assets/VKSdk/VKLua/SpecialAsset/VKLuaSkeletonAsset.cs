#if VK_SPINE
using Spine.Unity;
#endif
using UnityEngine;

namespace VKSdk.Lua
{
    public class VKLuaSkeletonAsset : MonoBehaviour
    {
#if VK_SPINE
        public SkeletonDataAsset[] skeletonAssets;
#endif
    }
}