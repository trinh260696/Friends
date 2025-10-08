using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLocalData : MonoBehaviour
{
    public static LevelLocalData Instance;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
