using UnityEngine;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Linq;

namespace VKSdk
{
    public static class Encryptor
    {
        const int keySize = 32;
        const string keyString = "O3T9HG5T5KVFuYE9";

        static readonly UTF8Encoding encoder;
        static readonly RijndaelManaged rijndael;

        static Encryptor()
        {
            encoder = new UTF8Encoding();
            rijndael = new RijndaelManaged { Key = encoder.GetBytes(keyString).Take(keySize).ToArray() };
            rijndael.BlockSize = 128; // only the 128-bit block size is specified in the AES standard.
            rijndael.Padding = PaddingMode.PKCS7;
            rijndael.Mode = CipherMode.CBC;
        }

        public static string Encrypt(string jsonData)
        {
            rijndael.GenerateIV();
            //rijndael.IV = new byte[rijndael.BlockSize/8];

            CryptData cryptData = new CryptData();
            cryptData.iv = Convert.ToBase64String(rijndael.IV);

            //		Debug.LogWarning("jsonData: " + jsonData);

            byte[] plainText = rijndael.CreateEncryptor().TransformFinalBlock(encoder.GetBytes(jsonData), 0, jsonData.Length);
            cryptData.data = Convert.ToBase64String(plainText);// .GetString(plainText);

            string jsonEncrypt = JsonUtility.ToJson(cryptData);

            //		Debug.LogWarning("jsonEncrypt: " + jsonEncrypt);
            return Convert.ToBase64String(encoder.GetBytes(jsonEncrypt));
        }

        public static string Decrypt(string jsonData)
        {
            byte[] byteDatas = Convert.FromBase64String(jsonData);
            string json = encoder.GetString(byteDatas);

            VKDebug.LogWarning(json);

            CryptData cryptData = JsonUtility.FromJson<CryptData>(json);

            rijndael.IV = Convert.FromBase64String(cryptData.iv);

            byte[] value = Convert.FromBase64String(cryptData.data);
            byte[] plainText = rijndael.CreateDecryptor().TransformFinalBlock(value, 0, value.Length);

            return encoder.GetString(plainText);
        }
    }

    [System.Serializable]
    public class CryptData
    {
        public string iv;
        public string data;
    }
}