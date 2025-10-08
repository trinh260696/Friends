using System;
using System.Collections.Generic;
using UnityEngine;

namespace VKSdk
{
    public class VKObjectPoolManager : MonoBehaviour
    {
        public GameObject prefab;
        public List<VKObjectPoolItem> pool;
        public Vector3 hidePosition;
        public int amountFirst;
        private int count = 1;
        
        // Use this for initialization
        void Start()
        {
            if (pool == null)
            {
                pool = new List<VKObjectPoolItem>();
            }
            else if (pool.Count < amountFirst)
            {
                for (int i = pool.Count; i < amountFirst; i++)
                {
                    VKObjectPoolItem nOIP = CreateObject();
                    nOIP.gameObject.SetActive(false);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public GameObject BorrowObject()
        {
            foreach (VKObjectPoolItem obj in pool)
            {
                if (!obj.isUsing)
                {
                    // obj.gameObject.SetActive(true);
                    obj.isUsing = true;
                    return obj.gameObject;
                }
            }
            VKObjectPoolItem nOIP = CreateObject();
            // nOIP.gameObject.SetActive(true);
            nOIP.isUsing = true;
            return nOIP.gameObject;
        }

        public T BorrowObject<T>(Transform parent = null)
        {
            foreach (VKObjectPoolItem obj in pool)
            {
                if (!obj.isUsing)
                {
                    // obj.gameObject.SetActive(true);
                    obj.isUsing = true;

                    if(parent != null) VKCommon.SetParent(obj.transform, parent);
                    return obj.gameObject.GetComponent<T>();
                }
            }

            VKObjectPoolItem nOIP = CreateObject();
            // nOIP.gameObject.SetActive(true);
            nOIP.isUsing = true;
            if(parent != null) VKCommon.SetParent(nOIP.transform, parent);
            return nOIP.gameObject.GetComponent<T>();
        }

        public Component BorrowObject(Type type)
        {
            foreach (VKObjectPoolItem obj in pool)
            {
                if (!obj.isUsing)
                {
                    // obj.gameObject.SetActive(true);
                    obj.isUsing = true;
                    return obj.gameObject.GetComponent(type);
                }
            }
            VKObjectPoolItem nOIP = CreateObject();
            // nOIP.gameObject.SetActive(true);
            nOIP.isUsing = true;
            return nOIP.gameObject.GetComponent(type);
        }

        public void GiveBackObject(GameObject obj)
        {
            // obj.SendMessage("PoolGiveBackObject", SendMessageOptions.DontRequireReceiver);
            var poolItem = obj.GetComponent<VKObjectPoolItem>();
            poolItem.GiveBack();
            
            obj.SetActive(false);

            obj.transform.SetParent(this.transform, true);
            obj.transform.localPosition = hidePosition;
            obj.transform.eulerAngles = Vector3.zero;
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        private VKObjectPoolItem CreateObject()
        {
            GameObject nObj = (GameObject)Instantiate(prefab, hidePosition, prefab.transform.rotation);
            nObj.name = nObj.name + "" + count;
            count++;
            nObj.transform.SetParent(this.transform);
            nObj.transform.localPosition = hidePosition;
            nObj.transform.localScale = new Vector3(1f, 1f, 1f);
            VKObjectPoolItem nOIP = nObj.GetComponent<VKObjectPoolItem>();
            nOIP.isUsing = false;
            pool.Add(nOIP);
            return nOIP;
        }

        public void GiveBackAll()
        {
            foreach (VKObjectPoolItem obj in pool)
            {
                if (obj.isUsing) GiveBackObject(obj.gameObject);
            }
        }
    }
}