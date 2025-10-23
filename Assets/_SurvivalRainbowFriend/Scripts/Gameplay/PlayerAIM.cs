using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
    void Update()
    {
#if UNITY_WEBGL || UNITY_WEBGL_API
        OnDesktop();
#else
        OnMobile();
#endif
    }
    void OnMobile()
    {
        // Kiểm tra nếu có ít nhất 1 chạm
        if (Input.touchCount > 0)
        {
            UnityEngine.Touch touch = Input.GetTouch(0);

            // Chỉ xử lý khi bắt đầu chạm (TouchPhase.Began)
            if (touch.phase == TouchPhase.Began)
            {
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

                // Raycast 2D vào vị trí chạm
                RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero, Mathf.Infinity, 1 << 17);

                if (hit.collider != null)
                {
                    Debug.Log("Chạm vào: " + hit.collider.name);

                    var slot = hit.collider.gameObject.GetComponent<Slot>();
                    if (slot != null)
                    {
                        // Nếu cần kiểm tra đối tượng cụ thể:
                        FieldAssistant.main.HandleSlotClick(slot);
                    }

                }
            }
        }
    }
    void OnDesktop()
    {
        // Kiểm tra nếu có ít nhất 1 chạm
        if (Input.GetMouseButtonDown(0))
        {

            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Raycast 2D vào vị trí chạm
            RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero, Mathf.Infinity, 1 << 17);

            if (hit.collider != null)
            {
                Debug.Log("Chạm vào: " + hit.collider.name);

                var slot = hit.collider.gameObject.GetComponent<Slot>();
                if (slot != null)
                {
                    // Nếu cần kiểm tra đối tượng cụ thể:
                    FieldAssistant.main.HandleSlotClick(slot);
                }

            }
        }
    }
}


