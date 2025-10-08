using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScript : MonoBehaviour
{
    public bool IsAlive = true;
    [SerializeField] TypeMission type;
    private SpriteRenderer spriteRender;
    [HideInInspector]
    public TypeMission typeMission { get => type; set => type = value; }

    // Start is called before the first frame update
    void Start()
    {
        spriteRender = GetComponent<SpriteRenderer>();
    }
    public void Hide()
    {
        //IsAlive = false;
      //  spriteRender.enabled = false;
        gameObject.SetActive(false);
    }
    public void Inactive()
    {
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReActive()
    {
        IsAlive = true;
        spriteRender.enabled = true;
    }
}
