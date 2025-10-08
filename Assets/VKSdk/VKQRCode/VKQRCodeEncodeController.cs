#if VK_QRCODE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TBEasyWebCam;
using ZXing.Common;
using ZXing;
using ZXing.QrCode.Internal;
using System.Text.RegularExpressions;

namespace VKSdk.QRCode
{
    public class VKQRCodeEncodeController : MonoBehaviour
    {
        public enum CodeMode
        {
            QR_CODE,
            CODE_39,
            CODE_128,
            EAN_8,
            EAN_13,
            //DATA_MATRIX,
            NONE
        }

        public CodeMode eCodeFormat = CodeMode.QR_CODE;

        public int qrCodeWidth = 512;
        public int qrCodeHeight = 512;

        public Texture2D qrLogoTex;
        public float embedLogoRatio = 0.2f;

        // private
        private Texture2D _texEncoded;
        Texture2D tempLogoTex = null;
        BitMatrix byteMatrix;

        // callback
        public delegate void QREncodeFinished(Texture2D tex);
        public event QREncodeFinished onQREncodeFinished;

        // unity method
        void Start()
        {
            int targetWidth = Mathf.Min(qrCodeWidth, qrCodeHeight);
            targetWidth = Mathf.Clamp(targetWidth, 128, 1024);
            qrCodeWidth = qrCodeHeight = targetWidth;
        }

        /// <summary>
        /// Encode the specified string .
        /// </summary>
        /// <param name="valueStr"> content string.</param>
        public int Encode(string valueStr)
        {
            //	var writer = new QRCodeWriter();
            var writer = new MultiFormatWriter();
            Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();
            //set the code type
            hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);

            switch (eCodeFormat)
            {
                case CodeMode.QR_CODE:
                    byteMatrix = writer.encode(valueStr, BarcodeFormat.QR_CODE, qrCodeWidth, qrCodeHeight, hints);
                    break;
                case CodeMode.EAN_13:
                    if ((valueStr.Length == 12 || valueStr.Length == 13) && bAllDigit(valueStr))
                    {
                        if (valueStr.Length == 13)
                        {
                            valueStr = valueStr.Substring(0, 12);
                        }
                        byteMatrix = writer.encode(valueStr, BarcodeFormat.EAN_13, qrCodeWidth, qrCodeWidth / 2, hints);
                    }
                    else
                    {

                        return -13;
                    }
                    break;
                case CodeMode.EAN_8:
                    if ((valueStr.Length == 7 || valueStr.Length == 8) && bAllDigit(valueStr))
                    {
                        if (valueStr.Length == 8)
                        {
                            valueStr = valueStr.Substring(0, 7);
                        }
                        byteMatrix = writer.encode(valueStr, BarcodeFormat.EAN_8, qrCodeWidth, qrCodeWidth / 2, hints);
                    }
                    else
                    {
                        return -8;
                    }
                    break;
                case CodeMode.CODE_128:
                    if (IsNumAndEnCh(valueStr) && valueStr.Length <= 80)
                    {
                        byteMatrix = writer.encode(valueStr, BarcodeFormat.CODE_128, qrCodeWidth, qrCodeWidth / 2, hints);
                    }
                    else
                    {
                        return -128;
                    }
                    break;
                case CodeMode.CODE_39:
                    if (bAllDigit(valueStr))
                    {
                        byteMatrix = writer.encode(valueStr, BarcodeFormat.CODE_39, qrCodeWidth, qrCodeHeight / 2, hints);
                    }
                    else
                    {
                        return -39;
                    }

                    break;

                case CodeMode.NONE:
                    return -1;
            }

            if (_texEncoded != null)
            {
                Destroy(_texEncoded);
                _texEncoded = null;
            }
            _texEncoded = new Texture2D(byteMatrix.Width, byteMatrix.Height);

            for (int i = 0; i != _texEncoded.width; i++)
            {
                for (int j = 0; j != _texEncoded.height; j++)
                {
                    if (byteMatrix[i, j])
                    {
                        _texEncoded.SetPixel(i, j, Color.black);
                    }
                    else
                    {
                        _texEncoded.SetPixel(i, j, Color.white);
                    }
                }
            }

