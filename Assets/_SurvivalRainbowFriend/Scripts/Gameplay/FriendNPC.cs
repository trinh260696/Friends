using UnityEngine;
using System.Collections.Generic;

public class FriendNPC : NPC
{
    private List<BossBase> detectedEnemies = new List<BossBase>();
    
    private float decoyDuration = 5f;
    private float decoyTimer = 0f;
    private Vector2 escapeDirection = Vector2.zero;
    private int foodCount = 3;
    
    // Variables for moving to box
    private BodyPart targetBodyPart = null;
    private Vector2 targetBoxPosition = Vector2.zero;
    private float boxReachDistance = 0.3f;
    private float boxMoveSpeed = 3f;

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (state == StateFriend.FRIEND_GO_MAIN) return;
        if(state==StateFriend.FRIEND_DIE) return;
        if (box) return;
        if (collision.CompareTag("Box") && state == StateFriend.FRIEND_PATROL)
        {
            BodyPart bodyPart = collision.GetComponent<BodyPart>();
            if (bodyPart != null && bodyPart.Free)
            {
                // Set target and change state to move towards box
                targetBodyPart = bodyPart;
                targetBoxPosition = (Vector2)frameBox.transform.position;
                state = StateFriend.FRIEND_GO_TARGET;
                
                // Hide collider of bodyPart
                Collider2D bodyPartCollider = bodyPart.GetComponent<Collider2D>();
                if (bodyPartCollider != null)
                {
                    bodyPartCollider.enabled = false;
                }
            }
        }

