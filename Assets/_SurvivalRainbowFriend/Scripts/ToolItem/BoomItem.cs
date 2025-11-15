using UnityEngine;

public class BoomItem : MonoBehaviour
{
    public Animator animator;
    public float time_out = 5f;
    public float explosion_radius = 5f;
    public float explosion_force = 100f;

    private float timer;
    private bool has_exploded = false;

    void Start()
    {
        timer = time_out;
    }

    void Update()
    {
        if (has_exploded) return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Explode();
        }
    }

    void Explode()
    {
        has_exploded = true;
        animator.SetTrigger("Explode");

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosion_radius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 direction = (collider.transform.position - transform.position).normalized;
                    rb.linearVelocity = direction * explosion_force;
                }
            }
        }
    }
}
