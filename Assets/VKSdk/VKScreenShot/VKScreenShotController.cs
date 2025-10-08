using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VKSdk
{

    public class VKScreenShotController : MonoBehaviour
    {
        public Camera camTakePicture;
        //public Camera camNewChar;

        private static VKScreenShotController instance;
        public static VKScreenShotController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<VKScreenShotController>();
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
        }

        //public Texture2D TakePicture(Vector3 pos)
        //{
        //    camTakePicture.transform.position = new Vector3(pos.x, pos.y + 45f, -20f);

        //    return ScaleTexture(TakePicture(camScore, 400, 400), 640, 640);
        //}

        //public Texture2D TakePictureNewChar()
        //{
        //    return ScaleTexture(TakePicture(camNewChar, 400, 500), 640, 800);
        //}

        //public Texture2D TakePicture(Camera cam, int sizeWidth, int sizeHeight)
        //{
        //    cam.gameObject.SetActive(true);

        //    int resWidth = cam.pixelWidth;
        //    int resHeight = cam.pixelHeight;

        //    RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);

        //    cam.targetTexture = rt;
        //    Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        //    cam.Render();
        //    RenderTexture.active = rt;
        //    screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        //    cam.targetTexture = null;
        //    RenderTexture.active = null;
        //    DestroyImmediate(rt);

        //    // create image
        //    Texture2D cropTexture;
        //    if (resWidth < resHeight)
        //    {
        //        float per = ((float)sizeHeight / sizeWidth);
        //        int newResHeight = (int)(resWidth * per);

        //        int crop = (resHeight - newResHeight) / 2;
        //        Color[] c = screenShot.GetPixels(0, crop, resWidth, newResHeight);

        //        cropTexture = new Texture2D(resWidth, newResHeight);
        //        cropTexture.SetPixels(c);
        //        cropTexture.Apply();
        //    }
        //    else
        //    {
        //        float per = ((float)sizeWidth / sizeHeight);
        //        int newResWidth = (int)(resHeight * per);

        //        int crop = (resWidth - newResWidth) / 2;
        //        Color[] c = screenShot.GetPixels(crop, 0, newResWidth, resHeight);

        //        cropTexture = new Texture2D(newResWidth, resHeight);
        //        cropTexture.SetPixels(c);
        //        cropTexture.Apply();
        //    }

        //    cam.gameObject.SetActive(false);

        //    return cropTexture;
        //}
        public Texture2D TakePicture(Camera cam)
        {
            int resWidth = cam.pixelWidth;
            int resHeight = cam.pixelHeight;

            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);

            cam.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            cam.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            screenShot.Apply();
            cam.targetTexture = null;
            RenderTexture.active = null;
            DestroyImmediate(rt);

            return screenShot;
        }

        public Texture2D TakePicture(Camera cam, Vector2 vStart, Vector2 vSize)
        {
            // create rendertexture by camera with real screensize
            int resWidth = cam.pixelWidth;
            int resHeight = cam.pixelHeight;

            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);

            cam.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            cam.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            cam.targetTexture = null;
            RenderTexture.active = null;
            DestroyImmediate(rt);

            // create image like default screensize
            Texture2D cropTexture;
            Color[] c = screenShot.GetPixels((int)vStart.x, (int)vStart.y, (int)vSize.x, (int)vSize.y);

            cropTexture = new Texture2D((int)vSize.x, (int)vSize.y);
            cropTexture.SetPixels(c);
            cropTexture.Apply();

            return cropTexture;
        }

        public Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
            float incX = (1.0f / (float)targetWidth);
            float incY = (1.0f / (float)targetHeight);
            for (int i = 0; i < result.height; ++i)
            {
                for (int j = 0; j < result.width; ++j)
                {
                    Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                    result.SetPixel(j, i, newColor);
                }
            }
            result.Apply();
            return result;
        }
    }
}