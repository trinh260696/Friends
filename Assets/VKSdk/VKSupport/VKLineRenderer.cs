using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if VK_LINERENDER
using UnityEngine.UI.Extensions;
#endif

namespace VKSdk.Support
{
    public class VKLineRenderer : MonoBehaviour
    {
#if VK_LINERENDER
        public UILineRenderer line;

        public void CreateLine(List<Vector2> pos)
        {
            line.Points = pos.ToArray();
        }
#else
        public void CreateLine(List<Vector2> pos)
        {
        }
#endif
    }
}
