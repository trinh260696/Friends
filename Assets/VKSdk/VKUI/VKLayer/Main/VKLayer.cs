using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VKSdk.Lua;
using VKSdk;
using System;

namespace VKSdk.UI
{
    public class VKLayer : MonoBehaviour
    {
        public enum AnimKey
        {
            OpenPopup,
            Open,
            ReOpen,
            Close,
            ClosePopup,
            Hide
        };

        public enum Position
        {
            Bootom = 0,
            Middle,
            Top
        }

        public enum AnimType
        {
            None = 0,
            Slide,
            Popup,
            Normal,
        }

        [Space(10)]
        public VKDragLayerEvent dragMini;

        [Space(10)]
        public GameObject gContentAll;

        [Space(10)]
        public AnimType layerAnimType;

        [Space(10)]
        public bool allowDestroy;
        public bool isGameLayer;
        public bool lockCanvasScale;

        [Space(10)]
        public Position position = Position.Bootom;

        [Space(10)]
        public GraphicRaycaster[] childGraphicRaycasters;

        [Space(10)]
        public List<VKLayerChildOrder> childOrders;

        [HideInInspector]
        public Animator anim;
        [HideInInspector]
        public Canvas canvas;
        [HideInInspector]
        public GraphicRaycaster graphicRaycaster;
        [HideInInspector]
        public int layerIndex;
        [HideInInspector]
        public string layerKey;
        [HideInInspector]
        public bool isLayerAnimOpenDone;
        [HideInInspector]
        public Action onActionClose;

#if VK_XLUA
        [Space(20)]
        public VKLuaBehaviour xlua;
#endif

        public void InitLayer(string layerKey, float screenRatio, float screenScale)
        {
            NotificationCenter.DefaultCenter().AddObserver(this, "HideBanner");
            isLayerAnimOpenDone = false;

            this.layerKey = layerKey;
            canvas = GetComponent<Canvas>();
            anim = GetComponent<Animator>();
            graphicRaycaster = GetComponent<GraphicRaycaster>();

            CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
            if (!lockCanvasScale)
                canvasScaler.scaleFactor = screenScale;

#if UNITY_EDITOR
        if (canvas == null)
            VKDebug.LogError("Layer need a Canvas");
        if (graphicRaycaster == null)
            VKDebug.LogError("Layer need a GraphicRaycaster");
#endif

#if VK_XLUA
            if (xlua != null)
            {
                xlua.InvokeLua("InitLayer", layerKey, screenRatio);
            }
#endif
        }

        public void SetLayerIndex(int index)
        {
            layerIndex = index;
        }

        public void SetActionClose(Action onClose)
        {
            onActionClose = onClose;
        }

        protected void SetGraphicRaycaster(bool isEnable)
        {
            graphicRaycaster.enabled = isEnable;
            if(childGraphicRaycasters != null && childGraphicRaycasters.Length > 0)
            {
                for(int i = 0; i < childGraphicRaycasters.Length; ++i)
                {
                    childGraphicRaycasters[i].enabled = isEnable;
                }
            }
            
        }

        public virtual void ReloadCanvasScale(float screenRatio, float screenScale)
        {
            if (!lockCanvasScale)
            {
                CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
                canvasScaler.scaleFactor = screenScale;
            }
            
        }
        public virtual void HideBanner()
        {
            var scaleFactor = VKLayerController.GetScale(Screen.width, Screen.height, new Vector2(1920, 1080));
            gameObject.GetComponent<CanvasScaler>().scaleFactor = scaleFactor;
        }

        /**
         * Khoi chay 1 lan khi layer duoc tao
         */
        public virtual void StartLayer()
        {
            if (layerAnimType == AnimType.None)
            {
                isLayerAnimOpenDone = true;
            }

#if VK_XLUA
            if (xlua != null)
            {
                xlua.InvokeLua("StartLayer");
            }
#endif
        }

