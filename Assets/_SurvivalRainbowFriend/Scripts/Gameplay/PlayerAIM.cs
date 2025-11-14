using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAIM : MonoBehaviour
{
    public Transform cCamera;//main camera
    public static float CamZoom = 6;//Zoom
    
    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.15f; // Điều chỉnh giá trị này để thay đổi độ mịn
    private Player player;
    void Awake()
    {
    // Marker.SetActive(false);//marker deactivation
        cCamera = Camera.main.transform;
        Camera.main.orthographicSize = 8;
        player = GetComponent<Player>();
    }
    private void OnEnable()
    {
        cCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -CamZoom);//moving camera to player pos 
    }
    void LateUpdate()
    {
        if (player.playerNPC.isAttacking)
        {
            return;
        }
        if (!StaticData.IsPlay) return;
        //  if (Marker.activeInHierarchy) Marker.transform.position = nearest.transform.position;//moving marker to target pos
        
        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, -CamZoom);
        cCamera.transform.position = Vector3.SmoothDamp(cCamera.transform.position, targetPosition, ref velocity, smoothTime);
    }
    void Update()
    {     
#if UNITY_EDITOR
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
                    Debug.LogWarning("Chạm vào: " + hit.collider.name);

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
               

                var slot = hit.collider.gameObject.GetComponent<Slot>();
                if (slot != null)
                {
                    // Nếu cần kiểm tra đối tượng cụ thể:
                    FieldAssistant.main.HandleSlotClick(slot);
                }

            }
            RaycastHit2D[] hits = Physics2D.RaycastAll(touchPos, Vector2.zero, Mathf.Infinity, 1 << 9);
            if(hits.Length == 0) return;
            var list=hits.Where(hits => hits.collider != null).OrderBy(h => Vector2.Distance(touchPos, h.collider.transform.position)).ToList();
            var hit2 = list[0];
            if (hit2.collider != null)
            {
                Debug.LogWarning("Chạm vào: " + hit2.collider.name);
                var enemy = hit2.collider.gameObject.GetComponent<BossBase>();
                player.playerNPC.MoveToAttack(enemy);
                
            }
        }
    }
}


