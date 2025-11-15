using UnityEngine;

public class TrapItem : MonoBehaviour
{
    public Animator animator;
    public float deltaTime=0.5f;
    private Collider2D trapCollider;
    public const string ActivateTrigger = "ActiveTrigger";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trapCollider = GetComponent<Collider2D>();
        Destroy(gameObject, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            animator.SetTrigger(ActivateTrigger);

            BossBase bossBase = collision.gameObject.GetComponent<BossBase>();
            if (bossBase != null)
            {
                bossBase.Body.linearVelocity = Vector2.zero;
                trapCollider.enabled = false;
                StartCoroutine(CrashBossDelay(bossBase, deltaTime));
            }
        }
    }

    System.Collections.IEnumerator CrashBossDelay(BossBase bossBase, float delay)
    {
        yield return new WaitForSeconds(delay);
        bossBase.CrashedBoss();
    }
}
