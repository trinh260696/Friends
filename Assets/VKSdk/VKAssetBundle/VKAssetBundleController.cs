using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LitJson;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VKSdk.Lua;
using VKSdk.Notify;
using VKSdk.UI;

namespace VKSdk
{
    [System.Serializable]
    public class VKAssetBundleConfig
    {
        public string link;
        public List<VKAssetBundleItem> assets;

        public VKAssetBundleItem GetAssetByName(string name)
        {
            if (assets == null)
                return null;

            return assets.FirstOrDefault(a => a.name.Equals(name));
        }
    }

    [System.Serializable]
    public class VKAssetBundleItem
    {
        public string name;
        public int android;
        public int ios;
        public int window;
        public int webgl;

        public int GetVersion()
        {
#if UNITY_ANDROID
        return android;
#elif UNITY_IOS
        return ios;
#elif UNITY_STANDALONE_WIN
        return window;
#elif UNITY_STANDALONE_OSX
        return window;
#elif UNITY_WEBGL
        return webgl;
#endif
        }
    }

    public class VKAssetBundleController : MonoBehaviour
    {
        #region Properties
        public Dictionary<string, AssetBundle> asset;
        public AssetBundle assetMainGame;

        public VKAssetBundleConfig assetConfig;
        #endregion

        #region Singleton
        private static VKAssetBundleController instance;

        public static VKAssetBundleController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<VKAssetBundleController>();
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

            DontDestroyOnLoad(this.gameObject);
        }

        #endregion

        #region UnityMethod
        void Start()
        {
            asset = new Dictionary<string, AssetBundle>();
        }
        #endregion

        #region Method
        public void Init(string json)
        {
            assetConfig = JsonMapper.ToObject<VKAssetBundleConfig>(json);
        }

        public void ClearAssetBundle()
        {
            assetMainGame = null;

            StopAllCoroutines();

            // ignore clear maingame
            if (asset != null)
            {
                //AssetBundle assMainGame = null;
                foreach (var ass in asset)
                {
                    ass.Value.Unload(true);
                }
                asset.Clear();
            }
        }

        public void ClearAssetBundleMainGame()
        {
            // ignore clear maingame
            if (assetMainGame != null)
            {
                assetMainGame.Unload(false);
                assetMainGame = null;
            }
        }

        public void ClearAssetBundleByKey(string key)
        {
            StopAllCoroutines();

            if (asset.ContainsKey(key))
            {
                var ass = asset[key];
                ass.Unload(true);
                asset.Remove(key);
            }
        }

        public void UnloadAsset(string assetName, bool isUnloadLoaded)
        {
            if (asset.ContainsKey(assetName))
            {
                asset[assetName].Unload(isUnloadLoaded);
                asset.Remove(assetName);
            }
        }
        #endregion

        #region Download Asset
        public void LoadAsset<T>(string assetName, string mainAssetName, VKProgressBar progressBar, Action<string, T> callback, bool isCache)  where T : UnityEngine.Object
        {
            Debug.Log("assetName: " + assetName);
            Debug.Log("mainAssetName: " + mainAssetName);
            LoadAsset(assetConfig.GetAssetByName(assetName), mainAssetName, progressBar, callback, isCache);
        }

        public void LoadAsset<T>(VKAssetBundleItem assetItem, string mainAssetName, VKProgressBar progressBar, Action<string, T> callback, bool isCache)  where T : UnityEngine.Object
        {
            // check config asset tu server
            if (assetItem == null)
            {
                VKNotifyController.Instance.AddNotify("50_PopupContents/SYS_ASSET_MISSING", VKNotifyController.TypeNotify.Error);
                callback.Invoke(mainAssetName, null);
                return;
            }

            // check co phai editor va bat simulator ko neu co thi tim luon prefab do trong project
#if UNITY_EDITOR
            if (SimulateAssetBundleInEditor)
            {
                callback.Invoke(mainAssetName, GetAssetFromLocal<T>(assetItem.name, mainAssetName));
                return;
            }
#endif

            // check co dang cache khong thi lay prefab tu assetbundle do luon
            if (asset.ContainsKey(assetItem.name))
            {
                StartCoroutine(LoadAssetFromAssetBundle(asset[assetItem.name], mainAssetName, callback));
            }
            else
            {
                // tai tu duoi cache hoac tu server sau do lay ra prefab
                StartCoroutine(DownloadAssetAndLoadAsset(assetItem, mainAssetName, progressBar, callback, isCache));
            }
        }

        IEnumerator LoadAssetFromAssetBundle<T>(AssetBundle assetBundle, string mainAssetName, Action<string, T> callback) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            yield return null;
            callback.Invoke(mainAssetName, assetBundle.LoadAsset<T>(mainAssetName));
#else
            AssetBundleRequest request = assetBundle.LoadAssetAsync<T>(mainAssetName);
            yield return request;

