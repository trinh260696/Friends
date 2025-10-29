using UnityEngine;

public enum FoodType
{
    BEAN = 0,
    CHICKEN_LEG = 1,
    BANANA = 2
}

public class Food : MonoBehaviour
{
    public FoodType foodType = FoodType.BEAN;
    public float attractionRadius = 15f;
    public float attractionForce = 3f;
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    private bool isConsumed = false;
    private float lifeDuration = 30f;
    private float lifeTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();

        //if (rb == null)
        //{
        //    rb = gameObject.AddComponent<Rigidbody2D>();
        //    rb.gravityScale = 0;
        //}

        if (circleCollider == null)
        {
            circleCollider = gameObject.AddComponent<CircleCollider2D>();
            circleCollider.radius = 0.5f;
        }
    }

    private void Update()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeDuration && !isConsumed)
        {
            Destroy(gameObject);
        }
    }

    public void Consume()
    {
        if (isConsumed) return;
        isConsumed = true;
        BaitManager.Instance.UnregisterFood(this);
        Destroy(gameObject);
    }

    public bool IsFoodType(FoodType type)
    {
        return foodType == type;
    }

    public float GetAttractionRadius()
    {
        return attractionRadius;
    }

    //public void OnTriggerEnter2D(Collider2D collision)
    //{
    //    BossBase boss = collision.GetComponent<BossBase>();
    //    if (boss != null && !isConsumed)
    //    {
    //        boss.EatFood();
    //        Consume();
    //    }
    //}
}