        /**
         * Khoi chay 1 lan tren layer dau tien duoc tao tren scene
         */
        public virtual void FirstLoadLayer()
        {
#if VK_XLUA
            if (xlua != null)
            {
                xlua.InvokeLua("FirstLoadLayer");
            }
#endif
        }

        /**
         * Khoi chay khi layer duoc add vao list active
         */
        public virtual void ShowLayer()
        {
#if VK_XLUA
            if (xlua != null)
            {
                xlua.InvokeLua("ShowLayer");
            }
#endif
            canvas.sortingLayerName = "UI";
        }

        /**
         * Khoi chay khi layer la layer dau tien
         */
        public virtual void EnableLayer()
        {
            SetGraphicRaycaster(true);

#if VK_XLUA
            if (xlua != null)
            {
                xlua.InvokeLua("EnableLayer");
            }
#endif
        }

        /**
         * Khoi chay 1 lan khi layer duoc goi lai
         */
        public virtual void ReloadLayer()
        {
#if VK_XLUA
            if (xlua != null)
            {
                xlua.InvokeLua("ReloadLayer");
            }
#endif
        }

        public virtual void BeforeHideLayer()
        {
#if VK_XLUA
            if (xlua != null)
            {
                xlua.InvokeLua("BeforeHideLayer");
            }
#endif
        }

        public virtual void DisableLayer()
        {
            if (position != Position.Middle)
            {
                SetGraphicRaycaster(false);
            }

#if VK_XLUA
            if (xlua != null)
            {
                xlua.InvokeLua("DisableLayer");
            }
#endif
        }

        public virtual void HideLayer()
        {
#if VK_XLUA
            if (xlua != null)
            {
                xlua.InvokeLua("HideLayer");
            }
#endif
        }

        public virtual void DestroyLayer()
        {
#if VK_XLUA
            if (xlua != null)
            {
                xlua.InvokeLua("DestroyLayer");
            }
#endif
        }

        // func
        public void SetSortOrder(int order)
        {
            canvas.sortingOrder = order;

            if (childOrders != null)
                childOrders.ForEach(a => a.ResetOrder(canvas.sortingOrder));
        }

        public void ResetPosition()
        {
            if (gContentAll != null)
            {
                RectTransform rect = gContentAll.GetComponent<RectTransform>();
                if (layerAnimType == AnimType.Slide)
                {
                    rect.offsetMin = new Vector2(1334, 0);
                    rect.offsetMax = new Vector2(1334, 0);
                }
                else
                {
                    rect.localPosition = new Vector2(0, 0);
                    rect.localPosition = new Vector2(0, 0);
                }
            }

#if VK_XLUA
            if (xlua != null)
            {
                xlua.InvokeLua("ResetPosition");
            }
#endif
        }

        private void ResetAfterAnim()
        {
            if (gContentAll != null)
            {
                if (layerAnimType != AnimType.Slide)
                {
                    gContentAll.transform.localScale = Vector3.one;

                    RectTransform rect = gContentAll.GetComponent<RectTransform>();
                    rect.localPosition = new Vector2(0, 0);
                    rect.localPosition = new Vector2(0, 0);

                    CanvasGroup cvGroup = gContentAll.GetComponent<CanvasGroup>();
                    cvGroup.alpha = 1;
                }
            }

#if VK_XLUA
            if (xlua != null)
            {
                xlua.InvokeLua("ResetAfterAnim");
            }
#endif
        }

        public void PlayAnimation(AnimKey key)
        {
            if((key == AnimKey.ReOpen || key == AnimKey.Hide) && (layerAnimType == AnimType.Popup))
            {
                isLayerAnimOpenDone = true;
                return;
            }

            if (anim != null)
            {
                isLayerAnimOpenDone = false;
                anim.enabled = true;

                switch(layerAnimType)
                {
                    case AnimType.Popup:
                        anim.SetTrigger(key.ToString());
                        if(key == AnimKey.OpenPopup) StartCoroutine(DelayToResetAfterAnim());
                        break;
                    case AnimType.None:
                    case AnimType.Normal:
                        SetGraphicRaycaster(false);
                        if(key == AnimKey.ReOpen || key == AnimKey.Hide)
                        {
                            StartCoroutine(DelaytoRunAnim(key));
                        }
                        else
                        {
                            anim.SetTrigger(key.ToString());
                        }
                        break;
                    case AnimType.Slide:
                        SetGraphicRaycaster(false);
                        StartCoroutine(DelaytoRunAnim(key));
                        break;
                }
            }
            else
            {
                isLayerAnimOpenDone = true;
            }
        }

