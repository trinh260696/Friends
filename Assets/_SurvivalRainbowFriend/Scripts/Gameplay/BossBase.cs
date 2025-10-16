using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
public class BossBase : MonoBehaviour
{
    public string nameBoss = "Boss";
    public int IQ = 1;
    public float attackSpeed = 1f;
    public int damage = 10;
    
    private EnemyState _state;
    public EnemyState State { get => _state; set => _state = value; }
    
    public Animator animator;
    public Rigidbody2D Body;//Enemy body      
    public Transform pivotTransform;
    public Transform Target;//Player transform
    public GameObject emoAnimation;
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
        OnFinishTravel();
        if (emoAnimation != null)
        {
            emoAnimation.gameObject.SetActive(false);
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
                OnTravelPoint();
                //StartCoroutine("TravelPoint");
            }             
            else
            {
                run = false;
            }
        }
        else
        {
            run = false;
            State = EnemyState.IDLE_STATE;
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
        BossStartGame();
    }
    public void BossEnd()
    {

    }
    public void StopDetect()
    {
        run = false;
        //Stop Travel Point
        
    }
    Vector3 Direction;
    public virtual void Update()
    {
        
        if (!isMoveWayPoint) return;
        SetKeyAnimation();

        if (!StaticData.IsPlay) return;
        if (Target == null) return;
        if (State == EnemyState.PATROL_STATE)
        {
            
        }else if (State == EnemyState.FIGHT_STATE)
        {

        }
        //check of reaching the last pos 

    }
    float deltaSpeed = 0f;

  

    public virtual void FixedUpdate()
    {
        if (!isChase) return;
        if (!_follow) return;
        if (!Target) return;
        if (!StaticData.IsPlay) return;
        //deltaSpeed += Time.deltaTime*2f;
        //float realSpeed =    Speed;
        //animator.transform.localScale = Direction.x > 0 ? Vector3.one * 0.7f : StaticData.ScaleInverse * 0.7f;
        //Body.AddRelativeForce(Direction * (realSpeed+deltaSpeed));//the speed depends on the current hp
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
        //if (collision.gameObject.CompareTag("Player"))
        //{
        //    Debug.LogWarning("Kill Player");
        //    State = EnemyState.FIGHT_STATE;
        //    if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        //    {
        //        var player = collision.gameObject.GetComponent<Player>();

        //        if (!player.IsAlivePlayer) return;               
        //        player.Damage(nameBoss,damage);
        //        Kill(collision.transform);
        //    }
        //    else
        //    {
        //        collision.gameObject.GetComponent<NPC>().Death();
        //        Kill(collision.transform);
        //    }   
        //}         
    }
    public virtual void Kill(Transform Destination)
    {
        killing = true;
        _follow = false;
        // Stop TravelPoint

        State = EnemyState.FIGHT_STATE;
        animator.SetTrigger(BossBase.BossKillProperties);
        Vector3 dir = Destination.position - transform.position;
             
        ContainAssistant.Instance.PlayEffectDeath(Destination, Destination.position+Vector3.right*Mathf.Sign(dir.x)+Vector3.up);
    }
    void OnTravelPoint()
    {
        State = EnemyState.PATROL_STATE;
    }
    void TravelPoint()
    {
        //if(DetectPoints.Count==0)
        //{
        //    run = false;
        //    yield break;

        //}
        //yield return null;
        //Vector2 currentPos = transform.position;
        //while (!_follow)
        //{
        //    run = true;
        //    for(int i=0; i<DetectPoints.Count; i++)
        //    {
        //        float distance = Vector2.Distance(currentPos, DetectPoints[i]);
        //        Vector2 dir = DetectPoints[i] - currentPos;
        //        animator.transform.localScale = dir.x > 0 ? Vector3.one * 0.7f : StaticData.ScaleInverse * 0.7f;
        //        LeanTween.move(gameObject, DetectPoints[i], distance / Speed);
        //        yield return new WaitForSeconds(distance / Speed);
        //        if (i >= DetectPoints.Count)
        //        {
        //            yield break;
        //        }
        //        currentPos = DetectPoints[i];                  
        //    }
        //    yield return null;
        //    SetupPointDetect();
        //    if (_follow)
        //    {                 
        //        yield break;
        //    }
        //}
    }
    
    public void OnFinishTravel()
    {
        //run = false;
        //StopCoroutine("TravelPoint");
        //Target = null;
        //_follow = false;
    }
    public void enum_emo(string anim_name)
    {
        if (emoAnimation == null) return;
        int rnd = UnityEngine.Random.Range(0, 10);
        if (rnd > 3) return;
        emoAnimation.SetActive(true);
        
        Invoke("hide_emo", 2f);
    }
    void hide_emo()
    {
        emoAnimation.SetActive(false);
    }
}


public enum EnemyState
{
    IDLE_STATE,
    PATROL_STATE,
    STUN_STATE,
    EAT_STATE,
    FIGHT_STATE,
    CHASE_STATE,
    DEAD
}