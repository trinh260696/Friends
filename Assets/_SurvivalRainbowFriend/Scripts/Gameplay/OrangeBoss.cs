using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeBoss : BossBase
{
    [SerializeField] private Transform pivot1;
    [SerializeField] private Transform pivot2;
    Vector2 pivot;
    public ScreenOrientation orientation=ScreenOrientation.Portrait;
    Vector2 direction;
    private void OnEnable()
    {
        transform.position = (pivot1.position + pivot2.position) / 2;
        pivot = (Vector2)pivot1.position;
    }
    public override void FixedUpdate()
    {
        if (killing) return;
        
        if (Vector2.Distance(transform.position, (Vector2)pivot1.position) < 0.5f)
        {
            pivot = (Vector2)pivot2.position;
        }
        if(Vector2.Distance(transform.position,(Vector2)pivot2.position)<0.5f)
        {
            pivot = (Vector2)pivot1.position;
           
        }
        direction = (pivot - (Vector2)transform.position).normalized;
        Body.AddRelativeForce(direction * Speed);
        
    }
    public override void Update()
    {
        base.Update();
    }
    public override void OnInit()
    {
        base.OnInit();
    }
   
    public override void OnTriggerEnter2D(Collider2D col)
    {
        base.OnTriggerEnter2D(col);          
    }
    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
    }
    public override void Kill(Transform Destination)
    {
        base.Kill(Destination);
        ContainAssistant.Instance.GetItem("3D_Hit_06", Destination.transform.position + Vector3.up);
    }
}

