using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAIM : MonoBehaviour
{
    public Transform cCamera;//main camera
    public static float CamZoom = 6;//Zoom
     
    void Awake()
    {
    // Marker.SetActive(false);//marker deactivation
        cCamera = Camera.main.transform;
        Camera.main.orthographicSize = 8;
    }
    private void OnEnable()
    {
        cCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -CamZoom);//moving camera to player pos 
    }
    void LateUpdate()
    {
        if (!StaticData.IsPlay) return;
        //  if (Marker.activeInHierarchy) Marker.transform.position = nearest.transform.position;//moving marker to target pos
        cCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -CamZoom);//moving camera to player pos 
        

    }
}


