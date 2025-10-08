using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

#if UNITY_ANDROID || UNITY_IOS
#else
using System.IO;
//using SFB;
#endif

namespace VKSdk
{
    public class VKGalleryController : MonoBehaviour
    {
        public string folderName;

        #region Singleton
        private static VKGalleryController instance;

        public static VKGalleryController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<VKGalleryController>();
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

        public void SaveImageToGallery(Texture2D tex, string filename, string ext, Action<bool, string> callback)
        {
            if (tex != null)
            {
#if UNITY_ANDROID || UNITY_IOS
                // Save the screenshot to Gallery/Photos
#if VK_GALLERY
                NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(tex, folderName, filename + "." + ext,
                    (success, path) =>
                    {
                        // VKDebug.Log("Media save result: " + success + " " + path);
                        callback.Invoke(success, path);
                    });
#endif
                // VKDebug.Log("Permission result: " + permission);

                // To avoid memory leaks
                // Destroy(tex);
#else
                //string path = StandaloneFileBrowser.SaveFilePanel("Save File", "", filename, ext);
                //if (!string.IsNullOrEmpty(path))
                //{
                //    // VKDebug.Log("Save file to path: " + path);
                //    if (ext.ToLower().Equals("png"))
                //    {
                //        File.WriteAllBytes(path, tex.EncodeToPNG());
                //    }
                //    else
                //    {
                //        File.WriteAllBytes(path, tex.EncodeToJPG());
                //    }
                //    callback.Invoke(true, path);
                //}
                //else
                //{
                //    callback.Invoke(false, "");
                //    // VKDebug.Log("Donot path select");
                //}
#endif
            }
        }

        public void OpenImageFromGallery(string ext, int maxSize, Action<bool, Texture2D> callback)
        {
#if UNITY_ANDROID || UNITY_IOS
#if VK_GALLERY
            NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
            {
                // VKDebug.Log("Image path: " + path);
                if (path != null)
                {
                    // Create Texture from selected image
                    Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize, markTextureNonReadable: false);
                    if (texture == null)
                    {
                        // VKDebug.Log("Couldn't load texture from " + path);
                        callback.Invoke(false, null);
                        return;
                    }
                    callback.Invoke(true, texture);
                }
                else
                {
                    callback.Invoke(false, null);
                }
            });
#endif
#else
            //var paths = StandaloneFileBrowser.OpenFilePanel("Chọn ảnh", "", ext, false);

            //if (paths.Length > 0)
            //{
            //    StartCoroutine(IEWaitToGetImage(new System.Uri(paths[0]).AbsoluteUri, callback));
            //}
            //else
            //{
            //    callback.Invoke(false, null);
            //}
#endif
        }

        private IEnumerator IEWaitToGetImage(string url, Action<bool, Texture2D> callback)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();
            try
            {
                Texture2D tex = DownloadHandlerTexture.GetContent(www);
                if (tex != null)
                {
                    callback.Invoke(true, tex);
                }
            }
            catch
            {
                callback.Invoke(false, null);
            }
        }
    }
}
