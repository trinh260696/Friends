using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
public class GreenBoss : BossBase
{
    Spine.EventData eventData;
    public SkeletonMecanim skeletonMecanim; 
    private void Start()
    {
        NotificationCenter.DefaultCenter().AddObserver(this, "PopulatePos");
      
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
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
        if (!StaticData.IsPlay) return;
        if (killing) return;
        if (!StaticData.ISREADY) return;
        base.OnTriggerEnter2D(col);
        if (col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Hide"))
        {
            if (!Follow)
            {
                Target = col.transform;
                Follow = true;
                StopDetect();
            }
        }
    }
    private Transform desTransform;
    public override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!StaticData.IsPlay) return;
        if (killing) return;
        if (!StaticData.ISREADY) return;
        if (collision.collider.CompareTag("Wall"))
        {
            var dir = (collision.collider.transform.position - transform.position).normalized;
            Body.AddForce(dir);
        }
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Hide"))
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                collision.transform.position = transform.position;
                var player = collision.gameObject.GetComponent<Player>();                
                //player.DeathGreen();
                Kill(collision.transform);
            }
            else
            {
                StopCoroutine("TravelPoint");
                LeanTween.cancel(gameObject);
                animator.transform.localScale = Vector3.one * 0.7f;
                animator.SetTrigger(BossBase.BossKillProperties);
               // collision.gameObject.GetComponent<NPC>().DeathGreen(transform.position);
                desTransform = collision.gameObject.transform;
                          
                killing = true;
                Follow = false;
               
                
                var colliders = GetComponents<Collider2D>();
                for (int i = 0; i < colliders.Length; i++)
                    colliders[i].enabled = false;             
                Target = null;
               // Invoke("KillFriend", 0.1f);
                Invoke("ReturnGreen", 3f);
            }
           
        }
    }
    public override void Kill(Transform Destination)
    {
        animator.transform.localScale = Vector3.one * 0.7f;
        animator.SetTrigger(BossBase.BossKillProperties);
        killing = true;
        Follow = false;
        StopCoroutine("TravelPoint");
        LeanTween.cancel(gameObject);
        var colliders = GetComponents<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = false;
        Debug.LogWarning("Kill" + Destination.gameObject.name);
        Target = null;
        Invoke("ReturnGreen", 3f);       
        
    }
    void KillFriend()
    {
        animator.SetTrigger(BossBase.BossKillProperties);
    }
    void ReturnGreen()
    {
        BossReturn();
    }
    public override void OnTriggerStay2D(Collider2D col)
    {
        if (!StaticData.IsPlay) return;
        if (!StaticData.ISREADY) return;
        if (killing) return;
        base.OnTriggerStay2D(col);
        
    }
}

