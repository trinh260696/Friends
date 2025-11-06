using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform preTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 Dir=transform.position-preTransform.position;
        float angle = Vector2.SignedAngle(Dir, Vector2.up);
        Debug.LogWarning("angle:" + angle.ToString());
    }
}