            callback.Invoke(mainAssetName, request.asset as T);
#endif
        }

        IEnumerator DownloadAssetAndLoadAsset<T>(VKAssetBundleItem assetItem, string mainAssetName, VKProgressBar progressBar, Action<string, T> callback, bool isCache) where T : UnityEngine.Object
        {
            string url = assetConfig.link + "/" + VKGameConfig.Instance.VersionGame() + "/" + VKGameConfig.Instance.PlatformDownloadAsset() + "/" + assetItem.name;
            string urlDebug = "*/" + VKGameConfig.Instance.VersionGame() + "/" + VKGameConfig.Instance.PlatformDownloadAsset() + "/" + assetItem.name;
            Debug.Log("DOWNLOAD ASSET: " + url);

            // check da download chua
            int currentVersion = PlayerPrefs.GetInt(assetItem.name, -1);
            bool isShowDownload = currentVersion != assetItem.GetVersion();

            // Show UI download
            if (isShowDownload && progressBar != null)
            {
                progressBar.gameObject.SetActive(true);
                progressBar.SetProgress(0f);
            }

            while (!Caching.ready)
                yield return null;

            using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(url, (uint)assetItem.GetVersion(), 0))
            {
                var operation = uwr.SendWebRequest();

                while (!operation.isDone)
                {
                    // Show progress download
                    if (isShowDownload && progressBar != null)
                    {
                        progressBar.SetProgress(uwr.downloadProgress * 100);
                    }

                    yield return new WaitForEndOfFrame();
                }

                // Show download done
                if (isShowDownload && progressBar != null)
                {
                    progressBar.SetProgress(100);
                }

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    VKDebug.Log("Error while Downloading Data: " + uwr.error);
                    LPopup.OpenPopup("ERROR", "Tải Asset bị lỗi xin thử lại sau!" + "\n" + urlDebug + "\n" + uwr.error);
                    yield return null;
                    callback.Invoke(mainAssetName, null);
                }
                else
                {
                    VKDebug.Log("Download Asset Success");
                    AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(uwr);

                    if (assetBundle == null) // || !assetBundle.Contains(mainAssetName))
                    {
                        VKDebug.Log("Asset được tải " + (assetBundle == null ? "NULL" : ("không có " + mainAssetName)));
                        LPopup.OpenPopup("ERROR", "Asset được tải không đúng! Xin vui lòng thử lại!");
                        callback.Invoke(mainAssetName, null);
                    }
                    else
                    {
                        // load game object
                        if (isCache)
                        {
                            if (asset.ContainsKey(assetItem.name))
                            {
                                asset[assetItem.name] = assetBundle;
                            }
                            else
                            {
                                asset.Add(assetItem.name, assetBundle);
                            }
                        }

                        PlayerPrefs.SetInt(assetItem.name, assetItem.GetVersion());

#if UNITY_EDITOR
                        yield return null;
                        callback.Invoke(mainAssetName, assetBundle.LoadAsset<T>(mainAssetName));
#else
                        AssetBundleRequest request = assetBundle.LoadAssetAsync<T>(mainAssetName);
                        yield return request;

                        callback.Invoke(mainAssetName, request.asset as T);
#endif

                        if (!isCache)
                        {
                            assetBundle.Unload(false);
                        }
                    }
                }
            }
        }
        #endregion

        #region Download Asset

//         public void LoadPrefab(string assetName, string prefabName, VKProgressBar progressBar, Action<string, GameObject> callback, bool isCache)
//         {
//             LoadPrefab(assetConfig.GetAssetByName(assetName), prefabName, progressBar, callback, isCache);
//         }

//         public void LoadPrefab(VKAssetBundleItem assetItem, string prefabName, VKProgressBar progressBar, Action<string, GameObject> callback, bool isCache)
//         {
//             // check config asset tu server
//             if (assetItem == null)
//             {
//                 VKNotifyController.Instance.AddNotify("50_PopupContents/SYS_ASSET_MISSING", VKNotifyController.TypeNotify.Error);
//                 return;
//             }

//             // check co phai editor va bat simulator ko neu co thi tim luon prefab do trong project
// #if UNITY_EDITOR
//             if (SimulateAssetBundleInEditor)
//             {
//                 callback.Invoke(prefabName, GetAssetFromLocal(assetItem.name, prefabName) as GameObject);
//                 return;
//             }
// #endif

//             // check co dang cache khong thi lay prefab tu assetbundle do luon
//             if (asset.ContainsKey(assetItem.name))
//             {
//                 StartCoroutine(LoadPrefabFromAssetBundle(asset[assetItem.name], prefabName, callback));
//             }
//             else
//             {
//                 // tai tu duoi cache hoac tu server sau do lay ra prefab
//                 StartCoroutine(DownloadAssetAndLoadPrefab(assetItem, prefabName, progressBar, callback, isCache));
//             }
//         }

