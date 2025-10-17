using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class ContentAssistant : MonoBehaviour
    {

        public static ContentAssistant main;

        public List<ContentAssistantItem> cItems;
        public List<Material> materials;

        private Dictionary<string, GameObject> content = new Dictionary<string, GameObject>();

        private Transform front;
        private Transform quarantine;

        private GameObject zObj;

        void Awake()
        {
            main = this;
            content.Clear();
            foreach (ContentAssistantItem item in cItems)
                content.Add(item.item.name, item.item);
           
        }
        public void ClearAsset()
        {
            StartCoroutine(UnloadUnusedAssetsAsync());
        }
        public T GetItem<T>(string key) where T : Component
        {
            return Instantiate(content[key]).GetComponent<T>();
        }

        public GameObject GetItem(string key)
        {
            return Instantiate(content[key]);
        }

        public T GetItem<T>(string key, Vector3 position) where T : Component
        {
            zObj = GetItem(key);
            zObj.transform.position = position;
            return zObj.GetComponent<T>();
        }

        public GameObject GetItem(string key, Vector3 position)
        {
            zObj = GetItem(key);
            zObj.transform.position = position;
            return zObj;
        }

        public GameObject GetItem(string key, Vector3 position, Quaternion rotation)
        {
            zObj = GetItem(key, position);
            zObj.transform.rotation = rotation;
            return zObj;
        }

        
        public Material GetMaterial(int id)
        {
            if(id>materials.Count) return materials[0];
            return materials[id];
        }
        IEnumerator UnloadUnusedAssetsAsync()
        {
            yield return Resources.UnloadUnusedAssets();
            
        }
        void PreloadTextures(string[] texturePaths)
        {
            materials = new List<Material>();
            foreach (string path in texturePaths)
            {
                Material material = Resources.Load<Material>(path);
                if (material != null)
                {
                    materials.Add(material);
                }
            }
        }
        [System.Serializable]
        public struct ContentAssistantItem
        {
            public GameObject item;
            public string category;
        }
    }
