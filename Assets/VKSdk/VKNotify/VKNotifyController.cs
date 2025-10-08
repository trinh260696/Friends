using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VKSdk.UI;

namespace VKSdk.Notify
{
    public class VKNotifyController : MonoBehaviour
    {
        public enum TypeNotify
        {
            Normal = 0,
            Error = 1,
            Success = 2,
            Other = 3
        }

        public Canvas canvas;
        public GameObject gNoti;
        public Transform content;
        public RectTransform rectMain;
        List<VKNotifyItem> notis;

        public GameObject goTextRun;
        public VKTextRun vkTextRun;

        #region Sinleton
        private static VKNotifyController instance;
        public static VKNotifyController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<VKNotifyController>();
                }
                return instance;
            }
        }

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
        #endregion

        public void SetCamera(Camera cam)
        {
            canvas.worldCamera = cam;
        }

        public void AddNotify(string content, TypeNotify type)
        {
            if (string.IsNullOrEmpty(content))
                return;

            Notify().Show(new NotifyItemData
            {
                type = type,
                content = content
            });
        }

        VKNotifyItem Notify()
        {
            if (notis == null)
            {
                notis = new List<VKNotifyItem>();
                return CreateNoti();
            }

            var noti = notis.FirstOrDefault(a => a.gameObject.activeSelf && a.isActive);
            if (noti == null)
                noti = notis.FirstOrDefault(a => !a.gameObject.activeSelf);
            return noti ?? CreateNoti();
        }

        VKNotifyItem CreateNoti()
        {
            GameObject obj = GameObject.Instantiate(gNoti, content);
            obj.transform.localScale = Vector3.one;
            obj.SetActive(false);
            VKNotifyItem noti = obj.GetComponent<VKNotifyItem>();
            noti.Init();
            notis.Add(noti);
            return noti;
        }

        public Vector2 GetRectSize()
        {
            if(rectMain != null) return rectMain.sizeDelta;
            return Vector2.zero;
        }

        public void ReloadCanvasScale(float screenRatio, float screenScale)
        {
            UnityEngine.UI.CanvasScaler canvasScaler = GetComponent<UnityEngine.UI.CanvasScaler>();
            canvasScaler.scaleFactor = screenScale;

        }


        // notify run
        public void ShowNotifyRun(string msg)
        {
            if(goTextRun != null)
            {
                goTextRun.SetActive(true);
                vkTextRun.Init(msg);
            }
        }

        public void HideNotifyRun()
        {
            if(goTextRun != null)
            {
                vkTextRun.StopMove();
                goTextRun.SetActive(false);
            }
        }
    }

    public class NotifyItemData
    {
        public VKNotifyController.TypeNotify type;
        public string content;
    }
}