//         IEnumerator LoadPrefabFromAssetBundle(AssetBundle assetBundle, string prefabName, Action<string, GameObject> callback)
//         {
// #if UNITY_EDITOR
//             yield return null;
//             callback.Invoke(prefabName, assetBundle.LoadAsset<GameObject>(prefabName));
// #else
//             AssetBundleRequest request = assetBundle.LoadAssetAsync<GameObject>(prefabName);
//             yield return request;

//             callback.Invoke(prefabName, request.asset as GameObject);
// #endif
//         }

//         IEnumerator DownloadAssetAndLoadPrefab(VKAssetBundleItem assetItem, string prefabName, VKProgressBar progressBar, Action<string, GameObject> callback, bool isCache)
//         {
//             string url = assetConfig.link + "/" + VKGameConfig.Instance.VersionGame() + "/" + VKGameConfig.Instance.PlatformDownloadAsset() + "/" + assetItem.name;
//             string urlDebug = "*/" + VKGameConfig.Instance.VersionGame() + "/" + VKGameConfig.Instance.PlatformDownloadAsset() + "/" + assetItem.name;
//             Debug.Log("DOWNLOAD ASSET: " + url);

//             // check da download chua
//             int currentVersion = PlayerPrefs.GetInt(assetItem.name, -1);
//             bool isShowDownload = currentVersion != assetItem.GetVersion();

//             // Show UI download
//             if (isShowDownload && progressBar != null)
//             {
//                 progressBar.gameObject.SetActive(true);
//                 progressBar.SetProgress(0f);
//             }

//             while (!Caching.ready)
//                 yield return null;

//             using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(url, (uint)assetItem.GetVersion(), 0))
//             {
//                 var operation = uwr.SendWebRequest();

//                 while (!operation.isDone)
//                 {
//                     // Show progress download
//                     if (isShowDownload && progressBar != null)
//                     {
//                         progressBar.SetProgress(uwr.downloadProgress * 100);
//                     }

//                     yield return new WaitForEndOfFrame();
//                 }

//                 // Show download done
//                 if (isShowDownload && progressBar != null)
//                 {
//                     progressBar.SetProgress(100);
//                 }

//                 if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
//                 {
//                     VKDebug.Log("Error while Downloading Data: " + uwr.error);
//                     LPopup.OpenPopup("ERROR", "Tải game bị lỗi xin thử lại sau!" + "\n" + urlDebug + "\n" + uwr.error);
//                     yield return null;
//                     callback.Invoke(prefabName, null);
//                 }
//                 else
//                 {
//                     VKDebug.Log("Download Asset Success");
//                     AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(uwr);

//                     if (assetBundle == null) // || !assetBundle.Contains(prefabName))
//                     {
//                         VKDebug.Log("Game được tải " + (assetBundle == null ? "NULL" : ("không có " + prefabName)));
//                         LPopup.OpenPopup("ERROR", "Game được tải không đúng! Xin vui lòng thử lại!");
//                         callback.Invoke(prefabName, null);
//                     }
//                     else
//                     {
//                         // load game object
//                         if (isCache)
//                         {
//                             if (asset.ContainsKey(assetItem.name))
//                             {
//                                 asset[assetItem.name] = assetBundle;
//                             }
//                             else
//                             {
//                                 asset.Add(assetItem.name, assetBundle);
//                             }
//                         }

//                         PlayerPrefs.SetInt(assetItem.name, assetItem.GetVersion());

// #if UNITY_EDITOR
//                         yield return null;
//                         callback.Invoke(prefabName, assetBundle.LoadAsset<GameObject>(prefabName));
// #else
//                         AssetBundleRequest request = assetBundle.LoadAssetAsync<GameObject>(prefabName);
//                         yield return request;

//                         callback.Invoke(prefabName, request.asset as GameObject);
// #endif

//                         if (!isCache)
//                         {
//                             assetBundle.Unload(false);
//                         }
//                     }
//                 }
//             }
//         }

//         public void LoadScene(VKAssetBundleItem assetItem, string sceneName, VKProgressBar progressBar, bool isCache)
//         {
//             // check config asset tu server
//             if (assetItem == null)
//             {
//                 VKNotifyController.Instance.AddNotify("50_PopupContents/SYS_ASSET_MISSING", VKNotifyController.TypeNotify.Error);
//                 return;
//             }

//             // check co phai editor va bat simulator ko neu co thi tim luon prefab do trong project
// #if UNITY_EDITOR
//             if (SimulateAssetBundleInEditor)
//             {
//                 string scenePath = GetSceneFromLocal(assetItem.name, sceneName);
//                 VKDebug.Log(scenePath);
//                 SceneManager.LoadScene(scenePath);
//                 return;
//             }
// #endif

