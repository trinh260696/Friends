using System;
using System.IO;
using UnityEngine;

namespace VKSdk
{
    public class FileHelper
    {
        public static byte[] LoadBinaryFromFile(string fileName)
        {
            VKDebug.Log("Load Binary File From : " + fileName);
            try
            {
                FileStream file = File.OpenRead(GetPath(fileName));
                BinaryReader rd = new BinaryReader(file);
                byte[] data = rd.ReadBytes((int)file.Length);
                rd.Close();
                file.Close();
                return data;
            }
            catch (Exception ex)
            {
                VKDebug.Log("Error when read binary file from " + GetPath(fileName) + " with exception : " + ex.Message);
                return null;
            }
        }

        public static string LoadTextFromFile(string fileName)
        {
            VKDebug.Log("Load Text File From : " + fileName);
            try
            {
                FileStream file = File.OpenRead(GetPath(fileName));
                StreamReader rd = new StreamReader(file);
                string data = rd.ReadToEnd();
                rd.Close();
                file.Close();
                return data;
            }
            catch (Exception ex)
            {
                VKDebug.Log("Error when read text file from " + GetPath(fileName) + " with exception : " + ex.Message);
                return null;
            }
        }

        public static void WriteBinaryToFile(string fileName, byte[] newData)
        {
            VKDebug.Log("Write Binary File To : " + fileName);
            FileStream file = File.Create(GetPath(fileName));
            BinaryWriter bw = new BinaryWriter(file);
            bw.Write(newData);
            bw.Close();
            file.Close();
        }

        public static void WriteBinaryToPath(string path, byte[] newData)
        {
            //Create the Directory if it does not exist
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            try
            {
                File.WriteAllBytes(path, newData);
                Debug.Log("Saved Data to: " + path.Replace("/", "\\"));
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed To Save Data to: " + path.Replace("/", "\\"));
                Debug.LogWarning("Error: " + e.Message);
            }
        }

        public static void WriteTextToFile(string fileName, string newData)
        {
            //        Debug.Log("Write Text File To : " + fileName);
            FileStream file = File.Create(GetPath(fileName));
            StreamWriter bw = new StreamWriter(file);
            bw.Write(newData);
            bw.Close();
            file.Close();
        }

        public static void DeleteFile(string fileName)
        {
            VKDebug.Log("Delete File : " + fileName);
            File.Delete(GetPath(fileName));
        }

        public static void DeleteDirection(string path)
        {
            Directory.Delete(path, true);
        }

        public static bool CheckDirectionExists(string path)
        {
            return Directory.Exists(Path.GetDirectoryName(path));
        }

        public static bool CheckFileExists(string fileName)
        {
            return File.Exists(GetPath(fileName));
        }

        public static string GetPath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }

        public static void SaveTextureToFile(Texture2D texture, string filename)
        {
            string path = GetPath(filename);

            ////Create the Directory if it does not exist
            //if (!Directory.Exists(Path.GetDirectoryName(path)))
            //{
            //    Directory.CreateDirectory(Path.GetDirectoryName(path));
            //}

            try
            {
                if (filename.Contains(".png"))
                    File.WriteAllBytes(path, texture.EncodeToPNG());
                else
                    File.WriteAllBytes(path, texture.EncodeToJPG());

                VKDebug.Log("Saved Data to: " + path.Replace("/", "\\"));
            }
            catch (Exception e)
            {
                VKDebug.LogWarning("Failed To Save Data to: " + path.Replace("/", "\\"));
                VKDebug.LogWarning("Error: " + e.Message);
            }
        }

        public static void CheckAndCreateDirection(string path)
        {
            //Create the Directory if it does not exist
            if (!CheckDirectionExists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
        }


        public static Texture2D LoadTexture(string filename, bool isResize = false)
        {
            Texture2D tex = null;
            byte[] fileData;

            string path = GetPath(filename);
            if (File.Exists(path))
            {
                fileData = File.ReadAllBytes(path);
// #if UNITY_WEBGL
//                 try
//                 {
//                     string filenamelower = filename.ToLower();
//                     if(filenamelower.EndsWith(".jpg") || filenamelower.EndsWith(".jpeg"))
//                     {
//                         tex = new Texture2D(1, 1, TextureFormat.RGB24, false);
//                     }
//                     else
//                     {
//                         tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
//                     }
//                 }
//                 catch
//                 {
//                     tex = new Texture2D(1, 1);
//                 }
// #else
                if(isResize) tex = new Texture2D(2, 2);
                else tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
// #endif
                tex.LoadImage(fileData);
            }
            return tex;
        }

    }

}