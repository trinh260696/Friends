using UnityEngine;

public class ItemBase : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public virtual void UseItem()
    {
        Debug.LogWarning("Using item: " + gameObject.name);
    }
}
