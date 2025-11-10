using Spine.Unity;
using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.Diagnostics;
using VKSdk.Notify;

public class PlayerNPC : NPC
{
    // public StateFriend State { get => state; set => state = value; }
    public bool isAttacking = false;
    public Player player;
    public BossBase Enemy;
    public const string WhisperProperties = "WhispTrigger";
    public const string PushProperties = "Push";
    public const string GayTrigger = "GayTrigger";
    public const string TurtleProperties = "Turtle";
    public void Init(string skinName)
    {
        skeletonMecanim = animator.GetComponent<SkeletonMecanim>();
        int playerID = PlayerPrefs.GetInt("SelectedSkinID", 0);       
        skeletonMecanim.Skeleton.SetSkin(skinName);
        skeletonMecanim.Skeleton.SetToSetupPose(); // Đặt về tư thế ban đầu nếu cần
        skeletonMecanim.LateUpdate();
        state = StateFriend.FRIEND_INIT;
        box_name = BoxItemData.Instance.userSkinBoxData.currentBox.BoxObject.nameVariable;
        player = GetComponent<Player>();
        decoyAnimator=decoyTransform.GetComponent<Animator>();
    }
    public override void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
    public override void Update()
    {
        SetKeyAnimation();
        if (Input.GetKeyDown(KeyCode.Space)) {
            if(Enemy!=null)
            ThrowFood(transform.position,Enemy);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (Enemy != null)
            {
                if (Enemy.State == EnemyState.STUN_STATE) return;
                ThrowFood(transform.position, Enemy);
                // isAttacking = true;
                // animator.SetTrigger(PlayerNPC.GayTrigger);
                // Invoke(nameof(AttackBoss), 0.5f);
            }
        }
        //if (isAttacking && Enemy)
        //{
        //    if (Vector2.Distance(transform.position, Enemy.transform.position) < 5f)
        //    {
        //        Vector3 dir = (Enemy.transform.position - transform.position).normalized;
        //        float sign = dir.x;
               
        //        Vector3 targetPos = Enemy.transform.position - Vector3.right*sign * 1f;
        //        transform.position= targetPos;
        //        animator.transform.localScale = dir.x > 0 ? Vector3.one * 0.7f : StaticData.ScaleInverse * 0.7f;
        //    }
            
        //}
    }
    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.15f;
    void AttackBoss()
    {
        Enemy.AttackedBoss();
        isAttacking = false;
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D (collision);
        if (state == StateFriend.FRIEND_DIE) return;
        if (collision.CompareTag("Enemy") && !isAttacking)
        {
            this.Enemy = collision.GetComponent<BossBase>();
        }
        if (state==StateFriend.FRIEND_GO_TARGET) return;
        if (state==StateFriend.FRIEND_GO_MAIN)
        {
            if (collision.CompareTag("EnBul"))
            {
                if (state == StateFriend.FRIEND_GO_MAIN && box)
                {
                    state = StateFriend.FRIEND_SORTING_FOOD;
                    FieldAssistant.main.FlashSlotsYellow();
                    VKNotifyController.Instance.AddNotify(" Touch to a slot matching your piece to sort exactly!", VKNotifyController.TypeNotify.Normal);
                }
            }
            return;
        }
        if (collision.CompareTag("Box"))
        {
            BodyPart bodyPart = collision.GetComponent<BodyPart>();
            if (bodyPart != null && bodyPart.Free)
            {
                this.bodyPart = bodyPart;
                bodyPart.Hide();
                box = true;
                state = StateFriend.FRIEND_GO_MAIN;
                PlayEmotionGetItem();
                frameBox.gameObject.SetActive(true);
               
                bodyPart.transform.SetParent(frameBox.transform);
                bodyPart.transform.localPosition = Vector3.zero;
                if (hide)
                {
                    frameBox.gameObject.SetActive(false);
                }
            }
        }else if (collision.CompareTag("Enemy"))
        {
            Enemy = collision.GetComponent<BossBase>();
        }
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
       
        if (collision.CompareTag("EnBul")|| collision.CompareTag("Finish"))
        {
            if (box)
            {
                state = StateFriend.FRIEND_SORTING_FOOD;
            }
        }
    }
    public override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
    }
    
    private void SetKeyAnimation()
    {       
        run0 = run & boosterRun;
        animator.SetBool(RunProperties, run);
        animator.SetBool(ReturnProperties,  box);
       
        animator.SetBool(RunBoosterProperties, run0);
            
       
    }
    public override void Death()
    {
        
        state = StateFriend.FRIEND_DIE;
        // StopCoroutine("DetectGift");
        // StopCoroutine("DetectPathReturn");
        //LeanTween.cancel(gameObject);

        if (box && bodyPart != null)
        {
            BodyPart bp = bodyPart as BodyPart;
            if (bp != null)
            {
                bp.ReActive();
            }
        }

        animator.SetTrigger(DieTrigger);
        var colliders = GetComponents<Collider2D>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }
        DestroyGameObject();
    }
    public void ActiveDecoy()
    {
        isDecoy = !isDecoy;
       
        if (decoyAnimator != null)
        {
            decoyAnimator.gameObject.SetActive(isDecoy);
        }
        HideAndSneek(!isDecoy);
    }

    public override void HideAndSneek(bool isTransparent)
    {
        base.HideAndSneek(isTransparent);
    }

    public override void ThrowFood(Vector3 npcPos,BossBase enemy)
    {
        base.ThrowFood(npcPos, enemy);
    }


    public override void RecoverFriend()
    {
        HP=Player.HP;
        foreach (var param in animator.parameters)
        {
            animator.ResetTrigger(param.name);
        }
        animator.SetTrigger(NPC.ReviveTrigger);
        if(previousState==StateFriend.FRIEND_SORTING_FOOD)
        {
            state = StateFriend.FRIEND_SORTING_FOOD;
            frameBox.gameObject.SetActive(true);
        }
       else
        if (box)
        {
            frameBox.gameObject.SetActive(true);
            state=StateFriend.FRIEND_GO_MAIN;
        }else
        {
            state = StateFriend.FRIEND_PATROL;
        }
    }
    public void Balance()
    {
        if (isDecoy)
        {
            gameObject.tag = "Hide";
            decoyAnimator.gameObject.SetActive(true);
        }         
        else
        {
            gameObject.tag = "Player";
            decoyAnimator.gameObject.SetActive(false);
        }   
        die = false;
    }
    void DestroyGameObject()
    {
        player.ProccessDie("");
    }
    public void UserComeback()
    {
        state= StateFriend.FRIEND_PATROL;
        box = false;
    }

}
