using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Transform baseTrans = transform.GetChild(0);
        foreach (Transform trans in transform)
        {
            Gizmos.DrawLine(baseTrans.position, trans.position);
            baseTrans = trans;
        }
    }
}