//             // tai tu duoi cache hoac tu server sau do lay ra prefab
//             StartCoroutine(DownloadAssetAndLoadScene(assetItem, sceneName, progressBar, isCache));
//         }

//         IEnumerator DownloadAssetAndLoadScene(VKAssetBundleItem assetItem, string sceneName, VKProgressBar progressBar, bool isCache)
//         {
//             string url = assetConfig.link + "/" + VKGameConfig.Instance.VersionGame() + "/" + VKGameConfig.Instance.PlatformDownloadAsset() + "/" + assetItem.name;
//             string urlDebug = "*/" + VKGameConfig.Instance.VersionGame() + "/" + VKGameConfig.Instance.PlatformDownloadAsset() + "/" + assetItem.name;
//             VKDebug.LogWarning("DOWNLOAD ASSET: " + url);

//             // check da download chua
//             // var currentAssetConfig = Database.Instance.ae dLocal.GetAssetItemByName(assetItem.name);

//             // Show UI download
//             if (progressBar != null)
//             {
//                 progressBar.gameObject.SetActive(true);
//                 progressBar.SetProgress(0f);
//             }

//             while (!Caching.ready)
//                 yield return null;

//             using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(url, (uint)assetItem.GetVersion(), 0))
//             {
//                 var operation = uwr.SendWebRequest();

//                 while (!operation.isDone)
//                 {
//                     // Show progress download
//                     if (progressBar != null)
//                     {
//                         progressBar.SetProgress(uwr.downloadProgress * 100);
//                     }

//                     yield return new WaitForEndOfFrame();
//                 }

//                 // Show download done
//                 if (progressBar != null)
//                 {
//                     progressBar.RunFakeProgress(0.5f);
//                 }

//                 if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
//                 {
//                     VKDebug.Log("Error while Downloading Data: " + uwr.error);
//                     LPopup.OpenPopup("ERROR", "Tải game bị lỗi xin thử lại sau!" + "\n" + urlDebug + "\n" + uwr.error);
//                     yield return null;
//                 }
//                 else
//                 {
//                     yield return new WaitForSeconds(0.6f);

//                     VKDebug.Log("Download Asset Success");

//                     AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(uwr);

//                     if (assetBundle == null)
//                     {
//                         VKDebug.Log("Scene được tải không đúng");
//                         LPopup.OpenPopup("ERROR", "Scene được tải không đúng! Xin vui lòng thử lại!");
//                     }
//                     else
//                     {
//                         // load game object
//                         assetMainGame = assetBundle;

//                         PlayerPrefs.SetInt(assetItem.name, assetItem.GetVersion());
//                         yield return new WaitForEndOfFrame();

//                         string[] scenePath = assetBundle.GetAllScenePaths();
//                         VKDebug.Log(scenePath[0]);

//                         AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scenePath[0], UnityEngine.SceneManagement.LoadSceneMode.Single);

//                         // Wait until the asynchronous scene fully loads
//                         while (!asyncLoad.isDone)
//                         {
//                             yield return null;
//                         }
//                     }
//                 }
//             }
//         }
        #endregion

        #region Simulator
#if UNITY_EDITOR
        static int m_SimulateAssetBundleInEditor = -1;
        const string kSimulateAssetBundles = "SimulateAssetBundles";
        // Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
        public static bool SimulateAssetBundleInEditor
        {
            get
            {
                if (m_SimulateAssetBundleInEditor == -1)
                    m_SimulateAssetBundleInEditor = EditorPrefs.GetBool(kSimulateAssetBundles, true) ? 1 : 0;

                return m_SimulateAssetBundleInEditor != 0;
            }
            set
            {
                int newValue = value ? 1 : 0;
                if (newValue != m_SimulateAssetBundleInEditor)
                {
                    m_SimulateAssetBundleInEditor = newValue;
                    EditorPrefs.SetBool(kSimulateAssetBundles, value);
                }
            }
        }

        private T GetAssetFromLocal<T>(string assetBundleName, string assetName) where T : UnityEngine.Object
        {
            string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
            if (assetPaths.Length == 0)
            {
                Debug.LogError("There is no asset with name \"" + assetName + "\" in " + assetBundleName);
                return null;
            }

            // @TODO: Now we only get the main object from the first asset. Should consider type also.
            return AssetDatabase.LoadAssetAtPath<T>(assetPaths[0]);
        }

        private string GetSceneFromLocal(string assetBundleName, string sceneName)
        {
            string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, sceneName);
            if (assetPaths.Length == 0)
            {
                Debug.LogError("There is no scene with name \"" + sceneName + "\" in " + assetBundleName);
                return null;
            }

            // @TODO: Now we only get the main object from the first asset. Should consider type also.
            return assetPaths[0];
        }
#endif
        #endregion
    }
}
