using UnityEngine;

namespace VKSdk
{
    public class VKAssetManager : MonoBehaviour
    {
        #region Sinleton
        public static VKAssetManager singleton;
        #endregion

        #region Method
        public virtual void Initialize()
        {
            singleton = this;
        }

        public virtual Font GetFontByName(string fname)
        {
            return null;
        }

        public virtual TMPro.TMP_FontAsset GetTMPFontByName(string fname)
        {
            return null;
        }

        public virtual Material GetTMPFontMaterialByName(string mname)
        {
            return null;
        }
        #endregion
    }
}