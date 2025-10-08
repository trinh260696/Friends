using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
public class BossBase : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D Body;//Enemy body      
    public Transform Target;//Player transform
    public SkeletonAnimation skeletonAnimation;
    private bool _follow;
    public bool Follow
    {
        get => _follow; set
        {
            _follow = value;
            deltaSpeed = 0;
        }
    }
    //If the enemy has ceased to see the player, he will follow on PlayerLastPos
    public float Range;
    public float Speed=5.0f;     
    //blood sprites (GameObjects)
   
   
    public bool isChase = false;
    public bool isMoveWayPoint = false;
    public GameObject BloodParticle;//for all bullets

    private RaycastHit2D hit;//for the visibility system     

    public bool killing = false;
    private List<Vector2> Waypoints;
    private List<Vector2> DetectPoints;
    private const string BossRunProperties = "bossrun";
    private const string BossChaseProperties = "bosschase";
    public const string BossKillProperties = "bosskill";
    private const string BossDamageProperties = "bosshit";
    private const string BossDieProperties = "bossdie";
    private bool run = false;
       
    public void SetKeyAnimation()
    {
        animator.SetBool(BossBase.BossRunProperties, run);
        animator.SetBool(BossBase.BossChaseProperties, Follow);
    }
    int currentIndex;
    private void Awake()
    {
        NotificationCenter.DefaultCenter().AddObserver(this, "BossStartGame");
        NotificationCenter.DefaultCenter().AddObserver(this, "OnFinish");
        DetectPoints = new List<Vector2>();
        OnFinish();
        if (skeletonAnimation != null)
        {
            skeletonAnimation.gameObject.SetActive(false);
        }      
    }
    
    public virtual void OnInit()
    {
        if (isMoveWayPoint)
        {
            StartDetect();
        }
    }
    public void BossStartGame()
    {
        OnInit();
    }
    public void PopulatePos(Notification receivedData)
    {
        var pos = GameManager.Instance.GetBossPos();
        transform.position = pos;
    }
    public void StartDetect()
    {
        if (isMoveWayPoint)
        {
            SetupPointDetect();
            if (DetectPoints.Count > 0)
            {
                run = true;
                StartCoroutine("TravelPoint");
            }             
            else
            {
                run = false;
            }              
        }
    }
    void SetupPointDetect()
    {
        int S, F;
        Waypoints = GameManager.Instance.GetBossPath(transform, out S, out F);
        FieldMini field = new FieldMini();
        field.WIDTH = Waypoints.Count;
        field.joints = new float[field.WIDTH, field.WIDTH];
        for (int i = 0; i < Waypoints.Count; i++)
            for (int j = i + 1; j < Waypoints.Count; j++)
            {
                float distance = Vector2.Distance((Vector2)Waypoints[i], Waypoints[j]);
                var raycastHit = Physics2D.Raycast(Waypoints[i], Waypoints[j] - Waypoints[i], distance, 1<<13|1<<1);
                if (raycastHit.collider == null)
                {
                    field.joints[i, j] = Vector2.Distance(Waypoints[i], Waypoints[j]);
                }
                else
                {
                    field.joints[i, j] = DIJKTRA.MAXC;
                }
            }
        DIJKTRA.Init(field);
        DIJKTRA.InputSecond(S, F);
        var resultSecond = DIJKTRA.Output();
        if(resultSecond!=null)
        {
            DetectPoints.Clear();
            for (int i = 0; i < resultSecond.Length; i++)
            {
                DetectPoints.Add(Waypoints[resultSecond[i]]);
            }
        }
        else
        {
            DetectPoints.Clear();
        }
           
    }
    public void BossReturn()
    {
        killing = false;
        Follow = false;
        run = true;
        var colliders = GetComponents<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = true;
        BossStartGame();
    }
    public void BossEnd()
    {

    }
    public void StopDetect()
    {
        run = false;
        StopCoroutine("TravelPoint");
        LeanTween.cancel(gameObject);        
    }
    Vector3 Direction;
    public virtual void Update()
    {
        
        if (!isMoveWayPoint) return;
        SetKeyAnimation();
        if (Target == null) return;
        if (!StaticData.IsPlay) return;
        //check of reaching the last pos 
                      
        float distance = Vector2.Distance((Vector2)Target.position, (Vector2)transform.position);
        var hitH = Physics2D.Raycast((Vector2)transform.position + Vector2.up * 1.8f, (Vector2)Target.position - (Vector2)transform.position, 2f, 1 << 13);

        hit = Physics2D.Raycast((Vector2)transform.position, (Vector2)Target.position - (Vector2)transform.position, distance, 1<<13);
        if (hit.collider != null)
        {
            //ray does not touch the walls
                //follow the target
            Vector3 moveDirection = Target.transform.position - transform.position;
            if (moveDirection != Vector3.zero)
            {
                float angle = Mathf.Atan2(-moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
                //Direction = Quaternion.AngleAxis(angle + 90, Vector3.forward) * moveDirection.normalized;
                Direction = Vector2.Perpendicular(moveDirection).normalized;
                // transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
                Follow = true;

                       
            }
                //show rays:
                //Debug.DrawLine((Vector2)transform.position, (Vector2)Player.Player_Pos);
                
               
        }
        else if (_follow)
        {//follow the last target position
            Vector3 moveDirection = Target.transform.position - transform.position;
            if (moveDirection != Vector3.zero)
            {
                Direction = moveDirection.normalized;
            }
        }

        if (Vector2.Distance((Vector2)Target.position, (Vector2)transform.position) > distance + 2f)
        {
            Target = null;
            _follow = false;
            BossStartGame();
            enum_emo(StaticParam.emo2);
            return;
        }
        if (Target.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (!Target.GetComponent<Player>().IsAlivePlayer)
            {
                Target = null;
                _follow = false;
                BossStartGame();
                enum_emo(StaticParam.emo2);
            }
        }
        if(Target)
        if (!Target.gameObject.activeSelf)
        {
            Target = null;
            _follow = false;
            BossStartGame();
            enum_emo(StaticParam.emo2);
        }
    }
    float deltaSpeed = 0f;

  

    public virtual void FixedUpdate()
    {
        if (!isChase) return;
        if (!_follow) return;
        if (!Target) return;
        if (!StaticData.IsPlay) return;
        deltaSpeed += Time.deltaTime*2f;
        float realSpeed =    Speed;
        animator.transform.localScale = Direction.x > 0 ? Vector3.one * 0.7f : StaticData.ScaleInverse * 0.7f;
        Body.AddRelativeForce(Direction * (realSpeed+deltaSpeed));//the speed depends on the current hp
    }
        
    public virtual void OnTriggerEnter2D(Collider2D col)
    {
           
    }
    public virtual void OnTriggerStay2D(Collider2D col)
    {

    }
    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!StaticData.IsPlay) return;
        if (killing) return;
        if (collision.collider.CompareTag("Wall"))
        {
            var dir = (collision.collider.transform.position - transform.position).normalized;
            Body.AddForce(dir);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.LogWarning("Kill Player");
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                var player = collision.gameObject.GetComponent<Player>();

                if (!player.IsAlivePlayer) return;
                string nameBoss = "Blue";
                if(this is DreamFaceBoss)
                {
                    nameBoss = "Dreamface";
                }else if(this is OBugaBoss)
                {
                    nameBoss = "Obuga";
                }else if(this is BlueBoss)
                {
                    nameBoss = "Blue";
                }
                player.Death(nameBoss);
                Kill(collision.transform);
            }
            else
            {
                collision.gameObject.GetComponent<Friend>().Death();
                Kill(collision.transform);
            }   
        }         
    }
    public virtual void Kill(Transform Destination)
    {
        killing = true;
        _follow = false;
        StopCoroutine("TravelPoint");
        LeanTween.cancel(gameObject);
        var colliders = GetComponents<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = false;
        Debug.LogWarning("Kill"+Destination.gameObject.name);
        Target = null;
        animator.SetTrigger(BossBase.BossKillProperties);
        Vector3 dir=Destination.position-transform.position;
        if (dir.x > 0)
        {
            Destination.position = transform.position + Vector3.right * Range + Vector3.up * 0.25f;
            Destination.transform.GetChild(0).localScale = StaticData.ScaleInverse*0.7f;
            animator.transform.localScale = Vector3.one * 0.7f;
        }
        else
        {
            Destination.position = transform.position - Vector3.right * Range - Vector3.up * 0.25f;
            Destination.transform.GetChild(0).localScale = Vector3.one*0.7f;
            animator.transform.localScale = StaticData.ScaleInverse * 0.7f;
        }        
        Invoke("BossReturn", 5f);
        ContainAssistant.Instance.PlayEffectDeath(Destination, Destination.position+Vector3.right*Mathf.Sign(dir.x)+Vector3.up);
    }
    IEnumerator TravelPoint()
    {
        if(DetectPoints.Count==0)
        {
            run = false;
            yield break;

        }
        yield return null;
        Vector2 currentPos = transform.position;
        while (!_follow)
        {
            run = true;
            for(int i=0; i<DetectPoints.Count; i++)
            {
                float distance = Vector2.Distance(currentPos, DetectPoints[i]);
                Vector2 dir = DetectPoints[i] - currentPos;
                animator.transform.localScale = dir.x > 0 ? Vector3.one * 0.7f : StaticData.ScaleInverse * 0.7f;
                LeanTween.move(gameObject, DetectPoints[i], distance / Speed);
                yield return new WaitForSeconds(distance / Speed);
                if (i >= DetectPoints.Count)
                {
                    yield break;
                }
                currentPos = DetectPoints[i];                  
            }
            yield return null;
            SetupPointDetect();
            if (_follow)
            {                 
                yield break;
            }
        }
    }
    
    public void OnFinish()
    {
        run = false;
        StopCoroutine("TravelPoint");
        Target = null;
        _follow = false;
    }
    public void enum_emo(string anim_name)
    {
        if (skeletonAnimation == null) return;
        int rnd = UnityEngine.Random.Range(0, 10);
        if (rnd > 3) return;
        skeletonAnimation.gameObject.SetActive(true);
        skeletonAnimation.AnimationState.SetAnimation(0, anim_name, true);
        Invoke("hide_emo", 2f);
    }
    void hide_emo()
    {
        skeletonAnimation.gameObject.SetActive(false);
    }
}


