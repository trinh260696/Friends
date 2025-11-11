using UnityEngine;

public class TurtleItem : MonoBehaviour
{
    public Animator animator;
    private CircleCollider2D circleCollider2D;
    [HideInInspector]
    public PlayerNPC playerNPC;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
    }

    public void ActivateTurtle(PlayerNPC playerNPC,Vector3 position)
    {
        animator.SetTrigger("StartTrigger");
        circleCollider2D.enabled = false;
        this.playerNPC = playerNPC;
        transform.position = position;
        transform.SetParent(playerNPC.transform);
        //LeanTween.move(gameObject, position, 0.5f).setOnComplete(() =>
        //{

        //});
    }
    private void Update()
    {
        if (playerNPC == null) return;
        animator.SetBool("run", playerNPC.run);
    }
    public void DeactivateTurtle()
    {
        animator.SetTrigger("EndTrigger");       
        Invoke(nameof(Clear), 1f);
    }
    void Clear()
    {
       Destroy(gameObject);
    }
}
