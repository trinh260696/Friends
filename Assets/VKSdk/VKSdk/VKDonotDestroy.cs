using UnityEngine;

namespace VKSdk
{
    public class VKDonotDestroy : MonoBehaviour
    {
        #region Sinleton
        private static VKDonotDestroy instance;

        public static VKDonotDestroy Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<VKDonotDestroy>();
                }
                return instance;
            }
        }
        #endregion

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                instance = this;
            }

            DontDestroyOnLoad(this.transform);
        }

        public void DestroyAll()
        {
            Destroy(this.gameObject);
        }
    }
}