        if (collision.CompareTag("Enemy"))
        {
            BossBase enemy = collision.GetComponent<BossBase>();
            if (enemy != null && !detectedEnemies.Contains(enemy))
            {
                HandleEnemyEncounter(enemy);
            }
        }
    }
    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
       
        if (collision.collider.CompareTag("Wall"))
        {
            if (State == StateFriend.FRIEND_PATROL || State == StateFriend.FRIEND_GO_MAIN || State== StateFriend.FRIEND_CHASED)
            {
                GetComponent<Collider2D>().isTrigger = true;
                // Xử lý phản xạ khi va chạm với tường
                Invoke(nameof(HandleCollisionWall), 0.3f);
            }


        }
    }

    public override void Update()
    {
        // Handle moving to box pickup
        if (state == StateFriend.FRIEND_GO_TARGET)
        {
            HandleMoveToBoxPickup();
            return; // Don't process other logic while moving to box
        }
        
        base.Update();
    }

    private void HandleMoveToBoxPickup()
    {
        if (targetBodyPart == null)
        {
            state = StateFriend.FRIEND_PATROL;
            return;
        }
        
        Vector2 currentPos = (Vector2)transform.position;
        float distance = Vector2.Distance(currentPos, targetBoxPosition);
        
        if (distance > boxReachDistance)
        {
            // Move towards box position
            Vector2 direction = (targetBoxPosition - currentPos).normalized;
            transform.position = Vector2.MoveTowards(currentPos, targetBoxPosition, boxMoveSpeed * Time.deltaTime);
            
            // Update animation direction
            animator.transform.localScale = direction.x > 0 ? Vector3.one * 0.7f : StaticData.ScaleInverse * 0.7f;
            animator.SetBool("run_normal", true);
        }
        else
        {
            // Reached box, execute pickup logic
            this.bodyPart = targetBodyPart;
            
            // Move bodyPart to spriteRendererBox transform position
            if (frameBox != null)
            {
                targetBodyPart.transform.SetParent(frameBox.transform);
                targetBodyPart.transform.position = frameBox.transform.position;
            }
            
            targetBodyPart.Free = false;
            box = true;
            animator.SetBool("run_normal", false);
            state = StateFriend.FRIEND_GO_MAIN;
            PlayEmotionGetItem();
            ReturnToBeginPoint();
            
            // Clear target
            targetBodyPart = null;
        }
    }

    public  void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            BossBase enemy = collision.GetComponent<BossBase>();
            if (enemy != null && detectedEnemies.Contains(enemy))
            {
                detectedEnemies.Remove(enemy);
            }
        }
    }
    private bool DetectionWall=false;
  
    public void HandleCollisionWall()
    {
        GetComponent<Collider2D>().isTrigger=false;
    }

    private void HandleEnemyEncounter(BossBase enemy)
    {
        bool isMultiEnemy = HandleMultipleEnemies();
        detectedEnemies.Add(enemy);
        if (state == StateFriend.FRIEND_CHASED) return;       
       
        switch (IQ)
        {
            case 1:
                HandleIQ1Behavior(enemy, isMultiEnemy);
                break;
            case 2:
                HandleIQ2Behavior(enemy, isMultiEnemy);
                break;
            case 3:
                HandleIQ3Behavior(enemy, isMultiEnemy);
                break;
            case 4:
                HandleIQ4Behavior(enemy, isMultiEnemy);
                break;
            case 5:
                HandleIQ5Behavior(enemy, isMultiEnemy);
                break;
            case 6:
                HandleIQ6Behavior(enemy, isMultiEnemy);
                break;
            case 7:
                HandleIQ7Behavior(enemy, isMultiEnemy);
                break;
        }
    }

    private void HandleIQ1Behavior(BossBase enemy, bool isMultiEnemy)
    {
        if (enemy.State == EnemyState.PATROL_STATE)
        {
            Vector2 oppositeDir = -(enemy.transform.position - transform.position).normalized;
            if (isMultiEnemy)
            {
                oppositeDir=escapeDirection;
            }
            MoveTo((Vector2)transform.position + oppositeDir * 10f);
            
           
        }
    }

    private void HandleIQ2Behavior(BossBase enemy, bool isMultiEnemy)
    {
        if (enemy.State == EnemyState.PATROL_STATE)
        {
            ActivateDecoyByTime(5f);
        }
    }

    private void HandleIQ3Behavior(BossBase enemy, bool isMultiEnemy)
    {
        ActivateDecoyByTime(float.MaxValue);
    }

    private void HandleIQ4Behavior(BossBase enemy, bool isMultiEnemy)
    {
        if (enemy.State == EnemyState.PATROL_STATE)
        {
            ActivateDecoyByEnemy(enemy);
        }
    }

    private void HandleIQ5Behavior(BossBase enemy, bool isMultiEnemy)
    {
        if (enemy.State == EnemyState.PATROL_STATE)
        {
            int enemyIQ = enemy.IQ;

            if ((enemyIQ == 5 || enemyIQ == 4 || enemyIQ == 3) && enemy.State != EnemyState.STUN_STATE)
            {
                Vector2 escapeDir = -(enemy.transform.position - transform.position).normalized;
                  if (isMultiEnemy)
                {
                    escapeDir=escapeDirection;
                }
                MoveTo((Vector2)transform.position + escapeDir * 15f);
                
            }
            else if (enemyIQ == 1 || enemyIQ == 2)
            {
                ActivateDecoyByEnemy(enemy);
            }
        }
    }

    private void HandleIQ6Behavior(BossBase enemy, bool isMultiEnemy)
    {
        if (enemy.State == EnemyState.PATROL_STATE)
        {
            if (foodCount > 0)
            {
                ThrowFood(transform.position, enemy.transform.position);
                foodCount--;
            }
            else
            {
                HandleIQ5Behavior(enemy, isMultiEnemy);
            }
        }
    }

    private void HandleIQ7Behavior(BossBase enemy, bool isMultiEnemy)
    {
        if (enemy.State == EnemyState.PATROL_STATE || enemy.State == EnemyState.CHASE_STATE || enemy.State == EnemyState.FIGHT_STATE)
        {
            if (foodCount > 0)
            {
                ThrowFood(transform.position, enemy.transform.position);
                foodCount--;
            }
            else if (enemy.State == EnemyState.PATROL_STATE)
            {
                HandleIQ5Behavior(enemy, isMultiEnemy);
            }
        }
    }

    private bool HandleMultipleEnemies()
    {
        if (detectedEnemies.Count <= 1) return false;

        Vector2 bisector = Vector2.zero;

        for (int i = 0; i < detectedEnemies.Count; i++)
        {
            Vector2 dirToEnemy = (detectedEnemies[i].transform.position - transform.position).normalized;

            if (i == 0)
            {
                bisector = dirToEnemy;
            }
            else
            {
                bisector = (bisector + dirToEnemy).normalized;
            }
        }

        escapeDirection = -bisector;
        return true;
    }

    private void ActivateDecoyByTime(float duration)
    {
        isDecoy = true;
        decoyDuration = duration;
        decoyTimer = 0f;
        if (decoyAnimator != null)
        {
            decoyAnimator.gameObject.SetActive(true);
        }
        HideAndSneek(false);
        if(duration< float.MaxValue)
        {
            Invoke("DeactivateDecoy", duration);
        }
        else
        {
            InvokeRepeating("DeactivateOnCall", 0.1f, 0.11f);
        }
    }
    private void ActivateDecoyByEnemy(BossBase enemy)
    {
        isDecoy = true;
        
        
        if (decoyAnimator != null)
        {
            decoyAnimator.gameObject.SetActive(true);
        }
        HideAndSneek(false);
        InvokeRepeating("DeactivateOnCall", 0.1f, 0.11f);
    }
    private void DeactivateOnCall()
    {
        if(detectedEnemies.Count==0)
        {
            DeactivateDecoy();
            CancelInvoke("DeactivateOnCall");
        }
    }
    private void DeactivateDecoy()
    {
        isDecoy = false;
        if (decoyAnimator != null)
        {
            decoyAnimator.gameObject.SetActive(false);
        }
        HideAndSneek(true);
    }

    public override void ThrowFood(Vector3 npcPos, Vector3 enemyPos)
    {
        base.ThrowFood(npcPos, enemyPos);
    }


    public override void Death()
    {
        base.Death();
  
        targetBodyPart = null;
        Invoke("DestroyGameObject", 1f);
    }
    public override void Deal(int damage)
    {
        base.Deal(damage);
    }
    public override void HideAndSneek(bool isTransparent)
    {
       base.HideAndSneek(isTransparent);
    }

    public override void RecoverFriend()
    {
        animator.SetTrigger("ReviveTrigger");
        
        // Cancel box pickup if in progress
        targetBodyPart = null;

        if (box)
        {
            hide = false;
            state = StateFriend.FRIEND_GO_MAIN;
            frameBox.gameObject.SetActive(true);
            ReturnToBeginPoint();
        }
        else
        {
            frameBox.gameObject.SetActive(false);
            state = StateFriend.FRIEND_PATROL;
            FriendStartGame();
        }

        detectedEnemies.Clear();
        DeactivateDecoy();
        foodCount = 3;
    }

    void DestroyGameObject()
    {
        gameObject.SetActive(false);
    }

    void PlayEmotionGetItem()
    {
        string[] emotions = { StaticParam.here_emo, StaticParam.smile_emo, StaticParam.poke_emo };
        int rnd = UnityEngine.Random.Range(0, emotions.Length);
        enum_emo(emotions[rnd]);
    }

    void enum_emo(string anim_name)
    {
        int rnd = UnityEngine.Random.Range(0, 10);
        if (rnd > 3) return;
        emoji.SetActive(true);
        Invoke("hide_emo", 2f);
    }

    void hide_emo()
    {
        emoji.SetActive(false);
    }
}
