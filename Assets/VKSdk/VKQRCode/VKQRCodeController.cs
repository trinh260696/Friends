#if VK_QRCODE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBEasyWebCam;
using UnityEngine.UI;
using VKSdk.UI;

namespace VKSdk.QRCode
{
    public class VKQRCodeController : MonoBehaviour
    {
        public VKQRCodeDecodeController vkQRCodeDecode;

        public Canvas uiCanvas;
        public CanvasScaler uiCanvasScaler;

        public GameObject goTryAgain;

        public void BtCloseClickListener()
        {
            VKAudioController.Instance.PlaySound("buttonclick_1");
        }

        public void BtTryAgainClickListener()
        {
            VKAudioController.Instance.PlaySound("buttonclick_1");
        }

        public void BtMyQRCodeClickListener()
        {
            VKLayerController.Instance.ShowLayer("LQRCode");
            
            VKAudioController.Instance.PlaySound("buttonclick_1");
        }

        public void BtReadQRCodeClickListener()
        {
            VKAudioController.Instance.PlaySound("buttonclick_1");
        }


    }
}
#endif