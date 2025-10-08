using System;
using UnityEngine;

namespace VKSdk
{
    [Serializable]
    public class VKTMPFaceSetting
    {
        public Color color;
        public float softness;
        public float dilate;
    }
    
    [Serializable]
    public class VKTMPOutlineSetting
    {
        public bool outlineActive;
        public Color color;
        public float thickness;
    }

    [Serializable]
    public class VKTMPUnderlaySetting
    {
        public bool underlayActive;
        public Color color;
        public float offsetX;
        public float offsetY;
        public float dilate;
        public float softness;
    }
}