using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VKSdk.Notify;
using VKSdk;

namespace VKSdk.UI
{
    public class VKLayerController : MonoBehaviour
    {
        public Camera uiCamera;

        public RectTransform rectMain;

        public GameObject layerMiniMask;
        public VKLoading layerLoading;

        public string firstLayer;
        public string path = "UI/Layers";

        public int deepOrder = 5;
        public int deepPlaneDistance = 100;

        public int[] deepOrderStarts;
        public int[] planeDistanceStarts;

        public bool lanscapse;

        public List<string> readyLayers;
        public Dictionary<string, List<VKLayer>> layerCaches;

        [Space(20)]
        public List<GameObject> prefabLayers;

        [Space(20)]
        public List<CanvasScaler> uiCanvasScalers;

        private Dictionary<VKLayer.Position, List<VKLayer>> layers;

        // [HideInInspector]
        public float screenRatio;
        // [HideInInspector]
        public float canvasScale;

        private int screenWidthCache;
        private int screenHeightCache;

        #region Sinleton


        public static VKLayerController Instance;
        
        void Awake()
        {

            if (Instance == null)
            {
                Instance = this;
                layers = new Dictionary<VKLayer.Position, List<VKLayer>>();
                layerCaches = new Dictionary<string, List<VKLayer>>();

                layers.Add(VKLayer.Position.Bootom, new List<VKLayer>());
                layers.Add(VKLayer.Position.Middle, new List<VKLayer>());
                layers.Add(VKLayer.Position.Top, new List<VKLayer>());

                // CalculatorScale();
                // ReloadCanvasScaler();
                if (VKNotifyController.Instance != null)
                {
                    if (uiCamera == null)
                    {
                        uiCamera = Camera.main;
                    }
                    VKNotifyController.Instance.SetCamera(uiCamera);
                }
                DontDestroyOnLoad(this);

            }
            else
            {
                DestroyImmediate(gameObject);
            }
            
            
        }
      
        #endregion

        #region Unity Method
       

        void Start()
        {
            CalculatorScale();
            ReloadCanvasScaler();

            if (readyLayers != null && readyLayers.Count > 0)
            {
                foreach (var key in readyLayers)
                {
                    if (Resources.Load<GameObject>(path + "/" + key) != null)
                    {
                        VKLayer layer = CreateLayer(key);
                        layer.BeforeHideLayer();
                        layer.HideLayer();
                        layer.gameObject.SetActive(false);

                        CacheLayer(layer);
                    }
                }
            }

            if (!string.IsNullOrEmpty(firstLayer))
            {
                VKLayer mLayer = ShowLayer(firstLayer);
                mLayer.FirstLoadLayer();
            }

            StartCoroutine(WaitCheckScreenSize());
        }

#if UNITY_WEBGL
    private IEnumerator WaitCheckScreenSize()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            if (rectMain != null && (screenWidthCache != (int)rectMain.sizeDelta.x || screenHeightCache != (int)rectMain.sizeDelta.y))
            {
                CalculatorScale();
                ReloadCanvasScaler();
            }
        }
    }
#else
    private IEnumerator WaitCheckScreenSize()
    {
        yield return new WaitForSeconds(10f);
        if (rectMain != null && (screenWidthCache != (int)rectMain.sizeDelta.x || screenHeightCache != (int)rectMain.sizeDelta.y))
        {
            CalculatorScale();
            ReloadCanvasScaler();
        }
    }