            ///rotation the image 
            Color32[] pixels = _texEncoded.GetPixels32();
            //pixels = RotateMatrixByClockwise(pixels, _texEncoded.width);
            _texEncoded.SetPixels32(pixels);

            _texEncoded.Apply();


            if (eCodeFormat == CodeMode.QR_CODE)
            {
                AddLogoToQRCode();
            }

            onQREncodeFinished(_texEncoded);
            return 0;
        }


        /// <summary>
        /// Rotates the matrix.Clockwise
        /// </summary>
        /// <returns>The matrix.</returns>
        /// <param name="matrix">Matrix.</param>
        /// <param name="n">N.</param>
        static Color32[] RotateMatrixByClockwise(Color32[] matrix, int n)
        {
            Color32[] ret = new Color32[n * n];
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    ret[i * n + j] = matrix[(n - i - 1) * n + j];
                }
            }
            return ret;
        }

        /// <summary>
        /// anticlockwise
        /// </summary>
        /// <returns>The matrix.</returns>
        /// <param name="matrix">Matrix.</param>
        /// <param name="n">N.</param>
        static Color32[] RotateMatrixByAnticlockwise(Color32[] matrix, int n)
        {
            Color32[] ret = new Color32[n * n];

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    ret[i * n + j] = matrix[(n - j - 1) * n + i];
                }
            }
            return ret;
        }


        bool isContainDigit(string str)
        {
            for (int i = 0; i != str.Length; i++)
            {
                if (str[i] >= '0' && str[i] <= '9')
                {
                    return true;
                }
            }
            return false;
        }

        bool IsNumAndEnCh(string input)
        {
            string pattern = @"^[A-Za-z0-9-_!@# |+/*]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }


        bool isContainChar(string str)
        {
            for (int i = 0; i != str.Length; i++)
            {
                if (str[i] >= 'a' && str[i] <= 'z')
                {
                    return true;
                }
            }
            return false;
        }

        bool bAllDigit(string str)
        {
            for (int i = 0; i != str.Length; i++)
            {
                if (str[i] >= '0' && str[i] <= '9')
                {
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public void AddLogoToQRCode()
        {
            if (qrLogoTex != null)
            {
                int maxLength = Mathf.Max(qrLogoTex.width, qrLogoTex.height);
                if (maxLength > (_texEncoded.width * embedLogoRatio))
                {

                    if (tempLogoTex == null)
                    {
                        tempLogoTex = new Texture2D(qrLogoTex.width, qrLogoTex.height, TextureFormat.RGBA32, true);
                        tempLogoTex.SetPixels(qrLogoTex.GetPixels());
                        tempLogoTex.Apply();
                    }

                    float scaleRatio = _texEncoded.width * embedLogoRatio / maxLength * 1.0f;
                    int newLogoWidth = (int)(qrLogoTex.width * scaleRatio);
                    int newLogoHeight = (int)(qrLogoTex.height * scaleRatio);
                    TextureScale.Bilinear(tempLogoTex, newLogoWidth, newLogoHeight);
                }
                else
                {
                    if (tempLogoTex == null)
                    {
                        tempLogoTex = new Texture2D(qrLogoTex.width, qrLogoTex.height, TextureFormat.RGBA32, true);
                        tempLogoTex.SetPixels(qrLogoTex.GetPixels());
                        tempLogoTex.Apply();
                    }
                }

            }
            else
            {
                return;
            }

            int startX = (_texEncoded.width - tempLogoTex.width) / 2;
            int startY = (_texEncoded.height - tempLogoTex.height) / 2;

            for (int x = startX; x < tempLogoTex.width + startX; x++)
            {
                for (int y = startY; y < tempLogoTex.height + startY; y++)
                {
                    Color bgColor = _texEncoded.GetPixel(x, y);
                    Color wmColor = tempLogoTex.GetPixel(x - startX, y - startY);
                    Color finalColor = Color.Lerp(bgColor, wmColor, wmColor.a / 1.0f);
                    _texEncoded.SetPixel(x, y, finalColor);
                }
            }

            Destroy(tempLogoTex);
            tempLogoTex = null;

            _texEncoded.Apply();
        }
    }
}
#endif