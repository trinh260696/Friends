using UnityEngine;

public class FriendNPC : NPC
{
    public override void Death()
    {
        base.Death();
    }

    public override void DeathGreen(Vector3 pos)
    {
        base.DeathGreen(pos);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void HideAndSneek()
    {
        base.HideAndSneek();
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    public override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
    }
}