#endif
        #endregion

        #region Method
        // create
        /* Show layer ở ngay trên layer gọi ra nó
         * keyParent = layer gọi ra nó
         */
        public VKLayer ShowLayer(string key)
        {
           
            return ShowLayer(CreateLayer(key));
        }

        private VKLayer ShowLayer(VKLayer layer)
        {
            if (layer == null)
                return null;

            VKLayer lastLayer = null;

            List<VKLayer> vkLayerTemps = layers[layer.position];

            int layerCount = vkLayerTemps.Count;

            // disable layer
            if (layerCount > 0)
            {
                lastLayer = vkLayerTemps[layerCount - 1];
                lastLayer.DisableLayer();
            }

            if (layer.position == VKLayer.Position.Middle)
                layerMiniMask.SetActive(true);


            layer.SetLayerIndex(layerCount);
            vkLayerTemps.Add(layer);
            layer.EnableLayer();

            // animation
            switch (layer.layerAnimType)
            {
                case VKLayer.AnimType.Popup:
                    layer.PlayAnimation(VKLayer.AnimKey.OpenPopup);
                    break;
                case VKLayer.AnimType.Normal:
                    layer.PlayAnimation(VKLayer.AnimKey.Open);
                    break;
                case VKLayer.AnimType.Slide:
                    layer.PlayAnimation(VKLayer.AnimKey.Open);
                    if (lastLayer != null)
                        lastLayer.PlayAnimation(VKLayer.AnimKey.Hide);
                    break;
            }

            return layer;
        }

        private VKLayer CreateLayer(string nameLayer, GameObject obj = null)
        {
            VKLayer sLayer = null;        
            // get exists
            bool isCreate = true;
            if (layerCaches.ContainsKey(nameLayer) && layerCaches[nameLayer].Count > 0)
            {
                isCreate = false;

                sLayer = layerCaches[nameLayer][0];
                sLayer.gameObject.SetActive(true);

                layerCaches[nameLayer].RemoveAt(0);
            }
            else
            {
                if (obj == null)
                {
                    obj = GetPrefabLayer(nameLayer);
                    if (obj == null)
                    {
                        try
                        {
                            obj = Resources.Load(path + "/" + nameLayer) as GameObject;
                        }
                        catch
                        {
                        }
                    }

                    if (obj == null)
                    {
                        VKDebug.LogError(nameLayer + " Layer not found from local");
                        return null;
                    }
                }

                obj = Instantiate(obj) as GameObject;
                sLayer = obj.GetComponent<VKLayer>();

                // seting init
                sLayer.InitLayer(nameLayer, screenRatio, canvasScale);              
                sLayer.canvas.renderMode = RenderMode.ScreenSpaceCamera;
                if (uiCamera == null)
                {
                    uiCamera = Camera.main;
                }
                sLayer.canvas.worldCamera = uiCamera;
            }

            List<VKLayer> vkLayerTemps = layers[sLayer.position];
            int countLayer = vkLayerTemps.Count;

            // set position
            int sorting = countLayer == 0 ? deepOrderStarts[(int)sLayer.position] : (vkLayerTemps[countLayer - 1].canvas.sortingOrder + deepOrder);
            float distance = countLayer == 0 ? planeDistanceStarts[(int)sLayer.position] : (vkLayerTemps[countLayer - 1].canvas.planeDistance - deepPlaneDistance);

            sLayer.transform.SetAsLastSibling();
            sLayer.name = nameLayer + "_" + (countLayer + 1);

            sLayer.SetSortOrder(sorting);
            sLayer.canvas.planeDistance = distance;

            // action
            if (isCreate)
                sLayer.StartLayer();
            sLayer.ShowLayer();

            return sLayer;
        }

        public void HideLayer(string key)
        {
            var layer = GetLayer(key);
            if (layer != null)
                HideLayer(layer);
        }

        public void HideLayer(VKLayer layer)
        {
            if (layer == null)
                return;

            List<VKLayer> vkLayerTemps = layers[layer.position];

            if (!vkLayerTemps.Contains(layer))
                return;

            // remove
            vkLayerTemps.Remove(layer);

            VKLayer lastLayer = null;
            if (layer.layerIndex > 0 && vkLayerTemps.Count > layer.layerIndex - 1)
            {
                try
                {
                    lastLayer = vkLayerTemps[layer.layerIndex - 1];
                    lastLayer.gameObject.SetActive(true);
                    lastLayer.ReloadLayer();
                }
                catch (Exception e)
                {
                    VKDebug.LogWarning("DONOT HAVE LAYER " + (layer.layerIndex - 1) + " - " + e.Message);
                }
            }

            if (layer.layerIndex == vkLayerTemps.Count)
            {
                if (lastLayer != null)
                    lastLayer.EnableLayer();
            }
            else
            {
                for (int i = layer.layerIndex; i < vkLayerTemps.Count; i++)
                    vkLayerTemps[i].SetLayerIndex(i);
            }

            // call hide
            layer.BeforeHideLayer();
            if (layer.position == VKLayer.Position.Middle && vkLayerTemps.Count <= 0)
            {
                layerMiniMask.SetActive(false);
                StartCoroutine(WaitRemoveLayerGame(layer));
            }

            switch (layer.layerAnimType)
            {
                case VKLayer.AnimType.None:
                    layer.HideLayer();

                    if (layer.allowDestroy)
                    {
                        layer.DestroyLayer();
                        Destroy(layer.gameObject);
                        UnloadAllAssets();
                    }
                    else
                    {
                        CacheLayer(layer);
                    }
                    break;
                case VKLayer.AnimType.Normal:
                    layer.PlayAnimation(VKLayer.AnimKey.Close);
                    break;
                case VKLayer.AnimType.Slide:
                    layer.PlayAnimation(VKLayer.AnimKey.Close);
                    if (lastLayer != null)
                    {
                        lastLayer.PlayAnimation(VKLayer.AnimKey.ReOpen);
                    }
                    break;
                case VKLayer.AnimType.Popup:
                    layer.PlayAnimation(VKLayer.AnimKey.ClosePopup);
                    break;
            }
        }

        public void CacheLayer(VKLayer layer)
        {
            if (layer.allowDestroy)
            {
                layer.DestroyLayer();
                Destroy(layer.gameObject);
                UnloadAllAssets();
            }
            else
            {
                layer.gameObject.SetActive(false);

                if (!layerCaches.ContainsKey(layer.layerKey))
                    layerCaches.Add(layer.layerKey, new List<VKLayer>());
                layerCaches[layer.layerKey].Add(layer);
            }
        }

        private void PrivateHideLayer(VKLayer layer)
        {
            layer.DisableLayer();
            layer.BeforeHideLayer();
            layer.HideLayer();

            if (layer.allowDestroy)
            {
                layer.DestroyLayer();
                Destroy(layer.gameObject);
            }
            else
            {
                layer.ResetPosition();
                CacheLayer(layer);
            }
        }

        private IEnumerator WaitRemoveLayerGame(VKLayer layer)
        {
            yield return new WaitUntil(() => (layer == null || !layer.gameObject.activeSelf));

            RemoveLayerGame();
        }

        public void RemoveLayerGame()
        {
            foreach (var item in layerCaches)
            {
                var layerTemps = item.Value;
                if (layerTemps.Count > 0)
                {
                    for (int i = layerTemps.Count - 1; i >= 0; i--)
                    {
                        VKLayer layer = layerTemps[i];
                        if (layer.isGameLayer)
                        {
                            layerTemps.Remove(layer);

                            layer.DisableLayer();
                            layer.BeforeHideLayer();
                            layer.HideLayer();
                            layer.DestroyLayer();

                            Destroy(layer.gameObject);
                        }
                    }
                }
            }

            UnloadAllAssets();
        }

        // go login
        public void GotoLogin()
        {
            // hide all
            HideAllLayer();

            ShowLayer(firstLayer);
        }

        // hide all layer
        public void HideAllLayer()
        {
            // hide mask
            HideLoading();
            layerMiniMask.SetActive(false);

            // khong co lobby nen hide truoc
            foreach (var layer in layers[VKLayer.Position.Middle])
                PrivateHideLayer(layer);

            // khong co lobby nen hide truoc
            foreach (var layer in layers[VKLayer.Position.Top])
                PrivateHideLayer(layer);

            // co lobby nen hide sau
            foreach (var layer in layers[VKLayer.Position.Bootom])
                PrivateHideLayer(layer);

            layers[VKLayer.Position.Bootom].Clear();
            layers[VKLayer.Position.Middle].Clear();
            layers[VKLayer.Position.Top].Clear();

            // remove layer game
            RemoveLayerGame();

            // clear asetbundle
            UnloadAllAssets();
        }

        // loading
        public void ShowLoading(bool reloadBG=false)
        {
            ShowLoading(true,null,reloadBG);
        }
        public void ShowLoading()
        {
            ShowLoading(true);
        }

        public void ShowLoading(bool autoHide, Action actionCallback = null, bool reloadBG=false)
        {
            // Debug.Log("ShowLoading");
            layerLoading.ShowLoading(autoHide,actionCallback,reloadBG);
        }

        public void HideLoading()
        {
            // Debug.Log("HideLoading");
            layerLoading.HideLoading();
        }

        // drag mini game
        public void FocusMiniGame(VKDragLayerEvent drag)
        {
            List<VKLayer> layerTemps = layers[VKLayer.Position.Middle].Where(a => a.dragMini != null).ToList();

            if (layerTemps.Count > 0)
            {
                if (drag == null)
                {
                    layerMiniMask.SetActive(false);
                    foreach (var layer in layerTemps)
                        layer.dragMini.canvasGroup.alpha = 0.4f;
                }
                else
                {
                    layerMiniMask.SetActive(true);

                    foreach (var layer in layerTemps)
                        layer.dragMini.canvasGroup.alpha = 1f;

                    layerTemps = layerTemps.OrderBy(a => a.canvas.sortingOrder).ToList();
                    VKLayer layerTop = layerTemps.Last();
                    VKLayer layerCurrent = layerTemps.FirstOrDefault(a => a.dragMini.Equals(drag));

                    if (layerCurrent != null && !layerTop.Equals(layerCurrent))
                    {
                        int order = layerTop.canvas.sortingOrder;
                        int layerIndex = layerTop.layerIndex;
                        float distance = layerTop.canvas.planeDistance;

                        layerTop.layerIndex = layerCurrent.layerIndex;
                        layerTop.SetSortOrder(layerCurrent.canvas.sortingOrder);
                        layerTop.canvas.planeDistance = layerCurrent.canvas.planeDistance;

                        layerCurrent.layerIndex = layerIndex;
                        layerCurrent.SetSortOrder(order);
                        layerCurrent.canvas.planeDistance = distance;
                    }
                    layers[VKLayer.Position.Middle] = layers[VKLayer.Position.Middle].OrderBy(a => a.canvas.sortingOrder).ToList();
                }
            }
        }

        //close all popup bottom
        public void CloseAllPopupBottom()
        {
            List<VKLayer> layerTemps = layers[VKLayer.Position.Bootom].Where(a => a.layerKey == "LPopup").ToList();
            foreach (var layer in layerTemps)
            {
                layer.Close();
            }
        }

        // get
        public T GetLayer<T>()
        {
            T cacheT = default(T);
            foreach (var item in layers)
            {
                foreach (var layer in item.Value)
                {
                    if (layer is T)
                    {
                        cacheT = (T)(object)layer;
                    }
                }
            }

            return cacheT;
        }

        public VKLayer GetLayer(string key)
        {
            VKLayer layer = null;
            foreach (var item in layers)
            {
                if (layer == null)
                {
                    layer = item.Value.LastOrDefault(a => a.layerKey.Equals(key));
                }
            }

            return layer;
        }

        public T GetLayerCache<T>()
        {
            T cacheT = default(T);
            foreach (var item in layerCaches)
            {
                foreach (var layer in item.Value)
                {
                    if (layer is T)
                    {
                        cacheT = (T)(object)layer;
                    }
                }
            }

            return cacheT;
        }

        public VKLayer GetLayerCache(string key)
        {
            VKLayer layer = null;
            foreach (var item in layerCaches)
            {
                if (layer == null)
                {
                    layer = item.Value.LastOrDefault(a => a.layerKey.Equals(key));
                }
            }

            return layer;
        }

        public VKLayer GetCurrentLayer(VKLayer.Position position)
        {
            var layerTemps = layers[position];
            if (layerTemps.Count > 0)
                return layerTemps[layerTemps.Count - 1];
            return null;
        }

        public bool IsCurrentLayer(string key)
        {
            foreach (var item in layers)
            {
                if (item.Value.Count > 0)
                {
                    if (item.Value[item.Value.Count - 1].layerKey.Equals(key))
                        return true;
                }
            }

            return false;
        }

        public bool IsLayerExisted<T>()
        {
            return GetLayer<T>() != null;
        }

        public bool IsLayerExisted(string key)
        {
            bool exist = false;
            foreach (var item in layers)
            {
                if (!exist)
                {
                    exist = item.Value.Exists(a => a != null && a.layerKey.Equals(key));
                }
            }

            return exist;
        }

        public bool IsLayerLoadingActive()
        {
            return layerLoading.gameObject.activeSelf;
        }
        
        // unload aset
        private void UnloadAllAssets()
        {
            StartCoroutine(UnloadAllUnusedAssets());
        }

        private IEnumerator UnloadAllUnusedAssets()
        {
            yield return Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        public GameObject GetPrefabLayer(string name)
        {
            return prefabLayers.FirstOrDefault(a => a.name.Equals(name));
        }
        #endregion

        #region Calculator Scale
        public static float GetScale(int width, int height, Vector2 scalerReferenceResolution)
        {
            float aspectRatio = Mathf.Max(Screen.width, Screen.height) / Mathf.Min(Screen.width, Screen.height);
            float scalerMatchWidthOrHeight = (aspectRatio <= 1.6f) ? 0 : 1f;
            return Mathf.Pow(width / scalerReferenceResolution.x, 1f - scalerMatchWidthOrHeight) *
                   Mathf.Pow(height / scalerReferenceResolution.y, scalerMatchWidthOrHeight);
        }
        public void CalculatorScale()
        {
            float width = Screen.width;
            float height = Screen.height;
            canvasScale = GetScale(Screen.width, Screen.height, new Vector2(1920, 1080));
            if (UserData.Instance.GameData.vip == 0)
            {
                canvasScale = canvasScale * StaticData.ADS_SCALE_RATIO;
            }
            if (rectMain != null)
            {
                width = rectMain.sizeDelta.x;
                height = rectMain.sizeDelta.y;
            }

            //if (UserData.Instance.GameData.vip==1)
            //{
            //    canvasScale = 1f;
            //}
            //else
            //{
            //    canvasScale = 0.9f;
            //}
            // Debug.Log("screenRatio " + screenRatio);

            screenWidthCache = (int)width;
            screenHeightCache = (int)height;
        }

        public void ReloadCanvasScaler()
        {
            CalculatorScale();
            foreach (var cv in uiCanvasScalers)
            {
                if (cv != null)
                {
                    cv.scaleFactor = canvasScale;
                }
            }

            foreach (var item in layers)
            {
                foreach (var layer in item.Value)
                {
                    if (layer != null)
                    {
                        layer.ReloadCanvasScale(screenRatio, canvasScale);
                    }
                }
            }

            if(VKNotifyController.Instance != null)
            {
                VKNotifyController.Instance.ReloadCanvasScale(screenRatio, canvasScale);
            }
        }
        #endregion

        #region Support
        // mouse
        public Vector3 GetMousePoint()
        {
            return uiCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        public Vector3 GetMousePoint(Vector3 pos)
        {
            return Camera.main.WorldToScreenPoint(pos);
        }

        public Vector3 WorldToScreenPoint(Vector3 worldPoint)
        {
            return uiCamera.WorldToScreenPoint(worldPoint);
        }

        public bool IsIpad()
        {
            return screenRatio < 1.5f;
        }

        // public Vector2 GetRectSize()
        // {
        //     if(rectMain != null) return rectMain.sizeDelta;
        //     return Vector2.zero;
        // }
        #endregion
    }
}