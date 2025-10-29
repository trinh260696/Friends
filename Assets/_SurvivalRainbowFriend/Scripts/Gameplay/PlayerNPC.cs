using Spine.Unity;
using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Diagnostics;

public class PlayerNPC : NPC
{
    public StateFriend State { get => state; set => state = value; }
    public Player player;
   
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
    public override void Update()
    {
        SetKeyAnimation();
        if (Input.GetKeyDown(KeyCode.Space)) {
            ThrowFood(transform.position, (Vector2)transform.position+UnityEngine.Random.insideUnitCircle*3f);
        }
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (state == StateFriend.FRIEND_DIE) return;

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
        }else if(collision.CompareTag("Finish"))
        {
            if(state == StateFriend.FRIEND_GO_MAIN && box)
            {
                state = StateFriend.FRIEND_SORTING_FOOD;                
            }
        }
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

        Invoke("DestroyGameObject", 5f);
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

    public override void ThrowFood(Vector3 npcPos, Vector3 enemyPos)
    {
        base.ThrowFood(npcPos, enemyPos);
    }


    public override void RecoverFriend()
    {
        foreach (var param in animator.parameters)
        {
            animator.ResetTrigger(param.name);
        }
        animator.SetTrigger("ReviveTrigger");
       
        if (box && !isDecoy)
        {
            frameBox.gameObject.SetActive(true);
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
   

}
