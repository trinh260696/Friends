using UnityEngine;

public class DogBoss : BossBase
{
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
    }

    public override void OnInit()
    {
        base.OnInit();
    }

    public override void OnTriggerEnter2D(Collider2D col)
    {
        base.OnTriggerEnter2D(col);
    }

    public override void OnTriggerStay2D(Collider2D col)
    {
        base.OnTriggerStay2D(col);
    }

    public override void Update()
    {
        base.Update();
    }
}
