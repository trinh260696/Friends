using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenRotate : MonoBehaviour
{
    private float rotateSpeed = 150f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }
}
