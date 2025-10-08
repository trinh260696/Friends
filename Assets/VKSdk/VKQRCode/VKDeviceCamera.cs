#if VK_QRCODE
using UnityEngine;
using System.Collections;
using TBEasyWebCam;

namespace VKSdk.QRCode
{
    public class VKDeviceCamera : MonoBehaviour
    {
        [Space(10)]
        public CameraPlaneController cameraPlaneController;

        public GameObject goCameraPlane;
        public Renderer camPlaneRender;
        public Texture texDefault;

        public bool isUseEasyWebCam = true;

        // private
        private bool isCorrected = false;
        private float screenVideoRatio = 1.0f;

        private int hasCamera = -1;

#if UNITY_IOS
        
#endif

        private DeviceCamera webcam;
        public DeviceCamera dWebCam
        {
            get
            {
                return webcam;
            }
        }

        public bool isPlaying
        {
            get
            {
                if (webcam != null)
                {
                    return webcam.isPlaying();
                }
                else
                {
                    return false;
                }
            }
        }

        public delegate void DeviceCameraPermissionFinished(bool allow);  
        public event DeviceCameraPermissionFinished onDeviceCameraPermissionFinished;

        // unity method
        void Start()
        {
            hasCamera = -1;
            webcam = new DeviceCamera(isUseEasyWebCam);
            // StartWork ();
        }

        void Update()
        {
            if (webcam != null && webcam.isPlaying())
            {
                if (webcam.Width() > 200 && !isCorrected)
                {
                    CorrectScreenRatio();
                }

                if (goCameraPlane.activeSelf)
                {
                    camPlaneRender.material.mainTexture = webcam.preview;
                }
            }
        }

        public bool HasDeviceCamera()
        {
            if(hasCamera != 1)
            {
                hasCamera = WebCamTexture.devices.Length > 0 ? 1 : 0;
            }
            return hasCamera == 1;
        }

        // method
        /// <summary>
        /// start the work.
        /// </summary>
        public void StartWork()
        {
#if UNITY_ANDROID
            //check and request permistion
            if (!EasyWebCam.checkPermissions())
            {
                RequestCameraPermissions();
                return;
            }
#elif UNITY_IOS
            if(!HasDeviceCamera())
            {
                StartCoroutine(IEWaitTimeToOpenCamera());
                return;
            }
#endif

            if (this.webcam != null)
            {
                this.webcam.Play();
            }
        }

        /// <summary>
        /// Stops the work.
        /// when you need to leave current scene ,you must call this func firstly
        /// </summary>
        public void StopWork()
        {
            if (this.webcam != null && this.webcam.isPlaying())
            {
                this.webcam.Stop();
            }
            if (goCameraPlane.activeSelf)
            {
                camPlaneRender.material.mainTexture = texDefault;
            }
        }

        /// <summary>
        /// Corrects the screen ratio.
        /// </summary>
        void CorrectScreenRatio()
        {
            int videoWidth = 640;
            int videoHeight = 480;
            int ScreenWidth = 640;
            int ScreenHeight = 480;

            float videoRatio = 1;
            float screenRatio = 1;

            if (this.webcam != null)
            {
                videoWidth = webcam.Width();
                videoHeight = webcam.Height();
            }

            videoRatio = videoWidth * 1.0f / videoHeight;
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            ScreenWidth = Mathf.Max (Screen.width, Screen.height);
		    ScreenHeight = Mathf.Min (Screen.width, Screen.height);
#else
            ScreenWidth = Screen.width;
            ScreenHeight = Screen.height;
#endif
            screenRatio = ScreenWidth * 1.0f / ScreenHeight;

            screenVideoRatio = screenRatio / videoRatio;
            isCorrected = true;

            if (goCameraPlane != null)
            {
                cameraPlaneController.correctPlaneScale(screenVideoRatio);
            }
        }

#if UNITY_ANDROID
        void RequestCameraPermissions()
        {
            CameraPermissionsController.RequestPermission(new[] { EasyWebCam.CAMERA_PERMISSION }, new AndroidPermissionCallback(
                grantedPermission =>
                {
                    StartCoroutine(IEWaitTimeToOpenCamera());
                    // The permission was successfully granted, restart the change avatar routine
                },
                deniedPermission =>
                {
                    if(onDeviceCameraPermissionFinished != null)
                    {
                        onDeviceCameraPermissionFinished.Invoke(false);
                    }
                    // The permission was denied
                },
                deniedPermissionAndDontAskAgain =>
                {
                    if(onDeviceCameraPermissionFinished != null)
                    {
                        onDeviceCameraPermissionFinished.Invoke(false);
                    }
                    // The permission was denied, and the user has selected "Don't ask again"
                    // Show in-game pop-up message stating that the user can change permissions in Android Application Settings
                    // if he changes his mind (also required by Google Featuring program)
                }));
        }

        IEnumerator IEWaitTimeToOpenCamera()
        {
            yield return new WaitForEndOfFrame();// (0.1f);

            hasCamera = 1;
            if(onDeviceCameraPermissionFinished != null)
            {
                onDeviceCameraPermissionFinished.Invoke(true);
            }
            StartWork();
        }
#elif UNITY_IOS
        IEnumerator IEWaitTimeToOpenCamera()
        {
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                hasCamera = 1;
                if(onDeviceCameraPermissionFinished != null)
                {
                    onDeviceCameraPermissionFinished.Invoke(true);
                }
                StartWork();
            }
            else
            {
                hasCamera = 0;
                if(onDeviceCameraPermissionFinished != null)
                {
                    onDeviceCameraPermissionFinished.Invoke(false);
                }
            }

            // yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
            // if (Application.HasUserAuthorization(UserAuthorization.Microphone))
            // {
            //     Debug.Log("Microphone found");
            // }
            // else
            // {
            //     Debug.Log("Microphone not found");
            // }
        }
#endif

        
    }
}
#endif