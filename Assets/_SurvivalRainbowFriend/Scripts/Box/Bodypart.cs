using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public bool Free = true;
    public int ID;
    // [SerializeField] TypeMission type;
    private SpriteRenderer spriteRender;
    private Collider2D col;
    private Transform m_parent;
  //  [HideInInspector]
  //  public TypeMission typeMission { get => type; set => type = value; }

    // Start is called before the first frame update
    void Start()
    {
       
    }
    public void InitBodyPart(int id, Sprite sprite,Transform Pr)
    {
        spriteRender = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        this.ID = id;
        this.Free = true;
        this.m_parent = Pr;
        spriteRender.sprite = sprite;
    }
    public void Hide()
    {
        Free = false;
        col.enabled = false;
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
        Free = true;
        col.enabled = true;
        transform.SetParent(m_parent);
    }
    
    public void DestroyNow()
    {
        Destroy(gameObject);
    }
}
