using UnityEngine;
using System.Collections.Generic;

public class BaitManager : MonoBehaviour
{
    public static BaitManager Instance;
    private List<Food> activeFood = new List<Food>();
    private const float maxDetectionRadius = 20f;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterFood(Food food)
    {
        if (!activeFood.Contains(food))
        {
            activeFood.Add(food);
        }
    }

    public void UnregisterFood(Food food)
    {
        activeFood.Remove(food);
    }

    public Food FindNearestFoodForEnemy(BossBase enemy)
    {
        if (activeFood.Count == 0) return null;

        Food nearestFood = null;
        float closestDistance = maxDetectionRadius;

        foreach (Food food in activeFood)
        {
            if (food == null || food.gameObject == null) continue;

            float distance = Vector2.Distance(enemy.transform.position, food.transform.position);

            if (distance < food.GetAttractionRadius() && distance < closestDistance)
            {
                if (IsEnemyAttractedToFood(enemy, food))
                {
                    closestDistance = distance;
                    nearestFood = food;
                }
            }
        }

        return nearestFood;
    }

    public bool IsEnemyAttractedToFood(BossBase enemy, Food food)
    {
        switch (enemy.IQ)
        {
            case 1:
            case 2:
                return false;
            case 3:
                return IsFoodTypeAttractiveForIQ3(food);
            case 4:
                return IsChainAttraction(enemy, food);
            case 5:
                return IsChainAttraction(enemy, food);
            case 6:
            case 7:
                return true;
            default:
                return false;
        }
    }

    private bool IsFoodTypeAttractiveForIQ3(Food food)
    {
        return food.IsFoodType(FoodType.BEAN) || 
               food.IsFoodType(FoodType.CHICKEN_LEG) || 
               food.IsFoodType(FoodType.BANANA);
    }

    private bool IsChainAttraction(BossBase enemy, Food food)
    {
        if (enemy.IQ >= 4)
        {
            float distance = Vector2.Distance(enemy.transform.position, food.transform.position);
            return distance < food.GetAttractionRadius() * 0.7f;
        }
        return false;
    }

    public void MoveEnemyTowardFood(BossBase enemy, Food food)
    {
        if (food == null || enemy == null) return;

        Vector2 direction = (food.transform.position - enemy.transform.position).normalized;
        float distance = Vector2.Distance(enemy.transform.position, food.transform.position);
        if (distance < 0.5f)
        {
            enemy.EatFood();
            food.Consume();
            return;
        }
        enemy.Target = food.transform;
        enemy.Body.linearVelocity = direction * food.attractionForce;
        enemy.animator.transform.localScale = direction.x > 0 ? Vector3.one  : new Vector3(-1, 1, 1f);
        
    }

    public Food GetRandomFood()
    {
        if (activeFood.Count == 0) return null;
        return activeFood[Random.Range(0, activeFood.Count)];
    }

    public List<Food> GetAllNearbyFood(Vector3 position, float radius)
    {
        List<Food> nearbyFood = new List<Food>();
        foreach (Food food in activeFood)
        {
            if (food != null && Vector2.Distance(position, food.transform.position) <= radius)
            {
                nearbyFood.Add(food);
            }
        }
        return nearbyFood;
    }

    internal Food SpawnFood(Vector3 foodSpawnPos, Vector3 throwDirection)
    {
        Food food = ContentAssistant.Instance.GetItem<Food>("Food", foodSpawnPos);
        RegisterFood(food);
        return food;
    }
}
