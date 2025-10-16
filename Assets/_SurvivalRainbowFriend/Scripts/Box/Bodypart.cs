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
  //  [HideInInspector]
  //  public TypeMission typeMission { get => type; set => type = value; }

    // Start is called before the first frame update
    void Start()
    {
        spriteRender = GetComponent<SpriteRenderer>();
    }
    public void InitBodyPart(int id, Sprite sprite)
    {
        this.ID = id;
        this.Free = true;      
        spriteRender.sprite = sprite;
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
        Free = true;
        spriteRender.enabled = true;
    }
}
