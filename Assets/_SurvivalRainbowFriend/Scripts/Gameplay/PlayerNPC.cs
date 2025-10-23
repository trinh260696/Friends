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
    }
    public override void Update()
    {
       
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
        animator.SetBool(ReturnProperties, run && box);
        animator.SetBool(RunBoosterProperties, run0);
        animator.SetBool(RunBoosterProperties, run0 && box);      
        animator.SetBool(DieProperties, die);
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

        animator.SetTrigger("HitTrigger1");
        var colliders = GetComponents<Collider2D>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        Invoke("DestroyGameObject", 5f);
    }

    public override void HideAndSneek(bool isTransparent)
    {
        base.HideAndSneek(isTransparent);
    }

    public override void ThrowFood(Vector3 npcPos, Vector3 enemyPos)
    {
        base.ThrowFood(npcPos, enemyPos);
    }


    public void RecoverFriend()
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
    // public void PlayLoseEmotion()
    //{
    //    int rnd = UnityEngine.Random.Range(0, 2);
    //    string anim_name = rnd == 0 ? StaticParam.cry2_emo : StaticParam.cry_emo;
    //    enum_emo(anim_name);
    //}
    //public void PlayWinEmotion()
    //{
    //    int rnd = UnityEngine.Random.Range(0, 2);
    //    string anim_name = rnd == 0 ? StaticParam.smile_emo : StaticParam.poke_emo;
    //    enum_emo(anim_name);
    //}
    //public void PlayEmotionTimeout()
    //{
    //    string[] arr = new string[] { StaticParam.tired_emo, StaticParam.hurry_up, StaticParam.scare_emo, StaticParam.run_emo };
    //    int rnd = UnityEngine.Random.Range(0, 4);
    //    enum_emo(arr[rnd]);
    //}
   
    //public void PlayEmotionGetItem()
    //{
    //    string[] emotions = { StaticParam.here_emo, StaticParam.smile_emo, StaticParam.poke_emo };
    //    int rnd = UnityEngine.Random.Range(0, emotions.Length);
    //    enum_emo(emotions[rnd]);
    //}

    //void enum_emo(string anim_name)
    //{
    //    int rnd = UnityEngine.Random.Range(0, 10);
    //    if (rnd > 3) return;
    //    emoji.SetActive(true);
    //    Invoke("hide_emo", 2f);
    //}

    //void hide_emo()
    //{
    //    emoji.SetActive(false);
    //}
    // public void PlayRndBeginGame()
    //{
    //    string[] arr = new string[] { StaticParam.poke_emo, StaticParam.emo2, StaticParam.tamgiac_emo_begin, StaticParam.tamgiac_emo_loop, StaticParam.smile_emo };
    //    int rnd = UnityEngine.Random.Range(0, 5);
    //    enum_emo(arr[rnd]);
    //}
    //public void PlayEmoRun()
    //{
    //    string[] arr = new string[] { StaticParam.run_emo, StaticParam.hurry_up, StaticParam.tamgiac_emo_begin, StaticParam.tamgiac_emo_loop, StaticParam.smile_emo };
    //    int rnd = UnityEngine.Random.Range(0, 5);
    //    enum_emo(arr[rnd]);
    //}


}
