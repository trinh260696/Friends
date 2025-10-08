using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamFaceBoss : BossBase
{
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
   
    public override void OnTriggerEnter2D(Collider2D col)
    {
    }
    public override void OnInit()
    {
        base.OnInit();
    }
    public override void OnTriggerStay2D(Collider2D col)
    {
       
    }
    public override void OnCollisionEnter2D(Collision2D collision)
    {
        if (killing) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            StopDetect();
        }
        base.OnCollisionEnter2D(collision);
    }
    public override void Kill(Transform Destination)
    {
        killing = true;
        Follow = false;
        StopCoroutine("TravelPoint");
        LeanTween.cancel(gameObject);
        var colliders = GetComponents<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = false;
        Debug.LogWarning("Kill" + Destination.gameObject.name);
        Target = null;
        animator.SetTrigger(BossBase.BossKillProperties);
        Vector3 dir = Destination.position - transform.position;
        if (dir.x > 0)
        {
            Destination.position = transform.position + Vector3.right * Range + Vector3.up * 0.25f;
            Destination.transform.GetChild(0).localScale = StaticData.ScaleInverse * 0.7f;
            animator.transform.localScale = Vector3.one * 0.7f;
        }
        else
        {
            Destination.position = transform.position - Vector3.right * Range - Vector3.up * 0.25f;
            Destination.transform.GetChild(0).localScale = Vector3.one * 0.7f;
            animator.transform.localScale = StaticData.ScaleInverse * 0.7f;
        }
        Invoke("BossReturn", 2f);
        ContainAssistant.Instance.PlayEffectDeath(Destination, Destination.position + Vector3.right * Mathf.Sign(dir.x) + Vector3.up);
        ContainAssistant.Instance.GetItem("3D_Hit_05", Vector3.right * Range + transform.position);
    }

}

