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
    [HideInInspector]
    public TurtleItem turtleItem = null;
    public bool isWhisper = false;
    public const string WhisperProperties = "Whisper";
    public const string WhisperTrigger="WhispTrigger";
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
        if (turtleItem && collision.collider.CompareTag("Enemy"))
        {
            collision.collider.GetComponent<BossBase>().CrashedBoss();
            GetComponent<CapsuleCollider2D>().enabled = false;
            Invoke(nameof(HandleCollsionEnemy), 0.2f);
        }
    }
    void HandleCollsionEnemy()
    {
        GetComponent<CapsuleCollider2D>().enabled = true;
    }
    public override void Update()
    {
        SetKeyAnimation();
        if (Input.GetKeyDown(KeyCode.Space)) {
          
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (Enemy != null)
            {
                if (Enemy.State == EnemyState.STUN_STATE) return;
                ThrowFood(transform.position, Enemy);
                
            }
        }
     
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
    void CancelTurtle()
    {
        boosterRun = false;
        animator.transform.localPosition=Vector3.zero;
        turtleItem.DeactivateTurtle();
        gameObject.tag = "Player";
        this.turtleItem = null;
    }
    void CancelWhisper()
    {
        isWhisper = false;

    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D (collision);
        if (state == StateFriend.FRIEND_DIE) return;
        
        if(collision.CompareTag("Enemy"))
        {
            dangerous = true;
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
            if (box) return;
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
        }else if (collision.CompareTag("Enemy") && isWhisper)
        {
            Enemy = collision.GetComponent<BossBase>();
            Enemy.AttractiveBoss();
        }
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        float distance;
        float angle;
        Vector2 dir;
        if (collision.CompareTag("EnBul")|| collision.CompareTag("Finish"))
        {
            if (box)
            {
                state = StateFriend.FRIEND_SORTING_FOOD;
            }
        }
        else if(collision.CompareTag("Turtle") && this.turtleItem == null) {
            

            LeanTween.moveLocalY(animator.gameObject, 0.8f, 0.5f);
            boosterRun = true;
            gameObject.tag = "Hide";
            this.turtleItem = collision.GetComponent<TurtleItem>();
            turtleItem.ActivateTurtle(this, transform.position - Vector3.up * 0.22f);
            Invoke(nameof(CancelTurtle), 30f);
        }
        else if (collision.CompareTag("Enemy") && turtleItem != null)
        {
            dir = (collision.transform.position - transform.position).normalized;
            angle = Vector2.SignedAngle(dir, Vector2.up);
            distance = Vector2.Distance(collision.transform.position, transform.position);
            if (distance > 2f) return;
            //if (angle < -135 || angle > 135) return;
            //else if (angle > -45 && angle < 45) return;
            this.Enemy = collision.GetComponent<BossBase>();
            this.Enemy.CrashedBoss();
        }else if (collision.CompareTag("Whisper") && !isWhisper)
        {
            dir = (collision.transform.position - transform.position).normalized;
            angle = Vector2.SignedAngle(dir, Vector2.up);
            distance = Vector2.Distance(collision.transform.position, transform.position);
            if (distance > 2f) return;
           isWhisper=true;
           collision.GetComponent<Whisper>().DeactivateWhisper();
           animator.SetTrigger(WhisperTrigger);
           Invoke(nameof(TurnOffWhisper), 20f);
        }else if (collision.CompareTag("Enemy") && isWhisper)
        {
            Enemy = collision.GetComponent<BossBase>();
            Enemy.AttractiveBoss();
        }else if (collision.CompareTag("Enemy") && !isWhisper && !turtleItem)
        {
           dangerous = true;
        }
    }
    void TurnOffWhisper()
    {
        isWhisper = false;
    }
    public override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        if (collision.CompareTag("Enemy"))
        {
            dangerous = false;
        }
    }
    
    private void SetKeyAnimation()
    {       
        run0 = run & boosterRun;
        animator.SetBool(RunProperties, run);
        animator.SetBool(ReturnProperties,  box);
        animator.SetBool(WhisperProperties, isWhisper);
        animator.SetBool(RunBoosterProperties, run0);
        animator.SetBool(ScareProperties, dangerous);

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