        IEnumerator DelaytoRunAnim(AnimKey key)
        {
            yield return new WaitForSeconds(0.2f);
            anim.SetTrigger(key.ToString());
        }

        IEnumerator DelayToResetAfterAnim()
        {
            yield return new WaitForSeconds(0.5f);

            if (gContentAll != null)
            {
                if (layerAnimType != AnimType.Slide)
                {
                    CanvasGroup cvGroup = gContentAll.GetComponent<CanvasGroup>();
                    if (cvGroup.alpha < 1)
                    {
                        gContentAll.transform.localScale = Vector3.one;

                        RectTransform rect = gContentAll.GetComponent<RectTransform>();
                        rect.localPosition = new Vector2(0, 0);
                        rect.localPosition = new Vector2(0, 0);

                        cvGroup.alpha = 1;
                    }
                }
            }
        }

        public virtual void Close()
        {
#if VK_XLUA
            if (xlua != null)
            {
                xlua.InvokeLua("BeforeClose");
            }
#endif

            if (onActionClose != null)
            {
                onActionClose.Invoke();
            }

#if VK_XLUA
            if (xlua != null)
            {
                xlua.InvokeLua("Close");
            }
#endif

            SetGraphicRaycaster(false);
            VKLayerController.Instance.HideLayer(this);
        }

        #region Anim Action Done
        public virtual void OnLayerOpenDone()
        {
            SetGraphicRaycaster(true);
            isLayerAnimOpenDone = true;

#if VK_XLUA
            if (xlua != null)
            {
                xlua.InvokeLua("OnLayerOpenDone");
            }
#endif
        }

        public virtual void OnLayerCloseDone()
        {
            HideLayer();
            VKLayerController.Instance.CacheLayer(this);
            isLayerAnimOpenDone = false;

#if VK_XLUA
            if (xlua != null)
            {
                xlua.InvokeLua("OnLayerCloseDone");
            }
#endif
        }

        public virtual void OnLayerOpenPopupDone()
        {
            SetGraphicRaycaster(true);
            anim.enabled = false;
            ResetAfterAnim();
            isLayerAnimOpenDone = true;

#if VK_XLUA
            if (xlua != null)
            {
                xlua.InvokeLua("OnLayerOpenPopupDone");
            }
#endif
        }

        public virtual void OnLayerPopupCloseDone()
        {
            anim.enabled = false;
            HideLayer();
            VKLayerController.Instance.CacheLayer(this);
            isLayerAnimOpenDone = false;

#if VK_XLUA
            if (xlua != null)
            {
                xlua.InvokeLua("OnLayerPopupCloseDone");
            }
#endif
        }

        // khong phai la hidelayer ma chi dich sang ben trai roi an di
        public virtual void OnLayerSlideHideDone()
        {
            anim.enabled = false;

            isLayerAnimOpenDone = false;
            if (!isGameLayer)
            {
                gameObject.SetActive(false);
            }

#if VK_XLUA
            if (xlua != null)
            {
                xlua.InvokeLua("OnLayerSlideHideDone");
            }
#endif
        }

        public virtual void OnLayerReOpenDone()
        {
            anim.enabled = false;

            SetGraphicRaycaster(true);
            isLayerAnimOpenDone = true;

#if VK_XLUA
            if (xlua != null)
            {
                xlua.InvokeLua("OnLayerReOpenDone");
            }
#endif
        }
        #endregion
    }
}