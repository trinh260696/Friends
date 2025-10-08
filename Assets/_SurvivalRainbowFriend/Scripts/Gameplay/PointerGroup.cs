using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerGroup : MonoBehaviour
{
    public Color32 color;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnDrawGizmos()
    {
#if Edit_Map
        Gizmos.color = color;
        for (int i = 0; i < transform.childCount; i++)
        {
            for (int j = 1; j < transform.childCount; j++)
            {
                float distance = Vector2.Distance((Vector2)transform.GetChild(i).position, (Vector2)transform.GetChild(j).position);
                var raycastHit = Physics2D.Raycast((Vector2)transform.GetChild(i).position, (Vector2)transform.GetChild(j).position - (Vector2)transform.GetChild(i).position, distance, 1<<13|1<<1);
                if (raycastHit.collider == null)
                {
                    Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(j).position);
                }
            }
        }
#endif

    }
}
